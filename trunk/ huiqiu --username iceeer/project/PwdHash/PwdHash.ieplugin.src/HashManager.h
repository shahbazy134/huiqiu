#ifndef __hashmanager_h
#define __hashmanager_h

enum HashParam { eBeginWithLetter, eNANChar, eDigit, eMaxLength, eEndAlgFlags };

class HashManager {
public:
  static const char* Hash(const char* msg, const int parameters);
  static const char* KeyedHash(const char* key, const char* msg, const int parameters);
private:
  static void ApplyHashParameter(char* str, HashParam param, int flag);
  static const char* ApplyHashParameters(char* msg, const int parameters);
  static int  GetDigit(int number, int index);
  
  static void MaxLength(char* str, size_t len);
  static void BeginsWithLetter(char* str);
  static void AtLeastOneDigit(char* str);
  static void NoDigits(char* str);
  static void AtLeastOneNonAlphaNumeric(char* str) ;
  static void NoNonAlphaNumeric(char* str);
};

#endif