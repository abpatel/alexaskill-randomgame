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

        private SsmlOutputSpeech GenerateSpeechText(LeaderBoardData[] leaderBoardDatas)
        {
            
            if(leaderBoardDatas.Length == 0)
            {
                return new SsmlOutputSpeech("<speak>The leaderboard is empty. Say new game to begin a new game or stop to exit.</speak>");
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<speak>");
            sb.AppendLine(@" <amazon:emotion name=""excited"" intensity=""medium"">");
            sb.AppendLine("Here are some of the top scores.");
            foreach (var leaderBoardData in leaderBoardDatas.Take(5))
            {
                sb.AppendLine($"{leaderBoardData.UserName}  {leaderBoardData.Score} {(leaderBoardData.Score == 1?"try":"tries")}.");
                sb.AppendLine(@" <break time=""0.5s""/>");
            }
            sb.AppendLine(@"</amazon:emotion>");
            sb.AppendLine(@" <break time=""3s""/>");
            context.Session.TryGetValue(Constants.SESSION_VAR_USERNAME, out string userName);
            if (string.IsNullOrEmpty(userName))
            {
                sb.AppendLine(@"Say new game to begin a new game, or stop to exit");
            }
            else
            {
                sb.AppendLine(@"Say play again, or stop to exit");
            }
            sb.AppendLine(@"</speak>");
            return new SsmlOutputSpeech(sb.ToString());
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
                                                         secondaryText = $"{item.Score} {(item.Score == 1 ? " try":" tries")}"
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
            var speech = GenerateSpeechText(leaderBoardDatas);
            Reprompt rp = new Reprompt("Say new game to begin a new game or stop to exit");
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
