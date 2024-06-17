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
using static Android.Webkit.WebStorage;
using AndroidX.AppCompat.App;

namespace finalproject216303628
{
    [Activity(Label = "AddSubjectsActivity")]
    public class AddSubjectsActivity : AppCompatActivity
    {
        SQLiteConnection dbCommand; // connection to the database
        ISharedPreferences sp; // connection to the users shared prefrences

        string[] subjects; // array for saving subject names
        CheckBox[] pickedSubjects;// array for saving the checkboxes on the screen for iterability
        LinearLayout subjectLl;// linear layout to display subject and checkboxes to screen
        Button saveSubjectsBtn;// button for saving the picked subjects

        List<Subjects> existingSubjects;

        string origin;

        // connects screen to layout
        // connects to the DB and the shared preference and sets up the screen dynamically
        // finds the views from the layout
        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.Title = "TutorTrove";

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddSubjectsLayout);

            dbCommand = Helper.DbCommand();
            sp = Helper.Sp();

            origin = Intent.GetStringExtra("Origin") ?? "";

            subjects = Resources.GetStringArray(Resource.Array.subjects);
            pickedSubjects = new CheckBox[subjects.Length];
            subjectLl = FindViewById<LinearLayout>(Resource.Id.subjectsLl);
            saveSubjectsBtn = FindViewById<Button>(Resource.Id.saveSubjectsBtn);

            for (int i = 0; i < pickedSubjects.Length; i++)
            {
                LinearLayout l = new LinearLayout(this);
                l.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
                l.Orientation = Orientation.Horizontal;
                l.SetGravity(GravityFlags.Center);
                TextView subject = new TextView(this);
                subject.Text = subjects[i];
                subject.TextSize = 17f;
                subject.LayoutParameters = new LinearLayout.LayoutParams(500, 120);
                pickedSubjects[i] = new CheckBox(this);
                pickedSubjects[i].Checked = false;
                pickedSubjects[i].LayoutParameters = new LinearLayout.LayoutParams(80, 80);
                l.AddView(subject);
                l.AddView(pickedSubjects[i]);
                subjectLl.AddView(l);
            }

            HandleData();

            saveSubjectsBtn.Click += SaveSubjectsBtn_Click;
        }

        public void HandleData()
        {
            try
            {
                existingSubjects = dbCommand.Query<Subjects>($"SELECT * FROM Subjects WHERE Email = '{sp.GetString("email", "")}'");
                foreach(Subjects subject in existingSubjects)
                {
                    for(int i = 0; i < subjects.Length; i++)
                    {
                        if (subjects[i] == subject.subject)
                        {
                            pickedSubjects[i].Checked = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        // saves all the picked subjects after validating them and moves user to main page or login page
        // depending on whether or not he is logged in
        private void SaveSubjectsBtn_Click(object sender, EventArgs e)
        {
            Subjects subject;
            if (CheckSubjectValidity())
            {
                try
                {
                    int rows = dbCommand.Execute($"DELETE FROM Subjects WHERE Email = '{sp.GetString("email", "")}'");
                    for (int i = 0; i < pickedSubjects.Length; i++)
                    {
                        if (pickedSubjects[i].Checked)
                        {
                            subject = new Subjects(sp.GetString("email", ""), subjects[i]);
                            dbCommand.Insert(subject);
                        }
                    }
                    Toast.MakeText(this, "Success!", ToastLength.Short).Show();
                    bool loggedIn = sp.GetBoolean("loggedIn", false);
                    Intent intent = null;

                    var editor = sp.Edit();
                    editor.PutInt("IsTeacher", 1);
                    editor.Commit();

                    string q = $"UPDATE TutorsTable SET IsTeacher = 1 WHERE Email = '{sp.GetString("email", "")}'";
                    dbCommand.Execute(q);

                    if (loggedIn)
                    {
                        intent = new Intent(this, typeof(MainPageActivity));
                        StartActivity(intent);
                    }
                    else
                    {
                        intent = new Intent(this, typeof(LoginActivity));
                        StartActivity(intent);
                    }
                }
                catch (Exception ex)
                { 
                    Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                }
            }
            else
            {
                Toast.MakeText(this, "Must Pick At Least One Subject!", ToastLength.Long).Show();  
            }
        }

        // checks if user has picked at least one subject, returns true if he did else returns false
        public bool CheckSubjectValidity()
        {
            for (int i = 0; i < pickedSubjects.Length; i++)
            {
                if (pickedSubjects[i].Checked)
                    return true;
            }
            return false;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (origin != "SignUpActivity")
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
                    Toast.MakeText(this, "Currently Becoming One!", ToastLength.Short).Show();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}