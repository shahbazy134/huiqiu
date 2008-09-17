#pragma once

#include "resource.h" 
#include <atlhost.h>
#include "HashField.h"

class StatusDlg : 
	public CAxDialogImpl<StatusDlg>
{
public:
  StatusDlg(CComObject<HashField>* field):mField(field)
	{
	}

	~StatusDlg()
	{
	}

	enum { IDD = IDD_STATUSDLG };

BEGIN_MSG_MAP(StatusDlg)
	MESSAGE_HANDLER(WM_INITDIALOG, OnInitDialog)
  COMMAND_HANDLER(IDCANCEL, BN_CLICKED, OnClickedCancel)
	CHAIN_MSG_MAP(CAxDialogImpl<StatusDlg>)
END_MSG_MAP()

// Handler prototypes:
//  LRESULT MessageHandler(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled);
//  LRESULT CommandHandler(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled);
//  LRESULT NotifyHandler(int idCtrl, LPNMHDR pnmh, BOOL& bHandled);

  LRESULT OnInitDialog(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
  {
    const char* key = NULL;
    if (mField) {
      SetDlgItemText(IDC_PASSWORD_YESNO, _T("Yes"));
	  bool isHashing = mField->GetContentType() == HashField::CIPHERTEXT;
      SetDlgItemText(IDC_HASHING_YESNO, isHashing ? _T("Yes") : _T("No"));
      key = mField->GetHashKey();
    }

    if (!key)
      key = "(none)";
      
    SetDlgItemText(IDC_DOMAIN, key);      
    
  	CAxDialogImpl<StatusDlg>::OnInitDialog(uMsg, wParam, lParam, bHandled);
  	return 1;  // Let the system set the focus
  }

	LRESULT OnClickedCancel(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled)
	{
		EndDialog(wID);
		return 0;
	}
private:
  CComObject<HashField>* mField;
};


