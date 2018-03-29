package com.example.xuhongcheng.archipelago.activitys;

/**
 * Created by xuhongcheng on 2018-03-29.
 */

import android.app.Service;
import android.content.Intent;
import android.media.MediaPlayer;
import android.os.IBinder;

import com.example.xuhongcheng.archipelago.myapplication.R;

public class MusicService extends Service {
    private MediaPlayer mediaPlayer;

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    @Override
    public void onStart(Intent intent, int startId) {
        super.onStart(intent, startId);
        if (mediaPlayer == null) {
            mediaPlayer = MediaPlayer.create(this, R.raw.bgm_game);
            mediaPlayer.setLooping(true);
            mediaPlayer.start();
        }

    }

    @Override
    public void onDestroy() {
        // TODO Auto-generated method stub
        super.onDestroy();
        mediaPlayer.stop();
        mediaPlayer.release();
    }
}
