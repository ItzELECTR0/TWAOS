using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Properties;

namespace Unity.Serialization.Binary.Tests
{
    unsafe partial class BinaryAdapterTests
    {
        interface IShape
        {
        }

        class Square : IShape
        {
        }

        class Circle : IShape
        {
        }

        interface IAnimal
        {
        }

        class Dog : IAnimal
        {
        }

        class Cat : IAnimal
        {
        }

        [GeneratePropertyBag]
        class ClassWithShapes
        {
            public IShape Shape;
            public Square Square;
            public Circle Circle;
            public IAnimal Animal;
            public Dog Dog;
            public Cat Cat;
        }

        enum TestStatus
        {
            Shape,
            Animal,
            Dog,
            Cat
        }

        class ShapeAdapter : IContravariantBinaryAdapter<IShape>
        {
            public List<TestStatus> Status;

            public void Serialize(IBinarySerializationContext context, IShape value)
            {
                Status.Add(TestStatus.Shape);
            }

            public object Deserialize(IBinaryDeserializationContext context)
            {
                return null;
            }
        }

        class AnimalAdapter : IContravariantBinaryAdapter<IAnimal>, IContravariantBinaryAdapter<Dog>, IBinaryAdapter<Cat>
        {
            public List<TestStatus> Status;

            public void Serialize(IBinarySerializationContext context, IAnimal value)
            {
                Status.Add(TestStatus.Animal);
            }

            object IContravariantBinaryAdapter<IAnimal>.Deserialize(IBinaryDeserializationContext context)
            {
                return null;
            }

            public void Serialize(IBinarySerializationContext context, Dog value)
            {
                Status.Add(TestStatus.Dog);
            }

            object IContravariantBinaryAdapter<Dog>.Deserialize(IBinaryDeserializationContext context)
            {
                return null;
            }

            public void Serialize(in BinarySerializationContext<Cat> context, Cat value)
            {
                Status.Add(TestStatus.Cat);
            }

            public Cat Deserialize(in BinaryDeserializationContext<Cat> context)
            {
                return null;
            }
        }

        [Test]
        public void SerializeAndDeserialize_WithContravariantUserDefinedAdapter_AdapterIsInvokedCorrectly()
        {
            var status = new List<TestStatus>();

            var binarySerializationParameters = new BinarySerializationParameters
            {
                UserDefinedAdapters = new List<IBinaryAdapter>
                {
                    new DummyAdapter(),
                    new ShapeAdapter {Status = status},
                    new AnimalAdapter {Status = status},
                }
            };

            var src = new ClassWithShapes
            {
                Shape = new Square(),
                Square = new Square(),
                Circle = new Circle(),
                Animal = new Cat(),
                Dog = new Dog(),
                Cat = null
            };

            using (var stream = new UnsafeAppendBuffer(16, 4, Allocator.Temp))
            {
                BinarySerialization.ToBinary(&stream, src, binarySerializationParameters);

                Assert.That(status.SequenceEqual(new[]
                {
                    TestStatus.Shape, TestStatus.Shape, TestStatus.Shape, TestStatus.Animal, TestStatus.Dog, TestStatus.Cat
                }));
            }
        }
    }
}