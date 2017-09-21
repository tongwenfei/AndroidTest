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
using Android.Text;
using Android.Graphics;
using Java.IO;

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
        private String StartDate, EndDate, StartTime, EndTime, Device_id, Api_Key,Limit_Cout;
        private delegate void MyDelegate(int value);
        private List<string> TIME = new List<string>();
        private List<String> Data = new List<string>();
        
        public  Display(String context)
        {
            this.context= context;
            


        }

        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.Display, container, false);
            Button button =view.FindViewById<Button>(Resource.Id.UpdateButton);
            button.Click += new EventHandler(Button_Click);
            Button export = view.FindViewById<Button>(Resource.Id.ExportButton);
            export.Click += Export_Click;
            textView = view.FindViewById<TextView>(Resource.Id.UpdateText);
            
            return view;
        }

        private void Export_Click(object sender, EventArgs e)
        {
            DateTime t = DateTime.Now;
            string s = t.ToString("yyyy年MM月dd日_HH:mm:ss");
            Context sContext = Android.App.Application.Context;
            Toast.MakeText(sContext, "开始写入到：" + s+".txt", ToastLength.Short).Show();
            writeInStorage(s, textView.Text);
        }

        private void Button_Click(object sender, System.EventArgs e)
        {
            if (worker == null || !worker.IsAlive)
            {
                Context sContext = Android.App.Application.Context;
                textView.Text = "";
                mContext = Android.App.Application.Context;
                ap = new AppPreferences(mContext);
                ReadPara(out  StartDate, out  EndDate, out  StartTime, out  EndTime, out  Device_id, out  Api_Key,out Limit_Cout);
                worker = new Thread(new ThreadStart(UpdateWork));
                Toast.MakeText(sContext, "开始发送请求" , ToastLength.Short).Show();
                worker.Start();
            }

        }

        private void UpdateWork()
        {
            
            TIME.Clear();
            Data.Clear();
            MyDelegate d = new MyDelegate(UpdateText);
            JObject jo = null;
            //JToken js = null;
            String starttime, endtime,limit;
            starttime = "&start=" + StartDate+"T"+ StartTime;
            endtime= "&end=" +  EndDate + "T" + EndTime;
            String apiheader = "api-key:" + Api_Key;
            limit = "&limit=" + Limit_Cout;
            //String Device_id = "11393745";
            Uri UpdateUri = new Uri("http://api.heclouds.com/devices/" + Device_id + "/datapoints?datastream_id=REPORT_STATE"+ starttime+endtime+ limit);






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
                    Context sContext = Android.App.Application.Context;
                    handler.Post(() =>
                    {
                        Toast.MakeText(sContext, "请求成功，正在解析数据", ToastLength.Short).Show();
                    });
                    
                    int len =Convert.ToInt32(jo["data"]["count"].ToString());
                    if(len>0)
                    {
                        for(int i=0;i<len;i++)
                        {
                            TIME.Add(jo["data"]["datastreams"][0]["datapoints"][i]["at"].ToString());
                            Data.Add(jo["data"]["datastreams"][0]["datapoints"][i]["value"].ToString());

                            
                            //this.Dispatcher.Invoke(d, i);
                        }
                    }
                    handler.Post(() =>
                    {
                        textView.Text = "       ***********Count=" + len.ToString()+ "***********\r\n";
                        for (int i = 0; i < len; i++)

                        {
                            //textView.SetTextColor(Color.Red);
                            //textView.SetText(Html.FromHtml("<font color='#ff0000'>" + TIME[i] + "</font>") + "\r\n", TextView.BufferType.Normal);
                            textView.Text += "       ***********Data " + (i+1).ToString()+ " Start***********\r\n"+ TIME[i]  + "\r\n" + Data[i] + "\r\n       ***********Data " + (i + 1).ToString() + " End***********\r\n\r\n";
                            //textView.SetTextColor(Color.White);
                            //textView.Text += Data[i] + "\r\n";
                            //String txt = String.Format("<font color='#ff0000'>" + TIME[i] + "</font>" + "\r\n");
                            //textView.TextFormatted  = Html.FromHtml(txt);
                            //textView.Text += Data[i] + "\r\n";
                            //textView.Text += Html.FromHtml("<font color='#ff0000'>" + TIME[i] + "</font>") + "\r\n" + Data[i] + "\r\n";
                        }
                             
                        


                    });
                    //TIME = jo["data"]["datastreams"][0]["datapoints"][0]["at"].ToString();
                    //js = jo["data"]["datastreams"][0]["datapoints"][0]["value"];
                    // JToken jv = (JToken)js["datapoints"];

                }
                else
                {
                    handler.Post(() =>
                    {
                        textView.Text = "请求参数错误";
                    });
                    
                }
            }
            catch
            {
                handler.Post(() =>
                {
                    textView.Text = "请求超时";
                });
            }
            
            worker.Abort();
        }

        private void UpdateText(int value)
        {
            textView.Text += TIME[value] + "\r\n" + Data[value];
        }

        private bool ReadPara(out String StartDate, out String EndDate, out String StartTime, out String EndTime, out String Device_id, out String Api_Key, out String limit_count)
        {
            if ((ap.GetAccessKey("StartDate", out StartDate) == false) ||
                (ap.GetAccessKey("StartTime", out StartTime) == false) ||
                (ap.GetAccessKey("EndDate", out EndDate) == false) ||
                (ap.GetAccessKey("EndTime", out EndTime) == false) ||
                (ap.GetAccessKey("Device_ID", out Device_id) == false) ||
                (ap.GetAccessKey("Api_Key", out Api_Key) == false) ||
                (ap.GetAccessKey("Limit_Count", out limit_count) == false))
            {
                StartDate = "";
                StartTime = "";
                EndDate = "";
                EndTime = "";
                Device_id = "";
                Api_Key = "";
                limit_count = "";
                return false;
            }
            else return true;

        }
        private void writeInStorage(String fileName, String TEXT)
        {
            bool test;
            Context sContext = Android.App.Application.Context;
            try
            {
                Java.IO.File dirv = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath + "/AndroidTest/"+ fileName + ".txt");
                Java.IO.File path=new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath + "/AndroidTest"); ;
                if (!path.Exists())
                {
                    path.Mkdirs();
                }
                //Java.IO.File dirv = new Java.IO.File("/storage/emulated/AndroidTest");
                if (!dirv.Exists())//工作目录是否存在？
                {
                    
                     test=   dirv.CreateNewFile();
                    
                }
                
                FileStream fileOS = new FileStream(dirv.ToString(), System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                //String str = "this is a test about Write SD card file";
                Java.IO.BufferedWriter buf1 = new Java.IO.BufferedWriter(new Java.IO.OutputStreamWriter(fileOS));
                buf1.Write(TEXT, 0, TEXT.Length);
                buf1.Flush();
                buf1.Close();
                fileOS.Close();
                
                Toast.MakeText(sContext, "写入完成", ToastLength.Short).Show();
            }
            catch (Java.IO.FileNotFoundException e)
            {
                
                Toast.MakeText(sContext, e.ToString(), ToastLength.Short).Show();
            }
            catch (UnsupportedEncodingException e)
            {
                
                Toast.MakeText(sContext, e.ToString(), ToastLength.Short).Show();
            }
            catch (Java.IO.IOException e)
            {
                
                Toast.MakeText(sContext, e.ToString(), ToastLength.Short).Show();
            }
        }
    }
}