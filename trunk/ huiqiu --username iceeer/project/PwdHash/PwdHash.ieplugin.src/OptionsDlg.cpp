#include "stdafx.h"
#include "OptionsDlg.h"
#include "Plugin.h"
#include ".\optionsdlg.h"

LRESULT COptionsDlg::OnInitDialog(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
  bool disableAll = false;
  if (Plugin::IsEnabled())
    CheckDlgButton(IDC_ENABLE_PASSWORD_HASHING, true);
  else {
    disableAll = true;
    ::EnableWindow(GetDlgItem(IDC_GLOBAL_PASSWORD), false);
    ::EnableWindow(GetDlgItem(IDC_USE_GLOBAL_PASSWORD), false);
    ::EnableWindow(GetDlgItem(IDC_EXCEPTIONS_TREE), false);
    ::EnableWindow(GetDlgItem(IDRULES), false);
  }

  if (Plugin::IsUsingGlobalPwd()) {
    CheckDlgButton(IDC_USE_GLOBAL_PASSWORD, true);
    if (!disableAll)
      ::EnableWindow(GetDlgItem(IDC_GLOBAL_PASSWORD), true);          
  }

  if (const char* global = Plugin::GetGlobalPwd())
    SetDlgItemText(IDC_GLOBAL_PASSWORD, (LPCTSTR)global);
  
  if (Plugin::IsAdvancedMode())
    CheckDlgButton(IDC_F2_MODE, true);

  if (Plugin::IsRecordingPasswords())
	  CheckDlgButton(IDC_RECORD_PASS, true);

	CAxDialogImpl<COptionsDlg>::OnInitDialog(uMsg, wParam, lParam, bHandled);
	return 1;  // Let the system set the focus
}

LRESULT COptionsDlg::OnClickedOK(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled)
{
  // Save settings
  HKEY key;
  DWORD res;
  HRESULT hr = RegCreateKeyEx(HKEY_CURRENT_USER, TEXT("Software\\PwdHash"),
                              0, NULL, REG_OPTION_NON_VOLATILE, KEY_SET_VALUE, 0, &key, &res);
  if (SUCCEEDED(hr)) {
    DWORD enabled = (IsDlgButtonChecked(IDC_ENABLE_PASSWORD_HASHING) == BST_CHECKED);
    hr = RegSetValueEx(key, TEXT("Enabled"), 0, REG_DWORD, (const BYTE*)&enabled, sizeof(enabled));

    HKEY toolbarKey;
    hr = RegCreateKeyEx(HKEY_LOCAL_MACHINE, TEXT("SOFTWARE\\Microsoft\\Internet Explorer\\Toolbar"),
                        0, NULL, REG_OPTION_NON_VOLATILE, KEY_SET_VALUE, 0, &toolbarKey, &res);
    if (SUCCEEDED(hr)) {
      LPOLESTR lpolestr;
      StringFromCLSID(__uuidof(CToolbar), &lpolestr);
      COLE2TEX<100> guid(lpolestr);
      if (enabled)
        RegSetValueEx(toolbarKey, guid, 0, REG_SZ, (const BYTE*)"", 0);          
      else
        RegDeleteValue(toolbarKey, guid);
    }

    DWORD useGlobal = (IsDlgButtonChecked(IDC_USE_GLOBAL_PASSWORD) == BST_CHECKED);
    hr = RegSetValueEx(key, TEXT("UseGlobalPassword"), 0, REG_DWORD, (const BYTE*)&useGlobal, sizeof(useGlobal));

    char global[50];
    int size = GetDlgItemText(IDC_GLOBAL_PASSWORD, (LPTSTR)global, sizeof(global));
    hr = RegSetValueEx(key, TEXT("GlobalPassword"), 0, REG_SZ, (const BYTE*)global, (DWORD)strlen(global) + 1);

	DWORD f2Mode = (IsDlgButtonChecked(IDC_F2_MODE) == BST_CHECKED);
    hr = RegSetValueEx(key, TEXT("F2Mode"), 0, REG_DWORD, (const BYTE*)&f2Mode, sizeof(f2Mode));

    DWORD recordPasswords = (IsDlgButtonChecked(IDC_RECORD_PASS) == BST_CHECKED);
    hr = RegSetValueEx(key, TEXT("RecordPasswords"), 0, REG_DWORD, (const BYTE*)&recordPasswords, sizeof(recordPasswords));
	Plugin::SetRecordingPasswords(recordPasswords);

	RegCloseKey(key);
  }

	EndDialog(wID);
	return 0;
}

LRESULT COptionsDlg::OnClickedCancel(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled)
{
	EndDialog(wID);
	return 0;
}

LRESULT COptionsDlg::OnBnClickedUseGlobalPassword(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled)
{    
  LRESULT checked = SendMessage(hWndCtl, BM_GETCHECK, 0, 0);
  ::EnableWindow(GetDlgItem(IDC_GLOBAL_PASSWORD), checked == BST_CHECKED);
  return 0;
}
LRESULT COptionsDlg::OnBnClickedEnablePasswordHashing(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled)
{
  HWND global = GetDlgItem(IDC_USE_GLOBAL_PASSWORD);
  bool checked = (SendMessage(hWndCtl, BM_GETCHECK, 0, 0) == BST_CHECKED);
  
  ::EnableWindow(global, checked == BST_CHECKED);
  ::EnableWindow(GetDlgItem(IDC_ENTER_PASSWORDS_ON_TOOLBAR), checked == BST_CHECKED);
  ::EnableWindow(GetDlgItem(IDRULES), checked == BST_CHECKED);
  if (checked) {    
    bool globalChecked = (SendMessage(global, BM_GETCHECK, 0, 0) == BST_CHECKED);
    if (globalChecked)
      ::EnableWindow(GetDlgItem(IDC_GLOBAL_PASSWORD), true);
  }
  else {
    ::EnableWindow(GetDlgItem(IDC_GLOBAL_PASSWORD), false);
  }
  return 0;
}

LRESULT COptionsDlg::OnBnClickedHelp(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled)
{
  ShellExecute(NULL, "open", Plugin::helpURL, NULL, NULL, SW_SHOWNORMAL);
  return 0;
}

LRESULT COptionsDlg::OnBnClickedRules(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled)
{
  char* rulesPath = Plugin::GetPathTo(Plugin::FILE_RULES);
  ShellExecute(NULL, "open", "notepad.exe", rulesPath, NULL, SW_SHOWNORMAL);
  free(rulesPath);
  return 0;
}

