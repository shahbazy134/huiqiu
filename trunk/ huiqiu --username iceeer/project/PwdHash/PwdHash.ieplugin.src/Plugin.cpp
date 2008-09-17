#include "stdafx.h"
#include "Plugin.h"
#include "ExDispID.h"
#include <Atlbase.h>
#include <windows.h>
#include <string>
#include <map>
#include <fstream>
#include "HashManager.h"
#include <algorithm>
#include "OptionsDlgCfg.h"
#include <wininet.h>
#include "regex-include.h"
#include "XMessageBox.h"

#import "pstorec.dll" no_namespace

// Init all the static variables (shared across all threads)
map<DWORD, DWORD> Plugin::mParentThreads;  // given a thread, find its parent window thread
map<DWORD, DWORD> Plugin::mActiveTabThreads;  // given a parent window thread, get active tab thread
map<DWORD, window> Plugin::mWindows;
set<const char*, ltstr> Plugin::mPasswords;
static KeystrokeChecker* checker = NULL;
const char* Plugin::mGlobalPwd = NULL;
bool Plugin::mUseGlobalPwd = false;
bool Plugin::mIsAdvancedMode = true;
bool Plugin::mIsEnabled = true;
bool Plugin::mIsRecordingPasswords = false;
bool Plugin::mRulesInitialized = false;
HANDLE Plugin::mHQuitEvent = 0;
vector<rule> Plugin::mRules;
const char* Plugin::helpURL = "http://crypto.stanford.edu/PwdHash/";
string keystream;

bool Plugin::IsAdvancedMode()
{ 
  return mIsAdvancedMode;
}

bool Plugin::IsEnabled()
{
  return mIsEnabled;
}

const char* Plugin::GetGlobalPwd()
{
  return mGlobalPwd;
}

bool Plugin::IsUsingGlobalPwd()
{
  return mUseGlobalPwd;
}

bool Plugin::IsRecordingPasswords()
{
	return mIsRecordingPasswords;
}

void Plugin::SetRecordingPasswords(bool value)
{
	mIsRecordingPasswords = value;
}

void Plugin::InitializeRegistry()
{
  HKEY key;
  if (FAILED(RegOpenKeyEx(HKEY_CURRENT_USER, TEXT("Software\\PwdHash"), 0,
                          KEY_QUERY_VALUE, &key))) {
    return;
  }

  char valueBuf[sizeof(DWORD)*2 + 50];
  DWORD size = sizeof(valueBuf);
  VALENT val[5] = { { TEXT("Enabled"), 0, NULL, REG_DWORD },
                    { TEXT("UseGlobalPassword"), 0, NULL, REG_DWORD },
					{ TEXT("GlobalPassword"), 0, NULL, REG_SZ },
					{ TEXT("F2Mode"), 0, NULL, REG_DWORD },
                    { TEXT("RecordPasswords"), 0, NULL, REG_DWORD } };

  if (SUCCEEDED(RegQueryMultipleValues(key, val, sizeof(val) / sizeof(VALENT), valueBuf, &size))) {
    if (val[0].ve_valueptr) {
      bool isEnabled = (*(DWORD*)(val[0].ve_valueptr) == 1);
      if (isEnabled != mIsEnabled) {
        mIsEnabled = isEnabled;        
        Notify(eEnabled);
        if (mIsEnabled)
          TurnOn();
        else
          TurnOff();
      }
    }
    if (val[1].ve_valueptr)
      mUseGlobalPwd = (*(DWORD*)(val[1].ve_valueptr) == 1);
    if (mGlobalPwd)
      free((void*)mGlobalPwd);
    if (val[2].ve_valueptr)
      mGlobalPwd = strdup((LPCTSTR)(val[2].ve_valueptr));
    else
      mGlobalPwd = NULL;

    if (val[3].ve_valueptr) {
      bool advancedMode = (*(DWORD*)(val[3].ve_valueptr) == 1);      
      if (advancedMode != mIsAdvancedMode) {
        mIsAdvancedMode = advancedMode;
        Notify(eMode);
      }
	}

	if (val[4].ve_valueptr) {
	  bool recordPassword = (*(DWORD*)(val[4].ve_valueptr) == 1);      
      if (recordPassword != mIsRecordingPasswords) {
        mIsRecordingPasswords = recordPassword;
        Notify(eMode);
      }
	}
  }

  RegCloseKey(key);
}

DWORD WINAPI Plugin::RegNotify(LPVOID evt)
{ 
  HANDLE hChangeEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
  HANDLE hQuitEvent = *(HANDLE*)evt;
  HANDLE events[2] = { hChangeEvent, hQuitEvent };

  while(true) {
    HKEY key;
    if (FAILED(RegOpenKeyEx(HKEY_CURRENT_USER, TEXT("Software\\PwdHash"), 0,
                            KEY_NOTIFY, &key))) {
      return E_FAIL;
    }
    RegNotifyChangeKeyValue(key, FALSE, REG_NOTIFY_CHANGE_LAST_SET, events[0], TRUE);
    DWORD evt = WaitForMultipleObjects(2, events, FALSE, INFINITE);
    switch(evt) {
      case WAIT_OBJECT_0 + 0:
        InitializeRegistry();
        RegCloseKey(key);
        ResetEvent(events[0]);
        break;
      case WAIT_OBJECT_0 + 1:
      default:
        RegCloseKey(key);
        return S_OK;
    }
  }
}

