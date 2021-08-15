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
        nameof(LanguageAPI),
        nameof(PrefabAPI)
    )]


    public class BloodMagePlugin : BaseUnityPlugin {
        public const string MODUID = "io.github.TNTftw21.BloodMageMod";
        public const string MODNAME = "BloodMageMod";
        public const string MODVERSION = "0.0.1";

        public const string developerPrefix = "TNTFTW21";

        public static BloodMagePlugin instance;

        public SurvivorDef bloodMageDef;
        public GameObject bloodMagePrefab;

        public void Awake() {
            instance = this;
            Log.Init(Logger);
            Modules.Assets.Initialize();
            Modules.Tokens.AddTokens();
            Modules.Buffs.RegisterBuffs();

            Hook();

            bloodMagePrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/characterbodies/MageBody"), "BloodMageBody");
            bloodMagePrefab.name = "Blood Mage";

            CharacterBody bodyComponent = bloodMagePrefab.GetComponent<CharacterBody>();

            bodyComponent.name = "BloodMageBody";
            bodyComponent.baseNameToken = Modules.Tokens.prefix + "NAME";
            bodyComponent.subtitleNameToken = Modules.Tokens.prefix + "SUB";
            bodyComponent.baseMaxHealth = 150;
            bodyComponent.levelMaxHealth = 33f;
            bodyComponent.baseRegen = 1.5f;
            bodyComponent.levelRegen = 0.3f;
            bodyComponent.baseMaxShield = 0;
            bodyComponent.levelMaxShield = 0;
            bodyComponent.baseDamage = 7f;
            bodyComponent.levelDamage = 1.4f;

            bloodMageDef = ScriptableObject.CreateInstance<SurvivorDef>();
            bloodMageDef.bodyPrefab = bloodMagePrefab;
            bloodMageDef.displayPrefab = Resources.Load<GameObject>("Prefabs/Characters/MageDisplay");
            bloodMageDef.displayNameToken = bodyComponent.baseNameToken;
            bloodMageDef.descriptionToken = Modules.Tokens.prefix + "DESC";
            bloodMageDef.unlockableDef = null;
            bloodMageDef.primaryColor = new Color(0.8039216f, 0.482352942f, 0.843137264f);
            /* new SurvivorDef
            {
                bodyPrefab = survivorPrefab,
                descriptionToken = Modules.Tokens.prefix + "DESC",
                displayPrefab = Resources.Load<GameObject>("Prefabs/Characters/BanditDisplay"),
                primaryColor = new Color(0.8039216f, 0.482352942f, 0.843137264f),
                unlockableDef = null
            };*/

            Modules.Skills.CreateSkillFamilies(bloodMagePrefab);
            Modules.Skills.SetupSkills(bloodMagePrefab);

            new Modules.ContentPacks().Initialize();
        }

        private void Hook()
        {
            //On.RoR2.SurvivorCatalog.Init += AddSkills;
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