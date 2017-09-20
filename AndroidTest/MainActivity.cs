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
using Android.Views;
using static Android.App.DatePickerDialog;

namespace AndroidTest
{
    [Activity(Label = "AndroidTest", MainLauncher = true)]
    public class MainActivity : Activity 
    {
        

        private TextView topBar;
        private TextView tabDeal;
        private TextView tabSetting;
        private TextView tabUser;
        private FrameLayout Disp_content;
        private Display f2;
        private Setting f3;
        
        
        //private FragmentManager fragmentManager;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            RequestWindowFeature(WindowFeatures.NoTitle);
            
            SetContentView(Resource.Layout.Main);


            BindView();
            //Button button = FindViewById<Button>(Resource.Id.UpdateButton);
            //button.Click += new EventHandler(Button_Click);
            //textView = FindViewById<TextView>(Resource.Id.UpdateText);
            //textView.SetMaxHeight(1000);




        }
        private void BindView()
        {
            topBar = (TextView)FindViewById(Resource.Id.txt_top);
            
            tabDeal = (TextView)FindViewById(Resource.Id.txt_deal);
            //tabPoi = (TextView)FindViewById(Resource.Id.txt_poi);
            tabUser = (TextView)FindViewById(Resource.Id.txt_user);
            tabSetting = (TextView)FindViewById(Resource.Id.txt_setting);
            Disp_content = (FrameLayout)FindViewById(Resource.Id.fragment_container);
            //topBar.Click += TopBar_Click;
            tabDeal.Click += TabDeal_Click;
            //tabPoi.Click += TabPoi_Click;
            tabUser.Click += TabUser_Click;
            tabSetting.Click += TabSetting_Click;
           
            
            //StartDate.Click += StartDate_Click;
            //tabDeal.setOnClickListener(this);
            //tabMore.setOnClickListener(this);
            //tabUser.setOnClickListener(this);
            //tabPoi.setOnClickListener(this);

        }

        

        private void StartDate_Click(object sender, EventArgs e)
        {
        //    IOnDateSetListener listener = new IOnDateSetListener() {

               
        //        public override void OnDateSet(DatePicker arg0, int year, int month, int day)
        //    {
        //        StartDate.Text=year + "-" + (++month) + "-" + day;      //将选择的日期显示到TextView中,因为之前获取month直接使用，所以不需要+1，这个地方需要显示，所以+1  
        //    }
        //};
        //DatePickerDialog dialog = new DatePickerDialog( 0, listener, year, month, day);//后边三个参数为显示dialog时默认的日期，月份从0开始，0-11对应1-12个月  
        //dialog.Show();  
        }

        //重置所有文本的选中状态
        public void Selected()
        {
            tabDeal.Selected = false;
            tabSetting.Selected = false;
            tabUser.Selected = false;
        }

        //隐藏所有Fragment
        public void HideAllFragment(FragmentTransaction transaction)
        {
            //if (f1 != null)
            //{
            //    transaction.Hide(f1);
                
            //}
            if (f2 != null)
            {
                transaction.Hide(f2);
            }
            if (f3 != null)
            {
                transaction.Hide(f3);
            } 
        }
        private void TabSetting_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            HideAllFragment(transaction);
            Selected();
            tabSetting.Selected = true;
            if (f3 == null)
            {
                f3 = new Setting("第一个Fragment");
                transaction.Add(Resource.Id.fragment_container, f3);
                
            }
            else
            {
                transaction.Show(f3);
            }
            transaction.Commit();
            
        }

        private void TabUser_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            HideAllFragment(transaction);
            Selected();
            tabDeal.Selected = true;
            if (f2 == null)
            {
                f2 = new Display("第一个Fragment");
                transaction.Add(Resource.Id.fragment_container, f2);
            }
            else
            {
                transaction.Show(f2);
            }
            transaction.Commit();
        }

        

        private void TabDeal_Click(object sender, EventArgs e)
        {
           

        }

        

       
    }
}

