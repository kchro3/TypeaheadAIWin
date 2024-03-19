using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace TypeaheadAIWin.Source.Components.Accessibility
{
    public class AXInvoker
    {
        public void InvokeElement(AutomationElement element)
        {
            // Check if the element supports the InvokePattern.
            if (element.TryGetCurrentPattern(InvokePattern.Pattern, out object pattern))
            {
                var invokePattern = (InvokePattern)pattern;

                // Perform the click action.
                invokePattern.Invoke();
            }
            else
            {
                // The element does not support the InvokePattern - handle accordingly.
                Console.WriteLine("Element does not support InvokePattern.");
            }
        }
    }
}
