using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using Plugin.Media;
using AndroidX.AppCompat.App;

namespace finalproject216303628
{
    [Activity(Label = "UpdateUserInfo")]
    public class UpdateUserInfo : AppCompatActivity
    {
        ImageButton ibUpdatePfp;
        Button btnSelectImg, btnCaptureImg, btnUpdateFname, btnUpdateCity, btnUpdateEmail, btnSelectAge, btnUpdateAge, btnSave, btnUpdatePfp, btnUpdatePassword;
        EditText etUpdateFname, etUpdateEmail;
        TextView tvUpdateAge;
        Spinner spUpdateCity;
        Dialog updateAgeDialog;
        DateTime pickedBirthdate;
        DatePicker datePicker;
        Bitmap pfp;

        Tutors user;

        SQLiteConnection dbCommand;
        ISharedPreferences sp;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.Title = "TutorTrove";

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.UpdateUserInfoLayout);

            dbCommand = Helper.DbCommand();
            sp = Helper.Sp();

            ibUpdatePfp = FindViewById<ImageButton>(Resource.Id.ibUpdatePfp);
            btnSelectImg = FindViewById<Button>(Resource.Id.btnSelectImg);
            btnCaptureImg = FindViewById<Button>(Resource.Id.btnCaptureImg);
            btnUpdateFname = FindViewById<Button>(Resource.Id.btnUpdateFname);
            btnUpdateCity = FindViewById<Button>(Resource.Id.btnUpdateCity);
            btnUpdateEmail = FindViewById<Button>(Resource.Id.btnUpdateEmail);
            btnSelectAge = FindViewById<Button>(Resource.Id.btnSelectAge);
            btnUpdateAge = FindViewById<Button>(Resource.Id.btnUpdateAge);
            btnUpdatePfp = FindViewById<Button>(Resource.Id.btnUpdatePfp);
            btnUpdatePassword = FindViewById<Button>(Resource.Id.btnUpdatePassword);
            etUpdateFname = FindViewById<EditText>(Resource.Id.etUpdateFname);
            etUpdateEmail = FindViewById<EditText>(Resource.Id.etUpdateEmail);
            tvUpdateAge = FindViewById<TextView>(Resource.Id.tvUpdateAge);
            spUpdateCity = FindViewById<Spinner>(Resource.Id.spUpdateCity);

            updateAgeDialog = new Dialog(this);
            updateAgeDialog.SetContentView(Resource.Layout.pickDateLayout);

            btnSave = updateAgeDialog.FindViewById<Button>(Resource.Id.btnSave);
            datePicker = updateAgeDialog.FindViewById<DatePicker>(Resource.Id.datePicker);

            btnSave.Click += BtnSave_Click;
            btnSelectAge.Click += BtnSelectAge_Click;

