using System.Collections;
using GameCreator.Runtime.Variables;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GameCreator.Tests.Core
{
    public class LocalListVariables_GameObjects
    {
        private const int INDEX_A = 0;
        private const int INDEX_B = 1;
        
        private static GameObject VALUE_A;
        private static GameObject VALUE_B;

        // PREPARE: -------------------------------------------------------------------------------
        
        private ListVariableRuntime MakeNameVariables()
        {
            VALUE_A = new GameObject($"Index_{INDEX_A}");
            VALUE_B = new GameObject($"Index_{INDEX_B}");
            
            ListVariableRuntime runtime = new ListVariableRuntime(
                ValueGameObject.TYPE_ID,
                new IndexVariable(new ValueGameObject(VALUE_A)),
                new IndexVariable(new ValueGameObject(VALUE_B))
            );

            runtime.OnStartup();
            return runtime;
        }
        
        // TESTS: ---------------------------------------------------------------------------------
        
        [UnityTest]
        public IEnumerator LocalList_GetFirst()
        {
            ListVariableRuntime runtime = this.MakeNameVariables();
            yield return null;
            
            Assert.AreEqual(VALUE_A, runtime.Get(INDEX_A));
        }

        [UnityTest]
        public IEnumerator LocalList_Set()
        {
            ListVariableRuntime runtime = this.MakeNameVariables();
            yield return null;
            
            runtime.Set(INDEX_A, VALUE_B);
            
            Assert.AreEqual(VALUE_B, runtime.Get(INDEX_A));
        }
        
        [UnityTest]
        public IEnumerator LocalList_RemoveLast()
        {
            ListVariableRuntime runtime = this.MakeNameVariables();
            yield return null;
            
            runtime.Remove(INDEX_B);
            
            Assert.AreEqual(VALUE_A, runtime.Get(INDEX_A));
        }
        
        [UnityTest]
        public IEnumerator LocalList_RemoveFirst()
        {
            ListVariableRuntime runtime = this.MakeNameVariables();
            yield return null;
            
            runtime.Remove(INDEX_A);
            
            Assert.AreEqual(VALUE_B, runtime.Get(INDEX_A));
        }
        
        [UnityTest]
        public IEnumerator LocalList_Move()
        {
            ListVariableRuntime runtime = this.MakeNameVariables();
            yield return null;
            
            runtime.Move(INDEX_A, INDEX_B);
            
            Assert.AreEqual(VALUE_B, runtime.Get(INDEX_A));
            Assert.AreEqual(VALUE_A, runtime.Get(INDEX_B));
        }
        
        [UnityTest]
        public IEnumerator LocalList_Insert()
        {
            ListVariableRuntime runtime = this.MakeNameVariables();
            yield return null;
            
            runtime.Insert(INDEX_A, VALUE_B);
            
            Assert.AreEqual(VALUE_B, runtime.Get(INDEX_A));
            Assert.AreEqual(VALUE_A, runtime.Get(INDEX_B));
        }
        
        [UnityTest]
        public IEnumerator LocalList_Push()
        {
            ListVariableRuntime runtime = this.MakeNameVariables();
            yield return null;
            
            runtime.Remove(INDEX_B);
            runtime.Push(VALUE_B);
            
            Assert.AreEqual(VALUE_A, runtime.Get(INDEX_A));
            Assert.AreEqual(VALUE_B, runtime.Get(INDEX_B));
        }
    }
}