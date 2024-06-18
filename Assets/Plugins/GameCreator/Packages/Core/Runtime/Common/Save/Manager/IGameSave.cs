using System;
using System.Threading.Tasks;

namespace GameCreator.Runtime.Common
{
	public interface IGameSave
	{
        string SaveID { get; }
        bool IsShared { get; }

        Type SaveType { get; }
        object GetSaveData(bool includeNonSavable);

		LoadMode LoadMode { get; }
		Task OnLoad(object value);
	}
}