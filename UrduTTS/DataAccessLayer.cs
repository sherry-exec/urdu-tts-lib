using System;
using System.Collections.Generic;
using System.Threading;
using System.Data;

namespace UrduTTS
{
    public static class DataAccessLayer
    {
        private static List<WordRecord> cacheRegister = null;
        private static bool caching = false;

        #region Data Retrieval
        public static List<WordRecord> SearchRecordsByWord(string urdu)
        {
            try
            {
                List<WordRecord> records = null;

                // If caching is enabled, check in cache register first
                if (caching)
                    records = CheckThroughCache(urdu);

                //
                if (records == null)
                {
                    DataTable table = Database.SelectRecord("Word", urdu);
                    if (table.Rows.Count > 0)
                    {
                        records = new List<WordRecord>();
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            records.Add(new WordRecord(Convert.ToInt32(table.Rows[i][0].ToString()), table.Rows[i][1].ToString(), table.Rows[i][2].ToString(), table.Rows[i][3].ToString(), table.Rows[i][4].ToString()));

                            //If caching is enabled, enter the newly fetched data into the cache register
                            if (caching)
                                cacheRegister.Add(new WordRecord(Convert.ToInt32(table.Rows[i][0].ToString()), table.Rows[i][1].ToString(), table.Rows[i][2].ToString(), table.Rows[i][3].ToString(), table.Rows[i][4].ToString()));
                        }
                        return records;
                    }
                    return null;
                }

                return records;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<WordRecord> SearchRecordsByDiacritic(string diacritic)
        {
            try
            {
                List<WordRecord> records = null;

                // If caching is enabled, check in cache register first
                if (caching)
                    records = CheckThroughCache(diacritic);

                //
                if (records == null)
                {
                    DataTable table = Database.SelectRecord("Diacritic", diacritic);
                    if (table.Rows.Count > 0)
                    {
                        records = new List<WordRecord>();
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            records.Add(new WordRecord(Convert.ToInt32(table.Rows[i][0].ToString()), table.Rows[i][1].ToString(), table.Rows[i][2].ToString(), table.Rows[i][3].ToString(), table.Rows[i][4].ToString()));

                            //If caching is enabled, enter the newly fetched data into the cache register
                            if (caching)
                                cacheRegister.Add(new WordRecord(Convert.ToInt32(table.Rows[i][0].ToString()), table.Rows[i][1].ToString(), table.Rows[i][2].ToString(), table.Rows[i][3].ToString(), table.Rows[i][4].ToString()));
                        }
                        return records;
                    }
                    return null;
                }

                return records;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static string UrduWordFromDiacriticRep(string diacritic)
        {
            try
            {
                List<WordRecord> records = null;

                // If caching is enabled, check in cache register first
                if (caching)
                    records = CheckThroughCache(diacritic);

                //
                if (records != null)
                {
                    return records[0].UrduWord;
                }
                else
                {
                    DataTable table = Database.SelectRecord("Diacritic", diacritic);
                    if (table.Rows.Count > 0)
                    {
                        records = new List<WordRecord>();
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            records.Add(new WordRecord(Convert.ToInt32(table.Rows[i][0].ToString()), table.Rows[i][1].ToString(), table.Rows[i][2].ToString(), table.Rows[i][3].ToString(), table.Rows[i][4].ToString()));

                            //If caching is enabled, enter the newly fetched data into the cache register
                            if (caching)
                                cacheRegister.Add(new WordRecord(Convert.ToInt32(table.Rows[i][0].ToString()), table.Rows[i][1].ToString(), table.Rows[i][2].ToString(), table.Rows[i][3].ToString(), table.Rows[i][4].ToString()));
                        }
                        return records[0].UrduWord;
                    }
                    return "";
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<WordRecord> GetAllRecords()
        {
            try
            {
                DataTable table = Database.AllRecords();
                List<WordRecord> records = new List<WordRecord>();

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    records.Add(new WordRecord(Convert.ToInt32(table.Rows[i][0].ToString()), table.Rows[i][1].ToString(), table.Rows[i][2].ToString(), table.Rows[i][3].ToString(), table.Rows[i][4].ToString()));
                }
                
                return records;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DataTable GetPhonemeDictionary()
        {
            try
            {
                return Database.PhonemeDictionary();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Data Insertion
        public static bool AddNewRecord(WordRecord newWordRecord)
        {
            try
            {
                WordRecord enteredRecord = Database.InsertRecord(newWordRecord);

                if (enteredRecord.Id > 0)
                {
                    if (caching)
                        UpdateCache(enteredRecord);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool AddNewPhoneme(PhonemeRecord newPhonemeRecord)
        {
            return Database.InsertPhoneme(newPhonemeRecord);
        }
        #endregion

        #region Data Updation
        public static bool UpdateRecord(WordRecord updatedRecord)
        {
            try
            {
                if(Database.UpdateRecord(updatedRecord))
                {
                    if (caching)
                        UpdateCache(updatedRecord);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Caching Functions
        public static bool DatabaseCaching
        {
            set
            {
                if (caching == value)
                    return;

                if ((caching = value))
                {
                    if (cacheRegister == null)
                        cacheRegister = new List<WordRecord>();
                }
                else
                {
                    if (cacheRegister != null)
                        cacheRegister.Clear();
                }
            }
            get
            {
                return caching;
            }
        }
        private static List<WordRecord> CheckThroughCache(string value)
        {
            try
            {

                if (caching)
                {
                    List<WordRecord> records = new List<WordRecord>();
                    for (int i = 0; i < cacheRegister.Count; i++)
                    {
                        if (cacheRegister[i].UrduWord == value || cacheRegister[i].DiacriticRep == value)
                        {
                            records.Add(cacheRegister[i]);
                        }
                    }

                    if (records.Count > 0)
                        return records;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private static void UpdateCache(WordRecord newRecord)
        {
            try
            {
                // if exists, then update it
                for (int i = 0; i < cacheRegister.Count; i++)
                {
                    if(cacheRegister[i].Id == newRecord.Id)
                    {
                        cacheRegister[i] = newRecord;
                        return;
                    }
                }

                // otherwise, enter new record in cache register
                cacheRegister.Add(newRecord);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}