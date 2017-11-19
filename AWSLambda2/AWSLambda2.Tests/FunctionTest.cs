using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using AWSLambda2;
using LambdaAlexa;

namespace AWSLambda2.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestToUpperFunction()
        {

            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();

            IntentRequest req = new IntentRequest();
            req.Intent = new Intent();
            req.Intent.Slots = new Dictionary<string, Slot>();
            req.Intent.Slots.Add("Email", new Slot() {Name = "Email", Value = "colinhughes98@gmail.com"});
            var upperCase = function.FunctionHandler(new SkillRequest(), context);

           // Assert.Equal("HELLO WORLD", upperCase);
        }
    }
}
