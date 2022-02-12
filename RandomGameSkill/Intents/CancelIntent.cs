using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomGameSkill.Intents
{

    class CancelIntent
    {      
        private IntentContext context;
        public CancelIntent(IntentContext context)
        {           
            this.context = context;
        }

        public SkillResponse Process()
        {
            return ResponseBuilder.Tell("Goodbye!");
        }
    }
}
