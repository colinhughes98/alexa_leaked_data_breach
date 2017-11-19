using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace LambdaAlexa
{
    public class Function
    {

        public const string INVOCATION_NAME = "Leaked Data Checker";

        public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
        {

            var requestType = input.GetRequestType();
            if (requestType == typeof(IntentRequest))
            {
                var inputRequest = input.Request as IntentRequest;
                DataBreach db = new DataBreach();
                var email = inputRequest?.Intent.Slots["Email"].Value;

                var apiresponse = await db.CheckEMailInBreach(email);
                string speak;
                switch (apiresponse)
                {
                    case Codes.Yes:
                        speak = $"{email} has been in a databreach, change any passwords now!";
                        break;
                    case Codes.No:
                        speak = $"{email} has not been in a databreach";
                        break;
                    default:
                        speak = $"I'm sorry, there has been an exception. Please re-try";
                        break;
                }
                return MakeSkillResponse(
                    speak,
                    true);
            }
            return MakeSkillResponse(
                $"I don't know how to handle this intent. Please say something like Alexa, ask {INVOCATION_NAME} if colinhughes98@gmail.com address was in a breach.",
                true);
        }


        private SkillResponse MakeSkillResponse(string outputSpeech,
            bool shouldEndSession,
            string repromptText = "Just say, has colinhughes98@gmail has been in a breach to learn more. To exit, say, exit.")
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

            var skillResponse = new SkillResponse
            {
                Response = response,
                Version = "1.0"
            };
            return skillResponse;
        }
    }

    public class DataBreach
    {
        private readonly HttpClient request;

        public DataBreach()
        {
            request = new HttpClient();
        }
        public async Task<Codes> CheckEMailInBreach(string emailAddress)
        {

            request.DefaultRequestHeaders.Add("user-agent", "Leaked-Data-Chekcer");
            var resp = await request.GetAsync($"https://haveibeenpwned.com/api/v2/breachedaccount/{emailAddress}");
            switch (resp.StatusCode)
            {
                case HttpStatusCode.OK:
                    return Codes.Yes;
                case HttpStatusCode.NotFound:
                    return Codes.No;
                default:
                    return Codes.Error;
            }
        }
    }

    public enum Codes
    {
        Yes,
        No,
        Error
    }
}
