using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloodMageMod.Modules.Survivors
{
    internal abstract class SurvivorBase
    {
        internal static SurvivorBase instance;

        internal abstract string bodyName { get; set; }
        
        internal abstract GameObject bodyPrefab { get; set; }
        internal abstract GameObject displayPrefab { get; set; }

        internal abstract float sortPosition { get; set; }

        internal string fullBodyName => bodyName + "Body";

        internal abstract UnlockableDef characterUnlockableDef { get; set; }

        internal abstract BodyInfo bodyInfo { get; set; }

        internal abstract int mainRendererIndex { get; set; }

    }
}
