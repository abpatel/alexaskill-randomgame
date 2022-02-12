using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberGameSkill
{

    class DataModelFactory
    {
        private static int[] GetGuesses(Session session)
        {
            int[] guesses = session.GetValue<string>("all_guesses")
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(i => int.Parse(i))
                .ToArray();
            return guesses;
        }
        internal static AnswerDataModel CreateAnswerDataModel(IntentRequest intentRequest, 
            Session session)
        {
            string userAnswerStr = intentRequest.Intent.Slots["number"].Value;
            var model = new AnswerDataModel
            {
                CurrentGuess =   Convert.ToInt32(long.Parse(userAnswerStr)),
                MagicNumber = session.GetValue<int>("magic_number"),
                NumGuesses = session.GetValue<int>("num_guesses"),
                AllGuesses  = GetGuesses(session),
                UserName = session.GetValue<string>("username") 
            };
            return model;

        }

        internal static AnswerDataModel CreateAnswerDataModel(UserEventRequest userEventRequest,
            Session session)
        {

            string eventName = userEventRequest.Arguments[0];
            string itemOrdinal = userEventRequest.Arguments[1];
            string itemSelectedText = userEventRequest.Arguments[2];
            var model = new AnswerDataModel
            {
                CurrentGuess = Convert.ToInt32(long.Parse(itemSelectedText)),
                MagicNumber = session.GetValue<int>("magic_number"),
                NumGuesses = session.GetValue<int>("num_guesses"),
                AllGuesses = GetGuesses(session),
                UserName = session.GetValue<string>("username")
            };
            return model;
        }
    }
}
