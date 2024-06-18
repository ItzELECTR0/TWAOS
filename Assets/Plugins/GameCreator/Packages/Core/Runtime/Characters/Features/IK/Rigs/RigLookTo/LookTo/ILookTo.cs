using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Characters.IK
{
    public interface ILookTo
    {
        int Layer { get; }
        bool Exists { get; }
        
        Vector3 Position { get; }
        GameObject Target { get; }
    }
    
    internal class ILookToComparer : IComparer<ILookTo>
    {
        public int Compare(ILookTo x, ILookTo y)
        {
            return (x?.Layer ?? 0).CompareTo(y?.Layer ?? 0);
        }
    }
}