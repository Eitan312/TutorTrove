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
using System.Runtime.CompilerServices;

namespace finalproject216303628
{
    [Table("Lessons")]
    public class Lessons
    {
        [Column("Date"), PrimaryKey]
        public DateTime _date { get; set; }

        [Column("TeacherEmail")]
        public string _teacherEmail { get; set; }
        [Column("StudentEmail")]
        public string _studentEmail { get; set; }
        [Column("City")]
        public string _city { get; set; }
        [Column("StartHour")]
        public int _startHour { get; set; }
        [Column("EndHour")]
        public int _endHour { get; set; }
        [Column("Day")]
        public int _day { get; set; }
        public Lessons() { }

        public Lessons(DateTime date, string teacherEmail, string studentEmail, string city, int startHour, int endHour, int day)
        {
            this._date = date;
            _teacherEmail = teacherEmail;
            _studentEmail = studentEmail;
            _city = city;
            _startHour = startHour;
            _endHour = endHour;
            _day = day;
        }
    }
}