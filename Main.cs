using BepInEx;
using RoR2;
using R2API;
using R2API.Utils;
using R2API.Networking;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using RoR2.ContentManagement;
using System.Collections;
using System.Security;
using System.Security.Permissions;
using MysticsRisky2Utils;
using System.Text;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
namespace EliteVariety
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency(MysticsRisky2UtilsPlugin.PluginGUID)]
    [BepInDependency(AspectAbilities.AspectAbilitiesPlugin.PluginGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(NetworkingAPI), nameof(PrefabAPI), nameof(SoundAPI))]
    public class EliteVarietyPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "com.themysticsword.elitevariety";
        public const string PluginName = "EliteVariety";
        public const string PluginVersion = "1.0.0";

        internal static BepInEx.Logging.ManualLogSource logger;

        public void Awake()
        {
            logger = Logger;
            Main.Init();
        }
    }

    public static class Main
    {
        public const string TokenPrefix = EliteVarietyPlugin.PluginName + "_";
        internal const BindingFlags bindingFlagAll = (BindingFlags)(-1);

        public static AssetBundle AssetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("EliteVariety.elitevarietyunityassetbundle"));

        public static BepInEx.Logging.ManualLogSource logger;

        public static Assembly executingAssembly;

        internal static bool aspectAbilitiesEnabled = false;

        public static void Init()
        {
            logger = EliteVarietyPlugin.logger;
            executingAssembly = Assembly.GetExecutingAssembly();

            aspectAbilitiesEnabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(AspectAbilities.AspectAbilitiesPlugin.PluginGUID);

            using (var soundBankStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EliteVariety.EliteVarietyWwiseSoundbank.bnk"))
            {
                var bytes = new byte[soundBankStream.Length];
                soundBankStream.Read(bytes, 0, bytes.Length);
                SoundAPI.SoundBanks.Add(bytes);
            }

            MysticsRisky2Utils.ContentManagement.ContentLoadHelper.PluginAwakeLoad<Buffs.BaseBuff>(executingAssembly);
            MysticsRisky2Utils.ContentManagement.ContentLoadHelper.PluginAwakeLoad<Equipment.BaseEquipment>(executingAssembly);
            MysticsRisky2Utils.ContentManagement.ContentLoadHelper.PluginAwakeLoad<Elites.BaseElite>(executingAssembly);

            ContentManager.collectContentPackProviders += (addContentPackProvider) =>
            {
                addContentPackProvider(new EliteVarietyContent());
            };

            On.RoR2.CharacterBody.Awake += CharacterBody_Awake;

            ConCommandHelper.Load(typeof(Equipment.BaseEliteAffix).GetMethod("CCAdjustElitePickupMaterial", bindingFlagAll));
        }

        private static void CharacterBody_Awake(On.RoR2.CharacterBody.orig_Awake orig, CharacterBody self)
        {
            orig(self);
            self.gameObject.AddComponent<EliteVarietyBodyFields>();
        }

        internal static StringBuilder sharedStringBuilder = new StringBuilder();
    }

    public class EliteVarietyBodyFields : MonoBehaviour
    {
        public bool eliteRampReplaced = false;
    }

    public class EliteVarietyContent : IContentPackProvider
    {
        public string identifier
        {
            get
            {
                return EliteVarietyPlugin.PluginName;
            }
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            contentPack.identifier = identifier;
            MysticsRisky2Utils.ContentManagement.ContentLoadHelper contentLoadHelper = new MysticsRisky2Utils.ContentManagement.ContentLoadHelper();
            System.Action[] loadDispatchers = new System.Action[]
            {
                () =>
                {
                    contentLoadHelper.DispatchLoad<EquipmentDef>(Main.executingAssembly, typeof(EliteVariety.Equipment.BaseEquipment), x => contentPack.equipmentDefs.Add(x));
                },
                () =>
                {
                    contentLoadHelper.DispatchLoad<BuffDef>(Main.executingAssembly, typeof(EliteVariety.Buffs.BaseBuff), x => contentPack.buffDefs.Add(x));
                },
                () =>
                {
                    contentLoadHelper.DispatchLoad<EliteDef>(Main.executingAssembly, typeof(EliteVariety.Elites.BaseElite), x => contentPack.eliteDefs.Add(x));
                }
            };
            int num;
            for (int i = 0; i < loadDispatchers.Length; i = num)
            {
                loadDispatchers[i]();
                args.ReportProgress(Util.Remap((float)(i + 1), 0f, (float)loadDispatchers.Length, 0f, 0.05f));
                yield return null;
                num = i + 1;
            }
            while (contentLoadHelper.coroutine.MoveNext())
            {
                args.ReportProgress(Util.Remap(contentLoadHelper.progress.value, 0f, 1f, 0.05f, 0.95f));
                yield return contentLoadHelper.coroutine.Current;
            }
            loadDispatchers = new System.Action[]
            {
                () =>
                {
                    ContentLoadHelper.PopulateTypeFields<EquipmentDef>(typeof(Equipment), contentPack.equipmentDefs);
                    MysticsRisky2Utils.ContentManagement.ContentLoadHelper.AddPrefixToAssets<EquipmentDef>(contentPack.equipmentDefs, Main.TokenPrefix);
                },
                () =>
                {
                    ContentLoadHelper.PopulateTypeFields<BuffDef>(typeof(Buffs), contentPack.buffDefs);
                    MysticsRisky2Utils.ContentManagement.ContentLoadHelper.AddPrefixToAssets<BuffDef>(contentPack.buffDefs, Main.TokenPrefix);
                },
                () =>
                {
                    ContentLoadHelper.PopulateTypeFields<EliteDef>(typeof(Elites), contentPack.eliteDefs);
                    MysticsRisky2Utils.ContentManagement.ContentLoadHelper.AddPrefixToAssets<EliteDef>(contentPack.eliteDefs, Main.TokenPrefix);
                },
                () =>
                {
                    contentPack.bodyPrefabs.Add(Resources.bodyPrefabs.ToArray());
                    contentPack.masterPrefabs.Add(Resources.masterPrefabs.ToArray());
                    contentPack.projectilePrefabs.Add(Resources.projectilePrefabs.ToArray());
                    contentPack.effectDefs.Add(Resources.effectPrefabs.ConvertAll(x => new EffectDef(x)).ToArray());
                    contentPack.networkSoundEventDefs.Add(Resources.networkSoundEventDefs.ToArray());
                    contentPack.unlockableDefs.Add(Resources.unlockableDefs.ToArray());
                    contentPack.entityStateTypes.Add(Resources.entityStateTypes.ToArray());
                    contentPack.skillDefs.Add(Resources.skillDefs.ToArray());
                }
            };
            for (int i = 0; i < loadDispatchers.Length; i = num)
            {
                loadDispatchers[i]();
                args.ReportProgress(Util.Remap((float)(i + 1), 0f, (float)loadDispatchers.Length, 0.95f, 0.99f));
                yield return null;
                num = i + 1;
            }
            MysticsRisky2Utils.ContentManagement.ContentLoadHelper.InvokeAfterContentPackLoaded<EliteVariety.Buffs.BaseBuff>(Main.executingAssembly);
            MysticsRisky2Utils.ContentManagement.ContentLoadHelper.InvokeAfterContentPackLoaded<EliteVariety.Equipment.BaseEquipment>(Main.executingAssembly);
            MysticsRisky2Utils.ContentManagement.ContentLoadHelper.InvokeAfterContentPackLoaded<EliteVariety.Elites.BaseElite>(Main.executingAssembly);
            loadDispatchers = null;
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        private ContentPack contentPack = new ContentPack();

        public static class Resources
        {
            public static List<GameObject> bodyPrefabs = new List<GameObject>();
            public static List<GameObject> masterPrefabs = new List<GameObject>();
            public static List<GameObject> projectilePrefabs = new List<GameObject>();
            public static List<GameObject> effectPrefabs = new List<GameObject>();
            public static List<NetworkSoundEventDef> networkSoundEventDefs = new List<NetworkSoundEventDef>();
            public static List<UnlockableDef> unlockableDefs = new List<UnlockableDef>();
            public static List<System.Type> entityStateTypes = new List<System.Type>();
            public static List<RoR2.Skills.SkillDef> skillDefs = new List<RoR2.Skills.SkillDef>();
        }

        public static class Equipment
        {
            public static EquipmentDef AffixArmored;
            public static EquipmentDef AffixBuffing;
            public static EquipmentDef AffixPillaging;
            public static EquipmentDef AffixSandstorm;
        }

        public static class Buffs
        {
            public static BuffDef AffixArmored;
            public static BuffDef AffixBuffing;
            public static BuffDef AffixPillaging;
            public static BuffDef AffixSandstorm;
            public static BuffDef ArmoredHeavyStun;
            public static BuffDef ArmoredSelfBuff;
            public static BuffDef SandstormBlind;
        }

        public static class Elites
        {
            public static EliteDef Armored;
            public static EliteDef Buffing;
            public static EliteDef Pillaging;
            public static EliteDef Sandstorm;
        }
    }
}
