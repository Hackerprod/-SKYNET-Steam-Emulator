using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SKYNET.DB
{
    public class KeyValueCollection<TK, TV> where TV : class
    {
        private readonly ConcurrentDictionary<TK, TV> concurrentDictionary;

        private readonly Func<TV, TK> func;

        private readonly bool _bool;

        private readonly IMongoCollection<TV> imongoCollection;

        public KeyValueCollection(Expression<Func<TV, TK>> keyField, IMongoDatabase db = null, string tableName = null)
        {


            func = keyField.Compile();
            concurrentDictionary = new ConcurrentDictionary<TK, TV>();
            if (db != null && !string.IsNullOrEmpty(tableName))
            {
                _bool = true;
                imongoCollection = db.GetCollection<TV>(tableName);
                FillFromMongo();
            }
        }

        public void FillFromMongo()
        {
            if (_bool)
            {
                foreach (TV item in imongoCollection.Find(FilterDefinition<TV>.Empty).ToEnumerable())
                {
                    concurrentDictionary[func(item)] = item;
                }
            }
        }

        public TV FindKey(TK k)
        {
            if (!concurrentDictionary.TryGetValue(k, out TV value))
            {
                return null;
            }
            return value;
        }

        public TV FindOne(Func<TV, bool> p)
        {
            return concurrentDictionary.Values.Where(p).FirstOrDefault();
        }

        public IEnumerable<TV> Find(Func<TV, bool> p)
        {
            return concurrentDictionary.Values.Where(p);
        }

        public void InsertOne(TV item)
        {
            TK key = func(item);
            if (concurrentDictionary.TryAdd(key, item) && _bool)
            {
                imongoCollection.InsertOne(item);
            }
        }

        public void ReplaceOne(Expression<Func<TV, bool>> exp, TV item)
        {
            Func<TV, bool> predicate = exp.Compile();
            TV val = concurrentDictionary.Values.Where(predicate).FirstOrDefault();
            if (val != null && concurrentDictionary.TryUpdate(func(val), item, val) && _bool)
            {
                imongoCollection.ReplaceOne(exp, item);
            }
        }

        public void DeleteOne(Expression<Func<TV, bool>> exp)
        {
            Func<TV, bool> predicate = exp.Compile();
            TV val = concurrentDictionary.Values.Where(predicate).FirstOrDefault();
            if (val != null && concurrentDictionary.TryRemove(func(val), out TV _) && _bool)
            {
                imongoCollection.DeleteOne(exp);
            }
        }

        public void Clear()
        {
            concurrentDictionary.Clear();
            if (_bool)
            {
                imongoCollection.DeleteMany(FilterDefinition<TV>.Empty);
            }
        }

        public int Count(Expression<Func<TV, bool>> exp)
        {
            return concurrentDictionary.Values.Count(exp.Compile());
        }
    }
}
