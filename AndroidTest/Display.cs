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
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace AndroidTest
{
    class Display: Fragment
    {
        private String context;
        Thread worker = null;
        Handler handler = new Handler();
        TextView textView = null;
        private AppPreferences ap;
        private Context mContext;
        private String StartDate, EndDate, StartTime, EndTime, Device_id, Api_Key;
        public  Display(String context)
        {
            this.context= context;
            


        }

        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.Display, container, false);
            Button button =view.FindViewById<Button>(Resource.Id.UpdateButton);
            button.Click += new EventHandler(Button_Click);
            textView = view.FindViewById<TextView>(Resource.Id.UpdateText);
            return view;
        }

        private void Button_Click(object sender, System.EventArgs e)
        {
            if (worker == null || !worker.IsAlive)
            {
                textView.Text = "";
                mContext = Android.App.Application.Context;
                ap = new AppPreferences(mContext);
                ReadPara(out  StartDate, out  EndDate, out  StartTime, out  EndTime, out  Device_id, out  Api_Key);
                worker = new Thread(new ThreadStart(UpdateWork));
                worker.Start();
            }

        }

        private void UpdateWork()
        {
            //String TIME = "";
            List<String> TIME=null;
            List<String> Data = null;
            JObject jo = null;
            JToken js = null;
            String starttime, endtime;
            starttime = "&start=" + StartDate+"T"+ StartTime;
            endtime= "&end=" +  EndDate + "T" + EndTime;
            String apiheader = "api-key:" + Api_Key;
            //String Device_id = "11393745";
            Uri UpdateUri = new Uri("http://api.heclouds.com/devices/" + Device_id + "/datapoints?datastream_id=REPORT_STATE"+ starttime+endtime);






            HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(UpdateUri);
            WebHeaderCollection headers = wbRequest.Headers;
            headers.Add(apiheader);
            wbRequest.Headers = headers;
            wbRequest.ContentType = "application/json";
            wbRequest.AllowAutoRedirect = false;
            wbRequest.Timeout = 5000;
            wbRequest.Method = "GET";
            try
            {
                HttpWebResponse response = (HttpWebResponse)wbRequest.GetResponse();

                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                jo = (JObject)JsonConvert.DeserializeObject(retString);
                if (jo["errno"].ToString().Contains("0") && (jo["error"].ToString().Contains("succ")))
                {

                    int len =Convert.ToInt32(jo["data"]["count"].ToString());
                    if(len>0)
                    {
                        for(int i=0;i<len;i++)
                        {
                            TIME.Add(jo["data"]["datastreams"][0]["datapoints"][i]["at"].ToString());
                            Data.Add(jo["data"]["datastreams"][0]["datapoints"][i]["value"].ToString());
                            handler.Post(() =>
                            {
                                textView.Text += TIME[i] + "\r\n" + Data[i];
                            });
                        }
                    }
                    //TIME = jo["data"]["datastreams"][0]["datapoints"][0]["at"].ToString();
                    js = jo["data"]["datastreams"][0]["datapoints"][0]["value"];
                    // JToken jv = (JToken)js["datapoints"];

                }
            }
            catch
            {

            }
            
            worker.Abort();
        }
        private bool ReadPara(out String StartDate, out String EndDate, out String StartTime, out String EndTime, out String Device_id, out String Api_Key)
        {
            if ((ap.GetAccessKey("StartDate", out StartDate) == false) ||
                (ap.GetAccessKey("StartTime", out StartTime) == false) ||
                (ap.GetAccessKey("EndDate", out EndDate) == false) ||
                (ap.GetAccessKey("EndTime", out EndTime) == false) ||
                (ap.GetAccessKey("Device_ID", out Device_id) == false) ||
                (ap.GetAccessKey("Api_Key", out Api_Key) == false))
            {
                StartDate = "";
                StartTime = "";
                EndDate = "";
                EndTime = "";
                Device_id = "";
                Api_Key = "";
                return false;
            }
            else return true;

        }
    }
}