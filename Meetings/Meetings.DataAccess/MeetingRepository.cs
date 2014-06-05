using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meetings.DataAccess.Interfaces;
using Meetings.Models;
using System.Data.Entity.Spatial;
using Meetings.Models.DTO;

namespace Meetings.DataAccess
{
   public  class MeetingRepository : IMeetingRepository
    {

       public List<MeetingVM> GetMeetingsInRadius(decimal latitude, decimal longitude, int miles)
       {
           var point = DbGeography.FromText(string.Format("POINT ({0} {1})", longitude, latitude), 4326);
           using (MeetingContext db = new MeetingContext())
           {
               var results = (from a in db.Meetings
                              where a.Geography.Distance(point) * 0.000621371 <= miles
                              select a).Select(x => new MeetingVM
                              {
                                  Address = x.Address,
                                  CasoID = x.Address,
                                  DayOfWeek = x.DayOfWeek,
                                  distance = x.Geography.Distance(point) * 0.000621371,
                                  Latitude = x.Latitude,
                                  LocationName = x.LocationName,
                                  Longitude = x.Longitude,
                                  MeetingName = x.MeetingName,
                                  MeetingType = x.MeetingType,
                                  Options = x.Options,
                                  Time = x.Time
                              }).OrderBy(x => x.Time).ToList();               
               return results;
           }           
       }


       public List<Meeting> GetFirstMeeting()
       {
           using (MeetingDbContext db = new MeetingDbContext())
           {
               var firstMeeting = db.Meeting.ToList();
               return firstMeeting;
           }          
       }


       public void AddNewMeeting(Meeting meeting)
       {
           using (MeetingContext db = new MeetingContext())
           {
               db.Meetings.Add(meeting);
               db.SaveChanges();
               return;
           }    
       }
    }
}
