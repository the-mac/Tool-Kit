
package us.the.mac.php.client;

import java.io.IOException;
import java.io.UnsupportedEncodingException;

import org.apache.http.HttpResponse;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.ByteArrayEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.params.BasicHttpParams;
import org.apache.http.params.HttpConnectionParams;
import org.apache.http.params.HttpParams;
import org.apache.http.util.EntityUtils;

import android.app.Activity;
import android.os.AsyncTask;
import android.os.Bundle;
import android.text.Html;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;

public class PHPClient extends Activity {

	private final String serverUrl = "MY_PHP_SCRIPT";
	private String postMessage;
	private TextView textView;
	private EditText editText;  
	
    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        
        setContentView(R.layout.main);
        textView = (TextView) findViewById(R.id.textView);
        editText = (EditText) findViewById(R.id.editText);
    }
    
    public void sendJSON(View v) {
    	if("MY_PHP_SCRIPT".equals(serverUrl))
    		throw new IllegalArgumentException("Alert: You need to set up your own PHP Script. See MAC Guide (https://github.com/the-mac/Tool-Kit/wiki/PHP-JSON-API-(Eclipse)-HowTo) on what to replace this value with.");

    	
    	new AsyncTask<Void, Void, String>() {// AsyncTask IS USED FOR NETWORK CONNECTIONS IN ANDROID 3.0+ (HONEYCOMB+)

    		protected void onPreExecute() {
    			super.onPreExecute();
    			postMessage = String.format("{\"message\":\"%s\"}", editText.getText());        			
    		}

    		@Override
    		protected String doInBackground(Void... params) {

    			try {

    	            // SET UP THE CONSTANTS/VARIABLES
    				final int TIMEOUT_MILLISEC = 10000;  // 10 seconds
    				final HttpParams httpParams = new BasicHttpParams();
    				HttpConnectionParams.setConnectionTimeout(httpParams, TIMEOUT_MILLISEC);
    				HttpConnectionParams.setSoTimeout(httpParams, TIMEOUT_MILLISEC);

    	            // SET UP THE REQUEST
    				HttpClient client = new DefaultHttpClient(httpParams);
    				HttpPost request = new HttpPost(serverUrl);
    				request.setEntity(new ByteArrayEntity(postMessage.getBytes("UTF8")));

    	            // SET UP THE RESPONSE
    				HttpResponse response = client.execute(request);
    				return EntityUtils.toString(response.getEntity());
    			}
    			catch (UnsupportedEncodingException e) { e.printStackTrace(); }
    			catch (ClientProtocolException e) { e.printStackTrace(); }
    			catch (IOException e) { e.printStackTrace(); }

    			return null;
    		}

    		protected void onPostExecute(String responseStr) {
    			super.onPostExecute(responseStr);
    			textView.setText(Html.fromHtml(responseStr));
    		}
    	}.execute();
    }
}