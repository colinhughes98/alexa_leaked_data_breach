using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

namespace DataLeakCheckerLambda
{
    public class Function : AlexaSkillBase
    {
                 

        protected override async Task<string> DoWork(IDictionary<string, Slot> slots)
        {
            InvocationName = "Leaked Data Checker";
            DataBreach db = new DataBreach();
            var email = slots["Email"].Value;
            var final = Sanitize(email);
            var speak = "I'm sorry, there has been an exception. Please re-try";
            if (!string.IsNullOrEmpty(final))
            {
                var apiresponse = await db.CheckEMailInBreach(final);
                
                switch (apiresponse)
                {
                    case Codes.Yes:
                        speak = $"{final} has been in a databreach, change any passwords now!";
                        break;
                    case Codes.No:
                        speak = $"{final} has not been in a databreach";
                        break;
                    default:
                        speak = $"I'm sorry, there has been an error from Troy's web service. Please re-try";
                        break;
                }            
            }
            return speak;           
        }

        private static string Sanitize(string email)
        {
            if (string.IsNullOrEmpty(email)) return string.Empty;

            email = email.ToUpper().Replace("AT", "@").Replace("DOT", ".");
            var words = email.Split(' ');

            var final = string.Join("", words);
            return final;
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
