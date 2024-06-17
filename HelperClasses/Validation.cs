using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace finalproject216303628
{
    public static class Validation
    {
        //checks for email validity using regular expressions
        public static bool IsEmailValid(string email)
        {
            if(email == null) return false;

            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            Regex regex = new Regex(pattern);

            return regex.IsMatch(email);
        }
        //checks for age validity using regular expressions
        public static bool IsAgeValid(DateTime birthDate)
        {
            int age = Helper.CalculateAge(birthDate);

            return age >= 13;
        }

        //checks full name validation
        public static bool IsFullNameValid(string fullName)
        {
            string pattern = @"^[a-zA-Z]+(?:\s[a-zA-Z]+)+$";

            Regex regex = new Regex(pattern);

            return regex.IsMatch(fullName);
        }

        //checks for phone validity using regular expressions
        public static bool IsPhoneValid(string phone)
        {
            if (phone == null) return false;

            if (phone.Length < 10) return false;

            if (phone[0] != '0' || phone[1] != '5') return false;

            string pattern = @"^\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$";

            return Regex.IsMatch(phone, pattern);
        }

        //checks for password validity using regular expressions
        public static bool IsPasswordValid(string password)
        {
            if (password == null) return false;

            if(password.Length < 8) return false;

            string pattern = "^[a-zA-Z0-9]+$";

            return Regex.IsMatch(password, pattern);
        }

        //confirms if the passwords match 
        public static bool ConfirmPassword(string password1, string password2)
        {
            if (password1 == null || password2 == null) return false;
            return password1.Equals(password2);
        }

        //checks if string contains only letters
        public static bool IsOnlyLetters(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            // Regex pattern to match only letters (both uppercase and lowercase)
            string pattern = @"^[a-zA-Z]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }
    }
}