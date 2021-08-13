using System;
using System.Reflection;
using System.IO;
using UnityEngine;

namespace BloodMageMod.Modules
{
    public class Assets
    {
        internal static AssetBundle mainAssetBundle;

        private static string[] assetNames = new string[0];

        internal static void Initialize()
        {
            LoadAssetBundle();
        }

        internal static void LoadAssetBundle()
        {
            if (mainAssetBundle == null) {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BloodMageMod.bloodmagebundle"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
            }

            assetNames = mainAssetBundle.GetAllAssetNames();
        }
    }
}
