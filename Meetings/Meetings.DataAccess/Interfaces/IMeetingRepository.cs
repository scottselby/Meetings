using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meetings.Models;
using Meetings.Models.DTO;

namespace Meetings.DataAccess.Interfaces
{
    public interface IMeetingRepository
    {
        List<Meeting> GetFirstMeeting();
        List<MeetingVM> GetMeetingsInRadius(decimal latitude, decimal longitude, int miles);
    }
}
