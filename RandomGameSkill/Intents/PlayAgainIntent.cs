using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomGameSkill.Intents
{

    class PlayAgainIntent
    {
        const string INTENT_NAME = "PlayAgainIntent";
        private IntentContext context;
        private readonly MagicNumberGenerator generator;

        public PlayAgainIntent(IntentContext context, MagicNumberGenerator generator)
        {
            if(string.Compare(context.Request.Intent.Name, INTENT_NAME) != 0)
            {
                throw new InvalidOperationException("Invalid Intent");
            }
            this.context = context;
            this.generator = generator;
        }

        public SkillResponse Process()
        {
            context.Logger?.LogLine($"Entered {nameof(PlayAgainIntent)}:");
            string userName = string.Empty;
            if (context.Session.TryGetValue(Constants.SESSION_VAR_USERNAME, out userName))
            {
                context.Request.Intent.Name = Constants.INTENT_CUSTOM_BEGIN_GAME_INTENT;
                var response = new BeginGameIntent(context, generator).Process();
                context.Logger?.LogLine($"Exiting {nameof(PlayAgainIntent)}");
                return response;
            }
            string speech = "What is your name?";
            Reprompt rp = new Reprompt("For example, you can say, my name is Jim");
            context.Logger?.LogLine($"Exiting {nameof(PlayAgainIntent)}");
            return ResponseBuilder.Ask(speech, rp, context.Session);

        }
    }
}
