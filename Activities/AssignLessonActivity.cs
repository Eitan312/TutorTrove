using Android.App;
using Android.Companion;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using SQLite;
using System.Drawing;
using static AndroidX.Core.Text.Util.LocalePreferences.FirstDayOfWeek;
using Android.Provider;
using Android.Graphics.Drawables;
using AndroidX.AppCompat.App;
using finalproject216303628.Broadcasts;

namespace finalproject216303628
{
    [Activity(Label = "AssignLessonActivity")]
    public class AssignLessonActivity : AppCompatActivity, Android.Views.View.IOnClickListener
    {
        // lists for every day, each specific list contains the lessons for that day
        List<Lessons> sunday = new List<Lessons>();
        List<Lessons> Monday = new List<Lessons>();
        List<Lessons> Tuesday = new List<Lessons>();
        List<Lessons> Wednesday = new List<Lessons>();
        List<Lessons> Thursday = new List<Lessons>();
        List<Lessons> Friday = new List<Lessons>();
        List<Lessons> Saturday = new List<Lessons>();
        List<Lessons>[] days = new List<Lessons>[7];// array for saving all the lists in a manner that is comfortable to loop

        List<Lessons> existingLessons;

        // lists for every day, each specific list contains the work hours for that day for the current teacher
        List<WorkingHours> sundayHours = new List<WorkingHours>();
        List<WorkingHours> MondayHours = new List<WorkingHours>();
        List<WorkingHours> TuesdayHours = new List<WorkingHours>();
        List<WorkingHours> WednesdayHours = new List<WorkingHours>();
        List<WorkingHours> ThursdayHours = new List<WorkingHours>();
        List<WorkingHours> FridayHours = new List<WorkingHours>();
        List<WorkingHours> SaturdayHours = new List<WorkingHours>();
        List<WorkingHours>[] workHours = new List<WorkingHours>[7];// array for saving all the lists in a manner that is comfortable to loop
        int day = 0;

        SQLiteConnection dbCommand;
        ISharedPreferences sp;

        LinearLayout assignLl;
        string[] times;
        Button[] hours;
        Button btnSaveAssign, btnReturnAssign;
        Spinner dayAssignSpinner;
        ScrollView assignSv;
        string email;
        string studentEmail;
        string origin;

        Tutors teacher;

        TextView displayDatesTv;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.Title = "TutorTrove";

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AssignLessonLayout);

            Intent intent = Intent;

            // Retrieve the data from the Intent
            email = intent.GetStringExtra("email");
            origin = intent.GetStringExtra("origin");

