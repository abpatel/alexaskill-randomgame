using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace NumberGameSkill.Intents
{

    class UnknownIntent
    {      
        private IntentContext context;
        public UnknownIntent(IntentContext context)
        {           
            this.context = context;
        }

        public SkillResponse Process()
        {
            context.Logger?.LogLine($"Unknown intent: " + context.Request.Intent.Name);
            string speech = "Sorry, I didn't get that. Please try again";
            Reprompt rp = new Reprompt(speech);
            context.Logger?.LogLine($"Exiting {nameof(UnknownIntent)}:");
            return ResponseBuilder.Ask(speech, rp, context.Session);
        }
    }
}
