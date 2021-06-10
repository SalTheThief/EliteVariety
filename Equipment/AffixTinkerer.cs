using RoR2;
using System.Collections.Generic;
using UnityEngine;
using MysticsRisky2Utils;
using RoR2.Audio;
using UnityEngine.Networking;
using R2API.Networking.Interfaces;
using R2API.Networking;
using RoR2.Orbs;

namespace EliteVariety.Equipment
{
    public class AffixTinkerer : BaseEliteAffix
    {
        public static GameObject linkOrbEffect;
        public static NetworkSoundEventDef onUseSound;
        public static GameObject effectRepairPrefab;

        public override void PreLoad()
        {
            base.PreLoad();
            equipmentDef.name = "AffixTinkerer";
            equipmentDef.cooldown = 15f;
        }

        public override void OnLoad()
        {
            base.OnLoad();
            SetAssets("Tinkerer");
            AdjustElitePickupMaterial(new Color32(50, 50, 50, 255), 0.5f, Main.AssetBundle.LoadAsset<Texture>("Assets/EliteVariety/Elites/Tinkerer/texRampEliteTinkerer.png"));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Head", new Vector3(0F, 0.34041F, 0.00382F), new Vector3(0F, 270F, 0F), new Vector3(0.14939F, 0.14939F, 0.14939F));
                AddDisplayRule("HuntressBody", "Head", new Vector3(0F, 0.29177F, -0.05573F), new Vector3(0F, 270F, 0F), new Vector3(0.09658F, 0.09658F, 0.09658F));
                AddDisplayRule("Bandit2Body", "Head", new Vector3(-0.00004F, 0.21336F, -0.01352F), new Vector3(346.0624F, 0F, 0F), new Vector3(0.11757F, 0.11757F, 0.11757F));
                AddDisplayRule("ToolbotBody", "Head", new Vector3(0F, 2.63622F, 2.21049F), new Vector3(358.1617F, 91.44128F, 51.89292F), new Vector3(1.20554F, 1.20554F, 1.20554F));
                AddDisplayRule("EngiBody", "HeadCenter", new Vector3(0F, 0.17322F, 0F), new Vector3(0F, 270F, 0F), new Vector3(0.13141F, 0.13141F, 0.13141F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0F, 0.76832F, -0.2789F), new Vector3(0F, 270F, 0F), new Vector3(0.80216F, 0.80385F, 0.80385F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0F, 1.3138F, -0.49117F), new Vector3(0F, 270F, 0F), new Vector3(0.79063F, 0.79229F, 0.79229F));
                AddDisplayRule("MageBody", "Head", new Vector3(-0.00408F, 0.13032F, -0.03472F), new Vector3(0F, 270F, 342.6337F), new Vector3(0.12078F, 0.12078F, 0.12078F));
                AddDisplayRule("MercBody", "Head", new Vector3(0F, -0.31854F, -0.09837F), new Vector3(0F, 270F, 349.7722F), new Vector3(0.46921F, 0.46921F, 0.46921F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(0.07958F, 2.21093F, -0.03811F), new Vector3(0F, 270F, 0F), new Vector3(0.46093F, 0.46093F, 0.46093F));
                AddDisplayRule("LoaderBody", "Head", new Vector3(0F, 0.24081F, 0.01637F), new Vector3(0F, 270F, 0F), new Vector3(0.14039F, 0.14039F, 0.14039F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(-0.36997F, 1.26087F, 1.90678F), new Vector3(301.5855F, 249.8843F, 287.3279F), new Vector3(1.48292F, 1.48292F, 1.48292F));
                AddDisplayRule("CaptainBody", "Head", new Vector3(0.00001F, 0.24554F, 0.04388F), new Vector3(0F, 270F, 0F), new Vector3(0.16429F, 0.16429F, 0.16429F));
                
                AddDisplayRule("WispBody", "Head", new Vector3(0F, 0F, 0.70763F), new Vector3(0F, 90F, 90F), new Vector3(0.63005F, 0.63005F, 0.63005F));
                AddDisplayRule("JellyfishBody", "Hull2", new Vector3(0.06641F, 0.94327F, 0.08761F), new Vector3(359.2905F, 274.3714F, 352.1665F), new Vector3(0.70429F, 0.70429F, 0.70429F));
                AddDisplayRule("BeetleBody", "Head", new Vector3(0.00703F, 0.43691F, 0.35467F), new Vector3(28.91702F, 319.116F, 341.4145F), new Vector3(0.62749F, 0.62749F, 0.62749F));
                AddDisplayRule("LemurianBody", "Head", new Vector3(-0.17158F, 3.78319F, -0.11333F), new Vector3(270F, 0F, 0F), new Vector3(1.43911F, 1.43911F, 1.43911F));
                AddDisplayRule("HermitCrabBody", "Base", new Vector3(0.00001F, 1.8819F, -0.00001F), new Vector3(0F, 0F, 0F), new Vector3(0.5111F, 0.5111F, 0.5111F));
                AddDisplayRule("ImpBody", "Neck", new Vector3(0F, 0.07492F, 0.01741F), new Vector3(0F, 90F, 0F), new Vector3(0.24011F, 0.24011F, 0.24011F));
                AddDisplayRule("VultureBody", "Head", new Vector3(-0.00002F, 1.8647F, -1.27185F), new Vector3(0F, 270F, 82.97913F), new Vector3(1.46533F, 1.21792F, 0.85331F));
                AddDisplayRule("RoboBallMiniBody", "ROOT", new Vector3(0F, 0.57858F, 0F), new Vector3(0F, 270F, 0F), new Vector3(0.80713F, 0.80713F, 0.80713F));
                AddDisplayRule("MiniMushroomBody", "Head", new Vector3(-0.63909F, -0.09858F, 0F), new Vector3(0F, 180F, 270F), new Vector3(0.54254F, 0.54254F, 0.54254F));
                AddDisplayRule("BellBody", "Chain", new Vector3(0F, 0.35436F, 0F), new Vector3(0F, 90F, 180F), new Vector3(1.4342F, 1.4342F, 1.4342F));
                AddDisplayRule("BeetleGuardBody", "Head", new Vector3(0F, -0.00002F, 2.27553F), new Vector3(0F, 90F, 90F), new Vector3(1.39082F, 1.39082F, 1.39082F));
                AddDisplayRule("BisonBody", "Head", new Vector3(0.00277F, 0.17604F, 0.79356F), new Vector3(0F, 270F, 270F), new Vector3(0.30597F, 0.30597F, 0.30597F));
                AddDisplayRule("GolemBody", "Head", new Vector3(0F, 0.80531F, 0.00001F), new Vector3(0F, 270F, 0F), new Vector3(0.68757F, 0.68757F, 0.68757F));
                AddDisplayRule("ParentBody", "Head", new Vector3(-72.77837F, 127.4748F, -0.57049F), new Vector3(0F, 0F, 42.30991F), new Vector3(50.0322F, 50.0322F, 50.0322F));
                AddDisplayRule("ClayBruiserBody", "Head", new Vector3(0.00077F, 0.49202F, 0.10396F), new Vector3(0F, 270F, 0.91104F), new Vector3(0.3562F, 0.3562F, 0.3562F));
                AddDisplayRule("GreaterWispBody", "MaskBase", new Vector3(0F, 0.89092F, 0F), new Vector3(0F, 270F, 0F), new Vector3(0.79403F, 0.79403F, 0.79403F));
                AddDisplayRule("LemurianBruiserBody", "Head", new Vector3(0F, 2.42635F, 0.04595F), new Vector3(0F, 270F, 270F), new Vector3(1.80072F, 1.80072F, 1.80072F));
                AddDisplayRule("NullifierBody", "Muzzle", new Vector3(0F, 1.18125F, 0.60903F), new Vector3(12.60184F, 0F, 0F), new Vector3(1.33467F, 1.33467F, 1.33467F));
                
                AddDisplayRule("BeetleQueen2Body", "Head", new Vector3(0F, 2.58076F, 0.26178F), new Vector3(0F, 90F, 330.7372F), new Vector3(1.46217F, 2.01176F, 2.23879F));
                AddDisplayRule("ClayBossBody", "PotLidTop", new Vector3(0F, 0.75866F, 1.03199F), new Vector3(0F, 270F, 0F), new Vector3(1.20857F, 1.20857F, 1.20857F));
                AddDisplayRule("TitanBody", "Head", new Vector3(0F, 6.38225F, 0.23452F), new Vector3(0F, 270F, 0F), new Vector3(2.02218F, 2.02218F, 2.02218F));
                AddDisplayRule("TitanGoldBody", "Head", new Vector3(0F, 5.42067F, 0.23449F), new Vector3(0F, 270F, 0F), new Vector3(2.02218F, 2.02218F, 2.02218F));
                AddDisplayRule("VagrantBody", "Hull", new Vector3(0F, 1.62232F, 0F), new Vector3(0F, 270F, 0F), new Vector3(0.98428F, 0.98428F, 0.98428F));
                string[] worms = new string[]
                {
                    "MagmaWormBody",
                    "ElectricWormBody"
                };
                foreach (string worm in worms)
                {
                    for (var i = 1; i <= 16; i++)
                    {
                        Vector3 scale = Vector3.one * (1.40164912F - 0.042983125F * (i - 1));
                        AddDisplayRule(worm, "Neck" + i.ToString(), new Vector3(0F, 0.58245F + 0.08892F * (i - 1), -0.42705F + 0.012033125F * (i - 1)), new Vector3(0F, 90F, 0F), scale);
                    }
                }
                AddDisplayRule("RoboBallBossBody", "Shell", new Vector3(0F, 1.21929F, 0F), new Vector3(0F, 270F, 0F), new Vector3(0.57092F, 0.57092F, 0.57092F));
                AddDisplayRule("SuperRoboBallBossBody", "Shell", new Vector3(0F, 1.21929F, 0F), new Vector3(0F, 270F, 0F), new Vector3(0.57092F, 0.57092F, 0.57092F));
                AddDisplayRule("GravekeeperBody", "Head", new Vector3(0F, 2.89883F, -0.10267F), new Vector3(0F, 0F, 0F), new Vector3(1.0386F, 1.0386F, 1.0386F));
                AddDisplayRule("ImpBossBody", "Neck", new Vector3(0F, 0.49847F, -0.04553F), new Vector3(0F, 90F, 358.5645F), new Vector3(1.32086F, 1.32086F, 1.32086F));
                AddDisplayRule("GrandParentBody", "Head", new Vector3(0F, 1.14019F, -0.55074F), new Vector3(0F, 270F, 0F), new Vector3(1.32372F, 1.32372F, 1.32372F));
                AddDisplayRule("ScavBody", "Chest", new Vector3(0F, 6.53225F, 2.49379F), new Vector3(0F, 90F, 20.32931F), new Vector3(4.73313F, 4.73313F, 4.73313F));
            };

            if (Main.aspectAbilitiesEnabled) AspectAbilitiesSupport();

            linkOrbEffect = Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Elites/Tinkerer/TinkererDroneLinkOrbEffect.prefab");
            EffectComponent effectComponent = linkOrbEffect.AddComponent<EffectComponent>();
            effectComponent.positionAtReferencedTransform = false;
            effectComponent.parentToReferencedTransform = false;
            effectComponent.applyScale = true;
            VFXAttributes vfxAttributes = linkOrbEffect.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            OrbEffect orbEffect = linkOrbEffect.AddComponent<OrbEffect>();
            orbEffect.startVelocity1 = new Vector3(0f, 7f, 0f);
            orbEffect.startVelocity2 = new Vector3(0f, 10f, 0f);
            orbEffect.endVelocity1 = new Vector3(-10f, -10f, -10f);
            orbEffect.endVelocity2 = new Vector3(10f, 10f, 10f);
            orbEffect.movementCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0f, 0f),
                    new Keyframe(1f, 1f)
                },
                preWrapMode = WrapMode.Clamp,
                postWrapMode = WrapMode.Clamp
            };
            orbEffect.faceMovement = true;
            orbEffect.callArrivalIfTargetIsGone = false;
            DestroyOnTimer destroyOnTimer = linkOrbEffect.transform.Find("TrailParent/Trail").gameObject.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 3f;
            destroyOnTimer.enabled = false;
            MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsOrbEffectOnArrivalDefaults onArrivalDefaults = linkOrbEffect.AddComponent<MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsOrbEffectOnArrivalDefaults>();
            onArrivalDefaults.orbEffect = orbEffect;
            onArrivalDefaults.transformsToUnparentChildren = new Transform[] {
                linkOrbEffect.transform.Find("TrailParent")
            };
            onArrivalDefaults.componentsToEnable = new MonoBehaviour[]
            {
                destroyOnTimer
            };

