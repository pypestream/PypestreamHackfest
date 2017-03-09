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
                        var newPicStream = "";

                        // Get the stream of the current picture
                        var originalPicStream = await getProfilePic(attachmentsContext.ConversationData.Get<string>("AccessToken"));

                        // TODO: Check if new picture is a close enough match to the original pic

                        //HACK
                        attachmentsContext.Done("Sorry, the picture you uploaded does not resemble you.");
                    }, "Please upload a new profile photo");
                }
            }, context.Activity, CancellationToken.None);
        }

        private async Task<Stream> getProfilePic(string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            using (var response = await client.GetAsync($"https://graph.microsoft.com/v1.0/me/photo/$value"))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStreamAsync();
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
    }
}