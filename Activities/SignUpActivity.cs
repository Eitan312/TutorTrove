using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using AndroidX.AppCompat.App;
using SQLite;
using Android.Graphics;
using Plugin.Media;
using Android;
using System.IO;
using Android.Support.V4.App;
using Android.Views.Animations;
using static Android.Webkit.WebStorage;

namespace finalproject216303628
{
    [Activity(Label = "SignUpActivity")]
    public class SignUpActivity : AppCompatActivity, Android.Views.View.IOnClickListener
    {
        #region views
        LinearLayout lChoice;
        EditText fnameEt, emailEt, phoneEt, passEt,cPassEt, etDescription;
        TextView fnameTv, emailTv, phoneTv, passTv, cPassTv, ageTv;
        Spinner citySpinner;
        RadioButton maleRb, femaleRb;
        ImageButton calendarIb;
        Dialog datePickDialog;
        Button btnSave, signUpBtn, btnUpload, btnCapture;
        DatePicker datePicker;
        TextView birthdayTv;
        DateTime pickedDt;
        ImageButton ibPfp, studentBtn, teacherBtn;
        int isTeacher; // since sqlite does not support booleans: 0 = student, 1 = teacher
        Bitmap pfp;
        ScrollView svSignUp;

        Animation fadeInAnim;
        Animation fadeOutAnim;
        #endregion

        SQLiteConnection dbCommand;
        ISharedPreferences sp;

        //OnCreate Function
        #region OnCreate
        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.Title = "TutorTrove";

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SignUpLayout);

            dbCommand = Helper.DbCommand();
            sp = Helper.Sp();

            lChoice = FindViewById<LinearLayout>(Resource.Id.lChoice);
            calendarIb = FindViewById<ImageButton>(Resource.Id.calendarIb);
            fnameEt = FindViewById<EditText>(Resource.Id.fNameEt);
            emailEt = FindViewById<EditText>(Resource.Id.emailEt);
            phoneEt = FindViewById<EditText>(Resource.Id.phoneEt);
            passEt = FindViewById<EditText>(Resource.Id.passEt);
            cPassEt = FindViewById<EditText>(Resource.Id.cPassEt);
            maleRb = FindViewById<RadioButton>(Resource.Id.maleRb);
            femaleRb = FindViewById<RadioButton>(Resource.Id.femaleRb);
            signUpBtn = FindViewById<Button>(Resource.Id.signUpBtn);
            fnameTv = FindViewById<TextView>(Resource.Id.fnameTv);
            emailTv = FindViewById<TextView>(Resource.Id.emailTv);
            phoneTv = FindViewById<TextView>(Resource.Id.phoneTv);
            passTv = FindViewById<TextView>(Resource.Id.passTv);
            cPassTv = FindViewById<TextView>(Resource.Id.cPassTv);
            ageTv = FindViewById<TextView>(Resource.Id.ageTv);
            citySpinner = FindViewById<Spinner>(Resource.Id.citySpinner);
            ibPfp = FindViewById<ImageButton>(Resource.Id.ibPfp);
            studentBtn = FindViewById<ImageButton>(Resource.Id.studentBtn);
            teacherBtn = FindViewById<ImageButton>(Resource.Id.teacherBtn);
            svSignUp = FindViewById<ScrollView>(Resource.Id.svSignUp);
            btnUpload = FindViewById<Button>(Resource.Id.btnUpload);
            btnCapture = FindViewById<Button>(Resource.Id.btnCapture);
            etDescription = FindViewById<EditText>(Resource.Id.etDescription);

            fadeInAnim = AnimationUtils.LoadAnimation(this, Resource.Animation.FadeInAnim);
            fadeOutAnim = AnimationUtils.LoadAnimation(this, Resource.Animation.FadeOutAnim);

