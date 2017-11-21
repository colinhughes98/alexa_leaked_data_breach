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
        protected string InvocationName { get; set; }

        protected string RepromptMessage { get; set; }

        protected virtual async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            var logger = context.Logger;

            try
            {
                var requestType = input.GetRequestType();
                if (requestType == typeof(IntentRequest))
                {
                    var inputRequest = input.Request as IntentRequest;
                    
                   var response = await DoWork(inputRequest?.Intent.Slots);
                    return MakeSkillResponse(
                        response,
                        true);
                }
                return MakeSkillResponse(
                    $"I don't know how to handle this intent. Please say something like Alexa, ask {InvocationName} if {RepromptMessage}.",
                    true);
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
                return MakeSkillResponse("Error", true);
            }
        }
        
        protected SkillResponse MakeSkillResponse(string outputSpeech,
           bool shouldEndSession)
        {
            var response = new ResponseBody
            {
                ShouldEndSession = shouldEndSession,
                OutputSpeech = new PlainTextOutputSpeech { Text = outputSpeech }
            };

            //if (repromptText != null)
            //{
            //    response.Reprompt = new Reprompt() { OutputSpeech = new PlainTextOutputSpeech() { Text = repromptText } };
            //}

            var skillResponse = new SkillResponse()
            {
                Response = response,
                Version = "1.0"
            };
            return skillResponse;
        }

        //protected abstract Task<SkillResponse> DoWork(IDictionary<string, Slot> slots);
        protected abstract Task<string> DoWork(IDictionary<string, Slot> slots);
    }

 }
