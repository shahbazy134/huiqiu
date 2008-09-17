#include "StdAfx.h"
#include "HashManager.h"
#include "md5.h"
#include "CBase64.h"

// Format: ABCD
// A: Max length of password (possible values: 0-9)
// B: At least one digit or not (possible values: 0 or 1)
// C: At least one non-alphanumeric character or not (possible values: 0 or 1)
// D: Begin with a letter

const char* HashManager::Hash(const char* msg, const int parameters)
{
  unsigned char hashed_msg[16];
  MD5_CTX tctx;
  MD5Init(&tctx);
  MD5Update(&tctx, (unsigned char*)msg, (UINT)strlen(msg));
  MD5Final(hashed_msg, &tctx);
	
  // Convert to base 64
  char* hashed_msg_b64;
	CBase64 B64;
	B64.CreateMatchingEncodingBuffer(sizeof(hashed_msg), &hashed_msg_b64);
	B64.EncodeBuffer((char*)hashed_msg, sizeof(hashed_msg), hashed_msg_b64);

  return ApplyHashParameters(hashed_msg_b64, parameters);
}

const char* HashManager::KeyedHash(const char *key, const char* msg, const int parameters)
{
	char binary[16];
	hmac_md5((unsigned char*)msg, (int)strlen(msg), (unsigned char*)key, (int)strlen(key), binary);

	// Convert hash to base 64
	char* str;
	CBase64 B64;
	B64.CreateMatchingEncodingBuffer(sizeof(binary), &str);
	B64.EncodeBuffer(binary, sizeof(binary), str);
	
  return ApplyHashParameters(str, parameters);
}

const char* HashManager::ApplyHashParameters(char* msg, const int parameters)
{
  for (int i = eEndAlgFlags - 1; i >= 0; --i) {
    ApplyHashParameter(msg, (HashParam)i, GetDigit(parameters, i));
  }
  
  return msg;
}

int HashManager::GetDigit(int number, int index)
{
  int i = -1, digit = -1;
  while (++i < index) {
    number = number / 10; 
  }
  return number % 10;
}

void HashManager::ApplyHashParameter(char* str, HashParam param, int flag)
{
  switch(param) {
		case eMaxLength:
			MaxLength(str, flag);
			break;
    case eDigit:
      if (flag)
        AtLeastOneDigit(str);
      else
			  NoDigits(str);
			break;
		case eNANChar:
			if (flag)
			  AtLeastOneNonAlphaNumeric(str);
      else
        NoNonAlphaNumeric(str);
			break;
    case eBeginWithLetter:
      if (flag)
        BeginsWithLetter(str);
      break;
	}
}

void HashManager::MaxLength(char* str, size_t len)
{
	if (strlen(str) > len)
    str[len] = '\0';
}

void HashManager::BeginsWithLetter(char* str)
{
	if (!(str[0] <= 'Z' && str[0] >= 'A') && !(str[0] <= 'z' && str[0] >= 'a'))
		str[0] = (str[0] % 26) + 'a';
}

void HashManager::AtLeastOneDigit(char* str)
{
	size_t i = 1;

	for (i = 1; i < strlen(str); i++) {
		if (str[i] <= '9' && str[i] >= '0')
      return;
	}

	i = 1;
	str[i] = (str[i] % 10) + '0';

}

void HashManager::NoDigits(char* str)
{
	size_t i = 0;
	for (i = 0; i < strlen(str); i++) {
		if (str[i] <= '9' && str[i] >= '0')
			str[i] = (str[i] % 26) + 'a';
	}
}

void HashManager::AtLeastOneNonAlphaNumeric(char* str)
{
	size_t i = 1;

	for (i = 1; i < strlen(str); i++) {
		if(	!(str[i] <= '9' && str[i] >= '0') &&
			!(str[i] <= 'Z' && str[i] >= 'A') &&
			!(str[i] <= 'z' && str[i] >= 'a')
			) return;
	}

	i = 2;
	if(str[i] % 2)
		str[i] = '+';
	else
		str[i] = '/';
}

void HashManager::NoNonAlphaNumeric(char* str)
{
	size_t i = 0;
	for (i = 0; i < strlen(str); i++) {
		if( 
			!(str[i] <= '9' && str[i] >= '0') &&
			!(str[i] <= 'Z' && str[i] >= 'A') &&
			!(str[i] <= 'z' && str[i] >= 'a')
			) {
			str[i] = (str[i] % 26) + 'a';
		}
	}
}
