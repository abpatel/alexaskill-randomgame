using Alexa.NET;
using Alexa.NET.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomGameSkill
{
    public class DefaultSkillResponse
    {

        public static SkillResponse Response
        {
            get => ResponseBuilder.Tell("Sorry, I didn't get that. Please try again");

        }
    }
}
