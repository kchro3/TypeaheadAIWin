using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TypeaheadAIWin.Source.Model.Functions
{
    public enum FunctionName
    {
        open_url // Add other function names as needed
    }

    public class FunctionCall
    {
        public string Id { get; set; }
        public FunctionName Name { get; set; }
        public Dictionary<string, JsonElement> Args { get; set; }

        // Parses the specific argument from the Args dictionary
        private string GetStringArg(string argName)
        {
            if (Args.TryGetValue(argName, out var value) && value.ValueKind == JsonValueKind.String)
            {
                return value.GetString();
            }
            return null;
        }

        public IFunctionArgs ParseArgs()
        {
            switch (Name)
            {
                case FunctionName.open_url:
                    var url = GetStringArg("url");
                    return new OpenUrlFunctionArgs
                    {
                        Url = url,
                        HumanReadable = $"Opening {url} and waiting for 5 seconds for the page to load..."
                    };
                default:
                    throw new NotImplementedException($"Function {Name} is not implemented");
            }
        }
    }
}
