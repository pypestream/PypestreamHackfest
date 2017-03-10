using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AuthBot;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using AuthBot.Helpers;
using AuthBot;
using AuthBot.Dialogs;
using System.Threading;

namespace PypestreamHackathon.Dialogs
{
    [Serializable]
    public class AuthDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> input)
        {
            var msg = await input;

            // Try to get a token
            var token = await context.GetAccessToken("https://graph.microsoft.com");

            if (!String.IsNullOrEmpty(token))
            {
                // Token request was successful so send back to caller
                context.Done(token);
            }
            else
            {
                // Forward to AzureAuthDialog to manage auth process
                await context.Forward(new AzureAuthDialog("https://graph.microsoft.com", "Wait a second...I don't know who you are...please sign-in:"), async (IDialogContext authContext, IAwaitable<string> authResult) => {
                    // Resume after auth
                    var message = await authResult;
                    await authContext.PostAsync(message);

                    // Get the token
                    var t = await authContext.GetAccessToken("https://graph.microsoft.com");
                    authContext.Done(t);
                }, msg, CancellationToken.None);
            }
        }
    }
}