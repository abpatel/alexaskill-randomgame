using Alexa.NET;
using Alexa.NET.APL;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace NumberGameSkill.Intents
{
    class AnswerIntent
    {
        const string INTENT_NAME = "AnswerIntent";
        const string ANSWER_DOC = "doc://alexa/apl/documents/answerdocument.json";
        private IntentContext context;
        private readonly Bounds bounds; 

        public AnswerIntent(IntentContext context, Bounds bounds)
        {
            if (string.Compare(context.Request.Intent.Name, INTENT_NAME) != 0)
            {
                throw new InvalidOperationException("Invalid Intent");
            }
            this.context = context;
            this.bounds = bounds;
        }

        private void AddSessionVariable(string name, object value)
        {
            context.Session.Attributes[name] = value;
        }

        
        public SkillResponse Process()
        {
            context.Logger?.LogLine($"Entered {nameof(AnswerIntent)}:");
            var model = DataModelFactory.CreateAnswerDataModel(context.Request, context.Session);
            var answerResponse = new AnswerHandler(context.Session,
                model, 
                context.Logger, 
                context.IsAPLSupported,
                bounds).Handle();
            context.Logger?.LogLine($"Exiting {nameof(AnswerIntent)}");
            return answerResponse;

        }
    }
}
