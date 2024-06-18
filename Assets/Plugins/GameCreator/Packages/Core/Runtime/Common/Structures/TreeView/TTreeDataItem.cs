using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class TTreeDataItem<T> where T : class
    {
	    public const string NAME_ID = nameof(m_Id);
	    public const string NAME_VALUE = nameof(m_Value);
			
	    // MEMBERS: -------------------------------------------------------------------------------
			
	    [SerializeField] private int m_Id;
	    [SerializeReference] private T m_Value;
			
	    // PROPERTIES: ----------------------------------------------------------------------------

	    public int Id => this.m_Id;

	    public T Value
	    {
		    get => this.m_Value;
		    set => this.m_Value = value;
	    }

	    // CONSTRUCTORS: --------------------------------------------------------------------------

	    public TTreeDataItem(int nodeId, T value)
	    {
		    this.m_Id = nodeId;
		    this.m_Value = value;
	    }
    }
}