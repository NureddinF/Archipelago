package com.example.xuhongcheng.archipelago.activitys;

import android.app.Activity;
import android.media.AudioManager;
import android.media.SoundPool;
import android.os.Bundle;
import android.view.View;
import android.widget.EditText;
import android.widget.Toast;

import com.example.xuhongcheng.archipelago.myapplication.R;

import java.io.File;
import java.io.FileOutputStream;




public class RegisterActivity extends Activity {

	EditText et_name;
	EditText et_pass;
	EditText et_email;
	private SoundPool soundPool;
	private int  soundId;


	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setTitle("Set Password");
		setContentView(R.layout.activity_register);
		et_name = (EditText) findViewById(R.id.et_name);
		et_pass = (EditText) findViewById(R.id.et_pass);
		et_email = (EditText) findViewById(R.id.et_email);

		soundPool = new SoundPool(10, AudioManager.STREAM_SYSTEM, 5);
		soundId = soundPool.load(this,R.raw.doink,1);
	}


	public void setAccount(View v) {
		soundPool.play(soundId,1,1,0,0,1);
		//Store username and password
		//1Get the username and password
		String name = et_name.getText().toString().trim();
		String pass = et_pass.getText().toString().trim();
		String email = et_email.getText().toString().trim();


		if (name.isEmpty()) {
			Toast.makeText(this, "Empty Username!", Toast.LENGTH_SHORT).show();
			return;
		} else if (pass.isEmpty()) {
			Toast.makeText(this, "Empty Password!", Toast.LENGTH_SHORT).show();
			return;
		} else if (email.isEmpty()) {
			Toast.makeText(this, "Empty Email!", Toast.LENGTH_SHORT).show();
			return;
		}
		String passRegex = "^(?:(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])).{6,12}$";
		String emailRegex = "^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\\.[a-zA-Z0-9_-]+)+$";
//		if(name.matches("^[a-zA-Z]\\w*$")){
//			Toast.makeText(this, "Invalid Username！!", Toast.LENGTH_SHORT).show();
//			return;
//		}
		if (!pass.matches(passRegex)) {
			Toast.makeText(this, "Invalid Password！!", Toast.LENGTH_SHORT).show();
			return;
		}
		if (!email.matches(emailRegex)) {
			Toast.makeText(this, "Invalid Email！!", Toast.LENGTH_SHORT).show();
			return;
		}

		//2Write the username and password to the file

		File file = new File(getCacheDir(), "/info.txt");
		try {
			FileOutputStream fos = new FileOutputStream(file);
			fos.write((name + "##" + pass + "##"+email).getBytes());
			fos.close();
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		Toast.makeText(this, "Register Finished！", Toast.LENGTH_SHORT).show();
		finish();
	}

}
