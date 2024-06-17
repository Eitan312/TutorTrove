using Android.App;
using Android.Content;
using Android.Drm;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Java.Sql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace finalproject216303628.Broadcasts
{
    [Service]
    [IntentFilter(new string[] { "com.yourname.FirstService" })]
    public class CalenderService : Service
    {
        public IBinder binder;

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            if (intent != null)
            {
                DateTime date = new DateTime(intent.GetLongExtra("Date", 0));
                string teacher = intent.GetStringExtra("name") ?? "";
                string city = intent.GetStringExtra("City") ?? "";
                string startHour = intent.GetStringExtra("StartHour") ?? "";
                string endHour = intent.GetStringExtra("EndHour") ?? "";
                int year = intent.GetIntExtra("year", 0);
                int month = intent.GetIntExtra("month", 0);
                int day = intent.GetIntExtra("day", 0);

                AddLessonToCalendar(date, teacher, city, startHour, endHour, year, month, day);
            }

            return StartCommandResult.NotSticky;
        }
        private void AddLessonToCalendar(DateTime date, string teacher, string city, string startHour, string endHour, int year, int month, int day)
        {
            DateTime startTime = DateTime.ParseExact(startHour, "HH:mm", CultureInfo.InvariantCulture);
            DateTime endTime = DateTime.ParseExact(endHour, "HH:mm", CultureInfo.InvariantCulture);

            // Combine the date with the parsed time to get full DateTime objects
            startTime = new DateTime(year, month, day, startTime.Hour, startTime.Minute, 0);
            endTime = new DateTime(year, month, day, endTime.Hour, endTime.Minute, 0);

            long eventTimeMillis = startTime.Ticks / TimeSpan.TicksPerMillisecond;

            // Create a calendar intent to insert the event
            Intent calendarIntent = new Intent(Intent.ActionInsert);
            calendarIntent.SetData(CalendarContract.Events.ContentUri);
            calendarIntent.PutExtra(CalendarContract.EventsColumns.CalendarId, 1); // Use 1 for the primary calendar
            calendarIntent.PutExtra(CalendarContract.EventsColumns.Title, $"Lesson with {teacher}");
            calendarIntent.PutExtra(CalendarContract.EventsColumns.Description, $"City: {city}");
            calendarIntent.PutExtra(CalendarContract.EventsColumns.EventTimezone, Java.Util.TimeZone.Default.ID);
            calendarIntent.PutExtra(CalendarContract.ExtraEventBeginTime, (startTime - DateTime.UnixEpoch).Ticks / TimeSpan.TicksPerMillisecond); // Convert ticks to milliseconds
            calendarIntent.PutExtra(CalendarContract.ExtraEventEndTime, (endTime - DateTime.UnixEpoch).Ticks / TimeSpan.TicksPerMillisecond);

            // Start the calendar activity to add the event
            StartActivity(calendarIntent);
        }

        public override IBinder OnBind(Intent intent)
        {
            binder = null;
            return binder;
        }
    }
}