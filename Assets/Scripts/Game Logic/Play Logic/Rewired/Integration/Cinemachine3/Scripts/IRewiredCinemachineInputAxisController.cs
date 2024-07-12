// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

namespace Rewired.Integration.Cinemachine3 {

    public interface IRewiredCinemachineInputAxisController {
        int GetPlayerId();
        bool ignoreTimeScale { get; }
    }
}
