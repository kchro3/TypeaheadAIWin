using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeaheadAIWin.Source.Model.Functions
{
    public class OpenUrlFunctionArgs : IFunctionArgs
    {
        public string HumanReadable { get; set; }

        public string Url { get; set; }
    }
}
