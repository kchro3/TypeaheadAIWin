using TypeaheadAIWin.Source.Model;

namespace TypeaheadAIWin.Source
{
    public class ChatRequest
    {
        public string Uuid { get; set; }

        public ApplicationContext AppContext { get; set; }

        public List<ChatMessage> Messages { get; set; }
    }
}
