using System;
using System.Collections.Generic;
using EntityStates;
using RoR2;
using RoR2.Skills;
using R2API;
using UnityEngine;

namespace BloodMageMod.Modules
{
    internal static class Skills
    {

        internal static List<SkillFamily> skillFamilies = new List<SkillFamily>();
        internal static List<SkillDef> skillDefs = new List<SkillDef>();

        private static SkillLocator skillLocator;

        internal static void CreateSkillFamilies(GameObject targetPrefab) {
            foreach (GenericSkill obj in targetPrefab.GetComponentsInChildren<GenericSkill>()) {
                BloodMagePlugin.DestroyImmediate(obj);
            }

            skillLocator = targetPrefab.GetComponent<SkillLocator>();

            skillLocator.primary = targetPrefab.AddComponent<GenericSkill>();
            SkillFamily primaryFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (primaryFamily as ScriptableObject).name = targetPrefab.name + "PrimaryFamily";
            primaryFamily.variants = new SkillFamily.Variant[0];
            skillLocator.primary._skillFamily = primaryFamily;

            skillLocator.secondary = targetPrefab.AddComponent<GenericSkill>();
            SkillFamily secondaryFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (secondaryFamily as ScriptableObject).name = targetPrefab.name + "SecondaryFamily";
            secondaryFamily.variants = new SkillFamily.Variant[0];
            skillLocator.secondary._skillFamily = secondaryFamily;

            skillLocator.utility = targetPrefab.AddComponent<GenericSkill>();
            SkillFamily utilityFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (utilityFamily as ScriptableObject).name = targetPrefab.name + "UtilityFamily";
            utilityFamily.variants = new SkillFamily.Variant[0];
            skillLocator.utility._skillFamily = utilityFamily;

            skillLocator.special = targetPrefab.AddComponent<GenericSkill>();
            SkillFamily specialFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (specialFamily as ScriptableObject).name = targetPrefab.name + "PrimaryFamily";
            specialFamily.variants = new SkillFamily.Variant[0];
            skillLocator.special._skillFamily = specialFamily;

            skillFamilies.Add(primaryFamily);
            skillFamilies.Add(secondaryFamily);
            skillFamilies.Add(utilityFamily);
            skillFamilies.Add(specialFamily);
        }

        public static void SetupSkills(GameObject targetPrefab) {

            skillLocator = targetPrefab.GetComponent<SkillLocator>();

            PassiveSetup();
            PrimarySetup(targetPrefab);
            SecondarySetup(targetPrefab);
            UtilitySetup(targetPrefab);
            SpecialSetup(targetPrefab);

            foreach (SkillDef def in skillDefs) {
                LoadoutAPI.AddSkillDef(def);
            }
        } 

        private static void PassiveSetup()
        {
            skillLocator.passiveSkill.enabled = true;
            skillLocator.passiveSkill.skillNameToken = Tokens.prefix + "PASSIVE_NAME";
            skillLocator.passiveSkill.skillDescriptionToken = Tokens.prefix + "PASSIVE_DESC";
            //skillLocator.passiveSkill.icon = Assets.iconP;    
        }

        private static void PrimarySetup(GameObject targetPrefab)
        {
            SkillDef bloodBoltDef = Modules.Skills.CreatePrimarySkillDef(new SerializableEntityStateType(typeof(SkillStates.BloodBoltState)), "Weapon", "BLOODBOLT",
                Assets.mainAssetBundle.LoadAsset<Sprite>("assets/sprites/skills/bloodbolt.png"), false, new string[] { "KEYWORD_SANGUINE" });
            AddPrimarySkill(targetPrefab, bloodBoltDef);
        }

