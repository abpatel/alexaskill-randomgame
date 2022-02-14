using System;
using System.Collections.Generic;
using System.Text;

namespace RandomGameSkill
{
    public static class Constants
    {
        public const string DYNAMIC_INDEX_LIST_ID = "numbersDynamicList";
        public const string ANSWER_DOC_TOKEN = "answerdoctoken";
        public const string LEADERBOARD_DOC_TOKEN = "leaderboardtoken";
        public const string APL_ANSWER_DOC = "doc://alexa/apl/documents/answerdocument.json";
        public const string APL_LAUNCH_DOC = "doc://alexa/apl/documents/launchdocument.json";
        public const string APL_LEADERBOARD_DOC = "doc://alexa/apl/documents/leaderboarddocument.json";
        public const string LABEL_LIST_VER = "listversion";
        public const string LABEL_APL_ANSWERDOC_DATASOURCE_GRIDLIST_NAME = "gridListData";
        public const string LABEL_APL_LEADERBOARDDOC_DATASOURCE_LIST_NAME = "listData";
        public const string LABEL_APL_LAUNCHDOC_DATASOURCE_NAME = "textToSet";

        public const string INTENT_AMAZON_CANCEL_INTENT = "AMAZON.CancelIntent";
        public const string INTENT_AMAZON_STOPINTENT = "AMAZON.StopIntent";
        public const string INTENT_AMAZON_HELPINTENT = "AMAZON.HelpIntent";
        public const string INTENT_CUSTOM_NEW_GAME_INTENT = "NewGameIntent";
        public const string INTENT_CUSTOM_BEGIN_GAME_INTENT = "BeginGameIntent";
        public const string INTENT_CUSTOM_ANSWER_INTENT = "AnswerIntent";
        public const string INTENT_CUSTOM_LEADERBOARD_INTENT = "LeaderBoardIntent";

        public const string SESSION_VAR_ALL_GUESSES = "all_guesses";
        public const string SESSION_VAR_MAGIC_NUMBER = "magic_number";
        public const string SESSION_VAR_NUM_GUESSES = "num_guesses";
        public const string SESSION_VAR_USERNAME = "username";

        public const string INTENT_SLOT_NUMBER = "number";
        public const string INTENT_SLOT_USERNAME = "username";

        public const string MSG_NEWGAME_AGAIN = "Say new game to play again, leaderboard to view top scores, or stop to exit.";
        public const string MSG_WRONG_GUESS = "Nope, guess again.";

    }
}
