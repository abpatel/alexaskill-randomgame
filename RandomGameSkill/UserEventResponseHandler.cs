using Alexa.NET;
using Alexa.NET.APL;
using Alexa.NET.APL.DataSources;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using RandomGameSkill.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomGameSkill
{
    class UserEventResponseHandler
    {
        private SkillRequest request;
        private readonly Bounds bounds;
        private readonly LeaderBoardRepo repo;
        private readonly ILambdaLogger logger;

        public UserEventResponseHandler(SkillRequest request, 
            Bounds bounds,
            ILambdaLogger logger)
        {
            this.request = request;
            this.bounds = bounds;
            this.repo = new LeaderBoardRepo(logger);
            this.logger = logger;
        }
        internal SkillResponse Handle()
        {
            logger?.LogLine($"Entered {nameof(UserEventResponseHandler.Handle)}:");
            UserEventRequest userEventRequest = request.Request as UserEventRequest;
            if(userEventRequest == null)
            {
                throw new InvalidOperationException($"Request received was not a {nameof(UserEventRequest)}");
            }
            var model = DataModelFactory.CreateAnswerDataModel(userEventRequest, request.Session);
            var answerResponse = new AnswerHandler(request.Session, 
                model, 
                logger, 
                request.APLSupported(),
                bounds,
                repo).Handle();
            logger?.LogLine($"Exiting {nameof(UserEventResponseHandler.Handle)}");
            return answerResponse;
        }
    }
}
