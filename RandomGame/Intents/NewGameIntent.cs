using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace NumberGameSkill.Intents
{

    class NewGameIntent
    {
        const string INTENT_NAME = "NewGameIntent";
        private IntentContext context;
        public NewGameIntent(IntentContext context)
        {
            if(string.Compare(context.Request.Intent.Name, INTENT_NAME) != 0)
            {
                throw new InvalidOperationException("Invalid Intent");
            }
            this.context = context;
        }

        public SkillResponse Process()
        {
            context.Logger?.LogLine($"Entered {nameof(NewGameIntent)}:");
            string speech = "What is your name?";
            Reprompt rp = new Reprompt("For example, you can say, my name is Jim");
            context.Logger?.LogLine($"Exiting {nameof(NewGameIntent)}"); 
            return ResponseBuilder.Ask(speech, rp, context.Session);
        }
    }
}
