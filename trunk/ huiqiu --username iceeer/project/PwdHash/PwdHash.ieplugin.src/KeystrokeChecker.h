#pragma once

typedef bool(*KBProc)(WPARAM wParam, LPARAM lParam);

class KeystrokeChecker {

public:
	KeystrokeChecker(KBProc proc);
	~KeystrokeChecker();

private:
  static LRESULT CALLBACK KeyboardProc(int code, WPARAM wParam, LPARAM lParam);
  static KBProc mKBProc;
  static HHOOK mHook;
};