using RoR2;
using System.Collections.Generic;
using UnityEngine;
using MysticsRisky2Utils;
using RoR2.Audio;

namespace EliteVariety.Equipment
{
    public class AffixBuffing : BaseEliteAffix
    {
        public static NetworkSoundEventDef useSound;

        public override void PreLoad()
        {
            base.PreLoad();
            equipmentDef.name = "AffixBuffing";
            equipmentDef.cooldown = 60f;
        }

        public override void OnLoad()
        {
            base.OnLoad();
            SetAssets("Buffing");
            AdjustElitePickupMaterial(new Color32(40, 40, 40, 255), 0.5f, Main.AssetBundle.LoadAsset<Texture>("Assets/EliteVariety/Elites/Buffing/texRampEliteBuffing.png"));
            Material clothMaterial = followerModel.transform.Find("mdlEliteBuffingBanner/Cloth").gameObject.GetComponent<Renderer>().sharedMaterial;
            clothMaterial.shader = Resources.Load<Shader>("shaders/deferred/hgwavycloth");
            clothMaterial.EnableKeyword("CUTOUT");
            clothMaterial.EnableKeyword("DITHER");
            clothMaterial.EnableKeyword("_ALPHATEST_ON");
            clothMaterial.SetFloat("_Cutoff", 0.5f);
            MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsClothReenabler clothReenabler = followerModel.AddComponent<MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsClothReenabler>();
            clothReenabler.clothToReenable = new Cloth[]
            {
                followerModel.transform.Find("mdlEliteBuffingBanner/Cloth").gameObject.GetComponent<Cloth>()
            };
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Chest", new Vector3(0.00102F, 0.17669F, -0.12973F), new Vector3(0.07531F, 90F, 359.3813F), new Vector3(0.36256F, 0.36256F, 0.36256F));
                AddDisplayRule("HuntressBody", "Chest", new Vector3(0.00002F, -0.02979F, -0.04754F), new Vector3(358.6577F, 86.58627F, 338.5598F), new Vector3(0.38563F, 0.38563F, 0.38563F));
                AddDisplayRule("Bandit2Body", "Chest", new Vector3(0F, 0.03892F, -0.14487F), new Vector3(0F, 90F, 344.5243F), new Vector3(0.27982F, 0.27982F, 0.27982F));
                AddDisplayRule("ToolbotBody", "Chest", new Vector3(0.00001F, 0.01734F, -1.24409F), new Vector3(0F, 90F, 344.6589F), new Vector3(1.95547F, 1.95547F, 1.95547F));
                AddDisplayRule("EngiBody", "Chest", new Vector3(0F, 0.11143F, -0.24884F), new Vector3(0F, 90F, 342.4611F), new Vector3(0.28191F, 0.28191F, 0.28191F));
                AddDisplayRule("EngiTurretBody", "Neck", new Vector3(0F, 0.48752F, -0.03041F), new Vector3(0F, 270F, 0F), new Vector3(1.05434F, 1.05655F, 1.05655F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0F, 0.37199F, -0.82251F), new Vector3(0F, 90F, 340F), new Vector3(1.05434F, 1.05655F, 1.05655F));
                AddDisplayRule("MageBody", "Chest", new Vector3(0F, -0.14577F, -0.1979F), new Vector3(0F, 90F, 0F), new Vector3(0.29608F, 0.29608F, 0.29608F));
                AddDisplayRule("MercBody", "Chest", new Vector3(0F, -0.13038F, -0.05179F), new Vector3(0F, 90F, 350.5615F), new Vector3(0.2856F, 0.2856F, 0.2856F));
                AddDisplayRule("TreebotBody", "PlatformBase", new Vector3(0F, 1.24573F, -0.64632F), new Vector3(-0.00008F, 89.99994F, 338.9332F), new Vector3(0.61149F, 0.61149F, 0.61149F));
                AddDisplayRule("LoaderBody", "MechBase", new Vector3(-0.0029F, 0.09242F, -0.04496F), new Vector3(0F, 90F, 0F), new Vector3(0.28298F, 0.28298F, 0.28298F));
                AddDisplayRule("CrocoBody", "Chest", new Vector3(0.67265F, -1.71814F, 3.68666F), new Vector3(321.8149F, 111.5687F, 359.7997F), new Vector3(2.11805F, 2.11805F, 2.11805F));
                AddDisplayRule("CaptainBody", "Chest", new Vector3(0F, 0.1173F, -0.00825F), new Vector3(0F, 90F, 335.0094F), new Vector3(0.24446F, 0.24446F, 0.24446F));
                
                AddDisplayRule("WispBody", "Head", new Vector3(0F, 0.00348F, -0.6174F), new Vector3(0F, 90F, 90F), new Vector3(1.18934F, 1.18934F, 1.18934F));
                AddDisplayRule("JellyfishBody", "Hull2", new Vector3(0.00906F, -0.0899F, 0.01635F), new Vector3(0F, 273.5969F, 0F), new Vector3(0.76511F, 0.76511F, 0.76511F));
                AddDisplayRule("BeetleBody", "Chest", new Vector3(0F, 0.13316F, -0.65895F), new Vector3(0F, 270F, 37.27843F), new Vector3(0.80709F, 0.80709F, 0.80709F));
                AddDisplayRule("LemurianBody", "Chest", new Vector3(0F, -2.10492F, 0.69056F), new Vector3(0F, 90F, 23.17685F), new Vector3(4.47252F, 4.47252F, 4.47252F));
                AddDisplayRule("HermitCrabBody", "Base", new Vector3(0.00001F, 1.1429F, -0.00001F), new Vector3(0F, 270F, 0F), new Vector3(0.8978F, 0.8978F, 0.8978F));
                AddDisplayRule("ImpBody", "Neck", new Vector3(0F, -0.38947F, 0.10343F), new Vector3(0F, 90F, 0.68676F), new Vector3(0.35844F, 0.35844F, 0.35844F));
                AddDisplayRule("VultureBody", "Chest", new Vector3(-0.00005F, -0.79747F, 1.95463F), new Vector3(6.13429F, 269.3485F, 83.92575F), new Vector3(3.8124F, 3.8124F, 3.8124F));
                AddDisplayRule("RoboBallMiniBody", "ROOT", new Vector3(0.81676F, 0.00111F, -0.02604F), new Vector3(0F, 0F, 347.2889F), new Vector3(0.97745F, 0.97745F, 0.97745F));
                AddDisplayRule("RoboBallMiniBody", "ROOT", new Vector3(-0.81676F, 0.00111F, -0.02604F), new Vector3(0F, 0F, -347.2889F), new Vector3(0.97745F, 0.97745F, 0.97745F));
                AddDisplayRule("MiniMushroomBody", "Head", new Vector3(1.00691F, 0.36443F, 0F), new Vector3(-0.00001F, 180F, 264.1599F), new Vector3(1.10492F, 1.08723F, 1.10492F));
                AddDisplayRule("BellBody", "Chain", new Vector3(0F, 1.12031F, 0F), new Vector3(0F, 335.945F, 180F), new Vector3(1.593F, 1.593F, 1.593F));
                AddDisplayRule("BeetleGuardBody", "Chest", new Vector3(-0.10637F, 0.34776F, -1.87907F), new Vector3(355.6954F, 274.0369F, 21.01935F), new Vector3(2.50349F, 2.50349F, 2.50349F));
                AddDisplayRule("BisonBody", "Chest", new Vector3(0F, -0.23085F, 0.17051F), new Vector3(0F, 90F, 58.0866F), new Vector3(0.74749F, 0.74749F, 0.74749F));
                AddDisplayRule("GolemBody", "Chest", new Vector3(0F, 0.11769F, -0.18732F), new Vector3(0F, 90F, 352.168F), new Vector3(1.19599F, 1.19599F, 1.19599F));
                AddDisplayRule("ParentBody", "Chest", new Vector3(-49.4163F, -39.60241F, -0.57086F), new Vector3(0.00012F, 0.00004F, 353.1383F), new Vector3(133.9619F, 133.9619F, 133.9619F));
                AddDisplayRule("ClayBruiserBody", "Chest", new Vector3(0.10359F, 0.57035F, -0.36894F), new Vector3(18.0352F, 223.9352F, 342.147F), new Vector3(0.52213F, 0.52213F, 0.52213F));
                AddDisplayRule("GreaterWispBody", "MaskBase", new Vector3(0F, -0.53085F, 0.35565F), new Vector3(0F, 270F, 351.6115F), new Vector3(0.97508F, 0.97508F, 0.97508F));
                AddDisplayRule("LemurianBruiserBody", "Chest", new Vector3(0F, -1.65961F, 0.42276F), new Vector3(0F, 90F, 41.72485F), new Vector3(3.70022F, 3.70022F, 3.70022F));
                AddDisplayRule("NullifierBody", "Muzzle", new Vector3(1.61304F, -0.89677F, -0.60892F), new Vector3(350.7634F, 280.3142F, 306.2213F), new Vector3(1.7403F, 1.7403F, 1.7403F));
                
                AddDisplayRule("BeetleQueen2Body", "Head", new Vector3(0F, 0.66384F, 2.77533F), new Vector3(0F, 90F, 324.9953F), new Vector3(1.8373F, 1.8373F, 1.8373F));
                AddDisplayRule("ClayBossBody", "PotBase", new Vector3(0F, -0.21454F, -1.78393F), new Vector3(0F, 270F, 354.0653F), new Vector3(1.20857F, 1.20857F, 1.20857F));
                AddDisplayRule("TitanBody", "Chest", new Vector3(0.89643F, 3.32113F, -2.67202F), new Vector3(0F, 270F, 0F), new Vector3(3.64626F, 3.64626F, 3.64626F));
                AddDisplayRule("TitanGoldBody", "Chest", new Vector3(0.89643F, 3.32113F, -2.67202F), new Vector3(0F, 270F, 0F), new Vector3(3.64626F, 3.64626F, 3.64626F));
                AddDisplayRule("VagrantBody", "Hull", new Vector3(0F, 0.98831F, -0.73006F), new Vector3(0F, 270F, 337.7484F), new Vector3(0.72348F, 0.72348F, 0.72348F));
                string[] worms = new string[]
                {
                    "MagmaWormBody",
                    "ElectricWormBody"
                };
                foreach (string worm in worms)
                {
                    AddDisplayRule(worm, "Head", new Vector3(-0.04405F, 1.49652F, 0.59592F), new Vector3(6.87863F, 90F, 48.29213F), new Vector3(1.05666F, 1.05666F, 1.05666F));
                    for (var i = 1; i <= 16; i++)
                    {
                        Vector3 scale = Vector3.one * 0.5777F * Mathf.Pow(0.934782609f, i - 1);
                        if ((i % 4) == 0) AddDisplayRule(worm, "Neck" + i.ToString(), new Vector3(0F, 0.70959F + 0.03293F * (i - 1), 0.72532F - 0.02657F * (i - 1)), new Vector3(0F, 90F, 42.11422F), scale);
                        if ((i % 4) == 1) AddDisplayRule(worm, "Neck" + i.ToString(), new Vector3(1.05941F - 0.02657F * (i - 1), 0.69999F + 0.03293F * (i - 1), -0.2738F), new Vector3(0F, 0F, 317.7116F), scale);
                        if ((i % 4) == 2) AddDisplayRule(worm, "Neck" + i.ToString(), new Vector3(0F, 0.70959F + 0.03293F * (i - 1), -1.27241F + 0.02657F * (i - 1)), new Vector3(0F, 270F, 40.70816F), scale);
                        if ((i % 4) == 3) AddDisplayRule(worm, "Neck" + i.ToString(), new Vector3(-1.03284F + 0.02657F * (i - 1), 0.69999F + 0.03293F * (i - 1), -0.2738F), new Vector3(0F, 180F, 317.0171F), scale);
                    }
                }
                AddDisplayRule("RoboBallBossBody", "Shell", new Vector3(0.48786F, 0.59529F, -0.18982F), new Vector3(321.6017F, 270F, 352.831F), new Vector3(0.27219F, 0.27219F, 0.27219F));
                AddDisplayRule("RoboBallBossBody", "Shell", new Vector3(-0.62949F, 0.72152F, -0.15464F), new Vector3(30.74866F, 262.1811F, 353.4654F), new Vector3(0.27219F, 0.27219F, 0.27219F));
                AddDisplayRule("RoboBallBossBody", "Shell", new Vector3(0.15394F, -0.05496F, -0.22929F), new Vector3(358.8803F, 355.0521F, 274.0744F), new Vector3(0.27219F, 0.27219F, 0.27219F));
                AddDisplayRule("RoboBallBossBody", "Shell", new Vector3(-0.05954F, 0.00563F, -0.14659F), new Vector3(0F, 0F, 90F), new Vector3(0.27219F, 0.27219F, 0.27219F));
                AddDisplayRule("SuperRoboBallBossBody", "Shell", new Vector3(0.48786F, 0.59529F, -0.18982F), new Vector3(321.6017F, 270F, 352.831F), new Vector3(0.27219F, 0.27219F, 0.27219F));
                AddDisplayRule("SuperRoboBallBossBody", "Shell", new Vector3(-0.62949F, 0.72152F, -0.15464F), new Vector3(30.74866F, 262.1811F, 353.4654F), new Vector3(0.27219F, 0.27219F, 0.27219F));
                AddDisplayRule("SuperRoboBallBossBody", "Shell", new Vector3(0.15394F, -0.05496F, -0.22929F), new Vector3(358.8803F, 355.0521F, 274.0744F), new Vector3(0.27219F, 0.27219F, 0.27219F));
                AddDisplayRule("SuperRoboBallBossBody", "Shell", new Vector3(-0.05954F, 0.00563F, -0.14659F), new Vector3(0F, 0F, 90F), new Vector3(0.27219F, 0.27219F, 0.27219F));
                AddDisplayRule("GravekeeperBody", "JarBase", new Vector3(1.98524F, 0.17719F, -0.40357F), new Vector3(340.6301F, 78.55086F, 159.8601F), new Vector3(2.53588F, 2.53588F, 2.53588F));
                AddDisplayRule("ImpBossBody", "Neck", new Vector3(0F, -1.40541F, 0.63202F), new Vector3(0F, 90F, 348.3492F), new Vector3(1.32086F, 1.32086F, 1.32086F));
                AddDisplayRule("GrandParentBody", "Chest", new Vector3(0F, 6.00729F, -4.75472F), new Vector3(0F, 270F, 0F), new Vector3(2.75441F, 2.57376F, 2.75441F));
                AddDisplayRule("ScavBody", "Backpack", new Vector3(1.25963F, 9.77923F, -0.35636F), new Vector3(354.4142F, 254.9262F, 346.4824F), new Vector3(4.73313F, 4.73313F, 4.73313F));
            };
            ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
            {
                modelName = "mdlClayBruiser",
                transformLocation = "ClayBruiserArmature/ROOT/base/stomach/chest",
                childName = "Chest"
            });
            ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
            {
                modelName = "mdlRoboBallBoss",
                transformLocation = "RoboBallBossArmature/ROOT/Shell",
                childName = "Shell"
            });
            ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
            {
                modelName = "mdlGravekeeper",
                transformLocation = "GravekeeperArmature/ROOT/JarBase",
                childName = "JarBase"
            });
            ChildLocatorAdditions.list.Add(new ChildLocatorAdditions.Addition
            {
                modelName = "mdlGrandparent",
                transformLocation = "GrandparentArmature/ROOT/base/stomach/chest",
                childName = "Chest"
            });
            On.RoR2.BuffCatalog.Init += (orig) =>
            {
                orig();
                equipmentDef.passiveBuffDef = EliteVarietyContent.Buffs.AffixBuffing;
            };

            if (Main.aspectAbilitiesEnabled) AspectAbilitiesSupport();

            useSound = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            useSound.eventName = "EliteVariety_Play_item_use_affixbuffing";
            EliteVarietyContent.Resources.networkSoundEventDefs.Add(useSound);
        }

        public override void AspectAbilitiesSupport()
        {
            AspectAbilities.AspectAbilitiesPlugin.RegisterAspectAbility(new AspectAbilities.AspectAbility
            {
                equipmentDef = equipmentDef,
                aiMaxUseDistance = Mathf.Infinity,
                onUseOverride = (equipmentSlot) =>
                {
                    Buffs.AffixBuffing.EliteVarietyAffixBuffingBehavior component = equipmentSlot.characterBody.GetComponent<Buffs.AffixBuffing.EliteVarietyAffixBuffingBehavior>();
                    if (component)
                    {
                        EntitySoundManager.EmitSoundServer(useSound.index, equipmentSlot.gameObject);
                        component.TriggerExtraWardRadius(15f);
                        return true;
                    }
                    return false;
                }
            });
        }
    }
}
