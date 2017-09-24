using System;
using System.Data;
using System.IO;
using System.Data.OleDb;


namespace UrduTTS
{
    internal static class Database
    {
        static DataTable table = null;
        static OleDbConnection connection = null;
        static OleDbCommand command = null;
        static OleDbDataAdapter adapter = null;
        static string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Directory.GetCurrentDirectory() + "\\Database\\New.accdb;Persist Security Info=False;";

        //
        //   Database Functions
        //

        #region Select
        public static DataTable SelectRecord(string col, string val)
        {
            try
            {
                string queryselect = "SELECT * FROM UrduWordRecords WHERE " + col + " = '" + val + "';";

                table = new DataTable();
                command = new OleDbCommand(queryselect, OpenConnection());
                adapter = new OleDbDataAdapter(command);
                adapter.Fill(table);

                return table;
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }
        public static DataTable PhonemeDictionary()
        {
            try
            {
                string queryselect = "SELECT * FROM PhonemeDictionary;";

                table = new DataTable();
                command = new OleDbCommand(queryselect, OpenConnection());
                adapter = new OleDbDataAdapter(command);
                adapter.Fill(table);

                return table;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }
        public static DataTable AllRecords()
        {
            try
            {
                string queryselect = "SELECT * FROM UrduWordRecords;";

                table = new DataTable();
                command = new OleDbCommand(queryselect, OpenConnection());
                adapter = new OleDbDataAdapter(command);
                adapter.Fill(table);

                return table;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }
        #endregion

        #region Insert
        public static WordRecord InsertRecord(WordRecord newRecord)
        {
            try
            {
                string queryinsert = "INSERT INTO UrduWordRecords (Word, Diacritic, IPA, UPS) VALUES (" +
                                     "'" + newRecord.UrduWord + "', " + "'" + newRecord.DiacriticRep + "', " + "'" + newRecord.IpaRep + "', " + "'" + newRecord.UpsRep + "');";
                command = new OleDbCommand(queryinsert, OpenConnection());
                if (command.ExecuteNonQuery() >= 0)
                {
                    string querylatest = "SELECT * FROM UrduWordRecords WHERE ID = (SELECT max(ID) FROM UrduWordRecords);";
                    table = new DataTable();
                    command = new OleDbCommand(querylatest, OpenConnection());
                    adapter = new OleDbDataAdapter(command);
                    adapter.Fill(table);

                    return new WordRecord(Convert.ToInt32(table.Rows[0][0].ToString()), table.Rows[0][1].ToString(), table.Rows[0][2].ToString(), table.Rows[0][3].ToString(), table.Rows[0][4].ToString());
                }

                return new WordRecord(-1, "", "", "", "");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }
        public static bool InsertPhoneme(PhonemeRecord newPhonemeRecord)
        {
            try
            {
                string queryinsert = "INSERT INTO PhonemeDictionary (UrduPhoneme, IPA, UPS) VALUES ('" + newPhonemeRecord.UrduPhoneme + "', '" + newPhonemeRecord.IpaPhoneme + "', '" + newPhonemeRecord.UpsPhoneme + "');";
                command = new OleDbCommand(queryinsert, OpenConnection());
                if (command.ExecuteNonQuery() >= 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                connection.Close();
            }
        }
        #endregion

        #region Update
        public static bool UpdateRecord(WordRecord newRecord)
        {
            try
            {
                string queryupdate = "UPDATE UrduWordRecords SET Word = '" + newRecord.UrduWord + "', Diacritic = '" + newRecord.DiacriticRep + "', IPA = '" + newRecord.IpaRep + "', UPS = '" + newRecord.UpsRep + "' " +
                                     "WHERE ID = " + newRecord.Id + ";";
                command = new OleDbCommand(queryupdate, OpenConnection());
                if (command.ExecuteNonQuery() >= 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }
        #endregion

        #region Database Connection
        public static OleDbConnection OpenConnection()
        {
            try
            {
                if (connection == null)                
                    connection = new OleDbConnection(constr);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                return connection;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void CloseConnection()
        {
            try
            {
                if (connection != null && connection.State == ConnectionState.Open)
                    connection.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }

    #region Struct Database Record Schema
    public struct WordRecord
    {
        private int id;
        private string word, diacritic, ipa, ups;

        // Getters
        public int Id { get { return id; } }
        public string UrduWord { get { return word; } }
        public string DiacriticRep { get { return diacritic; } }
        public string IpaRep { get { return ipa; } }
        public string UpsRep { get { return ups; } }

        // Constructor
        public WordRecord(int _id, string _word, string _diac, string _ipa, string _ups)
        {
            id = _id;
            word = _word;
            diacritic = _diac;
            ipa = _ipa;
            ups = _ups;
        }
        public WordRecord(string _word, string _diac, string _ipa, string _ups)
        {
            id = 0;
            word = _word;
            diacritic = _diac;
            ipa = _ipa;
            ups = _ups;
        }
    }
    public struct PhonemeRecord
    {
        private string urduPhoneme, ipa, ups;

        // Getters
        public string UrduPhoneme { get { return urduPhoneme; } }
        public string IpaPhoneme { get { return ipa; } }
        public string UpsPhoneme { get { return ups; } }

        // Constructor
        public PhonemeRecord(string _phoneme, string _ipa, string _ups)
        {
            urduPhoneme = _phoneme;
            ipa = _ipa;
            ups = _ups;
        }
    }
    #endregion

}
