using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeaheadAIWin.Source.Model;
using TypeaheadAIWin.Source.Model.Functions;

namespace TypeaheadAIWin.Source.Components.Functions
{
    public interface IFunctionExecutor<in T> where T : IFunctionArgs
    {
        void ExecuteFunction(T args, ApplicationContext appContext);
    }
}