            Setup();
            GetData();
        }

        // Retrives the existing lessons and work hours for the specific teacher from the db
        public void GetData()
        {
            try
            {
                existingLessons = dbCommand.Query<Lessons>($"SELECT * FROM Lessons WHERE TeacherEmail = '{email}' AND NOT StudentEmail = '{studentEmail}'");

                List<Lessons> userLessons = dbCommand.Query<Lessons>($"SELECT * FROM Lessons WHERE TeacherEmail = '{email}' AND StudentEmail = '{studentEmail}'");
                foreach (Lessons lesson in userLessons)
                {
                    days[lesson._day - 1].Add(lesson);
                }

                List<WorkingHours> existingHours = dbCommand.Query<WorkingHours>($"SELECT * FROM WorkHours WHERE Email = '{email}'");
                foreach (WorkingHours eHour in existingHours)
                {
                    workHours[eHour.day - 1].Add(eHour);
                }
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        //displays the lessons for the chosen day
        public void DisplayData()
        {
            for(int i = 0; i < hours.Length; i++)
            {
                hours[i].Clickable = false;
                hours[i].SetBackgroundColor(Android.Graphics.Color.White);
                hours[i].Text = "";
            }

            foreach(WorkingHours wh in workHours[day - 1])
            {
                for(int i = wh.start; i < wh.end; i++)
                {
                    hours[i].SetBackgroundColor(Android.Graphics.Color.Green);
                    hours[i].Clickable = true;
                }
            }

            foreach(Lessons lesson in existingLessons)
            {
                if(lesson._day == day)
                {
                    for(int i = lesson._startHour; i < lesson._endHour; i++)
                    {
                        hours[i].SetBackgroundColor(Android.Graphics.Color.Red);
                        hours[i].Text = "X";
                        hours[i].Clickable = false;
                    }
                }
            }
            foreach(Lessons lesson in days[day - 1])
            {
                for(int i = lesson._startHour; i < lesson._endHour; i++)
                {
                    hours[i].SetBackgroundColor(Android.Graphics.Color.Green);
                    hours[i].Text = "X";
                }
            }
        }

        // Retrieves the button's background color
        public Android.Graphics.Color GetButtonBackgroundColor(Button btn)
        {
            if (btn.Background is ColorDrawable colorDrawable)
            {
                return colorDrawable.Color;
            }
            else
            {
                // Handle cases where the background is not a ColorDrawable
                return Android.Graphics.Color.Transparent;
            }
        }

        // saves temporarily the current selection and updates the screen
        public void ManageLessons()
        {
            int start;
            days[day - 1] = new List<Lessons>();
            for(int i = 0; i < hours.Length; i++)
            {
                if (GetButtonBackgroundColor(hours[i]) == Android.Graphics.Color.Green && hours[i].Text == "X")
                {
                    start = i;
                    while(i < hours.Length && hours[i].Text == "X")
                    {
                        i++;
                    }

                    if(DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) < DateTime.Now.Day - (int)(DateTime.Now.DayOfWeek) + 1)
                        days[day - 1].Add(new Lessons(new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, DateTime.Now.Day - (int)(DateTime.Now.DayOfWeek) + day - 1 - DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), int.Parse(times[i].Split(":")[0]), int.Parse(times[i].Split(":")[1]), 0), email, studentEmail, teacher.City, start, i, day));
                    else
                        days[day - 1].Add(new Lessons(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - (int)(DateTime.Now.DayOfWeek) + day - 1, int.Parse(times[i].Split(":")[0]), int.Parse(times[i].Split(":")[1]), 0), email, studentEmail, teacher.City, start, i, day));
                }
            }
        }

        //Sets up the screen and connects views from layout, also connects to DB and to the shared preference file and retrives the current teacher
        // also connects between the click function to the appropriate buttons
        public void Setup()
        {
            dbCommand = Helper.DbCommand();
            sp = Helper.Sp();

            studentEmail = sp.GetString("email", "");

            try
            {
                teacher = dbCommand.Get<Tutors>(email);
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }

            days[0] = sunday;
            days[1] = Monday;
            days[2] = Tuesday;
            days[3] = Wednesday;
            days[4] = Thursday;
            days[5] = Friday;
            days[6] = Saturday;

            workHours[0] = sundayHours;
            workHours[1] = MondayHours;
            workHours[2] = TuesdayHours;
            workHours[3] = WednesdayHours;
            workHours[4] = ThursdayHours;
            workHours[5] = FridayHours;
            workHours[6] = SaturdayHours;

            assignLl = FindViewById<LinearLayout>(Resource.Id.Llassign);
            btnSaveAssign = FindViewById<Button>(Resource.Id.btnSaveAssign);
            dayAssignSpinner = FindViewById<Spinner>(Resource.Id.dayAssignSpinner);
            assignSv = FindViewById<ScrollView>(Resource.Id.assignSv);
            btnReturnAssign = FindViewById<Button>(Resource.Id.btnReturnAssign);
            displayDatesTv = FindViewById<TextView>(Resource.Id.displayDatesTv);

            DateTime thisSunday = DateTime.Now.AddDays(-1 * (int)DateTime.Now.DayOfWeek);
            DateTime thisSaturday = DateTime.Now.AddDays(6 - (int)DateTime.Now.DayOfWeek);
            displayDatesTv.Text = $"* assignment is from {thisSunday.ToString("dd/M/yyyy")} - {thisSaturday.ToString("dd/M/yyyy")}";

            btnReturnAssign.Click += BtnReturnAssign_Click;
            btnSaveAssign.Click += BtnSaveAssign_Click;

            string[] items = Resources.GetStringArray(Resource.Array.days);

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, items);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            dayAssignSpinner.Adapter = adapter;

            dayAssignSpinner.ItemSelected += DayAssignSpinner_ItemSelected;

            times = Resources.GetStringArray(Resource.Array.times);
            hours = new Button[times.Length];

            for (int i = 0; i < times.Length; i++)
            {
                LinearLayout tempLl = new LinearLayout(this);
                tempLl.Orientation = Orientation.Horizontal;
                tempLl.SetGravity(GravityFlags.Center);
                tempLl.SetPadding(0, 0, 0, 15);
                tempLl.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
                TextView tempTv = new TextView(this);
                tempTv.Gravity = GravityFlags.Center;
                tempTv.SetPadding(0, 0, 50, 0);
                tempTv.Text = times[i];
                tempTv.TextSize = 18f;
                tempTv.LayoutParameters = new LinearLayout.LayoutParams(180, 80);
                hours[i] = new Button(this);
                hours[i].SetOnClickListener(this);
                hours[i].SetBackgroundColor(Android.Graphics.Color.White);
                hours[i].LayoutParameters = new LinearLayout.LayoutParams(150, 150);
                hours[i].TextSize = 20f;
                hours[i].Clickable = false;
                tempLl.AddView(tempTv);
                tempLl.AddView(hours[i]);
                assignLl.AddView(tempLl);
            }
        }

        private void BtnSaveAssign_Click(object sender, EventArgs e)    
        {
            ManageLessons();
            bool valid = ValidateHours();
            if (!valid)
            {
                Toast.MakeText(this, "Lessons have to be at least 1 hour long!", ToastLength.Long).Show();
            }
            else
            {
                try
                {
                    string q = $"DELETE FROM Lessons WHERE TeacherEmail = '{email}' AND StudentEmail = '{studentEmail}'";
                    int rows = dbCommand.Execute(q);
                    for (int i = 0; i < days.Length; i++)
                    {
                        foreach (Lessons lesson in days[i])
                        {
                            dbCommand.Insert(lesson);
                            Intent calendarIntent = new Intent(this, typeof(CalenderService));
                            calendarIntent.PutExtra("Date", lesson._date.Ticks);
                            calendarIntent.PutExtra("name", teacher.Name);
                            calendarIntent.PutExtra("City", lesson._city);
                            calendarIntent.PutExtra("StartHour", times[lesson._startHour]);
                            calendarIntent.PutExtra("EndHour", times[lesson._endHour]);
                            calendarIntent.PutExtra("year", lesson._date.Year);
                            calendarIntent.PutExtra("month", lesson._date.Month);
                            calendarIntent.PutExtra("day", lesson._date.Day);
                            StartService(calendarIntent);
                        }
                    }
                    Toast.MakeText(this, "Success!", ToastLength.Short).Show();

                    Return();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                }
            }
        }

        //checks that every lesson is atleast 1 hour
        public bool ValidateHours()
        {
            for (int i = 0; i < days.Length; i++)
            {
                foreach(Lessons lessons in days[i])
                {
                    if(lessons._startHour == lessons._endHour - 1) { return false; }
                    if (lessons._date.DayOfWeek < DateTime.Now.DayOfWeek) 
                    {
                        Toast.MakeText(this, "Cannot assign to past days...", ToastLength.Short).Show();
                        return false; 
                    }
                }
            }
            return true;
        }

        // Returns user to the page they came from
        private void BtnReturnAssign_Click(object sender, EventArgs e)
        {
            Return();
        }

        //Checks where the user came from and returns him
        public void Return()
        {
            Intent intent;
            switch (origin)
            {
                case "MainPageActivity":
                    intent = new Intent(this, typeof(MainPageActivity));
                    StartActivity(intent);
                    return;
                case "SearchActivity":
                    intent = new Intent(this, typeof(SearchActivity));
                    StartActivity(intent);
                    return;
            }
        }

        // handles the actions to take after an item was picked in the spinner
        private void DayAssignSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            if (spinner.SelectedItem.ToString() == "Pick A Day")
            {
                assignSv.Visibility = ViewStates.Invisible;
            }
            else if (e.Position != AdapterView.InvalidPosition)
            {
                assignSv.Visibility = ViewStates.Visible;

                if (day != 0)
                    ManageLessons();

                switch (dayAssignSpinner.SelectedItem.ToString())
                {
                    case "Pick A Day":
                        day = 0;
                        break;
                    case "Sunday":
                        day = 1;
                        break;
                    case "Monday":
                        day = 2;
                        break;
                    case "Tuesday":
                        day = 3;
                        break;
                    case "Wednesday":
                        day = 4;
                        break;
                    case "Thursday":
                        day = 5;
                        break;
                    case "Friday":
                        day = 6;
                        break;
                    case "Saturday":
                        day = 7;
                        break;
                }

                if(day != 0)
                    DisplayData();
            }
        }

        //handles the click action for the buttons
        public void OnClick(View v)
        {
            Button btn = (Button)v;

            if(btn.Text == "X")
            {
                btn.Text = "";
                return;
            }

            if(btn.Text == "")
            {
                btn.Text = "X";
                return;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (sp.GetInt("IsTeacher", 0) == 0)
                MenuInflater.Inflate(Resource.Menu.MenuForStudent, menu);
            else
                MenuInflater.Inflate(Resource.Menu.AssignLessonActivityMenu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Intent intent;
            switch (item.ItemId)
            {
                case Resource.Id.logoutMenuBtn:
                    bool keep = sp.GetBoolean("keep", false);
                    var editor = sp.Edit();
                    if (keep)
                    {
                        editor.PutString("name", "");
                        editor.PutString("gender", "");
                        editor.PutString("city", "");
                    }
                    else if (!keep)
                    {
                        editor.PutString("name", "");
                        editor.PutString("gender", "");
                        editor.PutString("city", "");
                        editor.PutString("email", "");
                        editor.PutString("pass", "");
                    }
                    editor.PutBoolean("loggedIn", false);
                    editor.Commit();
                    intent = new Intent(this, typeof(MainActivity));
                    return true;
                case Resource.Id.ShowWorkHoursMenuBtn:
                    intent = new Intent(this, typeof(ShowWorkHoursActivity));
                    StartActivity(intent);
                    return true;
                case Resource.Id.ProfileMenuBtn:
                    intent = new Intent(this, typeof(ProfileActivity));
                    intent.PutExtra("email", sp.GetString("email", ""));
                    StartActivity(intent);
                    return true;
                case Resource.Id.MainPageMenuBtn:
                    intent = new Intent(this, typeof(MainPageActivity));
                    StartActivity(intent);
                    return true;
                case Resource.Id.UpdateWorkHoursMenuBtn:
                    StartActivity(new Intent(this, typeof(AddWorkHoursActivity)));
                    return true;
                case Resource.Id.BecomeTeacherMenuBtn:
                    if (Helper.CalculateAge(Helper.DbCommand().Get<Tutors>(sp.GetString("email", "")).BirthDate) >= 16)
                        StartActivity(new Intent(this, typeof(AddWorkHoursActivity)));
                    else
                        Toast.MakeText(this, "Not Old Enough!", ToastLength.Short).Show();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}