            string[] items = Resources.GetStringArray(Resource.Array.cityNames);

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, items);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            spUpdateCity.Adapter = adapter;

            FillUserInfo();

            btnSelectImg.Click += BtnSelectImg_Click;
            btnCaptureImg.Click += BtnCaptureImg_Click;
            btnUpdatePfp.Click += BtnUpdatePfp_Click;
            btnUpdateCity.Click += BtnUpdateCity_Click;
            btnUpdateFname.Click += BtnUpdateFname_Click;
            btnUpdateEmail.Click += BtnUpdateEmail_Click;
            btnUpdateAge.Click += BtnUpdateAge_Click;
            btnUpdatePassword.Click += BtnUpdatePassword_Click;
        }

        private void BtnUpdatePassword_Click(object sender, EventArgs e)
        {
            Dialog dialog = new Dialog(this);
            dialog.SetTitle("UpdatePassword");
            dialog.SetContentView(Resource.Layout.updatePassDialog);

            EditText etUpdatePass = dialog.FindViewById<EditText>(Resource.Id.etUpdatePass);
            Button submitPassBtn = dialog.FindViewById<Button>(Resource.Id.submitPassBtn);

            dialog.Show();
            submitPassBtn.Click += (sender, e) =>
            {
                if (Validation.IsPasswordValid(etUpdateEmail.Text))
                {
                    try
                    {
                        string q = $"UPDATE TutorsTable SET Password = '{etUpdateEmail.Text}' WHERE Email = '{sp.GetString("email", "")}'";
                        int rows = dbCommand.Update(q);

                        if(rows == 1)
                        {
                            Toast.MakeText(this, "Success" , ToastLength.Long).Show();
                            dialog.Cancel();
                        }
                        else
                        {
                            Toast.MakeText(this, "Error Updating User", ToastLength.Long).Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                    }
                }
                else
                    Toast.MakeText(this, "Password must be at least 8 characters and contain no special characters", ToastLength.Long).Show();
            };
        }

        private void BtnUpdateAge_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Validation.IsAgeValid(pickedBirthdate))
                {
                    throw new Exception("Too Young! Must Be 16 Or Older");
                }
                string q = $"UPDATE TutorsTable SET BirthDate = '{pickedBirthdate}' WHERE Email = '{sp.GetString("Email", "")}'";
                int rows = dbCommand.Execute(q);
                if (rows == 1)
                {
                    Toast.MakeText(this, "Success", ToastLength.Long).Show();
                }
                else
                    throw new Exception("Something Went Wrong Updating Age");
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        private void BtnUpdateEmail_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Validation.IsEmailValid(etUpdateEmail.Text))
                {
                    throw new Exception("Invalid Input!");
                }
                string q = $"UPDATE TutorsTable SET Email = '{etUpdateEmail}' WHERE Email = '{sp.GetString("Email", "")}'";
                int rows = dbCommand.Execute(q);
                if (rows == 1)
                {
                    var editor = sp.Edit();
                    editor.PutString("Email", etUpdateEmail.Text);
                    editor.Commit();
                    Toast.MakeText(this, "Success", ToastLength.Long).Show();
                }
                else
                    throw new Exception("Something Went Wrong Updating Email");
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        private void BtnUpdateFname_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Validation.IsFullNameValid(etUpdateFname.Text))
                {
                    throw new Exception("Invalid Input!");
                }
                string q = $"UPDATE TutorsTable SET FullName = '{etUpdateFname}' WHERE Email = '{sp.GetString("Email", "")}'";
                int rows = dbCommand.Execute(q);
                if (rows == 1)
                    Toast.MakeText(this, "Success", ToastLength.Long).Show();
                else
                    throw new Exception("Something Went Wrong Updating Full Name");
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        private void BtnUpdateCity_Click(object sender, EventArgs e)
        {
            try
            {
                if(spUpdateCity.SelectedItem.ToString() == "Pick A City")
                {
                    throw new Exception("You Have To Select A City To Update!");
                }
                string q = $"UPDATE TutorsTable SET City = '{spUpdateCity.SelectedItem.ToString()}' WHERE Email = '{sp.GetString("Email", "")}'";
                int rows = dbCommand.Execute(q);
                if (rows == 1)
                    Toast.MakeText(this, "Success", ToastLength.Long).Show();
                else
                    throw new Exception("Something Went Wrong Updating City");
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        private void BtnUpdatePfp_Click(object sender, EventArgs e)
        {
            try
            {
                string q = $"UPDATE TutorsTable SET ProfilePicture = '{Helper.EncodeImage(pfp)}' WHERE Email = '{sp.GetString("Email", "")}'";
                int rows = dbCommand.Execute(q);
                if (rows == 1)
                    Toast.MakeText(this, "Success", ToastLength.Long).Show();
                else
                    throw new Exception("Something Went Wrong Updating Profile Picture");
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        private void BtnCaptureImg_Click(object sender, EventArgs e)
        {
            TakePhoto();
        }

        private void BtnSelectImg_Click(object sender, EventArgs e)
        {
            UploadPhoto();
        }

        public void FillUserInfo()
        {
            try
            {
                user = dbCommand.Get<Tutors>(sp.GetString("email", ""));
                ibUpdatePfp.SetImageBitmap(Helper.DecodeImage(user.profilePicture));
                etUpdateFname.Text = user.Name;

                int pos = GetSpinnerIdxFromString(spUpdateCity, user.City);
                if (pos != -1)
                {
                    spUpdateCity.SetSelection(pos);
                }

                etUpdateEmail.Text = user.Email;
                tvUpdateAge.Text = Helper.CalculateAge(user.BirthDate).ToString();
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        // Retrieves -1 on failure.
        public int GetSpinnerIdxFromString(Spinner spinner, string str)
        {
            for (int i = 0; i < spinner.Count; i++)
            {
                string item_str = spinner.GetItemAtPosition(i).ToString();

                if (str.Equals(item_str))
                {
                    return i;
                }
            }

            return -1;

        }

        //handles datePickerDialog
        #region datepickerdialog
        private void BtnSelectAge_Click(object sender, EventArgs e)
        {
            updateAgeDialog.SetCancelable(true);
            updateAgeDialog.SetTitle("Pick Birth Date");
            updateAgeDialog.Show();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            pickedBirthdate = new DateTime(datePicker.Year, datePicker.Month + 1, datePicker.DayOfMonth);
            tvUpdateAge.Text = Helper.CalculateAge(pickedBirthdate).ToString();
            updateAgeDialog.Cancel();
        }
        #endregion

        //Handles Importing Photos from camera roll
        #region CameraHandling
        async void TakePhoto()
        {
            try
            {
                await CrossMedia.Current.Initialize();

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                    CompressionQuality = 40,
                    Name = "myimage.jpg",
                    Directory = "sample"
                });

                if (file == null)
                {
                    return;
                }

                // Convert file to byte array and set the resulting bitmap to imageview
                byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
                Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
                ibUpdatePfp.SetImageBitmap(bitmap);
                pfp = bitmap;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        async void UploadPhoto()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                Toast.MakeText(this, "Upload not supported on this device", ToastLength.Short).Show();
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Full,
                CompressionQuality = 40
            });

            if (file == null) { return; }

            // Convert file to byte array, to bitmap and set it to our ImageView

            byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            ibUpdatePfp.SetImageBitmap(bitmap);
            pfp = bitmap;
        }

        private void IbPfp_Click(object sender, EventArgs e)
        {
            UploadPhoto();
        }

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            UploadPhoto();
        }
        private void BtnCapture_Click(object sender, EventArgs e)
        {
            TakePhoto();
        }
        #endregion

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (sp.GetInt("IsTeacher", 0) == 0)
                MenuInflater.Inflate(Resource.Menu.MenuForStudent, menu);
            else
                MenuInflater.Inflate(Resource.Menu.UpdateUserInfoMenu, menu);
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