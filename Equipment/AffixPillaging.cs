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
    public class AffixPillaging : BaseEliteAffix
    {
        public static GameObject networkedCostPrefab;
        public static int baseCost = 50;
        public static CostTypeIndex costType = CostTypeIndex.Money;
        public static int itemCount = 5;
        public static GameObject itemAcquiredOrbEffect;
        public static GameObject onUseEffect;

        public override void PreLoad()
        {
            base.PreLoad();
            equipmentDef.name = "AffixPillaging";
            equipmentDef.cooldown = 90f;
        }

        public override void OnLoad()
        {
            base.OnLoad();
            SetAssets("Pillaging");
            AdjustElitePickupMaterial(new Color32(216, 207, 61, 255), 2f, true);
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Head", new Vector3(0F, 0.37741F, 0.02367F), new Vector3(5.07642F, 0F, 0F), new Vector3(0.14939F, 0.14939F, 0.14939F));
                AddDisplayRule("HuntressBody", "Head", new Vector3(0F, 0.32502F, -0.05572F), new Vector3(354.6689F, 0F, 0F), new Vector3(0.09658F, 0.09658F, 0.09658F));
                AddDisplayRule("Bandit2Body", "Head", new Vector3(-0.00003F, 0.17107F, -0.00583F), new Vector3(349.5076F, 0F, 0F), new Vector3(0.11757F, 0.11757F, 0.11757F));
                AddDisplayRule("ToolbotBody", "Head", new Vector3(0F, 2.44052F, 1.34699F), new Vector3(58.1895F, 291.422F, 294.7821F), new Vector3(1.20554F, 1.20554F, 1.20554F));
                AddDisplayRule("EngiBody", "HeadCenter", new Vector3(0F, 0.17322F, 0F), new Vector3(0F, 0F, 0F), new Vector3(0.13141F, 0.13141F, 0.13141F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0F, 0.8902F, -0.00008F), new Vector3(0F, 270F, 355.8558F), new Vector3(0.48895F, 0.48998F, 0.48998F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0.0006F, 1.56272F, -1.1861F), new Vector3(353.0465F, 0F, 0F), new Vector3(0.43814F, 0.43906F, 0.43906F));
                AddDisplayRule("MageBody", "Head", new Vector3(-0.00408F, 0.19861F, -0.11468F), new Vector3(344.8334F, 0F, 0F), new Vector3(0.08068F, 0.08068F, 0.08068F));
                AddDisplayRule("MercBody", "Head", new Vector3(0F, 0.26353F, 0.05679F), new Vector3(11.59233F, 0F, 0F), new Vector3(0.10902F, 0.10902F, 0.10902F));
                AddDisplayRule("TreebotBody", "FlowerBase", new Vector3(0.07958F, 1.06987F, -0.03811F), new Vector3(7.85321F, 355.2752F, 1.61757F), new Vector3(0.87261F, 0.87261F, 0.87261F));
                AddDisplayRule("LoaderBody", "Head", new Vector3(0F, 0.26025F, 0.01637F), new Vector3(2.0328F, 0F, 0F), new Vector3(0.10256F, 0.10256F, 0.10256F));
                AddDisplayRule("CrocoBody", "Head", new Vector3(0F, 0.62007F, 1.718F), new Vector3(79.09709F, 180F, 180F), new Vector3(1.48292F, 1.48292F, 1.48292F));
                AddDisplayRule("CaptainBody", "Head", new Vector3(0.00001F, 0.24555F, -0.02541F), new Vector3(329.5633F, 0F, 0F), new Vector3(0.12897F, 0.12897F, 0.12897F));
                
                AddDisplayRule("WispBody", "Head", new Vector3(0F, 0F, 0.70763F), new Vector3(0F, 90F, 90F), new Vector3(0.63005F, 0.63005F, 0.63005F));
                AddDisplayRule("JellyfishBody", "Hull2", new Vector3(0.06416F, 1.29074F, 0.10769F), new Vector3(0F, 273.5969F, 356.2539F), new Vector3(0.76511F, 0.76511F, 0.76511F));
                AddDisplayRule("BeetleBody", "Head", new Vector3(0F, 0.60674F, 0.47416F), new Vector3(27.76727F, 0F, 0F), new Vector3(0.48181F, 0.48181F, 0.48181F));
                AddDisplayRule("LemurianBody", "Head", new Vector3(-0.00005F, 0.56781F, -1.15991F), new Vector3(0F, 270F, 98.95686F), new Vector3(1.19111F, 0.88009F, 0.88009F));
                AddDisplayRule("HermitCrabBody", "Base", new Vector3(-0.00291F, 0.91927F, -0.00091F), new Vector3(0.4059F, 294.7679F, 353.7266F), new Vector3(0.44483F, 0.44483F, 0.44483F));
                AddDisplayRule("ImpBody", "Neck", new Vector3(0F, 0.07492F, 0.01741F), new Vector3(0F, 270F, 0F), new Vector3(0.15634F, 0.24011F, 0.30092F));
                AddDisplayRule("VultureBody", "Head", new Vector3(-0.00001F, 1.31799F, -1.15628F), new Vector3(0F, 90F, 270F), new Vector3(1.46895F, 1.22093F, 0.85542F));
                AddDisplayRule("RoboBallMiniBody", "ROOT", new Vector3(0F, 1.08641F, 0F), new Vector3(0F, 0F, 0F), new Vector3(0.74351F, 0.74351F, 0.74351F));
                AddDisplayRule("MiniMushroomBody", "Head", new Vector3(-0.52796F, 0F, 0F), new Vector3(0F, 0F, 90F), new Vector3(0.48255F, 0.48255F, 0.48255F));
                AddDisplayRule("BellBody", "Chain", new Vector3(0F, -1.0985F, 0F), new Vector3(0F, 335.945F, 180F), new Vector3(1.4342F, 1.4342F, 1.4342F));
                AddDisplayRule("BeetleGuardBody", "Head", new Vector3(-0.00002F, 0.28446F, 1.31143F), new Vector3(90F, 0F, 0F), new Vector3(0.90911F, 0.90911F, 0.90911F));
                AddDisplayRule("BisonBody", "Head", new Vector3(0.00278F, 0.197F, 0.65322F), new Vector3(357.2056F, 270F, 275.201F), new Vector3(0.30422F, 0.30422F, 0.30422F));
                AddDisplayRule("GolemBody", "Head", new Vector3(0F, 1.49375F, 0F), new Vector3(0F, 0F, 0F), new Vector3(0.68757F, 0.68757F, 0.68757F));
                AddDisplayRule("ParentBody", "Head", new Vector3(-62.64863F, 116.3463F, -0.57049F), new Vector3(0F, 0F, 42.30991F), new Vector3(50.0322F, 50.0322F, 50.0322F));
                AddDisplayRule("ClayBruiserBody", "Head", new Vector3(0.00077F, 0.43624F, 0.10485F), new Vector3(0F, 0F, 0.91104F), new Vector3(0.3562F, 0.3562F, 0.3562F));
                AddDisplayRule("ClayBruiserBody", "Muzzle", new Vector3(-0.00271F, -0.06877F, 0.00003F), new Vector3(90F, 0F, 0F), new Vector3(0.46344F, 0.46344F, 0.46344F));
                AddDisplayRule("GreaterWispBody", "MaskBase", new Vector3(0F, 1.20266F, 0F), new Vector3(0F, 270F, 0F), new Vector3(0.79403F, 0.79403F, 0.79403F));
                AddDisplayRule("LemurianBruiserBody", "Head", new Vector3(0F, 1.94651F, 1.22201F), new Vector3(88.15273F, 0F, 0F), new Vector3(1.57354F, 1.57354F, 1.57354F));
                AddDisplayRule("NullifierBody", "Muzzle", new Vector3(0F, 1.27662F, 0.63032F), new Vector3(12.60184F, 0F, 0F), new Vector3(1.11167F, 1.11167F, 1.11167F));
                
                AddDisplayRule("BeetleQueen2Body", "Head", new Vector3(0F, 2.16941F, 2.46084F), new Vector3(0F, 90F, 84.92046F), new Vector3(2.06568F, 2.84211F, 3.16285F));
                AddDisplayRule("ClayBossBody", "PotLidTop", new Vector3(0F, 0.75866F, 1.03199F), new Vector3(0F, 0F, 0F), new Vector3(1.20857F, 1.20857F, 1.20857F));
                AddDisplayRule("TitanBody", "Head", new Vector3(0F, 6.38225F, 0.23452F), new Vector3(0F, 270F, 0F), new Vector3(2.02218F, 2.02218F, 2.02218F));
                AddDisplayRule("TitanGoldBody", "Head", new Vector3(0F, 5.42067F, 0.23449F), new Vector3(0F, 270F, 0F), new Vector3(2.02218F, 2.02218F, 2.02218F));
                AddDisplayRule("VagrantBody", "Hull", new Vector3(0F, 1.82207F, 0F), new Vector3(0F, 270F, 0F), new Vector3(0.72348F, 0.72348F, 0.72348F));
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
                AddDisplayRule("GrandParentBody", "Head", new Vector3(0F, 0.52978F, -0.55073F), new Vector3(0F, 0F, 0F), new Vector3(1.32372F, 1.32372F, 1.32372F));
                AddDisplayRule("ScavBody", "Chest", new Vector3(0F, 6.86485F, 2.6171F), new Vector3(25.44132F, 0F, 0F), new Vector3(4.73313F, 4.73313F, 4.73313F));
            };
            On.RoR2.BuffCatalog.Init += (orig) =>
            {
                orig();
                equipmentDef.passiveBuffDef = EliteVarietyContent.Buffs.AffixPillaging;
            };

            if (Main.aspectAbilitiesEnabled) AspectAbilitiesSupport();

            On.RoR2.Language.GetLocalizedStringByToken += Language_GetLocalizedStringByToken;

            networkedCostPrefab = Utils.CreateBlankPrefab(Main.TokenPrefix + "AffixPillagingNetworkedCost");
            networkedCostPrefab.AddComponent<EliteVarietyAffixPillagingNetworkedCost>();

            NetworkingAPI.RegisterMessageType<EliteVarietyAffixPillagingNetworkedCost.SyncScaling>();

            On.RoR2.SceneDirector.PopulateScene += SceneDirector_PopulateScene;

            itemAcquiredOrbEffect = R2API.PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/ItemTakenOrbEffect"), Main.TokenPrefix + "AffixPillagingItemAcquiredOrbEffect", false);
            itemAcquiredOrbEffect.transform.localPosition = new Vector3(-0.4f, 0f, -9.9f);
            OrbEffect orbEffect = itemAcquiredOrbEffect.GetComponent<OrbEffect>();
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
            orbEffect.startVelocity1 = new Vector3(0f, 5f, 0f);
            orbEffect.startVelocity2 = new Vector3(0f, 10f, 0f);
            EliteVarietyContent.Resources.effectPrefabs.Add(itemAcquiredOrbEffect);

            onUseEffect = Resources.Load<GameObject>("Prefabs/Effects/MoneyPackPickupEffect");
        }

        public override void AspectAbilitiesSupport()
        {
            AspectAbilities.AspectAbilitiesPlugin.RegisterAspectAbility(new AspectAbilities.AspectAbility
            {
                equipmentDef = equipmentDef,
                aiMaxUseDistance = Mathf.Infinity,
                onUseOverride = (equipmentSlot) =>
                {
                    if (EliteVarietyAffixPillagingNetworkedCost.instance)
                    {
                        CostTypeDef costTypeDef = CostTypeCatalog.GetCostTypeDef(costType);
                        if (costTypeDef != null)
                        {
                            Interactor interactor = equipmentSlot.characterBody.GetComponent<Interactor>();
                            if (interactor) {
                                if (costTypeDef.IsAffordable(EliteVarietyAffixPillagingNetworkedCost.instance.cost, interactor))
                                {
                                    costTypeDef.PayCost(EliteVarietyAffixPillagingNetworkedCost.instance.cost, interactor, equipmentSlot.gameObject, RoR2Application.rng, ItemIndex.None);

                                    PickupIndex pickupIndex = RoR2Application.rng.NextElementUniform(Run.instance.availableTier1DropList);
                                    ItemIndex itemIndex = PickupCatalog.GetPickupDef(pickupIndex).itemIndex;

                                    equipmentSlot.inventory.GiveItem(itemIndex, itemCount);

                                    EffectData effectData = new EffectData
                                    {
                                        origin = equipmentSlot.characterBody.corePosition,
                                        genericFloat = 4f,
                                        genericUInt = (uint)(itemIndex + 1)
                                    };
                                    effectData.SetNetworkedObjectReference(equipmentSlot.characterBody.gameObject);
                                    EffectManager.SpawnEffect(itemAcquiredOrbEffect, effectData, true);

                                    EffectManager.SimpleEffect(onUseEffect, equipmentSlot.characterBody.corePosition, Quaternion.identity, true);

                                    return true;
                                }
                            }
                        }
                    }
                    return false;
                }
            });
        }

        public class EliteVarietyAffixPillagingNetworkedCost : MonoBehaviour
        {
            public int cost;
            public static EliteVarietyAffixPillagingNetworkedCost instance;

            public void Awake()
            {
                if (instance) Object.Destroy(instance.gameObject);
                instance = this;
                if (NetworkServer.active)
                {
                    if (Run.instance)
                    {
                        cost = Run.instance.GetDifficultyScaledCost(baseCost);
                        new SyncScaling(cost).Send(NetworkDestination.Clients);
                    }
                }
            }

            public class SyncScaling : INetMessage
            {
                int cost;

                public SyncScaling()
                {
                }

                public SyncScaling(int cost)
                {
                    this.cost = cost;
                }

                public void Deserialize(NetworkReader reader)
                {
                    cost = reader.ReadInt32();
                }

                public void OnReceived()
                {
                    if (NetworkServer.active) return;

                    if (!EliteVarietyAffixPillagingNetworkedCost.instance)
                    {
                        EliteVarietyAffixPillagingNetworkedCost.instance = Object.Instantiate<GameObject>(networkedCostPrefab).GetComponent<EliteVarietyAffixPillagingNetworkedCost>();
                    }
                    EliteVarietyAffixPillagingNetworkedCost.instance.cost = cost;
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(cost);
                }
            }
        }

        private static string Language_GetLocalizedStringByToken(On.RoR2.Language.orig_GetLocalizedStringByToken orig, Language self, string token)
        {
            string localizedString = orig(self, token);

            string chestCostReplacementToken = "ELITEVARIETY_PILLAGING_COST";
            if (localizedString.Contains(chestCostReplacementToken))
            {
                string costString = "";
                int cost = baseCost;
                if (EliteVarietyAffixPillagingNetworkedCost.instance) cost = EliteVarietyAffixPillagingNetworkedCost.instance.cost;

                CostTypeDef costTypeDef = CostTypeCatalog.GetCostTypeDef(costType);
                if (costTypeDef != null)
                {
                    Main.sharedStringBuilder.Clear();
                    costTypeDef.BuildCostStringStyled(cost, Main.sharedStringBuilder, true, false);
                    costString = string.Format(
                        "<color=#{1}>{0}</color>",
                        Main.sharedStringBuilder.ToString(),
                        ColorUtility.ToHtmlStringRGB(costTypeDef.GetCostColor(false))
                    );
                }

                localizedString = localizedString.Replace(chestCostReplacementToken, costString);
            }

            return localizedString;
        }

        private void SceneDirector_PopulateScene(On.RoR2.SceneDirector.orig_PopulateScene orig, SceneDirector self)
        {
            orig(self);
            Object.Instantiate<GameObject>(networkedCostPrefab);
        }
    }
}
