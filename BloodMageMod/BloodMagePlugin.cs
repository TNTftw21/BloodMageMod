using System;
using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace BloodMageMod {
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(
        MODUID,
        MODNAME,
        MODVERSION)]
    [R2APISubmoduleDependency(
        nameof(LoadoutAPI),
    //    nameof(SurvivorAPI),
        nameof(LanguageAPI),
    //    nameof(PrefabAPI),
        nameof(BuffAPI)
    )]


    public class BloodMagePlugin : BaseUnityPlugin {
        public const string MODUID = "com.TNTftw21.BloodMageMod";
        public const string MODNAME = "BloodMageMod";
        public const string MODVERSION = "0.0.1";

        public const string developerPrefix = "TNTFTW21";

        public void Awake() {
            Log.Init(Logger);
            Modules.Assets.Initialize();
            Modules.Tokens.AddTokens();
            Modules.Buffs.RegisterBuffs();

            Hook();

            /*GameObject survivorPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/characterbodies/BanditBody"), "BloodMageBody");

            SurvivorDef myDef = new SurvivorDef
            {
                bodyPrefab = survivorPrefab,
                descriptionToken = Modules.Tokens.prefix + "DESC",
                displayPrefab = Resources.Load<GameObject>("Prefabs/Characters/BanditDisplay"),
                primaryColor = new Color(0.8039216f, 0.482352942f, 0.843137264f),
                unlockableDef = null
            };
            SurvivorAPI.AddSurvivor(myDef);

            Modules.Skills.CreateSkillFamilies(survivorPrefab);*/
        }

        private void Hook()
        {
            On.RoR2.SurvivorCatalog.Init += AddSkills;
            SkillStates.PetrifiedBloodState.Hooks();
            SkillStates.DoomDesireState.Hooks();
        }

        private void AddSkills(On.RoR2.SurvivorCatalog.orig_Init orig) {
            orig();

            GameObject survivorPrefab = SurvivorCatalog.GetSurvivorDef(SurvivorCatalog.FindSurvivorIndex("Mage")).bodyPrefab;
            Modules.Skills.SetupSkills(survivorPrefab);
        }
    }
}