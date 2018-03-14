package com.example.xuhongcheng.archipelago.activitys;

import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.Toast;

import com.example.xuhongcheng.archipelago.myapplication.R;
import com.example.xuhongcheng.archipelago.utils.SharedPreferenceUtils;


public class MainActivity extends AppCompatActivity {

    public Button singlePlayer;
    public Button hostMulti;
    public Button joinMulti;
    public Button logout;
    public Button profile;
    public ImageButton btn_settings;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        singlePlayer = (Button) findViewById(R.id.singlePlayer);
        hostMulti = (Button) findViewById(R.id.host);
        joinMulti = (Button) findViewById(R.id.join);
        logout = (Button) findViewById(R.id.logout);
        btn_settings = (ImageButton) findViewById(R.id.setting);
        profile = (Button) findViewById(R.id.profile);

        boolean isLogin = SharedPreferenceUtils.getBoolean(MainActivity.this,"isLogin",false);
        if(!isLogin){
            profile.setVisibility(View.INVISIBLE);
        }




        singlePlayer.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent launchIntent = getPackageManager().getLaunchIntentForPackage(getResources().getString(R.string.gamePackageName));
                if (launchIntent != null) {
                    startActivity(launchIntent);
                } else {
                    Toast.makeText(getApplicationContext(), "Could not find game APK", Toast.LENGTH_SHORT).show();
                }
            }
        });

        logout.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                SharedPreferenceUtils.saveBoolean(MainActivity.this, "isSaved", false);
                startActivity(new Intent(MainActivity.this, LoginActivity.class));
            }
        });

        btn_settings.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startActivity(new Intent(MainActivity.this, SettingActivity.class));
            }
        });

        profile.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startActivity(new Intent(MainActivity.this, ProfileActivity.class));
            }
        });
    }


}
