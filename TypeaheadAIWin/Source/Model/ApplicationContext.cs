using System.Windows.Automation;
using UIAutomationClient;

namespace TypeaheadAIWin.Source.Model
{
    public class ApplicationContext
    {
        public required string AppName { get; set; }
        public required string ProcessName { get; set; }
        public required uint Pid { get; set; }
        public string SerializedUIElement { get; set; }
        public Dictionary<string, IUIAutomationElement> ElementReferences { get; set; }
    }
}
