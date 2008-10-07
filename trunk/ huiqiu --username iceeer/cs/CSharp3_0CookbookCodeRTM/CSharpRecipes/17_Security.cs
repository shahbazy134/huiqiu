using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Security.AccessControl;
using System.Text;
using System.Collections;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Web.Configuration;
using System.Configuration;
using System.Security.Policy;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Win32;
using System.Xml.Linq;



namespace CSharpRecipes
{
	public class Security
	{
        #region "17.1 Controlling Access to Types in a Local Assembly"	
        public static void ControlAccess()
        {
            // Create the security proxy here
            CompanyDataSecProxy companyDataSecProxy = new CompanyDataSecProxy();

            // Read some data
            Console.WriteLine("CEOPhoneNumExt: " + 
                companyDataSecProxy.CEOPhoneNumExt);

            // Write some data
            companyDataSecProxy.AdminPwd = "asdf";
            companyDataSecProxy.AdminUserName = "asdf";

            // Save and refresh this data
            companyDataSecProxy.SaveNewData();
            companyDataSecProxy.RefreshData();


            // Instantiate the CompanyData object directly without a proxy
            CompanyData companyData = new CompanyData();

            // Read some data
            Console.WriteLine("CEOPhoneNumExt: " + companyData.CEOPhoneNumExt);

            // Write some data
            companyData.AdminPwd = "asdf";
            companyData.AdminUserName = "asdf";

            // Save and refresh this data
            companyData.SaveNewData();
            companyData.RefreshData();

        }

        internal interface ICompanyData
        {
            string AdminUserName
            {
                get;
                set;
            }

            string AdminPwd
            {
                get;
                set;
            }

            string CEOPhoneNumExt
            {
                get;
                set;
            }

            void RefreshData();
            void SaveNewData();
        }

        internal class CompanyData : ICompanyData
        {
            public CompanyData()
            {
                Console.WriteLine("[CONCRETE] CompanyData Created");
                // Perform expensive initialization here
                this.AdminUserName = "admin";
                this.AdminPwd = "password";
                this.CEOPhoneNumExt = "0000";
            }

            public string AdminUserName
            {
                get;
                set;
            }
    
            public string AdminPwd
            {
                get;
                set;
            }

            public string CEOPhoneNumExt
            {
                get;
                set;
            }

            public void RefreshData()
            {
                Console.WriteLine("[CONCRETE] Data Refreshed");
            }

            public void SaveNewData()
            {
                Console.WriteLine("[CONCRETE] Data Saved");
            }
        }

        public class CompanyDataSecProxy : ICompanyData
        {
            public CompanyDataSecProxy()
            {
                Console.WriteLine("[SECPROXY] Created");

                // Must set principal policy first
                AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            }


            private ICompanyData coData = null;
            private PrincipalPermission admPerm = 
                new PrincipalPermission(null, @"BUILTIN\Administrators", true);
            private PrincipalPermission guestPerm = 
                new PrincipalPermission(null, @"BUILTIN\Guest", true);
            private PrincipalPermission powerPerm = 
                new PrincipalPermission(null, @"BUILTIN\PowerUser", true);
            private PrincipalPermission userPerm = 
                new PrincipalPermission(null, @"BUILTIN\User ", true);

            public string AdminUserName
            {
                get 
                {
                    string userName = "";
                    try
                    {
                        admPerm.Demand();
                        Startup();
                        userName = coData.AdminUserName;
                    }
                    catch(SecurityException e)
                    {
                        Console.WriteLine("AdminUserName_get Failed! {0}",e.ToString());
                    }
                    return (userName);
                }
                set 
                {
                    try
                    {
                        admPerm.Demand();
                        Startup();
                        coData.AdminUserName = value;
                    }
                    catch(SecurityException e)
                    {
                        Console.WriteLine("AdminUserName_set Failed! {0}",e.ToString());
                    }
                }
            }

            public string AdminPwd
            {
                get 
                {
                    string pwd = "";
                    try
                    {
                        admPerm.Demand();
                        Startup();
                        pwd = coData.AdminPwd;
                    }
                    catch(SecurityException e)
                    {
                        Console.WriteLine("AdminPwd_get Failed! {0}",e.ToString());
                    }

                    return (pwd);
                }
                set 
                {
                    try
                    {
                        admPerm.Demand();
                        Startup();
                        coData.AdminPwd = value;
                    }
                    catch(SecurityException e)
                    {
                        Console.WriteLine("AdminPwd_set Failed! {0}",e.ToString());
                    }
                }
            }



            public string CEOPhoneNumExt
            {
                get 
                {
                    string ceoPhoneNum = "";
                    try
                    {
                        admPerm.Union(powerPerm).Demand();
                        Startup();
                        ceoPhoneNum = coData.CEOPhoneNumExt;
                    }
                    catch(SecurityException e)
                    {
                        Console.WriteLine("CEOPhoneNum_set Failed! {0}",e.ToString());
                    }
                    return (ceoPhoneNum);
                }
                set 
                {
                    try
                    {
                        admPerm.Demand();
                        Startup();
                        coData.CEOPhoneNumExt = value;
                    }
                    catch(SecurityException e)
                    {
                        Console.WriteLine("CEOPhoneNum_set Failed! {0}",e.ToString());
                    }
                }
            }

            public void RefreshData()
            {
                try
                {
                    admPerm.Union(powerPerm.Union(userPerm)).Demand();
                    Startup();
                    Console.WriteLine("[SECPROXY] Data Refreshed");
                    coData.RefreshData();
                }
                catch(SecurityException e)
                {
                    Console.WriteLine("RefreshData Failed! {0}",e.ToString());
                }
            }

            public void SaveNewData()
            {
                try 
                {
                    admPerm.Union(powerPerm).Demand();
                    Startup();
                    Console.WriteLine("[SECPROXY] Data Saved");
                    coData.SaveNewData();
                }
                catch(SecurityException e)
                {
                    Console.WriteLine("SaveNewData Failed! {0}",e.ToString());
                }
            }

            // DO NOT forget to use [#define DOTRACE] to control the tracing proxy
            private void Startup()
            {
                if (coData == null)
                {
                    #if (DOTRACE)
                        coData = new CompanyDataTraceProxy();
                    #else
                        coData = new CompanyData();
                    #endif
                    Console.WriteLine("[SECPROXY] Refresh Data");
                    coData.RefreshData();
                }
            }

            public class CompanyDataTraceProxy : ICompanyData
            {
                public CompanyDataTraceProxy()
                {
                    Console.WriteLine("[TRACEPROXY] Created");
                    string path = Path.GetTempPath() + @"\CompanyAccessTraceFile.txt";
                    fileStream = new FileStream(path, FileMode.Append, 
                        FileAccess.Write, FileShare.None);
                    traceWriter = new StreamWriter(fileStream);
                    coData = new CompanyData();
                }

