// ManagedCppException.h

#pragma once
#include <windows.h>
#include <tchar.h>

using namespace System;

class MyCppExceptionObject;

public class MyCppExceptionObject
{
public:
	int m_code;
	TCHAR m_text[128];

	MyCppExceptionObject::MyCppExceptionObject(LPTSTR text, int code)
	{
		m_code = code;
		::_tcsncpy_s(m_text,text,127);
	}
};

void RaiseMyException()
{
	::RaiseException(10,EXCEPTION_NONCONTINUABLE,0,NULL);
}

namespace ManagedCppException
{
	public ref class ThrowAnException
	{
	public:
		void ThrowAnException::ThrowManagedException()
		{
			throw gcnew System::Exception("Managed Exception");
		}

		void ThrowAnException::ThrowUnmanagedException()
		{
			//throw 5;
			//throw "Throwing a string as an exception!";
			//throw new MyCppExceptionObject(_T("Custom Object Exception"),6);
			RaiseMyException();
		}

		void ThrowAnException::ThrowBoxedException()
		{
			System::Int32^ boxedInt = 42;
			throw boxedInt;
		}

	};
}
