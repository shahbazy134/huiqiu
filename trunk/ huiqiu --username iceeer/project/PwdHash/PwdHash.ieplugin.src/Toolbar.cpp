#include "stdafx.h"
#include "Toolbar.h"
#include <windows.h>
#include "Plugin.h"
#include "StatusDlg.h"

extern HINSTANCE g_hinstPub;

const DWORD DEFAULT_TOOLBAR_STYLE =
  WS_CHILD | WS_CLIPSIBLINGS | WS_VISIBLE | WS_TABSTOP  |     // Window styles
  TBSTYLE_TOOLTIPS | TBSTYLE_FLAT | TBSTYLE_TRANSPARENT |     // Toolbar styles
  TBSTYLE_LIST | TBSTYLE_WRAPABLE | TBSTYLE_CUSTOMERASE |
  CCS_TOP | CCS_NODIVIDER | CCS_NOPARENTALIGN | CCS_NORESIZE; // Common control styles

void CToolbar::OnPwdFieldFocus(HashField* field)
{
  const char* text = field->GetMaskedText();
  if (text) {
    SendMessage(mEditHandle, WM_SETTEXT, 0, (LPARAM)text);
    SendMessage(mEditHandle, EM_SETSEL, strlen(text), strlen(text));
    // SendMessage(mEditHandle, EM_SETSEL, 0, strlen(text));
    free((void*)text);
  }

  mField = field;
  EnableWindow(mEditHandle, TRUE);
  SetFocus(mEditHandle);
} 

STDMETHODIMP CToolbar::SetSite(IUnknown *pUnkSite)
{  
  if (mInputSite) {
    mInputSite->Release();
    mInputSite = NULL;
  }

  if (pUnkSite) {
    IOleWindow* oleWin;
    if(SUCCEEDED(pUnkSite->QueryInterface(IID_IOleWindow, (LPVOID*)&oleWin))) {
      HWND parent;
      oleWin->GetWindow(&parent);
      oleWin->Release();
      
      INITCOMMONCONTROLSEX x;
      x.dwSize = sizeof(INITCOMMONCONTROLSEX);
      x.dwICC =  ICC_BAR_CLASSES;
      InitCommonControlsEx(&x);  
      
      if (!RegisterAndCreateWindow(&parent) || !PopulateToolbar())
        return E_FAIL;
    }

    // Get and keep the IInputObjectSite pointer.
    pUnkSite->QueryInterface(IID_IInputObjectSite, (LPVOID*)&mInputSite);
  }
  else {
    ImageList_Destroy(mImageList);
    DestroyWindow(mHandle);
    Plugin::UnregisterToolbar();
  }

  return S_OK;
}

bool CToolbar::PopulateToolbar()
{
  SendMessage(mHandle, TB_BUTTONSTRUCTSIZE, sizeof(TBBUTTON), 0);

  // Set the maximum number of text rows and bitmap size.
  SendMessage(mHandle, TB_SETMAXTEXTROWS, 1, 0);

  // add our button caption to the toolbar window
  LRESULT iIndex = SendMessage(mHandle, TB_ADDSTRING, NULL, (LPARAM) _T(""));

  // load our button icon and create the image list to house it.
  HICON hGreenLightIcon = LoadIcon(GetModuleHandle("pwdhash.dll"), MAKEINTRESOURCE(IDI_ICONGREEN));
  HICON hRedLightIcon = LoadIcon(GetModuleHandle("pwdhash.dll"), MAKEINTRESOURCE(IDI_ICONRED));
  mImageList = ImageList_Create(SIGNAL_DIM, SIGNAL_DIM, ILC_COLOR16, 2, 2);
  ImageList_AddIcon(mImageList, hGreenLightIcon); // XXX Cleaner to store returned index instead of assuming 0, 1, ...?
  ImageList_AddIcon(mImageList, hRedLightIcon);
  DestroyIcon(hGreenLightIcon);
  DestroyIcon(hRedLightIcon);

  SendMessage(mHandle, TB_SETIMAGELIST, 0, (LPARAM) mImageList);
  
  TBBUTTON Button;
  ZeroMemory(&Button, sizeof Button);
  Button.idCommand = IDM_STATUSBUTTON;
  Button.fsState = TBSTATE_ENABLED;
  Button.fsStyle = TBSTYLE_AUTOSIZE | TBSTYLE_BUTTON/* | BTNS_SHOWTEXT*/;
  Button.dwData = 0;
  Button.iString = iIndex;
  Button.iBitmap = SIGNAL_RED;
  SendMessage(mHandle, TB_INSERTBUTTON, 0, (LPARAM) &Button);
  
  // XXX Use manifest file to get visual style (e.g. black circles instead of asteriks)
  
  CreateProxyPwdField();
  return true;
}

