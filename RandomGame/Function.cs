using Alexa.NET;
using Alexa.NET.APL;
using Alexa.NET.APL.DataSources;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace NumberGameSkill
{
    public class NumberGameLambdaFunc
    {
        private Bounds bounds = new Bounds(1, 10);
        public NumberGameLambdaFunc()
        {
            new UserEventRequestHandler().AddToRequestConverter();
        }
        public SkillResponse FunctionHandler(SkillRequest input,  ILambdaContext context)
        {
            ILambdaLogger logger = context.Logger;
            Session session = input.Session;
            if (session.Attributes == null)
                session.Attributes = new Dictionary<string, object>();            
           
            switch(input.Request)
            {
                case LaunchRequest _:
                    var launchResponse = new LaunchResponseHandler(input).Handle();
                    return launchResponse;               
                case IntentRequest intentRequest:
                    var intentResponse = new IntentResponseHandler(input, bounds, logger).Handle();
                    return intentResponse;                    
                case UserEventRequest userEventRequest:
                    var userEventResponse = new UserEventResponseHandler(input, bounds, logger).Handle();
                    return userEventResponse;
                case SessionEndedRequest _:
                    return ResponseBuilder.Tell("Goodbye!");
                default:
                    return ResponseBuilder.Tell("Goodbye!");
            }
        }       
    }
}