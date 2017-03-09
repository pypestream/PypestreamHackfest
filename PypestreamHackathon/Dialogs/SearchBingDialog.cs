using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using PypestreamHackathon.Helpers;

namespace PypestreamHackathon.Dialogs
{
    [Serializable]
    public class SearchBingDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait<LuisResult>(MessageRecievedAsync);
        }

        public async Task MessageRecievedAsync(IDialogContext context, IAwaitable<LuisResult> input)
        {
            var x = await input;
            //PypestreamHackathon.Helpers.BingHelper bingHelper = new BingHelper();

            //Task<List<SearchResult>> bingSearchResult = bingHelper.GetSearchResults("BitCoin");


            context.Done("TODO: search bing!");
        }
    }
}