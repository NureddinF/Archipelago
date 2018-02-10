package com.example.xuhongcheng.archipelago.activitys;

import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.widget.Button;
import android.widget.ImageButton;

import com.example.xuhongcheng.archipelago.R;
import com.example.xuhongcheng.archipelago.SinglePlayerActivity;


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

        singlePlayer = findViewById(R.id.singlePlayer);
        hostMulti = findViewById(R.id.host);
        joinMulti = findViewById(R.id.join);
        logout = findViewById(R.id.logout);
        btn_settings = findViewById(R.id.setting);
        profile = findViewById(R.id.profile);

        singlePlayer.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startActivity(new Intent(MainActivity.this, SinglePlayerActivity.class));
            }
        });

        logout.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startActivity(new Intent(MainActivity.this, LoginActivity.class));
            }
        });

        btn_settings.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startActivity(new Intent(MainActivity.this, SettingActivity.class ));
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
