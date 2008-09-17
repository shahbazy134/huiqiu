#include "stdafx.h"
#include "resource.h"

HINSTANCE g_hinstPub = NULL;

// XXX Buffer overflows
// XXX MT-Safety

// The module attribute causes DllMain, DllRegisterServer and DllUnregisterServer to be automatically implemented for you
[ module(dll, uuid = "{0E3DB2B9-AA93-413A-A235-A721A3D922BA}", 
     name = "pwdhash", 
     helpstring = "PwdHash Internet Explorer Plugin",
     resource_name = "IDR_PWDHASH") ]

class PluginModule
{
// Override CAtlDllModuleT members
public:
  BOOL WINAPI DllMain(DWORD dwReason, LPVOID lpReserved) 
  {
    void* data;
    switch(dwReason) {
      case DLL_PROCESS_ATTACH: {
        // Don't attach to Windows Explorer
        TCHAR pszLoader[MAX_PATH];
        GetModuleFileName(NULL, pszLoader, MAX_PATH);
        CAtlString sLoader = pszLoader;
        sLoader.MakeLower();
        if (sLoader.Find(_T("explorer.exe")) >= 0)
          return FALSE;

        g_hinstPub = _AtlBaseModule.m_hInst;
      }
    }
    return __super::DllMain(dwReason, lpReserved);
  }
  
  HRESULT DllRegisterServer(BOOL bRegTypeLib = TRUE) throw()
  {
    HRESULT hr = __super::DllRegisterServer(bRegTypeLib);
    if (SUCCEEDED(hr))
      hr = __super::UpdateRegistryFromResourceS(IDR_PWDHASH, TRUE);
    return hr;
  }
};