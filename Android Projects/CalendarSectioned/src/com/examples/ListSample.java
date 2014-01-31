package com.examples;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.Toast;

public class ListSample extends Activity
{

	public final static String ITEM_TITLE = "title";
	public final static String ITEM_CAPTION = "caption";

	// SectionHeaders
	private final static String[] days = new String[]{"Mon", "Tue", "Wed", "Thur", "Fri"};

	// Section Contents
	private final static String[][] day_events = new String[][]{
		{"Precalculus", "Java Lab"},
		{"Precalculus", "C++ Lab", "Tutoring"},
		{"Sleep all day", "Java Lab"},
		{"Precalculus", "C++ Lab", "Tutoring"},
		{"Precalculus", "Chess/Life", "The MAC"}
	};

	// Adapter for ListView Contents
	private SeparatedListAdapter adapter;

	// ListView Contents
	private ListView journalListView;

	@Override
	public void onCreate(Bundle icicle)
	{
		super.onCreate(icicle);

		// Sets the View Layer
		setContentView(R.layout.main);

		// Create the ListView Adapter
		adapter = new SeparatedListAdapter(this);

		// Add Sections
		for (int i = 0; i < days.length; i++)
		{
			// WHERE THE NEW SECTION ADAPTER WILL BE ADDED as day_events
			ArrayAdapter<String> listadapter = new ArrayAdapter<String>(this, R.layout.list_item, day_events[i]);
			adapter.addSection(days[i], listadapter);
		}

		// Get a reference to the ListView holder
		journalListView = (ListView) this.findViewById(R.id.list_journal);

		// Set the adapter on the ListView holder
		journalListView.setAdapter(adapter);

		// Listen for Click events
		journalListView.setOnItemClickListener(new OnItemClickListener()
		{
			@Override
			public void onItemClick(AdapterView<?> parent, View view, int position, long duration)
			{
				String item = (String) adapter.getItem(position);
				Toast.makeText(getApplicationContext(), item, Toast.LENGTH_SHORT).show();
			}
		});
	}
}
