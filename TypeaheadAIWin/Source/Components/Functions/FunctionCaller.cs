using System.Text.Json.Serialization;
using System.Text.Json;
using TypeaheadAIWin.Source.Model.Functions;
using TypeaheadAIWin.Source.Model;
using TypeaheadAIWin.Source.Accessibility;

namespace TypeaheadAIWin.Source.Components.Functions
{
    public class FunctionCaller
    {
        private readonly OpenUrlFunctionExecutor _openUrlFunctionExecutor;
        private readonly PerformUIActionFunctionExecutor _performUIElementFunctionExecutor;
        private readonly JsonSerializerOptions _options;

        public FunctionCaller(
            AXInspector axInspector,
            OpenUrlFunctionExecutor openUrlFunctionExecutor,
            PerformUIActionFunctionExecutor performUIActionFunctionExecutor)
        {
            _openUrlFunctionExecutor = openUrlFunctionExecutor;
            _performUIElementFunctionExecutor = performUIActionFunctionExecutor;

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

        public void Call(FunctionCall functionCall, ApplicationContext appContext)
        {
            switch (functionCall.Name)
            {
                case FunctionName.open_url:
                    var openUrlFunctionArgs = functionCall.ParseArgs() as OpenUrlFunctionArgs;
                    _openUrlFunctionExecutor.ExecuteFunction(openUrlFunctionArgs, appContext);
                    break;
                case FunctionName.perform_ui_action:
                    var performUIActionFunctionArgs = functionCall.ParseArgs() as PerformUIActionFunctionArgs;
                    _performUIElementFunctionExecutor.ExecuteFunction(performUIActionFunctionArgs, appContext);
                    break;
                default:
                    throw new NotImplementedException($"Function {functionCall.Name} is not implemented");
            }
        }
    }
}
