namespace GameCreator.Runtime.Common.SaveSystem
{
    public interface IDataEncryption
    {
        string Encrypt(string input);
        string Decrypt(string input);
    }
}