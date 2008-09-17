#include "StdAfx.h"
#include "HashField.h"
#include "mshtmdid.h"
#include <set>
#include <fstream>
#include "HashedPassword.h"
#include <wininet.h>
#include "Plugin.h"
#include "Toolbar.h"
#include "XMessageBox.h"

#define BGCOLOR_DONT_HASH "#FFFFCC"    // BG color of textbox that indicates not to hash

#define KEY_TOGGLE_PROTECT	VK_F2	// F2 toggles field protection

#define INTERNET_MAX_HOSTNAME_LENGTH 255

extern set<const char*, ltstr> passwords;

HashField::HashField()
  :
  mElt(0),
  mWindowHandle(0),
  mHashKey(0),
  mHashParams(0),
  mContentType(HashField::PLAINTEXT),
  mCookie(0),
  mHasNewValue(false)
{
}

void HashField::Init(CComPtr<IHTMLInputElement>& elt, HWND windowHandle)
{
  mElt = elt;
  mWindowHandle = windowHandle;

  CComQIPtr<IHTMLElement> pElt = elt;
  CComPtr<IDispatch> pDispatch;
  pElt->get_document(&pDispatch);
  CComQIPtr<IHTMLDocument2> pDoc = pDispatch;
  CComPtr<IHTMLLocation> pLoc;
  pDoc->get_location(&pLoc);
  CComBSTR sLoc;
  pLoc->get_hostname(&sLoc);

  // XXX Not sure what case this is supposed to cover, pages without domains?
  // if (!mHashKey) SetIsHashing(false, false);

  COLE2T szLoc(sLoc);
  char* pzLoc = new char[strlen(szLoc) + 1];
  _tcscpy(pzLoc, szLoc);
  Plugin::GetHashInfo(pzLoc, mHashKey, mHashParams);

  AtlAdvise(elt, (IDispatch*)this, DIID_HTMLInputTextElementEvents2, &mCookie);          
}

HashField::~HashField()
{
  AtlUnadvise((IDispatch*)this, DIID_HTMLInputTextElementEvents2, mCookie);
  mElt = NULL;
  //UpdateBackgroundColor(true);
}

void HashField::put_value(CComBSTR value)
{
	mNewValue = value;
	mHasNewValue = true;
}

STDMETHODIMP HashField::Invoke(DISPID dispidMember,
                               REFIID riid,
                               LCID lcid,
                               WORD wFlags,
                               DISPPARAMS* pdispparams,
                               VARIANT* pvarResult,
                               EXCEPINFO* pexcepinfo,
                               UINT* puArgErr)
{
  IHTMLEventObj* evt = (IHTMLEventObj*)pdispparams->rgvarg[0].pdispVal;

  switch(dispidMember) 
	{
    case DISPID_HTMLELEMENTEVENTS_ONDBLCLICK:
		// Double clicking is no longer a way to enable hashing
		// SetIsHashing(!mIsHashing, true);

		break;
    
    case DISPID_HTMLELEMENTEVENTS_ONFOCUS:
		{
		  // XXX This is a sanity check -- may not be necessary any longer.
		  UpdateTrafficLight();
		}
		break;

    case DISPID_HTMLELEMENTEVENTS_ONBLUR:
		{
			FinishEntry();
		}
		break;

	case DISPID_HTMLELEMENTEVENTS_ONKEYPRESS:	
		long keyCode;
		evt->get_keyCode(&keyCode);
		if(keyCode == 13) // enter key
		{
			FinishEntry();
		}
		break;

	case DISPID_HTMLELEMENTEVENTS_ONKEYUP:		
		// Delayed completion of put_value method
		if(mHasNewValue)
		{
			mElt->put_value(mNewValue);
			mHasNewValue = false;
		}
		break;

	case DISPID_HTMLELEMENTEVENTS_ONDROP:
		if(mContentType == HashField::CIPHERTEXT)
		{
			mElt->put_value(CComBSTR(""));
			SetContentType(HashField::PLAINTEXT);
		}
		else if(mContentType == HashField::MASKEDTEXT)
		{
			// Intercept the dropped text
			CComQIPtr<IHTMLEventObj2> evt2 = evt;
			CComPtr<IHTMLDataTransfer> dropData;
			if(FAILED(evt2->get_dataTransfer(&dropData))) break;
			if(!dropData) break;

			// Replace data with empty string
			COLE2T pData = CComBSTR("");
			CComVariant newData(pData);
			VARIANT_BOOL pret;
			dropData->setData(CComBSTR("Text"), &newData, &pret);

			// Display warning
			TCHAR title[50], message[512];

			LoadString(GetModuleHandle("pwdhash.dll"), IDS_DROP_WARNING_TITLE, title, sizeof(title) / sizeof(TCHAR));
			LoadString(GetModuleHandle("pwdhash.dll"), IDS_DROP_WARNING_MESSAGE, message, sizeof(message) / sizeof(TCHAR));
			
			XMessageBox((HWND)mWindowHandle, message, title, MB_OK | MB_ICONWARNING);
		}
		break;

	case DISPID_HTMLELEMENTEVENTS_ONPASTE:
		if(mContentType == HashField::MASKEDTEXT)
		{
			// Cancel the paste event
			evt->put_returnValue(CComVariant(VARIANT_FALSE));

			// Retrieve the clipboard data
			if(!OpenClipboard(mWindowHandle)) break;
			HANDLE clipboardHandle = GetClipboardData(CF_TEXT);
			if(!clipboardHandle) break;
			LPCSTR pClipboard = (LPCSTR)GlobalLock(clipboardHandle);
			CA2CT clipboardString(pClipboard);
			GlobalUnlock(clipboardHandle);
			CloseClipboard();

			// Send a masked version to the password field
			SimulateText(clipboardString);
		}
		break;
	}

  return S_OK;
}

