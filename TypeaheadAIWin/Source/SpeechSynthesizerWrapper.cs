using System.Speech.Synthesis;

namespace TypeaheadAIWin.Source
{
    public interface ISpeechSynthesizerWrapper
    {
        void SpeakAsync(string text);

        void SpeakAsyncCancelAll();
    }

    // Wrapper for SpeechSynthesizer to allow for easier testing
    public class SpeechSynthesizerWrapper : ISpeechSynthesizerWrapper
    {
        private readonly SpeechSynthesizer synthesizer;

        public SpeechSynthesizerWrapper()
        {
            synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();
        }

        public void SpeakAsync(string text)
        {
            synthesizer.SpeakAsync(text);
        }

        public void SpeakAsyncCancelAll()
        {
            synthesizer.SpeakAsyncCancelAll();
        }
    }
}
