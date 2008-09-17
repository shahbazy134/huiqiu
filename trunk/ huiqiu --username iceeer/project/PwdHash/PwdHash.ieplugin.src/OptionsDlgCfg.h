// OptionsDlgCfg.h : Declaration of the COptionsDlgCfg

#pragma once
#include "resource.h"       // main symbols
#include "OptionsDlg.h"

// IOptionsDlgCfg
[
	object,
	uuid("FF39B233-B1E6-4901-B97B-8E301D19EBD3"),
	dual,	helpstring("IOptionsDlgCfg Interface"),
	pointer_default(unique)
]
__interface IOptionsDlgCfg : IDispatch
{
};



// COptionsDlgCfg

[
	coclass,
	threading("apartment"),
	vi_progid("PwdHash.OptionsDlgCfg"),
	progid("PwdHash.OptionsDlgCfg.1"),
	version(1.0),
	uuid("C0050893-298D-4d7e-AC04-14D5AF2A6B4C"),
	helpstring("OptionsDlgCfg Class")
]
class ATL_NO_VTABLE COptionsDlgCfg : 
	public IObjectWithSiteImpl<COptionsDlgCfg>,
	public IOptionsDlgCfg,
  public IOleCommandTarget
{
public:
	COptionsDlgCfg()
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

  // XXX Make these private
  STDMETHODIMP Exec (const GUID *, DWORD, DWORD, VARIANTARG *, VARIANTARG *)
  {
    COptionsDlg().DoModal();
    return S_OK;
  }

  STDMETHODIMP QueryStatus(const GUID *, ULONG, OLECMD prgCmds[], OLECMDTEXT *pCmdText)
  {
    return S_OK;
  }
public:

};

