using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Broadcasts
{
    [BroadcastReceiver]
    public class AirplaneModeReceiver : BroadcastReceiver
    {
        public bool flightMode;
        LinearLayout mainLl;
        LinearLayout errorLl;

        public AirplaneModeReceiver() { }

        public AirplaneModeReceiver(LinearLayout mainLl, LinearLayout errorLl)
        {
            this.mainLl = mainLl;
            this.errorLl = errorLl;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            string action = intent.Action;

            if (action.Equals("android.intent.action.AIRPLANE_MODE"))
            {
                flightMode = intent.GetBooleanExtra("state", false);
            }

            if (flightMode)
            {
                mainLl.Visibility = ViewStates.Gone;
                errorLl.Visibility = ViewStates.Visible;
            }
            else
            {
                errorLl.Visibility = ViewStates.Gone;
                mainLl.Visibility = ViewStates.Visible;
            }
        }
    }
}