using Alexa.NET;
using Alexa.NET.APL;
using Alexa.NET.APL.DataSources;
using Alexa.NET.Response;
using RandomGameSkill.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomGameSkill.Intents
{
    class LeaderBoardIntent
    {
        const string INTENT_NAME = "LeaderBoardIntent";
        private IntentContext context;
        private int numEntriesToReturn = 10;
        public LeaderBoardIntent(IntentContext context)
        {
            if (string.Compare(context.Request.Intent.Name, INTENT_NAME) != 0)
            {
                throw new InvalidOperationException("Invalid Intent");
            }
            this.context = context;
        }

        private string GenerateSpeechText(LeaderBoardData[] leaderBoardDatas)
        {
            if(leaderBoardDatas.Length == 0)
            {
                return "The leaderboard is empty, say new game to begin a new game or stop to exit";
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<p>Here are some of the top scores.</p>");
            foreach (var leaderBoardData in leaderBoardDatas.Take(5))
            {
                sb.AppendLine($"<p>{leaderBoardData.UserName} took {leaderBoardData.Score} {(leaderBoardData.Score == 1?"try":"tries")}.</p>");
                sb.AppendLine();
            }            
            return sb.ToString();
        }

        private APLDataSource CreateDataSource(LeaderBoardData[] leaderBoardDatas)
        {
            var dataSource = new KeyValueDataSource
            {
                Properties = new Dictionary<string, object>()
                                            {
                                                {
                                                    "listItemsToShow",
                                                    leaderBoardDatas
                                                    .Select(item =>
                                                     new
                                                     {
                                                         primaryText = item.UserName,
                                                         secondaryText = item.Score
                                                     }).ToArray()
                                                }
                                            }
            };
            return dataSource;
        }

            public SkillResponse Process()
        {
            context.Logger?.LogLine($"Entered {nameof(LeaderBoardIntent.Process)}:");
            var leaderBoardDatas = context.LeaderBoardRepo.GetHighScores(numEntriesToReturn);
            string speech = GenerateSpeechText(leaderBoardDatas);
            Reprompt rp = new Reprompt(speech);
            var response = ResponseBuilder.Ask(speech, rp, context.Session);
            if (context.IsAPLSupported)
            {
                var launchDirective =
                    new RenderDocumentDirective(new APLDocumentLink(Constants.APL_LEADERBOARD_DOC));
                launchDirective.Token = Constants.LEADERBOARD_DOC_TOKEN;
                launchDirective.DataSources = new Dictionary<string, APLDataSource>()
                                {
                                    {
                                        Constants.LABEL_APL_LEADERBOARDDOC_DATASOURCE_LIST_NAME,
                                        CreateDataSource(leaderBoardDatas)
                                    }
                                };
                response.Response.Directives.Add(launchDirective);
            }
            context.Logger?.LogLine($"Exiting {nameof(LeaderBoardIntent.Process)}");
            return response;
        }
    }
}
