using Alexa.NET;
using Alexa.NET.APL;
using Alexa.NET.APL.DataSources;
using Alexa.NET.Request;
using Alexa.NET.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace NumberGameSkill
{
    class LaunchResponseHandler
    {
        private SkillRequest request;
        private const string LAUNCH_DOC = "doc://alexa/apl/documents/launchDocument.json";
        public LaunchResponseHandler(SkillRequest request)
        {
            this.request = request;
        }
        internal SkillResponse Handle()
        {
            string speech = "Welcome! Say new game to start";
            Reprompt rp = new Reprompt("Say new game to start");
            var response = ResponseBuilder.Ask(speech, rp, request.Session);
            if (request.APLSupported())
            {
                var launchDirective =
                    new RenderDocumentDirective(new
                    APLDocumentLink(LAUNCH_DOC)
                    );
                launchDirective.DataSources = new Dictionary<string, APLDataSource>()
                    {
                        { "textToSet",
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
