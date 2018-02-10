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
}
