var ScanditSDK = function() {

}

/**
 * Available options:
 * 
 * exampleStringForOption: defaultValue
 * Short explanation of option.
 *
 * 1DScanning: true
 * Enables or disables the recognition of 1D codes.
 *
 * 2DScanning: true
 * Enables or disables the recognition of 2D codes.
 * (Note: 2D scanning is not supported by all Scandit SDK versions)
 *
 * scanningHotSpot: "0.5/0.5" (x/y)
 * Changes the location of the spot where the recognition actively scans for 
 * barcodes. X and y can be between 0 and 1, where 0/0 is the top left corner 
 * and 1/1 the bottom right corner.
 *
 * searchBar: false
 * Adds (or removes) a search bar to the top of the scan screen.
 *
 * titleBar: true
 * Adds (or removes) the title bar at the top of the scan screen.
 * This parameter is deprecated but retained for use with the old GUI which is
 * superseeded by the new 3.0 GUI that does not have a title bar anymore.
 *
 * toolBar: true
 * Adds (or removes) the tool bar at the bottom of the scan screen.
 * This parameter is deprecated but retained for use with the old GUI which is
 * superseeded by the new 3.0 GUI that does not have a tool bar anymore.
     
 * mostLikelyBarcodeUIElement: true
 * Enables or disables the 'most likely barcode' UI element. This element is 
 * displayed below the viewfinder when the barcode engine is not 100% confident 
 * in its result and asks for user confirmation. This element is seldom 
 * displayed - typically only when decoding challenging barcodes with fixed
 * focus cameras. 
 *
 * beep: true
 * Enables or disables the sound played when a code was recognized.
 *
 * vibrate: true
 * Enables or disables the vibration when a code was recognized.
 *
 * torch: true
 * Enables or disables the icon that let's the user activate the LED torch
 * mode. If the device does not support torch mode, the icon to activate is
 * will not be visible regardless of the value.
 * 
 * textForInitialScanScreenState: "Align code with box"
 * Sets the text that will be displayed above the viewfinder to tell the user 
 * to align it with the barcode that should be recognized.
 *
 * textForBarcodePresenceDetected: "Align code and hold still"
 * Sets the text that will be displayed above the viewfinder to tell the user 
 * to align it with the barcode and hold still because a potential code seems 
 * to be on the screen.
 *
 * textForBarcodeDecodingInProgress: "Decoding ..."
 * Sets the text that will be displayed above the viewfinder to tell the user 
 * to hold still because a barcode is aligned with the box and the recognition 
 * is trying to recognize it.
 *
 * titleMessage: "Scan a barcode"
 * Sets the title shown at the top of the screen.
 *
 * leftButtonCaption: "KEYPAD"
 * Sets the caption of the left button.
 * Deprecated: This string is only used in the old GUI.
 *
 * leftButtonCaptionWhenKeypadVisible: "OK"
 * Sets the caption of the left button when the keypad is visible.
 *
 * rightButtonCaption: "CANCEL"
 * Sets the caption of the right button.
 *
 * rightButtonCaptionWhenKeypadVisible: "CANCEL"
 * Sets the caption of the right button when the keypad is visible.
 * 
 * textForMostLikelyBarcodeUIElement: "Tap to use this likely number"
 * Sets the text that is displayed alongside the 'most likely barcode' UI 
 * element that is displayed when the barcode engine is not 100% confident 
 * in its result and asks for user confirmation.
 *
 * textIfNoBarcodeWasRecognized: "No barcode recognized"
 * Sets the text that is displayed if the engine was unable to recognize
 * the barcode.
 *
 * textToSuggestManualEntry: "Enter barcode via keypad"
 * Sets the text that is displayed if the engine was unable to recognize
 * the barcode and it is suggested to enter the barcode manually.
 *
 * searchBarHint: "Scan barcode or enter it here"
 * Sets the text shown in the manual entry field when nothing has been
 * entered yet.
 *
 * viewfinderColor: "FFFFFF"
 * Sets the color of the viewfinder when no code has been recognized yet.
 * 
 * viewfinderDecodedColor: "00FF00"
 * Sets the color of the viewfinder once the barcode has been recognized.
 * 
 */

ScanditSDK.prototype.scan = function(success, fail, appKey, options) {
    return PhoneGap.exec(success, fail, 'ScanditSDK', 'scan', 
                         [appKey, options]);
};

PhoneGap.addConstructor(function() {
    PhoneGap.addPlugin('ScanditSDK', new ScanditSDK());
});