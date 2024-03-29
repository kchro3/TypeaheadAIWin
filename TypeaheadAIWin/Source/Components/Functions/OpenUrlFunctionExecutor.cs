using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeaheadAIWin.Source.Model;
using TypeaheadAIWin.Source.Model.Functions;

namespace TypeaheadAIWin.Source.Components.Functions
{
    public class OpenUrlFunctionExecutor : IFunctionExecutor<OpenUrlFunctionArgs>
    {
        public void ExecuteFunction(OpenUrlFunctionArgs args, ApplicationContext appContext)
        {
            var psi = new ProcessStartInfo
            {
                FileName = args.Url,
                UseShellExecute = true // Necessary for .NET Core
            };

            Process.Start(psi);

            Thread.Sleep(5000);
        }
    }
}
