#include <windows.h>
#include "StdAfx.h"
#include "KeystrokeChecker.h"

HHOOK KeystrokeChecker::mHook = NULL;
KBProc KeystrokeChecker::mKBProc = NULL;

KeystrokeChecker::KeystrokeChecker(KBProc proc)
{
  assert(proc);
  mHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeystrokeChecker::KeyboardProc, GetModuleHandle("pwdhash.dll"), NULL);
  mKBProc = proc;
}

LRESULT CALLBACK KeystrokeChecker::KeyboardProc(int code, WPARAM wParam, LPARAM lParam)
{
  if (code < 0 || code == HC_NOREMOVE || !mKBProc(wParam, lParam))
    return CallNextHookEx(mHook, code, wParam, lParam);

  return 1;
}

KeystrokeChecker::~KeystrokeChecker()
{
  UnhookWindowsHookEx(mHook);
}
