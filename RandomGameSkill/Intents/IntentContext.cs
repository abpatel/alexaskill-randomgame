using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Amazon.Lambda.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomGameSkill.Intents
{
    public class IntentContext
    {
        public Session Session { get; internal set; }
        public IntentRequest Request { get; internal set; }
        public ILambdaLogger Logger { get; internal set; }
        public bool IsAPLSupported {get; internal set;}
    }
}