void Plugin::Notify(RegValue reg)
{
  for (map<DWORD, window>::iterator iter = mWindows.begin(); iter != mWindows.end(); ++iter) {
    window& w = iter->second;
    CToolbar* toolbar = w.toolbar;

	/*
	if (reg == eMode) {
      if (toolbar)      
        toolbar->OnModeChange(mIsAdvancedMode);
    }
    else
	*/
	
	if (reg == eEnabled) {
      if (toolbar)
        toolbar->OnEnabledChange(mIsEnabled);
    }
  }
}

void Plugin::AddToRule(rule* r, const char* tokenName, size_t tokenNameLen, const char* tokenValue, size_t tokenValueLen)
{
  if (!strncmp(tokenName, "pattern", tokenNameLen)) {
    r->pattern = new char[tokenValueLen+1];
    memcpy(r->pattern, tokenValue, tokenValueLen);
    r->pattern[tokenValueLen] = '\0';
  }
  else if (!strncmp(tokenName, "key", tokenNameLen)) {
    r->key = new char[tokenValueLen+1];
    memcpy(r->key, tokenValue, tokenValueLen);
    r->key[tokenValueLen] = '\0';
  }
  else if (!strncmp(tokenName, "domains", tokenNameLen)) {
    char* temp = new char[tokenValueLen+1];
    memcpy(temp, tokenValue, tokenValueLen);
    temp[tokenValueLen] = '\0';
    r->domains = atoi(temp);
    delete[] temp;    
  }
  else if (!strncmp(tokenName, "params", tokenNameLen)) {
    char* temp = new char[tokenValueLen+1];
    memcpy(temp, tokenValue, tokenValueLen);
    temp[tokenValueLen] = '\0';
    r->params = atoi(temp);
    delete[] temp;
  }
}

void Plugin::InitializeRules()
{
  if (mRulesInitialized)
    return;

  const char* config, *curPos;
  config = curPos = ReadConfig();
  while(curPos = strstr(curPos, "<rule ")) {
    rule r;
    const char* tokenStart = curPos = curPos + strlen("<rule ");
    bool haveToken = false;
    bool isStartQuote = false;
    const char* tokenName = NULL;
    const char* tokenValue = NULL;
    size_t tokenNameLength = 0;
    size_t tokenValueLength = 0;
    while(true) {
      while(*curPos && *curPos != '\\' && *curPos != '/' && *curPos != '"' && *curPos != '=') {
        ++curPos;
      }
      if (*curPos == '=') {
        haveToken = true;
        isStartQuote = true;
        while(*tokenStart == ' ')
          ++tokenStart;
        tokenName = tokenStart;
        tokenNameLength = curPos - tokenStart;
      }
      else if (*curPos == '"') {
        if (isStartQuote) {
          // This is the beginning of the value for the token
          tokenValue = curPos + 1;
        }
        else {
          // This is the end of the value for the token
          tokenValueLength = curPos - tokenValue;
          haveToken = false;
          AddToRule(&r, tokenName, tokenNameLength, tokenValue, tokenValueLength);
        }
        isStartQuote = !isStartQuote;
      }
      else if (*curPos == '\\' && haveToken) {
        // We're in a token, so treat this as an escape signal
        ++curPos;
      }
      else if (*curPos == '/' && !haveToken) {
        // We're not in a token, so treat this as the end
        curPos += 2;
        break;
      }

      ++curPos;
      tokenStart = curPos;
    }
    mRules.push_back(r);
  }
  free((void*)config);
}

char* Plugin::GetPathTo(PluginFile file)
{
  const char* filename;
  if (file == FILE_RULES)
    filename = "rules.xml";
  else if (file == FILE_PASSWORDS)
    filename = "passwords.txt";

  size_t size = MAX_PATH + 1 /* \ */ + strlen(filename);
  char* buffer = (char*)malloc(size);
  GetModuleFileName(GetModuleHandle("pwdhash.dll"), buffer, (DWORD)size);
  PathRemoveFileSpec(buffer);
  
  strcat(buffer, "\\");
  strcat(buffer, filename);
  return buffer;
}

const char* Plugin::ReadConfig()
{  
  char* rulesPath = GetPathTo(FILE_RULES);
  ifstream rulesFile(rulesPath);
  string line, rules;
  while (getline(rulesFile, line)) {
    rules += line;
  }
  rulesFile.close();
  free(rulesPath);
  return strdup(rules.c_str());
}

void Plugin::RegisterToolbar(CToolbar* toolbar)
{
  window& w = mWindows[GetCurrentThreadId()];
  w.toolbar = toolbar;
}

