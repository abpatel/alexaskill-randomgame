using Alexa.NET;
using Alexa.NET.APL;
using Alexa.NET.APL.DataSources;
using Alexa.NET.APL.Operation;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using RandomGameSkill.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomGameSkill
{

    class AnswerDataModel
    {
        public string UserName { get; set; }
        public int MagicNumber { get; set; }
        public int CurrentGuess { get; set; }
        public int[] AllGuesses { get; set; }
        public int NumGuesses { get; set; }
    }
    class AnswerHandler
    { 
        private AnswerDataModel model;
        private readonly ILambdaLogger logger;
        private readonly bool isAPLSupported;
        private readonly Bounds bounds;
        private readonly LeaderBoardRepo repo;
        private Session session;
        private int wowThreshold;
   
        public AnswerHandler(Session session, 
            AnswerDataModel model,
            ILambdaLogger logger,
            bool isAPLSupported,
            Bounds bounds,
            LeaderBoardRepo repo,
            int wowThreshold = 3)
        {
            this.model = model;
            this.logger = logger;
            this.isAPLSupported = isAPLSupported;
            this.bounds = bounds;
            this.repo = repo;
            this.session = session;
            this.wowThreshold = wowThreshold;
        }        

        private int[] AddGuess(int[] allGuesses, int guessToAdd)
        {
            int[] guesses = new int[allGuesses.Length + 1];
            allGuesses.CopyTo(guesses, 0);
            guesses[guesses.Length - 1] = guessToAdd;
            return guesses;
        }
      
        private void TryAddAPLDirectiveOnWrongGuess(SkillResponse response)
        {
            if (isAPLSupported)
            {
                int listVersion = 0;
                session.TryGetValue(Constants.LABEL_LIST_VER, out listVersion);
                listVersion += 1;
                session.SetValue(Constants.LABEL_LIST_VER, listVersion);
                var updateListDataDirective = new UpdateIndexListDataDirective
                {
                    ListId = Constants.DYNAMIC_INDEX_LIST_ID,
                    Token = Constants.ANSWER_DOC_TOKEN,
                    ListVersion = listVersion,
                    Operations = new List<Operation>
                     {
                         new SetItem
                            {
                             Index = model.CurrentGuess,
                             Item = new {
                                            listItemText = model.CurrentGuess,
                                            disabled = model.CurrentGuess != model.MagicNumber
                                        }
                             }
                     }
                };
                response.Response.Directives.Add(updateListDataDirective);
            }
        }

        private void TryAddAPLDirectiveOnCorrectGuess(SkillResponse response)
        {
            if (isAPLSupported)
            {
                int listVersion = 0;
                session.TryGetValue(Constants.LABEL_LIST_VER, out listVersion);
                listVersion += 1;
                session.SetValue(Constants.LABEL_LIST_VER, listVersion);
                var updateListDataDirective = new UpdateIndexListDataDirective
                {
                    ListId = Constants.DYNAMIC_INDEX_LIST_ID,
                    Token = Constants.ANSWER_DOC_TOKEN,
                    ListVersion = listVersion,
                    Operations =
                    //new List<Operation>
                    //{
                    //    new SetItem
                    //    {
                    //        Index = model.CurrentGuess,
                    //        Item = new
                    //        {
                    //            listItemText = model.CurrentGuess,
                    //            disabled = true,
                    //            correctGuess = (model.CurrentGuess == model.MagicNumber)
                    //        }
                    //    }

                    //}
                    Enumerable.Range(bounds.Low, bounds.High).Except(model.AllGuesses)
                    .Select(i => new SetItem
                    {
                        Index = i,
                        Item = new
                        {
                            listItemText = i,
                            disabled = true,
                            correctGuess = (model.CurrentGuess == model.MagicNumber && i == model.MagicNumber)
                        }
                    }).OfType<Operation>().ToList()

                };
                response.Response.Directives.Add(updateListDataDirective);
            }
        }

        internal SkillResponse Handle()
        {
            logger?.LogLine($"Entered {nameof(AnswerHandler.Handle)}:");
            SsmlOutputSpeech ssmlSpeech;
            int numTries = model.NumGuesses + 1;
            string speech = string.Empty;
            SkillResponse response;
            if (model.MagicNumber == model.CurrentGuess)
            {
                if (numTries < wowThreshold)
                {
                    string triesMessage = numTries == 1 ? "try" : "tries";
                    speech = @$"Correct! Wow, you guessed it in {numTries} {triesMessage}.  
                                You can say play again, or leaderboard to view the leaderboard, or stop to exit. ";
                }
                else
                {
                    speech = $"Correct! You guessed it in {numTries} tries. You can say play again, or leaderboard to view the leaderboard, or stop to exit. ";
                }
                session.SetValue(Constants.SESSION_VAR_NUM_GUESSES, 0);
                Reprompt rp = new Reprompt(speech);
                response = ResponseBuilder.Ask(speech, rp, session);
                TryAddAPLDirectiveOnCorrectGuess(response);
                repo.WriteToLeaderboard(model.UserName, numTries);
                /*if (numTries < wowThreshold)
                {
                    StringBuilder sb = new StringBuilder(@"<speak><amazon:emotion name=""excited"" intensity=""medium"">");
                    string triesMessage = numTries == 1 ? "try" : "tries";
                    speech = $"Correct! Wow, you guessed it in {numTries} {triesMessage}.{Environment.NewLine}Amazing!";
                    sb.AppendLine(speech);                   
                    sb.AppendLine(@"</amazon:emotion>");
                    sb.AppendLine(Constants.MSG_NEWGAME_AGAIN);
                    sb.AppendLine(@"</speak>");
                    ssmlSpeech = new SsmlOutputSpeech(sb.ToString());
                }
                else
                {
                    speech = $"<speak>Correct! You guessed it in {numTries} tries.</speak>";
                    ssmlSpeech = new SsmlOutputSpeech(string.Concat(speech, Constants.MSG_NEWGAME_AGAIN));
                }
                session.SetValue(Constants.SESSION_VAR_NUM_GUESSES, 0);                
                Reprompt rp = new Reprompt(Constants.MSG_NEWGAME_AGAIN);
                response = ResponseBuilder.Ask(ssmlSpeech, rp, session);
                repo.WriteToLeaderboard(model.UserName, numTries);
                */
                //TryAddAPLDirectiveOnCorrectGuess(response);
            }
            else
            {               
                model.AllGuesses = AddGuess(model.AllGuesses, model.CurrentGuess);
                session.SetValue(Constants.SESSION_VAR_ALL_GUESSES, string.Join(",", model.AllGuesses));
                session.SetValue(Constants.SESSION_VAR_NUM_GUESSES, numTries);
                Reprompt rp = new Reprompt(Constants.MSG_WRONG_GUESS);
                response = ResponseBuilder.Ask(Constants.MSG_WRONG_GUESS, rp, session);
                TryAddAPLDirectiveOnWrongGuess(response);
            }           
            logger?.LogLine($"Exiting {nameof(AnswerHandler.Handle)}");
            return response;
        }
    }
}
