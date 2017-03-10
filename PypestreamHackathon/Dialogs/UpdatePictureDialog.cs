using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.ProjectOxford.Face;
using System.Web;
using System.Net;

namespace PypestreamHackathon.Dialogs
{
    [Serializable]
    public class UpdatePictureDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait<LuisResult>(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<LuisResult> input)
        {
            var msg = await input;

            await context.Forward(new AuthDialog(), async (IDialogContext authContext, IAwaitable<string> authResult) =>
            {
                var token = await authResult;

                // Save the token for later
                authContext.ConversationData.SetValue<string>("AccessToken", token);
                if (String.IsNullOrEmpty(token))
                {
                    authContext.Done("Error: unable to validate your identity.");
                }
                else
                {
                    // Prompt the user to upload a new photo
                    PromptDialog.Attachment(authContext, async (IDialogContext attachmentsContext, IAwaitable<IEnumerable<Attachment>> attachmentsResult) =>
                    {
                        var attachments = await attachmentsResult;
                        var attachment = attachments.FirstOrDefault();

                        // TODO: get stream of the attachment
                        var newPicContent = attachment.ContentUrl;
                        HttpClient client = new HttpClient();
                        var response = await client.GetAsync(newPicContent);
                        var newPicStream = await response.Content.ReadAsStreamAsync();
                        byte[] newBytes = new byte[newPicStream.Length];
                        newPicStream.Read(newBytes, 0, newBytes.Length);
                        newPicStream = new MemoryStream(newBytes);

                        // Get the stream of the current picture
                        var bytes = await getProfilePic(attachmentsContext.ConversationData.Get<string>("AccessToken"));
                        var oldPicStream = new MemoryStream(bytes, true);

                        double confidence = await GetScoreForTwoFaces(newPicStream, oldPicStream);
                        if (confidence >= 75)
                        {
                            //change picture
                            newPicStream = new MemoryStream(newBytes);
                            await updateProfilePic(attachmentsContext.ConversationData.Get<string>("AccessToken"), newPicStream);
                            attachmentsContext.Done($"Great! We're {confidence}% confident that the picture you uploaded is of yourself. We have updated your profile picture.");
                        }
                        else
                        {
                            attachmentsContext.Done($"Sorry, we are only {confidence}% confident that the picture you uploaded is you.");
                        }
                    }, "Please upload a new profile photo");
                }
            }, context.Activity, CancellationToken.None);
        }

        private async Task<byte[]> getProfilePic(string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            using (var response = await client.GetAsync($"https://graph.microsoft.com/v1.0/me/photo/$value"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    return bytes;
                }
                else
                    return null;
            }
        }

        private async Task updateProfilePic(string token, Stream newPhoto)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            client.DefaultRequestHeaders.Add("Accept", "application/json;odata.metadata=full");
            var fileContent = new StreamContent(newPhoto);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            using (var uploadRes = await client.PutAsync("https://graph.microsoft.com/v1.0/me/photo/$value", fileContent))
            {
                if (uploadRes.IsSuccessStatusCode)
                {
                    //TODO
                }
                else
                {
                    //TODO
                }
            }
        }
        public static async Task<double> GetScoreForTwoFaces(Stream img1, Stream img2)
        {
            try
            {
                FaceServiceClient client = new FaceServiceClient("f877a6f32c314fd2b6726229f9b3a81a");
                var faces1 = await client.DetectAsync(img1);
                var faces2 = await client.DetectAsync(img2);

                if (faces1 == null || faces2 == null)
                {
                    var x = 1;
                    //return Json(new { error = "Error: It looks like we can't detect faces in one of these photos..." });
                }
                if (faces1.Count() == 0 || faces2.Count() == 0)
                {
                    var x = 1;
                    //return Json(new { error = "Error: It looks like we can't detect faces in one of these photos..." });
                }
                if (faces1.Count() > 1 || faces2.Count() > 1)
                {
                    var x = 1;
                    //return Json(new { error = "Error: Each photo must have only one face. Nothing more, nothing less..." });
                }
                var res = await client.VerifyAsync(faces1[0].FaceId, faces2[0].FaceId);
                double score = 0;
                if (res.IsIdentical)
                    score = 100;
                else
                {
                    score = Math.Round((res.Confidence / 0.5) * 100);
                }

                return score;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}