using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
	[Serializable]
	public abstract class TSerializableMatrix2D<T> : ISerializationCallbackReceiver
	{
		// MEMBERS: -------------------------------------------------------------------------------

		[NonSerialized] protected T[,] m_Matrix;

		[SerializeField] private int m_MatrixWidth;
		[SerializeField] private T[] m_MatrixUnwrapped;

		// PROPERTIES: ----------------------------------------------------------------------------

		public int MatrixWidth => this.m_Matrix.GetLength(0);
		public int MatrixHeight => this.m_Matrix.GetLength(1);

		public T this[int index1, int index2]
		{
			get => this.m_Matrix[index1, index2];
			set => this.m_Matrix[index1, index2] = value;
		}
		
		public T this[Vector2Int position]
		{
			get => this.m_Matrix[position.x, position.y];
			set => this.m_Matrix[position.x, position.y] = value;
		}
		
		// CONSTRUCTOR: ---------------------------------------------------------------------------

		protected TSerializableMatrix2D() : this(0, 0)
		{ }
		
		protected TSerializableMatrix2D(int width, int height)
		{
			this.m_Matrix = new T[width, height];

			this.m_MatrixWidth = width;
			this.m_MatrixUnwrapped = new T[width * height];
		}
		
		// PUBLIC METHODS: ------------------------------------------------------------------------

		public bool TryGet(int index1, int index2, out T value)
		{
			value = default;
			
			if (index1 < 0 || index1 >= this.m_Matrix.GetLength(0)) return false;
			if (index2 < 0 || index2 >= this.m_Matrix.GetLength(1)) return false;

			value = this.m_Matrix[index1, index2];
			return true;
		}
		
		public bool TryGet(Vector2Int position, out T value)
		{
			return this.TryGet(position.x, position.y, out value);
		}

		// SERIALIZATION CALLBACKS: ---------------------------------------------------------------

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (AssemblyUtils.IsReloading) return;
			
			int width = this.m_MatrixWidth;
			int height = width > 0 ? this.m_MatrixUnwrapped.Length / this.m_MatrixWidth : 0;
			
			this.m_Matrix = new T[width, height];
			
			for (int i = 0; i < width; ++i)
			{
				for (int j = 0; j < height; ++j)
				{
					T value = this.m_MatrixUnwrapped[i + j * width];
					this.m_Matrix[i, j] = value;
				}
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (AssemblyUtils.IsReloading) return;
			
			int width = this.MatrixWidth;
			int height = this.MatrixHeight;
			
			this.m_MatrixWidth = width;
			this.m_MatrixUnwrapped = new T[this.m_Matrix.Length];

			for (int i = 0; i < width; ++i)
			{
				for (int j = 0; j < height; ++j)
				{
					T value = this.m_Matrix[i, j];
					this.m_MatrixUnwrapped[i + j * width] = value;
				}
			}
		}
	}
}