bool CToolbar::RegisterAndCreateWindow(HWND* parent)
{
  // If the window doesn't exist yet, create it now.
  // Can't create a child window without a parent.
  if (!mHandle && parent) {
    RECT rc;
    GetClientRect(*parent, &rc);

    WNDCLASS wc;
    wc.hIcon          = NULL;
    wc.hCursor        = LoadCursor(NULL, IDC_ARROW);
    wc.hbrBackground  = (HBRUSH)CreateSolidBrush(RGB(0, 0, 192));
    wc.lpszMenuName   = NULL;
    wc.lpfnWndProc    = (WNDPROC) CToolbar::WndProc; 
    wc.hInstance      = g_hinstPub; 
    wc.lpszClassName  = "ReflectionWnd"; 
    wc.style = wc.cbClsExtra = wc.cbWndExtra = 0;
    RegisterClass(&wc);    

    mReflectionHandle = CreateWindowEx(WS_EX_TRANSPARENT, "ReflectionWnd", "", WS_CHILD | WS_CLIPSIBLINGS | WS_TABSTOP,
                                       0, 0, 0, 0, *parent, NULL, g_hinstPub, (LPVOID)this);
   
    // Create the toolbar window. XXX Have WndProc set handle in WM_NCCREATE?
    mHandle = CreateWindowEx(0, TOOLBARCLASSNAME, NULL, DEFAULT_TOOLBAR_STYLE,
                             0, 0, 0, 0, // These are all ignored
                             mReflectionHandle, NULL, g_hinstPub, NULL); 

    Plugin::RegisterToolbar(this);
  }
  
  return (mHandle != 0);
}

void CToolbar::CreateProxyPwdField()
{
  long height = HIWORD(SendMessage(mHandle, TB_GETBUTTONSIZE, 0, 0));
  mEditHandle = CreateWindowEx(WS_EX_CLIENTEDGE,
                              "Edit",
                              NULL,
                              WS_CHILD | WS_TABSTOP | WS_BORDER |
                              ES_AUTOHSCROLL | ES_LEFT | ES_PASSWORD,
                              SIGNAL_DIM+PWD_FIELD_OFFSET, 0, PWD_FIELD_WIDTH, height,
                              mHandle, (HMENU)IDC_EDIT, g_hinstPub, (LPVOID)this);
  SetWindowLongPtr(mEditHandle, GWL_USERDATA, (LONG_PTR)this); 
  mOldEditProc = (WNDPROC)SetWindowLongPtr(mEditHandle, GWL_WNDPROC, (LONG_PTR)CToolbar::WndProc);

  SendMessage(mEditHandle, WM_SETFONT, (WPARAM)GetStockObject(DEFAULT_GUI_FONT), MAKELPARAM(FALSE, 0));
  EnableWindow(mEditHandle, FALSE);
}

void CToolbar::OnEnabledChange(bool isEnabled)
{
  ShowDW(isEnabled);
}

/*
void CToolbar::OnModeChange(bool isAdvanced)
{
  ShowWindow(mEditHandle, isAdvanced ? SW_SHOW : SW_HIDE);
  SendMessage(mHandle, TB_AUTOSIZE, 0, 0);
}
*/

STDMETHODIMP CToolbar::GetBandInfo(DWORD dwBandID, DWORD dwViewMode, DESKBANDINFO* pdbi)
{  
  if (pdbi->dwMask & DBIM_MINSIZE) {
    LRESULT size = SendMessage(mHandle, TB_GETBUTTONSIZE, 0, 0);
    long width = 28, height = HIWORD(size); // XXX Should use LOWORD(size) for width but the value is too large...
//    if (Plugin::IsAdvancedMode())
//      width += PWD_FIELD_OFFSET + PWD_FIELD_WIDTH;
    pdbi->ptMinSize.x = width;
    pdbi->ptMinSize.y = height;
  }

  if (pdbi->dwMask & DBIM_MAXSIZE) {
    pdbi->ptMaxSize.x = 0;  // ignored
    pdbi->ptMaxSize.y = -1; // no maximum height limit
  }

  if (pdbi->dwMask & DBIM_ACTUAL) {
    pdbi->ptActual.x = 250;
    pdbi->ptActual.y = 22;
  }
 
  if (pdbi->dwMask & DBIM_TITLE)
    wcscpy(pdbi->wszTitle, L""); // Don't display text in the toolbar, keep it slim
 
  if (pdbi->dwMask & DBIM_MODEFLAGS)
    pdbi->dwModeFlags = DBIMF_NORMAL;

  if (pdbi->dwMask & DBIM_BKCOLOR) {
    pdbi->dwMask &= ~DBIM_BKCOLOR;
	  pdbi->crBkgnd = RGB(255,0,0);
	  pdbi->crBkgnd = 0x7CFC00;
  }	
  return S_OK;
}

