using Meetings.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meetings.Models.DTO
{
    public class MeetingVM
    {
        public string CasoID { get; set; }
        public string MeetingName { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public Enums.DayOfWeek DayOfWeek { get; set; }
        public DateTime Time { get; set; }
        public string Options { get; set; }
        public MeetingType MeetingType { get; set; }
        public double? distance { get; set; }
    }
}
