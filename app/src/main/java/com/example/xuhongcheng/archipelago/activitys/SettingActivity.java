package com.example.xuhongcheng.archipelago.activitys;

import android.app.Activity;
import android.content.Context;
import android.media.AudioManager;
import android.os.Bundle;
import android.support.annotation.IdRes;
import android.support.annotation.Nullable;
import android.widget.RadioButton;
import android.widget.RadioGroup;

import com.example.xuhongcheng.archipelago.myapplication.R;

//http://blog.csdn.net/fund2010/article/details/49622497
public class SettingActivity extends Activity {
    private RadioGroup mOptions;
    private RadioButton mTurnOn;
    private RadioButton mTurnOff;
    private AudioManager audioManager;
    private int mediaVolume;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_settings);
        audioManager = (AudioManager) getApplication()
                .getSystemService(Context.AUDIO_SERVICE);
//        int maxVolume = audioManager.getStreamMaxVolume(AudioManager.STREAM_MUSIC);
//        audioManager.setStreamVolume(AudioManager.STREAM_MUSIC, 0, 0);
        initView();
    }

    private void initView() {
//        Initialize the object of button
        mOptions = (RadioGroup) findViewById(R.id.rg_options);
        mTurnOn = (RadioButton) findViewById(R.id.rb_turn_on);
        mTurnOff = (RadioButton) findViewById(R.id.rb_turn_off);
        mOptions.setOnCheckedChangeListener(new RadioGroup.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(RadioGroup radioGroup, @IdRes int i) {
                if (mTurnOn.getId() == i) {
//                    Toast.makeText(SettingActivity.this, "Turn On Sounds", Toast.LENGTH_SHORT).show();
                    audioManager.setStreamVolume(AudioManager.STREAM_MUSIC, mediaVolume, 0);
                } else if (mTurnOff.getId() == i) {
                    mediaVolume = audioManager.getStreamVolume(AudioManager.STREAM_MUSIC);
//                    Toast.makeText(SettingActivity.this, "Turn Off Sounds", Toast.LENGTH_SHORT).show();
                    audioManager.setStreamVolume(AudioManager.STREAM_MUSIC, 0, 0);
                }
            }
        });
    }
}
