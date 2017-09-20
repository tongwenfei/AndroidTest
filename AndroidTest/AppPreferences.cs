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
using Android.Preferences;

namespace AndroidTest
{
    class AppPreferences
    {
        private ISharedPreferences mSharedPrefs;
        private ISharedPreferencesEditor mPrefsEditor;
        private Context mContext;

        private static String PREFERENCE_ACCESS_KEY = "PREFERENCE_ACCESS_KEY";

        public AppPreferences(Context context)
        {
            this.mContext = context;
            mSharedPrefs = PreferenceManager.GetDefaultSharedPreferences(mContext);
            mPrefsEditor = mSharedPrefs.Edit();
        }

        public void SaveAccessKey(string vaule)
        {
            mPrefsEditor.PutString(PREFERENCE_ACCESS_KEY, vaule);
            mPrefsEditor.Commit();
        }
        public void SaveAccessKey(string key,String vaule)
        {
            mPrefsEditor.PutString(key, vaule);
            mPrefsEditor.Commit();
        }

        public string GetAccessKey()
        {
            return mSharedPrefs.GetString(PREFERENCE_ACCESS_KEY, "");
        }
        public bool GetAccessKey(String key,out String vaule)
        {
            bool ret = true;
            try
            {
                vaule=mSharedPrefs.GetString(key, "");
            }
            catch
            {
                vaule = "";
                ret = false;
            }
            return ret;
        }
    }
}