                private ICompanyData coData = null;
                private FileStream fileStream = null;
                private StreamWriter traceWriter = null;


                public string AdminPwd
                {
                    get 
                    {
                        traceWriter.WriteLine("AdminPwd read by user.");
                        traceWriter.Flush();
                        return (coData.AdminPwd);
                    }
                    set
                    {
                        traceWriter.WriteLine("AdminPwd written by user.");
                        traceWriter.Flush();
                        coData.AdminPwd = value;
                    }
                }

                public string AdminUserName
                {
                    get 
                    {
                        traceWriter.WriteLine("AdminUserName read by user.");
                        traceWriter.Flush();
                        return (coData.AdminUserName);
                    }
                    set
                    {
                        traceWriter.WriteLine("AdminUserName written by user.");
                        traceWriter.Flush();
                        coData.AdminUserName = value;
                    }
                }

                public string CEOPhoneNumExt
                {
                    get 
                    {
                        traceWriter.WriteLine("CEOPhoneNumExt read by user.");
                        traceWriter.Flush();
                        return (coData.CEOPhoneNumExt);
                    }
                    set
                    {
                        traceWriter.WriteLine("CEOPhoneNumExt written by user.");
                        traceWriter.Flush();
                        coData.CEOPhoneNumExt = value;
                    }
                }

                public void RefreshData()
                {
                    Console.WriteLine("[TRACEPROXY] Refresh Data");
                    coData.RefreshData();
                }

                public void SaveNewData()
                {
                    Console.WriteLine("[TRACEPROXY] Save Data");
                    coData.SaveNewData();
                }
            }

        }

        #endregion

        #region "17.2 Encrypting/Decrypting a String"	
        public static void EncDecString()
        {
            string encryptedString = CryptoString.Encrypt("MyPassword");
            Console.WriteLine("encryptedString: " + encryptedString);
            // get the key and IV used so you can decrypt it later
            byte [] key = CryptoString.Key;
            byte [] IV = CryptoString.IV;

            CryptoString.Key = key;
            CryptoString.IV = IV;
            string decryptedString = CryptoString.Decrypt(encryptedString);
            Console.WriteLine("decryptedString: " + decryptedString);

        }

        public sealed class CryptoString
        {
            private CryptoString() {}

            private static byte[] savedKey = null;
            private static byte[] savedIV = null;

            public static byte[] Key
            {
                get { return savedKey; }
                set { savedKey = value; }
            }

            public static byte[] IV
            {
                get { return savedIV; }
                set { savedIV = value; }
            }

            private static void RdGenerateSecretKey(RijndaelManaged rdProvider)
            {
                if (savedKey == null)
                {
                    rdProvider.KeySize = 256;
                    rdProvider.GenerateKey();
                    savedKey = rdProvider.Key;
                }
            }

            private static void RdGenerateSecretInitVector(RijndaelManaged rdProvider)
            {
                if (savedIV == null)
                {
                    rdProvider.GenerateIV();
                    savedIV = rdProvider.IV;
                }
            }

            public static string Encrypt(string originalStr)
            {
                // Encode data string to be stored in memory
                byte[] originalStrAsBytes = Encoding.ASCII.GetBytes(originalStr);
                byte[] originalBytes = {};

                // Create MemoryStream to contain output
                MemoryStream memStream = new MemoryStream(originalStrAsBytes.Length);

                RijndaelManaged rijndael = new RijndaelManaged();

                // Generate and save secret key and init vector
                RdGenerateSecretKey(rijndael);
                RdGenerateSecretInitVector(rijndael);

                if (savedKey == null || savedIV == null)
                {
                    throw (new NullReferenceException(
                        "savedKey and savedIV must be non-null."));
                }

                // Create encryptor, and stream objects
                ICryptoTransform rdTransform = 
                    rijndael.CreateEncryptor((byte[])savedKey.Clone(),
                                            (byte[])savedIV.Clone());
                CryptoStream cryptoStream = new CryptoStream(memStream, rdTransform, 
                    CryptoStreamMode.Write);

                // Write encrypted data to the MemoryStream
                cryptoStream.Write(originalStrAsBytes, 0, originalStrAsBytes.Length);
                cryptoStream.FlushFinalBlock();
                originalBytes = memStream.ToArray();

                // Release all resources
                memStream.Close();
                cryptoStream.Close();
                rdTransform.Dispose();
                rijndael.Clear();

                // Convert encrypted string
                string encryptedStr = Convert.ToBase64String(originalBytes);
                return (encryptedStr);
            }

            public static string Decrypt(string encryptedStr)
            {
                // Unconvert encrypted string
                byte[] encryptedStrAsBytes = Convert.FromBase64String(encryptedStr);
                byte[] initialText = new Byte[encryptedStrAsBytes.Length];

                RijndaelManaged rijndael = new RijndaelManaged();
                MemoryStream memStream = new MemoryStream(encryptedStrAsBytes);

                if (savedKey == null || savedIV == null)
                {
                    throw (new NullReferenceException(
                        "savedKey and savedIV must be non-null."));
                }

                // Create decryptor, and stream objects
                ICryptoTransform rdTransform = 
                    rijndael.CreateDecryptor((byte[])savedKey.Clone(), 
                                            (byte[])savedIV.Clone());
                CryptoStream cryptoStream = new CryptoStream(memStream, rdTransform, 
                    CryptoStreamMode.Read);

                // Read in decrypted string as a byte[]
                cryptoStream.Read(initialText, 0, initialText.Length);

                // Release all resources
                memStream.Close();
                cryptoStream.Close();
                rdTransform.Dispose();
                rijndael.Clear();

                // Convert byte[] to string
                string decryptedStr = Encoding.ASCII.GetString(initialText);
                return (decryptedStr);
            }
        }

        #endregion

        #region "17.3 Encrypting and Decrypting a File"	
        public static void EncDecFile()
        {
            // Use TripleDES
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            SecretFile secretTDESFile = new SecretFile(tdes,"tdestext.secret");

            string encrypt = "My TDES Secret Data!";

            Console.WriteLine("Writing secret data: {0}",encrypt);
            secretTDESFile.SaveSensitiveData(encrypt);
            // save for storage to read file
            byte [] key = secretTDESFile.Key;
            byte [] IV = secretTDESFile.IV;

            string decrypt = secretTDESFile.ReadSensitiveData();
            Console.WriteLine("Read secret data: {0}",decrypt);

            // release resources
            tdes.Clear();

            // Use Rijndael
            RijndaelManaged rdProvider = new RijndaelManaged();
            SecretFile secretRDFile = new SecretFile(rdProvider,"rdtext.secret");

            encrypt = "My Rijndael Secret Data!";

            Console.WriteLine("Writing secret data: {0}",encrypt);
            secretRDFile.SaveSensitiveData(encrypt);
            // save for storage to read file
            key = secretRDFile.Key;
            IV = secretRDFile.IV;

            decrypt = secretRDFile.ReadSensitiveData();
            Console.WriteLine("Read secret data: {0}",decrypt);

            // release resources
            rdProvider.Clear();
        }

