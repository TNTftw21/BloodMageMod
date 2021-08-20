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
        internal abstract CustomRendererInfo[] CustomRendererInfos { get; set; }
        
        internal abstract Type characterMainState { get; set; }

        internal abstract ItemDisplayRuleSet itemDisplayRuleSet { get; set; }
        internal abstract List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules { get; set; }

        internal void Initialize()
        {
            instance = this;
            InitializeCharacter();
        }

        internal virtual void InitializeCharacter()
        {
            InitializeUnlockables();

            bodyPrefab = Modules.Prefabs.CreatePrefab(bodyName + "Body", "mdl" + bodyName, bodyInfo);
        }

        internal virtual void InitializeUnlockables()
        {

        }

        internal virtual void InitializeSkills()
        {

        }

        internal virtual void InitializeHitboxes()
        {

        }

        internal virtual void InitializeSkins()
        {

        }

        internal virtual void InitializeDoppleganger()
        {

        }

        internal virtual void InitializeItemDisplays()
        {

        }

        internal virtual void SetItemDisplays()
        {

        }
    }
}
