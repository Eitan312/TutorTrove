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
using System.Runtime.InteropServices;

namespace finalproject216303628
{
    [Table("Ratings")]
    public class Rating
    {
        [Column("Id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Column("Email")]
        public string email { get; set; }
        [Column("RaterEmail")]
        public string remail { get; set; }
        [Column("Description")]
        public string description { get; set; }
        [Column("Reply")]
        public string reply { get; set; }
        [Column("Rating")]
        public double rating { get; set; }

        public Rating() { }

        public Rating(string email, string remail, string description, double rating)
        {
            this.email = email;
            this.remail = remail;
            this.rating = rating;
            this.description = description;
            this.reply = "";
        }
    }
}