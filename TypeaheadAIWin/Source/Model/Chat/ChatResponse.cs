using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeaheadAIWin.Source.Model.Chat
{
    public class ChatResponse
    {
        public string Text { get; set; }
        public ResponseMode? Mode { get; set; }
        public string FinishReason { get; set; }
    }
}