void Plugin::UnregisterToolbar()
{
  window& w = mWindows[GetCurrentThreadId()];
  w.toolbar = NULL;
}
CComObject<HashField>* Plugin::GetField()
{
  const window& w = mWindows[GetCurrentThreadId()];
  CComPtr<IDispatch> pDispatch;
  w.browser->get_Document(&pDispatch);
  CComQIPtr<IHTMLInputElement> inputElt;
  if (SUCCEEDED(GetActiveElement(pDispatch, inputElt)) && inputElt) {
    CComBSTR name;
    inputElt->get_name(&name);
    COLE2T nameCStr(name); // Guards against buffer overflow
    return GetFieldFor(nameCStr);
  }
  else return NULL;
}
CComObject<HashField>* Plugin::GetFieldFor(const char* name, DWORD thread)
{
  if (!thread)
    thread = GetCurrentThreadId();

  const window& w = mWindows[thread];
  map<const char*, CComObject<HashField>*, ltstr>::const_iterator iter = w.fields.find(name);
  if (iter == w.fields.end())
    return NULL;

  return iter->second;
}
STDMETHODIMP Plugin::SetSite(IUnknown *pUnkSite)
{
  DWORD parentThread = ::GetWindowThreadProcessId(::GetForegroundWindow(), NULL);
  DWORD thread = GetCurrentThreadId();
  window& w = mWindows[thread];
  Plugin::mParentThreads[thread] = parentThread;

	if (pUnkSite) {
    w.browser = pUnkSite;
    
    if (!checker) {
      // Read in initial registry values (we can't just spawn the thread and signal it
      // to have it do the read for us since we need this to happen immediately, before
      // all the plugin's activity begins
      InitializeRegistry();      
      mHQuitEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
      CreateThread(NULL, sizeof(mHQuitEvent), RegNotify, (LPVOID)&mHQuitEvent, 0, NULL);
      TurnOn();
      checker = new KeystrokeChecker(HandleKeystroke);
    }
    
    w.browser->get_HWND((long*)&w.handle);
    theDispEvent::DispEventAdvise(w.browser);
  }
  else {
    theDispEvent::DispEventUnadvise(w.browser);
  }
  return S_OK;
}

bool Plugin::ShouldHandleKeystroke(KBDLLHOOKSTRUCT* kb, WPARAM message, ProtectionType &protectionType)
{
	if( Plugin::IsAdvancedMode()		&&
	  !(kb->flags & LLKHF_INJECTED)     &&
	   message == WM_KEYUP			    &&
	   kb->vkCode == KEY_TOGGLE_PROTECT)
  {
	  protectionType = Plugin::PASSWORD_KEY_PROTECTION;
	  return true;
  }

  protectionType = Plugin::NO_PROTECTION;
  kb->vkCode = LOWORD(MapVirtualKey(kb->vkCode, 2));

  bool shouldHandle = !(kb->flags & LLKHF_INJECTED)     && 
					isprint(kb->vkCode)               &&
					!(GetKeyState(VK_CONTROL) & 0x80) &&
					!(GetKeyState(VK_MENU) & 0x80)    && 
					(message == WM_KEYDOWN || WM_KEYUP);
  return shouldHandle;
}

DWORD WINAPI Plugin::NonpasswordFieldWarning(LPVOID pParam)
{
  TCHAR message[1024];
  LoadString(GetModuleHandle("pwdhash.dll"), IDS_PROTECT_NONPASSWORD_FIELD_WARNING, message, sizeof(message) / sizeof(TCHAR));
  XMessageBox(NULL, message, "Warning", MB_OK);
  return 0;
}

bool Plugin::HandleKeystroke(WPARAM wParam, LPARAM lParam)
{
  if (!mIsEnabled)
    return false;

  // XXX Set windows proc, listen for WM_PARENTNOTIFY until _Server is set up
  // then reset hwnd (or actually, just hook up window there), then unset window proc
  // so dont have to FindWindowEx in keyboardproc every time

  ProtectionType protectionType;

  // Instead of checking class name, pass in HWND and compare?
  TCHAR cl[100];
  HWND focusedWnd = GetForegroundWindow();
  GetClassName(focusedWnd, cl, sizeof(cl));
  if (_tcscmp(cl, _T("IEFrame")))
    return false;

  DWORD thread = ::GetWindowThreadProcessId(focusedWnd, NULL);
  thread = Plugin::mActiveTabThreads[thread];
  GUITHREADINFO gui;
  gui.cbSize = sizeof(GUITHREADINFO);
  GetGUIThreadInfo(thread, &gui);

  const CToolbar* toolbar = Plugin::GetToolbar(thread);
  if (!toolbar || gui.hwndFocus != toolbar->GetEditWindow()) {
    GetClassName(gui.hwndFocus, cl, sizeof(cl));
    if (_tcscmp(cl, _T("Internet Explorer_Server")))
      return false;
  }

  // XXX What about international?
  KBDLLHOOKSTRUCT* kb = (KBDLLHOOKSTRUCT*)lParam;
  if (!ShouldHandleKeystroke(kb, wParam, protectionType))
    return false;

  char state[256];
  DWORD key = kb->vkCode;
  
  if (GetKeyboardState((PBYTE)state))
    ToAscii(key, kb->scanCode, (const BYTE*)state, (WORD*)&key, 0);

  if(wParam == WM_KEYUP)
  {
	if(keystream.size() == strlen(PASSWORD_PREFIX)) keystream = keystream.substr(1);
	keystream += key;
	if(keystream.compare(PASSWORD_PREFIX) == 0) 
	{
		protectionType = Plugin::PASSWORD_PREFIX_PROTECTION;
		keystream = "";
	}
  }

  CComPtr<IDispatch> pDispatch;     
  CComPtr<IWebBrowser2> browser = mWindows[thread].browser;
  if (!browser)
    return false;

  browser->get_Document(&pDispatch);
  if (!pDispatch)
    return false;

  CComQIPtr<IHTMLInputElement> inputElt;

  bool found = GetActiveElement(pDispatch, inputElt);

  CComBSTR val;
  if(found) inputElt->get_value(&val);	  

  if(protectionType != Plugin::NO_PROTECTION &&
	  (!found ||
	  !Plugin::IsPasswordField(inputElt) ||
	  protectionType == Plugin::PASSWORD_PREFIX_PROTECTION && val != PASSWORD_PREFIX))
  {
    // We have to do this warning in a separate thread or else Windows will hang for
    // a moment, since we're in the middle of a keyboard interrupt
	DWORD dwThreadID = 0;
	HANDLE hThread;
	hThread = ::CreateThread(0,
                     0,
					 (LPTHREAD_START_ROUTINE)Plugin::NonpasswordFieldWarning,
                     (void *)0,
                     0,
                     &dwThreadID);
    return false;
  }

  if(!found)
	  return false;

  if(Plugin::IsTextField(inputElt)) 
	  return HandleTextFieldKeystroke(inputElt, wParam, gui.hwndFocus);  

  if (Plugin::IsPasswordField(inputElt))
    return HandlePwdFieldKeystroke(inputElt, key, wParam, thread, protectionType);

  return false;
}

