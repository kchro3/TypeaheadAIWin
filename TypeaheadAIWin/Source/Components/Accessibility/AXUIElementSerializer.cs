using System.Diagnostics;
using System.Text;
using TypeaheadAIWin.Source.Model.Accessibility;
using UIAutomationClient;

namespace TypeaheadAIWin.Source.Components.Accessibility
{
    public class AXUIElementSerializer
    {
        private IUIAutomation _automation = GetUIAutomationInterface();

        // Change the signature to return Task<string> for async support
        public Task<AXUIState> SerializeAsync(nint hWnd)
        {
            // Run the serialization process on a background thread
            return Task.Run(() =>
            {
                var stopwatch = Stopwatch.StartNew(); // Start the stopwatch at the beginning of the task

                var result = Serialize(_automation.ElementFromHandle(hWnd));

                stopwatch.Stop(); // Stop the stopwatch when serialization is done
                Trace.WriteLine($"Serialization took {stopwatch.ElapsedMilliseconds} milliseconds.");

                return result;
            });
        }

        private AXUIState Serialize(IUIAutomationElement root)
        {
            var elementReferences = new Dictionary<string, IUIAutomationElement>();

            int nextId = 0;
            var walker = _automation.RawViewWalker;
            var elementStack = new Stack<Tuple<IUIAutomationElement, AXUIElement, int>>();
            var rootUIElement = ToAXUIElement(root, nextId);
            elementStack.Push(new Tuple<IUIAutomationElement, AXUIElement, int>(root, rootUIElement, 0));

            elementReferences.Add(rootUIElement.ShortId(), root);

            var stringBuilder = new StringBuilder();
            while (elementStack.Count > 0)
            {
                var current = elementStack.Pop();
                var element = current.Item1;
                var parentInfo = current.Item2;
                var currentDepth = current.Item3;

                if (element == null) continue;

                var uiElement = ToAXUIElement(element, ++nextId);
                var serializedUIElement = uiElement.ToString();
                stringBuilder.AppendLine(new string(' ', currentDepth * 2) + serializedUIElement);

                parentInfo.Children.Add(uiElement);
                elementReferences.Add(uiElement.ShortId(), element);

                // Instead of enqueueing children directly, use a temporary stack to reverse the order
                // of children before pushing them onto the main stack. This ensures we process the
                // leftmost child next, maintaining a DFS order.
                var tempStack = new Stack<IUIAutomationElement>();
                var child = walker.GetFirstChildElement(element);
                while (child != null)
                {
                    tempStack.Push(child);
                    child = walker.GetNextSiblingElement(child);
                }

                // Now push children onto the main stack in reverse order
                while (tempStack.Count > 0)
                {
                    var tempChild = tempStack.Pop();
                    elementStack.Push(new Tuple<IUIAutomationElement, AXUIElement, int>(tempChild, uiElement, currentDepth + 1));
                }
            }

            return new AXUIState
            {
                SerializedUIElement = stringBuilder.ToString(),
                ElementReferences = elementReferences
            };
        }

        private AXUIElement ToAXUIElement(IUIAutomationElement element, int id)
        {
            var controlType = element.CurrentLocalizedControlType;
            var name = element.CurrentName;
            var label = element.CurrentHelpText;
            var className = element.CurrentClassName;
            var ariaRole = element.CurrentAriaRole;
            
            return new AXUIElement
            {
                Id = id,
                ControlType = controlType,
                Name = name,
                Label = label,
                ClassName = className,
                AriaRole = ariaRole
            };
        }

        private static IUIAutomation GetUIAutomationInterface()
        {
            var isWindows7 = Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1;
            return isWindows7
                ? new CUIAutomation() as IUIAutomation
                : new CUIAutomation8() as IUIAutomation;
        }
    }
}
