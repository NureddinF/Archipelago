package com.example.xuhongcheng.archipelago.activitys;

import android.app.Activity;
import android.content.Intent;
import android.content.res.Configuration;
import android.media.AudioManager;
import android.media.SoundPool;
import android.net.wifi.WifiManager;
import android.os.Bundle;
import android.text.format.Formatter;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.Toast;

import com.MobileComputingGrp3.UnityPlayerActivity;

import com.example.xuhongcheng.archipelago.myapplication.R;
import com.example.xuhongcheng.archipelago.utils.SharedPreferenceUtils;

public class MainActivity extends Activity {

    private static final int PLAY_GAME = 600;

    public Button singlePlayer;
    public Button hostMulti;
    public Button joinMulti;
    public Button logout;
    public Button profile;
    public ImageButton btn_settings;
    private SoundPool soundPool;
    private int  soundId;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.i("onCreate","-----");
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

        singlePlayer.setOnClickListener(new GameLauncher("play"));
        hostMulti.setOnClickListener(new GameLauncher("host"));
        joinMulti.setOnClickListener(new GameLauncher("join"));

        logout.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                soundPool.play(soundId,1,1,0,0,1);
                SharedPreferenceUtils.saveBoolean(MainActivity.this, "isSaved", false);
                startActivity(new Intent(MainActivity.this, LoginActivity.class));
            }
        });

        btn_settings.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                soundPool.play(soundId,1,1,0,0,1);
                startActivity(new Intent(MainActivity.this, SettingActivity.class));
            }
        });

        profile.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                soundPool.play(soundId,1,1,0,0,1);
                startActivity(new Intent(MainActivity.this, ProfileActivity.class));
            }
        });

        soundPool = new SoundPool(10, AudioManager.STREAM_SYSTEM, 5);
        soundId = soundPool.load(this,R.raw.doink,1);
    }

    @Override
    public void onConfigurationChanged(Configuration newConfig) {
        super.onConfigurationChanged(newConfig);
        if (getResources().getConfiguration().orientation == Configuration.ORIENTATION_LANDSCAPE) {
            Log.i("aaaaa","ORIENTATION_LANDSCAPE");
            setContentView(R.layout.activity_main_land);
        } else if (getResources().getConfiguration().orientation == Configuration.ORIENTATION_PORTRAIT) {
            Log.i("aaaaa","ORIENTATION_PORTRAIT");
            setContentView(R.layout.activity_main);
        }
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data){
        //Toast.makeText(getApplicationContext(), "Game Closed, returned to menu.", Toast.LENGTH_SHORT).show();
    }


    private class GameLauncher implements View.OnClickListener {

        private final String startCommand;

        public GameLauncher(String startCommand){
            this.startCommand = startCommand;
        }

        @Override
        public void onClick(View view) {
            soundPool.play(soundId,1,1,0,0,1);
            Intent launchIntent = new Intent(getApplicationContext(), UnityPlayerActivity.class);
            if (launchIntent != null) {
                String username = SharedPreferenceUtils.getString(MainActivity.this,"username","Player");
                launchIntent.putExtra("username", username);
                launchIntent.putExtra("startCommand", startCommand);
                launchIntent.putExtra("ipaddr", getIpAddr());
                startActivityForResult(launchIntent, PLAY_GAME);
            } else {
                Toast.makeText(getApplicationContext(), "Could not find game APK", Toast.LENGTH_SHORT).show();
            }
        }
    }


    public String getIpAddr(){
        WifiManager wm = (WifiManager) getApplicationContext().getSystemService(WIFI_SERVICE);
        return Formatter.formatIpAddress(wm.getConnectionInfo().getIpAddress());
    }
}