// Finds the active element of pDocument and places it in pActiveElement
// Returns true on success, false otherwise
bool Plugin::GetActiveElement(CComPtr<IDispatch>& pDispatch, CComQIPtr<IHTMLInputElement>& pActiveElement)
{
  // First, try to get the active element directly
  CComQIPtr<IHTMLDocument2> pDocument = pDispatch;
  CComPtr<IHTMLElement> elt;
  if(SUCCEEDED(pDocument->get_activeElement(&elt)))
  {
	pActiveElement = elt;
	if (pActiveElement) return true;
  }

  // If that didn't work, try recursing through the frames
  CComQIPtr<IOleContainer> pDocContainer = pDocument;
  CComPtr<IEnumUnknown> pEnumerator;
  pDocContainer->EnumObjects(OLECONTF_EMBEDDINGS, &pEnumerator);
  while(1)
  {
	ULONG uFetched;
	CComPtr<IUnknown> pUnk;
	HRESULT hr = pEnumerator->Next(1, &pUnk, &uFetched);
	if(hr != S_OK) break;
	CComQIPtr<IWebBrowser2> pWebBrowser = pUnk;
	if(!pWebBrowser) continue;
	CComPtr<IDispatch> pNewDispatch;
	pWebBrowser->get_Document(&pNewDispatch);
    if(GetActiveElement(pNewDispatch, pActiveElement)) return true;
  }

  // Could not find the active element
  return false;
}


bool Plugin::HandleTextFieldKeystroke(CComQIPtr<IHTMLInputElement>& inputElt, WPARAM msg,
                                      HWND focusedWnd)
{
/*
  CComBSTR value;
  inputElt->get_value(&value);
  if (msg == WM_KEYUP && value.ByteLength()) {
    COLE2TEX<Plugin::MAX_PASSWORD_LENGTH> valueCStr(value);

    if (mIsRecordingPasswords && Plugin::IsPassword(valueCStr)) {
      // XXX Not focusedWnd (IE_Server) but main window?                
		TCHAR title[50], message[512];
		LoadString(GetModuleHandle("pwdhash.dll"), IDS_ALREADY_TYPED_PASSWORD_WARNING_TITLE, title, sizeof(title) / sizeof(TCHAR));
		LoadString(GetModuleHandle("pwdhash.dll"), IDS_ALREADY_TYPED_PASSWORD_WARNING_MESSAGE, message, sizeof(message) / sizeof(TCHAR));
		if (XMessageBox(focusedWnd, message, title, MB_YESNO | MB_ICONWARNING) == IDYES) {
        inputElt->put_value(CComBSTR(""));
      }
      return true;
    }
  }
*/
  return false;
}
void Plugin::ToggleCaps()
{
  KEYBDINPUT capsOn = { VK_CAPITAL, 0, KEYEVENTF_EXTENDEDKEY, NULL};
  KEYBDINPUT capsOff = { VK_CAPITAL, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, NULL };
  INPUT i[2];
  i[0].type = INPUT_KEYBOARD;
  i[0].ki = capsOn;
  i[1].type = INPUT_KEYBOARD;
  i[1].ki = capsOff;
  SendInput(2, i, sizeof(INPUT));
}

void Plugin::SetControlKeyState(bool down)
{
	DWORD flags = KEYEVENTF_EXTENDEDKEY;
	if(!down) flags |= KEYEVENTF_KEYUP;
	KEYBDINPUT action = { VK_CONTROL, 0, flags, NULL};
	INPUT i;
	i.type = INPUT_KEYBOARD;
	i.ki = action;
	SendInput(1, &i, sizeof(INPUT));
}

