#include "stdafx.h"
#include <iostream>
#include <string>
#include "CBase64.h"
#include "md5.h"
#include "HashedPassword.h"
#include "Plugin.h"

using namespace std;

char *HashedPassword::shorten(const char *source, int sourceLen, int shortenBy){ 
    int newLen = sourceLen - shortenBy;
    char *result =     new char[newLen];
    for(int i=0; i<newLen; i++){
        result[i] = source[i];
    }
    result[newLen-1] = '\0';

    return result; 
}

const char* HashedPassword::b64_hmac_md5(const char *key, const char* msg)
{
    char binary[16];
    hmac_md5((unsigned char*)msg, (int)strlen(msg), (unsigned char*)key, (int)strlen(key), binary);

    // Convert hash to base 64 
    char* str;
    CBase64 B64;
    int size = B64.CreateMatchingEncodingBuffer(sizeof(binary), &str);
    B64.EncodeBuffer(binary, sizeof(binary), str);

    
    cout << "Starting with: " << str << endl;; 

    if(str[size-2] == '='){
        cout << "Shorten by 2" << endl;
        char *tmp = str;
        str = shorten(str, size, 2);
        delete[] tmp;
    }else if(str[size-1] == '='){ 
        cout << "Shorten by 1" << endl;
        char *tmp = str;
        str = shorten(str, size, 1);
        delete[] tmp;
    }

    cout << "Returning: " << str << endl; 

    return str;
}

int HashedPassword::whitespaceIndex(const char *str){
    for(int i = 0; i<strlen(str); i++){
        if(!isalnum(str[i]) && str[i] != '_')
            return i;
    }

    return -1; 
}

bool HashedPassword::containsWhitespace(const char *str){
    return(whitespaceIndex(str) != -1);
}

string HashedPassword::nextExtra(string &extras){
    string result = "";

    
    if(extras.size() > 0){ 
        result += extras[0];
        extras = extras.substr(1);
    }

    return result;
}

void HashedPassword::rotate(string &arr, int amount){
    for(int i=0; i<amount; i++){
        int temp = arr[0]; 
        arr = arr.substr(1);
        arr += temp;
    }
}

int HashedPassword::between(int min, int interval, int offset){
    return min + offset % interval;
}

string HashedPassword::nextBetween(char base, int interval, string& extras){ 
    string extra = nextExtra(extras);

    int offset = (extra == "" ? 0 : extra[0]);
    char ch = (char) between(base, interval, offset);

    string result = "";
    result += ch; 

    return result;
}

bool HashedPassword::containsUpper(const char *str){
    for(int i=0; i<strlen(str); i++){
        if(isupper(str[i])) return true;
    }

    return false;
}

bool HashedPassword::containsLower(const char *str){ 
    for(int i=0; i<strlen(str); i++){
        if(islower(str[i])) return true;
    }

    return false;
}

bool HashedPassword::containsDigit(const char *str){
    for(int i=0; i<strlen(str); i++){
        if(isdigit(str[i])) return true; 
    }

    return false;
}

char *HashedPassword::applyConstraints(const char *hash, int size, bool nonalphanumeric){

    cout << "Applying constraints..." << endl;

    int startingSize = size - 4; 

    string result = "";
    for(int i=0; i<startingSize; i++){
        result += hash[i];
    }

    cout << "initial result: " << result << endl;

    string extras = ""; 
    for(int i = startingSize; i < strlen(hash); i++){
        extras += hash[i];
    }

    cout << "initial extras: " << extras << endl;

    result += (containsUpper(result.c_str ()) ? nextExtra(extras) : nextBetween('A', 26, extras));
    result += (containsLower(result.c_str()) ? nextExtra(extras) : nextBetween('a', 26, extras));
    cout << "after containsLower: " << result << endl; 

    result += (containsDigit(result.c_str()) ? nextExtra(extras) : nextBetween('0', 10, extras));
    cout << "after containsDigit: " << result << endl;

    result += ((containsWhitespace( result.c_str()) && nonalphanumeric) ? nextExtra(extras) : "+");

    cout << "Before while loop: " << result << endl;
    cout << "Extras before while loop: " << extras << endl; 

    while(containsWhitespace(result.c_str()) && !nonalphanumeric){
        cout << "Executing while loop!" << endl;
        int index = whitespaceIndex(result.c_str());
        result[index] = nextBetween('A', 26, extras)[0]; 
    }

    rotate(result, nextExtra(extras)[0]);

    char *final = new char[result.size() + 1];
    for(int i=0; i<result.size(); i++){
        final[i] = result[i];
    }
    final[result.size ()] = '\0';

    return final;
}

char *HashedPassword::getHashedPassword(const char *password, const char *realm){

	// Trim the password prefix off the front, if it's there
	if(!strncmp(PASSWORD_PREFIX, password, strlen(PASSWORD_PREFIX)))
		password += strlen(PASSWORD_PREFIX);

	const char *hash = b64_hmac_md5(password, realm);
    int size = strlen(password) + strlen(PASSWORD_PREFIX); 
    bool nonalphanumeric = containsWhitespace(password);
    char *result = applyConstraints(hash, size, nonalphanumeric);

    return result;
}

int main ()
{
    cout << "Hello, world!" << endl; 

    //cout << b64_hmac_md5("key", "data") << endl;

	cout << "Hashed password: " << HashedPassword::getHashedPassword("hello", " google.com");

    return 0;
}