using System;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;

namespace EncryptDecrypt
{
    public class Program
    {
        public static XmlDocument Encrypt(XmlDocument Doc, string ElementToEncrypt, string KeyName)
        {
            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = "XML_ENC_RSA_KEY";
            RSA Alg = new RSACryptoServiceProvider(cspParams);
            // Check the arguments.  
            if (Doc == null)
                throw new ArgumentNullException("Doc");
            if (ElementToEncrypt == null)
                throw new ArgumentNullException("ElementToEncrypt");

            XmlElement elementToEncrypt = Doc.GetElementsByTagName(ElementToEncrypt)[0] as XmlElement;

            // Throw an XmlException if the element was not found.
            if (elementToEncrypt == null)
            {
                throw new XmlException("The specified element was not found");
            }

            RijndaelManaged sessionKey = new RijndaelManaged();
            sessionKey.KeySize = 256;

            EncryptedXml eXml = new EncryptedXml();

            byte[] encryptedElement = eXml.EncryptData(elementToEncrypt, sessionKey, false);

            EncryptedData edElement = new EncryptedData();
            edElement.Type = EncryptedXml.XmlEncElementUrl;

            // Create an EncryptionMethod element so that the
            // receiver knows which algorithm to use for decryption.

            edElement.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncAES256Url);

            // Encrypt the session key and add it to an EncryptedKey element.
            EncryptedKey ek = new EncryptedKey();

            byte[] encryptedKey = EncryptedXml.EncryptKey(sessionKey.Key, Alg, false);

            ek.CipherData = new CipherData(encryptedKey);

            ek.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncRSA15Url);

            // Set the KeyInfo element to specify the
            // name of the RSA key.

            // Create a new KeyInfo element.
            edElement.KeyInfo = new KeyInfo();

            // Create a new KeyInfoName element.
            KeyInfoName kin = new KeyInfoName();

            // Specify a name for the key.
            kin.Value = KeyName;

            // Add the KeyInfoName element to the 
            // EncryptedKey object.
            ek.KeyInfo.AddClause(kin);

            // Add the encrypted key to the 
            // EncryptedData object.

            edElement.KeyInfo.AddClause(new KeyInfoEncryptedKey(ek));

            // Add the encrypted element data to the 
            // EncryptedData object.
            edElement.CipherData.CipherValue = encryptedElement;

            EncryptedXml.ReplaceElement(elementToEncrypt, edElement, false);

            return Doc;
        }

        public static XmlDocument Decrypt(XmlDocument Doc, string KeyName)
        {
            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = "XML_ENC_RSA_KEY";
            RSA Alg = new RSACryptoServiceProvider(cspParams);

            // Check the arguments.  
            if (Doc == null)
                throw new ArgumentNullException("Doc");           
            if (KeyName == null)
                throw new ArgumentNullException("KeyName");

            // Create a new EncryptedXml object.
            EncryptedXml exml = new EncryptedXml(Doc);

            // Add a key-name mapping.
            // This method can only decrypt documents
            // that present the specified key name.
            exml.AddKeyNameMapping(KeyName, Alg);

            // Decrypt the element.
            exml.DecryptDocument();

            return Doc;
        }
    }
}