void Plugin::SimulateKeyPress(char key, WPARAM msg)
{
  bool isCaps = (GetKeyState(VK_CAPITAL) & 0x1) != 0;

  if (isCaps) 
    ToggleCaps();

  KEYBDINPUT k = { key, 0, msg == WM_KEYUP ? KEYEVENTF_KEYUP : NULL, NULL, NULL };
  INPUT keys[1];
  keys[0].type = INPUT_KEYBOARD;
  keys[0].ki = k;
  SendInput(sizeof(keys)/sizeof(INPUT), keys, sizeof(INPUT));

/*
  k.dwFlags = KEYEVENTF_KEYUP;
  SendInput(sizeof(keys)/sizeof(INPUT), keys, sizeof(INPUT));
*/

  if (isCaps)
    ToggleCaps();
}
/*
const char* Plugin::GetActionURLFor(CComQIPtr<IHTMLInputElement>& inputElt)
{
  char* key = new char[INTERNET_MAX_URL_LENGTH + 1];
  CComPtr<IHTMLFormElement> formElt;
  HRESULT hr = inputElt->get_form(&formElt);
  if (FAILED(hr))
    return NULL;

  CComBSTR actionUrl;
  formElt->get_action(&actionUrl);
  COLE2TEX<INTERNET_MAX_URL_LENGTH + 1> actionUrlCStr(actionUrl);

  if (UrlIs(actionUrlCStr, URLIS_URL)) { // XXX Why doesn't URLIS_APPLIABLE work?
    DWORD length;
    UrlGetPart(actionUrlCStr, key, &length, URL_PART_HOSTNAME, NULL);
  }
  else {
    CComQIPtr<IHTMLElement> elt = inputElt;
    IDispatch* disp;
    elt->get_document(&disp);
    if (disp) {
      CComQIPtr<IHTMLDocument2> doc = disp;
      CComPtr<IHTMLLocation> loc;
      doc->get_location(&loc);
      CComBSTR hostname;
      loc->get_hostname(&hostname);
      hostname.ToLower();
      COLE2TEX<INTERNET_MAX_URL_LENGTH+1> host(hostname);
      memcpy(key, host, strlen(host) + 1);
    }
  }
  return key;
}
*/
CComObject<HashField>* Plugin::CreateFieldFor(const char* name, CComQIPtr<IHTMLInputElement>& inputElt, DWORD thread)
{
  window& w = mWindows[thread];
  map<const char*, CComObject<HashField>*, ltstr>& m = w.fields;
  if (m.find(name) == m.end()) {
    CComObject<HashField>* field;
    HRESULT hr = CComObject<HashField>::CreateInstance(&field);
    field->AddRef();
//    const char* actionURL = GetActionURLFor(inputElt), *hashKey;    
//    int hashParams;
//    GetHashInfo(actionURL, hashKey, hashParams);
    field->Init(inputElt, w.handle);
    m.insert(make_pair<const char*, CComObject<HashField>* >(strdup(name), field));
  }
  return GetFieldFor(name, thread);
}

bool Plugin::HandlePwdFieldKeystroke(CComQIPtr<IHTMLInputElement>& inputElt,
									 DWORD key, WPARAM msg, DWORD thread, Plugin::ProtectionType protectionType)
{ 
  CComBSTR name;
  inputElt->get_name(&name);
  COLE2T nameCStr(name); // The macro guards against buffer overflow


  // Create field object here if it doesn't exist already
  CComObject<HashField>* field = GetFieldFor(nameCStr, thread);
  if (!field)
    field = CreateFieldFor(nameCStr, inputElt, thread);

  if(protectionType != Plugin::NO_PROTECTION)
  {
    field->SetContentType(HashField::MASKEDTEXT);
	if(protectionType == Plugin::PASSWORD_KEY_PROTECTION)
	{
		field->put_value("");
	}
	else if(protectionType == Plugin::PASSWORD_PREFIX_PROTECTION)
	{
		CComBSTR masked("");
		for(int i = 0; i < strlen(PASSWORD_PREFIX); i++)
			masked.Append(field->GetMask(PASSWORD_PREFIX[i]));
		field->put_value(masked);
	}
	return false;
  }
	switch(field->GetContentType())
	{
	case HashField::PLAINTEXT:
		return false;
	case HashField::CIPHERTEXT:
		field->put_value(""); 
		field->SetContentType(HashField::PLAINTEXT);
		return false;
	case HashField::MASKEDTEXT:
		char mask = (msg == WM_KEYDOWN) ? field->GetMask((char)key) : 32;
		SimulateKeyPress(mask, msg);
		return true;
	}
	_ASSERT(false);  // cannot determine hash field content type
	return false;
}


const char* Plugin::GetTrailingDomains(const char* completeName, int trailingDomains)
{
	size_t i = 0;
	string name(completeName);
	int dotCount = 0;

  while((i = name.find(".",i)) != string::npos) {
		i++;
		dotCount++;
	}

	i = 0;
	for(; dotCount > trailingDomains - 1; --dotCount) {
		size_t j = name.find(".", i + 1);
    if (j != string::npos)
      i = j;
	}

	if (i)
    i++;

	return completeName + i;
}

void Plugin::GetHashInfo(const char* hostName, const char*& hashKey, int& hashParams)
{
  InitializeRules();

  hashKey = hostName;
  hashParams = DEFAULT_HASH_PARAMS;

  int domains = 2;
  const rule* hashRule = NULL;
  for (vector<rule>::reverse_iterator iter = mRules.rbegin();
       iter != mRules.rend(); ++iter) {
    
    if (iter->pattern) {
      regex_t pattern;
      // XXX Why are ^ and $ required?      
      if (!regcomp(&pattern, iter->pattern, REG_NOSUB | REG_EXTENDED)) {
        if (!regexec(&pattern, hostName, 0, NULL, NULL)) {
          if (iter->key)
 					  hashKey = iter->key;
          domains = iter->domains;
          if (iter->params)
            hashParams = iter->params;
        }
        regfree(&pattern);
			}
		}
  }
  if (!domains)
    hashKey = NULL;
  else
    hashKey = GetTrailingDomains(hostName, domains);    
}
	
bool Plugin::IsPasswordField(const CComPtr<IHTMLInputElement>& elt)
{
  CComBSTR type;
  elt->get_type(&type);
  type.ToLower();
  return type == CComBSTR("password");
}

