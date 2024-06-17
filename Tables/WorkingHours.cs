using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace finalproject216303628
{
    [Table("WorkHours")]
    public class WorkingHours
    {
        [Column("Id"), PrimaryKey, AutoIncrement]
        public int id { get; set; }
        [Column("Email")]
        public string email { get; set; }
        [Column("Day")]
        public int day { get; set; }
        [Column("startingHour")]
        public int start { get; set; }
        [Column("endHour")]
        public int end { get; set; }

        public WorkingHours() { }

        public WorkingHours(string email, int day, int startingHours, int endHours)
        {
            this.email = email;
            this.day = day;
            this.start = startingHours;
            this.end = endHours;
        }
    }
}