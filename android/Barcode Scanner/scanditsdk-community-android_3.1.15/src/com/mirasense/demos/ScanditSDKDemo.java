package com.mirasense.demos;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.location.Criteria;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.view.Display;
import android.view.Gravity;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup.LayoutParams;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.mirasense.scanditsdk.ScanditSDKBarcodePicker;
import com.mirasense.scanditsdk.interfaces.ScanditSDKListener;
import com.mirasense.scanditsdkdemo.R;

/**
 * Simple demo application illustrating the use of the Scandit SDK.
 * 
 * Important information for the developer with respect to Android 2.1 support!
 * 
 * Android 2.1 differs from subsequent versions of Android OS in that it 
 * does not offer a camera preview mode in portrait mode (landscape only). 
 * Android 2.2+ offers both - a camera preview in landscape mode and in portrait mode. 
 * 
 * To address this difference between the Android versions, the Scandit SDK 
 * offers the following approaches and the developer needs to choose his preferred option:
 * 
 * under Android 2.1:
 * 
 * - a scan view in landscape mode scanning only(!) that is fully 
 * customizable by the developer - ScanditSDKBarcodePicker.class
 *  
 *  or
 *  
 * - our own custom scan view with portrait mode scanning that offers only 
 * limited customization options (show/hide title & tool bars, 
 * but no additional Android UI elements) -  LegacyPortraitScanditSDKBarcodePicker.class
 * 
 * under Android 2.2: 
 * 
 * - a scan view with portrait mode scanning that is fully customizable 
 * by the developer (RECOMMENDED) - ScanditSDKBarcodePicker.class
 * 
 * or 
 * 
 * - any of the options listed under Android 2.1
 * 
 * We recommend that developers choose the scan view in portrait mode on Android 2.2.
 * It has the native Android look&feel and provides full customization. We provide our
 * own custom scan view (LegacyPortraitScanditSDKBarcodePicker.class) in Android 2.2
 * to provide backwards compatibility with Android 2.1.
 * 
 * For more details on how to implement your preferred option, see the ScanditSDKSampleBarcodeActivity 
 * class.
 * 
 * This ScanditSDKDemo uses the ScanditSDKSampleBarcodeActivity but also illustrates three different 
 * ways of integrating the Scandit SDK:
 * 
 *  - a scan view that uses the entire screen (via its own activity)
 *  
 *  - a scan view that is reduced in size (Android 2.2 and higher only)
 *  
 *  - a scan view that is cropped and moved to the top of the screen (Android 2.2 and higher only)
 * 
 * The demo app also illustrates how to integrate the Scandit SDK:
 * 
 * 1. Create a ScanditSDKBarcodePicker object that manages camera access and 
 *    bar code scanning:
 *    ScanditSDKBarcodePicker barcodePicker = new ScanditSDKBarcodePicker(this, 
 *              R.raw.class, "your app key", true, 
                ScanditSDKBarcodePicker.LOCATION_PROVIDED_BY_SCANDIT_SDK);
 *
 * 2. Add it to the activity:    
 *    my_activity.setContentView(barcodePicker);
 * 
 * 3. Implement the ScanditSDKListener interface (didCancel, didScanBarcode, 
 *    didManualSearch) and register with the ScanditSDKOverlayView to receive 
 *    callbacks:
 *    barcodePicker.getOverlayView().addListener(this);
 * 

 * 
 * 
 * Copyright 2010 Mirasense AG
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied
 * See the License for the specific language governing premissions and
 * limitations under the License.
 */
