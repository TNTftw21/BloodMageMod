using RoR2.ContentManagement;
using RoR2;

namespace BloodMageMod.Modules
{
    public class ContentPacks : IContentPackProvider
    {
        internal ContentPack contentPack = new ContentPack();

        public string identifier => BloodMagePlugin.MODUID;

        public void Initialize()
        {
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }

        public System.Collections.IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            this.contentPack.identifier = identifier;
            contentPack.bodyPrefabs.Add(new UnityEngine.GameObject[] { BloodMagePlugin.instance.bloodMagePrefab });
            contentPack.buffDefs.Add(Buffs.buffDefs.ToArray());
            contentPack.skillDefs.Add(Skills.skillDefs.ToArray());
            contentPack.skillFamilies.Add(Skills.skillFamilies.ToArray());
            contentPack.survivorDefs.Add(new SurvivorDef[] { BloodMagePlugin.instance.bloodMageDef });

            args.ReportProgress(1f);
            yield break;
        }

        public System.Collections.IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(this.contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public System.Collections.IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}
