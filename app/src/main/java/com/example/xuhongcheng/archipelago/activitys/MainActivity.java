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

import com.MobileComputingGrp3.UnityPlayerActivity;

public class MainActivity extends AppCompatActivity {

    private static final int PLAY_GAME = 600;

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
        //https://stackoverflow.com/questions/6173400/how-to-hide-a-button-programmatically
        boolean isLogin = SharedPreferenceUtils.getBoolean(MainActivity.this,"isLogin",false);
        if(!isLogin){
            profile.setVisibility(View.INVISIBLE);
        }

        singlePlayer.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {

                Intent launchIntent = new Intent(getApplicationContext(), UnityPlayerActivity.class);
                if (launchIntent != null) {
                    startActivityForResult(launchIntent, PLAY_GAME);
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

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data){
        //Toast.makeText(getApplicationContext(), "Game Closed, returned to menu.", Toast.LENGTH_SHORT).show();
    }

}