        public class SecretFile
        {
            private byte[] savedKey = null;
            private byte[] savedIV = null;
            private SymmetricAlgorithm symmetricAlgorithm;
            string path;

            public byte[] Key
            {
                get { return savedKey; }
                set { savedKey = value; }
            }

            public byte[] IV
            {
                get { return savedIV; }
                set { savedIV = value; }
            }

            public SecretFile(SymmetricAlgorithm algorithm, string fileName)
            {
                symmetricAlgorithm = algorithm;
                path = fileName;
            }

            public void SaveSensitiveData(string sensitiveData)
            {
                // Encode data string to be stored in encrypted file
                byte[] encodedData = Encoding.Unicode.GetBytes(sensitiveData);

                // Create FileStream and crypto service provider objects
                FileStream fileStream = new FileStream(path, 
                    FileMode.Create, 
                    FileAccess.Write);

                // Generate and save secret key and init vector
                GenerateSecretKey();
                GenerateSecretInitVector();

                // Create crypto transform and stream objects
                ICryptoTransform transform = symmetricAlgorithm.CreateEncryptor(savedKey,
                    savedIV);
                CryptoStream cryptoStream = 
                    new CryptoStream(fileStream, transform, CryptoStreamMode.Write);

                // Write encrypted data to the file 
                cryptoStream.Write(encodedData, 0, encodedData.Length);

                // Release all resources
                cryptoStream.Close();
                transform.Dispose();
                fileStream.Close();
            }

            public string ReadSensitiveData()
            {
                // Create file stream to read encrypted file back
                FileStream fileStream = new FileStream(path, 
                    FileMode.Open, 
                    FileAccess.Read);

                //print out the contents of the encrypted file
                BinaryReader binReader = new BinaryReader(fileStream);
                Console.WriteLine("---------- Encrypted Data ---------");
                int count = (Convert.ToInt32(binReader.BaseStream.Length));
                byte [] bytes = binReader.ReadBytes(count);
                char [] array = Encoding.Unicode.GetChars(bytes);
                string encdata = new string(array);
                Console.WriteLine(encdata);
                Console.WriteLine("---------- Encrypted Data ---------\r\n");
        
                // reset the file stream
                fileStream.Seek(0,SeekOrigin.Begin);

                // Create Decryptor
                ICryptoTransform transform = symmetricAlgorithm.CreateDecryptor(savedKey, 
                    savedIV);
                CryptoStream cryptoStream = new CryptoStream(fileStream, 
                    transform, 
                    CryptoStreamMode.Read);

                //print out the contents of the decrypted file
                StreamReader srDecrypted = new StreamReader(cryptoStream,
                    new UnicodeEncoding());
                Console.WriteLine("---------- Decrypted Data ---------");
                string decrypted = srDecrypted.ReadToEnd();
                Console.WriteLine(decrypted);
                Console.WriteLine("---------- Decrypted Data ---------");

                // Release all resources
                binReader.Close();
                srDecrypted.Close();
                cryptoStream.Close();
                transform.Dispose();
                fileStream.Close();
                return decrypted;
            }

            private void GenerateSecretKey()
            {
                if(null != (symmetricAlgorithm as TripleDESCryptoServiceProvider))
                {
                    TripleDESCryptoServiceProvider tdes;
                    tdes = symmetricAlgorithm as TripleDESCryptoServiceProvider;
                    tdes.KeySize = 192; //  Maximum key size
                    tdes.GenerateKey();
                    savedKey = tdes.Key;
                }
                else if(null != (symmetricAlgorithm as RijndaelManaged))
                {
                    RijndaelManaged rdProvider;
                    rdProvider = symmetricAlgorithm as RijndaelManaged;
                    rdProvider.KeySize = 256; // Maximum key size
                    rdProvider.GenerateKey();
                    savedKey = rdProvider.Key;
                }
            }

            private void GenerateSecretInitVector()
            {
                if(null != (symmetricAlgorithm as TripleDESCryptoServiceProvider))
                {
                    TripleDESCryptoServiceProvider tdes;
                    tdes = symmetricAlgorithm as TripleDESCryptoServiceProvider;
                    tdes.GenerateIV();
                    savedIV = tdes.IV;
                }
                else if(null != (symmetricAlgorithm as RijndaelManaged))
                {
                    RijndaelManaged rdProvider;
                    rdProvider = symmetricAlgorithm as RijndaelManaged;
                    rdProvider.GenerateIV();
                    savedIV = rdProvider.IV;
                }
            }
        }

        #endregion

        #region "17.4 Cleaning Up Cryptography Information"	
        public static void CleanUpCrypto()
        {
            string originalStr = "SuperSecret information";
            // Encode data string to be stored in memory
            byte[] originalStrAsBytes = Encoding.ASCII.GetBytes(originalStr);
            byte[] originalBytes = {};

            // Create MemoryStream to contain output
            MemoryStream memStream = new MemoryStream(originalStrAsBytes.Length);

            RijndaelManaged rijndael = new RijndaelManaged();

            // Generate secret key and init vector
            rijndael.KeySize = 256;
            rijndael.GenerateKey();
            rijndael.GenerateIV();

            // save off the key and IV for later decryption
            byte [] key = rijndael.Key;
            byte [] IV = rijndael.IV;

            // Create encryptor, and stream objects
            ICryptoTransform transform = rijndael.CreateEncryptor(rijndael.Key,
                rijndael.IV);
            CryptoStream cryptoStream = new CryptoStream(memStream, transform, 
                CryptoStreamMode.Write);

            // Write encrypted data to the MemoryStream
            cryptoStream.Write(originalStrAsBytes, 0, originalStrAsBytes.Length);
            cryptoStream.FlushFinalBlock();

            // Release all resources as soon as we are done with them
            // to prevent retaining any information in memory
            memStream.Close();
            memStream = null;
            cryptoStream.Close();
            cryptoStream = null;
            transform.Dispose();
            transform = null;
            // this clear statement regens both the key and the init vector so that
            // what is left in memory is no longer the values you used to encrypt with
            rijndael.Clear();
            // make this eligible for GC as soon as possible
            rijndael = null;
        }
        #endregion

