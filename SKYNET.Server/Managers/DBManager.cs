using SKYNET.DB;
using SQLite;
using System;
using System.IO;

namespace SKYNET.Managers
{
    public class DBManager
    {
        private static SQLiteDatabase DB;
        public static UsersDB Users;

        public static void Initialize()
        {
            try
            {
                DB = new SQLiteDatabase(Path.Combine(Common.GetPath(), "Data", "DB.bin"));
                DB.NativeDllPath = Path.Combine(Path.Combine(Common.GetPath(), "Data", "Assemblies"));
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
