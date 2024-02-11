using System.Text;

namespace TypeaheadAIWin.Source.Speech
{
    public class StreamingSpeechProcessor
    {
        private StringBuilder buffer = new StringBuilder();
        private string[] delimiters = new string[] { ". ", "! ", "? ", "\n" }; // Updated delimiters
        private ISpeechSynthesizerWrapper synthesizer;

        public StreamingSpeechProcessor(ISpeechSynthesizerWrapper speechSynthesizer)
        {
            synthesizer = speechSynthesizer;
        }

        public void ProcessToken(string token)
        {
            buffer.Append(token);
            CheckAndProcessBuffer();
        }

        private void CheckAndProcessBuffer()
        {
            int lastDelimiterIndex = FindLastDelimiter();
            if (lastDelimiterIndex != -1)
            {
                string textToRead = buffer.ToString(0, lastDelimiterIndex);
                synthesizer.SpeakAsync(textToRead);

                buffer.Remove(0, lastDelimiterIndex);
            }
        }

        private int FindLastDelimiter()
        {
            int lastIndex = -1;
            foreach (var delimiter in delimiters)
            {
                int index = buffer.ToString().LastIndexOf(delimiter);
                if (index > lastIndex)
                {
                    lastIndex = index + delimiter.Length;  // Offset by length of delimiter
                }
            }
            return lastIndex;
        }

        public void FlushBuffer()
        {
            if (buffer.Length > 0)
            {
                synthesizer.SpeakAsync(buffer.ToString());
                buffer.Clear();
            }
        }

        public void Cancel()
        {
            synthesizer.SpeakAsyncCancelAll();
            buffer.Clear();
        }
    }
}