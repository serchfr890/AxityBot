using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace PoluxServices.Models
{
    public class SAPCollection
    {
        public ObjectId _id { get; set; }
        public string UserId { get; set; }
        public int EmployeeId { get; set; }
        public string AdmisionDate { get; set; }
        public string BirthDate { get; set; }
        public string Password { get; set; }
    }
}
