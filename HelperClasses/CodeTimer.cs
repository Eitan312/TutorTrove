using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;
using Android.Drm;

namespace finalproject216303628
{
    class CodeTimer : AsyncTask<int, int, string>
    {
        TextView tvSec;
        TextView tvMin;
        Activity context;

        string phone;
        public string code;

        public bool isRun = true;
        public int num;

        public CodeTimer(TextView tvSec, TextView tvMin, string phone, Activity context)
        {
            this.tvSec = tvSec;
            this.tvMin = tvMin;
            this.phone = phone;
            this.context = context;
            Random rnd = new Random();
            this.code = rnd.Next(100000, 1000000).ToString();
            Helper.SendSms(context, phone, code);
            tvMin.Text = "3:";
        }

        protected override string RunInBackground(params int[] @params)
        {
            num = @params[0];
            while (num > 0 && isRun)
            {
                try
                {
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {

                }
                if (isRun)
                {
                    num--;
                    PublishProgress(num);
                }
                if (num == 0)
                {
                    Helper.SendSms(context, phone, code);
                    num = 180;
                    tvMin.Text = "3:";
                }
            }
            return "timer finished " + num;
        }

        protected override void OnProgressUpdate(params int[] values)
        {
            base.OnProgressUpdate(values);
            int sec = values[0] % 60;
            if (sec < 10)
                tvSec.Text = "0" + sec.ToString() + " Until Code Expires";
            else
                tvSec.Text = sec.ToString() + " Until Code Expires";
            if (sec == 59)
                tvMin.Text = (int.Parse(tvMin.Text.Split(':')[0]) - 1).ToString() + ":";
        }

        protected override void OnPostExecute(string result)
        {
            base.OnPostExecute(result);
            tvMin.Text = result;
        }

    }
}