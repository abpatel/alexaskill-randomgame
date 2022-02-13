using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.CredentialManagement;


namespace RandomeGameSkill.LeaderBoardSeeder
{
    class Program
    {
      
        private static Random random = new Random(Guid.NewGuid().GetHashCode());
       
        static void Main(string[] args)
        {
            Console.WriteLine("Connecting...");
            var sharedFile = new SharedCredentialsFile();
            // replace profile name with your own shared credentials
            sharedFile.TryGetProfile("default", out var profile);
            if (AWSCredentialsFactory.TryGetAWSCredentials(
                profile, sharedFile, out var credentials))
            {
                SeedLeaderBoard();
            }
            else
            {
                Console.WriteLine("Unable to find credentials... exiting");
            }
        }

        public static string[] GetRandomNames(int count)
        {
            var names = File.ReadAllLines("names.txt");
            var randomNames = Enumerable.Range(0, count)
                .Select(i => names[random.Next(names.Length)])
                .ToArray();
            return names;
        }

        private static void SeedLeaderBoard()
        {
            const int numUsers = 20;
            var userNames = GetRandomNames(numUsers);
            //Change region as needed
            using (var client = new AmazonDynamoDBClient(RegionEndpoint.USWest2))
            {
                using (var dbContext = new DynamoDBContext(client))
                {
                    Table table = Table.LoadTable(client, "Leaderboard");
                    foreach (var userName in userNames)
                    {
                        int score = random.Next(1, 9);
                        var entry = new Document();
                        entry["Username"] = userName;
                        entry["Game"] = "Random Numbers Game";
                        entry["Score"] = score;
                        entry["Timestamp"] = DateTime.UtcNow;
                        Console.WriteLine($"Adding:{userName}, Score={score}");
                        _ = table.PutItemAsync(entry).Result;
                    }
                }
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
