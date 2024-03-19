using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeaheadAIWin.Source.Model.Accessibility
{
    public class AXUIElement
    {
        public string AutomationId { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string ClassName { get; set; }
        public string ControlType { get; set; }
        public List<string> Patterns { get; set; }
        public List<AXUIElement> Children { get; set; } = new List<AXUIElement>();
    }
}