using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using Microsoft.ML.Data;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using SocialNetworkDashboard.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using static Microsoft.ML.DataOperationsCatalog;

namespace SocialNetworkDashboard.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TwitterController : ControllerBase
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private List<string> locationList = new List<string> { "Florida", "California", "texas", "Germany", "Ireland", "Spain", "London", "Japan", "Canada", "Newzland" };
        public TwitterController(IConfiguration config)
        {
            configuration = config;
            
        }

        private IRestResponse GetBearerToken()
        {
            var client = new RestClient("https://api.twitter.com/oauth2/token?grant_type=client_credentials");
            
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Basic ZUhzeE5RMEE5UGt5bkR0SHplUUtTVDRsRjpjZXh2VnQ0MUxwZlBMZW13ek9yVDdXUklDbWVlTkI4YWVWbHF4NmZWM0tnQnB6ZklyWg==");
          
            var response = client.Execute(request);
            return response;
        }

        [HttpGet("average")]
        public List<AverageSentimentModel> GetAllTweets()
        {
            var dataModelList = GetAllTweetData();
            var groupsList = dataModelList.GroupBy(m => m.Location).Select(tweet => tweet.ToList()).ToList();
            var mlContext = new MLContext();
            TrainTestData splitDataView = LoadData(mlContext, dataModelList);
            ITransformer model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);
            Evaluate(mlContext, model, splitDataView.TestSet);
            var finalResult = new List<AverageSentimentModel>();
            foreach (var groups in groupsList) { 
                var sentimentResult = UseModelWithSingleItem(mlContext, model, groups);
                var tweets = new List<string>();
                foreach (var result in sentimentResult)
                {
                    tweets.Add(result.Text);
                }
                var tempSentimentalModel = new AverageSentimentModel
                {
                    Tweets = tweets,
                    Location = sentimentResult[0].Location,
                    AverageSentimentValue = sentimentResult.Average(x => x.SentimentValue)
                };
                finalResult.Add(tempSentimentalModel);
            }
            return finalResult;
        }

        [HttpGet("individual")]
        public List<SentimentModel> GetIndividualSentimentValue()
        {
            var dataModelList = GetAllTweetData();
            var groupsList = dataModelList.GroupBy(m => m.Location).Select(tweet => tweet.ToList()).ToList();
            var mlContext = new MLContext();
            TrainTestData splitDataView = LoadData(mlContext, dataModelList);
            ITransformer model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);
            Evaluate(mlContext, model, splitDataView.TestSet);
            var finalResult = new List<SentimentModel>();
            foreach (var groups in groupsList)
            {
                var sentimentResult = UseModelWithSingleItem(mlContext, model, groups);
                finalResult.AddRange(sentimentResult);
            }
            return finalResult;
        }

        public static TrainTestData LoadData(MLContext mlContext, List<SentimentData> dataModelList)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(@"TrainDataSet\DataModel.txt", hasHeader: false);
            TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            return splitDataView;

        }

        public static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
        {
            var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
            var model = estimator.Fit(splitTrainSet);
           
            return model;
        }

        public static void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
        {
            IDataView predictions = model.Transform(splitTestSet);
            CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");
        }

        private static List<SentimentModel> UseModelWithSingleItem(MLContext mlContext, ITransformer model, List<SentimentData> dataModelList)
        {
            var sentimentDataResult = new List<SentimentModel>();
            PredictionEngine<SentimentData, SentimentPrediction> predictionFunction = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            foreach (var dataModel in dataModelList) {

                var resultPrediction = predictionFunction.Predict(dataModel);
                var tempSentimentModel = new SentimentModel
                {
                    Text = resultPrediction.SentimentText,
                    Location = resultPrediction.Location,
                    SentimentValue = resultPrediction.Probability
                };
                sentimentDataResult.Add(tempSentimentModel);
               
            }
            return sentimentDataResult;
        }
        public int GetRandom()
        {
            var random = new Random();
            return random.Next(0, 2);
        }   

        public int GetLocation()
        {
            
            var random = new Random();
             return random.Next(locationList.Count);
        }

        public List<SentimentData> GetAllTweetData()
        {
            var client = new RestClient("https://api.twitter.com/2/users/22853963/mentions?&expansions=geo.place_id&tweet.fields=geo&user.fields=location&place.fields=contained_within,country,country_code,full_name,geo,id,name,place_type");
            var bearerToken = GetBearerToken();
            var request = new RestRequest(Method.GET);

            var token = JsonConvert.DeserializeObject<Token>(bearerToken.Content);

            request.AddHeader("Authorization", "Bearer " + token.Access_Token);
            request.AddParameter("max_results", 10);

            var response = client.Execute(request);
            var content = JsonConvert.DeserializeObject<Root>(response.Content);
            var nextToken = content.meta.next_token;
            request.AddParameter("pagination_token", nextToken);
            List<Datum> tweetList = content.data;
            int retryCount = 5;
            for (int i = 0; i < retryCount; i++)
            {
                request.Parameters[2].Value = nextToken;
                var nextResponse = client.Execute(request);
                var nextContent = JsonConvert.DeserializeObject<Root>(nextResponse.Content);

                tweetList.AddRange(nextContent.data);
                nextToken = nextContent.meta.next_token;
            }

            var dataModelList = new List<SentimentData>();
            foreach (var tweet in tweetList)
            {
                var tempModel = new SentimentData
                {
                    SentimentText = tweet.text,
                    Location = locationList[GetLocation()],
                    Sentiment = Convert.ToBoolean(GetRandom())
                };

                dataModelList.Add(tempModel);
            }
            return dataModelList;
        }
    }

}
