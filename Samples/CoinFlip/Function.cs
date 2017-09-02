using System;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace CoinFlip
{
    public class Function
    {
        public SkillResponse FunctionHandler(SkillRequest request, ILambdaContext context)
        {
            if (request.GetRequestType() != typeof(LaunchRequest))
            {
                return ResponseBuilder.Empty();
            }

            var result = new Random().Next(0, 2) == 0 ? "heads" : "tails";
            var resultText = $"Certainly. And the result is. {result}";

            var speech = new PlainTextOutputSpeech {Text = resultText};

            return ResponseBuilder.Tell(speech);
        }
    }
}
