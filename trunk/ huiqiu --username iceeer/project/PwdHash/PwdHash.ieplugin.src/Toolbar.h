#pragma once
#include "resource.h" 
#include "shlobj.h"
#include "HashField.h"

[
	object,
	uuid("70FEDE52-F651-40E8-8D2E-CB53304963BB"),
	dual,	helpstring("IToolbar Interface"),
	pointer_default(unique)
]
__interface IToolbar : IDispatch
{
};

[
	coclass,
	threading("apartment"),
  aggregatable("never"),
	vi_progid("PwdHash.Toolbar"),
	progid("PwdHash.Toolbar.1"),
	version(1.0),
	uuid("1F088139-2F7F-41E2-A801-42761CB3D2AC"),
	helpstring("Toolbar Class")
]
class ATL_NO_VTABLE CToolbar : 
	public IObjectWithSiteImpl<CToolbar>,
  public IDispatchImpl<IToolbar, &__uuidof(IToolbar)>,
  public IDeskBand,
  public IPersistStream,
  public IInputObject     // so toolbar can accept input
{
public:
  CToolbar():mHandle(NULL),
             mEditHandle(NULL),
             mOldEditProc(NULL),
             mFocused(false),
             mInputSite(NULL),
             m_dwIndex(0),
             mField(NULL)
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
  
  BEGIN_CATEGORY_MAP(CToolbar)
    IMPLEMENTED_CATEGORY(CATID_DeskBand)
  END_CATEGORY_MAP()

  BEGIN_COM_MAP(CToolbar)
    COM_INTERFACE_ENTRY(IToolbar)
    COM_INTERFACE_ENTRY(IObjectWithSite)
    COM_INTERFACE_ENTRY(IDispatch)
    COM_INTERFACE_ENTRY(IOleWindow)
    COM_INTERFACE_ENTRY_IID(IID_IDockingWindow, IDockingWindow)
    COM_INTERFACE_ENTRY_IID(IID_IInputObject, IInputObject)
    COM_INTERFACE_ENTRY_IID(IID_IDeskBand, IDeskBand)
  END_COM_MAP()

  // IObjectWithSite
  STDMETHOD(SetSite)(IUnknown *pUnkSite);

  // IPersist and IPersistStream
  STDMETHOD(GetClassID)(CLSID* pClassID);

  STDMETHOD(IsDirty)(){ return S_FALSE; }
  STDMETHOD(Load)(IStream* pStm) { return S_OK; }
  STDMETHOD(Save)(IStream* pStm, BOOL fClearDirty) { return S_OK; }
  STDMETHOD(GetSizeMax)(ULARGE_INTEGER* pcbSize) { return E_NOTIMPL; }

  // IDeskBand
  STDMETHOD(GetBandInfo)(DWORD dwBandID, DWORD dwViewMode, DESKBANDINFO* pdbi);

  // IDockingWindow
  STDMETHOD(ShowDW)(BOOL bShow);
  STDMETHOD(CloseDW)(DWORD dwReserved);
  STDMETHOD(ResizeBorderDW)(LPCRECT prcBorder, IUnknown* pToolbarSite, BOOL fReserved) {
    return E_NOTIMPL;
  }

  //IOleWindow methods
  STDMETHOD(GetWindow) (HWND*);
  STDMETHOD(ContextSensitiveHelp) (BOOL);

  // IInputObject
  STDMETHOD(UIActivateIO)(BOOL bActivate, LPMSG pMsg);
  STDMETHOD(HasFocusIO)();
  STDMETHOD(TranslateAcceleratorIO)(LPMSG pMsg);

  enum Signal { SIGNAL_GREEN, SIGNAL_RED };
  void ChangeSignal(Signal signal);

  void OnPwdFieldFocus(HashField* field);
  //void OnModeChange(bool isAdvanced);
  void OnEnabledChange(bool isEnabled);

  HWND GetEditWindow() const { return mEditHandle; }

private:
  IInputObjectSite* mInputSite;
  HIMAGELIST mImageList;
  WNDPROC mOldEditProc;
  DWORD m_dwIndex;
  HWND mHandle;
  HWND mEditHandle;
  HWND mReflectionHandle;
  bool mFocused;
  bool mLockFocus;
  HashField* mField;
 
  bool RegisterAndCreateWindow(HWND* parent);
  bool PopulateToolbar();
  static LRESULT CALLBACK WndProc(HWND hWnd, UINT uMessage, WPARAM wParam, LPARAM lParam);
  LRESULT OnFocusChange(bool bSetFocus);
  LRESULT OnCommand(WPARAM wParam, LPARAM lParam);
  void CreateProxyPwdField();

  const static int SIGNAL_DIM = 16;
  const static int PWD_FIELD_OFFSET = 9;
  const static int PWD_FIELD_WIDTH = 125;
};

