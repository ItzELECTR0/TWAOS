using System;
using System.Collections.Generic;

namespace Unity.Serialization.Json
{
    [Flags]
    enum JsonAdapterFilter
    {
        None = 0,
        
        UserDefined = 1 << 0,
        Global      = 1 << 1,
        Internal    = 1 << 2,
            
        All = ~0
    }

    struct JsonAdapterCollection
    {
        /// <summary>
        /// Enumerates a set of adapters in a pre-defined order. This will iterate user, global and finally internal.
        /// </summary>
        public struct Enumerator
        {
            enum State
            {
                User,
                Global,
                Internal,
                End
            }

            readonly List<IJsonAdapter> m_UserDefinedAdapters;
            readonly List<IJsonAdapter> m_GlobalAdapters;
            readonly JsonAdapter m_InternalAdapter;
            
            IJsonAdapter m_Current;
            State m_State;
            int m_Index;

            public Enumerator(List<IJsonAdapter> userDefinedAdapters, List<IJsonAdapter> globalAdapters, JsonAdapter internalAdapter)
            {
                m_UserDefinedAdapters = userDefinedAdapters;
                m_GlobalAdapters = globalAdapters;
                m_InternalAdapter = internalAdapter;
                m_Current = null;
                m_State = null != userDefinedAdapters ? State.User : null != globalAdapters ? State.Global : null != internalAdapter ? State.Internal : State.End;
                m_Index = -1;
            }

            public IJsonAdapter Current => m_Current;

            public bool MoveNext()
            {
                for (;;)
                {
                    m_Index++;
                    
                    switch (m_State)
                    {
                        case State.User:
                            if (m_Index < m_UserDefinedAdapters.Count)
                            {
                                m_Current = m_UserDefinedAdapters[m_Index];
                                return true;
                            }
                            m_State = null != m_GlobalAdapters ? State.Global : null != m_InternalAdapter ? State.Internal : State.End;
                            m_Index = -1;
                            break;
                        case State.Global:
                            if (m_Index < m_GlobalAdapters.Count)
                            {
                                m_Current = m_GlobalAdapters[m_Index];
                                return true;
                            }
                            m_State = null != m_InternalAdapter ? State.Internal : State.End;
                            m_Index = -1;
                            break;
                        case State.Internal:
                            m_Current = m_InternalAdapter;
                            m_State = State.End;
                            m_Index = -1;
                            return true;
                        case State.End:
                            m_Index = -1;
                            return false;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
        
        public JsonAdapter InternalAdapter;
        public List<IJsonAdapter> Global;
        public List<IJsonAdapter> UserDefined;
        
        public Enumerator GetEnumerator(JsonAdapterFilter filter = JsonAdapterFilter.All) 
            => new Enumerator(filter.HasFlag(JsonAdapterFilter.UserDefined) ? UserDefined : null, filter.HasFlag(JsonAdapterFilter.Global) ? Global : null, filter.HasFlag(JsonAdapterFilter.Internal) ? InternalAdapter : null);
        
        public static bool ContainsPrimitiveOrStringAdapter(List<IJsonAdapter> adapters)
        {
            if (null == adapters)
                return false;
            
            foreach (var adapter in adapters)
            {
                switch (adapter)
                {
                    case IJsonAdapter<sbyte> _:
                    case IJsonAdapter<short> _:
                    case IJsonAdapter<int> _:
                    case IJsonAdapter<long> _:
                    case IJsonAdapter<byte> _:
                    case IJsonAdapter<ushort> _:
                    case IJsonAdapter<uint> _:
                    case IJsonAdapter<ulong> _:
                    case IJsonAdapter<float> _:
                    case IJsonAdapter<double> _:
                    case IJsonAdapter<bool> _:
                    case IJsonAdapter<char> _:
                    case IJsonAdapter<string> _:
                        return true;
                }
            }

            return false;
        }
    }
}
