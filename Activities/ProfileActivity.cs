using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using SQLite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace finalproject216303628
{
    [Activity(Label = "ProfileActivity")]
    public class ProfileActivity : AppCompatActivity
    {
        ImageButton pfp;
        Button rateBtn, viewRateBtn, viewLessonBtn;
        TextView tvPfName, tvPfAge, tvPfCity, tvPfEmail, tvPfGender, tvPfDescription;
        Bitmap pfpBitmap;

        SQLiteConnection dbCommand;
        ISharedPreferences sp;

        string email;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.Title = "TutorTrove";

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ProfilePageLayout);

            dbCommand = Helper.DbCommand();
            sp = Helper.Sp();

            Setup();
        }

        public void Setup()
        {
            pfp = FindViewById<ImageButton>(Resource.Id.ibProfilePic);
            tvPfName = FindViewById<TextView>(Resource.Id.tvPfName);
            tvPfAge = FindViewById<TextView>(Resource.Id.tvPfAge);
            tvPfCity = FindViewById<TextView>(Resource.Id.tvPfCity);
            tvPfEmail = FindViewById<TextView>(Resource.Id.tvPfEmail);
            tvPfGender = FindViewById<TextView>(Resource.Id.tvPfGender);
            tvPfDescription = FindViewById<TextView>(Resource.Id.tvPfDescription);
            rateBtn = FindViewById<Button>(Resource.Id.rateBtn);
            viewRateBtn = FindViewById<Button>(Resource.Id.viewRateBtn);
            viewLessonBtn = FindViewById<Button>(Resource.Id.viewLessonBtn);

            Tutors currentUser;

            email = Intent.GetStringExtra("email");

            if(email == sp.GetString("email", "")) { rateBtn.Visibility = ViewStates.Gone; }

            try
            {
                currentUser = dbCommand.Get<Tutors>(email);

                if(currentUser.isTeacher == 0) 
                { 
                    rateBtn.Visibility = ViewStates.Gone; 
                    viewRateBtn.Visibility = ViewStates.Gone;
                }

                if(email != sp.GetString("email", "d"))
                {
                    viewLessonBtn.Visibility = ViewStates.Gone;
                    rateBtn.SetPadding(0, 0, 0, 0);
                }

                pfp.SetImageBitmap(Helper.DecodeImage(currentUser.profilePicture));
                tvPfName.Text += currentUser.Name;
                tvPfAge.Text += Helper.CalculateAge(currentUser.BirthDate);
                tvPfCity.Text += currentUser.City;
                tvPfEmail.Text += currentUser.Email;
                tvPfGender.Text += currentUser.Gender;
                tvPfDescription.Text += currentUser.description;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }

            rateBtn.Click += RateBtn_Click;
            viewRateBtn.Click += ViewRateBtn_Click;
            viewLessonBtn.Click += ViewLessonBtn_Click;
        }

        private void ViewLessonBtn_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ViewLessonsActivity));
            intent.PutExtra("email", sp.GetString("email", ""));
            StartActivity(intent);
        }

        private void ViewRateBtn_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ViewRatingActivity));
            intent.PutExtra("email", email);
            StartActivity(intent);
        }

        private void RateBtn_Click(object sender, EventArgs e)
        {
            Dialog dialog = new Dialog(this);
            dialog.SetContentView(Resource.Layout.RateDialog);

            RatingBar ratingBar = dialog.FindViewById<RatingBar>(Resource.Id.ratingBar);
            ratingBar.StepSize = 0.5f;
            ratingBar.NumStars = 5;

            Button saveRateBtn = dialog.FindViewById<Button>(Resource.Id.saveRateBtn);

            EditText etRateDescription = dialog.FindViewById<EditText>(Resource.Id.etRateDescription);

            saveRateBtn.Click += (sender, e) =>
            {
                Rating rating;
                try
                {
                    List<Rating> temp = dbCommand.Query<Rating>($"SELECT * FROM Ratings WHERE Email = '{email}' AND RaterEmail = '{sp.GetString("email", "")}'");

                    if(temp.Count != 0) 
                      Toast.MakeText(this, "Cannot Rate A Teacher Twice!", ToastLength.Short).Show();
                    else
                    {
                        rating = new Rating(email, sp.GetString("email", ""), etRateDescription.Text, ratingBar.Rating);
                        dbCommand.Insert(rating);

                        string q = $"SELECT AVG(Rating) FROM Ratings WHERE Email = '{email}'";

                        var result = dbCommand.ExecuteScalar<double?>(q);

                        string updateQ = $"UPDATE TutorsTable SET Rating = '{result}' WHERE Email = '{email}' ";

                        int rows = dbCommand.Execute(updateQ);

                        if (rows == 1)
                            Toast.MakeText(this, "Success", ToastLength.Short).Show();
                        else
                            Toast.MakeText(this, "Failed Updating Rating", ToastLength.Short).Show();
                    }
                    dialog.Cancel();
                }
                catch(Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                }
            };

            dialog.SetTitle("Rate Teacher");
            dialog.Show();
        }

        //handles menu inflation and navigation/action
        #region menus

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (sp.GetInt("IsTeacher", 0) == 0)
                MenuInflater.Inflate(Resource.Menu.MenuForStudent, menu);
            else
                MenuInflater.Inflate(Resource.Menu.profilePageMenu, menu);
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
                    intent = new Intent(this, typeof(LoginActivity));
                    StartActivity(intent);
                    return true;
                case Resource.Id.UpdateWorkHoursMenuBtn:
                    intent = new Intent(this, typeof(AddWorkHoursActivity));
                    StartActivity(intent);
                    return true;
                case Resource.Id.ShowWorkHoursMenuBtn:
                    intent = new Intent(this, typeof(ShowWorkHoursActivity));
                    StartActivity(intent);
                    return true;
                case Resource.Id.MainPageMenuBtn:
                    intent = new Intent(this, typeof(MainPageActivity));
                    StartActivity(intent);
                    return true;
                case Resource.Id.UpdateInfoMenuBtn:
                    intent = new Intent(this, typeof(UpdateUserInfo));
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
    #endregion
}