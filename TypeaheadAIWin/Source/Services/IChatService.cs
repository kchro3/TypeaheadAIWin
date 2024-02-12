using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeaheadAIWin.Source.Services
{
    public interface IChatService
    {
        Task StreamChatAsync(ChatRequest chatRequest, CancellationToken cancellationToken);

        event EventHandler<ChatResponse> OnChatResponseReceived;
    }
}
