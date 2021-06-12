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
                AddDisplayRule("CommandoBody", "Head", new Vector3(0F, 0.29966F, 0.24059F), new Vector3(345.4218F, 0F, 0F), new Vector3(0.12113F, 0.12113F, 0.12113F));
                AddDisplayRule("HuntressBody", "Head", new Vector3(0F, 0.26231F, 0.22105F), new Vector3(344.1714F, 0F, 0F), new Vector3(0.09658F, 0.09658F, 0.09658F));
                AddDisplayRule("Bandit2Body", "Head", new Vector3(-0.00101F, 0.05083F, 0.3401F), new Vector3(5.42616F, 0F, 0F), new Vector3(0.09104F, 0.09104F, 0.09104F));
                AddDisplayRule("ToolbotBody", "Head", new Vector3(-0.17239F, 4.0752F, -1.83269F), new Vector3(303.0908F, 179.2949F, 1.63074F), new Vector3(1.20554F, 1.20554F, 1.20554F));
                AddDisplayRule("EngiBody", "HeadCenter", new Vector3(0F, -0.01063F, 0.24091F), new Vector3(11.36212F, 0F, 0F), new Vector3(0.13141F, 0.13141F, 0.13141F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0F, 1.72741F, 0.27924F), new Vector3(0F, 0F, 0F), new Vector3(0.56685F, 0.56804F, 0.56804F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0F, 0.85031F, 1.16546F), new Vector3(0F, 0F, 0F), new Vector3(0.56316F, 0.56434F, 0.56434F));
                AddDisplayRule("MageBody", "Head", new Vector3(-0.00936F, 0.05186F, 0.18905F), new Vector3(0F, 0F, 0F), new Vector3(0.12078F, 0.12078F, 0.12078F));
                AddDisplayRule("MercBody", "Head", new Vector3(0F, 0.09905F, 0.22047F), new Vector3(0F, 0F, 0F), new Vector3(0.13241F, 0.13241F, 0.13241F));
                AddDisplayRule("TreebotBody", "PlatformBase", new Vector3(0F, -0.56082F, 0.90972F), new Vector3(24.55128F, 0F, 0F), new Vector3(0.29737F, 0.29737F, 0.29737F));
                AddDisplayRule("LoaderBody", "Head", new Vector3(0F, 0.11089F, 0.4385F), new Vector3(0F, 0F, 0F), new Vector3(0.14039F, 0.14039F, 0.14039F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(-1.97458F, 2.59594F, 0.50771F), new Vector3(339.2264F, 270F, 270F), new Vector3(0.78129F, 0.78129F, 0.78129F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(2.16853F, 2.59597F, 0.50755F), new Vector3(343.907F, 90F, 90F), new Vector3(0.78129F, 0.78129F, 0.78129F));
                AddDisplayRule("CaptainBody", "Head", new Vector3(-0.0056F, 0.08758F, 0.28825F), new Vector3(0F, 0F, 0F), new Vector3(0.1122F, 0.1122F, 0.1122F));
                
                AddDisplayRule("WispBody", "Head", new Vector3(0F, 0.71386F, -0.0195F), new Vector3(270F, 180F, 0F), new Vector3(0.58181F, 0.58181F, 0.58181F));
                AddDisplayRule("JellyfishBody", "Hull2", new Vector3(0.30352F, 1.43813F, 0.19542F), new Vector3(282.2123F, 56.21837F, 308.6966F), new Vector3(0.76073F, 0.76073F, 0.76073F));
                AddDisplayRule("BeetleBody", "Head", new Vector3(0.08712F, 0.48435F, -0.71049F), new Vector3(333.6425F, 178.0887F, 357.6602F), new Vector3(0.43294F, 0.43294F, 0.43294F));
                AddDisplayRule("LemurianBody", "Head", new Vector3(-0.17158F, 3.78319F, -0.11333F), new Vector3(270F, 0F, 0F), new Vector3(1.43911F, 1.43911F, 1.43911F));
                AddDisplayRule("HermitCrabBody", "Base", new Vector3(0.00002F, 1.20259F, 0.74658F), new Vector3(0F, 0F, 0F), new Vector3(0.5111F, 0.5111F, 0.5111F));
                AddDisplayRule("ImpBody", "Neck", new Vector3(0F, -0.172F, -0.30964F), new Vector3(348.5848F, 180F, 0F), new Vector3(0.13068F, 0.13068F, 0.13068F));
                AddDisplayRule("VultureBody", "Head", new Vector3(-0.00004F, 5.13124F, -0.62601F), new Vector3(278.3651F, 180F, 180F), new Vector3(1.23141F, 1.23141F, 1.23141F));
                AddDisplayRule("RoboBallMiniBody", "ROOT", new Vector3(0F, 0.00998F, 1.09398F), new Vector3(0F, 0F, 0F), new Vector3(0.31248F, 0.31248F, 0.28216F));
                AddDisplayRule("MiniMushroomBody", "Head", new Vector3(0.91357F, -1.02106F, 0F), new Vector3(84.28553F, 90F, 180F), new Vector3(0.26536F, 0.26536F, 0.26536F));
                AddDisplayRule("BellBody", "Chain", new Vector3(-1.62579F, 1.10939F, -1.08786F), new Vector3(0F, 236.2126F, 180F), new Vector3(0.39593F, 0.39593F, 0.39593F));
                AddDisplayRule("BeetleGuardBody", "Head", new Vector3(0.17396F, 1.90719F, 0.42468F), new Vector3(276.597F, 348.2153F, 185.774F), new Vector3(0.67094F, 0.67094F, 0.67094F));
                AddDisplayRule("BisonBody", "Head", new Vector3(-0.00756F, 0.88121F, 0.0533F), new Vector3(270.8393F, 270.0001F, 270F), new Vector3(0.28029F, 0.28029F, 0.28029F));
                AddDisplayRule("GolemBody", "Head", new Vector3(0F, 0.70964F, 0.73467F), new Vector3(0F, 0F, 0F), new Vector3(0.30644F, 0.30644F, 0.30644F));
                AddDisplayRule("ParentBody", "Head", new Vector3(62.93F, 100.125F, 0.65524F), new Vector3(317.6959F, 88.84124F, 0.77998F), new Vector3(33.58714F, 33.58714F, 33.58714F));
                AddDisplayRule("ClayBruiserBody", "Head", new Vector3(0.03907F, 0.40008F, 0.70187F), new Vector3(351.5047F, 3.68439F, 359.9409F), new Vector3(0.20678F, 0.20678F, 0.20678F));
                AddDisplayRule("GreaterWispBody", "MaskBase", new Vector3(0F, 0.25121F, 1.07853F), new Vector3(0F, 0F, 0F), new Vector3(0.24062F, 0.24062F, 0.24062F));
                AddDisplayRule("LemurianBruiserBody", "Head", new Vector3(0.00455F, 5.71222F, 0.04572F), new Vector3(270.6796F, 270F, 270F), new Vector3(0.84517F, 0.84517F, 0.84517F));
                AddDisplayRule("NullifierBody", "Muzzle", new Vector3(0F, -0.42413F, 2.33217F), new Vector3(32.39762F, 0F, 0F), new Vector3(0.44779F, 0.44779F, 0.44779F));
                
                AddDisplayRule("BeetleQueen2Body", "Head", new Vector3(-0.07684F, 1.65008F, -2.01268F), new Vector3(0.60901F, 186.126F, 358.306F), new Vector3(0.46099F, 0.46099F, 0.46099F));
                AddDisplayRule("ClayBossBody", "PotBase", new Vector3(0F, 0.41822F, 3.11682F), new Vector3(0F, 0F, 0F), new Vector3(0.5485F, 0.5485F, 0.5485F));
                AddDisplayRule("TitanBody", "Head", new Vector3(0.34594F, 1.3132F, 2.81336F), new Vector3(0F, 0F, 0F), new Vector3(0.90732F, 0.90732F, 0.90732F));
                AddDisplayRule("TitanGoldBody", "Head", new Vector3(0.34594F, 1.3132F, 2.81336F), new Vector3(0F, 0F, 0F), new Vector3(0.90732F, 0.90732F, 0.90732F));
                AddDisplayRule("VagrantBody", "Hull", new Vector3(0F, 1.30998F, 1.3594F), new Vector3(0F, 0F, 0F), new Vector3(0.38323F, 0.38323F, 0.38323F));
                string[] worms = new string[]
                {
                    "MagmaWormBody",
                    "ElectricWormBody"
                };
                foreach (string worm in worms)
                {
                    AddDisplayRule(worm, "Head", new Vector3(0F, -1.83452F, -1.28648F), new Vector3(82.25841F, 180.0001F, 0F), new Vector3(0.5628F, 0.5628F, 0.5628F));
                }
                AddDisplayRule("RoboBallBossBody", "MainEyeMuzzle", new Vector3(0F, 0.03414F, -0.05411F), new Vector3(0F, 0F, 0F), new Vector3(0.24644F, 0.24644F, 0.24644F));
                AddDisplayRule("SuperRoboBallBossBody", "MainEyeMuzzle", new Vector3(0F, 0.03414F, -0.05411F), new Vector3(0F, 0F, 0F), new Vector3(0.24644F, 0.24644F, 0.24644F));
                AddDisplayRule("GravekeeperBody", "Head", new Vector3(0F, 1.15209F, 3.44118F), new Vector3(342.9781F, 0F, 0F), new Vector3(0.83189F, 0.83189F, 0.83189F));
                AddDisplayRule("ImpBossBody", "Neck", new Vector3(0.04392F, -1.25395F, -2.24839F), new Vector3(350.0941F, 180.2507F, 358.5428F), new Vector3(0.95173F, 0.95173F, 0.95173F));
                AddDisplayRule("GrandParentBody", "Head", new Vector3(0F, 1.14006F, 1.14233F), new Vector3(0F, 0F, 0F), new Vector3(0.91952F, 0.91952F, 0.91952F));
                AddDisplayRule("ScavBody", "Head", new Vector3(0F, 6.55211F, -1.8542F), new Vector3(288.6726F, 180F, 180F), new Vector3(2.64228F, 2.64228F, 2.64228F));
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
            destroyOnTimer.duration = 0.5f;
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
