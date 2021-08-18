using System;
using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace BloodMageMod.Modules
{

    public static class Buffs
    {
        public static BuffDef petrifiedBloodBuff;
        public static BuffDef petrifiedBloodDot;
        public static BuffDef doomDesireBuff;
        
        internal static List<BuffDef> buffDefs = new List<BuffDef>();

        public static void RegisterBuffs()
        {
            petrifiedBloodBuff = CreateBuffDef("Petrified Blood", Assets.mainAssetBundle.LoadAsset<Sprite>("assets/sprites/buffs/petrifiedblood.png"),
                new Color(1f, 1f, 1f), false, false);
            petrifiedBloodDot = CreateBuffDef("Petrified Blood DoT", Assets.mainAssetBundle.LoadAsset<Sprite>("assets/sprites/buffs/petrifiedblood.png"),
                new Color(1f, 0.8f, 0.8f), false, true);
            doomDesireBuff = CreateBuffDef("Doom Desire", Assets.mainAssetBundle.LoadAsset<Sprite>("assets/sprites/buffs/doomdesire.png"),
                new Color(1f, 1f, 1f), true, false);
        }

        public static BuffDef CreateBuffDef(string name, Sprite icon, Color color, bool isDebuff = false, bool canStack = false) {
            BuffDef def = ScriptableObject.CreateInstance<BuffDef>();
            def.name = name;
            def.iconSprite = icon;
            def.buffColor = color;
            def.isDebuff = isDebuff;
            def.canStack = canStack;
            def.eliteDef = null;
            buffDefs.Add(def);
            return def;
        }
    }
}
