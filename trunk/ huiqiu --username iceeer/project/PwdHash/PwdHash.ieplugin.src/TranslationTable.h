#ifndef __translation_table_h
#define __translation_table_h

#include <string>
using namespace std;

class TranslationTable {
public:
  TranslationTable():mSize(0) { };

  char GetMask(char c);
  const char* Translate(char* str) const;
  char Translate(char c) const;
private:
  char mData[256];
  int mSize;
};

#endif