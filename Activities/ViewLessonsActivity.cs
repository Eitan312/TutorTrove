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
using finalproject216303628.Adapters;
using AndroidX.AppCompat.App;
using System.Runtime.Remoting.Contexts;
using Android.Media;

namespace finalproject216303628
{
    [Activity(Label = "ViewLessonsActivity")]
    public class ViewLessonsActivity : AppCompatActivity
    {
        ListView viewLessonsLv;
        TextView noLessonTv;
        List<Lessons> lessons;

        SQLiteConnection dbCommand;
        ISharedPreferences sp;

        string email;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.Title = "TutorTrove";

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewLessonsLayout);

            email = Intent.GetStringExtra("email");

            viewLessonsLv = FindViewById<ListView>(Resource.Id.viewLessonsLv);
            noLessonTv = FindViewById<TextView>(Resource.Id.noLessonTv);

            dbCommand = Helper.DbCommand();
            sp = Helper.Sp();

            DisplayLessons();
        }

        public void DisplayLessons()
        {
            try
            {
                string q;

                if(sp.GetInt("IsTeacher", 0) == 1)
                    q = $"SELECT * FROM Lessons WHERE TeacherEmail = '{email}' ORDER BY Date ASC";
                else
                    q = $"SELECT * FROM Lessons WHERE StudentEmail = '{email}' ORDER BY Date ASC";

                lessons = dbCommand.Query<Lessons>(q);

                if (lessons.Count == 0)
                {
                    viewLessonsLv.Visibility = ViewStates.Gone;
                    noLessonTv.Visibility = ViewStates.Visible;
                    return;
                }

                viewLessonsLv.Adapter = new LessonsAdapter(this, lessons);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (sp.GetInt("IsTeacher", 0) == 0)
                MenuInflater.Inflate(Resource.Menu.MenuForStudent, menu);
            else
                MenuInflater.Inflate(Resource.Menu.ViewLessonsMenu, menu);
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