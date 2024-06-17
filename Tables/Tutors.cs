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
using Android.Net.Wifi.Aware;
using System.Diagnostics.Contracts;

namespace finalproject216303628
{
    [Table("TutorsTable")]
    public class Tutors
    {
        [PrimaryKey, Column("Email")]
        public string Email {  get; set; }
        [Column("FullName")]
        public string Name { get; set; }
        [Column("City")]
        public string City { get; set; }
        [Column("Phone")]
        public string phone { get; set; }
        [Column("Gender")]
        public string Gender { get; set; }
        [Column("BirthDate")]
        public DateTime BirthDate { get; set; }
        [Column("Password")]
        public string Password { get; set; }
        [Column("IsTeacher")]
        public int isTeacher { get; set; }
        [Column("ProfilePicture")]
        public string profilePicture { get; set; }
        [Column("Description")]
        public string description { get; set; }
        [Column("Rating")]
        public double rating { get; set; }

        public Tutors() { }

        public Tutors(string email, string name, string city, string phone,string gender, DateTime birthDate, string password, string profilePicture, string description)
        {
            this.Email = email;
            this.Name = name;
            this.City = city;
            this.phone = phone;
            this.Gender = gender;
            this.BirthDate = birthDate;
            this.Password = password;
            this.isTeacher = 0;
            this.profilePicture = profilePicture;
            this.description = description;
            this.rating = 0.0;
        }
    }
}