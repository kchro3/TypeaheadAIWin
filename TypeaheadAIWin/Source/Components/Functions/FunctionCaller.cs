using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using TypeaheadAIWin.Source.Model.Functions;
using TypeaheadAIWin.Source.Model;
using TypeaheadAIWin.Source.Accessibility;

namespace TypeaheadAIWin.Source.Components.Functions
{
    public class FunctionCaller
    {
        private readonly AXInspector _axInspector;
        private readonly OpenUrlFunctionExecutor _openUrlFunctionExecutor;
        private readonly JsonSerializerOptions _options;

        public FunctionCaller(
            AXInspector axInspector,
            OpenUrlFunctionExecutor openUrlFunctionExecutor)
        {
            _axInspector = axInspector;
            _openUrlFunctionExecutor = openUrlFunctionExecutor;

            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
        }

        public FunctionCall Parse(string jsonString)
        {
            var functionCall = JsonSerializer.Deserialize<FunctionCall>(jsonString, _options);
            if (functionCall == null)
            {
                throw new JsonException("Failed to parse function call");
            }
            return functionCall;
        }

        public void Call(FunctionCall functionCall)
        {
            switch (functionCall.Name)
            {
                case FunctionName.open_url:
                    var openUrlFunctionArgs = functionCall.ParseArgs() as OpenUrlFunctionArgs;
                    _openUrlFunctionExecutor.ExecuteFunction(openUrlFunctionArgs);
                    break;
                default:
                    throw new NotImplementedException($"Function {functionCall.Name} is not implemented");
            }
        }
    }
}
