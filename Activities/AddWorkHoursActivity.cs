using Android.App;
using Android.Content;
using Android.Net.Wifi.Aware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using System.Net.NetworkInformation;
using AndroidX.AppCompat.App;
using static Android.Webkit.WebStorage;
using Java.Nio.Channels;

namespace finalproject216303628
{
    [Activity(Label = "AddWorkHoursAcrtivity")]
    public class AddWorkHoursActivity : AppCompatActivity, Android.Views.View.IOnClickListener
    {
        // lists for every day, each specific list contains the work hours for that day for the current user
        List<WorkingHours> sundayHours = new List<WorkingHours>();
        List<WorkingHours> MondayHours = new List<WorkingHours>();
        List<WorkingHours> TuesdayHours = new List<WorkingHours>();
        List<WorkingHours> WednesdayHours = new List<WorkingHours>();
        List<WorkingHours> ThursdayHours = new List<WorkingHours>();
        List<WorkingHours> FridayHours = new List<WorkingHours>();
        List<WorkingHours> SaturdayHours = new List<WorkingHours>();
        List<WorkingHours>[] hours = new List<WorkingHours>[7];// array for saving all the lists in a manner that is comfortable to loop
        int day = 0;// saves the current day the user picked
        string email;// saves the current users email

        Button btnUpdateHours;

        string origin;//contains the name of the activity the user came from

        // buttons for the user to select work hours
        Button btn1, btn2, btn3, btn4, btn5, btn6, btn7, btn8, btn9, btn10, btn11, btn12, btn13, btn14, btn15, btn16, btn17, btn18, btn19, btn20, btn21, btn22, btn23, btn24, btn25, btn26, btn27;
        Button[] btnHours = new Button[27];// array for saving the button so that i will be able to run through all the buttons comfortably
        Spinner days;// spinner for day selection
        ScrollView svSelect;// scroll view that displays all the buttons and hours

        SQLiteConnection db_command;
        ISharedPreferences sp;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.Title = "TutorTrove";

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddWorkHoursLayout);

            db_command = Helper.DbCommand();
            sp = Helper.Sp();

            //get origin
            origin = Intent.GetStringExtra("origin") ?? "";

