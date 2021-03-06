﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Text;
using Android.Preferences;

namespace AndroidTest
{
    class Setting : Fragment
    {
        Context mContext ;
        AppPreferences ap ;
        private String context;
        // private TextView mTextView;
        private Button SavePara;
        private EditText StartDate, EndDate, StartTime, EndTime,Device_id,Api_Key,Limit_Count;
        


        public Setting(String context)
        {
            this.context = context;



        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.Setting, container, false);
            
            //mTextView = (TextView)view.FindViewById(Resource.Id.txt_content);
            ////mTextView = (TextView)getActivity().findViewById(R.id.txt_content);
            //mTextView.Text = context;
            SavePara = (Button)view.FindViewById(Resource.Id.SaveBtn);
            StartDate = (EditText)view.FindViewById(Resource.Id.SD_ShowDialog);
            EndDate = (EditText)view.FindViewById(Resource.Id.ED_ShowDialog);
            StartTime = (EditText)view.FindViewById(Resource.Id.ST_ShowDialog);
            EndTime = (EditText)view.FindViewById(Resource.Id.ET_ShowDialog);
            Device_id = (EditText)view.FindViewById(Resource.Id.Device_ID_Input);
            Api_Key = (EditText)view.FindViewById(Resource.Id.API_KEY_Input);
            Limit_Count=(EditText)view.FindViewById(Resource.Id.Limit_ShowDialog);
            
            
            mContext = Android.App.Application.Context;
            ap = new AppPreferences(mContext);
            if (ReadPara(out String StartDatesStr, out String EndDateStr, out String StartTimeStr, out String EndTimeStr, out String Device_idStr, out String Api_KeyStr, out String LimitStr))
            {
                StartDate.Text = StartDatesStr;
                EndDate.Text = EndDateStr;
                StartTime.Text = StartTimeStr;
                EndTime.Text = EndTimeStr;
                Device_id.Text = Device_idStr;
                Api_Key.Text = Api_KeyStr;
                Limit_Count.Text = LimitStr;

            }
            else
            {
                Context sContext = Android.App.Application.Context;
                Toast.MakeText(sContext, "读取参数出错", ToastLength.Short).Show();
            } 
            SavePara.Click += SavePara_Click;

            return view;
        }
        private void SavePara_Click(object sender, EventArgs e)
        {
            
            Context sContext = Android.App.Application.Context;
            if (IsValidDate(StartDate.Text) && IsValidDate(EndDate.Text)&&IsValidTime(StartTime.Text) && IsValidTime(EndTime.Text)&&(Device_id.Text!=String.Empty) &&(Api_Key.Text!=String.Empty) &&(Limit_Count.Text != String.Empty))
            {
                if ((Convert.ToInt16(Limit_Count.Text) > 0) && (Convert.ToInt16(Limit_Count.Text) <= 6000))
                {
                    ap.SaveAccessKey("StartDate", StartDate.Text);
                    ap.SaveAccessKey("EndDate", EndDate.Text);
                    ap.SaveAccessKey("StartTime", StartTime.Text);
                    ap.SaveAccessKey("EndTime", EndTime.Text);
                    ap.SaveAccessKey("Device_ID", Device_id.Text);
                    ap.SaveAccessKey("Api_Key", Api_Key.Text);
                    ap.SaveAccessKey("Limit_Count", Limit_Count.Text);
                   
                    Toast.MakeText(sContext, "保存成功", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(sContext, "输入参数出错", ToastLength.Short).Show();
                }
                
            }
            else
            {
                Toast.MakeText(sContext, "输入参数出错", ToastLength.Short).Show();
            }
        }
        public static bool IsValidDate(String str)
        {
            bool convertSuccess = true;
            // 指定日期格式为四位年/两位月份/两位日期，注意yyyy/MM/dd区分大小写；
            SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd");
            if(str==null||str==String.Empty)
            {
                return false;
            }
            try
            {
                // 设置lenient为false. 否则SimpleDateFormat会比较宽松地验证日期，比如2007/02/29会被接受，并转换成2007/03/01
                format.Lenient = false;
                format.Parse(str);
            }
            catch 
            {
                // e.printStackTrace();
                // 如果throw java.text.ParseException或者NullPointerException，就说明格式不对
                convertSuccess = false;
            }
            return convertSuccess;
        }
        public static bool IsValidTime(String str)
        {
            bool convertSuccess = true;
            // 指定日期格式为四位年/两位月份/两位日期，注意yyyy/MM/dd区分大小写；
            SimpleDateFormat format = new SimpleDateFormat("hh:mm:ss");
            if (str == null || str == String.Empty)
            {
                return false;
            }
            try
            {
                // 设置lenient为false. 否则SimpleDateFormat会比较宽松地验证日期，比如2007/02/29会被接受，并转换成2007/03/01
                
                format.Parse(str);
            }
            catch 
            {
                // e.printStackTrace();
                // 如果throw java.text.ParseException或者NullPointerException，就说明格式不对
                convertSuccess = false;
            }
            return convertSuccess;
        }
        private  bool ReadPara(out String StartDate, out String EndDate, out String StartTime, out String EndTime, out String Device_id, out String Api_Key,out String limit_count)
        {
            if ((ap.GetAccessKey("StartDate", out StartDate) == false) ||
                (ap.GetAccessKey("StartTime", out StartTime) == false) ||
                (ap.GetAccessKey("EndDate", out EndDate) == false) ||
                (ap.GetAccessKey("EndTime", out EndTime) == false) ||
                (ap.GetAccessKey("Device_ID", out Device_id) == false) ||
                (ap.GetAccessKey("Api_Key", out Api_Key) == false)||
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


    }
   
}