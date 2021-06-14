using RoR2;
using System.Collections.Generic;
using UnityEngine;
using MysticsRisky2Utils;
using RoR2.Audio;
using UnityEngine.Networking;
using R2API.Networking.Interfaces;
using R2API.Networking;
using RoR2.Orbs;
using R2API;
using UnityEngine.Events;

namespace EliteVariety.Equipment
{
    public class AffixImpPlane : BaseEliteAffix
    {
        public static DeployableSlot deployableSlot;

        public override void PreLoad()
        {
            base.PreLoad();
            equipmentDef.name = "AffixImpPlane";
            equipmentDef.cooldown = 90f;
        }

        public override void OnLoad()
        {
            base.OnLoad();
            SetAssets("ImpPlane");
            AdjustElitePickupMaterial(new Color32(50, 50, 50, 255), 0.5f, Main.AssetBundle.LoadAsset<Texture>("Assets/EliteVariety/Elites/ImpPlane/texRampEliteImpPlane.png"));
            onSetupIDRS += () =>
            {
                AddDisplayRule("CommandoBody", "Head", new Vector3(-0.00126F, 0.48154F, -0.04871F), new Vector3(345.4218F, 0F, 0F), new Vector3(0.89712F, 0.89712F, 0.89712F));
                AddDisplayRule("HuntressBody", "Head", new Vector3(-0.00259F, 0.48967F, -0.14657F), new Vector3(344.1714F, 0F, 0F), new Vector3(0.65889F, 0.65889F, 0.65889F));
                AddDisplayRule("Bandit2Body", "Head", new Vector3(-0.05818F, 0.30676F, 0.03399F), new Vector3(-0.00299F, 0.03356F, 6.51725F), new Vector3(0.6081F, 0.6081F, 0.6081F));
                AddDisplayRule("ToolbotBody", "Head", new Vector3(-0.00033F, 3.45978F, 2.30121F), new Vector3(303.0908F, 179.2949F, 1.63074F), new Vector3(7.25493F, 7.25493F, 7.25493F));
                AddDisplayRule("EngiBody", "HeadCenter", new Vector3(-0.00041F, 0.67026F, 0.06282F), new Vector3(11.36212F, 0F, 0F), new Vector3(1.1165F, 0.73043F, 0.97422F));
                AddDisplayRule("EngiTurretBody", "Head", new Vector3(0F, 1.65904F, 0.12268F), new Vector3(0F, 0F, 0F), new Vector3(3.46611F, 3.47339F, 3.47339F));
                AddDisplayRule("EngiWalkerTurretBody", "Head", new Vector3(0F, 1.97967F, -0.44208F), new Vector3(0F, 0F, 0F), new Vector3(2.77523F, 2.78104F, 2.78104F));
                AddDisplayRule("MageBody", "Head", new Vector3(-0.01112F, 0.40792F, 0.01596F), new Vector3(0F, 0F, 0F), new Vector3(0.93946F, 0.93946F, 0.93946F));
                AddDisplayRule("MercBody", "Head", new Vector3(-0.00147F, 0.43593F, 0.01005F), new Vector3(0F, 0F, 0F), new Vector3(1.01246F, 1.01246F, 1.01246F));
                AddDisplayRule("TreebotBody", "Base", new Vector3(0F, 0.42206F, -3.30873F), new Vector3(270F, 0F, 0F), new Vector3(2.82236F, 2.82236F, 2.82236F));
                AddDisplayRule("LoaderBody", "Head", new Vector3(-0.00156F, 0.41031F, 0.00739F), new Vector3(0F, 0F, 0F), new Vector3(0.96196F, 0.96196F, 0.96196F));
                AddDisplayRule("CrocoBody", "Base", new Vector3(0F, -3.54336F, 8.66909F), new Vector3(90F, 0F, 0F), new Vector3(14.48214F, 19.38662F, 18.77905F));
                AddDisplayRule("CaptainBody", "Head", new Vector3(0F, 0.39846F, -0.0148F), new Vector3(0F, 0F, 0F), new Vector3(1.47522F, 1.47522F, 1.47522F));
                
                AddDisplayRule("WispBody", "Head", new Vector3(0F, 0F, -1.36332F), new Vector3(270F, 0F, 0F), new Vector3(2.63941F, 2.63941F, 2.63941F));
                AddDisplayRule("JellyfishBody", "Hull2", new Vector3(-0.34525F, -2.01444F, -0.95515F), new Vector3(344.4426F, 182.5215F, 185.5433F), new Vector3(3.15894F, 3.09263F, 3.15894F));
                AddDisplayRule("BeetleBody", "Head", new Vector3(0.01635F, 0.78858F, 0.44616F), new Vector3(333.6425F, 178.0887F, 357.6602F), new Vector3(2.17542F, 2.17542F, 1.86206F));
                AddDisplayRule("LemurianBody", "Chest", new Vector3(0F, 4.57347F, 0.80123F), new Vector3(40.95168F, 0F, 0F), new Vector3(15.38386F, 11.43963F, 13.17726F));
                AddDisplayRule("HermitCrabBody", "Base", new Vector3(0.00003F, 2.29478F, -0.0535F), new Vector3(0F, 0F, 0F), new Vector3(1.75963F, 1.75963F, 1.75963F));
                AddDisplayRule("ImpBody", "Neck", new Vector3(0F, 0.3394F, 0.12532F), new Vector3(348.5848F, 180F, 0F), new Vector3(1.23525F, 1.23525F, 1.23525F));
                AddDisplayRule("VultureBody", "Chest", new Vector3(-0.00003F, -1.0526F, -7.15841F), new Vector3(278.3651F, 180F, 180F), new Vector3(19.74622F, 19.74622F, 19.74622F));
                AddDisplayRule("RoboBallMiniBody", "ROOT", new Vector3(0F, 1.17958F, -0.02169F), new Vector3(0F, 0F, 0F), new Vector3(2.64201F, 2.64201F, 2.38566F));
                AddDisplayRule("MiniMushroomBody", "Head", new Vector3(-0.63297F, -0.07522F, 0F), new Vector3(84.28546F, 90.00004F, 180F), new Vector3(3.53148F, 3.53148F, 3.53148F));
                AddDisplayRule("BellBody", "Chain", new Vector3(0F, -1.12773F, 0F), new Vector3(0F, 236.2126F, 180F), new Vector3(5.79201F, 5.79201F, 5.79201F));
                AddDisplayRule("BeetleGuardBody", "Chest", new Vector3(0F, 4.22376F, -3.5825F), new Vector3(299.4801F, 0F, 1.8916F), new Vector3(7.14403F, 7.14403F, 7.14403F));
                AddDisplayRule("BisonBody", "Chest", new Vector3(0F, -0.42551F, 1.17086F), new Vector3(59.63197F, 180F, 180F), new Vector3(3.16017F, 3.88499F, 3.88499F));
                AddDisplayRule("GolemBody", "Head", new Vector3(0F, 1.52261F, 0.1374F), new Vector3(0F, 0F, 0F), new Vector3(4.09795F, 4.69594F, 3.32573F));
                AddDisplayRule("ParentBody", "Head", new Vector3(-89.90115F, 184.8263F, -0.1825F), new Vector3(317.6959F, 88.84123F, 0.77998F), new Vector3(348.7296F, 348.7296F, 348.7296F));
                AddDisplayRule("ClayBruiserBody", "Head", new Vector3(-0.00376F, 0.95324F, 0.22792F), new Vector3(351.5047F, 3.68439F, 359.9409F), new Vector3(2.08528F, 2.08528F, 2.08528F));
                AddDisplayRule("GreaterWispBody", "MaskBase", new Vector3(0F, -1.58687F, 0.18614F), new Vector3(-0.00001F, 180F, 180F), new Vector3(2.40124F, 2.40124F, 2.40124F));
                AddDisplayRule("LemurianBruiserBody", "Chest", new Vector3(0.01801F, 4.57732F, 3.43275F), new Vector3(310.846F, 180.7861F, 358.9608F), new Vector3(16.55212F, 16.55212F, 16.55212F));
                AddDisplayRule("NullifierBody", "Muzzle", new Vector3(0F, 1.39092F, 1.55025F), new Vector3(32.39762F, 0F, 0F), new Vector3(6.81418F, 6.81418F, 6.81418F));
                
                AddDisplayRule("BeetleQueen2Body", "Head", new Vector3(0.04308F, 5.08368F, 1.99988F), new Vector3(349.2091F, 186.4669F, 358.2756F), new Vector3(10.86423F, 10.86423F, 10.86423F));
                AddDisplayRule("ClayBossBody", "PotLidTop", new Vector3(0F, 1.33203F, 1.76139F), new Vector3(0F, 0F, 0F), new Vector3(5.25578F, 5.25578F, 5.25578F));
                AddDisplayRule("TitanBody", "Chest", new Vector3(0.34598F, 12.58396F, 1.03366F), new Vector3(0F, 0F, 0F), new Vector3(15.04987F, 11.42391F, 13.57924F));
                AddDisplayRule("TitanGoldBody", "Chest", new Vector3(0.34598F, 12.58396F, 1.03366F), new Vector3(0F, 0F, 0F), new Vector3(15.04987F, 11.42391F, 13.57924F));
                AddDisplayRule("VagrantBody", "Hull", new Vector3(0F, 2.21966F, 0F), new Vector3(0F, 0F, 0F), new Vector3(4.12204F, 4.12204F, 4.00566F));
                string[] worms = new string[]
                {
                    "MagmaWormBody",
                    "ElectricWormBody"
                };
                foreach (string worm in worms)
                {
                    AddDisplayRule(worm, "Head", new Vector3(-0.05169F, 1.24925F, -0.39707F), new Vector3(358.9208F, 180.0001F, 352.0788F), new Vector3(3.97806F, 3.97806F, 3.97806F));
                }
                AddDisplayRule("RoboBallBossBody", "Center", new Vector3(0F, 1.25704F, -0.05411F), new Vector3(0F, 0F, 0F), new Vector3(3.00215F, 3.00215F, 3.00215F));
                AddDisplayRule("SuperRoboBallBossBody", "Center", new Vector3(0F, 1.25704F, -0.05411F), new Vector3(0F, 0F, 0F), new Vector3(3.00215F, 3.00215F, 3.00215F));
                AddDisplayRule("GravekeeperBody", "Head", new Vector3(0F, 3.37295F, -1.70155F), new Vector3(342.9781F, 0F, 0F), new Vector3(10.91438F, 10.91438F, 10.91438F));
                AddDisplayRule("ImpBossBody", "Neck", new Vector3(0.16716F, 1.88254F, 0.14107F), new Vector3(5.77289F, 179.8548F, 358.5572F), new Vector3(5.69394F, 5.69394F, 5.69394F));
                AddDisplayRule("GrandParentBody", "Head", new Vector3(0F, 17.52644F, 1.14229F), new Vector3(0F, 0F, 0F), new Vector3(25.44026F, 25.44026F, 25.44026F));
                AddDisplayRule("ScavBody", "Head", new Vector3(-0.57235F, -8.07841F, -18.04656F), new Vector3(288.6726F, 180F, 180F), new Vector3(27.58408F, 27.58408F, 27.58408F));
            };

            if (Main.aspectAbilitiesEnabled) AspectAbilitiesSupport();
        }