        private static void SecondarySetup(GameObject targetPrefab)
        {
            SkillDef essenceSapDef = CreateSkillDef(new SkillDefInfo {
                skillName = "ESSENCESAP",
                skillNameToken = Modules.Tokens.prefix + "SECONDARY_ESSENCESAP_NAME",
                skillDescriptionToken = Modules.Tokens.prefix + "SECONDARY_ESSENCESAP_DESC",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("assets/sprites/skills/essencesap.png"),
                activationState = new SerializableEntityStateType(typeof(SkillStates.EssenceSapState)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 4,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_AGILE" }
            });

            AddSecondarySkill(targetPrefab, essenceSapDef);
        }

        private static void UtilitySetup(GameObject targetPrefab)
        {
            SkillDef petrifiedBloodDef = CreateSkillDef(new SkillDefInfo {
                skillName = "PETRIBLOOD",
                skillNameToken = Modules.Tokens.prefix + "UTILITY_PETRIBLOOD_NAME",
                skillDescriptionToken = Modules.Tokens.prefix + "UTILITY_PETRIBLOOD_DESC",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("assets/sprites/skills/petrifiedblood.png"),
                activationState = new SerializableEntityStateType(typeof(SkillStates.PetrifiedBloodState)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 15.0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_AGILE" }
            });

            AddUtilitySkill(targetPrefab, petrifiedBloodDef);
        }

        private static void SpecialSetup(GameObject targetPrefab)
        {
            SkillDef doomDesireDef = CreateSkillDef(new SkillDefInfo {
                skillName = "DOOMDESIRE",
                skillNameToken = Modules.Tokens.prefix + "SPECIAL_DOOMDESIRE_NAME",
                skillDescriptionToken = Modules.Tokens.prefix + "SPECIAL_DOOMDESIRE_DESC",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("assets/sprites/skills/doomdesire.png"),
                activationState = new SerializableEntityStateType(typeof(SkillStates.DoomDesireState)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 10.0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] {}
            });

            AddSpecialSkill(targetPrefab, doomDesireDef);
        }

        internal static void AddPrimarySkill(GameObject targetPrefab, SkillDef skillDef)
        {
            SkillLocator skillLocator = targetPrefab.GetComponent<SkillLocator>();

            SkillFamily skillFamily = skillLocator.primary.skillFamily;

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
            };
        }

        internal static void AddPrimarySkills(GameObject targetPrefab, SkillDef[] skillDefs) {
            foreach (SkillDef i in skillDefs)
            { 
                AddPrimarySkill(targetPrefab, i);
            }
        }

        internal static void AddSecondarySkill(GameObject targetPrefab, SkillDef skillDef)
        {
            SkillLocator skillLocator = targetPrefab.GetComponent<SkillLocator>();

            SkillFamily skillFamily = skillLocator.secondary.skillFamily;

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
            };
        }

        internal static void AddSecondarySkills(GameObject targetPrefab, params SkillDef[] skillDefs) {
            foreach (SkillDef i in skillDefs)
            { 
                AddSecondarySkill(targetPrefab, i);
            }
        }

        internal static void AddUtilitySkill(GameObject targetPrefab, SkillDef skillDef)
        {
            SkillLocator skillLocator = targetPrefab.GetComponent<SkillLocator>();

            SkillFamily skillFamily = skillLocator.utility.skillFamily;

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
            };
        }

        internal static void AddUtilitySkills(GameObject targetPrefab, params SkillDef[] skillDefs)
        {
            foreach (SkillDef i in skillDefs)
            {
                AddUtilitySkill(targetPrefab, i);
            }
        }

