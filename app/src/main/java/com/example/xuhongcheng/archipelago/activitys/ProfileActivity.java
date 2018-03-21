package com.example.xuhongcheng.archipelago.activitys;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.TextView;

import com.example.xuhongcheng.archipelago.myapplication.R;


public class ProfileActivity extends Activity {

    public Button logout;
    public ImageButton btn_setting;
    public TextView userName;
    public TextView wins;
    public TextView losses;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_profile);

        logout = (Button) findViewById(R.id.logout);
        btn_setting = (ImageButton) findViewById(R.id.setting);
        userName = (TextView) findViewById(R.id.username);
        wins = (TextView) findViewById(R.id.wins);
        losses = (TextView) findViewById(R.id.losses);

        btn_setting.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startActivity(new Intent(ProfileActivity.this,SettingActivity.class));
            }
        });

        logout.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startActivity(new Intent(ProfileActivity.this, LoginActivity.class));
            }
        });
    }
}
