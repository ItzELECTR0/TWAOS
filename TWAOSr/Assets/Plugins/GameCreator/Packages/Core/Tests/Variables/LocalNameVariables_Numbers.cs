using GameCreator.Runtime.Variables;
using NUnit.Framework;

namespace GameCreator.Tests.Core
{
    public class LocalNameVariables_Numbers
    {
        private const string NAME_A = "variable_A";
        private const string NAME_B = "variable_B";

        private const double VALUE_A = 3;
        private const double VALUE_B = 5;

        // PREPARE: -------------------------------------------------------------------------------
        
        private NameVariableRuntime MakeNameVariables()
        {
            NameVariableRuntime runtime = new NameVariableRuntime(
                new NameVariable(NAME_A, new ValueNumber(VALUE_A)),
                new NameVariable(NAME_B, new ValueNumber(VALUE_B))
            );

            runtime.OnStartup();
            return runtime;
        }
        
        // TESTS: ---------------------------------------------------------------------------------
        
        [Test]
        public void LocalName_Get()
        {
            NameVariableRuntime runtime = this.MakeNameVariables();
            Assert.AreEqual(VALUE_A, runtime.Get(NAME_A));
        }
        
        [Test]
        public void LocalName_Sum()
        {
            NameVariableRuntime runtime = this.MakeNameVariables();
            double valueA = (double) runtime.Get(NAME_A);
            double valueB = (double) runtime.Get(NAME_B);
            
            Assert.AreEqual(
                VALUE_A + VALUE_B,
                valueA + valueB
            );
        }
        
        [Test]
        public void LocalName_Set()
        {
            NameVariableRuntime runtime = this.MakeNameVariables();
            runtime.Set(NAME_A, VALUE_B);
            
            Assert.AreEqual(
                VALUE_B,
                runtime.Get(NAME_A)
            );
        }
    }
}