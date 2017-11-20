using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;

namespace DataLeakCheckerLambda
{

    public abstract class AlexaSkillBase
    {
        protected const string INVOCATION_NAME = "Leaked Data Checker";

        protected virtual async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            var logger = context.Logger;

            try
            {
                var requestType = input.GetRequestType();
                if (requestType == typeof(IntentRequest))
                {
                    var inputRequest = input.Request as IntentRequest;
                    
                    

                    var a = await DoWork(inputRequest?.Intent.Slots);


                    //below- remove
                    //var email = inputRequest?.Intent.Slots["Email"].Value;
                    //var final = Sanitize(email);
                    //if (!string.IsNullOrEmpty(final))
                    //{
                    //    DataBreach db = new DataBreach();

                    //    var apiresponse = await db.CheckEMailInBreach(final);
                    //    string speak;
                    //    switch (apiresponse)
                    //    {
                    //        case Codes.Yes:
                    //            speak = $"{final} has been in a databreach, change any passwords now!";
                    //            break;
                    //        case Codes.No:
                    //            speak = $"{final} has not been in a databreach";
                    //            break;
                    //        default:
                    //            speak = $"I'm sorry, there has been an exception. Please re-try";
                    //            break;
                    //    }

                    //    return MakeSkillResponse(
                    //        speak,
                    //        true);
                    //}
                }
                return MakeSkillResponse(
                    $"I don't know how to handle this intent. Please say something like Alexa, ask {INVOCATION_NAME} if colinhughes98@gmail.com address was in a breach.",
                    true);
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
                return MakeSkillResponse("Error", true);
            }
        }
        
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

        protected abstract Task<SkillResponse> DoWork(IDictionary<string, Slot> slots);
    }

 }
