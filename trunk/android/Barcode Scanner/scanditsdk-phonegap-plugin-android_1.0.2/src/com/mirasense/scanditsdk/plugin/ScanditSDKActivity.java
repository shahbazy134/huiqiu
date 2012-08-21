package com.mirasense.scanditsdk.plugin;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.os.Bundle;
import android.view.Window;
import android.view.WindowManager;

import com.mirasense.cordova.R;
import com.mirasense.scanditsdk.LegacyPortraitScanditSDKBarcodePicker;
import com.mirasense.scanditsdk.ScanditSDKBarcodePicker;
import com.mirasense.scanditsdk.interfaces.ScanditSDK;
import com.mirasense.scanditsdk.interfaces.ScanditSDKListener;

/**
 * Activity integrating the barcode scanner.
 * 
 * Copyright 2011 Mirasense AG. All rights reserved.
 */
public class ScanditSDKActivity extends Activity implements ScanditSDKListener {
    
    public static final int CANCEL = 0;
    public static final int SCAN = 1;
    public static final int MANUAL = 2;
    
    private ScanditSDK mBarcodePicker;
    
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        initializeAndStartBarcodeRecognition(getIntent().getExtras());
        super.onCreate(savedInstanceState);
    }
    
    @SuppressWarnings("deprecation")
    public void initializeAndStartBarcodeRecognition(Bundle extras) {
        // Switch to full screen.
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN, 
                             WindowManager.LayoutParams.FLAG_FULLSCREEN);
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        
        if (ScanditSDKBarcodePicker.canRunPortraitPicker()) {
            // The new GUI is able to run in portrait and landscape mode, the
            // best use of space however is in portrait mode.
            setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);
            
            // create ScanditSDKBarcodePicker that takes care of the camera access and 
            // barcode recognition.
            ScanditSDKBarcodePicker picker = new ScanditSDKBarcodePicker(
                    this, R.raw.class, extras.getString("appKey"));
            
            // Add both views to activity, with the scan GUI on top.
            this.setContentView(picker);
            mBarcodePicker = picker;
        } else {
            // Make sure the orientation is correct as the old GUI will only
            // be displayed correctly if the activity is in landscape mode.
            setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);
            
            // create ScanditSDKBarcodePicker that takes care of the camera access and 
            // barcode recognition.
            LegacyPortraitScanditSDKBarcodePicker picker = new LegacyPortraitScanditSDKBarcodePicker(
                    this, R.raw.class, extras.getString("appKey"));
            
            // Add both views to activity, with the scan GUI on top.
            this.setContentView(picker);
            mBarcodePicker = picker;
            mBarcodePicker.getOverlayView().setRightButtonCaption("CANCEL");
        }
        
        // Put all options to effect.
        if (extras.containsKey("1DScanning")) {
            mBarcodePicker.set1DScanningEnabled(extras.getBoolean("1DScanning"));
        }
        if (extras.containsKey("2DScanning")) {
            mBarcodePicker.set2DScanningEnabled(extras.getBoolean("2DScanning"));
        }
        if (extras.containsKey("scanningHotSpot")) {
            String hotspot = extras.getString("scanningHotSpot");
            String[] split = hotspot.split("[/]");
            if (split.length == 2) {
                try {
                    Float x = new Float(split[0]);
                    Float y = new Float(split[1]);
                    mBarcodePicker.setScanningHotSpot(x, y);
                } catch (NumberFormatException e) {}
            }
        }
        if (extras.containsKey("searchBar")) {
            mBarcodePicker.getOverlayView().showSearchBar(
                    extras.getBoolean("searchBar"));
        }
        if (extras.containsKey("titleBar")) {
            mBarcodePicker.getOverlayView().showTitleBar(
                    extras.getBoolean("titleBar"));
        }
        if (extras.containsKey("toolBar")) {
            mBarcodePicker.getOverlayView().showToolBar(
                    extras.getBoolean("toolBar"));
        }
        if (extras.containsKey("mostLikelyBarcodeUIElement")) {
            mBarcodePicker.getOverlayView().showMostLikelyBarcodeUIElement(
                    extras.getBoolean("mostLikelyBarcodeUIElement"));
        }
        if (extras.containsKey("beep")) {
            mBarcodePicker.getOverlayView().setBeepEnabled(
                    extras.getBoolean("beep"));
        }
        if (extras.containsKey("vibrate")) {
            mBarcodePicker.getOverlayView().setVibrateEnabled(
                    extras.getBoolean("vibrate"));
        }
        if (extras.containsKey("torch")) {
            mBarcodePicker.getOverlayView().setTorchEnabled(
                    extras.getBoolean("torch"));
        }
        if (extras.containsKey("textForInitialScanScreenState")) {
            mBarcodePicker.getOverlayView().setTextForInitialScanScreenState(
                    extras.getString("textForInitialScanScreenState"));
        }
        if (extras.containsKey("textForBarcodePresenceDetected")) {
            mBarcodePicker.getOverlayView().setTextForBarcodePresenceDetected(
                    extras.getString("textForBarcodePresenceDetected"));
        }
        if (extras.containsKey("textForBarcodeDecodingInProgress")) {
            mBarcodePicker.getOverlayView().setTextForBarcodeDecodingInProgress(
                    extras.getString("textForBarcodeDecodingInProgress"));
        }
        if (extras.containsKey("titleMessage")) {
            mBarcodePicker.getOverlayView().setTitleMessage(
                    extras.getString("titleMessage"));
        }
        if (extras.containsKey("leftButtonCaption")) {
            mBarcodePicker.getOverlayView().setLeftButtonCaption(
                    extras.getString("leftButtonCaption"));
        }
        if (extras.containsKey("leftButtonCaptionWhenKeypadVisible")) {
            mBarcodePicker.getOverlayView().setLeftButtonCaptionWhenKeypadVisible(
                    extras.getString("leftButtonCaptionWhenKeypadVisible"));
        }
        if (extras.containsKey("rightButtonCaption")) {
            mBarcodePicker.getOverlayView().setRightButtonCaption(
                    extras.getString("rightButtonCaption"));
        }
        if (extras.containsKey("rightButtonCaptionWhenKeypadVisible")) {
            mBarcodePicker.getOverlayView().setRightButtonCaptionWhenKeypadVisible(
                    extras.getString("rightButtonCaptionWhenKeypadVisible"));
        }
        if (extras.containsKey("textForMostLikelyBarcodeUIElement")) {
            mBarcodePicker.getOverlayView().setTextForMostLikelyBarcodeUIElement(
                    extras.getString("textForMostLikelyBarcodeUIElement"));
        }
        if (extras.containsKey("textIfNoBarcodeWasRecognized")) {
            mBarcodePicker.getOverlayView().setTextIfNoBarcodeWasRecognized(
                    extras.getString("textIfNoBarcodeWasRecognized"));
        }
        if (extras.containsKey("textToSuggestManualEntry")) {
            mBarcodePicker.getOverlayView().setTextToSuggestManualEntry(
                    extras.getString("textToSuggestManualEntry"));
        }
        if (extras.containsKey("searchBarHint")) {
            mBarcodePicker.getOverlayView().setSearchBarPlaceholderText(
                    extras.getString("searchBarHint"));
        }
        if (extras.containsKey("viewfinderColor")) {
            String color = extras.getString("viewfinderColor");
            if (color.length() == 6) {
                try {
                    String red = color.substring(0, 2);
                    String green = color.substring(2, 4);
                    String blue = color.substring(4, 6);
                    float r = ((float) Integer.parseInt(red, 16)) / 256.0f;
                    float g = ((float) Integer.parseInt(green, 16)) / 256.0f;
                    float b = ((float) Integer.parseInt(blue, 16)) / 256.0f;
                    mBarcodePicker.getOverlayView().setViewfinderColor(r, g, b);
                } catch (NumberFormatException e) {}
            }
        }
        if (extras.containsKey("viewfinderDecodedColor")) {
            String color = extras.getString("viewfinderDecodedColor");
            if (color.length() == 6) {
                try {
                    String red = color.substring(0, 2);
                    String green = color.substring(2, 4);
                    String blue = color.substring(4, 6);
                    float r = ((float) Integer.parseInt(red, 16)) / 256.0f;
                    float g = ((float) Integer.parseInt(green, 16)) / 256.0f;
                    float b = ((float) Integer.parseInt(blue, 16)) / 256.0f;
                    mBarcodePicker.getOverlayView().setViewfinderDecodedColor(r, g, b);
                } catch (NumberFormatException e) {}
            }
        }

        // Register listener, in order to be notified about relevant events 
        // (e.g. a successfully scanned bar code).
        mBarcodePicker.getOverlayView().addListener(this);
    }
    
    @Override
    protected void onPause() {
        // When the activity is in the background immediately stop the 
        // scanning to save resources and free the camera.
        mBarcodePicker.stopScanning();
        super.onPause();
    }
    
    @Override
    protected void onResume() {
        // Once the activity is in the foreground again, restart scanning.
        mBarcodePicker.startScanning();
        super.onResume();
    }

    /** 
     * Called when the user canceled the bar code scanning.
     */
    public void didCancel() {
        finishView();
        
        setResult(CANCEL);
        finish();
    }

    /** 
     * Called when a bar code has been scanned.
     *  
     * @param barcode Scanned bar code content.
     * @param symbology Scanned bar code symbology .
     */
    public void didScanBarcode(String barcode, String symbology) {
        finishView();
        
        Intent intent = new Intent();
        intent.putExtra("barcode", barcode.trim());
        intent.putExtra("symbology", symbology);
        setResult(SCAN, intent);
        finish();
    }

    /** 
     * Called when the user entered a bar code manually.
     * 
     * @param entry The information entered by the user.
     */
    public void didManualSearch(String entry) {
        Intent intent = new Intent();
        intent.putExtra("barcode", entry.trim());
        setResult(MANUAL, intent);
        finish();
    }
    
    /**
     * Called before this activity is finished to improve on the transitioning
     * time.
     */
    private void finishView() {
        mBarcodePicker.stopScanning();
        setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_UNSPECIFIED);
    }
    
    @Override
    public void onBackPressed() {
        didCancel();
    }
}
