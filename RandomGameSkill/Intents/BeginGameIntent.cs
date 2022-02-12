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
        const string ANSWER_DOC = "doc://alexa/apl/documents/answerdocument.json";
        const string ANSWER_DOC_TOKEN = "answerdoctoken";
        const string DYNAMIC_INDEX_LIST_ID = "numbersDynamicList";

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
            var dataSource = new DynamicIndexList
            {
                StartIndex = generator.Low,
                MinimumInclusiveIndex = generator.Low,
                MaximumExclusiveIndex = generator.High,
                ListId = DYNAMIC_INDEX_LIST_ID,
                Items = GenerateDataSourceItems()
            };
            return dataSource;
        }
        public SkillResponse Process()
        {
            context.Logger?.LogLine($"Entered {nameof(BeginGameIntent)}:");
            string userName = context.Request.Intent.Slots["username"].Value;
            AddSessionVariable("username", userName);
            AddSessionVariable("num_guesses", 0);
            AddSessionVariable("all_guesses", "");
            int magicNumber = generator.Generate();
            AddSessionVariable("magic_number", magicNumber);
            string next = $"OK {userName}, Let's begin. Guess a number betwen {generator.Low} and {generator.High}";
            Reprompt rp = new Reprompt(next);
            var response = ResponseBuilder.Ask(next, rp, context.Session);
            if (context.IsAPLSupported)
            {

                var launchDirective =
                    new RenderDocumentDirective(new APLDocumentLink(ANSWER_DOC));
                launchDirective.Token = ANSWER_DOC_TOKEN;
                launchDirective.DataSources = new Dictionary<string, APLDataSource>()
                                {
                                    {
                                        "gridListData",
                                         new KeyValueDataSource
                                        {
                                            Properties = new Dictionary<string, object>()
                                            {
                                                {
                                                    "listItemsToShow",
                                                    Enumerable.Range(generator.Low, generator.High)
                                                    .Select(item =>
                                                     new { listItemText = item, disabled = false
                                                     }).ToArray()
                                                }
                                            }
                                        }
                                        //new ObjectDataSource
                                        //{
                                        //    Properties = new Dictionary<string, object>()
                                        //    {
                                        //        {
                                        //            "listItemsToShow",
                                        //            Enumerable.Range(generator.Low, generator.High).Select(item =>
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