        #region "17.5 Verifying That a String is Not Corrupted During Transmission"	
        public static void VerifyNonStringCorruption()
        {
            string testString = "This is the string that we'll be testing.";
            string unhashedString;
            string hashedString = HashOps.CreateStringHash(testString);

            bool result = HashOps.IsStringCorrupted(hashedString, out unhashedString);
            Console.WriteLine(result);
            if (!result)
                Console.WriteLine("The string sent is: " + unhashedString);
            else
                Console.WriteLine("The string " + unhashedString + 
                    " has become corrupted.");
        }

        public class HashOps 
        {
            public static string CreateStringHash(string unHashedString)
            {
                byte[] encodedUnHashedString = Encoding.Unicode.GetBytes(unHashedString);

                SHA256Managed hashingObj = new SHA256Managed();
                byte[] hashCode = hashingObj.ComputeHash(encodedUnHashedString);

                string hashBase64 = Convert.ToBase64String(hashCode);
                string stringWithHash = unHashedString + hashBase64; 

                hashingObj.Clear();

                return (stringWithHash);
            }

            public static bool IsStringCorrupted(string stringWithHash, 
                out string originalStr)
            {
                // Code to quickly test the handling of a tampered string
                //stringWithHash = stringWithHash.Replace('a', 'b');

                if (stringWithHash.Length < 45)
                {
                    originalStr = null;
                    return (true);
                }

                string hashCodeString = 
                    stringWithHash.Substring(stringWithHash.Length - 44);
                string unHashedString = 
                    stringWithHash.Substring(0, stringWithHash.Length - 44);

                byte[] hashCode = Convert.FromBase64String(hashCodeString);

                byte[] encodedUnHashedString = Encoding.Unicode.GetBytes(unHashedString);

                SHA256Managed hashingObj = new SHA256Managed();
                byte[] receivedHashCode = hashingObj.ComputeHash(encodedUnHashedString);

                bool hasBeenTamperedWith = false;
                for (int counter = 0; counter < receivedHashCode.Length; counter++)
                {
                    if (receivedHashCode[counter] != hashCode[counter])
                    {
                        hasBeenTamperedWith = true;
                        break;
                    }
                }

                if (!hasBeenTamperedWith)
                {
                    originalStr = unHashedString;
                }
                else
                {
                    originalStr = null;
                }

                hashingObj.Clear();

                return (hasBeenTamperedWith);
            }
        }

        #endregion

        #region "17.6 Securely Storing Data"	
        public static void SecurelyStoringData()
        {
            UserSettings settings = new UserSettings("89734kdkjs8734kjhd");
            if (settings.IsPasswordValid("89734kdkjs8734kjhd"))
            {
                Console.WriteLine("Welcome");
            }
        }

        // class to hold user settings 
        public class UserSettings
        {
            IsolatedStorageFile isoStorageFile = null;
            IsolatedStorageFileStream isoFileStream = null;
            XmlDocument settingsDoc = null;
            XmlTextWriter writer = null;
            const string storageName = "SettingsStorage.xml";

            // constructor
            public UserSettings(string password)
            {
                // get the isolated storage
                isoStorageFile = IsolatedStorageFile.GetUserStoreForDomain();
                // create an internal DOM for settings
                settingsDoc = new XmlDocument();
                // if no settings, create default
                if(isoStorageFile.GetFileNames(storageName).Length == 0)
                {
                    isoFileStream = 
                        new IsolatedStorageFileStream(storageName,
                        FileMode.Create,
                        isoStorageFile);

                    writer = new XmlTextWriter(isoFileStream,Encoding.UTF8);
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Settings");
                    writer.WriteStartElement("User");
                    // get current user as that is the user
                    WindowsIdentity user = WindowsIdentity.GetCurrent();
                    writer.WriteString(user.Name);
                    writer.WriteEndElement();
                    writer.WriteStartElement("Password");
                    // pass null as the salt to establish one
                    string hashedPassword = CreateHashedPassword(password,null);
                    writer.WriteString(hashedPassword);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                    writer.Close();
                    Console.WriteLine("Creating settings for " + user.Name);
                }
        
                // set up access to settings store
                isoFileStream = 
                    new IsolatedStorageFileStream(storageName,
                    FileMode.Open,
                    isoStorageFile);

                // load settings from isolated filestream
                settingsDoc.Load(isoFileStream);
                Console.WriteLine("Loaded settings for " + User);
            }

            // The User property provides access to the WindowsIdentity of the user that this set of settings belongs to:

            // User Property
            public string User
            {
                get
                {
                    XmlNode userNode = settingsDoc.SelectSingleNode("Settings/User");
                    if(userNode != null)
                    {
                        return userNode.InnerText;
                    }
                    return "";
                }
            }

            // The Password property gets the salted and hashed password value from the XML store and when updating the password, takes the plain text of the password and creates the salted and hashed version which is then stored.

            // Password Property
            public string Password
            {
                get
                {
                    XmlNode pwdNode = 
                        settingsDoc.SelectSingleNode("Settings/Password");
                    if(pwdNode != null)
                    {
                        return pwdNode.InnerText;
                    }
                    return "";
                }
                set
                {
                    XmlNode pwdNode = 
                        settingsDoc.SelectSingleNode("Settings/Password");

                    string hashedPassword = CreateHashedPassword(value,null);
                    if(pwdNode != null)
                    {
                        pwdNode.InnerText = hashedPassword;
                    }
                    else
                    {
                        XmlNode settingsNode = 
                            settingsDoc.SelectSingleNode("Settings");
                        XmlElement pwdElem = 
                            settingsDoc.CreateElement("Password");
                        pwdElem.InnerText=hashedPassword;
                        settingsNode.AppendChild(pwdElem);
                    }
                }
            }

