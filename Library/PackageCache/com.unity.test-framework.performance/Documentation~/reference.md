# Reference

This section contains a reference for attributes and classes the Performance Testing Package supports.

## Test Attributes
**[Performance]** - Use this with  `Test` and `UnityTest` attributes. It will initialize necessary test setup for performance tests.

**[Test]** -  Non-yielding test. This type of test starts and ends within the same frame.

**[UnityTest]** - Yielding test. This is a good choice if you want to sample measurements across multiple frames. For more on the difference between `[Test]` and `[UnityTest]`, see the Unity Test Framework [documentation](https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/reference-attribute-unitytest.html).

**[Version(string version)]** - Performance tests should be versioned with every change. If not specified, the default used is 1. This is essential when comparing results as results will vary any time the test changes.


## SampleGroup

**class SampleGroup** - represents a group of samples with the same purpose that share a name, sample unit and whether an increase is better. 

Optional parameters
- **Name** : Name of the measurement. If unspecified, "Time" is used as the default name.
- **Unit** : Unit of the measurement to report samples in. Possible values are:
Nanosecond, Microsecond, Millisecond, Second, Byte, Kilobyte, Megabyte, Gigabyte
- **IncreaseIsBetter** : If true, an increase in the measurement value is considered a performance improvement (progression). If false, an increase is treated as a performance regression. False by default. 
