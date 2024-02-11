using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Text.Json;

namespace TypeaheadAIWin.Source.Accessibility
{
    class AXElementVisitor
    {
        public List<UIElementInfo> Elements { get; set; }

        public AXElementVisitor()
        {
            Elements = new List<UIElementInfo>();
            CaptureSnapshot(AutomationElement.RootElement);
        }

        private void CaptureSnapshot(AutomationElement element)
        {
            if (element == null)
            {
                return;
            }

            if (element.Current.ControlType.ProgrammaticName != "ControlType.Custom")
            {
                Elements.Add(new UIElementInfo
                {
                    Name = element.Current.Name,
                    ControlType = element.Current.ControlType.ProgrammaticName,
                    AutomationId = element.Current.AutomationId,
                    ClassName = element.Current.ClassName,
                    ProcessId = element.Current.ProcessId,
                    BoundingRectangle = element.Current.BoundingRectangle
                });
            }

            foreach (AutomationElement child in element.FindAll(TreeScope.Children, Condition.TrueCondition))
            {
                CaptureSnapshot(child);
            }
        }

        public string SerializeElements()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
            };
            return JsonSerializer.Serialize(Elements, options);
        }
    }

    public class UIElementInfo
    {
        public string Name { get; set; }
        public string ControlType { get; set; }
        public string AutomationId { get; set; }
        public string ClassName { get; set; }
        public int ProcessId { get; set; }
        public System.Windows.Rect BoundingRectangle { get; set; }
    }
}
