using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeaheadAIWin.Source.Components.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeaheadAIWin.Source.Model.Functions;

namespace TypeaheadAIWin.Source.Components.Functions.Tests
{
    [TestClass()]
    public class FunctionCallerTests
    {
        [TestMethod()]
        public void FunctionCallerTest()
        {
            // Arrange
            var jsonString = @"{
                ""Id"": ""abc123"",
                ""Name"": ""open_url"",
                ""Args"": {
                    ""url"": ""https://example.com""
                }
            }";
            var functionCaller = new FunctionCaller();

            // Act
            var functionCall = functionCaller.Parse(jsonString);
            var openUrlFunctionArgs = functionCall.ParseArgs() as OpenUrlFunctionArgs;

            // Assert
            Assert.IsNotNull(functionCall);
            Assert.IsNotNull(openUrlFunctionArgs);
            Assert.AreEqual("https://example.com", openUrlFunctionArgs.Url);
            Assert.AreEqual("Opening https://example.com and waiting for 5 seconds for the page to load...", openUrlFunctionArgs.HumanReadable);
        }
    }
}