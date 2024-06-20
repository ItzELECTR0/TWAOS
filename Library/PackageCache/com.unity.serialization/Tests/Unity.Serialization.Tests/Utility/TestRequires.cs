using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Unity.Serialization.Tests
{
    // ReSharper disable once InconsistentNaming
    class TestRequires_ENABLE_UNITY_COLLECTION_CHECKS : Attribute, ITestAction
    {
        public ActionTargets Targets { get; }
        
        public TestRequires_ENABLE_UNITY_COLLECTION_CHECKS()
        {
        }

        public void BeforeTest(ITest test)
        {
#if !ENABLE_UNITY_COLLECTIONS_CHECKS
            Assert.Ignore($"Test requires Define=[ENABLE_UNITY_COLLECTIONS_CHECKS]");
#endif
        }

        public void AfterTest(ITest test)
        {

        }
    }
}