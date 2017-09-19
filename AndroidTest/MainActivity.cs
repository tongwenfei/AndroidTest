using Android.App;
using Android.Widget;
using Android.OS;
using System;
using System.Threading;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AndroidTest
{
    [Activity(Label = "AndroidTest", MainLauncher = true)]
    public class MainActivity : Activity
    {
        Thread worker = null;
        Handler handler = new Handler();
        TextView textView = null;
       
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            Button button = FindViewById<Button>(Resource.Id.UpdateButton);
            button.Click += new EventHandler(Button_Click);
            textView = FindViewById<TextView>(Resource.Id.UpdateText);
            //textView.SetMaxHeight(1000);
            
         


        }

        private void Button_Click(object sender, System.EventArgs e)
        {
            if (worker == null || !worker.IsAlive)
            {
                textView.Text = "";
                 worker = new Thread(new ThreadStart(UpdateWork));
                worker.Start();
            }
            
        }

        private void UpdateWork()
        {
            String TIME="";
            JObject jo=null;
            JToken js = null;
           

                String Device_id = "11393745";
                Uri UpdateUri = new Uri("http://api.heclouds.com/devices/" + Device_id + "/datapoints?datastream_id=REPORT_STATE");
             
                




                HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(UpdateUri);
                WebHeaderCollection headers = wbRequest.Headers;
                headers.Add("api-key:DLy5z6Bks=RibaGWFMJC7IDhC0A=");
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
                        TIME = jo["data"]["datastreams"][0]["datapoints"][0]["at"].ToString();
                         js = jo["data"]["datastreams"][0]["datapoints"][0]["value"];
                        // JToken jv = (JToken)js["datapoints"];
                        
                    }
                }
                catch
                {

                }
                handler.Post(() =>
                {
                    textView.Text = TIME+"\r\n" +js.ToString();
                });
            worker.Abort();
        }
    }
}

