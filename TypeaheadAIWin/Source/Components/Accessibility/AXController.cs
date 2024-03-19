using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeaheadAIWin.Source.Components.Accessibility
{
    public class AXController
    {
        private readonly AXInvoker _axInvoker;

        public AXController(AXInvoker axInvoker)
        {
            _axInvoker = axInvoker;
        }

        /// <summary>
        /// Take a snapshot of the current application state
        /// </summary>
        public void TakeSnapshot()
        {

        }

        /// <summary>
        /// Perform an action on some element
        /// </summary>
        /// <param name="action"></param>
        /// <param name="elementId"></param>
        public void PerformAction(string action, string elementId)
        {

        }
    }
}
