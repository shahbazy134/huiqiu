#include "stdafx.h"
#include "TranslationTable.h"

#define CHAR_A 65

char TranslationTable::Translate(char c) const
{
  return mData[toupper(c)];
}

const char* TranslationTable::Translate(char* str) const
{
  size_t len = strlen(str);
  for (size_t i = 0; i < len; ++i) {
    str[i] = Translate(str[i]);
  }
  return str;
}

char TranslationTable::GetMask(char c)
{
  char mask = CHAR_A + (mSize++ % 26);
  mData[mask] = c;
  return mask;  
}