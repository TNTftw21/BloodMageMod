using System;
using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace BloodMageMod.Modules
{

    public static class Buffs
    {
        public static CustomBuff petrifiedBloodBuff;
        public static CustomBuff doomDesireBuff;
        
        internal static List<CustomBuff> buffDefs = new List<CustomBuff>();

        public static void RegisterBuffs()
        {
            BuffAPI.Add(petrifiedBloodBuff = new CustomBuff("Petrified Blood", Assets.mainAssetBundle.LoadAsset<Sprite>("assets/sprites/buffs/petrifiedblood.png"),
                new Color(0.8039216f, 0.482352942f, 0.843137264f), false, false));
            BuffAPI.Add(doomDesireBuff = new CustomBuff("Doom Desire", Assets.mainAssetBundle.LoadAsset<Sprite>("assets/sprites/buffs/doomdesire.png"),
                new Color(0.560784214f, 0.247058824f, 0.729411765f), true, false));
        }
    }
}