bool Plugin::IsTextField(const CComPtr<IHTMLInputElement>& elt)
{
  CComBSTR type;
  elt->get_type(&type);
  type.ToLower();
  return !type.ByteLength() || (type == CComBSTR("text"));
}
/*
void Plugin::StorePassword(const char* pwd)
{ 
  pwd = HashManager::Hash(pwd, DEFAULT_HASH_PARAMS);
  mPasswords.insert(pwd);
}

bool Plugin::IsPassword(const char* pwd)
{
  pwd = HashManager::Hash(pwd, DEFAULT_HASH_PARAMS);
  set<const char*, ltstr>::iterator iter = mPasswords.find(pwd);
  free((void*)pwd);
  return iter != mPasswords.end();
}
*/
CToolbar* Plugin::GetToolbar(DWORD thread)
{
  // XXX Does this always get the right thread?
  if (!thread) 
  {
	thread = ::GetWindowThreadProcessId(::GetForegroundWindow(), NULL);
	thread = Plugin::mActiveTabThreads[thread];
  }
  window w = mWindows[thread];
  return w.toolbar;
}

void Plugin::LoadPasswords()
{  
  // LoadAutocompletePasswords();
  char* pwPath = GetPathTo(FILE_PASSWORDS);
  ifstream f(pwPath);
  string pwd;
  while(getline(f, pwd))    
    mPasswords.insert(strdup(pwd.c_str()));
  
  free(pwPath);
  f.close();
}

STDMETHODIMP Plugin::DocumentComplete(IDispatch* pDisp, VARIANT* URL)
{
  if (!mIsEnabled)
    return S_OK;
/*
  // Document finished loading, so hook up our event sinks
  CComQIPtr<IWebBrowser2> browser = pDisp;
  if (browser) {
    IDispatch* disp;
    browser->get_Document(&disp);
    if (disp) {
      CComQIPtr<IHTMLDocument2> doc = disp;
      if (doc)
        CreateHashFields(doc);
    }
  }
*/
  return S_OK;
}
/*
void Plugin::CreateHashFields(const CComPtr<IHTMLDocument2>& doc)
{
  assert(doc);
  CComPtr<IHTMLElementCollection> pCollection;
  if (FAILED(doc->get_all(&pCollection)) || !pCollection)
    return;

  // Iterate through elements
  // Originally we were just iterating through forms,
  // but that misses password fields outside of forms
  long numElts;
  pCollection->get_length(&numElts);
  for (int i = 0; i < numElts; ++i) {
      CComPtr<IDispatch> inputDisp;
      pCollection->item(CComVariant(i), CComVariant(i), &inputDisp);
      if (!inputDisp)
        continue;

      CComQIPtr<IHTMLInputElement> inputElt(inputDisp);
      if (!inputElt)
        continue;
 
      // We only care about password fields
      if (!IsPasswordField(inputElt))
        continue;

      CComBSTR name;
      inputElt->get_name(&name);
      COLE2T nameCStr(name); 

	  // Ignore unnamed form fields
	  if(nameCStr)
	    CreateFieldFor(nameCStr, inputElt, GetCurrentThreadId());    
  }
}
*/
void Plugin::SerializePasswords()
{
  char* pwPath = GetPathTo(FILE_PASSWORDS);
  ofstream f(pwPath, ofstream::trunc);
  for(set<const char*, ltstr>::iterator iter = mPasswords.begin(); iter != mPasswords.end(); ++iter) {
    f << *iter << endl;
  }
  f.close();
}

char* Plugin::TranslateFormData(char* formData)
{
  map<const char*,CComObject<HashField>*, ltstr>& m = mWindows[GetCurrentThreadId()].fields;
  vector<char*> dataFields;
  char *dataField = strtok(formData, "&");
  if(!dataField) return 0;
  do 
  { 
	  dataFields.push_back(dataField); 
  } while(dataField = strtok(NULL, "&"));
  for(vector<char*>::iterator iter = dataFields.begin(); iter != dataFields.end(); ++iter)
  {
    char* name = strtok(*iter, "=");
	char* data = strtok(NULL, "=");
  }
  return NULL;
}
//  bool translated = false;
//  map<const char*,CComObject<HashField>*, ltstr>& m = mWindows[GetCurrentThreadId()].fields;
//  FormDataTranslator translator(m);
//  char* newFormData;
//  if(translator.translate(startPos, &newFormData));
//  free(formData);
//  formData = newFormData;
//  return translated;
//}
/*
  for (map<const char*, CComObject<HashField>*, ltstr>::iterator iter = m.begin(); iter != m.end(); ++iter) {
	const char* fieldName = iter->first;   
    if (char* match = strstr(startPos, fieldName)) { 
      const CComObject<HashField>* fieldObj = iter->second;
      
      // Store (hash of) plain text pwd
      const char* pt = fieldObj->GetPlainText();      
      UrlUnescapeInPlace((LPSTR)pt, 0);
	  if(mIsRecordingPasswords) StorePassword(pt);
      free((void*)pt);

	  // If hashing is disabled, leave this field untouched
      if (!fieldObj->IsHashing()) continue;

      // Got a match, find the equals sign
      char* equals = strchr(match, '=');

	  // If no equals sign, we're confused; abort safely
	  if(!equals) continue;

	  // The ampersand is located at the end of the password
      char* amp = strchr(match, '&');

	  // The null terminator is the position where the next ampersand would be
	  if (!amp) amp = match + strlen(match);

      // Retrieve password
      const char* pwd = fieldObj->GetCipherText();
      DWORD size = strlen(pwd)*2;
      char* escaped = (char*)malloc(size);
      UrlEscape(pwd, escaped, &size, 0);
      free((void*)pwd);
      pwd = escaped;
      
      // Prepare to edit form data
      size_t currLength = amp - equals - 1;     
	  size_t formDataSize = strlen(formData) + 1;
      size_t prefixLength = equals - formData + 1;
	  size_t suffixLength = formDataSize - prefixLength - currLength;
	  size_t pwdLength = strlen(pwd);

	  // Allocate more space
	  startPos = formData = (char*)realloc(formData, formDataSize + pwdLength);
      char* pwdStart = formData + prefixLength;
	  amp = pwdStart + currLength;
	  assert(formData); // Ensure not out of memory...

      // Shift suffix to position it after new password
      memmove(amp + pwdLength - currLength, amp, suffixLength);

      // Copy new password in
      memcpy(pwdStart, pwd, pwdLength);
      translated = true;

      free((void*)pwd);
    }
  }
  return translated;
}
*/

