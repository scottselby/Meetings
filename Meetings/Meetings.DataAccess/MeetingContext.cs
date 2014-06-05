namespace Meetings.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Meetings.Models;

    public partial class MeetingContext : DbContext
    {
        public MeetingContext()
            : base("name=MeetingContext")
        {
        }

        public virtual DbSet<Meetings.Models.Meeting> Meetings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Meeting>().Property(x => x.Latitude).HasPrecision(15, 8);
            modelBuilder.Entity<Meeting>().Property(x => x.Longitude).HasPrecision(15, 8);
            base.OnModelCreating(modelBuilder);
        }

    }
}
