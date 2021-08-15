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
        public static BuffDef doomDesireBuff;
        
        internal static List<BuffDef> buffDefs = new List<BuffDef>();

        public static void RegisterBuffs()
        {
            petrifiedBloodBuff = CreateBuffDef("Petrified Blood", Assets.mainAssetBundle.LoadAsset<Sprite>("assets/sprites/buffs/petrifiedblood.png"),
                new Color(0.8039216f, 0.482352942f, 0.843137264f), false, false);
            doomDesireBuff = CreateBuffDef("Doom Desire", Assets.mainAssetBundle.LoadAsset<Sprite>("assets/sprites/buffs/doomdesire.png"),
                new Color(0.560784214f, 0.247058824f, 0.729411765f), true, false);
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
