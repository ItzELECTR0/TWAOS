using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Common
{
    internal class TypeChapter
    {
        public TypePage First { get; }

        public TypeChapter(Type type)
        {
            Trie<Type> trie = TypeUtils.GetTypesTree(type);
            this.First = new TypePage(trie, false);
        }
    }
}
