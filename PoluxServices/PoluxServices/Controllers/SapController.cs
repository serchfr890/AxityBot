using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PoluxServices.Models;
using PoluxServices.DataBase;

namespace PoluxServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SapController : ControllerBase
    {
        [HttpPost]
        public void Post([FromBody] SAPCollection sap)
        {
            if (!(sap == null))
            {
                var userSap = Mongo.getUserFromMongo(sap.EmployeeId);
                if (userSap.Count > 0
                    && string.Equals(userSap[0].UserId.ToLower(), sap.UserId.ToLower())
                    && string.Equals(userSap[0].AdmisionDate, sap.AdmisionDate)
                    && string.Equals(userSap[0].BirthDate, sap.BirthDate))
                {
                    string newPassword = "123sdfg";

                }
            }
        }
    }
}