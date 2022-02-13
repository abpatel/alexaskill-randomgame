using Alexa.NET;
using Alexa.NET.APL;
using Alexa.NET.APL.DataSources;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using RandomGameSkill.Intents;
using RandomGameSkill.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomGameSkill
{
    class IntentResponseHandler
    {
        private SkillRequest skillRequest;
        private readonly Bounds bounds;
        private ILambdaLogger logger;
       
        public IntentResponseHandler(SkillRequest skillRequest, Bounds bounds, ILambdaLogger logger)
        {
            this.skillRequest = skillRequest;
            this.bounds = bounds;
            this.logger = logger;
        }

        private IntentContext GetContext(IntentRequest intentRequest)
        {
            var context = new IntentContext
            {
                Session = skillRequest.Session,
                Request = intentRequest,
                IsAPLSupported = skillRequest.APLSupported(),
                Logger = logger,
                LeaderBoardRepo = new LeaderBoardRepo(logger)
            };
            return context;
        }
        internal SkillResponse Handle()
        {
            logger?.LogLine($"Entered {nameof(IntentResponseHandler.Handle)}:");

            var intentRequest = skillRequest.Request as IntentRequest;
            if (intentRequest != null)
            {
                var context = GetContext(intentRequest);
                switch (intentRequest.Intent.Name)
                {                   
                    case Constants.INTENT_AMAZON_CANCEL_INTENT:
                        {
                            var response = new CancelIntent(context).Process();
                            return response;
                        }
                    case Constants.INTENT_AMAZON_STOPINTENT:
                        return ResponseBuilder.Tell("Goodbye!");
                    case Constants.INTENT_AMAZON_HELPINTENT:
                        {
                            var response = new HelpIntent(context).Process();
                            return response;
                        }
                    case Constants.INTENT_CUSTOM_NEW_GAME_INTENT:
                        {
                            var response = new NewGameIntent(context).Process();
                            return response;
                        }
                    case Constants.INTENT_CUSTOM_BEGIN_GAME_INTENT:
                        {
                            MagicNumberGenerator generator = new MagicNumberGenerator(bounds.Low, bounds.High);
                            var response = new BeginGameIntent(context, generator).Process();
                            return response;
                        }
                    case Constants.INTENT_CUSTOM_ANSWER_INTENT:
                        {
                            var response = new AnswerIntent(context, bounds).Process();
                            return response;
                        }
                    case Constants.INTENT_CUSTOM_LEADERBOARD_INTENT:
                        {
                            var response = new LeaderBoardIntent(context).Process();
                            return response;
                        }
                    default:
                        {
                            var response = new UnknownIntent(context).Process();
                            return response;
                        }
                }
            }
            else
                return DefaultSkillResponse.Response;
        }
    }
}