void Plugin::FreePassword(const char* pwd)
{
  free((void*)pwd);
}

void Plugin::FreeRule(rule& r)
{
  if (r.key)
    delete[] r.key;
  
  if (r.pattern)
    delete[] r.pattern;
}

void Plugin::TurnOn()
{
  LoadPasswords();
}

void Plugin::TurnOff()
{
  for_each(mWindows.begin(), mWindows.end(), DestroyWindowFieldsEnum);
  SerializePasswords();
  for_each(mPasswords.begin(), mPasswords.end(), FreePassword);
  for_each(mRules.begin(), mRules.end(), FreeRule);
  if (mGlobalPwd) {
    free((void*)mGlobalPwd);
    mGlobalPwd = NULL;
  }
}

STDMETHODIMP Plugin::OnQuit()
{
  if (!mIsEnabled)
    return S_OK;

  // Check if we're the last window being closed
  const window& w = mWindows[GetCurrentThreadId()];
  HWND next = FindWindowEx(NULL, NULL, "IEFrame", NULL);
  if (next == w.handle)
    next = FindWindowEx(NULL, next, "IEFrame", NULL);
  if (!next)
    TurnOff();
 
  return S_OK;
}

void Plugin::DeleteField(pair<const char*, CComObject<HashField>*> iter)
{
  free((void*)iter.first);
  iter.second->Release();
  iter.second = NULL;
}

void Plugin::DestroyWindowFieldsEnum(pair<DWORD, window> iter)
{
  DestroyWindowFields(iter.second);
}

void Plugin::DestroyWindowFields(window& w)
{
  map<const char*, CComObject<HashField>*, ltstr>& m = w.fields;
  if (m.size()) {
    for_each(m.begin(), m.end(), DeleteField);
    m.clear();
  }
}

char* Plugin::ExtractPostData(VARIANT* postData)
{
  char *szTemp = NULL;
  long plLbound, plUbound;

  SAFEARRAY *parrTemp = postData->pvarVal->parray;
  SafeArrayAccessData(parrTemp, (void HUGEP **) &szTemp);
  if (!szTemp)
    return NULL;

  SafeArrayGetLBound(parrTemp, 1, &plLbound);
  SafeArrayGetUBound(parrTemp, 1, &plUbound);

  char* szPostData = (char*)malloc(plUbound - plLbound + 2);
  StrCpyN(szPostData, szTemp, plUbound - plLbound + 1);
  szPostData[plUbound-plLbound] = '\0';
  SafeArrayUnaccessData(parrTemp);

  return szPostData;
}

bool Plugin::IsFormSubmission(const char* headers, const char* url)
{
  return (headers && (strstr(headers, "Content-Type: application/x-www-form-urlencoded") ||
                      strstr(headers, "Content-Type: multipart/form-data"))) ||
         (url && strchr(url, '?')); // UrlGetPart
}

void Plugin::AssignNewPostData(VARIANT*& oldPostData, const char* newPostData)
{
  VariantClear(oldPostData);
  SafeArrayDestroy(oldPostData->parray);
  oldPostData = new VARIANT;
  oldPostData->vt = VT_ARRAY;

  SAFEARRAYBOUND rgb [] = { (ULONG)strlen(newPostData) + 1, 0 };
  oldPostData->parray = SafeArrayCreate(VT_UI1, 1, rgb);
  char* rgelems;
  SafeArrayAccessData(oldPostData->parray, (void**)&rgelems);
  for (size_t i = 0; i < strlen(newPostData) + 1; ++i)
    rgelems[i] = newPostData[i];
  SafeArrayUnaccessData(oldPostData->parray);      
}

STDMETHODIMP Plugin::WindowStateChanged(DWORD dwFlags, DWORD dwValidFlagsMask)
{
	if ((dwValidFlagsMask & 1) && (dwFlags & 1))
	{
		// The content window is visible to the user
		DWORD thread = GetCurrentThreadId();
		Plugin::mActiveTabThreads[Plugin::mParentThreads[thread]] = thread;
		return S_OK;	
	}

	return S_OK;	
}

STDMETHODIMP Plugin::BeforeNavigate2(IDispatch *pDisp, VARIANT *URL, VARIANT *Flags,
                                     VARIANT *TargetFrameName, VARIANT *PostData,
                                     VARIANT *Headers, VARIANT_BOOL *Cancel)
{
  if (!mIsEnabled)
    return S_OK;

  // Avoid intercepting Navigate2 a second time if it's already been intercepted once
  vector<CComBSTR>::iterator pend = find(mPending.begin(), mPending.end(), URL->bstrVal);  
  if (pend != mPending.end()) 
  {
    mPending.erase(pend);    
    return S_OK;
  }  

  CComBSTR headers = Headers->bstrVal;
  CComBSTR url = URL->bstrVal;
  COLE2T headerCStr(headers);
  COLE2TEX<INTERNET_MAX_URL_LENGTH> urlCStr(url);

  bool dirty = false;		// Determine if any data needs to be changed
  if(IsFormSubmission(headerCStr, urlCStr))
	  dirty = ProcessPostData(PostData) || ProcessGetData(URL);

  // Changing pages or quitting, so disconnect old event sinks (delete old textboxes)
  window& w = mWindows[GetCurrentThreadId()];  
  DestroyWindowFields(w);
 
  if(dirty)
  {   
    *Cancel = VARIANT_TRUE; // Issue our own Navigate2 event with modified data
	mPending.push_back(URL->bstrVal);
	CComQIPtr<IWebBrowser2> browser = pDisp;
	return browser->Navigate2(URL, Flags, TargetFrameName, PostData, Headers);
  }
  else return S_OK;        // Allow Navigate2 event to go through normally
}

