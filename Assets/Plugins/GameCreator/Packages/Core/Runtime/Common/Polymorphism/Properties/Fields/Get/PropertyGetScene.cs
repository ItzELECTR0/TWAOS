using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertyGetScene : TPropertyGet<PropertyTypeGetScene, int>
    {
        public PropertyGetScene() : base(new GetSceneAsset())
        { }

        public PropertyGetScene(PropertyTypeGetScene defaultType) : base(defaultType)
        { }
    }
}