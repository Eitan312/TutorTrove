using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.Webkit.WebStorage;
using static AndroidX.Core.Text.Util.LocalePreferences.FirstDayOfWeek;
using static Java.Util.Jar.Attributes;

namespace finalproject216303628
{
    [Activity(Label = "ShowWorkHoursActivity")]
    public class ShowWorkHoursActivity : AppCompatActivity
    {
        // lists for each day of the week for saving the hours to a specific day (to save runtime)
        List<WorkingHours> SundayHours = new List<WorkingHours>();
        List<WorkingHours> MondayHours = new List<WorkingHours>();
        List<WorkingHours> TuesdayHours = new List<WorkingHours>();
        List<WorkingHours> WednesdayHours = new List<WorkingHours>();
        List<WorkingHours> ThursdayHours = new List<WorkingHours>();
        List<WorkingHours> FridayHours = new List<WorkingHours>();
        List<WorkingHours> SaturdayHours = new List<WorkingHours>();
        List<WorkingHours>[] hours = new List<WorkingHours>[7];// list array for saving all the work hour lists for easier looping

        Spinner daySpinner;// spinner for day selection
        Button btnReturn;// button for returning to main page

        // textviews for displaying chosen work hours
        TextView tv1, tv2, tv3, tv4, tv5, tv6, tv7, tv8, tv9, tv10, tv11, tv12, tv13, tv14, tv15, tv16, tv17, tv18, tv19, tv20, tv21, tv22, tv23, tv24, tv25, tv26, tv27;
        TextView[] tvHours = new TextView[27]; // array for saving all the textviews for easier looping
        ScrollView svSelect;// scroll view for displaying the chosen hours

        int day = 0;
        SQLiteConnection db_command;
        ISharedPreferences sp;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.Title = "TutorTrove";

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ShowWorkHoursLayout);

            db_command = Helper.DbCommand();
            sp = Helper.Sp();

