using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace TypeaheadAIWin.Source.Components
{
    public class AXUIElementMapper
    {
        private Dictionary<string, AutomationElement> elementMap = new Dictionary<string, AutomationElement>();
        private int nextId = 0;

        public void PrintActionableElements(AutomationElement root)
        {
            var walker = TreeWalker.ControlViewWalker;
            var elementQueue = new Queue<AutomationElement>();
            elementQueue.Enqueue(root);

            while (elementQueue.Count > 0)
            {
                var element = elementQueue.Dequeue();
                if (element == null) continue;

                // Check if the element is visible
                bool isOffscreen = (bool)element.GetCurrentPropertyValue(AutomationElement.IsOffscreenProperty);
                //if (!isOffscreen && IsElementActionable(element))
                if (!isOffscreen)
                {
                    PrintElementDetails(element);
                }

                var child = walker.GetFirstChild(element);
                while (child != null)
                {
                    elementQueue.Enqueue(child);
                    child = walker.GetNextSibling(child);
                }
            }
        }

        private static bool IsElementActionable(AutomationElement element)
        {
            return (bool)element.GetCurrentPropertyValue(AutomationElement.IsInvokePatternAvailableProperty);
        }

        private void PrintElementDetails(AutomationElement element)
        {
            var controlType = element.Current.ControlType.ProgrammaticName;
            var automationId = controlType + $"{++nextId}";
            var name = element.Current.Name;
            var label = element.Current.HelpText;
            var className = element.Current.ClassName;
            Trace.WriteLine($"{automationId}, Name: {name}, Label: {label}, Class Name: {className}");

            // Optionally, print available patterns for the element
            PrintAvailablePatterns(element);
        }

        private static void PrintAvailablePatterns(AutomationElement element)
        {
            var patternIds = element.GetSupportedPatterns();
            foreach (var patternId in patternIds)
            {
                Trace.WriteLine($" - Supports Pattern: {patternId.ProgrammaticName}");
            }
        }
    }
}