STDMETHODIMP CToolbar::ShowDW(BOOL bShow)
{
  if (mHandle)
    ShowWindow(mHandle, bShow ? SW_SHOW : SW_HIDE);

  return S_OK;
}

STDMETHODIMP CToolbar::CloseDW(DWORD dwReserved)
{
  ShowDW(FALSE);
  if (IsWindow(mHandle))
    DestroyWindow(mHandle);

  mHandle = NULL;
  return S_OK;
}

STDMETHODIMP CToolbar::UIActivateIO(BOOL bActivate, LPMSG pMsg)
{
  if (bActivate)
    SetFocus(mHandle);
  return S_OK;
}

STDMETHODIMP CToolbar::TranslateAcceleratorIO(LPMSG pMsg)
{
  // Ignore if CTRL or ALT are pressed so system menu and IE's accelerators
  // work (this isn't perfect since things like CTRL+<- should probably work in our
  // field... XXX Alt+Space doesn't work
  if ((GetKeyState(VK_CONTROL) & 0x80) || (GetKeyState(VK_MENU) & 0x80))
    return S_FALSE;
  
  if (pMsg->message == WM_KEYDOWN && pMsg->wParam == VK_TAB)
    return S_FALSE;

  pMsg->hwnd = mEditHandle;
  TranslateMessage(pMsg);
  DispatchMessage(pMsg);
  return S_OK;
}

STDMETHODIMP CToolbar::HasFocusIO()
{
  if (mFocused)
    return S_OK;
  return S_FALSE;
}

STDMETHODIMP CToolbar::GetClassID(CLSID* pClassID)
{
  *pClassID = __uuidof(CToolbar);
  return S_OK;
}

STDMETHODIMP CToolbar::GetWindow(HWND* hwnd)
{
  *hwnd = mHandle;
  return S_OK;
}

STDMETHODIMP CToolbar::ContextSensitiveHelp(BOOL bEnterMode)
{
  return E_NOTIMPL;
}

LRESULT CALLBACK CToolbar::WndProc(HWND hWnd, UINT uMessage, WPARAM wParam, LPARAM lParam)
{
  CToolbar* pThis = (CToolbar*)GetWindowLongPtr(hWnd, GWL_USERDATA);

  if (pThis && hWnd == pThis->mEditHandle)
    return CallWindowProc(pThis->mOldEditProc, hWnd, uMessage, wParam, lParam);
 
  if (uMessage == WM_NCCREATE) {
    LPCREATESTRUCT lpcs = (LPCREATESTRUCT)lParam;
    pThis = (CToolbar*)(lpcs->lpCreateParams);
    if (pThis) {
      SetWindowLongPtr(hWnd, GWL_USERDATA, (LONG_PTR)pThis);
      pThis->mReflectionHandle = hWnd;
    }
  }
  else if (uMessage == WM_COMMAND)
    return pThis->OnCommand(wParam, lParam); // XXX Ctrl+F, etc. don't work in our textbox
    
  return DefWindowProc(hWnd,uMessage,wParam,lParam);
}

LRESULT CToolbar::OnCommand(WPARAM wParam, LPARAM lParam)
{
  WORD id = LOWORD(wParam);
  if (id == IDM_STATUSBUTTON) {
    StatusDlg(Plugin::GetField()).DoModal();
    return 0;
  }
  else if (id == IDC_EDIT) {
    WORD evt = HIWORD(wParam);
    if (evt == EN_SETFOCUS || EN_KILLFOCUS)
      return OnFocusChange(evt == EN_SETFOCUS);
  }

  return 0;
}

LRESULT CToolbar::OnFocusChange(bool bSetFocus)
{
  mFocused = bSetFocus;
  if (mInputSite)
    mInputSite->OnFocusChangeIS((IDockingWindow*)this, bSetFocus);

  if (!bSetFocus && mField) {
    TCHAR buf[Plugin::MAX_PASSWORD_LENGTH];    
    SendMessage(mEditHandle, WM_GETTEXT, 50, (LPARAM)buf);
    // SendMessage(mEditHandle, WM_SETTEXT, 0, (LPARAM)"");
    mField->SetMaskedText(buf);
    mField = NULL;
    ChangeSignal(SIGNAL_RED);
    EnableWindow(mEditHandle, FALSE);
  }

  return 0;
}

void CToolbar::ChangeSignal(Signal signal)
{
  TBBUTTONINFO tbi;

  tbi.cbSize = sizeof(TBBUTTONINFO);
  tbi.dwMask = TBIF_IMAGE;    
  tbi.iImage = signal;

  SendMessage(mHandle, TB_SETBUTTONINFO, IDM_STATUSBUTTON, (LPARAM) &tbi);
}