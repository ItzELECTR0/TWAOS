using System.Collections.Generic;
using NUnit.Framework;
using Unity.Properties;

namespace Unity.Serialization.Json.Tests
{
    partial class JsonAdapterTests
    {
        interface IShape
        {
            
        }

        [GeneratePropertyBag]
        class Square : IShape
        {
            
        }
        
        [GeneratePropertyBag]
        class Circle : IShape
        {
            
        }

        interface IAnimal
        {
            
        }

        [GeneratePropertyBag]
        class Dog : IAnimal
        {
            
        }

        [GeneratePropertyBag]
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

        class ShapeAdapter : IContravariantJsonAdapter<IShape>
        {
            public void Serialize(IJsonSerializationContext context, IShape value)
            {
                context.Writer.WriteValue("a shape");
            }

            public object Deserialize(IJsonDeserializationContext context)
            {
                return null;
            }
        }

        class AnimalAdapter : 
            IContravariantJsonAdapter<IAnimal>,
            IContravariantJsonAdapter<Dog>,
            IJsonAdapter<Cat>
        {
            public void Serialize(IJsonSerializationContext context, IAnimal value)
            {
                context.Writer.WriteValue("an animal");
            }

            object IContravariantJsonAdapter<IAnimal>.Deserialize(IJsonDeserializationContext context)
            {
                return null;
            }
            
            public void Serialize(IJsonSerializationContext context, Dog value)
            {
                context.Writer.WriteValue("a dog");
            }

            object IContravariantJsonAdapter<Dog>.Deserialize(IJsonDeserializationContext context)
            {
                return null;
            }

            public void Serialize(in JsonSerializationContext<Cat> context, Cat value)
            {
                context.Writer.WriteValue("a cat");
            }

            public Cat Deserialize(in JsonDeserializationContext<Cat> context)
            {
                return null;
            }
        }

        [Test]
        public void SerializeAndDeserialize_WithContravariantUserDefinedAdapter_AdapterIsInvokedCorrectly()
        {
            var jsonSerializationParameters = new JsonSerializationParameters
            {
                UserDefinedAdapters = new List<IJsonAdapter>
                {
                    new DummyAdapter(),
                    new ShapeAdapter(),
                    new AnimalAdapter()
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

            var json = JsonSerialization.ToJson(src, jsonSerializationParameters);

            Assert.That(UnFormat(json), Is.EqualTo(@"{""Shape"":""a shape"",""Square"":""a shape"",""Circle"":""a shape"",""Animal"":""an animal"",""Dog"":""a dog"",""Cat"":""a cat""}"));
        }
    }
}