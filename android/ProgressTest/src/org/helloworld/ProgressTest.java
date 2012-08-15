package org.helloworld;

import android.app.Activity;
import android.os.Bundle;
import android.view.Window;
import android.webkit.WebChromeClient;
import android.webkit.WebView;

public class ProgressTest extends Activity {
	
	final Activity context = this;  
	
    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
    	super.onCreate(savedInstanceState);  
    	   requestWindowFeature(Window.FEATURE_PROGRESS);//让进度条显示在标题栏上  
    	   setContentView(R.layout.main);  
    	  
    	   WebView webview = (WebView)findViewById(R.id.webview);  
    	   webview.setWebChromeClient(new WebChromeClient() {  
    	              public void onProgressChanged(WebView view, int progress) {  
    	                //Activity和Webview根据加载程度决定进度条的进度大小  
    	               //当加载到100%的时候 进度条自动消失  
    	                context.setProgress(progress * 100);  
    	       }  
    	    });  
    	   String url="http://www.google.com";
		webview.loadUrl(url);  
    }
}