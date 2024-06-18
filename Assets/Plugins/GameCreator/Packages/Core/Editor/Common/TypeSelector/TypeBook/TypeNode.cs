using System;
using System.Linq;
using System.Reflection;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    internal abstract class TypeNode
    {
        public string Name { get; }

        protected TypeNode(string name)
        {
            this.Name = name;
        }
    }
    
    internal class TypeNodeFolder : TypeNode
    {
        public TypePage Page { get; }

        public TypeNodeFolder(string name, Trie<Type> trie) : base(name)
        {
            this.Page = new TypePage(trie, false);
        }
    }

    internal class TypeNodeValue : TypeNode
    {
        public Type Value { get; }

        private Texture _texture;
        public Texture Texture
        {
            get
            {
                if (this._texture == null)
                {
                    ImageAttribute icon = this.Value
                        .GetCustomAttributes<ImageAttribute>()
                        .FirstOrDefault();
                    
                    this._texture = icon?.Image;
                }
                
                return this._texture;
            }
        }

        public TypeNodeValue(Type type) 
            : base(type.GetCustomAttributes<CategoryAttribute>()
                .FirstOrDefault()?.Name ?? TypeUtils.GetNiceName(type))
        {
            this.Value = type;
        }
    }
}
