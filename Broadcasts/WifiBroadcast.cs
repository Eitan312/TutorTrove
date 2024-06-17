using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Org.Apache.Commons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace finalproject216303628
{
    [BroadcastReceiver]
    public class WifiBroadcast : BroadcastReceiver
    {
        LinearLayout mainLl;
        LinearLayout errorLl;

        public WifiBroadcast() { }

        public WifiBroadcast(LinearLayout mainLl, LinearLayout errorLl)
        {
            this.mainLl = mainLl;
            this.errorLl = errorLl;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            string action = intent.Action;

            if (action.Equals("android.net.wifi.STATE_CHANGE"))
            {
                WifiManager wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);
                WifiInfo wifiInfo = wifiManager.ConnectionInfo;

                if(wifiInfo.IpAddress == 0)
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
}