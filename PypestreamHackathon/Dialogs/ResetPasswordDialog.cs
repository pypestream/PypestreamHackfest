using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PypestreamHackathon.Dialogs
{
    [Serializable]
    public class ResetPasswordDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait<LuisResult>(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<LuisResult> input)
        {
            var luis = await input;

            PromptDialog.Text(context, async (IDialogContext textContext, IAwaitable<string> textResult) =>
            {
                var alias = await textResult;
                textContext.ConversationData.SetValue<string>("Alias", alias);

                // Can only reset passwords for people with mobile numbers in AAD
                var mobile = await getMobile(alias);
                if (String.IsNullOrEmpty(mobile))
                {
                    textContext.Done("Sorry...I can only reset your password if you have a mobile number in Active Directory. You should call the help desk instead (potential bot to human handoff here).");
                }
                else
                {
                    //TODO: all validation stuff
                    mobile = mobile.Replace(" ", "");
                    var magicNumber = GenerateRandomNumber();
                    textContext.ConversationData.SetValue<int>("ResetMagicNumber", magicNumber);
                    Helpers.TwilioHelper.sendSMS(mobile, $"Pypestream: your code is " + magicNumber);
                    PromptDialog.Text(textContext, async (IDialogContext mnContext, IAwaitable<string> mnResult) =>
                    {
                        var code = await mnResult;
                        if (Convert.ToInt32(code) == mnContext.ConversationData.Get<int>("ResetMagicNumber"))
                        {
                            // Reset the password for the user
                            var randomPassword = System.Web.Security.Membership.GeneratePassword(10, 1);
                            await resetPassword(mnContext.ConversationData.Get<string>("Alias"), randomPassword);
                            mnContext.Done($"Your password has been reset to ${randomPassword} and you will be forced to change it after logging in");
                        }
                        else
                        {
                            // Error...codes didn't match
                            mnContext.Done("ERROR: Your code did not match the code we sent");
                        }
                    }, "Please paste the code Pypestream just sent to your mobile phone");
                }


            }, "Locked out huh? I can help you reset your password...what is your alias?");
        }

        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        private int GenerateRandomNumber()
        {
            int number = 0;
            byte[] randomNumber = new byte[1];
            do
            {
                rngCsp.GetBytes(randomNumber);
                var digit = randomNumber[0] % 10;
                number = number * 10 + digit;
            } while (number.ToString().Length < 6);
            return number;
        }

        private async Task<AuthenticationResult> getAppOnlyToken(string resource)
        {
            AuthenticationContext authenticationContext = new AuthenticationContext("https://login.microsoftonline.com/rzna.onmicrosoft.com", false);

            //read the certificate private key from the executing location
            //NOTE: This is a hack…Azure Key Vault is best approach
            var certPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~"), "PypestreamCert.pfx");
            var certfile = System.IO.File.OpenRead(certPath);
            var certificateBytes = new byte[certfile.Length];
            certfile.Read(certificateBytes, 0, (int)certfile.Length);
            var cert = new X509Certificate2(
                certificateBytes,
                "pass@word1",
                X509KeyStorageFlags.Exportable |
                X509KeyStorageFlags.MachineKeySet |
                X509KeyStorageFlags.PersistKeySet);
            ClientAssertionCertificate cac = new ClientAssertionCertificate(ConfigurationManager.AppSettings["aad:ClientId"], cert);
            return await authenticationContext.AcquireTokenAsync(resource, cac);
        }

        private async Task resetPassword(string alias, string password)
        {
            var token = await getAppOnlyToken("https://graph.windows.net");

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.AccessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var data = JObject.Parse("{ \"passwordProfile\": { \"password\": \"" + password + "\", \"forceChangePasswordNextLogin\": false }}");
            var uri = $"https://graph.windows.net/myorganization/users/{alias}?api-version=1.6";
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), uri)
            {
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };
            using (var response = await client.SendAsync(request))
            {
                //TODO: this will fail...need to look at admin consent or keeping refresh token of a directory admin
                if (response.IsSuccessStatusCode)
                {
                }
            }
        }

        private async Task<string> getMobile(string alias)
        {
            var token = await getAppOnlyToken("https://graph.microsoft.com");

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.AccessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            using (var response = await client.GetAsync($"https://graph.microsoft.com/v1.0/users/{alias}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var jobj = JObject.Parse(json);
                    return jobj.Value<string>("mobilePhone");
                }
                else
                    return null;
            }
        }
    }
}