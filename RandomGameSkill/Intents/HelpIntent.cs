using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomGameSkill.Intents
{

    class HelpIntent
    {      
        private IntentContext context;
        public HelpIntent(IntentContext context)
        {           
            this.context = context;
        }

        public SkillResponse Process()
        {
            Reprompt rp = new Reprompt("What's next?");
            return ResponseBuilder.Ask("Here's some help. What's next?",
                rp,
                context.Session);
        }
    }
}
