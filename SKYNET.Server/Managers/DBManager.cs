using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using SKYNET.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SKYNET.Managers
{
    public class DBManager
    {
        private static MongoClient Client;

        public static IMongoDatabase DB { get; set; }
        public static string Host { get; set; }
        public static int Port { get; set; }

        static DBManager()
        {
            Host = "127.0.0.1";
            Port = 27002;
        }

        public static async Task<bool> Initialize()
        {
            try
            {
                Write("Initializing Database connection");

                MongoUrlBuilder mongoUrlBuilder = new MongoUrlBuilder
                {
                    Servers = new List<MongoServerAddress>() { new MongoServerAddress(Host, Port) },
                    DatabaseName = "SKYNET",
                    ReadPreference = ReadPreference.Primary,
                    ConnectTimeout = TimeSpan.FromSeconds(3.0),
                    SocketTimeout = TimeSpan.FromSeconds(30),
                    ServerSelectionTimeout = TimeSpan.FromSeconds(3.0)
                };

                Client = new MongoClient(mongoUrlBuilder.ToMongoUrl()); 
                var db = Client.GetDatabase(mongoUrlBuilder.DatabaseName); 

                bool errorCollecting = true;

                try
                {
                    Socket sockets = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IAsyncResult asyncResult = sockets.BeginConnect(new IPEndPoint(IPAddress.Parse(Host), Port), new AsyncCallback((IAsyncResult ar) => { try { ((Socket)ar.AsyncState).EndConnect(ar); } catch { } }), sockets);
                    if (!asyncResult.AsyncWaitHandle.WaitOne(1000, false))
                    {
                        var DBPath = Path.Combine(Common.GetPath(), "Data", "MongoDB", "Database.cmd");
                        if (File.Exists(DBPath))
                        {
                            var iInfo =

                            Process.Start(new ProcessStartInfo()
                            {
                                FileName = "Database.cmd",
                                WorkingDirectory = Path.Combine(Common.GetPath(), "Data", "MongoDB"),
                                Arguments = "",
                                CreateNoWindow = true,
                                WindowStyle = ProcessWindowStyle.Hidden,
                                UseShellExecute = true
                            });
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }

                while (errorCollecting)
                {
                    try
                    {
                        await db.ListCollectionsAsync();
                        errorCollecting = false;
                    }
                    catch { }
                }

                DB = Client.GetDatabase("SKYNET");

                SetIgnoreConvention();

                UserDB.Initialize();
                StatsDB.Initialize();

                Write("Database connected successfully");
                return true;
            }
            catch 
            {
                Write("Error initializing database");
                return false;
            }
        }

        private static void SetIgnoreConvention()
        {
            ConventionPack conventionPack = new ConventionPack
            {
                new MongoIngoreConvention(),
                new IgnoreExtraElementsConvention(ignoreExtraElements: true),
                new UnsignedConventions()
            };
            conventionPack.AddClassMapConvention("IgnoreExtraElements", delegate (BsonClassMap map)
            {
                map.SetIgnoreExtraElements(ignoreExtraElements: true);
            });
            ConventionRegistry.Register("Ignores", conventionPack, (Type t) => true);
        }

        public static async Task WriteCollectionToFile(string collectionName, string fileName)
        {
            //Common.VerifyDirectoryStructure();

            var collection = DB.GetCollection<RawBsonDocument>(collectionName);
            // Make sure the file is empty before we start writing to it
            File.WriteAllText(fileName, string.Empty);

            using (var cursor = await collection.FindAsync(new BsonDocument()))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (var document in batch)
                    {
                        File.AppendAllLines(fileName, new[] { document.ToString() });
                    }
                }
            }
        }

        public static async Task LoadCollectionFromFile(string collectionName, string fileName)
        {
            //Common.VerifyDirectoryStructure();

            using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                var collection = DB.GetCollection<BsonDocument>(collectionName);

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    try
                    {
                        BsonDocument sd = BsonDocument.Parse(line);
                        if (sd.TryGetValue("ObjectId", out BsonValue value))
                        {
                            //modCommon.Show(value.AsString);
                        }

                        await collection.InsertOneAsync(BsonDocument.Parse(line));
                    }
                    catch (Exception ex)
                    {
                        //modCommon.Show(ex);
                    }
                }
            }
        }

        private static void Write(object msg)
        {
            Log.Write("DBManager", msg);
        }
    }
}
