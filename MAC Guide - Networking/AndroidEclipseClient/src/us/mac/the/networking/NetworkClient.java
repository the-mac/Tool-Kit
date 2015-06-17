package us.mac.the.networking;

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.InetAddress;
import java.net.Socket;

import android.app.Activity;
import android.content.Context;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.os.AsyncTask;
import android.os.Bundle;
import android.text.format.Formatter;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;
import android.widget.ToggleButton;

public class NetworkClient extends Activity {

    private ToggleButton connect;
    private TextView text;
    private EditText ipBox;
    private EditText msgBox;
    private Button send;

    private Socket client;
    private String ipAddress;
    private String ipSentence;
    private DataOutputStream outToServer;
    private BufferedReader inFromServer;

    private final int START = 0xffffff;
    private final int msgBoxHint = START;
    private final int appendText = START + 1;


    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setUpAllViews(R.layout.main);
    }

    private void setUpAllViews(int layout)
    {
        setContentView(layout);
        ipAddress = getIPAddress();

        text = (TextView) findViewById(R.id.text);
        send = (Button) findViewById(R.id.send);
        connect = (ToggleButton) findViewById(R.id.connect);
        ipBox = (EditText) findViewById(R.id.ipBox);
        msgBox = (EditText) findViewById(R.id.msgBox);

        setText(R.id.ipBox, ipAddress);
        setText(R.id.text,"To start the client press the connect button");
    }

    private void setText(int view, String content)
    {
        switch(view)
        {
            case R.id.ipBox: ipBox.setText(content); break;
            case R.id.msgBox: msgBox.setText(content); break;
            case R.id.text: text.setText(content + "\n\n"); break;
            case appendText: text.append(content + "\n\n"); break;
            case msgBoxHint: msgBox.setHint(content); break;
        }
    }

    private void setValues(int view, boolean value)
    {
        switch(view)
        {
            case R.id.ipBox: ipBox.setEnabled(value); break;
            case R.id.msgBox: msgBox.setEnabled(value); break;
            case R.id.send: send.setEnabled(value); break;
            case R.id.connect: connect.setChecked(value); break;
        }
    }

    private String getIPAddress() {

        WifiManager wifi = (WifiManager) getSystemService(Context.WIFI_SERVICE);

        // Get WiFi status
        WifiInfo info = wifi.getConnectionInfo();

        return Formatter.formatIpAddress(info.getIpAddress());
    }

    private void setUpIOStreams() throws Exception
    {
    	new AsyncTask<Void, Void, Object>() {
    		@Override
    		protected Void doInBackground(Void... params) {
    			try {
    				InetAddress addr = InetAddress.getByName(
    						ipBox.getText().toString());

    				client = new Socket(addr, 8888);

    				outToServer = new DataOutputStream(
    						client.getOutputStream());

    				inFromServer = new BufferedReader(
    						new InputStreamReader(
    								client.getInputStream()));
    			}
    			catch(Exception e) {}
    			return null;
    		}
    	}.execute().get();
    }

    private void enableConnection()
    {
        try {
            setUpIOStreams();
            String ip = client.getInetAddress().toString();

            setText(R.id.text,"Device's IP Address: " + ipAddress);
            setText(appendText,"Server's IP Address: " + ip.substring(1));
            setText(msgBoxHint, "Say something...");
            setText(appendText, "Enter your message then press the send button");

            setValues(R.id.connect,true);
            setValues(R.id.send,true);
            setValues(R.id.ipBox,false);
            setValues(R.id.msgBox,true);
        } catch (Exception e) {

            setValues(R.id.connect,false);
            Toast.makeText(this, e.toString(), Toast.LENGTH_LONG).show();
        }

    }

    private void disableConnection() throws Exception
    {
        if(client != null)
        {

        	new AsyncTask<Void, Void, Object>() {
        		@Override
        		protected Void doInBackground(Void... params) {

                    try {
                        client.close();
                    } catch (final IOException e) {
                    	runOnUiThread(new Runnable() {
							@Override public void run() {
								Toast.makeText(NetworkClient.this, e.toString(), Toast.LENGTH_LONG).show();
							}
						});
                    }
        			return null;
        		}
        	}.execute().get();
        	
            setText(R.id.text,"Press the connect button to start the client");
            setText(R.id.msgBox,"");
            setText(msgBoxHint,"");

            setValues(R.id.connect,false);
            setValues(R.id.ipBox,true);
            setValues(R.id.msgBox,false);
            setValues(R.id.send,false);
        }
        else
        {
            setValues(R.id.connect,false);
        }
    }

    private void sendDataOverConnection() throws Exception
    {
        ipSentence = msgBox.getText().toString() + "\n";
        setText(R.id.msgBox, "");
        try {

        	new AsyncTask<Void, Void, Object>() {
        		@Override
        		protected Void doInBackground(Void... params) {

                    try {
                        if(client.isClosed()) setUpIOStreams();

                        outToServer.writeBytes(ipSentence);
                        String modifiedSentence = inFromServer.readLine();

                        ipSentence = ("OUT TO SERVER: " + ipSentence);
                        ipSentence += "\n" +("IN FROM SERVER: " + modifiedSentence);
                        client.close();
                    } catch (final Exception e) {
                    	runOnUiThread(new Runnable() {
							@Override public void run() {
								Toast.makeText(NetworkClient.this, e.toString(), Toast.LENGTH_LONG).show();
							}
						});
                    }
                    
        			return null;
        		}
        	}.execute().get();
        } catch (Exception e) {
            Toast.makeText(this, e.toString(), Toast.LENGTH_LONG).show();

            setValues(R.id.ipBox,true);
            setValues(R.id.connect,false);
            setValues(R.id.send,false);
            setValues(R.id.msgBox,false);
        }

        setText(appendText, ipSentence);
    }

    public void onClick(View v)
    {
    	try {
			
    		switch(v.getId())
    		{
    		case R.id.connect:
    			if(connect.isChecked())
    				enableConnection();
    			else
    				disableConnection();
    			break;
    		case R.id.send:
    			sendDataOverConnection();
    			break;
    		}
		} catch (Exception e) {
			Toast.makeText(this, e.toString(), Toast.LENGTH_LONG).show();
		}
    }

}
