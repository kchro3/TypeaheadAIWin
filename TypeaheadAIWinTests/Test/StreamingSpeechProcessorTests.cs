using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TypeaheadAIWin.Source;

namespace TypeaheadAIWinTests.Test
{
    [TestClass()]
    public class StreamingSpeechProcessorTests
    {
        private Mock<ISpeechSynthesizerWrapper> mockSynthesizer;
        private StreamingSpeechProcessor processor;

        [TestInitialize]
        public void Setup()
        {
            // Setup the mock speech synthesizer
            mockSynthesizer = new Mock<ISpeechSynthesizerWrapper>();
            processor = new StreamingSpeechProcessor(mockSynthesizer.Object);
        }

        [TestMethod()]
        public void ProcessToken_WithSingleWord_SpeaksWord()
        {
            processor.ProcessToken("Hello");
            processor.FlushBuffer();

            mockSynthesizer.Verify(s => s.SpeakAsync("Hello"), Times.Once());
        }

        [TestMethod()]
        public void ProcessToken_WithDelimiter_SpeaksWithDelimiter()
        {
            processor.ProcessToken("Hello! ");
            processor.ProcessToken("How are you? ");
            processor.FlushBuffer();

            mockSynthesizer.Verify(s => s.SpeakAsync("Hello! "), Times.Once());
            mockSynthesizer.Verify(s => s.SpeakAsync("How are you? "), Times.Once());
        }

        [TestMethod()]
        public void ProcessToken_WithMultipleDelimiters_SpeaksEachSentence()
        {
            processor.ProcessToken("Hello! ");
            processor.ProcessToken("This is a test. ");
            processor.ProcessToken("How are you?");
            processor.FlushBuffer();

            mockSynthesizer.Verify(s => s.SpeakAsync("Hello! "), Times.Once());
            mockSynthesizer.Verify(s => s.SpeakAsync("This is a test. "), Times.Once());
            mockSynthesizer.Verify(s => s.SpeakAsync("How are you?"), Times.Once());
        }

        [TestMethod()]
        public void ProcessToken_WithNoDelimiter_DoesNotSpeak()
        {
            processor.ProcessToken("Hello");
            // Do not call FlushBuffer to simulate no delimiter

            mockSynthesizer.Verify(s => s.SpeakAsync(It.IsAny<string>()), Times.Never());
        }

        [TestMethod()]
        public void FlushBuffer_WithRemainingData_SpeaksRemainingData()
        {
            processor.ProcessToken("Hello");
            processor.FlushBuffer();

            mockSynthesizer.Verify(s => s.SpeakAsync("Hello"), Times.Once());
        }

        [TestMethod()]
        public void FlushBuffer_WithNoData_DoesNotSpeak()
        {
            processor.FlushBuffer();

            mockSynthesizer.Verify(s => s.SpeakAsync(It.IsAny<string>()), Times.Never());
        }

        // Additional test to cover new delimiter handling
        [TestMethod()]
        public void ProcessToken_WithLineBreak_SpeaksLineBreak()
        {
            processor.ProcessToken("Hello\n");
            processor.ProcessToken("world!");
            processor.FlushBuffer();

            mockSynthesizer.Verify(s => s.SpeakAsync("Hello\n"), Times.Once());
            mockSynthesizer.Verify(s => s.SpeakAsync("world!"), Times.Once());
        }
    }
}
