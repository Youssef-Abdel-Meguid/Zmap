using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Zmap.Dtos;
using Zmap.Models;

namespace Zmap.Controllers
{
    public class SeatMapController : ApiController
    {
        private ZmapEntities db = new ZmapEntities();
        private ZmapEntities logger = new ZmapEntities();

        [HttpPost]
        [Route("api/SeatMap/{id}/{userId}")]
        public bool Post(int id, int userId, JObject jsonObject)
        {
            try
            {
                List<string> numbers = new List<string>();
                dynamic jsonData = jsonObject;
                JArray numbersJson = jsonData.seatsNumber;
                foreach (var item in numbersJson)
                {
                    numbers.Add(item.ToObject<string>());
                }

                foreach (var item in numbers)
                {
                    var reservedSeat = new ReservedSeat()
                    {
                        Active = true,
                        BusTripId = id,
                        CreatedByUserId = userId,
                        ByAdmin = true,
                        CreatedDate = DateTime.Now,
                        SeatNumber = item
                    };

                    db.ReservedSeats.Add(reservedSeat);
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                logger.ErrorLoggers.Add(new ErrorLogger()
                {
                    ActionName = "update seats map",
                    CreatedDate = DateTime.Now,
                    Error = e.Message.ToString(),
                    UserId = userId
                });

                logger.SaveChanges();
                return false;
            }
            return true;
        }

    }
}