package com.example.first;

import android.app.Activity;
import android.content.Context;
import android.content.res.Resources;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.CompoundButton.OnCheckedChangeListener;
import android.widget.ImageButton;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

public class MainActivity extends Activity {
	private String[] planets;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);


		//resources grabbed from the application ((THE WHOLE PRJECT))
		Resources res = getResources();
		//strings grabbed from the resources ((THE WHOLE PRJECT))
		planets = res.getStringArray(R.array.string_array_name);

		//listview grabbed from the activity
		ListView lv = (ListView) findViewById(R.id.listView1);

		//assigned array to the listview
		lv.setAdapter(new Adapter<String>(this, android.R.layout.simple_list_item_1, planets));
	}


	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.main, menu);
		return true;
	}
	private static class AccessoriesViewHolder {
		public CheckBox star;
		public TextView content;
		public TextView content_;
	}

	private class Adapter<T> extends ArrayAdapter<T> {

		public Adapter(Context context, int textViewResourceId, T[] objects) {
			super(context, textViewResourceId, objects);
		}

		@Override
		public int getCount() {
			return planets.length;
		}

		@Override
		public T getItem(int position) {
			return (T)planets[position];
		}

		@Override
		public long getItemId(int position) {
			return position;
		}

		@Override
		public View getView(int position, View convertView, ViewGroup parent) {

			AccessoriesViewHolder holder = null;

			if (convertView == null) {
				LayoutInflater li = getLayoutInflater();
				//            	Log.d("MainActivity.getView", "convertView == null");
				convertView = li.inflate(R.layout.row_result, parent, false);

				holder = new AccessoriesViewHolder();
				holder.star = (CheckBox) convertView.findViewById(R.id.btn_star);
				holder.star.setOnCheckedChangeListener(mStarCheckedChanceChangeListener);
				holder.content = (TextView) convertView.findViewById(R.id.content);
				holder.content_ = (TextView) convertView.findViewById(R.id.content_);

				((ImageButton) convertView.findViewById(R.id.imageButton2)).setOnClickListener(mBuyButtonClickListener);

				convertView.setTag(holder);
			} else {
				holder = (AccessoriesViewHolder) convertView.getTag();
			}

			//            holder.star.setChecked(false);
			holder.content.setText(planets[position]);
			holder.content_.setText(planets[position]);

			return convertView;
		}
	}private void showMessage(String message) {
		Toast.makeText(this, message, Toast.LENGTH_SHORT).show();
	}

	private OnClickListener mBuyButtonClickListener = new OnClickListener() {
		@Override
		public void onClick(View v) 
		{
			showMessage("Say something from button.");
		}
	};

	private OnCheckedChangeListener mStarCheckedChanceChangeListener = new OnCheckedChangeListener() {
		@Override
		public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) 
		{
			showMessage("Say something from checked.");
		}


	};
}
