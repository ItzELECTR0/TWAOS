using System;
using System.Threading.Tasks;

namespace GameCreator.Runtime.Common.SaveSystem
{
    [Title("Save System")]
    
    public interface IDataStorage
    {
        string Title { get; }
        string Description { get; }

        Task DeleteAll();

        Task DeleteKey(string key);
        Task<bool> HasKey(string key);

        Task<object> GetBlob(string key, Type type, object value);
        Task SetBlob(string key, object value);

        Task<string> GetString(string key, string value);
        Task SetString(string key, string value);

        Task<double> GetDouble(string key, double value);
        Task SetDouble(string key, double value);

        Task<int> GetInt(string key, int value);
        Task SetInt(string key, int value);

        Task Commit();
    }
}