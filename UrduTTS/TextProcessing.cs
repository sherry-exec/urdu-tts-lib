using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UrduTTS
{
    #region Text Processing
    public static class TextProcessing
    {
        private static List<Delimeter> delimeters = new List<Delimeter>();
        
        // Returns List<bool, Token>
        // boolean tells if the word is entered in the database, if the token is a word
        public static List<string> UnrecognizedWords(string text)
        {
            try
            {
                List<Token> tokens = parse(text);
                List<string> unrecognizedWords = new List<string>();

                for (int i = 0; i < tokens.Count; i++)
                {
                    if(tokens[i].classPart == TokenType.Word)
                    {
                        if (DataAccessLayer.SearchRecordsByWord(tokens[i].valuePart) == null)
                        {
                            unrecognizedWords.Add(tokens[i].valuePart);
                        }
                    }
                }

                return unrecognizedWords;
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
                List<Token> tokens = parse(text);

                List<WordRecord> temp;
                List<string> upsReps = new List<string>();

                for (int i = 0; i < tokens.Count; i++)
                {
                    if (tokens[i].classPart == TokenType.Delimeter)
                        upsReps.Add(EncodeDelimeterToPhoneme(isDelimeter(tokens[i].valuePart)));               // encode delimeter to phoneme??????????
                    else
                    {
                        // Try for Word representation
                        temp = DataAccessLayer.SearchRecordsByWord(tokens[i].valuePart);

                        // Try for Diacritic representation
                        if(temp == null)
                            temp = DataAccessLayer.SearchRecordsByDiacritic(tokens[i].valuePart);

                        if (temp != null)
                            upsReps.Add(temp[0].UpsRep);
                    }
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
                List<Token> tokens = parse(text);

                List<WordRecord> temp;
                List<string> ipaReps = new List<string>();

                for (int i = 0; i < tokens.Count; i++)
                {
                    if (tokens[i].classPart == TokenType.Delimeter)
                        ipaReps.Add(EncodeDelimeterToPhoneme(isDelimeter(tokens[i].valuePart)));               // encode delimeter to phoneme??????????
                    else
                    {
                        // Try for Word representation
                        temp = DataAccessLayer.SearchRecordsByWord(tokens[i].valuePart);

                        // Try for Diacritic representation
                        if (temp == null)
                            temp = DataAccessLayer.SearchRecordsByDiacritic(tokens[i].valuePart);

                        if (temp != null)
                            ipaReps.Add(temp[0].IpaRep);
                    }
                }

                return ipaReps;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Initializer
        internal static void init()
        {
            try
            {
                if (delimeters.Count == 0)
                {
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
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Desc:    Parses urdu text and separates it into tokens of string
        // Input:   Urdu text (with delimeters and diacritics(optional))
        // Output:  List of strings containing urdu words
        private static List<Token> parse(string urduText)
        {
            try
            {
                init();

                List<Token> tokensList = new List<Token>();
                string buffer = null;

                for (int i = 0; i < urduText.Length; i++)
                { 
                    // if current character a delimeter, get its index
                    if (isDelimeter(urduText[i].ToString()) >= 0)            // if it is delimeter
                    {
                        if ((buffer = buffer.Trim()) != String.Empty)
                        {
                            tokensList.Add(new Token(TokenType.Word, buffer));     // break here, add buffered word till here
                            buffer = "";
                        }
                        tokensList.Add(new Token(TokenType.Delimeter, urduText[i].ToString()));
                    }
                    else
                    {
                        buffer += urduText[i];
                    }

                    if (i == urduText.Length - 1) //end is reached
                    {
                        if ((buffer = buffer.Trim()) != "")
                        {
                            tokensList.Add(new Token(TokenType.Word, buffer));     // break here, add buffered word till here
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
        internal static short isDelimeterPhoneme(string delimPhoneme)
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
    public enum TokenType { Delimeter, Word }
    public struct Token
    {
        public TokenType classPart;
        public string valuePart;

        public Token(TokenType cp, string vp)
        {
            classPart = cp;
            valuePart = vp;
        }
    }
    #endregion
}