        internal static void AddSpecialSkill(GameObject targetPrefab, SkillDef skillDef)
        {
            SkillLocator skillLocator = targetPrefab.GetComponent<SkillLocator>();

            SkillFamily skillFamily = skillLocator.special.skillFamily;

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
            };
        }

        internal static void AddSpecialSkills(GameObject targetPrefab, params SkillDef[] skillDefs)
        {
            foreach (SkillDef i in skillDefs)
            {
                AddSpecialSkill(targetPrefab, i);
            }
        }

        internal static SkillDef CreatePrimarySkillDef(SerializableEntityStateType state, string stateMachine, string skillName, Sprite skillIcon, bool agile, string[] keywords) {
            SkillDef skillDef = ScriptableObject.CreateInstance<SkillDef>();

            skillDef.skillName = Tokens.prefix + "PRIMARY_" + skillName + "_NAME";
            skillDef.skillNameToken = Tokens.prefix + "PRIMARY_" + skillName + "_NAME";
            skillDef.skillDescriptionToken = Tokens.prefix + "PRIMARY_" + skillName + "_DESC";
            skillDef.icon = skillIcon;

            skillDef.activationState = state;
            skillDef.activationStateMachineName = stateMachine;
            skillDef.baseMaxStock = 1;
            skillDef.baseRechargeInterval = 0;
            skillDef.beginSkillCooldownOnSkillEnd = false;
            skillDef.canceledFromSprinting = false;
            skillDef.forceSprintDuringState = false;
            skillDef.fullRestockOnAssign = true;
            skillDef.interruptPriority = InterruptPriority.Any;
            skillDef.resetCooldownTimerOnUse = false;
            skillDef.isCombatSkill = true;
            skillDef.mustKeyPress = false;
            skillDef.cancelSprintingOnActivation = !agile;
            skillDef.rechargeStock = 1;
            skillDef.requiredStock = 0;
            skillDef.stockToConsume = 0;

            skillDef.keywordTokens = keywords;

            skillDefs.Add(skillDef);
            return skillDef;
        }

        internal static SkillDef CreateSkillDef(SkillDefInfo skillDefInfo)
        {
            SkillDef skillDef = ScriptableObject.CreateInstance<SkillDef>();

            skillDef.skillName = skillDefInfo.skillName;
            skillDef.skillNameToken = skillDefInfo.skillNameToken;
            skillDef.skillDescriptionToken = skillDefInfo.skillDescriptionToken;
            skillDef.icon = skillDefInfo.skillIcon;

            skillDef.activationState = skillDefInfo.activationState;
            skillDef.activationStateMachineName = skillDefInfo.activationStateMachineName;
            skillDef.baseMaxStock = skillDefInfo.baseMaxStock;
            skillDef.baseRechargeInterval = skillDefInfo.baseRechargeInterval;
            skillDef.beginSkillCooldownOnSkillEnd = skillDefInfo.beginSkillCooldownOnSkillEnd;
            skillDef.canceledFromSprinting = skillDefInfo.canceledFromSprinting;
            skillDef.forceSprintDuringState = skillDefInfo.forceSprintDuringState;
            skillDef.fullRestockOnAssign = skillDefInfo.fullRestockOnAssign;
            skillDef.interruptPriority = skillDefInfo.interruptPriority;
            skillDef.resetCooldownTimerOnUse = skillDefInfo.resetCooldownTimerOnUse;
            skillDef.isCombatSkill = skillDefInfo.isCombatSkill;
            skillDef.mustKeyPress = skillDefInfo.mustKeyPress;
            skillDef.cancelSprintingOnActivation = skillDefInfo.cancelSprintingOnActivation;
            skillDef.rechargeStock = skillDefInfo.rechargeStock;
            skillDef.requiredStock = skillDefInfo.requiredStock;
            skillDef.stockToConsume = skillDefInfo.stockToConsume;

            skillDef.keywordTokens = skillDefInfo.keywordTokens;

            skillDefs.Add(skillDef);

            return skillDef;
        }

    }

    internal struct SkillDefInfo
    {
        public string skillName;
        public string skillNameToken;
        public string skillDescriptionToken;
        public Sprite skillIcon;

        public SerializableEntityStateType activationState;
        public string activationStateMachineName;
        public int baseMaxStock;
        public float baseRechargeInterval;
        public bool beginSkillCooldownOnSkillEnd;
        public bool canceledFromSprinting;
        public bool forceSprintDuringState;
        public bool fullRestockOnAssign;
        public InterruptPriority interruptPriority;
        public bool resetCooldownTimerOnUse;
        public bool isCombatSkill;
        public bool mustKeyPress;
        public bool cancelSprintingOnActivation;
        public int rechargeStock;
        public int requiredStock;
        public int stockToConsume;

        public string[] keywordTokens;
    }
}