        public override void AfterContentPackLoaded()
        {
            base.AfterContentPackLoaded();
            equipmentDef.passiveBuffDef = EliteVarietyContent.Buffs.AffixImpPlane;
        }

        public override void AspectAbilitiesSupport()
        {
            GameObject targetFinderVisualizerPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/WoodSpriteIndicator"), Main.TokenPrefix + "ImpPlaneAspectAbilityIndicator", false);
            Object.Destroy(targetFinderVisualizerPrefab.GetComponentInChildren<Rewired.ComponentControls.Effects.RotateAroundAxis>());
            targetFinderVisualizerPrefab.GetComponentInChildren<SpriteRenderer>().sprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/EliteVariety/Elites/ImpPlane/texImpPlaneAspectAbilityIndicator.png");
            targetFinderVisualizerPrefab.GetComponentInChildren<SpriteRenderer>().color = new Color32(230, 0, 60, 255);
            targetFinderVisualizerPrefab.GetComponentInChildren<SpriteRenderer>().transform.rotation = Quaternion.identity;
            targetFinderVisualizerPrefab.GetComponentInChildren<TMPro.TextMeshPro>().color = new Color32(230, 0, 60, 255);
            while (targetFinderVisualizerPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.length > 0) targetFinderVisualizerPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.RemoveKey(0);
            targetFinderVisualizerPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(0f, 2f);
            targetFinderVisualizerPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(0.5f, 1f);
            targetFinderVisualizerPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(1f, 1f);

            deployableSlot = DeployableAPI.RegisterDeployableSlot(GetImpPlaneDeployableSameSlotLimit);

            UseTargetFinder(TargetFinderType.Enemies, targetFinderVisualizerPrefab);
            AspectAbilities.AspectAbilitiesPlugin.RegisterAspectAbility(new AspectAbilities.AspectAbility
            {
                equipmentDef = equipmentDef,
                onUseOverride = (equipmentSlot) =>
                {
                    if (equipmentSlot.characterBody && equipmentSlot.characterBody.master)
                    {
                        int deployableCount = equipmentSlot.characterBody.master.GetDeployableCount(deployableSlot);
                        int deployableSameSlotLimit = equipmentSlot.characterBody.master.GetDeployableSameSlotLimit(deployableSlot);
                        if (deployableCount < deployableSameSlotLimit) {
                        MysticsRisky2UtilsEquipmentTarget targetInfo = equipmentSlot.GetComponent<MysticsRisky2UtilsEquipmentTarget>();
                            if (targetInfo && targetInfo.obj)
                            {
                                CharacterMaster master = equipmentSlot.characterBody.master;
                                if (master)
                                {
                                    HurtBox targetHB = targetInfo.obj.GetComponent<CharacterBody>().mainHurtBox;
                                    if (targetHB)
                                    {
                                        DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest((SpawnCard)Resources.Load("SpawnCards/CharacterSpawnCards/cscImpBoss"), new DirectorPlacementRule
                                        {
                                            placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                                            spawnOnTarget = targetHB.transform
                                        }, RoR2Application.rng)
                                        {
                                            summonerBodyObject = equipmentSlot.characterBody.gameObject
                                        };
                                        directorSpawnRequest.onSpawnedServer += (spawnResult) =>
                                        {
                                            GameObject summonedMasterObject = spawnResult.spawnedInstance;
                                            if (summonedMasterObject)
                                            {
                                                CharacterMaster summonedMaster = summonedMasterObject.GetComponent<CharacterMaster>();
                                                summonedMaster.inventory.GiveItem(RoR2Content.Items.HealthDecay, 20);
                                                summonedMaster.inventory.GiveItem(RoR2Content.Items.AlienHead, 3);
                                                foreach (RoR2.CharacterAI.BaseAI ai in summonedMaster.aiComponents)
                                                {
                                                    ai.currentEnemy.gameObject = targetHB.healthComponent.gameObject;
                                                    ai.currentEnemy.bestHurtBox = targetHB;
                                                }

                                                Deployable deployable = summonedMasterObject.AddComponent<Deployable>();
                                                deployable.onUndeploy = (deployable.onUndeploy ?? new UnityEvent());
                                                if (summonedMaster)
                                                {
                                                    deployable.onUndeploy.AddListener(new UnityAction(summonedMaster.TrueKill));
                                                }
                                                else
                                                {
                                                    deployable.onUndeploy.AddListener(delegate ()
                                                    {
                                                        UnityEngine.Object.Destroy(summonedMasterObject);
                                                    });
                                                }

                                                equipmentSlot.characterBody.master.AddDeployable(deployable, deployableSlot);
                                            }
                                        };
                                        DirectorCore.instance.TrySpawnObject(directorSpawnRequest);

                                        targetInfo.Invalidate();
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    return false;
                }
            });
        }

        public static int GetImpPlaneDeployableSameSlotLimit(CharacterMaster self, int deployableCountMultiplier)
        {
            return 1 * deployableCountMultiplier;
        }
    }
}
