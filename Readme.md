# Overview

This is a simple custom Alexa "magic number" skill wherein the user has to guess a magic number.
The skill uses touch enabled APL for enabling a voice and touch based experience.
A simple rudimentarty Leaderboard is also used to display top scores.

The Alexa model for this skill can be found in the Models folder

The backend of this skill is written in .NET and deployed as an AWS Lambda.
You will need to create an  Alexa Skill trigger after deploying the lambda to allow Alexa
to invoke your Lambda
You will also need to add the Lambda as an "endpoint" in your Skill configuration. This essentially
establishes a link between the skill frontend and backend.

The Leaderboard feature uses DynamoDB as a backend.

## Model
Contains the Alexa "front end" model for this skill.
## RandomGameSkill
The .NET core lambda backend logic containing the APL for reference and 
the DynamoDb backend schema for recreating the Leaderboard table
## RandomGameSkill.LeaderBaordSeeder
Console app to generate some seed data for the leaderboard

**NOTE:** You will need to create a role (RandomGameLambdarole) in AWS IAM that contains the 
following permissions:
- AWSLambdaBasicExecutionRole
- AmazonDynamoDBFullAccess (ideally needs only read/write, need to revisit)

**Role description**
```
{
    "Role": {
        "Path": "/",
        "RoleName": "RandomGameLambdarole",
        "RoleId": "xxxx",
        "Arn": "arn:aws:iam::121212121212:role/RandomGameLambdarole",
        "CreateDate": "2022-01-24T01:15:43+00:00",
        "AssumeRolePolicyDocument": {
            "Version": "2012-10-17",
            "Statement": [
                {
                    "Effect": "Allow",
                    "Principal": {
                        "Service": "lambda.amazonaws.com"
                    },
                    "Action": "sts:AssumeRole"
                }
            ]
        },
        "Description": "Allows Lambda functions to call AWS services on your behalf.",
        "MaxSessionDuration": 3600,
        "RoleLastUsed": {
            "LastUsedDate": "2022-02-13T20:05:35+00:00",
            "Region": "us-west-2"
        }
    }
}
```

**Attached role policies**
```
{
    "AttachedPolicies": [
        {
            "PolicyName": "AmazonDynamoDBFullAccess",
            "PolicyArn": "arn:aws:iam::aws:policy/AmazonDynamoDBFullAccess"
        },
        {
            "PolicyName": "AWSLambdaBasicExecutionRole",
            "PolicyArn": "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
        }
    ]
}
```
Eventually the role creation and the DynamoDB table creation would belong in a 
terraform template.


Feel free to use this as a reference/learning tool. 
I learnt a lot (and had fun) in doing so. Feedback welcome.