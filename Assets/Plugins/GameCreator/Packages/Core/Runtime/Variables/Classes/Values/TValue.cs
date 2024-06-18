using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Value Type")]
    [Image(typeof(IconCircleOutline), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public abstract class TValue : TPolymorphicItem<TValue>
    {
        protected class TypeData
        {
            public readonly Type type;
            public readonly Func<object, TValue> callback;

            public TypeData(Type type, Func<object, TValue> callback)
            {
                this.type = type;
                this.callback = callback;
            }
        }
        
        private class Type_LUT : Dictionary<IdString, TypeData>
        { }
        
        private class ID_LUT : Dictionary<Type, IdString>
        { }
        
        private class ConverterType_LUT : Dictionary<Type, IdString>
        { }
        
        private static readonly Type_LUT LUT_ID_TO_DATA = new Type_LUT();
        private static readonly ID_LUT LUT_TYPE_TO_ID = new ID_LUT();
        
        private static readonly ConverterType_LUT LUT_CONVERTER = new ConverterType_LUT();
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public object Value
        {
            get => this.Get();
            set
            {
                if (this.Get() == value) return;
                
                this.Set(value);
                this.EventChange?.Invoke(this.Get());
            }
        }

        public override string Title => this.ToString();

        public abstract IdString TypeID { get; }
        public abstract Type Type { get; }
        
        public abstract bool CanSave { get; }
        public abstract TValue Copy { get; }
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected abstract object Get();
        protected abstract void Set(object value);

        public abstract override string ToString();

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<object> EventChange;
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        // CREATION METHODS: ----------------------------------------------------------------------

        protected static void RegisterValueType(IdString typeID, TypeData data, Type convertFrom)
        {
            LUT_ID_TO_DATA.TryAdd(typeID, data);
            LUT_TYPE_TO_ID.TryAdd(data.type, typeID);
            
            if (convertFrom == null) return;
            LUT_CONVERTER.TryAdd(convertFrom, typeID);
        }

        public static TValue CreateValue(IdString typeID, object value = default)
        {
            return LUT_ID_TO_DATA.TryGetValue(typeID, out TypeData data)
                ? data.callback(value) 
                : new ValueNull();
        }

        public static Type GetType(IdString typeID)
        {
            return LUT_ID_TO_DATA.TryGetValue(typeID, out TypeData data)
                ? data.type 
                : typeof(ValueNull);
        }

        public static IdString GetTypeIDFromValueType(Type type)
        {
            return LUT_TYPE_TO_ID.TryGetValue(type, out IdString typeID)
                ? typeID
                : ValueNull.TYPE_ID;
        }

        public static IdString GetTypeIDFromObjectType(Type objectType)
        {
            return LUT_CONVERTER.TryGetValue(objectType, out IdString typeID)
                ? typeID
                : ValueNull.TYPE_ID;
        }
    }
}