#pragma once
#include "resource.h"
#include <exdispid.h>
#include "KeystrokeChecker.h"
#include "HashField.h"
#include <vector>
#include <set>
#include <map>
#include "Toolbar.h"

// Should be in exdispid.h, but missing for some reason
#define DISPID_WINDOWSTATECHANGED           283

#define KEY_TOGGLE_PROTECT	VK_F2

#define PASSWORD_PREFIX "@@"

struct ltstr
{
  bool operator()(const char* s1, const char* s2) const
  {
    return strcmp(s1, s2) < 0;
  }
};

class window {
  friend class Plugin;
  map<const char*, CComObject<HashField>*, ltstr> fields;
  CToolbar* toolbar;
  HWND handle;
  CComQIPtr<IWebBrowser2> browser;

public:
  window():toolbar(NULL), handle(0) { }
};

[
  object,
  uuid("B737E20D-51A2-4F6F-BD8D-8E905A78F408"),
  dual,  helpstring("IPlugin Interface"),
  pointer_default(unique)
]
__interface IPlugin : IDispatch
{
};

[
  coclass,
  threading("apartment"),
  aggregatable("never"),
  vi_progid("PwdHash.Plugin"),
  progid("PwdHash.Plugin.1"),
  version(1.0),
  uuid("EC41BF0A-01A1-4A85-9265-A074875305B5"),
  helpstring("Plugin Class")
]
class ATL_NO_VTABLE Plugin :
  public IObjectWithSiteImpl<Plugin>,
  public IDispatchImpl<IPlugin>,
  public IObjectSafety,
  public IDispEventImpl<0, Plugin, &__uuidof(/*SHDocVw::*/DWebBrowserEvents2), &LIBID_SHDocVw, 1, 0>
{
public:

  typedef IDispEventImpl<0, Plugin, &__uuidof(/*SHDocVw::*/DWebBrowserEvents2), &LIBID_SHDocVw, 1, 0> theDispEvent;

  Plugin()
  {
  }

  DECLARE_PROTECT_FINAL_CONSTRUCT()

  HRESULT FinalConstruct()
  {
    return S_OK;
  }
  
  void FinalRelease() 
  {    
  }

  STDMETHOD(SetSite)(IUnknown *pUnkSite);
   
  STDMETHODIMP GetInterfaceSafetyOptions(REFIID riid, DWORD* pdwSupportedOptions, DWORD* pdwEnabledOptions)
  {
    *pdwSupportedOptions = INTERFACESAFE_FOR_UNTRUSTED_CALLER |
                           INTERFACESAFE_FOR_UNTRUSTED_DATA;
    *pdwEnabledOptions = *pdwSupportedOptions;
    return S_OK;
  }

  STDMETHODIMP SetInterfaceSafetyOptions(REFIID riid, DWORD dwOptionSetMask, DWORD dwEnabledOptions)
  {
    return S_OK;
  }

  BEGIN_SINK_MAP(Plugin)
    SINK_ENTRY_EX(0, (__uuidof(DWebBrowserEvents2)), DISPID_BEFORENAVIGATE2, BeforeNavigate2)
	  SINK_ENTRY_EX(0, (__uuidof(DWebBrowserEvents2)), DISPID_ONQUIT, OnQuit)
    SINK_ENTRY_EX(0, (__uuidof(DWebBrowserEvents2)), DISPID_DOCUMENTCOMPLETE, DocumentComplete)
    SINK_ENTRY_EX(0, (__uuidof(DWebBrowserEvents2)), DISPID_WINDOWSTATECHANGED, WindowStateChanged)
  END_SINK_MAP()

  BEGIN_COM_MAP(Plugin)
    COM_INTERFACE_ENTRY(IPlugin)
    COM_INTERFACE_ENTRY(IObjectWithSite)
    COM_INTERFACE_ENTRY(IDispatch)
	COM_INTERFACE_ENTRY(IObjectSafety)
  END_COM_MAP()

  static bool IsAdvancedMode();
  static bool IsEnabled();
  static bool IsUsingGlobalPwd();
  static bool IsRecordingPasswords();
  static void SetRecordingPasswords(bool value);
  static const char* GetGlobalPwd();
  
  static bool IsPasswordField(const CComPtr<IHTMLInputElement>& elt);
  static bool IsTextField(const CComPtr<IHTMLInputElement>& elt);
  
  // static bool IsPassword(const char* pwd);
  
  static void RegisterToolbar(CToolbar* toolbar);
  static void UnregisterToolbar();
  static CToolbar* GetToolbar(DWORD thread = NULL);

  static CComObject<HashField>* GetFieldFor(const char* name, DWORD thread = NULL);
  static CComObject<HashField>* GetField();

  static bool HandleKeystroke(WPARAM wParam, LPARAM lParam);

  static const int MAX_PASSWORD_LENGTH = 26;

  static const char* helpURL;

  enum PluginFile { FILE_RULES, FILE_PASSWORDS };
  static char* GetPathTo(PluginFile file);

  static void SimulateKeyPress(char key, WPARAM msg);
  static void ToggleCaps();
  static void SetControlKeyState(bool down);

  static void GetHashInfo(const char* hostName, const char*& hashKey, int& hashParams);
private:
  static bool mIsAdvancedMode;
  static bool mUseGlobalPwd;
  static bool mIsEnabled;
  static bool mIsRecordingPasswords;
  static bool mRulesInitialized;
  static const char* mGlobalPwd;
  static HANDLE mHQuitEvent;  
  static set<const char*, ltstr> mPasswords;
  static map<DWORD, window> mWindows;
  static map<DWORD, DWORD> mParentThreads;
  static map<DWORD, DWORD> mActiveTabThreads;
  static vector<rule> mRules;

  enum RegValue { eMode, eEnabled };
  static void Notify(RegValue reg);

  static DWORD WINAPI RegNotify(LPVOID evt);

  static const vector<struct rule>& GetRules();

  // static void StorePassword(const char* pwd);
  static void LoadPasswords();
  static void LoadAutocompletePasswords();
  static void SerializePasswords();
  static void FreePassword(const char* pwd);

  // static void CreateHashFields(const CComPtr<IHTMLDocument2>& doc);
  static CComObject<HashField>* CreateFieldFor(const char* name, CComQIPtr<IHTMLInputElement>& inputElt, DWORD thread);
  static void DeleteField(pair<const char*, CComObject<HashField>*> iter);
  static void DestroyWindowFields(window& w);
  static void DestroyWindowFieldsEnum(pair<DWORD, window> iter);

  static void InitializeRegistry();


  enum ProtectionType { NO_PROTECTION, PASSWORD_KEY_PROTECTION, PASSWORD_PREFIX_PROTECTION };
  static bool ShouldHandleKeystroke(KBDLLHOOKSTRUCT* kb, WPARAM message, ProtectionType &protectionType);
  static bool HandlePwdFieldKeystroke(CComQIPtr<IHTMLInputElement>& inputElt, DWORD key, WPARAM msg, DWORD thread, ProtectionType protectionType);
  static bool HandleTextFieldKeystroke(CComQIPtr<IHTMLInputElement>& inputElt, WPARAM msg, HWND focusedWnd);
  static bool GetActiveElement(CComPtr<IDispatch>& pDispatch, CComQIPtr<IHTMLInputElement>& pActiveElement);

  static const char* ReadConfig();
  static void AddToRule(rule* r, const char* tokenName, size_t tokenNameLen,
                        const char* tokenValue, size_t tokenValueLen);
  // static const char* GetActionURLFor(CComQIPtr<IHTMLInputElement>& inputElt);
  static const int DEFAULT_HASH_PARAMS = 8101;
  static void InitializeRules();
  static void FreeRule(rule& r);
  static const char* GetTrailingDomains(const char* completeName, int numDomains);

  static bool IsFormSubmission(const char* headers, const char* url);
  static char* ExtractPostData(VARIANT* postData);
  static void AssignNewPostData(VARIANT*& oldPostData, const char* newPostData);
  static char* TranslateFormData(char* formData);
  static bool ProcessPostData(VARIANT* PostData);
  static bool ProcessGetData(VARIANT* URL);

  static void TurnOn();
  static void TurnOff();

  static DWORD WINAPI NonpasswordFieldWarning(LPVOID pParam);

  STDMETHOD(OnQuit)();
  STDMETHOD(DocumentComplete)(IDispatch* pDisp, VARIANT* URL);
  STDMETHOD(BeforeNavigate2)(IDispatch *pDisp, VARIANT *URL, VARIANT *Flags, 
                             VARIANT *TargetFrameName, VARIANT *PostData,
                             VARIANT *Headers, VARIANT_BOOL *Cancel);
  STDMETHOD(WindowStateChanged)(DWORD dwFlags, DWORD dwValidFlagsMask);
  
  vector<CComBSTR> mPending;   

};

