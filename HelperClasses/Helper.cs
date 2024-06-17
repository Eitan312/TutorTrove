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
using System.IO;
using System.Drawing;
using Android.Util;
using Android.Graphics;
using Android.Content;
using System.Net.Mail;
using Android.Telephony;

namespace finalproject216303628
{
    public static class Helper
    {
        private static string dbName = "TutorTroveDB";
        private static ISharedPreferences sp;
        private static SQLiteConnection dbCommand;

        public static void Initialize(Activity activity)
        {
            string path = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbName);
            dbCommand = new SQLiteConnection(path);

            sp = activity.GetSharedPreferences("details", Android.Content.FileCreationMode.Private);
            try
            {
                dbCommand.CreateTable<Tutors>();
                dbCommand.CreateTable<Lessons>();
                dbCommand.CreateTable<WorkingHours>();
                dbCommand.CreateTable<Subjects>();
                dbCommand.CreateTable<Rating>();
            }
            catch(Exception e)
            {
                Toast.MakeText(activity, e.Message, ToastLength.Long).Show();
            }
        }

        public static ISharedPreferences Sp()
        {
            return sp;
        }

        public static SQLiteConnection DbCommand()
        {
            return dbCommand;
        }

        public static string EncodeImage(Android.Graphics.Bitmap bitmap)
        {
            string str = "";
            using(var stream = new MemoryStream())
            {
                bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 0, stream);
                var bytes = stream.ToArray();
                str = Convert.ToBase64String(bytes);
            }
            return str;
        }

        public static int CalculateAge(DateTime birthday)
        {
            int age = DateTime.Now.Year - birthday.Year;

            //checks if the birthday has occured this year, if it didnt it will remove 1 from the age
            //for exmaple: if he was born in 17/1/2000 and the current time is 12/1/2024 it will first say he is 24 and then check and see that he has yet to celebrate his
            //birthday and will subtract one laving him 23 years old
            if (DateTime.Now.Month < birthday.Month || (DateTime.Now.Month < birthday.Month && DateTime.Now.Day < birthday.Day))
            {
                age--;
            }

            return age;
        }
        public static void SendEmail(Activity context, string email, string message)
        {
            string mailTo = email;
            MailMessage objeto_mail = new MailMessage();
            SmtpClient client = new SmtpClient();

            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.Timeout = 20000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("YudBet4IroniA@gmail.com", "qhip immedcek jgus");//the details

            //sending email and its password
            objeto_mail.From = new MailAddress("YudBet4IroniA@gmail.com");//from
            objeto_mail.To.Add(new MailAddress(mailTo));//to
            client.EnableSsl = true;
            objeto_mail.Subject = "Password Recovery - TutorTrove";

            objeto_mail.Body = message;
            client.Send(objeto_mail);

            Toast.MakeText(context, "Email sent", ToastLength.Long).Show();
        }

        public static void SendSms(Activity context, string phone, string message)
        {
            var smsManager = SmsManager.Default;

            PendingIntent sentPI = PendingIntent.GetBroadcast(context, 0, new Intent("SMS_SENT"), 0);

            try
            {
                smsManager.SendTextMessage(phone, null, $"Your verification code is: {message}", sentPI, null);
            }
            catch (Exception e)
            {
                Toast.MakeText(context, e.Message, ToastLength.Long).Show();
            }
        }

        public static Android.Graphics.Bitmap DecodeImage(string strImage)
        {
            byte[] bytes = Convert.FromBase64String(strImage);
            return BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
        }
    }
}