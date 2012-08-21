package com.mirasense.scanditsdk.plugin;

import java.util.Iterator;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.content.Intent;
import android.util.Log;

import org.apache.cordova.api.Plugin;
import org.apache.cordova.api.PluginResult;
import org.apache.cordova.api.PluginResult.Status;

/**
 * 
 * Copyright 2011 Mirasense AG. All rights reserved.
 */
public class ScanditSDK extends Plugin {
    
    public static final String SCAN = "scan";
    
    private String mCallbackId;
    
    
    @Override
    public PluginResult execute(String action, JSONArray data, String callbackId) {
        mCallbackId = callbackId;
        PluginResult result = null;

        if (action.equals(SCAN)) {
            scan(data);
            result = new PluginResult(Status.NO_RESULT);
            result.setKeepCallback(true);
        } else {
            result = new PluginResult(Status.INVALID_ACTION);
        }
        
        return result;
    }
    
    private void scan(JSONArray data) {
        Intent intent = new Intent(ctx.getContext(), ScanditSDKActivity.class);
        try {
            intent.putExtra("appKey", data.getString(0));
        } catch (JSONException e) {
            Log.e("ScanditSDK", "Function called through Java Script contained illegal objects.");
            e.printStackTrace();
            return;
        }
        
        if (data.length() > 1) {
            // We extract all options and add them to the intent extra bundle.
            try {
                JSONObject options = data.getJSONObject(1);
                @SuppressWarnings("unchecked")
                Iterator<String> iter = (Iterator<String>) options.keys();
                while (iter.hasNext()) {
                    String key = iter.next();
                    Object obj = options.opt(key);
                    if (obj != null) {
                        if (obj instanceof Integer) {
                            intent.putExtra(key, (Integer) obj);
                        } else if (obj instanceof Boolean) {
                            intent.putExtra(key, (Boolean) obj);
                        } else if (obj instanceof String) {
                            intent.putExtra(key, (String) obj);
                        }
                    }
                }
            } catch (JSONException e) {
                e.printStackTrace();
            }
        }
        ctx.startActivityForResult(this, intent, 1);
    }
    
    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (resultCode == ScanditSDKActivity.SCAN) {
            String barcode = data.getExtras().getString("barcode");
            String symbology = data.getExtras().getString("symbology");
            JSONArray args = new JSONArray();
            args.put(barcode);
            args.put(symbology);
            this.success(new PluginResult(Status.OK, args), mCallbackId);
        
        } else if (resultCode == ScanditSDKActivity.MANUAL) {
            String barcode = data.getExtras().getString("barcode");
            this.success(new PluginResult(Status.OK, barcode), mCallbackId);
            
        } else if (resultCode == ScanditSDKActivity.CANCEL) {
            this.error(new PluginResult(Status.NO_RESULT, "Canceled"), mCallbackId);
        }
    }
}
