using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using PoluxServices.Models;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace PoluxServices.DataBase
{
    public class Mongo
    {
        public static IMongoCollection<SAPCollection> ConnectionToMongoDBSAP()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("SigmaDB");
            return db.GetCollection<SAPCollection>("UsersSAP");
        }
        public static IList<SAPCollection> getUserFromMongo(int employeeId)
        {
            var collection = ConnectionToMongoDBSAP();
            return collection.Find(x => x.EmployeeId == employeeId)
                .Limit(5)
                .ToListAsync()
                .Result;
        }

        public static void FindAndUpdatePasswordSAP()
        {

        }
    }
}
