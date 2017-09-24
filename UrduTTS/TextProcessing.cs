using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UrduTTS
{
    enum TokenType { Delimeter, Word }

    #region Text Processing
    public static class TextProcessing
    {
        private static List<Delimeter> delimeters = null;

        // Constructor
        public static void Init()
        {
            try
            {
                delimeters = new List<Delimeter>();

                // insert predefined delimeters
                delimeters.Add(new Delimeter(" ", "none"));
                delimeters.Add(new Delimeter(",", "x-weak"));
                delimeters.Add(new Delimeter("،", "x-weak"));
                delimeters.Add(new Delimeter("۔", "weak"));
                delimeters.Add(new Delimeter("؟", "weak"));
                delimeters.Add(new Delimeter(".", "weak"));
                delimeters.Add(new Delimeter("?", "weak"));
                delimeters.Add(new Delimeter("!", "weak"));
                delimeters.Add(new Delimeter("\n", "medium"));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<string> UnrecognizedWords(string text)
        {
            try
            {
                List<string> urduWords = parse(text);
                List<string> unenteredWords = new List<string>();

                for (int i = 0; i < urduWords.Count; i++)
                {
                    if (isDelimeter(urduWords[i]) == -1)    // If it is not a delimeter, if it is a word
                        if (DataAccessLayer.SearchRecordsByWord(urduWords[i]) == null)
                            unenteredWords.Add(urduWords[i]);
                }

                return unenteredWords;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<string> ToUPSReps(string text)
        {
            try
            {
                List<string> upsReps = new List<string>();
                List<string> diacs = new List<string>();
                List<string> urduWords = parse(text);

                // Choose diacritics - main problem here - right now we are choosing 0th diacritics always
                for (int i = 0; i < urduWords.Count; i++)
                {
                    short delimIndex = isDelimeter(urduWords[i]);
                    if (delimIndex < 0)
                    {
                        List<WordRecord> diacRec = DataAccessLayer.SearchRecordsByWord(urduWords[i]);

                        if (diacRec != null)
                            diacs.Add(diacRec[0].DiacriticRep);
                    }
                    else
                    {
                        diacs.Add(EncodeDelimeterToPhoneme(delimIndex));
                    }
                }

                for (int i = 0; i < diacs.Count; i++)
                {
                    short delimIndex = isDelimeterPhoneme(diacs[i]);
                    if (delimIndex < 0)
                        upsReps.Add(DataAccessLayer.SearchRecordsByDiacritic(diacs[i])[0].UpsRep);
                    else
                        upsReps.Add(diacs[i]);
                }

                return upsReps;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<string> ToIPAReps(string text)
        {
            try
            {
                List<string> ipaReps = new List<string>();
                List<string> urduWords = parse(text);

                for (int i = 0; i < urduWords.Count; i++)
                {

                }

                return ipaReps;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Desc:    Parses urdu text and separates it into tokens of string
        // Input:   Urdu text (with delimeters and diacritics(optional))
        // Output:  List of strings containing urdu words
        private static List<string> parse(string urduText)
        {
            try
            {
                List<string> tokensList = new List<string>();
                string buffer = null;
                for (int i = 0; i < urduText.Length; i++)
                { 
                   
                    // if current character a delimeter, get its index
                    if (isDelimeter(urduText[i].ToString()) >= 0)            // if it is delimeter
                    {
                        if ((buffer = buffer.Trim()) != String.Empty)
                        {
                            tokensList.Add(buffer);     // break here, add buffered word till here
                            buffer = "";
                        }
                        tokensList.Add(urduText[i].ToString());
                    }
                    else
                    {
                        buffer += urduText[i];
                    }

                    if (i == urduText.Length - 1) //end is reached
                    {
                        if ((buffer = buffer.Trim()) != "")
                        {
                            tokensList.Add(buffer);     // break here, add buffered word till here
                            buffer = "";
                        }
                    }
                }
                return tokensList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static List<string> urduWordToDiacritics(List<string> urduWords)
        {
            try
            {
                return null;

                //List<string> diacriticsList = new List<string>();
                //foreach (string word in urduWords)
                //{
                //    if (isDelimeterPhoneme(word) < 0)   // if it is not a delimeter, i.e. its a word
                //    {
                //        // then proceed to look for its diacritics
                //        List<WordRecord> availDiacs = DataAccessLayer.SearchRecordsByWord(word);
                //        if (availDiacs == null)
                //        {
                //            diacriticsList.Add(word);
                //        }
                //        else
                //        {
                //            diacriticsList.Add(availDiacs[0].DiacriticRep);
                //        }
                //    }
                //    else
                //        diacriticsList.Add(word);
                //}
                //return diacriticsList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Desc:    Encodes delimeter into its UPS representation for SSML <break>
        // Input:   Delimeter
        // Output:  Its strength for SSML <break strength='_____'>
        private static string EncodeDelimeterToPhoneme(short delimIndex, string config = "strength")
        {
            try
            {
                return config == "time" ? delimeters[delimIndex].PauseDuration.ToString() + "ms" : delimeters[delimIndex].PauseStrength;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static short isDelimeter(string delim)
        {
            try
            {
                for (short i = 0; i < delimeters.Count; i++)
                {
                    if (delimeters[i].Character == delim)
                        return i;
                }
                return -1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static short isDelimeterPhoneme(string delimPhoneme)
        {
            try
            {
                for (short i = 0; i < delimeters.Count; i++)
                {
                    if (delimeters[i].PauseStrength == delimPhoneme)
                        return i;
                }
                return -1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool containDelimeter(string word)
        {
            try
            {
                for (int i = 0; i < delimeters.Count; i++)
                {
                    if (delimeters[i].PauseStrength == word.Trim())
                        return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
    #endregion

    #region Struct Delimeter
    struct Delimeter
    {
        // Delimeter character
        private string delimeter;

        // Pause configuration
        private short pauseDuration;
        private string pauseStrength;

        // Constructors
        public Delimeter(string delim)
        {
            delimeter = delim;
            pauseDuration = 0;
            pauseStrength = "none";
        }

        public Delimeter(string delim, short pause)
        {
            delimeter = delim;
            pauseDuration = pause;
            pauseStrength = "none";
        }

        public Delimeter(string delim, string stren)
        {
            delimeter = delim;
            pauseStrength = stren;
            pauseDuration = 0;
        }

        // Utility functions
        private object match(string delim, string config = "strength")
        {
            // config can be "strength" or "duration"

            if (delimeter == delim)
            {
                if (config == "strength")
                    return pauseStrength;
                if (config == "duration")
                    return pauseDuration;
            }

            return null;
        }

        // Getters
        public string Character { get { return delimeter; } }
        public short PauseDuration { get { return pauseDuration; } }
        public string PauseStrength { get { return pauseStrength.Trim(); } }
    }
    #endregion

    #region Struct Token
    public struct Token
    {
        TokenType classPart;
        string valuePart;
    }
    #endregion
}
