using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIAutomationClient;

namespace TypeaheadAIWin.Source.Model.Accessibility
{
    public class AXUIState
    {
        public string SerializedUIElement { get; set; }
        public Dictionary<string, IUIAutomationElement> ElementReferences { get; set; }
    }
}
