using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeaheadAIWin.Source.Components.Accessibility;
using TypeaheadAIWin.Source.Model;
using TypeaheadAIWin.Source.Model.Functions;

namespace TypeaheadAIWin.Source.Components.Functions
{
    public class PerformUIActionFunctionExecutor : IFunctionExecutor<PerformUIActionFunctionArgs>
    {
        private readonly AXInvoker _axInvoker;

        public PerformUIActionFunctionExecutor(AXInvoker axInvoker)
        {
            _axInvoker = axInvoker;
        }

        public void ExecuteFunction(PerformUIActionFunctionArgs args, ApplicationContext appContext)
        {
            Trace.WriteLine($"Performing UI action: {args.HumanReadable} ({args.Id})");
            if (appContext.ElementReferences.ContainsKey(args.Id) == false)
            {
                Trace.WriteLine($"Element with id {args.Id} not found in context");
                return;
            }

            var element = appContext.ElementReferences[args.Id];
            _axInvoker.InvokeElement(element);

            Thread.Sleep(3000);
        }
    }
}
