using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RandomGameSkill.Repositories
{
    class LeaderBoardRepo
    {
        private readonly ILambdaLogger logger;

        public LeaderBoardRepo(ILambdaLogger logger)
        {
            this.logger = logger;
        }
        private void WriteToLeaderboard(string userName,
            int numTries)
        {
            Task.Run(async () => await WriteToLeaderboardAsync(userName, numTries))
                .Wait();

        }
        private async Task WriteToLeaderboardAsync(string userName, int numTries)
        {
            try
            {
                logger.LogLine("Entered WriteToLeaderboardAsync");

                using (var client = new AmazonDynamoDBClient())
                {
                    using (var dbContext = new DynamoDBContext(client))
                    {
                        Table table = Table.LoadTable(client, "Leaderboard");
                        var doc = new Document();
                        doc["Username"] = userName;
                        doc["Game"] = "Numbers Game";
                        doc["Score"] = numTries;
                        await table.PutItemAsync(doc);
                        logger.LogLine("Exiting WriteToLeaderboardAsync");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogLine(ex.ToString());
            }
        }

        //    public void FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
        //    {
        //        context.Logger.LogLine($"Beginning to process {dynamoEvent.Records.Count} records...");

        //        foreach (var record in dynamoEvent.Records)
        //        {
        //            context.Logger.LogLine($"Event ID: {record.EventID}");
        //            context.Logger.LogLine($"Event Name: {record.EventName}");

        //// TODO: Add business logic processing the record.Dynamodb object.
        //        }

        //        context.Logger.LogLine("Stream processing complete.");
        //    }
    }
}
