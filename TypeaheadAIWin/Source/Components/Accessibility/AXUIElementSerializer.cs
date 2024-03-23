using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using TypeaheadAIWin.Source.Model.Accessibility;

namespace TypeaheadAIWin.Source.Components.Accessibility
{
    public class AXUIElementSerializer
    {
        private int nextId = 0;

        // Change the signature to return Task<string> for async support
        public Task<string> SerializeAsync(AutomationElement root)
        {
            // Run the serialization process on a background thread
            return Task.Run(() =>
            {
                var stopwatch = Stopwatch.StartNew(); // Start the stopwatch at the beginning of the task

                var result = Serialize(root);

                stopwatch.Stop(); // Stop the stopwatch when serialization is done
                Trace.WriteLine($"Serialization took {stopwatch.ElapsedMilliseconds} milliseconds.");
                Trace.WriteLine(result);

                return result;
            });
        }

        public string Serialize(AutomationElement root)
        {
            var walker = TreeWalker.ControlViewWalker;
            var elementQueue = new Queue<Tuple<AutomationElement, AXUIElement, int>>();
            var rootInfo = new AXUIElement { AutomationId = "root", Children = new List<AXUIElement>() };
            elementQueue.Enqueue(new Tuple<AutomationElement, AXUIElement, int>(root, rootInfo, 0));

            while (elementQueue.Count > 0)
            {
                var current = elementQueue.Dequeue();
                var element = current.Item1;
                var parentInfo = current.Item2;
                var currentDepth = current.Item3;

                if (element == null) continue;

                bool isOffscreen = (bool)element.GetCurrentPropertyValue(AutomationElement.IsOffscreenProperty);
                if (!isOffscreen)
                {
                    var elementInfo = GetElementDetails(element);
                    parentInfo.Children.Add(elementInfo);

                    var child = walker.GetFirstChild(element);
                    while (child != null)
                    {
                        elementQueue.Enqueue(new Tuple<AutomationElement, AXUIElement, int>(child, elementInfo, currentDepth + 1));
                        child = walker.GetNextSibling(child);
                    }
                }
            }

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore // Ignore null fields during serialization
            };

            return JsonConvert.SerializeObject(rootInfo, settings);
        }

        private AXUIElement GetElementDetails(AutomationElement element)
        {
            var controlType = element.Current.ControlType.ProgrammaticName;
            var automationId = controlType + $"{++nextId}";
            var name = element.Current.Name;
            var label = element.Current.HelpText;
            var className = element.Current.ClassName;
            var patterns = GetSupportedPatterns(element);

            return new AXUIElement
            {
                AutomationId = automationId,
                Name = name,
                Label = label,
                ClassName = className,
                ControlType = controlType,
                Patterns = patterns
            };
        }

        private List<string> GetSupportedPatterns(AutomationElement element)
        {
            var patterns = new List<string>();
            foreach (var pattern in element.GetSupportedPatterns())
            {
                patterns.Add(pattern.ProgrammaticName);
            }
            return patterns;
        }
    }
}