            // The CreateHashedPassword method performs the creation of the salted and hashed password.  The password parameter is the plaintext of the password and the existingSalt parameter is the salt to use when creating the salted and hashed version.  If no salt exists, like the first time a password is stored, existingSalt should be passed null and a random salt will be generated.
            // Once we have the salt, it is combined with the plaintext password and hashed using the SHA512Managed class.  The salt value is then appended to the end of the hashed value and returned.  The salt is appended so that when we attempt to validate the password, we know what salt was used to create the hashed value.  The entire value is then base64 encoded and returned.
            // Make a hashed password
            private string CreateHashedPassword(string password, 
                                                byte[] existingSalt)
            {
                byte [] salt = null;
                if(existingSalt == null)
                {
                    // Make a salt of random size
                    Random  random = new Random();
                    int size = random.Next(16, 64);

                    // create salt array
                    salt = new byte[size];

                    // Use the better random number generator to get
                    // bytes for the salt
                    RNGCryptoServiceProvider rng = 
                            new RNGCryptoServiceProvider();
                    rng.GetNonZeroBytes(salt); 
                }
                else
                    salt = existingSalt;

                // Turn string into bytes
                byte[] pwd = Encoding.UTF8.GetBytes(password);

                // make storage for both password and salt
                byte[] saltedPwd = new byte[pwd.Length + salt.Length];

                // add pwd bytes first
                pwd.CopyTo(saltedPwd,0);
                // now add salt
                salt.CopyTo(saltedPwd,pwd.Length);

                // Use SHA512 as the hashing algorithm
                SHA512Managed sha512 = new SHA512Managed();

                // Get hash of salted password
                byte[] hash = sha512.ComputeHash(saltedPwd);

                // append salt to hash so we have it
                byte[] hashWithSalt = new byte[hash.Length + salt.Length];

                // copy in bytes
                hash.CopyTo(hashWithSalt,0);
                salt.CopyTo(hashWithSalt,hash.Length);
            
                // return base64 encoded hash with salt
                return Convert.ToBase64String(hashWithSalt);
            }

            // To check a given password against the stored salted and hashed value, we call IsPasswordValid and pass in the plaintext password to check.  First the stored value is retrieved using the Password property and converted from base64.  Then we know we used SHA512, so there are 512 bits in the hash but we need the byte size so we do the math and get that size in bytes.  This allows us to figure out where to get the salt from in the value so we copy it out of the value and call CreateHashedPassword using that salt and the plaintext password parameter.  This gives us the hashed value for the password that was passed in to verify and once we have that, we just compare it to the Password property to see if we have a match and return true or false appropriately.

            // Check the password against our storage
            public bool IsPasswordValid(string password)
            {
                // Get bytes for password
                // this is the hash of the salted password and the salt
                byte[] hashWithSalt = Convert.FromBase64String(Password);

                // We used 512 bits as the hash size (SHA512)
                int hashSizeInBytes = 512 / 8;

                // make holder for original salt
                int saltSize = hashWithSalt.Length - hashSizeInBytes;
                byte[] salt = new byte[saltSize];

                // copy out the salt
                Array.Copy(hashWithSalt,hashSizeInBytes,salt,0,saltSize);

                // Figure out hash for this password
                string passwordHash = CreateHashedPassword(password,salt);

                // If the computed hash matches the specified hash,
                // the plain text value must be correct.
                // see if Password (stored) matched password passed in
                return (Password == passwordHash);
            }
        }

        #endregion

        #region "17.7 Making a Security Assert Safe"	
        public static void SafeAssert()
        {
            CallSecureFunctionSafelyAndEfficiently();
        }

        public static void CallSecureFunctionSafelyAndEfficiently()
        {

            // set up a permission to be able to access non-public members 
            // via reflection
            ReflectionPermission perm = 
                new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);

            // Demand the permission set we have compiled before using Assert
            // to make sure we have the right before we Assert it.  We do
            // the Demand to insure that we have checked for this permission
            // before using Assert to short-circuit stackwalking for it which
            // helps us stay secure, while performing better.
            perm.Demand();

            // Assert this right before calling into the function that
            // would also perform the Demand to short-circuit the stack walk
            // each call would generate.  The Assert helps us to optimize
            // out use of SecureFunction
            perm.Assert();

            // we call the secure function 100 times but only generate
            // the stackwalk from the function to this calling function
            // instead of walking the whole stack 100 times.
            for(int i=0;i<100;i++)
            {
                SecureFunction();
            }
        }

        public static void SecureFunction()
        {
            // set up a permission to be able to access non-public members 
            // via reflection
            ReflectionPermission perm = 
                new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);

            // Demand the right to do this and cause a stack walk
            perm.Demand();            

