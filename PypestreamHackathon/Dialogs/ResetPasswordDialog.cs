using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
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

                // Can only reset passwords for people with mobile numbers in AAD
                var mobile = await getMobile(alias);
                if (String.IsNullOrEmpty(mobile))
                {
                    textContext.Done("Sorry...I can only reset your password if you have a mobile number in Active Directory. You should call the help desk instead (potential bot to human handoff here).");
                }
                else
                {
                    //TODO: all validation stuff
                    textContext.Done("TODO: Please paste the code we sent your mobile phone");
                }


            }, "Locked out huh? I can help you reset your password...what is your alias?");
        }

        private async Task<AuthenticationResult> getAppOnlyToken()
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
            return await authenticationContext.AcquireTokenAsync("https://graph.microsoft.com", cac);
        }

        private async Task<string> getMobile(string alias)
        {
            var token = await getAppOnlyToken();

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