public class ScanditSDKDemo extends Activity implements ScanditSDKListener, 
                                                        LocationListener {

	// The main object for scanning barcodes.
	private ScanditSDKBarcodePicker mBarcodePicker;
	
	// Enter your Scandit SDK App key here.
	// Your Scandit SDK App key is available via your Scandit SDK web account.
	private static final String sScanditSdkAppKey = "";
	
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

        startLocationGathering();

		RelativeLayout rootView = new RelativeLayout(this);
		rootView.setBackgroundResource(R.drawable.splash);
		
		// Create the buttons to start scanner examples.
		Button activityButton = createActivityButton(rootView);
        Button scaledButton = createScaledOverlayButton(rootView, activityButton);
        createCroppedOverlayButton(rootView, scaledButton);
        
		setContentView(rootView);
	}

    /**
     * Creates a button that shows how to start a new Activity that implements 
     * the Scandit SDK as a full screen scanner. The Activity can be found in 
     * the ScanditSDKSampleBarcodeActivity in this demo project. The old and
     * new GUIs can both be easily opened this way, which is also shown in the 
     * aforementioned activity.
     * 
     * @param rootView The view this button should be added to.
     * @return Button that starts a scanning Activity on click.
     */
	private Button createActivityButton(RelativeLayout rootView) {
        Button button = new Button(this);
        button.setText("Full Screen Scan View");
        RelativeLayout.LayoutParams rParams = new RelativeLayout.LayoutParams(
                LayoutParams.WRAP_CONTENT, LayoutParams.WRAP_CONTENT);
        rParams.addRule(RelativeLayout.CENTER_HORIZONTAL);
        rParams.addRule(RelativeLayout.ALIGN_PARENT_BOTTOM);
        rParams.bottomMargin = 100;
        rootView.addView(button, rParams);
        button.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                startActivity(new Intent(ScanditSDKDemo.this, 
                        ScanditSDKSampleBarcodeActivity.class));
            }
        });
        button.setId(1234);
        return button;
	}

    /**
     * Creates a button that shows how to add a view that is a scaled down 
     * version of the full screen Scandit SDK scanner. To only see part of the
     * scanner, check out the third example.
     * 
     * @param rootView The view this button should be added to.
     * @param anchorView Reference point for placing the button in the rootView.
     * @return Button that places a scanner on rootView.
     */
	private Button createScaledOverlayButton(final RelativeLayout rootView, 
	        View anchorView) {
        Button button = new Button(this);
        button.setText("Scaled Scan View");
        RelativeLayout.LayoutParams rParams = new RelativeLayout.LayoutParams(
                LayoutParams.WRAP_CONTENT, LayoutParams.WRAP_CONTENT);
        rParams.addRule(RelativeLayout.CENTER_HORIZONTAL);
        rParams.addRule(RelativeLayout.ABOVE, anchorView.getId());
        rParams.bottomMargin = 10;
        rootView.addView(button, rParams);
        button.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                RelativeLayout.LayoutParams rParams;
                
                RelativeLayout r = new RelativeLayout(ScanditSDKDemo.this);
                rParams = new RelativeLayout.LayoutParams(
                        LayoutParams.FILL_PARENT, LayoutParams.FILL_PARENT);
                r.setBackgroundColor(0x00000000);
                rootView.addView(r, rParams);
                r.setOnClickListener(new OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        rootView.removeView(v);
                        rootView.removeView(mBarcodePicker);
                        mBarcodePicker.stopScanning();
                        mBarcodePicker = null;
                    }
                });
                
                mBarcodePicker = new ScanditSDKBarcodePicker(
                        ScanditSDKDemo.this, R.raw.class, sScanditSdkAppKey);

                // Register listener, in order to be notified about relevant events 
                // (e.g. a recognized bar code).
                mBarcodePicker.getOverlayView().addListener(ScanditSDKDemo.this);

                WindowManager wm = (WindowManager) getSystemService(Context.WINDOW_SERVICE);
                Display display = wm.getDefaultDisplay();
                
                rParams = new RelativeLayout.LayoutParams(
                        display.getWidth() * 3 / 4, display.getHeight() * 3 / 4);
                rParams.addRule(RelativeLayout.CENTER_HORIZONTAL);
                rParams.bottomMargin = 20;
                rootView.addView(mBarcodePicker, rParams);
                mBarcodePicker.startScanning();
            }
        });
        button.setId(1235);
        
        // Must be able to run the portrait version for this button to work.
        if (!ScanditSDKBarcodePicker.canRunPortraitPicker()) {
            button.setEnabled(false);
        }
        
        return button;
	}
	
	/**
	 * Creates a button that shows how to add a cropped version of the full
	 * screen Scandit SDK scanner. The cropping is accomplished by overlaying
	 * parts of the scanner with an opaque view.
     * 
     * @param rootView The view this button should be added to.
     * @param anchorView Reference point for placing the button in the rootView.
     * @return Button that places a scanner on rootView.
	 */
	private Button createCroppedOverlayButton(final RelativeLayout rootView, 
	        View anchorView) {
        Button button = new Button(this);
        button.setText("Cropped Scan View");
        RelativeLayout.LayoutParams rParams = new RelativeLayout.LayoutParams(
                LayoutParams.WRAP_CONTENT, LayoutParams.WRAP_CONTENT);
        rParams.addRule(RelativeLayout.CENTER_HORIZONTAL);
        rParams.addRule(RelativeLayout.ABOVE, anchorView.getId());
        rParams.bottomMargin = 10;
        rootView.addView(button, rParams);
        button.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                RelativeLayout.LayoutParams rParams;
                
                mBarcodePicker = new ScanditSDKBarcodePicker(
                        ScanditSDKDemo.this, R.raw.class, sScanditSdkAppKey);

                // Register listener, in order to be notified about relevant events 
                // (e.g. a recognized bar code).
                mBarcodePicker.getOverlayView().addListener(ScanditSDKDemo.this);
                
                rParams = new RelativeLayout.LayoutParams(
                        LayoutParams.FILL_PARENT, LayoutParams.FILL_PARENT);
                rParams.addRule(RelativeLayout.CENTER_HORIZONTAL);
                rParams.bottomMargin = 20;
                rootView.addView(mBarcodePicker, rParams);


                WindowManager wm = (WindowManager) getSystemService(Context.WINDOW_SERVICE);
                Display display = wm.getDefaultDisplay();
                
                TextView overlay = new TextView(ScanditSDKDemo.this);
                rParams = new RelativeLayout.LayoutParams(
                        LayoutParams.FILL_PARENT, display.getHeight() / 2);
                rParams.addRule(RelativeLayout.ALIGN_PARENT_BOTTOM);
                overlay.setBackgroundColor(0xFF000000);
                rootView.addView(overlay, rParams);
                overlay.setText("Touch to close");
                overlay.setGravity(Gravity.CENTER);
                overlay.setTextColor(0xFFFFFFFF);
                overlay.setOnClickListener(new OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        rootView.removeView(v);
                        rootView.removeView(mBarcodePicker);
                        mBarcodePicker.stopScanning();
                        mBarcodePicker.setScanningHotSpot(0.5f, 0.5f);
                        mBarcodePicker = null;
                    }
                });
                mBarcodePicker.setScanningHotSpot(0.5f, 0.25f);
                mBarcodePicker.getOverlayView().setInfoBannerY(0.4f);
                mBarcodePicker.startScanning();
            }
        });
        // Must be able to run the portrait version for this button to work.
        if (!ScanditSDKBarcodePicker.canRunPortraitPicker()) {
            button.setEnabled(false);
        }
        
        return button;
	}
	
	@Override
	protected void onPause() {
	    // When the activity is in the background immediately stop the 
	    // scanning to save resources and free the camera.
	    if (mBarcodePicker != null) {
	        mBarcodePicker.stopScanning();
	    }
        LocationManager locationManager = 
            (LocationManager) getSystemService(Context.LOCATION_SERVICE);
        locationManager.removeUpdates(this);
	    super.onPause();
	}
	
	@Override
	protected void onResume() {
	    // Once the activity is in the foreground again, restart scanning.
        if (mBarcodePicker != null) {
            mBarcodePicker.startScanning();
        }
	    super.onResume();
	}

	/** 
     *  Called when a barcode has been decoded successfully.
     *  
     *  @param barcode Scanned barcode content.
     *  @param symbology Scanned barcode symbology.
     */
	public void didScanBarcode(String barcode, String symbology) {
		// Example code that would typically be used in a real-world app using 
		// the Scandit SDK.
		/*
		// Access the image in which the bar code has been recognized.
		byte[] imageDataNV21Encoded = barcodePicker.getCameraPreviewImageOfFirstBarcodeRecognition();
		int imageWidth = barcodePicker.getCameraPreviewImageWidth();
		int imageHeight = barcodePicker.getCameraPreviewImageHeight();
		
		// Stop recognition to save resources.
		mBarcodePicker.stopScanning();
		*/
	}
	
	/** 
     * Called when the user entered a bar code manually.
     * 
     * @param entry The information entered by the user.
     */
	public void didManualSearch(String entry) {
		// Example code that would typically be used in a real-world app using 
	    // the Scandit SDK.
		/*
		// Access the current camera image.
		byte[] imageDataNV21Encoded = barcodePicker.getMostRecentCameraPreviewImage();
		int imageWidth = barcodePicker.getCameraPreviewImageWidth();
		int imageHeight = barcodePicker.getCameraPreviewImageHeight(); 
		 
		// Stop recognition to save resources.
		mBarcodePicker.stopScanning();
		*/
	}
	
	@Override
	public void didCancel() {
	}
	
	@Override
	public void onBackPressed() {
	    if (mBarcodePicker != null) {
	        mBarcodePicker.stopScanning();
	    }
	    finish();
	}
	
	/**
	 * Starts to gather location data from the finest allowed location provider.
	 * For this to work either one of the following permissions has to be set:
	 * ACCESS_COARSE_LOCATION, ACCESS_FINE_LOCATION.
	 */
	private void startLocationGathering() {
        LocationManager locationManager = 
            (LocationManager) getSystemService(Context.LOCATION_SERVICE);

        Criteria criteria = new Criteria();
        criteria.setAccuracy(Criteria.ACCURACY_FINE);
        String provider = locationManager.getBestProvider(criteria, true);
        if (provider != null) {
            locationManager.requestLocationUpdates(provider, 5, 5, this);
        }
	}
	
	@Override
	public void onLocationChanged(Location arg0) {
        LocationManager locationManager = 
            (LocationManager) getSystemService(Context.LOCATION_SERVICE);
        locationManager.removeUpdates(this);
	}
	
	@Override
	public void onProviderDisabled(String arg0) {}
	
	@Override
	public void onProviderEnabled(String arg0) {}
	
	public void onStatusChanged(String arg0, int arg1, Bundle arg2) {};
}
