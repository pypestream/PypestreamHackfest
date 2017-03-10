using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Threading;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Luis;

namespace PypestreamHackathon.Dialogs
{

    //[LuisModel("0400b904-80eb-447e-93b0-8fd00d195241", "d677c6288aa848fe9fa0351e74d34f60")]
    // Richards
    [LuisModel("b7b20998-52e9-4e70-8848-d495cf0d9d6b", "25670274c6ad4180a3e4be0e8f4d91d5")]
    [Serializable]
    public class RootDialog : LuisDialog<IMessageActivity>
    {
        //[LuisIntent("")]
        //public async Task None(IDialogContext context, LuisResult result)
        //{
        //    await context.PostAsync("I didn't understand what you are looking for...I'm just a bot. Try this menu instead.");
        //}

        [LuisIntent("")]
        public async Task Blank(IDialogContext context, LuisResult result)
        {
            await context.Forward(new SearchBingDialog(), forwardResume, result, CancellationToken.None);
        }

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.Forward(new SearchBingDialog(), forwardResume, result, CancellationToken.None);
        }

        [LuisIntent("SearchFor")]
        public async Task SearchFor(IDialogContext context, LuisResult result)
        {
            await context.Forward(new SearchBingDialog(), forwardResume, result, CancellationToken.None);
        }


        [LuisIntent("ResetPassword")]
        public async Task ResetPassword(IDialogContext context, LuisResult result)
        {
            await context.Forward(new ResetPasswordDialog(), forwardResume, result, CancellationToken.None);
        }

        [LuisIntent("UpdateMobile")]
        public async Task UpdateMobile(IDialogContext context, LuisResult result)
        {
            await context.Forward(new UpdateMobileDialog(), forwardResume, result, CancellationToken.None);
        }

        [LuisIntent("UpdatePicture")]
        public async Task UpdatePicture(IDialogContext context, LuisResult result)
        {
            await context.Forward(new UpdatePictureDialog(), forwardResume, result, CancellationToken.None);
        }

        public async Task forwardResume(IDialogContext context, IAwaitable<string> result)
        {
            var msg = await result;
            await context.PostAsync(msg);
            await context.PostAsync("What else can I help you with?");
        }
    }
}