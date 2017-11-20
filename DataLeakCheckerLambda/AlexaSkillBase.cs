using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET.Response;

namespace DataLeakCheckerLambda
{
    public abstract class AlexaSkillBase
    {
        protected SkillResponse MakeSkillResponse(string outputSpeech,
           bool shouldEndSession,
           string repromptText = null)
        {
            var response = new ResponseBody
            {
                ShouldEndSession = shouldEndSession,
                OutputSpeech = new PlainTextOutputSpeech { Text = outputSpeech }
            };

            if (repromptText != null)
            {
                response.Reprompt = new Reprompt() { OutputSpeech = new PlainTextOutputSpeech() { Text = repromptText } };
            }

            var skillResponse = new SkillResponse()
            {
                Response = response,
                Version = "1.0"
            };
            return skillResponse;
        }
    }
}
