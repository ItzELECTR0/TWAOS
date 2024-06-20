using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Serialization.Json
{
    struct UnsafeJsonValidator
    {
        public JsonTypeStack Stack;
        public int CharBufferPosition;
        public ushort PrevChar;
        public JsonType Expected;
        public JsonType Actual;
        public int LineCount;
        public int LineStart;
        public int CharCount;
        public ushort Char;
        public JsonType PartialTokenType;
        public int PartialTokenState;
    }
    
    unsafe partial struct JsonValidator : IDisposable
    {
        const int k_ResultSuccess = 0;
        const int k_ResultEndOfStream = -1;
        const int k_ResultInvalidJson = -2;
        const int k_DefaultDepthLimit = 128;

        readonly JsonValidationType m_ValidationType;
        readonly Allocator m_Label;
        
        [NativeDisableUnsafePtrRestriction] UnsafeJsonValidator* m_Data;

        public bool IsCreated => null != m_Data;
        
        public JsonValidator(JsonValidationType validationType, Allocator label = SerializationConfiguration.DefaultAllocatorLabel)
        {
            m_ValidationType = validationType;
            m_Label = label;
            m_Data = (UnsafeJsonValidator*) UnsafeUtility.Malloc(sizeof(UnsafeJsonValidator), UnsafeUtility.AlignOf<Json.UnsafeJsonValidator>(), label);
            UnsafeUtility.MemClear(m_Data, sizeof(Json.UnsafeJsonValidator));
            m_Data->Stack = new JsonTypeStack(k_DefaultDepthLimit, label);
            Reset();
        }
        
        public void Dispose()
        {
            if (null == m_Data)
                return;
            
            m_Data->Stack.Dispose();
            UnsafeUtility.Free(m_Data, m_Label);
            m_Data = null;
        }

        public void Reset()
        {
            if (null == m_Data)
                return;
            
            m_Data->Stack.Clear();
            m_Data->CharBufferPosition = 0;
            m_Data->PrevChar = '\0';
            m_Data->Expected = JsonType.Value;
            m_Data->Actual = JsonType.Undefined;
            m_Data->LineCount = 1;
            m_Data->LineStart = -1;
            m_Data->CharCount = 1;
            m_Data->Char = '\0';
            m_Data->PartialTokenType = JsonType.Undefined;
            m_Data->PartialTokenState = 0;
        }
        
        public JsonValidationResult Validate(UnsafeBuffer<char> buffer, int start, int count)
        {
            m_Data->CharBufferPosition = start;

            switch (m_ValidationType)
            {
                case JsonValidationType.None:
                    return default;
                
                case JsonValidationType.Standard:
                {
                    new StandardJsonValidation
                    {
                        Data = m_Data,
                        CharBuffer = (ushort*) buffer.Buffer,
                        CharBufferLength = start + count
                    }.Validate();
                    
                    break;
                }

                case JsonValidationType.Simple:
                {
                    new SimpleJsonValidation
                    {
                        Data = m_Data,
                        CharBuffer = (ushort*)buffer.Buffer,
                        CharBufferLength = start + count
                    }.Validate();

                    break;
                }
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return GetResult();
        }
        
        public JsonValidationResult GetResult()
        {
            return new JsonValidationResult
            {
                ValidationType = m_ValidationType,
                ExpectedType = m_Data->Expected,
                ActualType = m_Data->Actual,
                Char = (char) m_Data->Char,
                LineCount = m_Data->LineCount,
                CharCount = m_Data->CharCount
            };
        }
    }
}