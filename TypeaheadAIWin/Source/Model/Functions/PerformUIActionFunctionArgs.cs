using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeaheadAIWin.Source.Model.Functions
{
    public class PerformUIActionFunctionArgs : IFunctionArgs
    {
        public string HumanReadable { get; set; }

        public string Id { get; set; }

        public string Narration { get; set; }

        public string InputText { get; set; }

        public bool PressEnter { get; set; }
    }
}