void HashField::FinishEntry()
{
	if(mContentType == HashField::MASKEDTEXT)
	{
		const char* pzHashedPassword = GetCipherText();
		CA2W sHashedPassword(pzHashedPassword);
		mElt->put_value(sHashedPassword);
		this->SetContentType(HashField::CIPHERTEXT);
	}
}

void HashField::SimulateText(LPCSTR text)
{
	if(text)
	{
		bool ctrlOn = (GetKeyState(VK_CONTROL) & 0x80) != 0;
		if(ctrlOn) Plugin::SetControlKeyState(FALSE);
		while(*text)
		{
			char c = GetMask(*text++);
			Plugin::SimulateKeyPress(c, WM_KEYDOWN);
			Plugin::SimulateKeyPress(c, WM_KEYUP);
		}
		if(ctrlOn) Plugin::SetControlKeyState(TRUE);
	}
}

void HashField::InterceptDataTransfer(CComPtr<IHTMLDataTransfer> dt)
{
	// Get the existing data
	CComVariant dataResult;
	if(FAILED(dt->getData(CComBSTR("Text"), &dataResult))) return;
	if(dataResult.vt == VT_EMPTY) return;
	CComBSTR bstrData = dataResult.bstrVal;

	// Create an array of masked characters
	COLE2T pData = bstrData;
	size_t iDataSize = strlen(pData);
	for(unsigned long i = 0; i < iDataSize; i++)
		pData[i] = GetMask(pData[i]);

	// Save the masked data on top of the old data
	CComVariant newData(pData);
	VARIANT_BOOL pret;
	dt->setData(CComBSTR("Text"), &newData, &pret);
}
/*
void HashField::SetIsProtected(bool flag)
{
	mIsProtected = flag;

	if(mIsProtected && !mIsHashing)
		SetIsHashing(true, false);		

	UpdateTrafficLight();
}
*/
void HashField::UpdateTrafficLight()
{
	CToolbar* toolbar = Plugin::GetToolbar();
	if(toolbar)
		toolbar->ChangeSignal(mContentType == HashField::MASKEDTEXT ? CToolbar::SIGNAL_GREEN : CToolbar::SIGNAL_RED);
}
/*
void HashField::WarnUnprotectedUser()
{
    TCHAR title[50], message[512];

	XMSGBOXPARAMS params;
	params.hInstanceStrings = (HINSTANCE) GetModuleHandle("pwdhash.dll");
	params.nIdCustomButtons = IDS_UNPROTECTED_PASSWORD_WARNINGa_BUTTONS;

    LoadString(GetModuleHandle("pwdhash.dll"), IDS_UNPROTECTED_PASSWORD_WARNING_TITLE, title, sizeof(title) / sizeof(TCHAR));
	LoadString(GetModuleHandle("pwdhash.dll"), IDS_UNPROTECTED_PASSWORD_WARNING_MESSAGE, message, sizeof(message) / sizeof(TCHAR));
	
	if (XMessageBox((HWND)mWindowHandle, message, title,
                   MB_YESNO | MB_ICONWARNING | MB_DEFBUTTON3, &params) == IDCUSTOM2)
	   SetIsHashing(false, false);
}
void HashField::SetIsHashing(bool flag, bool confirm)
{
  // Show a confirmation dialog if the user toggled to unhash
  if (!flag && confirm) {
    // XXX Should probably use SizeOfResource here
    // XXX Should have better button names and checkbox, using XMessageBox
    TCHAR title[50], message[512];
    // Load Shdoclc.dll and the IE message box title string
    HINSTANCE handle = LoadLibrary(TEXT("SHDOCLC.DLL"));
    if (handle) {
      #define IDS_MESSAGE_BOX_TITLE 2213
      LoadString(handle, IDS_MESSAGE_BOX_TITLE, title, sizeof(title) / sizeof(TCHAR));
      FreeLibrary(handle);
    }
    else {
      LoadString(GetModuleHandle("pwdhash.dll"), IDS_DISABLE_HASHING_WARNING_TITLE, title, sizeof(title) / sizeof(TCHAR));
    }
	LoadString(GetModuleHandle("pwdhash.dll"), IDS_DISABLE_HASHING_WARNING_MESSAGE, message, sizeof(message) / sizeof(TCHAR));
	if (XMessageBox((HWND)mWindowHandle, message, title,
                   MB_OKCANCEL | MB_ICONWARNING | MB_DEFBUTTON2) == IDCANCEL) {      
      return;
    }
  }
  mIsHashing = flag;

  mElt->put_value(BSTR(""));
  //UpdateBackgroundColor(flag);
  UpdateTrafficLight();
}
*/
void HashField::UpdateBackgroundColor(bool flag)
{
  // Now change background color to indicate whether to hash or not
  CComQIPtr<IHTMLElement> htmlElt(mElt);
  if (!htmlElt)
    return;

  CComPtr<IHTMLStyle> style;
  HRESULT hr = htmlElt->get_style(&style);
  if (FAILED(hr))
    return;

  // Get and store old bg color: if they toggle to don't-hash and then back again,
  // we should restore bg color to the original one, not to white
  if (!mOldColor.ByteLength()) {
    VARIANT oldColor;
    oldColor.vt = VT_BSTR;
    style->get_backgroundColor(&oldColor);
    if (!CComBSTR(oldColor.bstrVal).ByteLength())
      mOldColor = CComBSTR("white"); // No bgcolor specified, assume white
    else
      mOldColor = oldColor.bstrVal;
  }

  VARIANT newColor;
  newColor.vt = VT_BSTR;
  newColor.bstrVal = flag ? mOldColor : CComBSTR(BGCOLOR_DONT_HASH);
  style->put_backgroundColor(newColor);
}

