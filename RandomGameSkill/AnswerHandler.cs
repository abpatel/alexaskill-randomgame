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
      
        private void TryAddAPLDirective(SkillResponse response)
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
      
        internal SkillResponse Handle()
        {
            logger?.LogLine($"Entered {nameof(AnswerHandler.Handle)}:");

            int magicNumber = model.MagicNumber;
            int numTries = model.NumGuesses + 1;
            string speech = "";
            SkillResponse response;
            if (magicNumber == model.CurrentGuess)
            {
                if (numTries < wowThreshold)
                {
                    string triesMessage = numTries == 1 ? "try" : "tries";
                    speech = $"Correct! Wow, you guessed it in {numTries} {triesMessage}.Amazing! Say new game to play again, or stop to exit. ";
                }
                else
                {
                    speech = $"Correct! You guessed it in {numTries} tries. Say new game to play again, or stop to exit. ";
                }
                session.SetValue(Constants.SESSION_VAR_NUM_GUESSES, 0);
                Reprompt rp = new Reprompt(speech);
                response = ResponseBuilder.Ask(speech, rp, session);
                repo.WriteToLeaderboard(model.UserName, numTries);
                //WriteToLeaderboard(
                //   session.Attributes["username"] as string,
                //   numTries,
                //   logger
                //   );
            }
            else
            {
                speech = "Nope, guess again.";
                model.AllGuesses = AddGuess(model.AllGuesses, model.CurrentGuess);
                session.SetValue(Constants.SESSION_VAR_ALL_GUESSES, string.Join(",", model.AllGuesses));
                session.SetValue(Constants.SESSION_VAR_NUM_GUESSES, numTries);
                Reprompt rp = new Reprompt(speech);
                response = ResponseBuilder.Ask(speech, rp, session);
                TryAddAPLDirective(response);
            }           
            logger?.LogLine($"Exiting {nameof(AnswerHandler.Handle)}");
            return response;
        }
    }
}
