using System.Windows.Automation;

namespace TypeaheadAIWin.Source.Model
{
    public class ApplicationContext
    {
        public required string AppName { get; set; }
        public required string ProcessName { get; set; }
        public required uint Pid { get; set; }
        public required AutomationElement CurrentWindow { get; set; }
        public string SerializedAppState { get; set; }
    }
}