            string[] items = Resources.GetStringArray(Resource.Array.cityNames);

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, items);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            pfp = BitmapFactory.DecodeResource(Resources, Resource.Drawable.pfp);

            citySpinner.Adapter = adapter;

            datePickDialog = new Dialog(this);
            datePickDialog.SetContentView(Resource.Layout.pickDateLayout);

            signUpBtn.Click += SignUpBtn_Click;
            btnSave = datePickDialog.FindViewById<Button>(Resource.Id.btnSave);
            datePicker = datePickDialog.FindViewById<DatePicker>(Resource.Id.datePicker);
            birthdayTv = FindViewById<TextView>(Resource.Id.birthdayTv);

            btnUpload.Click += BtnUpload_Click;
            btnCapture.Click += BtnCapture_Click;
            calendarIb.Click += CalendarIb_Click;
            btnSave.Click += BtnSave_Click;
            maleRb.SetOnClickListener(this);
            femaleRb.SetOnClickListener(this);
            ibPfp.Click += IbPfp_Click;
            teacherBtn.Click += TeacherBtn_Click;
            studentBtn.Click += StudentBtn_Click;
        }

        private void StudentBtn_Click(object sender, EventArgs e)
        {
            isTeacher = 0;

            lChoice.StartAnimation(fadeOutAnim);
            lChoice.Visibility = ViewStates.Gone;

            svSignUp.Visibility = ViewStates.Visible;
            svSignUp.StartAnimation(fadeInAnim);
        }

        private void TeacherBtn_Click(object sender, EventArgs e)
        {
            isTeacher = 1;

            lChoice.StartAnimation(fadeOutAnim);
            lChoice.Visibility = ViewStates.Gone;

            svSignUp.StartAnimation(fadeInAnim);
            svSignUp.Visibility = ViewStates.Visible;
        }
        #endregion


        //listener to make sure only one of the radio buttons can be checked at one time
        public void OnClick(View v)
        {
            if (v == maleRb)
            {
                femaleRb.Checked = false;
            }
            if (v == femaleRb)
            {
                maleRb.Checked = false;
            }
        }

        //handles menu inflation and action
        #region menu
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SignUpMenu, menu);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.loginMenuBtn:
                    Intent intent = new Intent(this, typeof(LoginActivity));
                    StartActivity(intent);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        #endregion

        //handels sign up
        #region signup
        private void SignUpBtn_Click(object sender, EventArgs e)
        {
            bool valid;
            Tutors tutor1;
            string res = citySpinner.SelectedItem.ToString();
            // if any of the required field are empty
            if (fnameEt.Text == "" || citySpinner.SelectedItem.ToString() == "Pick A City" || pickedDt == null || emailEt.Text == "" || phoneEt.Text == "" || passEt.Text == "" || cPassEt.Text == "" || (!maleRb.Checked && !femaleRb.Checked))
            {
                Toast.MakeText(this, "Please Fill All Fields", ToastLength.Short).Show();
            }
            else
            {
                //checks if the inputs are valid
                valid = Validate();

                if(valid)
                {
                    try
                    {
                        //check if email exists in the database
                        var tutor = dbCommand.Get<Tutors>(emailEt.Text);
                        Toast.MakeText(this, "User Exists!", ToastLength.Long).Show();
                    }
                    catch
                    {
                        // create user object depending on user choice
                        if (maleRb.Checked)
                            tutor1 = new Tutors(emailEt.Text, ToUpFirstName(fnameEt.Text), citySpinner.SelectedItem.ToString(), phoneEt.Text,"Male", pickedDt, passEt.Text, Helper.EncodeImage(pfp), etDescription.Text);
                        else
                            tutor1 = new Tutors(emailEt.Text, ToUpFirstName(fnameEt.Text), citySpinner.SelectedItem.ToString(), phoneEt.Text,"Female", pickedDt, passEt.Text, Helper.EncodeImage(pfp), etDescription.Text);

                        try
                        {
                            // insert user into appropriate table
                            int rows = dbCommand.Insert(tutor1);
                            if (rows == 1)
                            {
                                // save every detail about the costumer in the shared preference
                                var editor = sp.Edit();
                                Intent intent;
                                editor.PutString("email", tutor1.Email);
                                editor.PutBoolean("keep", false);
                                Toast.MakeText(this, "Success!", ToastLength.Long).Show();
                                // save whether teacher picked hours if user is a teacher
                                if (isTeacher == 1)
                                {
                                    editor.PutBoolean("enteredHours", false);
                                    editor.Commit();
                                    intent = new Intent(this, typeof(AddWorkHoursActivity));
                                }
                                else
                                {
                                    editor.PutInt("IsTeacher", 0);
                                    editor.Commit();
                                    intent = new Intent(this, typeof(MainActivity));
                                    intent.PutExtra("origin", this.LocalClassName.Split('.')[1]);
                                }
                                // move to appropriate page according to user choice
                                StartActivity(intent);
                            }
                            else
                                Toast.MakeText(this, "Error In Inserting User", ToastLength.Long).Show();
                        }
                        catch (Exception ex)
                        {
                            Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                        }
                    }
                }
                else
                {
                    Toast.MakeText(this, "Some Inputs Were Invalid!", ToastLength.Long).Show();
                }
            }
        }
        #endregion

        // gets a full name string in the format 'fname lname' and returns the full name in the format 'Fname Lname' (uppercases first letters)
        public string ToUpFirstName(string name)
        {
            string[] words = name.Split(" ");

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (string word in words)
            {
                sb.Append(char.ToUpper(word[0]) + word.Substring(1).ToLower());
                sb.Append(" ");
            }

            return sb.ToString().Trim();
        }

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
                ibPfp.SetImageBitmap(bitmap);
                pfp = bitmap;
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        async void UploadPhoto()
        {
            try
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
                ibPfp.SetImageBitmap(bitmap);
                pfp = bitmap;
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
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

        //handles datePickerDialog
        #region datepickerdialog
        private void CalendarIb_Click(object sender, EventArgs e)
        {
            datePickDialog.SetCancelable(true);
            datePickDialog.SetTitle("Pick Birth Date");
            datePickDialog.Show();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            pickedDt = new DateTime(datePicker.Year, datePicker.Month + 1, datePicker.DayOfMonth);
            birthdayTv.Text = "Birthday:\n" + pickedDt.ToShortDateString();
            datePickDialog.Cancel();
        }
        #endregion

        //handles validation
        #region validation
        public bool Validate()
        {
            bool valid = true;

            //full name validation
            if (!Validation.IsFullNameValid(fnameEt.Text))
            {
                fnameTv.Visibility = ViewStates.Visible;
                valid = false;
            }
            else
                fnameTv.Visibility = ViewStates.Invisible;


            //email validation
            if (!Validation.IsEmailValid(emailEt.Text))
            {
                emailTv.Visibility = ViewStates.Visible;
                valid = false;
            }
            else
                emailTv.Visibility = ViewStates.Invisible;

            //phone validation
            if (!Validation.IsPhoneValid(phoneEt.Text))
            {
                phoneTv.Visibility = ViewStates.Visible;
                valid = false;
            }
            else
                phoneTv.Visibility = ViewStates.Invisible;

            //age validation
            if (!Validation.IsAgeValid(pickedDt))
            {
                ageTv.Visibility = ViewStates.Visible;
                valid = false;
            }
            else
                ageTv.Visibility = ViewStates.Invisible;


            //password validation
            if (!Validation.IsPasswordValid(passEt.Text))
            {
                passTv.Visibility = ViewStates.Visible;
                valid = false;
            }
            else
                passTv.Visibility = ViewStates.Invisible;

            if (!Validation.ConfirmPassword(passEt.Text, cPassEt.Text))
            {
                cPassTv.Visibility = ViewStates.Visible;
                valid = false;
            }
            else
                cPassTv.Visibility = ViewStates.Invisible;

            if (Helper.CalculateAge(pickedDt) < 16 && isTeacher == 1)
            {
                valid = false;
                Toast.MakeText(this, "Too Young To Be A Teacher!", ToastLength.Long).Show();
            }

            return valid;
        }
        #endregion
    }
}