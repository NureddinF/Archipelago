package com.example.xuhongcheng.archipelago.activitys;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.widget.EditText;
import android.widget.Toast;

import com.example.xuhongcheng.archipelago.R;

import java.io.File;
import java.io.FileOutputStream;

public class RegisterActivity extends Activity {

	EditText et_name;
	EditText et_pass;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
       setTitle("设置密码");
        setContentView(R.layout.activity_register);
        et_name = (EditText) findViewById(R.id.et_name);
    	et_pass = (EditText) findViewById(R.id.et_pass);
    }


    public void setAccount(View v){
    		//保存账号密码
    		//1拿到账号和密码
    		String name = et_name.getText().toString().trim();
    		String pass = et_pass.getText().toString().trim();

			if(name.isEmpty()){
				Toast.makeText(this, "Empty Username!", Toast.LENGTH_SHORT).show();
				return ;
			}else if (pass.isEmpty()){
				Toast.makeText(this, "Empty Password!", Toast.LENGTH_SHORT).show();
				return ;
			}
    		//2把账号密码写入文件
    		//返回一个file对象，路径就是data/data/com.wuzhoudao.note/files
//    		File file = new File(getFilesDir(), "/info.txt");
    		//返回一个file对象，路径就是data/data/com.itheima.apirwinrom/cache
    		File file = new File(getCacheDir(), "/info.txt");
    		try {
				FileOutputStream fos = new FileOutputStream(file);
				fos.write((name + "##" + pass).getBytes());
				fos.close();
			} catch (Exception e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
    		Toast.makeText(this, "Register Finished！", Toast.LENGTH_SHORT).show();
			finish();
    	}
    
}
