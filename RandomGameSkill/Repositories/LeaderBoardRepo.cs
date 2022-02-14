using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RandomGameSkill.Repositories
{
    class LeaderBoardData
    {
        public int Score { get; set; }
        public string UserName { get; set; }
    }
    class LeaderBoardRepo
    {
        private readonly ILambdaLogger logger;
        private const string gameName = "Random Numbers Game";

        public LeaderBoardRepo(ILambdaLogger logger)
        {
            this.logger = logger;
        }
        
        private LeaderBoardData[] CreateLeaderBoardDatas(QueryResponse response)
        {
            StringBuilder sb = new StringBuilder();
            List<LeaderBoardData> leaderBoardDatas = new List<LeaderBoardData>();
            foreach (var item in response.Items)
            {
                string userName = item["Username"].S;
                int score = int.Parse(item["Score"].N);
                string strTimeStamp = item.ContainsKey("Timestamp") ?
                    item["Timestamp"].S : string.Empty;
                leaderBoardDatas.Add(new LeaderBoardData { UserName = userName, Score = score });
                sb.AppendLine($"userName:{userName}, score={score}, timeStamp={strTimeStamp}");
            }
            logger.LogLine(sb.ToString());
            return leaderBoardDatas.ToArray();
        }
     
        internal async Task WriteToLeaderboardAsync(string userName, int numTries)
        {
            try
            {
                logger.LogLine($"Entered {nameof(LeaderBoardRepo.WriteToLeaderboardAsync)}");

                using (var client = new AmazonDynamoDBClient())
                {
                    using (var dbContext = new DynamoDBContext(client))
                    {
                        Table table = Table.LoadTable(client, "Leaderboard");
                        var doc = new Document();
                        doc["Username"] = userName;
                        doc["Game"] = gameName;
                        doc["Score"] = numTries;
                        doc["Timestamp"] = DateTime.UtcNow;
                        await table.PutItemAsync(doc);
                        logger.LogLine($"Exiting {nameof(LeaderBoardRepo.WriteToLeaderboardAsync)}");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogLine(ex.ToString());
                throw;
            }
        }

        internal void WriteToLeaderboard(string userName,
             int numTries)
        {
            Task.Run(async () => await WriteToLeaderboardAsync(userName, numTries))
                .Wait();
        }
        internal LeaderBoardData[] GetHighScores(int limit)
        {
            // query using Global Secondary Index 
            // partition key is Game and sort key is TopScore
            // note we specify indexName in the query
            // ScanIndexForward is false so results are descending
            // and Limit is 1 so we only get the single high score back
            try
            {
                logger.LogLine($"Entered {nameof(LeaderBoardRepo.WriteToLeaderboardAsync)}");

                using (var client = new AmazonDynamoDBClient())
                {
                    using (var dbContext = new DynamoDBContext(client))
                    {
                        var request = new QueryRequest
                        {
                            TableName = "Leaderboard",
                            IndexName = "Game-Score-index",
                            KeyConditionExpression = "Game = :v_Game",
                            ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                                 { ":v_Game", new AttributeValue { S = gameName }},
                             },
                            ScanIndexForward = true,
                            Limit = limit
                        };
                        var queryResponse = client.QueryAsync(request).Result;
                        var datas = CreateLeaderBoardDatas(queryResponse);
                        return datas;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogLine(ex.ToString());
                throw;
            }
        }

    }
}
