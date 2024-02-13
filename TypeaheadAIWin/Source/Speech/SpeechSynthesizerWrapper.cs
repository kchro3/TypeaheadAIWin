using System.Speech.Synthesis;
using TypeaheadAIWin.Source.Model;

namespace TypeaheadAIWin.Source.Speech
{
    public interface ISpeechSynthesizerWrapper
    {
        void SpeakAsync(string text);

        void SpeakAsyncCancelAll();
    }

    // Wrapper for SpeechSynthesizer to allow for easier testing
    public class SpeechSynthesizerWrapper : ISpeechSynthesizerWrapper
    {
        private readonly SpeechSynthesizer _synthesizer;
        private readonly UserDefaults _userDefaults;

        public SpeechSynthesizerWrapper(UserDefaults userDefaults)
        {
            _userDefaults = userDefaults;
            _synthesizer = new SpeechSynthesizer();
            _synthesizer.SetOutputToDefaultAudioDevice();
        }

        /**
         * Wrap SpeakAsync with configurable style.
         */
        public void SpeakAsync(string text)
        {
            PromptBuilder builder = new PromptBuilder();
            builder.StartStyle(new PromptStyle() 
            { 
                Rate = _userDefaults.PromptRate
            });
            builder.AppendText(text);
            builder.EndStyle();

            _synthesizer.SpeakAsync(builder);
        }

        public void SpeakAsyncCancelAll()
        {
            _synthesizer.SpeakAsyncCancelAll();
        }
    }
}
