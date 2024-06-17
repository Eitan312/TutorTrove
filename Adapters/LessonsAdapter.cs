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
using Android.Content.Res;
using Android.Support.V4.Text;

namespace finalproject216303628.Adapters
{
    internal class LessonsAdapter : BaseAdapter<Lessons>
    {

        Activity context;
        List<Lessons> list;

        public LessonsAdapter(Activity context, List<Lessons> list)
        {
            this.context = context;
            this.list = list;
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Lessons this[int position]
        {
            get { return this.list[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            Android.Views.LayoutInflater layoutInflater = ((Activity)context).LayoutInflater;

            Android.Views.View view = layoutInflater.Inflate(Resource.Layout.LessonsLayout, parent, false);

            TextView tvName = view.FindViewById<TextView>(Resource.Id.tvName);
            TextView tvTName = view.FindViewById<TextView>(Resource.Id.tvTName);
            TextView tvLocation = view.FindViewById<TextView>(Resource.Id.tvLocation);
            TextView tvHours = view.FindViewById<TextView>(Resource.Id.tvHours);
            TextView tvDate = view.FindViewById<TextView>(Resource.Id.tvDate);
            ImageView ivTPfp = view.FindViewById<ImageView>(Resource.Id.ivTPfp);
            Button btnDeleteLesson = view.FindViewById<Button>(Resource.Id.btnDeleteLesson);

            Lessons temp = list[position];

            SQLiteConnection dbCommand = Helper.DbCommand();
            ISharedPreferences sp = Helper.Sp();

            try
            {
                Tutors teacher = dbCommand.Get<Tutors>(temp._teacherEmail);
                Tutors student = dbCommand.Get<Tutors>(temp._studentEmail);

                tvDate.Text += temp._date.ToShortDateString();
                tvName.Text += student.Name;
                tvTName.Text += teacher.Name;
                tvLocation.Text += temp._city;

                string[] times = context.Resources.GetStringArray(Resource.Array.times);

                tvHours.Text += times[temp._startHour] + " - " + times[temp._endHour];

                ivTPfp.SetImageBitmap(Helper.DecodeImage(teacher.profilePicture));

                btnDeleteLesson.Click += (sender, e) =>
                {
                    Dialog dialog = new Dialog(context);
                    dialog.SetTitle("Delete Lesson");
                    dialog.SetContentView(Resource.Layout.DeleteLessonDialog);

                    Button sureBtn = dialog.FindViewById<Button>(Resource.Id.sureBtn);
                    Button cancelBtn = dialog.FindViewById<Button>(Resource.Id.cancelDialogBtn);

                    dialog.Show();

                    sureBtn.Click += (sender, e) =>
                    {
                        try
                        {
                            string q = $"DELETE FROM Lessons WHERE Date = '{temp._date.Ticks}'";
                            int rows = dbCommand.Execute(q);
                            if (rows == 1)
                            {
                                tvDate.Text = "Deleted";
                                tvName.Visibility = ViewStates.Gone;
                                tvTName.Visibility = ViewStates.Gone;
                                tvLocation.Visibility = ViewStates.Gone;
                                tvHours.Visibility = ViewStates.Gone;
                                ivTPfp.Visibility = ViewStates.Gone;
                                btnDeleteLesson.Visibility = ViewStates.Gone;
                                dialog.Cancel();
                            }
                            else
                            {
                                Toast.MakeText(context, "Something went wrong", ToastLength.Long).Show();
                            }
                        }
                        catch (Exception ex)
                        {
                            Toast.MakeText(context, ex.Message, ToastLength.Short).Show();
                            dialog.Cancel();
                        }
                    };

                    cancelBtn.Click += (sender, e) =>
                    {
                        dialog.Cancel();
                    };
                };
            }
            catch (Exception ex)
            {
                Toast.MakeText(context, ex.Message, ToastLength.Short).Show();  
            }
            
            return view;
        }

        public override int Count
        {
            get
            {
                return this.list.Count;
            }
        }

    }
}