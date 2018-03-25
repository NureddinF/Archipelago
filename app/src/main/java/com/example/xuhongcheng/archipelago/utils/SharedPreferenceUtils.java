package com.example.xuhongcheng.archipelago.utils;

import android.content.Context;
import android.content.SharedPreferences;

public class SharedPreferenceUtils {
	private static SharedPreferences sp;
	private static String SP_NAME = "config1";
	public static void saveBoolean(Context context,String key,boolean value) {
		// TODO Auto-generated method stub
//		if(sp == null)
			sp = context.getSharedPreferences(SP_NAME , 0);
		sp.edit().putBoolean(key, value).commit();
		
	}
	public static boolean getBoolean(Context context,String key,boolean defValue) {
		// TODO Auto-generated method stub
		// TODO Auto-generated method stub
//		if(sp == null)
			sp = context.getSharedPreferences(SP_NAME , 0);
		return sp.getBoolean(key, defValue);
		
	}



	public static void saveString(Context context,String key,String value) {
		sp = context.getSharedPreferences(SP_NAME , 0);
		sp.edit().putString(key, value).commit();

	}
	public static String getString(Context context,String key,String defValue) {
		sp = context.getSharedPreferences(SP_NAME , 0);
		return sp.getString(key, defValue);

	}


	public class password {

		String s;

		public password(String s) {
			this.s = s;
		}

		public int Name(){
			if (s.equals("password"))
				return 0;
			else
				return 1;
		}

		public int Length(){
			if (s.length() >= 8)
				return 1;
			else
				return 0;
		}

		public int Special(){
			if (s.contains("s"))
				return 1;
			else
				return 0;
		}

		public int Digit(){
			if (s.matches(".*\\d+.*"))
				return 1;
			else
				return 0;
		}

		public int UpLo(){
			if (s.equals(s.toLowerCase()) || s.equals(s.toUpperCase()))
				return 0;
			else
				return 1;
		}

		public int pass() {
			return Name() + Length() + Special() + Digit() + UpLo();
		}



	}
}
