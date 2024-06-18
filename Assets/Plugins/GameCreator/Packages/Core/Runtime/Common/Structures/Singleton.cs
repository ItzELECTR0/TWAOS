using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
	[AddComponentMenu("")]
	[DisallowMultipleComponent]
	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
        [field: NonSerialized] private static T _Instance { get; set; }

        public static T Instance
		{
			get
			{
				if (_Instance == null)
				{
					if (ApplicationManager.IsExiting) return null;
					
					GameObject singleton = new GameObject();
					_Instance = singleton.AddComponent<T>();
					
					string name = TextUtils.Humanize(typeof(T).Name);
					singleton.name = $"{name} (singleton)";

                    Singleton<T> component = _Instance.GetComponent<Singleton<T>>();
					component.OnCreate();

					if (component.SurviveSceneLoads) DontDestroyOnLoad(singleton);
				}
				
				return _Instance;
			}
		}

        // PROTECTED METHODS: ---------------------------------------------------------------------
		
		protected void WakeUp()
		{ }

		// VIRTUAL METHODS: -----------------------------------------------------------------------

		protected virtual void OnCreate()
		{ }

		protected virtual bool SurviveSceneLoads => true;

		// PRIVATE METHODS: -----------------------------------------------------------------------

		private void OnDestroy()
		{
			_Instance = null;
		}
	}
}