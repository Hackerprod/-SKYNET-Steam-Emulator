using SKYNET.DB;
using SQLite;
using System;
using System.IO;

namespace SKYNET.Managers
{
    public class DBManager
    {
        private static SQLiteAsyncConnection DB;
        public static UsersDB Users;

        public static void Initialize()
        {
            try
            {
                // Get an absolute path to the database file
                DB = new SQLiteAsyncConnection(Path.Combine(Common.GetPath(), "Data", "DB.bin"));

                Users = new UsersDB(DB);
                Write("Initialized local Database");
            }
            catch (Exception)
            {
                Write("Error initializing database");
            }
        }

        private static void Write(object msg)
        {
            Log.Write("DBManager", msg);
        }
    }
}
