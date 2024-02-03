using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Media;

namespace TypeaheadAIWin
{
    public class ChatMessage
    {
        public string Text { get; set; }
        public ImageSource Image { get; set; }
        public AutomationElement FocusedElement { get; set; }
    }
}
