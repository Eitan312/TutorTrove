using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.BottomNavigation;
using Java.Nio.Channels;
using Org.Apache.Http.Conn;
using System;
using System.Threading;
using System.Drawing;
using System.Text.RegularExpressions;
using SQLite;
using Plugin.Media;
using Android;
using Xamarin.Essentials;
using Android.Support.V4.App;
using static Android.Manifest;
using static Android.Webkit.WebStorage;
using Org.Apache.Commons.Logging;
using Broadcasts;

namespace finalproject216303628
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        LinearLayout mainLl, errorLl;
        Button loginPageBtn, signUpPageBtn;

        WifiBroadcast wifiBroadCast;
        AirplaneModeReceiver airplaneModeReceiver;
        int isFlightMode;

        readonly string[] permissionGroup =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.ManageExternalStorage,
            Manifest.Permission.Camera,
            Manifest.Permission.SendSms,
            Manifest.Permission.AccessNetworkState,
            Manifest.Permission.Internet,
            Manifest.Permission.ReadCalendar,
            Manifest.Permission.WriteCalendar
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.Title = "TutorTrove";

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            Helper.Initialize(this);

            RequestPermissions(permissionGroup, 0);

            loginPageBtn = FindViewById<Button>(Resource.Id.LoginPageBtn);
            signUpPageBtn = FindViewById<Button>(Resource.Id.SignUpPageBtn);
            mainLl = FindViewById<LinearLayout>(Resource.Id.mainLl);
            errorLl = FindViewById<LinearLayout>(Resource.Id.errorLl);

            loginPageBtn.Click += LoginPageBtn_Click;
            signUpPageBtn.Click += SignUpPageBtn_Click;

            MediaPlayer mp = MediaPlayer.Create(this, Resource.Raw.welcome);
            mp.Start();

            wifiBroadCast = new WifiBroadcast(mainLl, errorLl);
            airplaneModeReceiver = new AirplaneModeReceiver(mainLl, errorLl);

            isFlightMode = Android.Provider.Settings.Global.GetInt(this.ContentResolver, Android.Provider.Settings.Global.AirplaneModeOn);
            if (isFlightMode == 1)
                airplaneModeReceiver.flightMode = true;
        }

        private void SignUpPageBtn_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SignUpActivity));
            StartActivity(intent);
        }

        private void LoginPageBtn_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(LoginActivity));
            StartActivity(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnResume()
        {
            base.OnResume();
            RegisterReceiver(wifiBroadCast, new IntentFilter("android.net.wifi.STATE_CHANGE"));
            RegisterReceiver(airplaneModeReceiver, new IntentFilter("android.intent.action.AIRPLANE_MODE"));
        }

        protected override void OnPause()
        {
            UnregisterReceiver(wifiBroadCast);
            UnregisterReceiver(airplaneModeReceiver);
            base.OnPause();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MainActivityMenu, menu);
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
                case Resource.Id.loginMenuBtn:
                    intent = new Intent(this, typeof(LoginActivity));
                    StartActivity(intent);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}