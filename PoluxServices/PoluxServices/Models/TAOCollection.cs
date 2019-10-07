using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
namespace PoluxServices.Models
{
    public class TAOCollection
    {
        public ObjectId _id { get; set; }
        public int EmployeeId { get; set; }
        public string AdmisionDate { get; set; }
    }
}
