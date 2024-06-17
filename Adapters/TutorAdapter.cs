using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using Java.Nio.Channels;
using Org.Apache.Commons.Logging;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

namespace finalproject216303628
{
    public class TutorAdapter : BaseAdapter<Tutors>
    {

        Activity context;
        List<Tutors> List;

        public TutorAdapter(Activity context, System.Collections.Generic.List<Tutors> List)
        {
            this.context = context;
            this.List = List;
        }

        public List<Tutors> GetTutorsList()
        {
            return this.List;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }
        public override Tutors this[int position]
        {
            get { return this.List[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            Android.Views.LayoutInflater layoutInflater = ((Activity)context).LayoutInflater;

            Android.Views.View view = layoutInflater.Inflate(Resource.Layout.teacherLvLayout, parent, false);

            ImageView iv = view.FindViewById<ImageView>(Resource.Id.tutorImg);
            TextView nameTv = view.FindViewById<TextView>(Resource.Id.tvName);
            TextView emailTv = view.FindViewById<TextView>(Resource.Id.tvEmail);
            TextView ratingTv = view.FindViewById<TextView>(Resource.Id.tvRating);
            TextView ageTv = view.FindViewById<TextView>(Resource.Id.tvAge);

            Tutors tempTutor = List[position];

            SQLiteConnection dbCommand = Helper.DbCommand();
            double res;

            try
            {
                if (tempTutor != null)
                {
                    iv.SetImageBitmap(Helper.DecodeImage(tempTutor.profilePicture));
                    nameTv.Text += tempTutor.Name;
                    emailTv.Text += tempTutor.Email;
                    ratingTv.Text += dbCommand.ExecuteScalar<double>($"SELECT AVG(Rating) FROM Ratings WHERE Email = '{tempTutor.Email}'");
                    ageTv.Text += Helper.CalculateAge(tempTutor.BirthDate);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(context, ex.Message, ToastLength.Short).Show();
            }

            view.Click += (sender, e) =>
            {
                Dialog dialog = new Dialog(context);
                dialog.SetContentView(Resource.Layout.teacherDialog);

                ImageView tutorIv = dialog.FindViewById<ImageView>(Resource.Id.tutorImg);
                TextView nameTv = dialog.FindViewById<TextView>(Resource.Id.tvName);
                TextView phoneTv = dialog.FindViewById<TextView>(Resource.Id.tvNumber);
                TextView emailTv = dialog.FindViewById<TextView>(Resource.Id.tvEmail);
                TextView ratingTv = dialog.FindViewById<TextView>(Resource.Id.tvRating);
                TextView cityTv = dialog.FindViewById<TextView>(Resource.Id.tvCity);
                TextView subjectTv = dialog.FindViewById<TextView>(Resource.Id.tvSubjects);
                TextView descTv = dialog.FindViewById<TextView>(Resource.Id.tvDescription);
                TextView ageTv = dialog.FindViewById<TextView>(Resource.Id.tvAge);
                Button assignBtn = dialog.FindViewById<Button>(Resource.Id.assignBtn);
                Button ProfileBtn = dialog.FindViewById<Button>(Resource.Id.ProfileBtn);

                tutorIv.SetImageBitmap(Helper.DecodeImage(tempTutor.profilePicture));
                nameTv.Text += tempTutor.Name;
                phoneTv.Text += tempTutor.phone;
                emailTv.Text += tempTutor.Email;
                cityTv.Text += tempTutor.City;
                ageTv.Text += Helper.CalculateAge(tempTutor.BirthDate).ToString();

                try
                {
                    res = dbCommand.ExecuteScalar<double>($"SELECT AVG(Rating) FROM Ratings WHERE Email = '{tempTutor.Email}'");
                    ratingTv.Text += res;

                    List<Subjects> tempSubjects = dbCommand.Query<Subjects>($"SELECT * FROM Subjects WHERE Email = '{tempTutor.Email}'");
                    for(int i = 0; i < tempSubjects.Count - 1; i++)
                    {
                        subjectTv.Text += tempSubjects[i].subject + ", ";
                    }
                    subjectTv.Text += tempSubjects[tempSubjects.Count - 1].subject;
                }
                catch (Exception ex)
                {
                    Toast.MakeText(context, ex.Message, ToastLength.Short).Show();
                }

                descTv.Text += tempTutor.description;

                assignBtn.Click += (sender, e) =>
                {
                    ISharedPreferences sp = Helper.Sp();
                    if(sp.GetString("email", "") == tempTutor.Email)
                    {
                        Toast.MakeText(context, "Cannot assign lessons to yourself!", ToastLength.Long).Show();
                    }
                    else
                    {
                        Intent intent = new Intent(context, typeof(AssignLessonActivity));
                        intent.PutExtra("email", tempTutor.Email);
                        intent.PutExtra("origin", context.LocalClassName.Split('.')[1]);
                        context.StartActivity(intent);
                    }
                };

                ProfileBtn.Click += (sender, e) =>
                {
                    Intent intent = new Intent(context, typeof(ProfileActivity));
                    intent.PutExtra("email", tempTutor.Email);
                    context.StartActivity(intent); 
                };

                dialog.Show();
            };

            return view;
        }

        public override int Count
        {
            get
            {
                return this.List.Count;
            }
        }

    }
}