using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meetings.Models;
using Meetings.DataAccess;
using System.Data.Entity.Spatial;
using Meetings.DataAccess.Interfaces;

namespace Meetings.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMeetingRepository _meetingRepository;

        public  HomeController(IMeetingRepository meetingRepository)
        {
             _meetingRepository = meetingRepository;
        }

        public ActionResult Index()
        {
           // List<Meeting> thisMeeting = new MeetingRepository().GetMeetingsInRadius(42.049516m, -87.920601m, 20);
            return View("Index2");
        }

        [HttpGet]
        public ActionResult AddMeeting()
        {
            Meeting meeting = new Meeting();
            return View(meeting);
        }

        [HttpPost]
        public ActionResult AddMeeting(Meeting _meeting)
        {
            decimal[] latlong = Meetings.DbFiller.Program.GeoCodeAddress(_meeting.Address);
            var text = string.Format("POINT({0} {1})", latlong[1].ToString(), latlong[0].ToString());
            _meeting.Geography = DbGeography.PointFromText(text, 4326);
            _meeting.Latitude = latlong[0];
            _meeting.Longitude = latlong[1];
            _meeting.DateCreated = DateTime.Now;
            _meeting.DateModified = DateTime.Now;
            new Meetings.DataAccess.MeetingRepository().AddNewMeeting(_meeting);
            return View(_meeting);        
        }

        [HttpPost]
        public ActionResult GetMeetingsInRadius(decimal latitude, decimal longitude, int radius)
        {
            var result = _meetingRepository.GetMeetingsInRadius(latitude, longitude, radius);
            return this.Json(result);
        }
    }
}