            // Perform the action here...
        }

        #endregion

        #region "17.8 Verifying if an Assembly has been Granted Specific Permissions"	
        public static void VerifyAssemblyPerms()
        {
            // This would set up a Regex for the O’Reilly web site then use it to create a WebPermission for connecting to that site and all sites containing the www.oreilly.com string.  We would then check the WebPermission against the SecurityManager to see if we have the permission to do this.
            Regex regex = new Regex(@"http://www\.oreilly\.com/.*");
            WebPermission webConnectPerm = new WebPermission(NetworkAccess.Connect,regex);
            if(SecurityManager.IsGranted(webConnectPerm))
            {
                // connect to the oreilly site
            }
        }
        #endregion

        #region "17.9 Minimizing the Attack Surface of an Assembly"	
        public static void MinimizeAttackSurface()
        {
            Console.WriteLine("See the text about how to implement SecurityAction.RequestRefuse");
        }
        #endregion

		#region "17.10 Obtaining Security Information"
		// ms-help://MS.VSCC.v80/MS.MSDNQTR.v80.en/MS.MSDN.v80/MS.NETDEVFX.v20.en/cpref/html/M_System_Security_AccessControl_ObjectSecurity_GetOwner_1_a6998a77.htm


		public static void TestViewFileRegRights()
		{
			// Get security information from a file
			string file = @"c:\BOOT.INI";
			FileSecurity fileSec = File.GetAccessControl(file);
			DisplayFileSecurityInfo(fileSec);

			// Get security information from a registry key
			RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Compuware\Weasel\2.0");
			RegistrySecurity regSecurity = regKey.GetAccessControl();
			DisplayRegKeySecurityInfo(regSecurity);
		}


		public static void DisplayRegKeySecurityInfo(RegistrySecurity regSec)
		{
			Console.WriteLine("GetSecurityDescriptorSddlForm:  " + regSec.GetSecurityDescriptorSddlForm(AccessControlSections.All));

			foreach (RegistryAccessRule ace in regSec.GetAccessRules(true, true, typeof(NTAccount)))
			{
				Console.WriteLine("\tIdentityReference.Value: " + ace.IdentityReference.Value);
				Console.WriteLine("\tAccessControlType: " + ace.AccessControlType);
				Console.WriteLine("\tRegistryRights: " + ace.RegistryRights.ToString());
				Console.WriteLine("\tInheritanceFlags: " + ace.InheritanceFlags);
				Console.WriteLine("\tIsInherited: " + ace.IsInherited);
				Console.WriteLine("\tPropagationFlags: " + ace.PropagationFlags);

				//NTAccount name = (NTAccount)ace.IdentityReference;
				//Console.WriteLine("\tname: " + name);
				// Change NTAccount to SecurityIdentifier to get the non-human readable version of the SID

				Console.WriteLine("-----------------\r\n\r\n");
			}


			foreach (RegistryAuditRule ace in regSec.GetAuditRules(true, true, typeof(NTAccount)))
			{
				Console.WriteLine("\tIdentityReference.Value: " + ace.IdentityReference.Value);
				Console.WriteLine("\tAuditFlags: " + ace.AuditFlags);
				Console.WriteLine("\tRegistryRights: " + ace.RegistryRights.ToString());
				Console.WriteLine("\tInheritanceFlags: " + ace.InheritanceFlags);
				Console.WriteLine("\tIsInherited: " + ace.IsInherited);
				Console.WriteLine("\tPropagationFlags: " + ace.PropagationFlags);

				Console.WriteLine("-----------------\r\n\r\n");
			}

			Console.WriteLine("GetGroup(typeof(NTAccount)).Value: " + regSec.GetGroup(typeof(NTAccount)).Value);
			Console.WriteLine("GetOwner(typeof(NTAccount)).Value: " + regSec.GetOwner(typeof(NTAccount)).Value);

			Console.WriteLine("---------------------------------------\r\n\r\n\r\n");
		}




		public static void DisplayFileSecurityInfo(FileSecurity fileSec)
		{
			Console.WriteLine("GetSecurityDescriptorSddlForm:  " + fileSec.GetSecurityDescriptorSddlForm(AccessControlSections.All));

			foreach (FileSystemAccessRule ace in fileSec.GetAccessRules(true, true, typeof(NTAccount)))
			{
				Console.WriteLine("\tIdentityReference.Value: " + ace.IdentityReference.Value);
				Console.WriteLine("\tAccessControlType: " + ace.AccessControlType);
				Console.WriteLine("\tFileSystemRights: " + ace.FileSystemRights);
				Console.WriteLine("\tInheritanceFlags: " + ace.InheritanceFlags);
				Console.WriteLine("\tIsInherited: " + ace.IsInherited);
				Console.WriteLine("\tPropagationFlags: " + ace.PropagationFlags);

				Console.WriteLine("-----------------\r\n\r\n");
			}


			foreach (FileSystemAuditRule ace in fileSec.GetAuditRules(true, true, typeof(NTAccount)))
			{
				Console.WriteLine("\tIdentityReference.Value: " + ace.IdentityReference.Value);
				Console.WriteLine("\tAuditFlags: " + ace.AuditFlags);
				Console.WriteLine("\tFileSystemRights: " + ace.FileSystemRights);
				Console.WriteLine("\tInheritanceFlags: " + ace.InheritanceFlags);
				Console.WriteLine("\tIsInherited: " + ace.IsInherited);
				Console.WriteLine("\tPropagationFlags: " + ace.PropagationFlags);

				Console.WriteLine("-----------------\r\n\r\n");
			}

			Console.WriteLine("GetGroup(typeof(NTAccount)).Value: " + fileSec.GetGroup(typeof(NTAccount)).Value);
			Console.WriteLine("GetOwner(typeof(NTAccount)).Value: " + fileSec.GetOwner(typeof(NTAccount)).Value);

			Console.WriteLine("---------------------------------------\r\n\r\n\r\n");
		}
		#endregion

		#region "17.11 Granting/Revoking Access to a File or Registry Key"
		// ms-help://MS.VSCC.v80/MS.MSDNQTR.v80.en/MS.MSDN.v80/MS.NETDEVFX.v20.en/cpref/html/M_System_Security_AccessControl_FileSystemSecurity_AddAccessRule_1_609c1893.htm
		// ms-help://MS.VSCC.v80/MS.MSDNQTR.v80.en/MS.MSDN.v80/MS.NETDEVFX.v20.en/cpref/html/O_T_System_IO_File_GetAccessControl.htm
		// ms-help://MS.VSCC.v80/MS.MSDNQTR.v80.en/MS.MSDN.v80/MS.NETDEVFX.v20.en/cpref/html/M_System_IO_File_SetAccessControl_1_8d292e58.htm

		//  http://pluralsight.com/wiki/default.aspx/Keith.GuideBook/HowToProgramWithSIDs.html


		public static void TestGrantRevokeFileRights()
		{
			NTAccount user = new NTAccount("STEIHETW2K\\Debugger Users");

			string file = @"c:\BOOT.INI";
			GrantFileRights(file, user, FileSystemRights.Delete, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow);
			RevokeFileRights(file, user, FileSystemRights.Delete, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow);

			RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Compuware\Weasel\2.0");
			GrantRegKeyRights(regKey, user, RegistryRights.Notify, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Deny);
			RevokeRegKeyRights(regKey, user, RegistryRights.Notify, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Deny);
		}



		public static void GrantRegKeyRights(RegistryKey regKey, NTAccount user, RegistryRights rightsFlags, InheritanceFlags inherFlags, PropagationFlags propFlags, AccessControlType actFlags)
		{
			RegistrySecurity regSecurity = regKey.GetAccessControl();

			DisplayRegKeySecurityInfo(regSecurity);

			RegistryAccessRule rule = new RegistryAccessRule(user, rightsFlags, inherFlags, propFlags, actFlags);
			regSecurity.AddAccessRule(rule);
			regKey.SetAccessControl(regSecurity);

			DisplayRegKeySecurityInfo(regSecurity);
		}

		public static void RevokeRegKeyRights(RegistryKey regKey, NTAccount user, RegistryRights rightsFlags, InheritanceFlags inherFlags, PropagationFlags propFlags, AccessControlType actFlags)
		{
			RegistrySecurity regSecurity = regKey.GetAccessControl();

			DisplayRegKeySecurityInfo(regSecurity);

			RegistryAccessRule rule = new RegistryAccessRule(user, rightsFlags, inherFlags, propFlags, actFlags);
			regSecurity.AddAccessRule(rule);

			//System.Security.Principal.IdentityNotMappedException was unhandled
			//      Message="Some or all identity references could not be translated."
			//--Only when account is DISABLED

			regKey.SetAccessControl(regSecurity);

			DisplayRegKeySecurityInfo(regSecurity);
		}




		public static void GrantFileRights(string file, NTAccount user, FileSystemRights rightsFlags, InheritanceFlags inherFlags, PropagationFlags propFlags, AccessControlType actFlags)
		{
			FileSecurity fileSecurity = File.GetAccessControl(file);

			DisplayFileSecurityInfo(fileSecurity);

			FileSystemAccessRule rule = new FileSystemAccessRule(user, rightsFlags, inherFlags, propFlags, actFlags);
			fileSecurity.AddAccessRule(rule);
			File.SetAccessControl(file, fileSecurity);

			DisplayFileSecurityInfo(fileSecurity);
		}

		public static void RevokeFileRights(string file, NTAccount user, FileSystemRights rightsFlags, InheritanceFlags inherFlags, PropagationFlags propFlags, AccessControlType actFlags)
		{
			FileSecurity fileSecurity = File.GetAccessControl(file);

			DisplayFileSecurityInfo(fileSecurity);

			FileSystemAccessRule rule = new FileSystemAccessRule(user, rightsFlags, inherFlags, propFlags, actFlags);
			fileSecurity.RemoveAccessRuleSpecific(rule);
			File.SetAccessControl(file, fileSecurity);

			DisplayFileSecurityInfo(fileSecurity);
		}
		#endregion

		#region "17.12 Securing Your Strings"
		public static void TestSecureString()
		{
			StreamReader sr = new StreamReader("data.txt");
			SecureString secretStr = CreateSecureString(sr);

			// Cannot modify this string:  secretStr.AppendChar('x');

			// In order to read back the string you need to use some special methods
			IntPtr secretStrPtr = Marshal.SecureStringToBSTR(secretStr);
			string nonSecureStr = Marshal.PtrToStringBSTR(secretStrPtr);

			Console.WriteLine("secretStr = " + secretStr.ToString());
			Console.WriteLine("nonSecureStr = " + nonSecureStr);
		}

		public static SecureString CreateSecureString(StreamReader secretStream)
		{
			SecureString secretStr = new SecureString();
			char[] buf = new char[1];

			while (secretStream.Peek() >= 0)
			{
				int nextByte = secretStream.Read(buf, 0, 1);
				secretStr.AppendChar(buf[0]);
			}

			// Make the secretStr object read-only
			secretStr.MakeReadOnly();

			return (secretStr);
		}
		#endregion

		#region "17.13 Securing Stream Data (see AuthenticatedStream, NegotiateStream, and SslStream)"
		// ms-help://MS.VSCC.v80/MS.MSDNQTR.v80.en/MS.MSDN.v80/MS.NETDEVFX.v20.en/cpref/html/T_System_Net_Security_SslStream.htm

		//http://www.leastprivilege.com/CategoryView.aspx?category=Samples
		//http://www.devnewsgroups.net/group/microsoft.public.dotnet.framework/topic11749.aspx
		//      http://www.devnewsgroups.net/link.aspx?url=http://blogs.msdn.com/jhoward/archive/2005/02/02/365323.aspx
		//      http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cptools/html/cpgrfcertificatecreationtoolmakecertexe.asp
		//      http://www.inventec.ch/chdh/notes/14.htm
		#endregion

		#region "17.14 Encrypting web.config Information"
		public static void TestEncryptDecryptWebConfigSection()
		{
			DisplayWebConfigInfo();

			EncryptWebConfigSection("/WebApplication1", "appSettings", "DataProtectionConfigurationProvider");

			DisplayWebConfigInfo();

			DecryptWebConfigSection("/WebApplication1", "appSettings");

			DisplayWebConfigInfo();
		}

		//http://www.ondotnet.com/pub/a/dotnet/2005/02/15/encryptingconnstring.html
		//http://www.devx.com/dotnet/Article/27562/0/page/2
		//http://beta.asp.net/QUICKSTARTV20/aspnet/doc/management/mgmtapi.aspx
		//http://www.developer.com/net/vb/article.php/10926_3500906_1

		//ms-help://MS.VSCC.v80/MS.MSDNQTR.v80.en/MS.MSDN.v80/MS.VisualStudio.v80.en/dv_aspnetcon/html/e1652f90-eac5-4f51-bff1-cf1acc2e1180.htm



		public static void EncryptWebConfigSection(string appPath, string protectedSection, string dataProtectionProvider)
		{
			System.Configuration.Configuration webConfig = WebConfigurationManager.OpenWebConfiguration(appPath);
			ConfigurationSection webConfigSection = webConfig.GetSection(protectedSection);

			if (!webConfigSection.SectionInformation.IsProtected)
			{
				webConfigSection.SectionInformation.ProtectSection(dataProtectionProvider);
				webConfig.Save();
			}
		}

		public static void DecryptWebConfigSection(string appPath, string protectedSection)
		{
            System.Configuration.Configuration webConfig = WebConfigurationManager.OpenWebConfiguration(appPath);
			ConfigurationSection webConfigSection = webConfig.GetSection(protectedSection);

			if (webConfigSection.SectionInformation.IsProtected)
			{
				webConfigSection.SectionInformation.UnprotectSection();
				webConfig.Save();
			}
		}

		public static void DisplayWebConfigInfo()
		{
			//Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None, @"");
            System.Configuration.Configuration webConfig = WebConfigurationManager.OpenWebConfiguration("/WebApplication1");  //"http://localhost/WebApplication1/web.config");   //WebForm1.aspx

			Console.WriteLine("\r\n\r\n------------------------------------");
			Console.WriteLine("FilePath: " + webConfig.FilePath);
			Console.WriteLine("APP_SETTINGS");
			foreach (KeyValueConfigurationElement o in webConfig.AppSettings.Settings)
			{
				Console.WriteLine("\tKEY: " + o.Key);
				Console.WriteLine("\tVALUE: " + o.Value);
			}
			Console.WriteLine("APP_CONN_STRS");
			foreach (ConnectionStringSettings css in webConfig.ConnectionStrings.ConnectionStrings)
			{
				Console.WriteLine("\tNAME: " + css.Name);
				Console.WriteLine("\tCONN_STR: " + css.ConnectionString);
			}
			Console.WriteLine("APP_SECTION_GROUP");
			foreach (ConfigurationSectionGroup sg in webConfig.SectionGroups)
			{
				Console.WriteLine("\tNAME: " + sg.Name);
				//Console.WriteLine("\tGROUP_NAME: " + sg.SectionGroupName);
				//Console.WriteLine("\tTYPE: " + sg.Type);
			}
			Console.WriteLine("SECTIONS");
			foreach (ConfigurationSection cs in webConfig.Sections)
			{
				Console.WriteLine("\tNAME: " + cs.SectionInformation.Name);
				//Console.WriteLine("\tSECTION_NAME: " + cs.SectionInformation.SectionName);
				//Console.WriteLine("\tSOURCE: " + cs.ElementInformation.Source);
			}
		}
		#endregion

		#region "17.15 Obtaining the Full Reason a SecurityException was Thrown (see new properties on SecurityException)"
		// ms-help://MS.VSCC.v80/MS.MSDNQTR.v80.en/MS.MSDN.v80/MS.NETDEVFX.v20.en/cpref/html/T_System_Security_SecurityException.htm

		//http://blogs.msdn.com/shawnfa/archive/2004/07/30/202468.aspx
		//http://blog.hundhausen.com/ASPNET+20+SecurityException.aspx


		public static void TestSecurityException()
		{
			RunCode();
			Console.WriteLine("Press the ENTER key to exit.");
			Console.Read();
		}

		public static void RunCode()
		{
			try
			{
				// Deny a permission.
				RegistryPermission regPerm = new RegistryPermission(RegistryPermissionAccess.Read, "HKEY_LOCAL_MACHINE\\Software\\MyApp");
				regPerm.Deny();

				// Demand the denied permission and display the 
				// exception properties.
				Display("Demanding a denied permission. \n\n");
				DemandDeniedPermission();
				Display("************************************************\n");
				CodeAccessPermission.RevertDeny();

				// Demand the permission refused in the 
				// assembly-level attribute.
				Display("Demanding a refused permission. \n\n");
				DemandRefusedPermission();
				Display("************************************************\n");

				// Demand the permission implicitly refused through a 
				// PermitOnly attribute. Permit only the permission that 
				// will cause the failure and the security permissions 
				// necessary to display the results of the failure.
				PermissionSet permitOnly = new PermissionSet(PermissionState.None);
				permitOnly.AddPermission(new KeyContainerPermission(KeyContainerPermissionFlags.Import));
				permitOnly.AddPermission(new SecurityPermission(SecurityPermissionFlag.ControlEvidence |
					SecurityPermissionFlag.ControlPolicy |
					SecurityPermissionFlag.SerializationFormatter));
				permitOnly.PermitOnly();
				Display("Demanding an implicitly refused permission. \n\n");
				DemandPermitOnly();
			}
			catch (Exception sE)
			{
				Display("************************************************\n");
				Display("Displaying an exception using the ToString method: ");
				Display(sE.ToString());
			}
		}

		public static void DemandDeniedPermission()
		{
			try
			{
				RegistryPermission regPerm = new RegistryPermission(RegistryPermissionAccess.Read, "HKEY_LOCAL_MACHINE\\Software\\MyApp");
				regPerm.Demand();
			}
			catch (SecurityException sE)
			{
				Display("The denied permission is: " + ((PermissionSet)sE.DenySetInstance).ToString());
				Display("The demanded permission is: " + sE.Demanded.ToString());
				Display("The security action is: " + sE.Action.ToString());
				Display("The method is: " + sE.Method);
				Display("The permission state at the time of the exception was: " + sE.PermissionState);
				Display("The permission that failed was: " + (IPermission)sE.FirstPermissionThatFailed);
				Display("The permission type is: " + sE.PermissionType.ToString());
				Display("Demonstrating the use of the GetObjectData method.");
				SerializationInfo si = new SerializationInfo(typeof(Security), new FormatterConverter());
				sE.GetObjectData(si, new StreamingContext(StreamingContextStates.All));
				Display("The FirstPermissionThatFailed from the call to GetObjectData is: ");
				Display(si.GetString("FirstPermissionThatFailed"));
			}
		}

		public static void DemandRefusedPermission()
		{
			try
			{
				RegistryPermission regPerm = new RegistryPermission(RegistryPermissionAccess.Read, "HKEY_LOCAL_MACHINE\\Software\\MyApp");
				regPerm.Demand();
			}
			catch (SecurityException sE)
			{
				Display("The refused permission set is: " + (sE.RefusedSet).ToString());
				Display("The exception message is: " + sE.Message);
				Display("The failed assembly is: " + sE.FailedAssemblyInfo.EscapedCodeBase);
				Display("The granted set is: \n" + sE.GrantedSet);
				Display("The permission that failed is: " + sE.FirstPermissionThatFailed);
				Display("The permission type is: " + sE.PermissionType.ToString());
				Display("The source is: " + sE.Source);
			}
		}

		public static void DemandPermitOnly()
		{
			try
			{
				RegistryPermission regPerm = new RegistryPermission(RegistryPermissionAccess.Read, "HKEY_LOCAL_MACHINE\\Software\\MyApp");
				regPerm.Demand();
			}
			catch (SecurityException sE)
			{
				Display("The permitted permission is: " + ((PermissionSet)sE.PermitOnlySetInstance).ToString());
				Display("The demanded permission is: " + sE.Demanded.ToString());
				Display("The security action is: " + sE.Action.ToString());
				Display("The method is: " + sE.Method.ToString());
				Display("The permission state at the time of the exception was: " + sE.PermissionState);
				Display("The permission that failed was: " + (IPermission)sE.FirstPermissionThatFailed);
				Display("The permission type is: " + sE.PermissionType.ToString());

				//Demonstrate the SecurityException constructor by 
				// throwing the exception again.
				Display("Rethrowing the exception thrown as a result of a PermitOnly security action.");
				throw new SecurityException(sE.Message, sE.DenySetInstance, sE.PermitOnlySetInstance,
											sE.Method, sE.Demanded, (IPermission)sE.FirstPermissionThatFailed);
			}
		}
		public static void Display(string line)
		{
			Console.WriteLine(line);
		}
		#endregion

		#region "17.16 Achieving Secure Unicode Encoding"
		public static void TestUnicodeEncodingWithSecurity()
		{
			byte[] SourceArray2 = {128, 0, 83, 0, 111, 0, 117, 0, 114, 0, 99, 0, 
			                       101, 0, 32, 0, 83, 0, 116, 0, 114, 0, 105, 0, 
                                   110, 0, 103, 0, 128, 0};

			Console.WriteLine("\r\nCalling FromUnicodeByteArray...");
			Console.WriteLine(FromUnicodeByteArray(SourceArray2));

			Console.WriteLine("\r\nCalling ToUnicodeByteArray...");
			string SourceStr = "Source String";
			foreach (byte b in ToUnicodeByteArray(SourceStr))
				Console.WriteLine(b);

			Console.WriteLine();
		}


		// From recipe 2.13
		public static string FromUnicodeByteArray(byte[] characters)
		{
			UnicodeEncoding encoding = new UnicodeEncoding(false, true, true);
			string constructedString = encoding.GetString(characters);

			return (constructedString);
		}

		// From recipe 2.14
		public static byte[] ToUnicodeByteArray(string characters)
		{
			UnicodeEncoding encoding = new UnicodeEncoding(false, true, true);
			int numberOfChars = encoding.GetByteCount(characters);
			byte[] retArray = new byte[numberOfChars];

			retArray = encoding.GetBytes(characters);

			return (retArray);
		}
		#endregion

		#region "17.17 Obtaining a Safer File Handle"
		//ms-help://MS.VSCC.v80/MS.MSDNQTR.v80.en/MS.MSDN.v80/MS.NETDEVFX.v20.en/cpref/html/T_System_Runtime_InteropServices_SafeHandle.htm
		#endregion
	}
}
