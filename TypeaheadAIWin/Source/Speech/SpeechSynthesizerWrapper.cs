using System.Diagnostics;
using System.Speech.Synthesis;

namespace TypeaheadAIWin.Source.Speech
{
    public interface ISpeechSynthesizerWrapper
    {
        void SpeakAsync(string text);

        void SpeakAsyncCancelAll();

        void SetPromptRate(PromptRate rate);
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

        /**
         * Wrap SpeakAsync with configurable style.
         */
        public void SpeakAsync(string text)
        {
            Trace.WriteLine(text);
            PromptBuilder builder = new PromptBuilder();
            builder.StartStyle(new PromptStyle()
            {
                Rate = GetPromptRate()
            });
            builder.AppendText(text);
            builder.EndStyle();

            synthesizer.SpeakAsync(builder);
        }

        public void SpeakAsyncCancelAll()
        {
            synthesizer.SpeakAsyncCancelAll();
        }

        private PromptRate GetPromptRate()
        {
            var rate = Properties.Settings.Default.PromptRate;
            if (Enum.IsDefined(typeof(PromptRate), rate))
            {
                return (PromptRate)rate;
            }
            else
            {
                return PromptRate.NotSet;
            }
        }

        public void SetPromptRate(PromptRate rate)
        {
            Properties.Settings.Default.PromptRate = (int) rate;
            Properties.Settings.Default.Save();
        }
    }
}
