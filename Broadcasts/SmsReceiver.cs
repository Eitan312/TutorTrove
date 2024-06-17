using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Telephony;
using Android.Widget;

namespace finalproject216303628
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" }, Priority = (int)IntentFilterPriority.HighPriority)]
    public class SmsReceiver : BroadcastReceiver
    {
        EditText codeEt;

        public SmsReceiver() { }

        public SmsReceiver(EditText codeEt)
        {
            this.codeEt = codeEt;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action.Equals("android.provider.Telephony.SMS_RECEIVED"))
            {
                startBroadCast(intent, context);
            }
        }

        public void startBroadCast(Intent intent, Context context)
        {
            SmsMessage[] messages = null;
            string strMessage = "";

            if (intent.HasExtra("pdus"))
            {
                var pdus = (Java.Lang.Object[])intent.Extras.Get("pdus");
                messages = new SmsMessage[pdus.Length];

                if(messages.Length > 0)
                {
                    SmsMessage latestMessage;
                    if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
                    {
                        string format = intent.GetStringExtra("format");
                        latestMessage = SmsMessage.CreateFromPdu((byte[])pdus[messages.Length - 1], format);
                    }
                    else
                    {
                        latestMessage = SmsMessage.CreateFromPdu((byte[])pdus[messages.Length - 1]);
                    }

                    codeEt.Text = latestMessage.MessageBody;
                }
            }
        }
    }
}