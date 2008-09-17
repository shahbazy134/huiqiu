#ifndef __hashfield_h
#define __hashfield_h

#include "TranslationTable.h"
#include "mshtml.h"
[
  object,
  uuid("BF1AA681-A22F-450f-8677-8170A6CE49FE"),
  dual, helpstring("IHashField Interface"),
  pointer_default(unique)
]
__interface IHashField : IDispatch
{
};

struct rule {
  rule():pattern(NULL), key(NULL), domains(2), params(NULL) { }
  char* pattern, *key;
  int domains, params;
};

[
  coclass,
  threading("apartment"),
  aggregatable("never"),
  vi_progid("PwdHash.HashField"),
  progid("PwdHash.HashField.1"),
  version(1.0),
  uuid("2A298F23-E84D-4e22-B4EE-7FA49A2E3FC2"),
  helpstring("HashField Class")
]
class ATL_NO_VTABLE HashField : 
  private IDispatchImpl<IHashField, &__uuidof(IHashField)>
{
public:
  HashField();
  ~HashField();

  void Init(CComPtr<IHTMLInputElement>& elt, HWND windowHandle);
  const char* GetCipherText() const;
  const char* GetPlainText() const;
  
  const char* GetMaskedText() const;
  void        SetMaskedText(const char* text);

  char GetMask(char c);
  char Translate(char c) const;  

  enum ContentType { PLAINTEXT, MASKEDTEXT, CIPHERTEXT };
  HashField::ContentType GetContentType() const { return mContentType; }
  void SetContentType(const HashField::ContentType contentType) 
  { 
	  mContentType = contentType; 
	  UpdateTrafficLight();
  }

  void WarnUnprotectedUser();

  const char* GetHashKey() const;

  void put_value(CComBSTR value);

private:
  // IDispatchImpl override
  STDMETHOD(Invoke)(DISPID dispidMember,
    REFIID riid,
    LCID lcid,
    WORD wFlags,
    DISPPARAMS* pdispparams,
    VARIANT* pvarResult,
    EXCEPINFO* pexcepinfo,
    UINT* puArgErr);

  void UpdateBackgroundColor(bool flag);
  void ChangeIcon(int icon, HWND handle);
  void UpdateTrafficLight();
  void UpdateStatusString();
  void InterceptDataTransfer(CComPtr<IHTMLDataTransfer> dt);
  void SimulateText(LPCSTR text);
  void FinishEntry();

  DWORD mCookie;

  CComPtr<IHTMLInputElement> mElt;
  const char* mHashKey;
  int mHashParams;
  TranslationTable mTable;
  bool mHasNewValue;
  CComBSTR mNewValue;

  HashField::ContentType mContentType;

  HWND mWindowHandle;
  CComBSTR mOldColor;
};
#endif
