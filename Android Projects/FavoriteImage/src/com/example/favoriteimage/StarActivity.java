package com.example.favoriteimage;

import android.os.Bundle;
import android.app.Activity;
import android.view.Menu;

public class StarActivity extends Activity {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.star);
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.star, menu);
		return true;
	}

}
