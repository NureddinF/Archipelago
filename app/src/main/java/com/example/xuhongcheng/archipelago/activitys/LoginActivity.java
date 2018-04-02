package com.example.xuhongcheng.archipelago.activitys;

import android.Manifest;
import android.app.Activity;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.media.AudioManager;
import android.media.SoundPool;
import android.os.Bundle;
import android.support.v4.app.ActivityCompat;
import android.util.Log;
import android.view.View;
import android.view.Window;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.TextView;
import android.widget.Toast;

import com.example.xuhongcheng.archipelago.myapplication.R;
import com.example.xuhongcheng.archipelago.utils.SharedPreferenceUtils;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.InputStreamReader;
import java.util.Random;

public class LoginActivity extends Activity {

	private String name;  // Local username
	private String pass;  // Local password
	EditText et_name;
	EditText et_pass;
	TextView tv_guest_login;
	ImageButton btn_setting;
    private SoundPool soundPool;
    private int soundId;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		requestWindowFeature(Window.FEATURE_NO_TITLE);
		setContentView(R.layout.activity_login);
		checkPermission();
		//Acquire inputs from user
		readAccount(); //Read the username and password for local files
		et_name = (EditText) findViewById(R.id.et_name);
		et_pass = (EditText) findViewById(R.id.et_pass);
		tv_guest_login = (TextView) findViewById(R.id.tv_guest_login);
		btn_setting = (ImageButton) findViewById(R.id.setting);
        btn_setting.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                soundPool.play(soundId, 1, 1, 0, 0, 1);
                startActivity(new Intent(LoginActivity.this, SettingActivity.class));
            }
        });
        tv_guest_login.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                soundPool.play(soundId, 1, 1, 0, 0, 1);
                String tempName = "Player" + new Random().nextInt();
                SharedPreferenceUtils.saveString(LoginActivity.this, "username", tempName);
                startActivity(new Intent(LoginActivity.this, MainActivity.class));
            }
        });
		//**Use Utils method, used to check
		//判断是否保存过信息，如果isSaved存在代表保存过,返回true，不存在默认返回false
		if (SharedPreferenceUtils.getBoolean(LoginActivity.this, "isSaved", false)) {
			//把账号密码回显至输入框
			et_name.setText(name);
			et_pass.setText(pass);
		}
        soundPool = new SoundPool(10, AudioManager.STREAM_SYSTEM, 5);
        soundId = soundPool.load(this, R.raw.click_bgm, 1);
	}
	public void login(View v) {
        readAccount(); //读取本地文件的账号密码
		switch (v.getId()) {
			case R.id.login: {
                soundPool.play(soundId, 1, 1, 0, 0, 1);
                CheckBox cb = (CheckBox) findViewById(R.id.cb);
                //记录 保存密码 设置
                if (cb.isChecked()) {
                    SharedPreferenceUtils.saveBoolean(LoginActivity.this, "isSaved", true);
                } else {
                    SharedPreferenceUtils.saveBoolean(LoginActivity.this, "isSaved", false);
                }
                //1拿到用户输入的账号和密码
                String name1 = et_name.getText().toString();
                String pass1 = et_pass.getText().toString();
                //2.检验账号和密码
                if (name1.equals(name) && pass1.equals(pass)) {
                    Toast.makeText(this, "Login Successfully!", Toast.LENGTH_SHORT).show();
                    SharedPreferenceUtils.saveBoolean(LoginActivity.this, "isLogin", true);
                    SharedPreferenceUtils.saveString(LoginActivity.this, "username", name);
                    startActivity(new Intent(this, MainActivity.class));
                } else {
                    Toast.makeText(this, "Wrong username or password!", Toast.LENGTH_SHORT).show();
                }

                break;
            }
			case R.id.regist: {
                soundPool.play(soundId, 1, 1, 0, 0, 1);
                startActivity(new Intent(this, RegisterActivity.class));
                break;
            }
		}
	}

    @Override
    protected void onResume() {
        super.onResume();
        startService(new Intent(this, MusicService.class));
    }

    //回显账号
    public void readAccount() {
        //读取本地存储的账号和密码
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
                fis.close();
            } catch (Exception e) {
                // TODO Auto-generated catch block
                e.printStackTrace();
            }
        }
    }

    private void checkPermission() {
        //检查权限（NEED_PERMISSION）是否被授权 PackageManager.PERMISSION_GRANTED表示同意授权
        if (ActivityCompat.checkSelfPermission(this, Manifest.permission.WRITE_EXTERNAL_STORAGE)
                != PackageManager.PERMISSION_GRANTED) {
            //用户已经拒绝过一次，再次弹出权限申请对话框需要给用户一个解释
            if (ActivityCompat.shouldShowRequestPermissionRationale(this, Manifest.permission
                    .WRITE_EXTERNAL_STORAGE)) {
//                Toast.makeText(this, "请开通相关权限，否则无法正常使用本应用！", Toast.LENGTH_SHORT).show();
            }
            //申请权限
            ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE}, 1);

        } else {
			Log.e("XXX", "checkPermission: 已经授权！");
		}
	}

    @Override
    protected void onStop() {
        super.onStop();
        SharedPreferenceUtils.saveBoolean(LoginActivity.this, "isLogin", false);
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        stopService(new Intent(this, MusicService.class));
    }
}
