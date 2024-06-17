using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace finalproject216303628
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : AppCompatActivity
    {
        SQLiteConnection dbCommand;
        ISharedPreferences sp;

        SmsReceiver smsReceiver;

        CodeTimer timer;

        int code;

        #region views
        CheckBox rememberCb;

        Dialog dialogForgotPass;//dialog for interaction in case of forgetting password
        Button loginBtn, SignUpBtn, CloseDialogBtn, ShowPassBtn; // Buttons for logging in/ signing up

        ImageView ivLogo;//ImageView for displaying the logo

        TextView tvHeader1, tvForgotPass, tvHeader2, dialogHeader, showPassTv, tvMin, tvSec; // TextViews for headers, tvForgotPass is clickable and will open a dialog
                                                                               // for password recovery abd showPassTv is where i will display the lost password
                                                                               // I made sure to declare the views in order of appearance in the layout

        TextView emailTv, passTv;// TextViews for displaying invalid input messages

        EditText etEmail, etPass, dialogEtEmail; //EditTexts for user input for login credentials

        LinearLayout MainLayout, SignUpLayout, LoginViewsLayout, dialogLayout, dialogButtonsLayout;// MainLayout is the large layout that i dynamically input into
                                                                                                   // SignUpLayout is the small layout for organizing the sign up button and header
                                                                                                   //dialogLayout is the layout for the dialog
                                                                                                   //dialogButtonsLayout is the layout for the buttons in the dialog
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.Title = "TutorTrove";

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.LoginLayout);

            dbCommand = Helper.DbCommand();
            sp = Helper.Sp();

            MainLayout = FindViewById<LinearLayout>(Resource.Id.L1);

            Setup();//Sets up the layout dynamically

            loginBtn.Click += LoginBtn_Click;
            tvForgotPass.Click += TvForgotPass_Click;
            CloseDialogBtn.Click += CloseDialogBtn_Click;
            SignUpBtn.Click += SignUpBtn_Click;
            ShowPassBtn.Click += ShowPassBtn_Click;

            KeepPrefrences();

        }


        //transfers to sign up page
        private void SignUpBtn_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SignUpActivity));
            StartActivity(intent);
        }

        //handles executing prefrences
        #region keepPrefrences
        public void KeepPrefrences()
        {
            bool keep = sp.GetBoolean("keep", false);

            if (keep)
            {
                etEmail.Text = sp.GetString("email", "");
                etPass.Text = sp.GetString("pass", "");
                rememberCb.Checked = true;
            }
        }
        #endregion

        //handles menu inflation and action
        #region menu
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.LoginMenu, menu);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Intent intent;
            switch (item.ItemId)
            {
                case Resource.Id.SignUpMenuBtn:
                    intent = new Intent(this, typeof(SignUpActivity));
                    StartActivity(intent);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        #endregion

        //handles dialog
        #region dialog
        private void CloseDialogBtn_Click(object sender, System.EventArgs e)
        {
            dialogForgotPass.Cancel();
        }

        private void TvForgotPass_Click(object sender, System.EventArgs e)
        {
            dialogForgotPass.SetTitle("Forgot Password");
            dialogForgotPass.Show();
        }


        private void ShowPassBtn_Click(object sender, EventArgs e)
        {
            if (dialogEtEmail.Text == "")
                Toast.MakeText(this, "Please Fill The Field", ToastLength.Long).Show();
            else
            {
                bool valid = Validation.IsEmailValid(dialogEtEmail.Text);
                if (valid)
                {
                    try
                    {
                        Tutors user = dbCommand.Get<Tutors>(dialogEtEmail.Text);

                        Helper.SendEmail(this, dialogEtEmail.Text, user.Password);
                    }
                    catch
                    {
                        Toast.MakeText(this, "User Not Found", ToastLength.Long).Show();
                    }
                }
                else
                    Toast.MakeText(this, "Invalid Email", ToastLength.Long).Show();
            }
        }
        #endregion

        //handles login and saving
        #region login
        private void LoginBtn_Click(object sender, System.EventArgs e)
        {
            bool valid;
            if (etEmail.Text == "" || etPass.Text == "")
            {
                Toast.MakeText(this, "Please Fill All Inputs!", ToastLength.Long).Show();
            }
            else
            {

                valid = Validate();

                if (valid)
                {
                    try
                    {
                        string query = $"SELECT * FROM TutorsTable WHERE Email = '{etEmail.Text}' AND Password = '{etPass.Text}'";
                        var users = dbCommand.Query<Tutors>(query);
                        if (users.Count == 1)
                        {
                            Dialog dialog = new Dialog(this);
                            dialog.SetTitle("Code Sent");
                            dialog.SetContentView(Resource.Layout.SendMessageDialog);

                            EditText etCode = dialog.FindViewById<EditText>(Resource.Id.etCode);
                            Button submitBtn = dialog.FindViewById<Button>(Resource.Id.submitBtn);
                            Button resendBtn = dialog.FindViewById<Button>(Resource.Id.resendBtn);

                            tvMin = dialog.FindViewById<TextView>(Resource.Id.tvMin);
                            tvSec = dialog.FindViewById<TextView>(Resource.Id.tvSec);

                            dialog.Show();

                            if (timer == null || timer.num == 0)
                            {
                                timer = new CodeTimer(tvSec, tvMin, users[0].phone, this);
                                timer.Execute(180);
                            }

                            timer.isRun = true;

                            submitBtn.Click += (sender, e) =>
                            {
                                if(etCode.Text == timer.code)
                                {
                                     
                                    dialog.Cancel();
                                    var editor = sp.Edit();
                                    editor.PutBoolean("keep", rememberCb.Checked);
                                    editor.PutString("email", etEmail.Text);
                                    editor.PutString("pass", etPass.Text);
                                    editor.PutString("name", users[0].Name);
                                    editor.PutString("gender", users[0].Gender);
                                    editor.PutString("city", users[0].City);
                                    editor.PutBoolean("loggedIn", true);
                                    editor.PutInt("IsTeacher", users[0].isTeacher);
                                    editor.Commit();

                                    Intent intent = new Intent(this, typeof(MainPageActivity));
                                    StartActivity(intent);
                                }
                                else
                                {
                                    Toast.MakeText(this, "Incorrect Code", ToastLength.Long).Show();
                                }
                            };

                            resendBtn.Click += (sender, e) =>
                            {
                                Random rnd = new Random();
                                code = rnd.Next(100000, 1000000);
                                timer.code = code.ToString();
                                timer.num = 180;

                                Helper.SendSms(this, users[0].phone, code.ToString());
                            };
                        }
                        else
                        {
                            Toast.MakeText(this, "Error In Finding User", ToastLength.Long).Show();
                        }
                    }
                    catch(Exception ex)
                    {
                        Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this, "Some Inputs Were Invalid", ToastLength.Long).Show();
                }
            }
        }
        #endregion

        //handles validation
        #region validation
        public bool Validate()
        {
            bool valid = true;

            if (!Validation.IsEmailValid(etEmail.Text))
            {
                emailTv.Visibility = ViewStates.Visible;
                valid = false;
            }
            else
                emailTv.Visibility = ViewStates.Invisible;

            if (!Validation.IsPasswordValid(etPass.Text))
            {
                passTv.Visibility = ViewStates.Visible;
                valid = false;
            }
            else
                passTv.Visibility = ViewStates.Invisible;

            return valid;
        }
        #endregion

        //handles dynamic setup
        #region Setup
        private void Setup()
        {
            loginBtn = new Button(this);
            SignUpBtn = new Button(this);
            ivLogo = new ImageView(this);
            tvHeader1 = new TextView(this);
            tvForgotPass = new TextView(this);
            tvHeader2 = new TextView(this);
            etEmail = new EditText(this);
            etPass = new EditText(this);
            SignUpLayout = new LinearLayout(this);
            dialogForgotPass = new Dialog(this);
            dialogLayout = new LinearLayout(this);
            CloseDialogBtn = new Button(this);
            ShowPassBtn = new Button(this);
            dialogEtEmail = new EditText(this);
            dialogHeader = new TextView(this);
            dialogButtonsLayout = new LinearLayout(this);
            rememberCb = new CheckBox(this);
            LoginViewsLayout = new LinearLayout(this);
            emailTv = new TextView(this);
            passTv = new TextView(this);
            showPassTv = new TextView(this);

            LinearLayout.LayoutParams dialogEtParams = new LinearLayout.LayoutParams(700, 120);
            LinearLayout.LayoutParams dialogBtnSendParams = new LinearLayout.LayoutParams(300, 125);
            LinearLayout.LayoutParams dialogBtnCloseParams = new LinearLayout.LayoutParams(290, 125);
            LinearLayout.LayoutParams IvLogoParams = new LinearLayout.LayoutParams(500, 400);
            LinearLayout.LayoutParams loginBtnParams = new LinearLayout.LayoutParams(280, 130);
            LinearLayout.LayoutParams etParams = new LinearLayout.LayoutParams(800, 120);
            LinearLayout.LayoutParams tvHeader1Params = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 140);
            LinearLayout.LayoutParams tvInvalidEmailParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 100);
            LinearLayout.LayoutParams tvInvalidPassParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 150);
            LinearLayout.LayoutParams tvForgotPassParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 100);
            LinearLayout.LayoutParams tvHeader2Params = new LinearLayout.LayoutParams(600, 100);
            LinearLayout.LayoutParams SignUpBtnParams = new LinearLayout.LayoutParams(270, 110);
            LinearLayout.LayoutParams SignUpLayoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 150);
            LinearLayout.LayoutParams loginViewsLayoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 220);
            LinearLayout.LayoutParams dialogButtonsLayoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 200);
            LinearLayout.LayoutParams rememberCbParams = new LinearLayout.LayoutParams(430, 100);

            tvHeader1Params.SetMargins(0, 30, 0, 0);
            etParams.SetMargins(0, 90, 0, 0);
            loginViewsLayoutParams.SetMargins(0, 50, 0, 0);
            tvForgotPassParams.SetMargins(0, 20, 0, 0);
            SignUpLayoutParams.SetMargins(0, 40, 0, 0);

            dialogEtParams.SetMargins(100, 50, 0, 0);
            dialogBtnSendParams.SetMargins(0, 0, 40, 0);
            dialogButtonsLayoutParams.SetMargins(0, 80, 0, 0);

            dialogButtonsLayout.LayoutParameters = dialogButtonsLayoutParams;
            dialogHeader.LayoutParameters = tvHeader1Params;
            dialogEtEmail.LayoutParameters = dialogEtParams;
            ShowPassBtn.LayoutParameters = dialogBtnSendParams;
            CloseDialogBtn.LayoutParameters = dialogBtnCloseParams;
            ivLogo.LayoutParameters = IvLogoParams;
            tvHeader1.LayoutParameters = tvHeader1Params;
            etEmail.LayoutParameters = etParams;
            etPass.LayoutParameters = etParams;
            loginBtn.LayoutParameters = loginBtnParams;
            tvForgotPass.LayoutParameters = tvForgotPassParams;
            SignUpLayout.LayoutParameters = SignUpLayoutParams;
            tvHeader2.LayoutParameters = tvHeader2Params;
            SignUpBtn.LayoutParameters = SignUpBtnParams;
            rememberCb.LayoutParameters = rememberCbParams;
            LoginViewsLayout.LayoutParameters = loginViewsLayoutParams;
            emailTv.LayoutParameters = tvInvalidEmailParams;
            passTv.LayoutParameters = tvInvalidPassParams;
            showPassTv.LayoutParameters = tvHeader1Params;

            dialogHeader.Text = "No Worries, Enter Your Email:";
            dialogHeader.TextSize = 20;
            dialogHeader.Typeface = Typeface.Serif;
            dialogHeader.Gravity = GravityFlags.Center;

            dialogEtEmail.Hint = "Email";
            dialogEtEmail.TextSize = 18;
            dialogEtEmail.Typeface = Typeface.Serif;
            dialogEtEmail.Gravity = GravityFlags.Center;
            dialogEtEmail.SetBackgroundResource(Resource.Drawable.etStyle);

            CloseDialogBtn.Text = "Cancel";
            CloseDialogBtn.Typeface = Typeface.Serif;
            CloseDialogBtn.SetAllCaps(false);
            CloseDialogBtn.Gravity = GravityFlags.Center;
            CloseDialogBtn.SetBackgroundResource(Resource.Drawable.buttonStyle);

            ShowPassBtn.Text = "Get Password";
            ShowPassBtn.Typeface = Typeface.Serif;
            ShowPassBtn.SetAllCaps(false);
            ShowPassBtn.Gravity = GravityFlags.Center;
            ShowPassBtn.SetBackgroundResource(Resource.Drawable.buttonStyle);

            dialogButtonsLayout.Orientation = Android.Widget.Orientation.Horizontal;
            dialogButtonsLayout.SetGravity(GravityFlags.Center);
            dialogButtonsLayout.AddView(ShowPassBtn);
            dialogButtonsLayout.AddView(CloseDialogBtn);

            showPassTv.Visibility = ViewStates.Invisible;
            showPassTv.TextSize = 20;
            showPassTv.Typeface = Typeface.Serif;
            showPassTv.Gravity = GravityFlags.Center;


            dialogLayout.Orientation = Android.Widget.Orientation.Vertical;
            dialogLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#36594a"));

            dialogLayout.AddView(dialogHeader);
            dialogLayout.AddView(dialogEtEmail);
            dialogLayout.AddView(dialogButtonsLayout);

            dialogForgotPass.SetContentView(dialogLayout);
            dialogForgotPass.Window.SetLayout(1000, 800);

            ivLogo.SetBackgroundResource(Resource.Drawable.logo);

            tvHeader1.Gravity = GravityFlags.Center;
            tvHeader1.TextSize = 20;
            tvHeader1.Typeface = Typeface.Serif;
            tvHeader1.Text = "Welcome To TutorTrove! \n Your Learning Journey Begins Now";

            etEmail.Hint = "Email";
            etEmail.Gravity = GravityFlags.Center;
            etEmail.TextSize = 18;
            etEmail.Typeface = Typeface.Serif;
            etEmail.SetBackgroundResource(Resource.Drawable.etStyle);

            emailTv.Visibility = ViewStates.Invisible;
            emailTv.Text = "Email Is Invalid!";
            emailTv.TextSize = 20;
            emailTv.Typeface = Typeface.Serif;
            emailTv.Gravity = GravityFlags.Center;

            etPass.Hint = "Password";
            etPass.Gravity = GravityFlags.Center;
            etPass.TextSize = 18;
            etPass.InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;
            etPass.SetBackgroundResource(Resource.Drawable.etStyle);


            passTv.Visibility = ViewStates.Invisible;
            passTv.Text = "Password Should be At Least 8 characters and only contain legal characters and numbers";
            passTv.TextSize = 17;
            passTv.Typeface = Typeface.Serif;
            passTv.Gravity = GravityFlags.Center;

            loginBtn.Gravity = GravityFlags.Center;
            loginBtn.Typeface = Typeface.Serif;
            loginBtn.Text = "Login";
            loginBtn.TextSize = 17;
            loginBtn.SetAllCaps(false);
            loginBtn.SetBackgroundResource(Resource.Drawable.buttonStyle);

            rememberCb.Checked = false;
            rememberCb.Text = "Remember Me";
            rememberCb.Typeface = Typeface.Serif;
            rememberCb.TextSize = 17;
            rememberCb.Gravity = GravityFlags.Center;

            LoginViewsLayout.Orientation = Android.Widget.Orientation.Horizontal;
            LoginViewsLayout.SetGravity(GravityFlags.Center);

            LoginViewsLayout.SetGravity(GravityFlags.Center);
            LoginViewsLayout.SetForegroundGravity(GravityFlags.Center);
            LoginViewsLayout.AddView(loginBtn);
            LoginViewsLayout.AddView(rememberCb);

            tvForgotPass.Gravity = GravityFlags.Center;
            tvForgotPass.TextSize = 19;
            tvForgotPass.Clickable = true;
            tvForgotPass.Text = "Forgot Your Password? Click Me!";
            tvForgotPass.Typeface = Typeface.Serif;

            SignUpLayout.Orientation = Android.Widget.Orientation.Horizontal;
            SignUpLayout.SetGravity(GravityFlags.Center);

            tvHeader2.Typeface = Typeface.Serif;
            tvHeader2.Text = "Don't Have An Account?";
            tvHeader2.Gravity = GravityFlags.Center;
            tvHeader2.TextSize = 18;

            SignUpBtn.Text = "Sign Up";
            SignUpBtn.Gravity = GravityFlags.Center;
            SignUpBtn.SetAllCaps(false);
            SignUpBtn.SetBackgroundResource(Resource.Drawable.buttonStyle);

            SignUpLayout.AddView(tvHeader2);
            SignUpLayout.AddView(SignUpBtn);

            MainLayout.SetGravity(GravityFlags.Center);

            MainLayout.AddView(ivLogo);
            MainLayout.AddView(tvHeader1);
            MainLayout.AddView(etEmail);
            MainLayout.AddView(emailTv);
            MainLayout.AddView(etPass);
            MainLayout.AddView(passTv);
            MainLayout.AddView(LoginViewsLayout);
            MainLayout.AddView(tvForgotPass);
            MainLayout.AddView(SignUpLayout);

        }
        #endregion
    }
}