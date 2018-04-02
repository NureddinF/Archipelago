package com.example.xuhongcheng.archipelago.activitys;

import android.app.Application;
import android.content.Context;
import android.support.multidex.MultiDex;
//https://stackoverflow.com/questions/26609734/how-to-enable-multidexing-with-the-new-android-multidex-support-library
public class BaseApplication extends Application {

    @Override
    protected void attachBaseContext(Context base) {
        super.attachBaseContext(base);
        MultiDex.install(this);
    }
}
