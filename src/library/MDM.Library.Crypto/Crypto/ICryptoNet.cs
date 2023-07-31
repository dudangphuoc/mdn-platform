namespace MDMLibrary.Crypto
{
    public interface ICrypto
    {
        CryptoNetInfo Info { get; }
        string ExportKey(bool? privateKey = null);
        void ExportKeyAndSave(FileInfo fileInfo, bool? privateKey = false);
        byte[] EncryptFromString(string content);
        string DecryptToString(byte[] bytes);
        byte[] EncryptFromBytes(byte[] bytes);
        byte[] DecryptToBytes(byte[] bytes);
    }
}
