using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GeocodingApi;
using Newtonsoft;
using System.Net;
using System.IO;
using System.Data;
using Meetings.Models;
using System.Data.Entity.Spatial;
using Meetings.DataAccess;

namespace Meetings.DbFiller
{
   public class Program
    {

        static void Main(string[] args)
        {

            Dictionary<int,string> days = new Dictionary<int,string>();
            days.Add(0, "SUNDAY");
            days.Add(1, "MONDAY");
            days.Add(2, "TUESDAY");
            days.Add(3, "WEDNESDAY");
            days.Add(4, "THURSDAY");
            days.Add(5, "FRIDAY");
            days.Add(6, "SATURDAY");

            using (StreamReader sr = new StreamReader("d:/aa1.csv"))
            {
                string currentLine;
                // currentLine will be null when the StreamReader reaches the end of file
                while ((currentLine = sr.ReadLine()) != null)
                {



                    var Address = currentLine.Split(',')[0].ToString().Split('-')[1].Trim() + currentLine.Split(',')[1].ToString().Trim('"') + ", IL";
                    {
                        Thread.Sleep(650);
                        Console.WriteLine(Address);
                        decimal[] latlong = GeoCodeAddress(Address);
                        var text = string.Format("POINT({0} {1})",  latlong[1].ToString(), latlong[0].ToString());
                        Meeting meeting = new Meeting();
                        meeting.Address = Address;
                        meeting.DateCreated = DateTime.Now;
                        meeting.DateModified = DateTime.Now;
                        meeting.DayOfWeek = (Models.Enums.DayOfWeek) days.Where(x => x.Value == currentLine.Split(',')[3].ToString()).FirstOrDefault().Key;
                        meeting.Geography = DbGeography.PointFromText(text, 4326);
                        meeting.Latitude = latlong[0];
                        meeting.Longitude = latlong[1];
                        meeting.LocationName = currentLine.Split(',')[0].ToString().Split('-')[0].Trim('"');
                        meeting.CasoID = currentLine.Split(',')[7].ToString();
                        meeting.MeetingName = currentLine.Split(',')[2].ToString();
                        meeting.MeetingType = Models.Enums.MeetingType.AA;
                        meeting.Options = "";
                        meeting.Time = Convert.ToDateTime(currentLine.Split(',')[5].ToString());
                        new MeetingRepository().AddNewMeeting(meeting);
                    }

                }
            }
        }


        public static decimal[] GeoCodeAddress(string Address)
        {
            decimal[] latlong = new decimal[2];
            string url = "https://maps.google.com/maps/api/geocode/xml?address=" + Address + "&sensor=false&key=AIzaSyB21WhpoVeB3YdfHK0usPbUO9ogVBwnnoo";
            WebRequest request = WebRequest.Create(url);
            using (WebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    DataSet dsResult = new DataSet();
                    dsResult.ReadXml(reader);
                    DataTable dtCoordinates = new DataTable();
                    dtCoordinates.Columns.AddRange(new DataColumn[4] { new DataColumn("Id", typeof(int)),
                        new DataColumn("Address", typeof(string)),
                        new DataColumn("Latitude",typeof(string)),
                        new DataColumn("Longitude",typeof(string)) });
                    foreach (DataRow row in dsResult.Tables["result"].Rows)
                    {
                        string geometry_id = dsResult.Tables["geometry"].Select("result_id = " + row["result_id"].ToString())[0]["geometry_id"].ToString();
                        DataRow location = dsResult.Tables["location"].Select("geometry_id = " + geometry_id)[0];
                        latlong[0] = Decimal.Parse(location["lat"].ToString());
                        latlong[1] = Decimal.Parse(location["lng"].ToString());
                    }
                    return latlong;
                }
            }

        }
    }
}
