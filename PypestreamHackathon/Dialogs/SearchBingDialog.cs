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
            string query = x.Query;
            PypestreamHackathon.Helpers.BingHelper bingHelper = new BingHelper();

            var bingSearchResults = await bingHelper.GetSearchResults(query, 5);

            List<string> rez = new List<string>();

            foreach (var searchResult in bingSearchResults)
            {
                rez.Add(searchResult.Title);
            }

            string searchResultsString = String.Join(string.Empty, rez);

            // Add carosel here...time provided

            context.Done(searchResultsString);
        }
    }
}