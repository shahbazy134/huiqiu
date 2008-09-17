#ifndef __hashedpassword_h
#define __hashedpassword_h

using namespace std;

class HashedPassword {
public:
  static char *getHashedPassword(const char *password, const char *realm);

private:
  static char *shorten(const char *source, int sourceLen, int shortenBy);
  static const char* b64_hmac_md5(const char *key, const char* msg);
  static int whitespaceIndex(const char *str);
  static bool containsWhitespace(const char *str);
  static string nextExtra(string &extras);
  static void rotate(string &arr, int amount);
  static int between(int min, int interval, int offset);
  static string nextBetween(char base, int interval, string& extras);
  static bool containsUpper(const char *str);
  static bool containsLower(const char *str); 
  static bool containsDigit(const char *str);
  static char *applyConstraints(const char *hash, int size, bool nonalphanumeric);
};

#endif

