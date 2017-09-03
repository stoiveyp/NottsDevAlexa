using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace DoorNumberThree
{
    public class Function
    {
        private const string TokenAttribute = "tokens";

        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            if (input.GetRequestType() == typeof(LaunchRequest))
            {
                input.Session.Attributes = new Dictionary<string, object> { { TokenAttribute, 5 } };
                return LaunchReply();
            }

            if (input.GetRequestType() == typeof(IntentRequest))
            {
                var intentRequest = (IntentRequest)input.Request;
                return HandleIntent(intentRequest.Intent, input.Session);
            }

            return ResponseBuilder.Empty();
        }

        private SkillResponse HandleIntent(Intent intent, Session session)
        {
            switch (intent.Name)
            {
                case BuiltInIntent.Stop:
                case BuiltInIntent.Cancel:
                    return FinalScore(session);
                default:
                    const string sorryText = "That's not part of the game. Please try again";
                    return ResponseBuilder.Ask(new PlainTextOutputSpeech { Text = sorryText }, null);
            }
        }

        private SkillResponse FinalScore(Session session)
        {
            var amount = (int)session.Attributes[TokenAttribute];
            var text = $"Thank you for playing. Your final score was {amount} tokens";
            var speech = new PlainTextOutputSpeech { Text = text };
            return ResponseBuilder.Tell(speech);
        }

        private SkillResponse LaunchReply()
        {
            var text = "Three doors, only one wins the prize. Which do you pick?";
            var speech = new PlainTextOutputSpeech { Text = text };
            return ResponseBuilder.Ask(speech, null);
        }
    }
}
