using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meetings.Models;
using System.Data.Entity.ModelConfiguration.Conventions;
using  System.Configuration;


namespace Meetings.DataAccess
{
    public class MeetingDbContext : DbContext
    {
        public MeetingDbContext() : base("MeetingDbContext")
        {
        
        } 
         public DbSet<Meeting> Meeting { get; set; }
        
        


    }
}
