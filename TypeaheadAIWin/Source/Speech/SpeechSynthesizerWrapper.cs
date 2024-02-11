using System.Speech.Synthesis;

namespace TypeaheadAIWin.Source.Speech
{
    public interface ISpeechSynthesizerWrapper
    {
        void SpeakAsync(string text);

        void SpeakAsyncCancelAll();

        // Property for PromptRate with getter and setter
        PromptRate PromptRate { get; set; }
    }

    // Wrapper for SpeechSynthesizer to allow for easier testing
    public class SpeechSynthesizerWrapper : ISpeechSynthesizerWrapper
    {
        private readonly SpeechSynthesizer synthesizer;
        
        public PromptRate PromptRate
        {
            get
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
            set
            {
                Properties.Settings.Default.PromptRate = (int)value;
                Properties.Settings.Default.Save();
            }
        }

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
            PromptBuilder builder = new PromptBuilder();
            builder.StartStyle(new PromptStyle()
            {
                Rate = PromptRate
            });
            builder.AppendText(text);
            builder.EndStyle();

            synthesizer.SpeakAsync(builder);
        }

        public void SpeakAsyncCancelAll()
        {
            synthesizer.SpeakAsyncCancelAll();
        }
    }
}
