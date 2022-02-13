using Alexa.NET;
using Alexa.NET.APL;
using Alexa.NET.APL.DataSources;
using Alexa.NET.Request;
using Alexa.NET.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomGameSkill
{
    class LaunchResponseHandler
    {
        private SkillRequest request; 
        public LaunchResponseHandler(SkillRequest request)
        {
            this.request = request;
        }
        internal SkillResponse Handle()
        {
            string speech = "Welcome! You can either say new game or say show leaderboard. Which would you like?";
            Reprompt rp = new Reprompt("You can say, new game, or leaderboard, which would you like?");
            var response = ResponseBuilder.Ask(speech, rp, request.Session);
            if (request.APLSupported())
            {
                var launchDirective =
                    new RenderDocumentDirective(new
                    APLDocumentLink(Constants.APL_LAUNCH_DOC)
                    );
                launchDirective.DataSources = new Dictionary<string, APLDataSource>()
                    {
                        {  Constants.LABEL_APL_LAUNCHDOC_DATASOURCE_NAME,
                            new KeyValueDataSource
                            {
                                Properties = new Dictionary<string, object>()
                                {
                                    { "start","Welcome"},
                                    { "middle", "to"},
                                    { "end", "Numbers Game" }
                                }
                            }
                        }
                    };
                response.Response.Directives.Add(launchDirective);
            }
            return response;
        }
    }
}
