using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace Labor_3
{
    public class SQLiteHandler
    {
        private SQLiteConnection _conn;

        public void ConnectToDb()
        {
            try
            {
                if (!File.Exists("MyDatabase.sqlite"))
                {
                    SQLiteConnection.CreateFile("MyDatabase.sqlite");
                }

                _conn = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
                _conn.Open();

                string createTable = "CREATE TABLE IF NOT EXISTS Keywords(" + "id INTEGER PRIMARY KEY AUTOINCREMENT, " + "keyword TEXT NOT NULL UNIQUE)";

                SQLiteCommand cmd = new SQLiteCommand(createTable, _conn);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error creating table! ERROR: " + ex.ToString());
                    return;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to database! ERROR: " + ex.ToString());
                return;
            }

            }
        public void DisconnectFromDb()
        {
            if (_conn != null)
            {
                _conn.Close();
                _conn = null;
            }

        }

        public void InsertKeyword(string keyword)
        {
            if (_conn == null)
            {
                Console.WriteLine("Database not connected!");
                return;
            }
            string insertK = "INSERT INTO Keywords (keyword) VALUES (@keyword)";
            SQLiteCommand cmd = new SQLiteCommand(insertK, _conn);
            cmd.Parameters.AddWithValue("@keyword", keyword.ToLower());

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting keyword! ERROR: " + ex.ToString());
                return;
            }
        }


        public List<string> GetAllKeywords()
        {

            List<string> keywords = new List<string>();


            string selectAll = "SELECT keyword FROM Keywords";

            SQLiteCommand cmd = new SQLiteCommand(selectAll, _conn);
            SQLiteDataReader reader = null;

            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving keywords! ERROR: " + ex.ToString());
                return keywords;
            }
            string data = "Fields: ";
            for (int i = 0; i < reader.FieldCount; i++)
            {
                data += reader.GetName(i) + " \n ";
            }
            data += "DATA: \n";

            while (reader.Read())
            {
                keywords.Add(reader.GetString(0));

            }
            reader.Close();
            return keywords;

        }

        public void DeleteKeyword(string keyword)
        {
            if (_conn == null)
            {
                Console.WriteLine("Database not connected!");
                return;
            }

            string deleteK = "DELETE FROM Keywords WHERE keyword = @keyword";
            SQLiteCommand cmd = new SQLiteCommand(deleteK, _conn);
            cmd.Parameters.AddWithValue("@keyword", keyword.ToLower());

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting keyword! ERROR: " + ex.ToString());
            }
        }

    }


}


