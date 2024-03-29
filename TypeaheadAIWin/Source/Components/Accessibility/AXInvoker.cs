using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using UIAutomationClient;

namespace TypeaheadAIWin.Source.Components.Accessibility
{
    public class AXInvoker
    {
        // Define the Invoke pattern ID
        private static readonly int UIA_InvokePatternId = 10000; // UIA_InvokePatternId

        public void InvokeElement(IUIAutomationElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element), "The UI Automation element cannot be null.");
            }

            // Check if the element supports the Invoke pattern
            var invokePatternObj = element.GetCurrentPattern(UIA_InvokePatternId);
            if (invokePatternObj != null)
            {
                // Cast the returned object to IUIAutomationInvokePattern
                var invokePattern = (IUIAutomationInvokePattern)invokePatternObj;

                // Invoke the pattern's action
                invokePattern.Invoke();
            }
            else
            {
                throw new InvalidOperationException("The specified element does not support the Invoke pattern.");
            }
        }
    }
}
