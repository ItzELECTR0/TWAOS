using System.Collections;
using GameCreator.Runtime.Variables;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GameCreator.Tests.Core
{
    public class LocalNameVariables_GameObjects
    {
        private const string NAME_A = "variable_A";
        private const string NAME_B = "variable_B";

        private static GameObject VALUE_A;
        private static GameObject VALUE_B;

        // PREPARE: -------------------------------------------------------------------------------
        
        private NameVariableRuntime MakeNameVariables()
        {
            VALUE_A = new GameObject(NAME_A);
            VALUE_B = new GameObject(NAME_B);
            
            NameVariableRuntime runtime = new NameVariableRuntime(
                new NameVariable(NAME_A, new ValueGameObject(VALUE_A)),
                new NameVariable(NAME_B, new ValueGameObject(VALUE_B))
            );

            runtime.OnStartup();
            return runtime;
        }
        
        // TESTS: ---------------------------------------------------------------------------------
        
        [UnityTest]
        public IEnumerator LocalName_Get()
        {
            NameVariableRuntime runtime = this.MakeNameVariables();
            yield return null;
            
            Assert.AreEqual(
                VALUE_A,
                runtime.Get(NAME_A)
            );
        }

        [UnityTest]
        public IEnumerator LocalName_SetNull()
        {
            NameVariableRuntime runtime = this.MakeNameVariables();
            yield return null;
            
            runtime.Set(NAME_A, null);
            Assert.IsNull(runtime.Get(NAME_A));
        }
        
        [UnityTest]
        public IEnumerator LocalName_SetValue()
        {
            NameVariableRuntime runtime = this.MakeNameVariables();
            yield return null;
            
            runtime.Set(NAME_A, VALUE_B);
            Assert.AreEqual(
                VALUE_B,
                runtime.Get(NAME_B)
            );
        }
    }
}