﻿using System.ComponentModel;
using System.Security.Cryptography;

namespace MDMLibrary.Crypto
{
    public class CryptoNetInfo
    {
        public EncryptionType EncryptionType { get; set; }
        public KeyType KeyType { get; set; }
        public RsaDetail? RsaDetail { get; set; }
        public AesDetail? AesDetail { get; set; }
    }

    public class RsaDetail
    {
        public RSA? Rsa { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] PrivateKey { get; set; }
    }

    public class AesDetail
    {
        public AesDetail(byte[] key, byte[] iv)
        {
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException(nameof(iv));
            }

            AesKeyValue = new AesKeyValue()
            {
                Key = key,
                Iv = iv
            };
        }

        public Aes? Aes { get; set; }
        public AesKeyValue AesKeyValue { get; set; }
    }

    [Serializable()]
    public class AesKeyValue
    {
        [System.Xml.Serialization.XmlElement("key")]
        public byte[] Key { get; set; }

        [System.Xml.Serialization.XmlElement("iv")]
        public byte[] Iv { get; set; }

    }

    public enum KeyType
    {
        [Description("Key does not exist.")]
        NotSet,

        [Description("Symmetric key is set.")]
        SymmetricKey,

        [Description("Public key is set.")]
        PublicKey,

        [Description("Asymmertric key (both public and private) are set.")]
        PrivateKey
    }

    public enum EncryptionType
    {
        [Description("Rsa encryption.")]
        Rsa,

        [Description("Aes encryption.")]
        Aes
    }
}