            //Retrieves the views by their id
            //enters all the buttons in order into an array and sets them to the same listener
            //retrieves the list of working hours from the database for the current user
            Setup();
        }

        // Inserts the selected work hours into the database
        public bool InsertWorkHours()
        {
            try
            {
                bool chose = false;
                for (int i = 0; i < hours.Length; i++)
                {
                    if (hours[i].Count != 0) { chose = true; }
                }
                if (chose)
                {
                    string q = $"DELETE FROM WorkHours WHERE Email ='{email}'";
                    int rows = db_command.Execute(q);
                    foreach (List<WorkingHours> list in hours)
                    {
                        foreach (WorkingHours h in list)
                        {
                            db_command.Insert(h);
                        }
                    }
                    Toast.MakeText(this, "Success!", ToastLength.Long).Show();
                    return true;
                }
                else
                    throw new Exception("Didn't choose hours");
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        // sets up the layout by fitting the id's of every view to appropriate object and inserting items into appropriate arrays
        // also saves user information constants from the shared preference
        public void Setup()
        {
            email = sp.GetString("email", "");

            btn1 = FindViewById<Button>(Resource.Id.btn1);
            btn2 = FindViewById<Button>(Resource.Id.btn2);
            btn3 = FindViewById<Button>(Resource.Id.btn3);
            btn4 = FindViewById<Button>(Resource.Id.btn4);
            btn5 = FindViewById<Button>(Resource.Id.btn5);
            btn6 = FindViewById<Button>(Resource.Id.btn6);
            btn7 = FindViewById<Button>(Resource.Id.btn7);
            btn8 = FindViewById<Button>(Resource.Id.btn8);
            btn9 = FindViewById<Button>(Resource.Id.btn9);
            btn10 = FindViewById<Button>(Resource.Id.btn10);
            btn11 = FindViewById<Button>(Resource.Id.btn11);
            btn12 = FindViewById<Button>(Resource.Id.btn12);
            btn13 = FindViewById<Button>(Resource.Id.btn13);
            btn14 = FindViewById<Button>(Resource.Id.btn14);
            btn15 = FindViewById<Button>(Resource.Id.btn15);
            btn16 = FindViewById<Button>(Resource.Id.btn16);
            btn17 = FindViewById<Button>(Resource.Id.btn17);
            btn18 = FindViewById<Button>(Resource.Id.btn18);
            btn19 = FindViewById<Button>(Resource.Id.btn19);
            btn20 = FindViewById<Button>(Resource.Id.btn20);
            btn21 = FindViewById<Button>(Resource.Id.btn21);
            btn22 = FindViewById<Button>(Resource.Id.btn22);
            btn23 = FindViewById<Button>(Resource.Id.btn23);
            btn24 = FindViewById<Button>(Resource.Id.btn24);
            btn25 = FindViewById<Button>(Resource.Id.btn25);
            btn26 = FindViewById<Button>(Resource.Id.btn26);
            btn27 = FindViewById<Button>(Resource.Id.btn27);
            btnUpdateHours = FindViewById<Button>(Resource.Id.btnUpdateHours);
            days = FindViewById<Spinner>(Resource.Id.daySpinner);
            svSelect = FindViewById<ScrollView>(Resource.Id.svSelect);

            string[] items = Resources.GetStringArray(Resource.Array.days);

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, items);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            days.Adapter = adapter;

            days.ItemSelected += Days_ItemSelected;
            btnUpdateHours.Click += BtnUpdateHours_Click;

            btnHours[0] = btn1;
            btnHours[1] = btn2;
            btnHours[2] = btn3;
            btnHours[3] = btn4;
            btnHours[4] = btn5;
            btnHours[5] = btn6;
            btnHours[6] = btn7;
            btnHours[7] = btn8;
            btnHours[8] = btn9;
            btnHours[9] = btn10;
            btnHours[10] = btn11;
            btnHours[11] = btn12;
            btnHours[12] = btn13;
            btnHours[13] = btn14;
            btnHours[14] = btn15;
            btnHours[15] = btn16;
            btnHours[16] = btn17;
            btnHours[17] = btn18;
            btnHours[18] = btn19;
            btnHours[19] = btn20;
            btnHours[20] = btn21;
            btnHours[21] = btn22;
            btnHours[22] = btn23;
            btnHours[23] = btn24;
            btnHours[24] = btn25;
            btnHours[25] = btn26;
            btnHours[26] = btn27;

            for (int i = 0; i < btnHours.Length; i++)
            {
                btnHours[i].SetOnClickListener(this);
                btnHours[i].SetBackgroundResource(Resource.Drawable.buttonStyle);
            }

            hours[0] = sundayHours;
            hours[1] = MondayHours;
            hours[2] = TuesdayHours;
            hours[3] = WednesdayHours;
            hours[4] = ThursdayHours;
            hours[5] = FridayHours;
            hours[6] = SaturdayHours;

            GetHours();
        }

        // saves/updates the teacher's hours into the database and moves him to login/mainpage
        private void BtnUpdateHours_Click(object sender, EventArgs e)
        {
            ManageHours();
            if (!CheckHoursValidity())
            {
                Toast.MakeText(this, "Lessons have to be at least 1 hour!", ToastLength.Short).Show();
                return;
            }
            var editor = sp.Edit();
            bool saved = InsertWorkHours();
            if (!saved)
            {
                Toast.MakeText(this, "Must pick work hours for teaching!", ToastLength.Long).Show();
            }
            else
            {
                Intent intent = new Intent(this, typeof(AddSubjectsActivity));
                intent.PutExtra("Origin", origin);
                StartActivity(intent);
            }
        }

        // gets the exising selected work hours from the database
        public void GetHours()
        {
            try
            {
                string q = $"SELECT * FROM WorkHours WHERE Email = '{email}'";
                List<WorkingHours> existingHours = db_command.Query<WorkingHours>(q);
                foreach(WorkingHours eHour in existingHours) 
                {
                    hours[eHour.day - 1].Add(eHour);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        //handles displaying appropriate work hours for selected day
        private void Days_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            if(spinner.SelectedItem.ToString() == "Pick A Day")
            {
                svSelect.Visibility = ViewStates.Invisible;
            }
            else if(e.Position != AdapterView.InvalidPosition)
            {
                svSelect.Visibility = ViewStates.Visible;

                if(day != 0)
                    ManageHours();

                switch (days.SelectedItem.ToString())
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
                    ShowHours(hours[day - 1]);
            }
        }

        // displays the work hours to the user
        public void ShowHours(List<WorkingHours> lst)
        {
            for (int j = 0; j < btnHours.Length; j++)
            {
                btnHours[j].Text = "";
            }
            foreach (WorkingHours h in lst)
            {
                EnterHours(h.start, h.end);
            }
        }

        // displays specific work hours range from the list to the user
        public void EnterHours(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                btnHours[i].Text = "X";
            }
        }

        // goes through the list and searches for X (picked), it then counts how long it is by running until it finds a " "
        // it then saves the start and end and creates a work hours object that is inserted to the appropriate list with the day
        // it then changes the day obejct for this class to represent the new chosen day
        public void ManageHours()
        {
            int start;
            int end;
            hours[day - 1] = new List<WorkingHours>();
            int i = 0;
            while (i < btnHours.Length)
            {
                if (btnHours[i].Text == "X")
                {
                    start = i;
                    while (i < btnHours.Length && btnHours[i].Text == "X")
                    {
                        i++;
                    }
                    end = i;
                    hours[day - 1].Add(new WorkingHours(email, day, start, end));
                }
                i++;
            }
        }

        //checks the selected hours' validity, if user checks working hours for a lesson that is less than 1 hour it is invalids
        public bool CheckHoursValidity()
        {
            foreach (List<WorkingHours> list in hours)
            {
                foreach (WorkingHours h in list)
                {
                    if(h.start == h.end - 1) 
                        return false;
                }
            }
            return true;
        }

        // in charge of how the user will select the hours
        public void OnClick(View v)
        {
            Button btn = (Button)v;
            int i;
            for (i = 0; i < btnHours.Length; i++)
            {
                if (btnHours[i] == v)
                    break;
            }
            if(i == 26)
            {
                if (btnHours[i-1].Text == "")
                {
                    Toast.MakeText(this, "Lessons have to be at least 1 hours long!", ToastLength.Long).Show();
                    return;
                }
            }
            if(btn.Text == "")
            {
                while (i < btnHours.Length && btnHours[i].Text != "X")
                {
                    btnHours[i].Text = "X";
                    i++;
                }
            }
            else if(btn.Text == "X")
            {
                btn.Text = "";
                if (i == 26) { return; }
                i++;
                
                while(i < btnHours.Length && btnHours[i].Text != "")
                {
                    btnHours[i].Text = "";
                    i++;
                }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if(origin != "SignUpActivity")
            {
                if (sp.GetInt("IsTeacher", 0) == 0)
                    MenuInflater.Inflate(Resource.Menu.MenuForStudent, menu);
                else
                    MenuInflater.Inflate(Resource.Menu.AssignLessonActivityMenu, menu);
                return true;
            }
            return false;
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
                    Toast.MakeText(this, "Currently Becoming One!", ToastLength.Short).Show();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}