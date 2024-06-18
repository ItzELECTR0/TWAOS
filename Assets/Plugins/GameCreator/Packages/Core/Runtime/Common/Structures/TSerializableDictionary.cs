using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
	[Serializable]
	public abstract class TSerializableDictionary<TKey, TValue>
        : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		public const string NAME_KEYS = nameof(m_Keys);
		public const string NAME_VALUES = nameof(m_Values);
		
		// MEMBERS: -------------------------------------------------------------------------------

		[NonSerialized] 
		protected Dictionary<TKey, TValue> m_Dictionary = new Dictionary<TKey, TValue>();

		[SerializeField] private TKey[] m_Keys;
		[SerializeField] private TValue[] m_Values;

		// PROPERTIES: ----------------------------------------------------------------------------
		
		public int Count => this.m_Dictionary.Count;
		
		public ICollection<TKey> Keys => this.m_Dictionary.Keys;
		public ICollection<TValue> Values => this.m_Dictionary.Values;

		public TValue this[TKey key]
		{
			get => this.m_Dictionary[key];
			set => this.m_Dictionary[key] = value;
		}
		
		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public void Add(TKey key, TValue value)
		{
			this.m_Dictionary.Add(key, value);
		}

		public bool ContainsKey(TKey key)
		{
			return this.m_Dictionary.ContainsKey(key);
		}

		public bool Remove(TKey key)
		{
			return this.m_Dictionary.Remove(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return this.m_Dictionary.TryGetValue(key, out value);
		}

		public void Clear()
		{
			this.m_Dictionary.Clear();
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			(this.m_Dictionary as ICollection<KeyValuePair<TKey, TValue>>).Add(item);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return (this.m_Dictionary as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			(this.m_Dictionary as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			return (this.m_Dictionary as ICollection<KeyValuePair<TKey, TValue>>).Remove(item);
		}

		public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
		{
			return this.m_Dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.m_Dictionary.GetEnumerator();
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return this.m_Dictionary.GetEnumerator();
		}

		// SERIALIZATION CALLBACKS: ---------------------------------------------------------------

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (AssemblyUtils.IsReloading) return;
			this.BeforeSerialize();
		}
		
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (AssemblyUtils.IsReloading) return;
			this.AfterSerialize();
		}

		protected virtual void BeforeSerialize()
		{
			if (this.m_Dictionary == null || this.m_Dictionary.Count == 0)
			{
				this.m_Keys = null;
				this.m_Values = null;
			}
			else
			{
				int count = this.m_Dictionary.Count;
				this.m_Keys = new TKey[count];
				this.m_Values = new TValue[count];
				int i = 0;

				using Dictionary<TKey, TValue>.Enumerator dict = this.m_Dictionary.GetEnumerator();
				while (dict.MoveNext())
				{
					this.m_Keys[i] = dict.Current.Key;
					this.m_Values[i] = dict.Current.Value;
					i++;
				}
			}
		}

		protected virtual void AfterSerialize()
		{
			this.m_Dictionary ??= new Dictionary<TKey, TValue>();
			this.m_Dictionary.Clear();
			
			if (this.m_Keys == null || this.m_Values == null) return;
			
			for (int i = 0; i < this.m_Keys.Length; i++)
			{
				this.m_Dictionary[this.m_Keys[i]] = i < this.m_Values.Length
					? this.m_Values[i]
					: default;
			}
		}
	}
}