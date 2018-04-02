package com.example.xuhongcheng.archipelago.activitys;

import android.content.Intent;
import android.media.AudioManager;
import android.media.SoundPool;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.TextView;

import com.example.xuhongcheng.archipelago.myapplication.R;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.InputStreamReader;


public class ProfileActivity extends AppCompatActivity {

    public Button logout;
    public ImageButton btn_setting;
    public TextView tv_userName;
    public TextView tv_email;
    private SoundPool soundPool;
    private int  soundId;
    private String name;  // Local username
    private String pass;  // Local password
    private String email;  // Local email

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_profile);

        logout = (Button) findViewById(R.id.logout);
        btn_setting = (ImageButton) findViewById(R.id.setting);
        tv_userName = (TextView) findViewById(R.id.username);
        tv_email = (TextView) findViewById(R.id.email);
        btn_setting.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startActivity(new Intent(ProfileActivity.this,SettingActivity.class));
            }
        });

        logout.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                soundPool.play(soundId,1,1,0,0,1);
                startActivity(new Intent(ProfileActivity.this, LoginActivity.class));
            }
        });

        soundPool = new SoundPool(10, AudioManager.STREAM_SYSTEM, 5);
        soundId = soundPool.load(this,R.raw.doink,1);
    }

    @Override
    protected void onResume() {
        super.onResume();
        readAccount();
        tv_userName.setText("Username: "+ name);
        tv_email.setText("Email: "+email);
    }

    //Get Account Info
    public void readAccount() {
        //读取本地存储的账号和密码
//    	File file = new File(getFilesDir(), "/info.txt");
        File file = new File(getCacheDir(), "/info.txt");
        if (file.exists()) {
            Log.i("XXX", "xxxxxxxx");
            try {
                FileInputStream fis = new FileInputStream(file);
                //把字节流转换成字符流
                BufferedReader br = new BufferedReader(new InputStreamReader(fis));
                String text = br.readLine();
                String[] s = text.split("##");
                Log.i("XXX", s.toString());
                name = s[0];
                pass = s[1];
                email = s[2];
                fis.close();
            } catch (Exception e) {
                // TODO Auto-generated catch block
                e.printStackTrace();
            }
        }
    }
}
