using System;
using System.Data.Entity.Spatial;
using Meetings.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;
namespace Meetings.Models
{
    [Table("Meeting")]
    public partial class Meeting
    {
        public int MeetingId { get; set; }
        public string CasoID { get; set; }
        public string MeetingName { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DbGeography Geography { get; set; }
        public Enums.DayOfWeek DayOfWeek { get; set; }
        public DateTime Time { get; set; }
        public string Options { get; set; }
        public MeetingType MeetingType { get; set; } 
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