            EliteVarietyContent.Resources.effectPrefabs.Add(linkOrbEffect);

            onUseSound = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            onUseSound.eventName = "EliteVariety_Play_item_use_affixtinkerer";
            EliteVarietyContent.Resources.networkSoundEventDefs.Add(onUseSound);

            effectRepairPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Elites/Tinkerer/RepairEffect.prefab");
            effectComponent = effectRepairPrefab.AddComponent<EffectComponent>();
            effectComponent.applyScale = true;
            effectComponent.parentToReferencedTransform = true;
            vfxAttributes = effectRepairPrefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            EliteVarietyContent.Resources.effectPrefabs.Add(effectRepairPrefab);
        }

        public override void AfterContentPackLoaded()
        {
            base.AfterContentPackLoaded();
            equipmentDef.passiveBuffDef = EliteVarietyContent.Buffs.AffixTinkerer;
        }

        public override void AspectAbilitiesSupport()
        {
            AspectAbilities.AspectAbilitiesPlugin.RegisterAspectAbility(new AspectAbilities.AspectAbility
            {
                equipmentDef = equipmentDef,
                aiMaxUseDistance = Mathf.Infinity,
                onUseOverride = (equipmentSlot) =>
                {
                    if (equipmentSlot.inventory)
                    {
                        foreach (TeamComponent teamMember in TeamComponent.GetTeamMembers(equipmentSlot.teamComponent.teamIndex))
                        {
                            if (teamMember.body && (teamMember.body.bodyFlags & CharacterBody.BodyFlags.Mechanical) > 0 && teamMember.body.master && teamMember.body.master.minionOwnership && teamMember.body.master.minionOwnership.ownerMaster && teamMember.body.healthComponent)
                            {
                                TinkererHealOrb orb = new TinkererHealOrb();
                                orb.healthComponentToHeal = teamMember.body.healthComponent;
                                orb.healFlat = 8f;
                                orb.healFraction = 0.5f;
                                orb.origin = equipmentSlot.characterBody.corePosition;
                                orb.target = teamMember.body.mainHurtBox;
                                OrbManager.instance.AddOrb(orb);
                            }
                        }

                        EntitySoundManager.EmitSoundServer(onUseSound.index, equipmentSlot.gameObject);

                        return true;
                    }
                    return false;
                }
            });
        }

        public class TinkererHealOrb : Orb
        {
            public HealthComponent healthComponentToHeal;
            public float healFlat;
            public float healFraction;

            public override void Begin()
            {
                duration = 0.5f;
                EffectData effectData = new EffectData
                {
                    origin = origin,
                    genericFloat = duration
                };
                effectData.SetHurtBoxReference(target);
                EffectManager.SpawnEffect(linkOrbEffect, effectData, true);
            }

            public override void OnArrival()
            {
                if (healthComponentToHeal)
                {
                    healthComponentToHeal.Heal(healFlat + healthComponentToHeal.fullHealth * healFraction, default(ProcChainMask));
                }
                if (target && target.healthComponent && target.healthComponent.body)
                {
                    EffectData effectData = new EffectData
                    {
                        origin = target.healthComponent.body.corePosition,
                        scale = target.healthComponent.body.radius
                    };
                    effectData.SetHurtBoxReference(target.healthComponent.body.gameObject);
                    EffectManager.SpawnEffect(effectRepairPrefab, effectData, true);
                }
            }
        }
    }
}
