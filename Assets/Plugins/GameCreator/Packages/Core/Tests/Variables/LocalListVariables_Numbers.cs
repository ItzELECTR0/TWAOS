using GameCreator.Runtime.Variables;
using NUnit.Framework;

namespace GameCreator.Tests.Core
{
    public class LocalListVariables_Numbers
    {
        private const int INDEX_A = 0;
        private const int INDEX_B = 1;
        
        private const double VALUE_A = 3;
        private const double VALUE_B = 5;

        // PREPARE: -------------------------------------------------------------------------------
        
        private ListVariableRuntime MakeNameVariables()
        {
            ListVariableRuntime runtime = new ListVariableRuntime(
                ValueNumber.TYPE_ID,
                new IndexVariable(new ValueNumber(VALUE_A)),
                new IndexVariable(new ValueNumber(VALUE_B))
            );

            runtime.OnStartup();
            return runtime;
        }
        
        // TESTS: ---------------------------------------------------------------------------------
        
        [Test]
        public void LocalList_GetFirst()
        {
            ListVariableRuntime runtime = this.MakeNameVariables();
            Assert.AreEqual(VALUE_A, runtime.Get(INDEX_A));
        }

        [Test]
        public void LocalList_IterationSum()
        {
            ListVariableRuntime runtime = this.MakeNameVariables();

            double sum = 0f;
            for (int i = 0; i < runtime.Count; i++)
            {
                sum += (double) runtime.Get(i);
            }

            Assert.AreEqual(VALUE_A + VALUE_B, sum);
        }

        [Test]
        public void LocalList_Set()
        {
            ListVariableRuntime runtime = this.MakeNameVariables();
            runtime.Set(INDEX_A, VALUE_B);
            
            Assert.AreEqual(VALUE_B, runtime.Get(INDEX_A));
        }
        
        [Test]
        public void LocalList_RemoveLast()
        {
            ListVariableRuntime runtime = this.MakeNameVariables();
            runtime.Remove(INDEX_B);
            
            Assert.AreEqual(VALUE_A, runtime.Get(INDEX_A));
        }
        
        [Test]
        public void LocalList_RemoveFirst()
        {
            ListVariableRuntime runtime = this.MakeNameVariables();
            runtime.Remove(INDEX_A);
            
            Assert.AreEqual(VALUE_B, runtime.Get(INDEX_A));
        }
        
        [Test]
        public void LocalList_Move()
        {
            ListVariableRuntime runtime = this.MakeNameVariables();
            runtime.Move(INDEX_A, INDEX_B);
            
            Assert.AreEqual(VALUE_B, runtime.Get(INDEX_A));
            Assert.AreEqual(VALUE_A, runtime.Get(INDEX_B));
        }
        
        [Test]
        public void LocalList_Insert()
        {
            ListVariableRuntime runtime = this.MakeNameVariables();
            runtime.Insert(INDEX_A, VALUE_B);
            
            Assert.AreEqual(VALUE_B, runtime.Get(INDEX_A));
            Assert.AreEqual(VALUE_A, runtime.Get(INDEX_B));
        }
        
        [Test]
        public void LocalList_Push()
        {
            ListVariableRuntime runtime = this.MakeNameVariables();
            runtime.Remove(INDEX_B);
            runtime.Push(VALUE_B);
            
            Assert.AreEqual(VALUE_A, runtime.Get(INDEX_A));
            Assert.AreEqual(VALUE_B, runtime.Get(INDEX_B));
        }
    }
}