bool Plugin::ProcessPostData(VARIANT *PostData)
{
  char* data = ExtractPostData(PostData);
  if (data) {
    char* newData;
    if (strlen(data) && (newData = TranslateFormData(data))) 
	{
      AssignNewPostData(PostData, newData);
	  free(newData);
      free(data);
	  return true;
    }
	free(data);
  }
  return false;
}

bool Plugin::ProcessGetData(VARIANT* URL)
{
  COLE2TEX<INTERNET_MAX_URL_LENGTH> urlCStr(URL->bstrVal);
  char* pszUrlStart = strdup(urlCStr);
  if(pszUrlStart)
  {
	char* data = strchr(pszUrlStart, '?');
	if (data) 
	{
		*data = '\0';   // Separate query from URL
		char* newData;
		if (strlen(data) && (newData = TranslateFormData(data + 1)))
		{
			CComBSTR bstrFullUrl(pszUrlStart);
			bstrFullUrl.Append("?");
			bstrFullUrl.Append(newData);
			URL->bstrVal = bstrFullUrl;
			free(newData);
			free(pszUrlStart);
			return true;
		}
	}
  free(pszUrlStart);
  }
  return false;
}



/*
void Plugin::LoadAutocompletePasswords()
{
  typedef HRESULT (WINAPI *tPStoreCreateInstance)(IPStore **, DWORD, DWORD, DWORD);
  HMODULE hpsDLL; 
  hpsDLL = LoadLibrary("pstorec.dll");

  tPStoreCreateInstance pPStoreCreateInstance;
  pPStoreCreateInstance = (tPStoreCreateInstance)GetProcAddress(hpsDLL, "PStoreCreateInstance");

  IPStorePtr PStore; 
  HRESULT hRes = pPStoreCreateInstance(&PStore, 0, 0, 0); 

  IEnumPStoreTypesPtr EnumPStoreTypes;
  hRes = PStore->EnumTypes(0, 0, &EnumPStoreTypes);

  if (!FAILED(hRes)) {
	  GUID TypeGUID;
    char szItemName[512];       
    char szItemData[512];
    char szResName[1512];
    char szResData[512];
    char szItemGUID[50];

     while(EnumPStoreTypes->raw_Next(1,&TypeGUID,0) == S_OK){   

	     wsprintf(szItemGUID,"%x",TypeGUID);
       if(lstrcmp(szItemGUID,"e161255a")) continue;
	     IEnumPStoreTypesPtr EnumSubTypes;
       hRes = PStore->EnumSubtypes(0, &TypeGUID, 0, &EnumSubTypes);
	  
	     GUID subTypeGUID;
	     while(EnumSubTypes->raw_Next(1,&subTypeGUID,0) == S_OK){
         IEnumPStoreItemsPtr spEnumItems;
		     HRESULT hRes = PStore->EnumItems(0, &TypeGUID, &subTypeGUID, 0, &spEnumItems);

  		   LPWSTR itemName;
	  	   while(spEnumItems->raw_Next(1,&itemName,0) == S_OK){             
		  	   wsprintf(szItemName,"%ws",itemName);			 
			    	char chekingdata[200];
			      unsigned long psDataLen = 0;
            unsigned char *psData = NULL;
            _PST_PROMPTINFO *pstiinfo = NULL;
			      hRes = PStore->ReadItem(0,&TypeGUID,&subTypeGUID,itemName,&psDataLen,&psData,pstiinfo,0);
			      if(lstrlen((char *)psData)<(psDataLen-1)) {
				      int i=0;
				      for(int m=0;m<psDataLen;m+=2){
					      if(psData[m]==0)
				  		    szItemData[i]=',';
				  	    else
				  	      szItemData[i]=psData[m];
				  	    i++;
				      }
				      szItemData[i-1]=0;				  			
			      }
			      else {		  				  
  				    wsprintf(szItemData,"%s",psData);				  
	  		    }	
		  	    lstrcpy(szResName,"");
			      lstrcpy(szResData,"");

	  		   
		  	      if(strstr(szItemName,"StringIndex")==0){
			          if(strstr(szItemName,":String")!=0) *strstr(szItemName,":String")=0;			  
			          if((StrCmpN(szItemName,"http:/", 6))&&(StrCmpN(szItemName,"https:/", 7)))
			            StorePassword(chekingdata);
    		        else{
		              lstrcpy(chekingdata,"");
				          if(strstr(szItemData,",")!=0){
  					        lstrcpy(chekingdata,strstr(szItemData,",")+1);
	  				        *(strstr(szItemData,","))=0;				  
		  		        }
			            StorePassword(chekingdata);
			          }
			        }
			      ZeroMemory(szItemName,sizeof(szItemName));
			      ZeroMemory(szItemData,sizeof(szItemData));			  
		      }		  
	      }
      }
   }    
}
*/

