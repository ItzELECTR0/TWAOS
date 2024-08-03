// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649 // disable warnings about unused variables

namespace Rewired.Demos.CustomPlatform {

    /// <summary>
    /// Example implementation of a Hardware Joystick Map Platform Map for a custom platform.
    /// This allows you to create controller definitions to match to connected controllers.
    /// This is required for controllers to be recognized and work with the Controller Map system.
    /// This implementation uses a Unity SerializedObject for ease of managing definitions.
    /// This could alternately be implemented entirely in code without using a SerializedObject if you like.
    /// 
    /// This example shows the most complex setup which includes custom matching criteria with user-defined variables
    /// and map variants. These are both optional features and should only be implemented if you need them.
    /// 
    /// If you have no special needs and matching on device string name is good enough, you can skip creating these
    /// classes entirely and simply use the built-in HardwareJoystickMapCustomPlatformMapSimpleSO class.
    /// This requires no code. Simply create an instance from the right-click menu:
    /// Create -> Rewired -> Custom Platform -> Simple Joystick Platform Map
    /// </summary>
    public sealed class MyPlatformHardwareJoystickMapPlatformMap : Rewired.Platforms.Custom.HardwareJoystickMapCustomPlatformMapSO {

        /// <summary>
        /// The platform map.
        /// This contains all the serialized data.
        /// IMPORTANT: This field MUST exist, MUST be named "platformMap" and MUST be serializable.
        /// (If you make it private, add the [UnityEngine.SerializeField] attribute.)
        /// </summary>
        public PlatformMap platformMap;

        /// <summary>
        /// Returns the Platform Map.
        /// </summary>
        /// <returns>Platform Map</returns>
        public override Rewired.Data.Mapping.HardwareJoystickMap.Platform GetPlatformMap() {
            return platformMap;
        }

        #region Classes

        /// <summary>
        /// (Optional) Platform map interim base class.
        /// This interim class is only required if using variants.
        /// DO NOT add variants to this class.
        /// Variants must be serialized in the child class below to avoid infinite recursion.
        /// </summary>
        [System.Serializable]
        public class PlatformMapBase : Rewired.Platforms.Custom.HardwareJoystickMapCustomPlatformMap<MatchingCriteria> {

            /// <summary>
            /// Creates a new instance of this object.
            /// Every subclass class must implement this.
            /// </summary>
            /// <returns>A new instance of this object.</returns>
            protected override object CreateInstance() {
                return new PlatformMapBase(); // create a new instance of this class
            }
        }

        /// <summary>
        /// Platform map implementation.
        /// User-defined MatchingCriteria is used as generic type argument in HardwareJoystickMapCustomPlatformMap.
        /// </summary>
        [System.Serializable]
        public sealed class PlatformMap : PlatformMapBase { // inherit from PlatformMapBase created above if supporting variants, otherwise inherit from Rewired.Platforms.Custom.HardwareJoystickMapCustomPlatformMap<MatchingCriteria>.

            /// <summary>
            /// (Optional) Platform map variants.
            /// Each variant defines an independent set of matching criteria and a list of axes and buttons.
            /// This allows matching multiple different controllers or variants of controller hardware to the same Rewired Joystick type.
            /// Example: You may want to match other controller types to the Xbox One controller definition.
            /// If you want to support variants, this field must be named "variants" and be serialized.
            /// </summary>
            public PlatformMapBase[] variants;

            /// <summary>
            /// (Optional) Gets the list of variants.
            /// This function must be implemented if using variants.
            /// </summary>
            /// <returns></returns>
            public override System.Collections.Generic.IList<Rewired.Data.Mapping.HardwareJoystickMap.Platform> GetVariants() { return variants; }

            /// <summary>
            /// Creates a new instance of this object.
            /// Every subclass class must implement this.
            /// </summary>
            /// <returns>A new instance of this object.</returns>
            protected override object CreateInstance() {
                return new PlatformMap(); // create a new instance of this class
            }
        }

        /// <summary>
        /// Matching criteria implementation.
        /// Determines if a controller matches specific criteria.
        /// This is only necessary if you need custom variables for matching.
        /// If you don't, just pass Rewired.Platforms.Custom.HardwareJoystickMapCustomPlatformMapSimple.MatchingCriteria
        /// as the generic type argument in Rewired.Platforms.Custom.HardwareJoystickMapCustomPlatformMap<TMatchingCriteria>
        /// instead of defining your own MatchingCriteria class.
        /// </summary>
        [System.Serializable]
        public sealed class MatchingCriteria : Rewired.Platforms.Custom.HardwareJoystickMapCustomPlatformMap.MatchingCriteria {

            // User-defined serialized values for matching based on custom criteria
            public uint vendorId;
            public uint productId;

            /// <summary>
            /// Determines if the controller matches to this platform map based on user-defined identifying information.
            /// Override this function to match on custom matching criteria.
            /// </summary>
            /// <param name="customIdentifier">User-defined identifying information supplied to the Controller on creation.</param>
            /// <returns>True if identifying information matches, false if not.</returns>
            public override bool Matches(object customIdentifier) {
                if (!(customIdentifier is MyPlatformControllerIdentifier)) return false;
                MyPlatformControllerIdentifier identifier = (MyPlatformControllerIdentifier)customIdentifier;
                return identifier.productId == productId &&
                    identifier.vendorId == vendorId;
            }

            /// <summary>
            /// Creates a new instance of this object.
            /// Every subclass class must implement this.
            /// </summary>
            /// <returns>A new instance of this object.</returns>
            protected override object CreateInstance() {
                return new MatchingCriteria(); // create a new instance of this class
            }

            /// <summary>
            /// Called when object is deep cloned.
            /// Override this function to clone local data the class, if any.
            /// </summary>
            /// <param name="destination">The object which will receive the cloned data.</param>
            protected override void DeepClone(object destination) {
                base.DeepClone(destination);

                MatchingCriteria tDest = (MatchingCriteria)destination; // cast to this type
                
                // Copy all user-defined values
                tDest.vendorId = vendorId;
                tDest.productId = productId;

                // If copying arrays or other reference object types, they need to be explicitly
                // copied so the original object reference is not copied to the clone.
            }
        }

        #endregion
    }
}