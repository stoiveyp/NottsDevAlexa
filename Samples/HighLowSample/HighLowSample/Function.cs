using System;
using System.Collections.Generic;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HighLowSample
{
    public class Function
    {
        private const string TurnIntent = "TakeTurn";

        private readonly string[] Cards = { "Ace", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King" };

        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            switch (input.Request)
            {
                case LaunchRequest launch:
                    return StartGame(input.Session);
                case IntentRequest intent:
                    switch (intent.Intent.Name)
                    {
                        case BuiltInIntent.Stop:
                        case BuiltInIntent.Cancel:
                            return EndGame(input.Session);
                        case BuiltInIntent.StartOver:
                            return StartGame(input.Session);
                        case TurnIntent:
                            return TakeTurn(intent.Intent, input.Session);
                    }
                    break;
            }

            return InvalidIntent();
        }

        private SkillResponse InvalidIntent()
        {
            var speech = new PlainTextOutputSpeech { Text = "Sorry, that's not part of the game. Try again?" };
            return ResponseBuilder.Ask(speech, null);
        }

        private SkillResponse TakeTurn(Intent intent, Session session)
        {
            var nextValue = new Random().Next(0, Cards.Length);
            var nextCard = Cards[nextValue];

            var actual = (int)session.Attributes["value"] < nextValue ? "higher" : "lower";
            var expected = intent.Slots["choice"].Value;

            if (actual != expected)
            {
                return WrongGuess(nextCard,(int)session.Attributes["count"]);
            }

            var currentCount = (int)session.Attributes["count"];
            session.Attributes["count"] = currentCount + 1;
            var speech = new PlainTextOutputSpeech {Text = $"Well done. It was a {nextCard}. So is the next card higher or lower than a  {nextCard}?"};
            return ResponseBuilder.AskWithCard(
                speech,
                "Correct Guess!",
                $"Well done. The next card is a {nextCard}",
                null,
                session);
        }

        private SkillResponse WrongGuess(string nextCard,int count)
        {
            var speech = new PlainTextOutputSpeech
            {
                Text = $"Sorry, but the card was a {nextCard}. You managed to get {count} correct guesses"
            };
            return ResponseBuilder.Tell(speech);
        }


        private SkillResponse EndGame(Session session)
        {
            var final = (int)session.Attributes["count"];
            var speech = new PlainTextOutputSpeech { Text = $"No problem. Your final score was {final} correct guesses" };
            return ResponseBuilder.TellWithCard(speech, "Final Score", $"{final} correct guesses");
        }

        private SkillResponse StartGame(Session session)
        {
            var cardValue = new Random().Next(0, Cards.Length);
            session.Attributes = new Dictionary<string, object>
            {
                {"count", 0},
                {"value",cardValue}
            };

            var speech = $"Let's begin. So we start your game with a {Cards[cardValue]}. Is the next card higher or lower?";
            var reminder = $"The question is, is the next card higher or lower than a {Cards[cardValue]}";

            return ResponseBuilder.Ask(
                new PlainTextOutputSpeech { Text = speech },
                new Reprompt { OutputSpeech = new PlainTextOutputSpeech { Text = reminder } },
                session);
        }
    }
}
