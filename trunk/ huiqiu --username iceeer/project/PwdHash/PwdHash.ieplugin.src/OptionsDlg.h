#pragma once

#include "resource.h" 
#include <atlhost.h>
#include <windows.h>
#include "Toolbar.h"

class COptionsDlg : 
	public CAxDialogImpl<COptionsDlg>
{
public:
	COptionsDlg()
	{
	}

	~COptionsDlg()
	{
	}

	enum { IDD = IDD_OPTIONS };

BEGIN_MSG_MAP(COptionsDlg)
	MESSAGE_HANDLER(WM_INITDIALOG, OnInitDialog)
	COMMAND_HANDLER(IDOK, BN_CLICKED, OnClickedOK)
	COMMAND_HANDLER(IDCANCEL, BN_CLICKED, OnClickedCancel)
  COMMAND_HANDLER(IDC_USE_GLOBAL_PASSWORD, BN_CLICKED, OnBnClickedUseGlobalPassword)
  COMMAND_HANDLER(IDC_ENABLE_PASSWORD_HASHING, BN_CLICKED, OnBnClickedEnablePasswordHashing)
  COMMAND_HANDLER(IDHELP, BN_CLICKED, OnBnClickedHelp)
  COMMAND_HANDLER(IDRULES, BN_CLICKED, OnBnClickedRules)
  CHAIN_MSG_MAP(CAxDialogImpl<COptionsDlg>)
END_MSG_MAP()

// Handler prototypes:
//  LRESULT MessageHandler(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled);
//  LRESULT CommandHandler(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled);
//  LRESULT NotifyHandler(int idCtrl, LPNMHDR pnmh, BOOL& bHandled);

	LRESULT OnInitDialog(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled);
	LRESULT OnClickedOK(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled);
	LRESULT OnClickedCancel(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled);
  LRESULT OnBnClickedUseGlobalPassword(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled);
  LRESULT OnBnClickedEnablePasswordHashing(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled);
  LRESULT OnBnClickedHelp(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled);
  LRESULT OnBnClickedRules(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled);
};


