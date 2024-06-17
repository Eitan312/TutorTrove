using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomNavigation;
using AndroidX.AppCompat.App;
using Java.Security;
using System;
using SQLite;
using System.Collections.Generic;
using static Android.Icu.Text.Transliterator;
using System.Runtime.Remoting.Contexts;

namespace finalproject216303628
{
    [Activity(Label = "MainPageActivity")]
    public class MainPageActivity : AppCompatActivity
    {
        TextView textMessage;
        TextView headerTv;
        LinearLayout bestLl, mathLl, physicsLl, chemLl, csLl;

        ISharedPreferences sp;
        SQLiteConnection db_command;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.Title = "TutorTrove";

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainPageLayout);

            sp = Helper.Sp();
            db_command = Helper.DbCommand();

            textMessage = FindViewById<TextView>(Resource.Id.message);
            headerTv = FindViewById<TextView>(Resource.Id.headerTv);

            bestLl = FindViewById<LinearLayout>(Resource.Id.bestLl);
            mathLl = FindViewById<LinearLayout>(Resource.Id.mathLl);
            physicsLl = FindViewById<LinearLayout>(Resource.Id.physicsLl);
            chemLl = FindViewById<LinearLayout>(Resource.Id.chemLl);
            csLl = FindViewById<LinearLayout>(Resource.Id.csLl);

            DisplayTeachers();

            headerTv.Text = $"Welcome {sp.GetString("name", "").Split(" ")[0]}!";
        }

        public void DisplayTeachers()
        {
            try
            {


                List<Tutors> bestTutors = db_command.Query<Tutors>("SELECT * FROM TutorsTable WHERE IsTeacher = '1' ORDER BY Rating");

                if(bestTutors.Count > 0)
                {
                    TutorAdapter bestTutorsAdapter = new TutorAdapter(this, bestTutors);

                    for (int i = 0; i < bestTutors.Count; i++)
                    {
                        View temp = bestTutorsAdapter.GetView(i, null, bestLl);
                        bestLl.AddView(temp);
                    }
                }
                else
                {
                    TextView emptyTv = new TextView(this);
                    emptyTv.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.MatchParent);
                    emptyTv.Gravity = GravityFlags.Center;
                    emptyTv.Text = "Empty!";
                    emptyTv.TextSize = 18f;
                    bestLl.AddView(emptyTv);
                }

                List<Tutors> mathTutors = db_command.Query<Tutors>("SELECT TutorsTable.* FROM TutorsTable INNER JOIN Subjects ON TutorsTable.Email = Subjects.Email WHERE Subjects.Subject = 'Math'");
                
                if (mathTutors.Count > 0)
                {
                    TutorAdapter mathTutorsAdapter = new TutorAdapter(this, mathTutors);

                    for (int i = 0; i < mathTutors.Count; i++)
                    {
                        View temp = mathTutorsAdapter.GetView(i, null, mathLl);
                        mathLl.AddView(temp);
                    }
                }
                else
                {
                    TextView emptyTv = new TextView(this);
                    emptyTv.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.MatchParent);
                    emptyTv.Gravity = GravityFlags.Center;
                    emptyTv.Text = "Empty!";
                    emptyTv.TextSize = 18f;
                    mathLl.AddView(emptyTv);
                }

                List<Tutors> physicsTutors = db_command.Query<Tutors>("SELECT TutorsTable.* FROM TutorsTable INNER JOIN Subjects ON TutorsTable.Email = Subjects.Email WHERE Subjects.Subject = 'Physics'");
                
                if (physicsTutors.Count > 0)
                {
                    TutorAdapter physicsTutorsAdapter = new TutorAdapter(this, physicsTutors);

                    for (int i = 0; i < physicsTutors.Count; i++)
                    {
                        View temp = physicsTutorsAdapter.GetView(i, null, physicsLl);
                        physicsLl.AddView(temp);
                    }
                }
                else
                {
                    TextView emptyTv = new TextView(this);
                    emptyTv.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.MatchParent);
                    emptyTv.Gravity = GravityFlags.Center;
                    emptyTv.Text = "Empty!";
                    emptyTv.TextSize = 18f;
                    physicsLl.AddView(emptyTv);
                }

                List<Tutors> chemTutors = db_command.Query<Tutors>("SELECT TutorsTable.* FROM TutorsTable INNER JOIN Subjects ON TutorsTable.Email = Subjects.Email WHERE Subjects.Subject = 'Chemistry'");

                if (chemTutors.Count > 0)
                {
                    TutorAdapter chemTutorsAdapter = new TutorAdapter(this, chemTutors);

                    for (int i = 0; i < chemTutors.Count; i++)
                    {
                        View temp = chemTutorsAdapter.GetView(i, null, chemLl);
                        chemLl.AddView(temp);
                    }
                }
                else
                {
                    TextView emptyTv = new TextView(this);
                    emptyTv.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.MatchParent);
                    emptyTv.Gravity = GravityFlags.Center;
                    emptyTv.Text = "Empty!";
                    emptyTv.TextSize = 18f;
                    chemLl.AddView(emptyTv);
                }

                List<Tutors> csTutors = db_command.Query<Tutors>("SELECT TutorsTable.* FROM TutorsTable INNER JOIN Subjects ON TutorsTable.Email = Subjects.Email WHERE Subjects.Subject = 'Computer Science'");

                if (csTutors.Count > 0)
                {
                    TutorAdapter csTutorsAdapter = new TutorAdapter(this, csTutors);

                    for (int i = 0; i < csTutors.Count; i++)
                    {
                        View temp = csTutorsAdapter.GetView(i, null, csLl);
                        csLl.AddView(temp);
                    }
                }
                else
                {
                    TextView emptyTv = new TextView(this);
                    emptyTv.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.MatchParent);
                    emptyTv.Gravity = GravityFlags.Center;
                    emptyTv.Text = "Empty!";
                    emptyTv.TextSize = 18f;
                    csLl.AddView(emptyTv);
                }
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.Message, ToastLength.Long).Show();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if(sp.GetInt("IsTeacher", 0) == 0)
                MenuInflater.Inflate(Resource.Menu.MenuForStudent, menu);
            else
                MenuInflater.Inflate(Resource.Menu.MainPageMenu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MainPageMenuBtn:
                    Toast.MakeText(this, "Already In Main Page!", ToastLength.Short).Show();
                    return true;
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
                    StartActivity(new Intent(this, typeof(LoginActivity)));
                    return true;
                case Resource.Id.addWorkHoursBtn:
                    StartActivity(new Intent(this, typeof(AddWorkHoursActivity)));
                    return true;
                case Resource.Id.showWorkHoursBtn:
                    if (Helper.CalculateAge(Helper.DbCommand().Get<Tutors>(sp.GetString("email", "")).BirthDate) >= 16)
                        StartActivity(new Intent(this, typeof(ShowWorkHoursActivity)));
                    else
                        Toast.MakeText(this, "Not Old Enough!", ToastLength.Short).Show();
                    return true;
                case Resource.Id.searchMenubtn:
                    StartActivity(new Intent(this, typeof(SearchActivity)));
                    return true;
                case Resource.Id.profileMenuBtn:
                    Intent intent = new Intent(this, typeof(ProfileActivity));
                    intent.PutExtra("email", sp.GetString("email", ""));
                    StartActivity(intent);
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