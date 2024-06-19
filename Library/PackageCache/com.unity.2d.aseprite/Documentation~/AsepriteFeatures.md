# Aseprite Features
This page highlights which Aseprite feature the Aseprite Importer supports/does not support.

## Supported features
**File formats**
- .ase & .aseprite
- Color modes (All modes are supported)
    - RGBA
    - Grayscale
    - Indexed

**Layer settings**
- Visible/Hidden layer
    - Hidden layers are by default not imported. This can be changed by checking “Include hidden layers” in the import settings.
- Layer blend modes
    - All blend modes are supported with Import Mode: Merge frames.
- Layer & Cel opacity
- Linked Cels
- Tags
    - Only Animation Direction: Forward is supported.
    - Values set in the repeating field only has two results on import:
        - ∞ will result in a looping Animation Clip. This value is the default for all Tags in Aseprite.
        - 1 -> N will result in a non looping Animation Clip.
- [Individual frame timings](https://www.aseprite.org/docs/frame-duration/)
- [Layer groups](https://www.aseprite.org/docs/layer-group/)
    - The importer respects the visibility mode selected for the group. If a group is hidden, underlying layers will not be imported by default.
    - Layer groups will be generated in the prefab hierarchy if the import mode is set to **Individual layers**.

## Unsupported features
- [Slices](https://www.aseprite.org/docs/slices/)
- [Tilemaps](https://www.aseprite.org/docs/tilemap/)
