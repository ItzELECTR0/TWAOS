using System.Collections.Generic;
using NUnit.Framework;
using Unity.Properties;

namespace Unity.Serialization.Tests
{
    [TestFixture]
    partial class SerializationTestFixture
    {
        
        [GeneratePropertyBag]
        class ConcreteA : IBase
        {
            public List<string> List = new List<string>();
        }

        interface IBase
        {
        }

        [GeneratePropertyBag]
        class ClassWithPolymorphicArray
        {
            public object ArrayOfInterfaces;
        }

        [Test]
        public void ClassWithObject_WhenValueIsInt32_CanBeSerializedAndDeserialized()
        {
            var src = new ClassWithPolymorphicFields
            {
                ObjectValue = 1
            };

            var dst = SerializeAndDeserialize(src);

            Assert.That(dst, Is.Not.SameAs(src));
            Assert.That(dst.ObjectValue, Is.EqualTo(src.ObjectValue));
        }

        [Test]
        public void ClassWithPolymorphicArray_CanBeSerializedAndDeserialized()
        {
            PropertyBag.RegisterArray<ClassWithPolymorphicArray, IBase>();
            
            var src = new ClassWithPolymorphicArray
            {
                ArrayOfInterfaces = new IBase[]
                {
                    new ConcreteA
                    {
                        List = new List<string>
                        {
                            "A",
                            "B",
                            "C"
                        }
                    },
                    new ConcreteA()
                }
            };

            var dst = SerializeAndDeserialize(src);

            Assert.That(dst, Is.Not.SameAs(src));
            Assert.That(dst.ArrayOfInterfaces is IBase[], Is.True);

            var arr = dst.ArrayOfInterfaces as IBase[];
            
            Assert.That(arr.Length, Is.EqualTo(2));
            Assert.That(arr[0] is ConcreteA, Is.True);
        }
    }
}