            Setup();
        }

        // sets up the layout by fitting every object with the appropriate view by its id
        // also enters the views into specified arrays for easier looping
        public void Setup()
        {
            btnReturn = FindViewById<Button>(Resource.Id.btnReturn);
            tv1 = FindViewById<TextView>(Resource.Id.tv1);
            tv2 = FindViewById<TextView>(Resource.Id.tv2);
            tv3 = FindViewById<TextView>(Resource.Id.tv3);
            tv4 = FindViewById<TextView>(Resource.Id.tv4);
            tv5 = FindViewById<TextView>(Resource.Id.tv5);
            tv6 = FindViewById<TextView>(Resource.Id.tv6);
            tv7 = FindViewById<TextView>(Resource.Id.tv7);
            tv8 = FindViewById<TextView>(Resource.Id.tv8);
            tv9 = FindViewById<TextView>(Resource.Id.tv9);
            tv10 = FindViewById<TextView>(Resource.Id.tv10);
            tv11 = FindViewById<TextView>(Resource.Id.tv11);
            tv12 = FindViewById<TextView>(Resource.Id.tv12);
            tv13 = FindViewById<TextView>(Resource.Id.tv13);
            tv14 = FindViewById<TextView>(Resource.Id.tv14);
            tv15 = FindViewById<TextView>(Resource.Id.tv15);
            tv16 = FindViewById<TextView>(Resource.Id.tv16);
            tv17 = FindViewById<TextView>(Resource.Id.tv17);
            tv18 = FindViewById<TextView>(Resource.Id.tv18);
            tv19 = FindViewById<TextView>(Resource.Id.tv19);
            tv20 = FindViewById<TextView>(Resource.Id.tv20);
            tv21 = FindViewById<TextView>(Resource.Id.tv21);
            tv22 = FindViewById<TextView>(Resource.Id.tv22);
            tv23 = FindViewById<TextView>(Resource.Id.tv23);
            tv24 = FindViewById<TextView>(Resource.Id.tv24);
            tv25 = FindViewById<TextView>(Resource.Id.tv25);
            tv26 = FindViewById<TextView>(Resource.Id.tv26);
            tv27 = FindViewById<TextView>(Resource.Id.tv27);
            daySpinner = FindViewById<Spinner>(Resource.Id.daySpinner);
            svSelect = FindViewById<ScrollView>(Resource.Id.svSelect);

            tvHours[0] = tv1;
            tvHours[1] = tv2;
            tvHours[2] = tv3;
            tvHours[3] = tv4;
            tvHours[4] = tv5;
            tvHours[5] = tv6;
            tvHours[6] = tv7;
            tvHours[7] = tv8;
            tvHours[8] = tv9;
            tvHours[9] = tv10;
            tvHours[10] = tv11;
            tvHours[11] = tv12;
            tvHours[12] = tv13;
            tvHours[13] = tv14;
            tvHours[14] = tv15;
            tvHours[15] = tv16;
            tvHours[16] = tv17;
            tvHours[17] = tv18;
            tvHours[18] = tv19;
            tvHours[19] = tv20;
            tvHours[20] = tv21;
            tvHours[21] = tv22;
            tvHours[22] = tv23;
            tvHours[23] = tv24;
            tvHours[24] = tv25;
            tvHours[25] = tv26;
            tvHours[26] = tv27;

            hours[0] = SundayHours;
            hours[1] = MondayHours;
            hours[2] = TuesdayHours;
            hours[3] = WednesdayHours;
            hours[4] = ThursdayHours;
            hours[5] = FridayHours;
            hours[6] = SaturdayHours;

            string[] items = Resources.GetStringArray(Resource.Array.days);

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, items);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            daySpinner.Adapter = adapter;

            GetHours();

            daySpinner.ItemSelected += DaySpinner_ItemSelected;
            btnReturn.Click += BtnReturn_Click;
        }

        // transfers user back to main page
        private void BtnReturn_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MainPageActivity));
            StartActivity(intent);
        }

        //in charge of dispalying work hours and handling day picking
        private void DaySpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            if (spinner.SelectedItem.ToString() == "Pick A Day")
            {
                svSelect.Visibility = ViewStates.Invisible;
            }
            else if (e.Position != AdapterView.InvalidPosition)
            {
                svSelect.Visibility = ViewStates.Visible;
                switch (daySpinner.SelectedItem.ToString())
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
                if (day != 0)
                    ShowHours();
            }
        }

        //gets the existing work hours of the current user from the database
        public void GetHours()
        {
            try
            {
                string q = $"SELECT * FROM WorkHours WHERE Email = '{sp.GetString("email", "")}'";
                List<WorkingHours> existingHours = db_command.Query<WorkingHours>(q);
                foreach (WorkingHours eHour in existingHours)
                {
                    hours[eHour.day - 1].Add(eHour);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        // displays users work hours to user in a specific day
        public void ShowHours()
        {
            for(int i =0; i< tvHours.Length; i++)
            {
                tvHours[i].Text = "";
            }
            if (hours[day - 1].Count == 0) { return; }
            foreach(WorkingHours wHour in hours[day - 1])
            {
                EnterHours(wHour.start, wHour.end);
            }
        }

        // displays specific work hour range to user
        public void EnterHours(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                tvHours[i].Text = "X";
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
                    if (sp.GetInt("IsTeacher", 0) == 1)
                    {
                        intent = new Intent(this, typeof(ShowWorkHoursActivity));
                        StartActivity(intent);
                    }
                    else
                        Toast.MakeText(this, "Not A Teacher!", ToastLength.Short).Show();
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
                    if (Helper.CalculateAge(Helper.DbCommand().Get<Tutors>(sp.GetString("email", "")).BirthDate) >= 16)
                        StartActivity(new Intent(this, typeof(AddWorkHoursActivity)));
                    else
                        Toast.MakeText(this, "Not Old Enough!", ToastLength.Short).Show();
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