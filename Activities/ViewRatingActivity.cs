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

namespace finalproject216303628
{
    [Activity(Label = "ViewRatingActivity")]
    public class ViewRatingActivity : AppCompatActivity
    {
        ListView ratingLv;
        TextView noRatingTv;

        string email;

        SQLiteConnection dbCommand;
        ISharedPreferences sp;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.Title = "TutorTrove";

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ViewRatingsLayout);

            ratingLv = FindViewById<ListView>(Resource.Id.ratingLv);
            noRatingTv = FindViewById<TextView>(Resource.Id.noRatingTv);

            email = Intent.GetStringExtra("email");

            dbCommand = Helper.DbCommand();
            sp = Helper.Sp();

            DisplayRatings();
        }

        public void DisplayRatings()
        {
            try
            {
                List<Rating> ratings = dbCommand.Query<Rating>($"SELECT * FROM Ratings WHERE Email = '{email}'");

                if(ratings.Count == 0)
                {
                    ratingLv.Visibility = ViewStates.Gone;
                    noRatingTv.Visibility = ViewStates.Visible;
                    return;
                }

                ratingLv.Adapter = new RatingAdapter(this, ratings);
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (sp.GetInt("IsTeacher", 0) == 0)
                MenuInflater.Inflate(Resource.Menu.MenuForStudent, menu);
            else
                MenuInflater.Inflate(Resource.Menu.viewRatingsMenu, menu);
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