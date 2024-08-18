using System;
using System.Threading.Tasks;

namespace GameCreator.Runtime.Common.SaveSystem
{
    public interface IDataStorage
    {
        Task WithEncryption(IDataEncryption encryption);
        
        Task DeleteAll();
        Task DeleteKey(string key);
        
        Task<bool> HasKey(string key);
        
        Task<object> Get(string key, Type type);
        Task Set(string key, object value);
        
        Task Commit();
    }
}