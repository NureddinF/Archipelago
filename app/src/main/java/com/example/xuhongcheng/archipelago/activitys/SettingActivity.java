package com.example.xuhongcheng.archipelago.activitys;

import android.app.Activity;
import android.content.Context;
import android.media.AudioManager;
import android.os.Bundle;
import android.support.annotation.IdRes;
import android.support.annotation.Nullable;
import android.widget.Button;
import android.widget.RadioButton;
import android.widget.RadioGroup;
import android.widget.Toast;

import com.example.xuhongcheng.archipelago.R;


public class SettingActivity extends Activity {
    private RadioGroup mOptions;
    private RadioButton mTurnOn;
    private RadioButton mTurnOff;
    private Button mOkBt;
    private AudioManager audioManager;
    private int mediaVolume1;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_settings);
        audioManager = (AudioManager) getApplication()
                .getSystemService(Context.AUDIO_SERVICE);
        mediaVolume1 = audioManager.getStreamVolume(AudioManager.STREAM_MUSIC);
//        int maxVolume = audioManager.getStreamMaxVolume(AudioManager.STREAM_MUSIC);
//        audioManager.setStreamVolume(AudioManager.STREAM_MUSIC, 0, 0);
        initView();
    }

    private void initView() {
//        初始化按钮对象
        mOptions = (RadioGroup) findViewById(R.id.rg_options);
        mTurnOn = (RadioButton) findViewById(R.id.rb_turn_on);
        mTurnOff = (RadioButton) findViewById(R.id.rb_turn_off);
        mOptions.setOnCheckedChangeListener(new RadioGroup.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(RadioGroup radioGroup, @IdRes int i) {
                int mediaVolume = audioManager.getStreamVolume(AudioManager.STREAM_MUSIC);
                if (mTurnOn.getId() == i) {
                    Toast.makeText(SettingActivity.this, "Turn On Sounds", Toast.LENGTH_SHORT).show();
                    audioManager.setStreamVolume(AudioManager.STREAM_MUSIC, mediaVolume, 0);
                } else if (mTurnOff.getId() == i) {
                    mediaVolume = audioManager.getStreamVolume(AudioManager.STREAM_MUSIC);
                    Toast.makeText(SettingActivity.this, "Turn Off Sounds", Toast.LENGTH_SHORT).show();
                    audioManager.setStreamVolume(AudioManager.STREAM_MUSIC, 0, 0);
                }
            }
        });
    }
}
