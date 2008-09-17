#pragma once

#ifndef STRICT
#define STRICT
#endif

// Modify the following defines if you have to target a platform prior to the ones specified below.
// Refer to MSDN for the latest info on corresponding values for different platforms.
#define WINVER 0x0502         // Change this to the appropriate value to target Windows 98 and Windows 2000 or later.

#define _WIN32_WINNT 0x0502   // Change this to the appropriate value to target Windows 2000 or later.

#define _WIN32_WINDOWS 0x0502 // Change this to the appropriate value to target Windows Me or later.

#define _WIN32_IE 0x0600      // Change this to the appropriate value to target IE 5.0 or later.

#define _ATL_APARTMENT_THREADED
#define _ATL_NO_AUTOMATIC_NAMESPACE

#define _ATL_CSTRING_EXPLICIT_CONSTRUCTORS  // some CString constructors will be explicit

// turns off ATL's hiding of some common and often safely ignored warning messages
#define _ATL_ALL_WARNINGS

#include <atlbase.h>
#include <atlcom.h>
#include <atlwin.h>
#include <atltypes.h>
#include <atlctl.h>
#include <atlhost.h>
#include <assert.h>

using namespace ATL;