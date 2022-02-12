using Alexa.NET;
using Alexa.NET.APL;
using Alexa.NET.APL.DataSources;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberGameSkill
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
        private Session session;
        private int wowThreshold;
        const string ANSWER_DOC = "doc://alexa/apl/documents/answerdocument.json";
        const string ANSWER_DOC_TOKEN = "answerdoctoken";

        public AnswerHandler(Session session, 
            AnswerDataModel model,
            ILambdaLogger logger,
            bool isAPLSupported,
            Bounds bounds,
            int wowThreshold = 3)
        {
            this.model = model;
            this.logger = logger;
            this.isAPLSupported = isAPLSupported;
            this.bounds = bounds;
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
      
        private void TryAddAPL(SkillResponse response)
        {
            if (isAPLSupported)
            {
                var launchDirective =
                    new RenderDocumentDirective(new APLDocumentLink(ANSWER_DOC));
                launchDirective.Token = ANSWER_DOC_TOKEN;
                launchDirective.DataSources = new Dictionary<string, APLDataSource>()
                                {
                                    {
                                        "gridListData",
                                         new KeyValueDataSource
                                        {
                                            Properties = new Dictionary<string, object>()
                                            {
                                                {
                                                    "listItemsToShow",
                                                    Enumerable.Range(bounds.Low, bounds.High)
                                                    .Select(item =>
                                                     new 
                                                     { 
                                                         listItemText = item, 
                                                         disabled = Array.Find(model.AllGuesses, i => i == item) != 0
                                                     })
                                                    .ToArray()
                                                }
                                            }
                                        }
                                    }
                                };
                response.Response.Directives.Add(launchDirective);
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
                    speech = $"Correct! Wow, you guessed it in {numTries} tries! That is amazing! Say new game to play again, or stop to exit. ";
                }
                else
                {
                    speech = $"Correct! You guessed it in {numTries} tries. Say new game to play again, or stop to exit. ";
                }
                session.SetValue("num_guesses", 0);
                Reprompt rp = new Reprompt(speech);
                response = ResponseBuilder.Ask(speech, rp, session);
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
                session.SetValue("all_guesses", string.Join(",", model.AllGuesses));
                session.SetValue("num_guesses", numTries);
                Reprompt rp = new Reprompt(speech);
                response = ResponseBuilder.Ask(speech, rp, session);
                TryAddAPL(response);
            }           
            logger?.LogLine($"Exiting {nameof(AnswerHandler.Handle)}");
            return response;
        }
    }
}