const char* HashField::GetHashKey() const
{
  return mHashKey;
}

void HashField::SetMaskedText(const char* text)
{
  mElt->put_value(CComBSTR(text));
}

const char* HashField::GetMaskedText() const
{
  CComBSTR val;
  HRESULT hr = mElt->get_value(&val);
  
  // The following check prevents the browser from crashing on empty password fields.
  if(FAILED(hr) || val.Length() == 0) return strdup("");
  
  COLE2TEX<Plugin::MAX_PASSWORD_LENGTH> valCStr(val);
  return strdup(valCStr);
}

const char* HashField::GetPlainText() const
{
  const char* mask = GetMaskedText();
  return mTable.Translate((char*)mask);
}

const char* HashField::GetCipherText() const
{
  const char* str = GetPlainText();
  if (mContentType == HashField::MASKEDTEXT) {    
    char* text;
    if (Plugin::IsUsingGlobalPwd()) {
      const char* globalPwd = Plugin::GetGlobalPwd();
	    text = new char[strlen(str) + strlen(globalPwd) + 1];
	    _tcscpy(text, str);
      _tcscat(text, globalPwd);
    }
    else {
      text = new char[strlen(str) + 1];
      _tcscpy(text, str);
    }
	str = HashedPassword::getHashedPassword(text, mHashKey);
  }  
  return str;
}

char HashField::GetMask(char c)
{
  return mTable.GetMask(c);
}

char HashField::Translate(char c) const
{
  return mTable.Translate(c);
}

