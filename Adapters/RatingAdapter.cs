using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace finalproject216303628.Adapters
{
    internal class RatingAdapter : BaseAdapter<Rating>
    {

        Activity context;
        List<Rating> list;

        public RatingAdapter(Activity context, List<Rating> list)
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

        public override Rating this[int position]
        {
            get { return this.list[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {

            Android.Views.LayoutInflater layoutInflater = ((Activity)context).LayoutInflater;

            Android.Views.View view = layoutInflater.Inflate(Resource.Layout.RatingLayout, parent, false);

            ImageView ivPfp = view.FindViewById<ImageView>(Resource.Id.raterImg);
            TextView tvName = view.FindViewById<TextView>(Resource.Id.tvName);
            TextView tvRating = view.FindViewById<TextView>(Resource.Id.tvRating);
            TextView tvDesc = view.FindViewById<TextView>(Resource.Id.tvDesc);
            TextView tvReply = view.FindViewById<TextView>(Resource.Id.tvReply);
            Button addReplyBtn = view.FindViewById<Button>(Resource.Id.addReplyBtn);
            Button viewProfileBtn = view.FindViewById<Button>(Resource.Id.viewProfileBtn);

            Rating tempRating = list[position];

            SQLiteConnection dbCommand = Helper.DbCommand();
            ISharedPreferences sp = Helper.Sp();

            Tutors tempRater;

            try
            {
                tempRater = dbCommand.Get<Tutors>(tempRating.remail);

                ivPfp.SetImageBitmap(Helper.DecodeImage(tempRater.profilePicture));
                tvName.Text += tempRater.Name;
                tvRating.Text += tempRating.rating.ToString();
                tvDesc.Text += tempRating.description;

                if (tempRating.reply != "")
                {
                    tvReply.Text += tempRating.reply;
                    addReplyBtn.Visibility = ViewStates.Gone;
                }
                else
                {
                    tvReply.Text += "Teacher Has Yet To Reply";
                }

            }
            catch (System.Exception e)
            {
                Toast.MakeText(context, e.Message, ToastLength.Short).Show();
            }

            viewProfileBtn.Click += (sender, e) =>
            {
                Intent intent = new Intent(context, typeof(ProfileActivity));
                intent.PutExtra("email", tempRating.remail);
                context.StartActivity(intent);
            };

            addReplyBtn.Click += (sender, e) =>
            {
                Dialog dialog = new Dialog(context);
                dialog.SetTitle("Add Reply");
                dialog.SetContentView(Resource.Layout.AddReplyLayout);

                EditText etReply = dialog.FindViewById<EditText>(Resource.Id.etAddReply);
                Button saveReplyBtn = dialog.FindViewById<Button>(Resource.Id.saveReplyBtn);

                saveReplyBtn.Click += (sender, e) =>
                {
                    try
                    {
                        int rows = dbCommand.Execute($"UPDATE Ratings  SET Reply = '{etReply.Text}' WHERE Email = '{tempRating.email}' AND RaterEmail = '{tempRating.remail}'");
                        if(rows == 1)
                        {
                            Toast.MakeText(context, "Success", ToastLength.Short).Show();
                            tvReply.Text = "Teacher Reply: " + etReply.Text;
                        }
                        else
                            Toast.MakeText(context, "Error", ToastLength.Short).Show();


                        dialog.Cancel();
                    }
                    catch(System.Exception ex)
                    {
                        Toast.MakeText(context, ex.Message, ToastLength.Short).Show(); 
                    }
                };

                dialog.Show();

            };

            return view;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return this.list.Count;
            }
        }

    }
}