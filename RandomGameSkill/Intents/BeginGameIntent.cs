using Alexa.NET;
using Alexa.NET.APL;
using Alexa.NET.APL.DataSources;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomGameSkill.Intents
{
    class BeginGameIntent
    {
        const string INTENT_NAME = "BeginGameIntent";

        private IntentContext context;
        private readonly MagicNumberGenerator generator;

        public BeginGameIntent(IntentContext context, MagicNumberGenerator generator)
        {
            if (string.Compare(context.Request.Intent.Name, INTENT_NAME) != 0)
            {
                throw new InvalidOperationException("Invalid Intent");
            }
            this.context = context;
            this.generator = generator;
        }

        private void AddSessionVariable(string name, object value)
        {
            context.Session.Attributes[name] = value;
        }

        private IList<object> GenerateDataSourceItems()
        {
            var range = Enumerable.Range(generator.Low, generator.High)
                                                    .Select(item =>
                                                     new
                                                     {
                                                         listItemText = item,
                                                         disabled = false
                                                     })
                                                    .OfType<object>()
                                                    .ToList();
            return range;
        }
        private APLDataSource CreateDataSource()
        {
            //ref: https://developer.amazon.com/en-US/docs/alexa/alexa-presentation-language/apl-data-source.html#use-directives-and-requests-to-manage-the-list
            var dataSource = new DynamicIndexList
            {
                StartIndex = generator.Low,
                MinimumInclusiveIndex = generator.Low,
                MaximumExclusiveIndex = generator.High + 1,
                ListId = Constants.DYNAMIC_INDEX_LIST_ID,
                Items = GenerateDataSourceItems()
            };
            return dataSource;
        }
        public SkillResponse Process()
        {
            context.Logger?.LogLine($"Entered {nameof(BeginGameIntent)}:");
            string userName = context.Request.Intent.Slots[Constants.INTENT_SLOT_USERNAME].Value;
            AddSessionVariable(Constants.SESSION_VAR_USERNAME, userName);
            AddSessionVariable(Constants.SESSION_VAR_NUM_GUESSES, 0);
            AddSessionVariable(Constants.LABEL_LIST_VER, 0);
            AddSessionVariable(Constants.SESSION_VAR_ALL_GUESSES, "");
            int magicNumber = generator.Generate();
            AddSessionVariable(Constants.SESSION_VAR_MAGIC_NUMBER, magicNumber);
            string next = $"OK {userName}, Let's begin. Guess a number betwen {generator.Low} and {generator.High}";
            Reprompt rp = new Reprompt(next);
            var response = ResponseBuilder.Ask(next, rp, context.Session);
            if (context.IsAPLSupported)
            {

                var launchDirective =
                    new RenderDocumentDirective(new APLDocumentLink(Constants.APL_ANSWER_DOC));
                launchDirective.Token = Constants.ANSWER_DOC_TOKEN;
                launchDirective.DataSources = new Dictionary<string, APLDataSource>()
                                {
                                    {
                                        Constants.LABEL_APL_ANSWERDOC_DATASOURCE_GRIDLIST_NAME,
                                        CreateDataSource()
                                        // new KeyValueDataSource
                                        //{
                                        //    Properties = new Dictionary<string, object>()
                                        //    {
                                        //        {
                                        //            "listItemsToShow",
                                        //            Enumerable.Range(generator.Low, generator.High)
                                        //            .Select(item =>
                                        //             new { listItemText = item, disabled = false
                                        //             }).ToArray()
                                        //        }
                                        //    }
                                        //}
                                    }
                                };
                response.Response.Directives.Add(launchDirective);
            }
            context.Logger?.LogLine($"Exiting {nameof(BeginGameIntent)}");
            return response;
        }
    }
}
