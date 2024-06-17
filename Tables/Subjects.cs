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
using Android.Drm;

namespace finalproject216303628
{
    [Table("Subjects")]
    public class Subjects
    {
        [Column("Id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Column("Email")]
        public string email { get; set; }
        [Column("Subject")]
        public string subject { get; set; }

        public Subjects() { }

        public Subjects(string email, string subject)
        {
            this.email = email;
            this.subject = subject;
        }
    }
}