package com.example.xuhongcheng.archipelago.activitys;

import android.app.Activity;
import android.content.Context;
import android.media.AudioManager;
import android.media.SoundPool;
import android.os.Bundle;
import android.support.annotation.IdRes;
import android.support.annotation.Nullable;
import android.widget.Button;
import android.widget.RadioButton;
import android.widget.RadioGroup;

import com.example.xuhongcheng.archipelago.myapplication.R;


public class SettingActivity extends Activity {
    private RadioGroup mOptions;
    private RadioButton mTurnOn;
    private RadioButton mTurnOff;
    private Button mOkBt;
    private AudioManager audioManager;
    private int mediaVolume1;
    private SoundPool soundPool;
    private int soundId;


    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_settings);
        audioManager = (AudioManager) getApplication()
                .getSystemService(Context.AUDIO_SERVICE);
        initView();
//        int maxVolume = audioManager.getStreamMaxVolume(AudioManager.STREAM_MUSIC);
//        audioManager.setStreamVolume(AudioManager.STREAM_MUSIC, 0, 0);
        soundPool = new SoundPool(10, AudioManager.STREAM_SYSTEM, 5);
        soundId = soundPool.load(this, R.raw.doink, 1);
    }

    @Override
    protected void onResume() {
        super.onResume();
        mediaVolume1 = audioManager.getStreamVolume(AudioManager.STREAM_MUSIC);
        if (mediaVolume1 > 0) {
            mTurnOn.setChecked(true);
        } else {
            mTurnOff.setChecked(true);
        }
    }

    private void initView() {
//        Initialize the object of button
        mOptions = (RadioGroup) findViewById(R.id.rg_options);
        mTurnOn = (RadioButton) findViewById(R.id.rb_turn_on);
        mTurnOff = (RadioButton) findViewById(R.id.rb_turn_off);
        mOptions.setOnCheckedChangeListener(new RadioGroup.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(RadioGroup radioGroup, @IdRes int i) {
                soundPool.play(soundId, 1, 1, 0, 0, 1);
                if (mTurnOn.getId() == i) {
//                    Toast.makeText(SettingActivity.this, "Turn On Sounds", Toast.LENGTH_SHORT).show();
                    audioManager.setStreamVolume(AudioManager.STREAM_MUSIC, mediaVolume1, 0);
                    audioManager.setStreamVolume(AudioManager.STREAM_SYSTEM, mediaVolume1, 0);
                } else if (mTurnOff.getId() == i) {
                    mediaVolume1 = audioManager.getStreamVolume(AudioManager.STREAM_MUSIC);
//                    Toast.makeText(SettingActivity.this, "Turn Off Sounds", Toast.LENGTH_SHORT).show();
                    audioManager.setStreamVolume(AudioManager.STREAM_MUSIC, 0, 0);
                    audioManager.setStreamVolume(AudioManager.STREAM_SYSTEM, 0, 0);
                }
            }
        });
    }
}
