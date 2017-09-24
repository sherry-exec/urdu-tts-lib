using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Speech.Synthesis;

namespace UrduTTS
{
    enum PhoneticRepresentation { IPA,UPS }
    
    public class SsmlEncoding
    {
        // Private data members
        SpeechSynthesizer synthesizer = null;
        PromptBuilder ssmlBuilder = null;

        PhoneticRepresentation OutputPhoneticRepresentation = PhoneticRepresentation.UPS;

        // Constructors
        public SsmlEncoding()
        {
            try
            {
                synthesizer = new SpeechSynthesizer();
                synthesizer.Rate = 1;
                synthesizer.TtsVolume = 70;
                synthesizer.SetOutputToDefaultAudioDevice();

                TextProcessing.Init();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Public Functions
        public void SpeakStart(string text, bool async = true)
        {
            try
            {
                ssmlBuilder = new PromptBuilder();
                ssmlBuilder.AppendSsmlMarkup(GetSsml(text));

                if (async)
                    synthesizer.SpeakAsync(ssmlBuilder);
                else
                    synthesizer.Speak(ssmlBuilder);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SpeakStart(List<string> phonemes, bool async = true)
        {
            try
            {
                ssmlBuilder = new PromptBuilder();
                ssmlBuilder.AppendSsmlMarkup(GetSsml(phonemes));

                if (async)
                    synthesizer.SpeakAsync(ssmlBuilder);
                else
                    synthesizer.Speak(ssmlBuilder);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveToFile(string text, string path)
        {
            try
            {
                ssmlBuilder = new PromptBuilder();
                ssmlBuilder.AppendSsmlMarkup(GetSsml(text));
                synthesizer.SetOutputToWaveFile(path);
            }
            catch (Exception)
            {
                throw;
            }
        }
            
        public void SpeakPause()
        {
            try
            {
                synthesizer.Pause();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SpeakResume()
        {
            try
            {
                synthesizer.Resume();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SpeakCancel()
        {
            try
            {
                synthesizer.SpeakAsyncCancelAll();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Volume
        {
            set
            {
                synthesizer.TtsVolume = value;
            }
            get
            {
                return synthesizer.TtsVolume;
            }
        }

        public int Rate
        {
            set
            {
                synthesizer.Rate = value;
            }
            get
            {
                return synthesizer.Rate;
            }
        }

        // Private Functions
        private string EncodePhoneme(string phoneme)
        {
            if (TextProcessing.isDelimeterPhoneme(phoneme) >= 0)
                return "<break strength=\"" + phoneme + "\" />";
            else
                return "<phoneme alphabet =\"x-microsoft-ups\" ph=\"" + phoneme + "\" />";
        }

        private string GetSsml(string text)
        {
            try
            {
                if(OutputPhoneticRepresentation == PhoneticRepresentation.UPS)
                    return GetSsml(TextProcessing.ToUPSReps(text));
                else
                    return GetSsml(TextProcessing.ToIPAReps(text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetSsml(List<string> phonemes)
        {
            try
            {
                string ssml = "<voice name =\"Microsoft Server Speech Text to Speech Voice (en-IN, Heera)\"><s>";

                for (int i = 0; i < phonemes.Count; i++)
                    if (phonemes[i] != null)
                        ssml += EncodePhoneme(phonemes[i]);

                ssml += "</s></voice>";

                return ssml;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
