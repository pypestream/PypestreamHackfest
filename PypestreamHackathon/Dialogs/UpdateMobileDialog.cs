using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PypestreamHackathon.Dialogs
{
    [Serializable]
    public class UpdateMobileDialog : IDialog<string>
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
                    // TODO: allow them to type in a new mobile and save it
                    authContext.Done("Good");
                }
            }, context.Activity, CancellationToken.None);
        }
    }
}