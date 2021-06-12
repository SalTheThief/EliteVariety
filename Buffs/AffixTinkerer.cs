using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using MysticsRisky2Utils;
using MysticsRisky2Utils.MonoBehaviours;
using R2API.Networking.Interfaces;
using R2API.Networking;
using System.Collections.Generic;
using RoR2.Orbs;
using System.Linq;

namespace EliteVariety.Buffs
{
    public class AffixTinkerer : BaseBuff
    {
        public static DeployableSlot deployableSlot = (DeployableSlot)(-19085); // this will break if any other mod uses the same slot index. consider making a custom MysticsRisky2Utils deployable system

        public override Sprite LoadSprite(string assetName)
        {
            return Main.AssetBundle.LoadAsset<Sprite>("Assets/EliteVariety/Elites/Tinkerer/BuffIcon.png");
        }

        public override void OnLoad()
        {
            base.OnLoad();
            buffDef.name = "AffixTinkerer";
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            GenericGameEvents.OnHitEnemy += GenericGameEvents_OnHitEnemy;

            NetworkingAPI.RegisterMessageType<EliteVarietyAffixTinkererBehavior.EliteVarietyAffixTinkererRecipientBehavior.SyncDroneStatBonus>();

            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
        }

        public override void AfterContentPackLoaded()
        {
            base.AfterContentPackLoaded();
            buffDef.eliteDef = EliteVarietyContent.Elites.Tinkerer;
        }

        public void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (NetworkServer.active)
            {
                EliteVarietyAffixTinkererBehavior component = self.AddItemBehavior<EliteVarietyAffixTinkererBehavior>(self.HasBuff(buffDef) ? 1 : 0);
                if (component) component.RecalculateDroneStatBonus();
            }
        }

        public class EliteVarietyAffixTinkererBehavior : CharacterBody.ItemBehavior
        {
            private List<CharacterMaster> _droneMasters;
            public List<CharacterMaster> droneMasters {
                get
                {
                    _droneMasters.RemoveAll(x => !x);
                    return _droneMasters;
                }
                set
                {
                    _droneMasters = value;
                }
            }
            public DeployableMinionSpawner droneSpawner;
            public int droneStatBonus = 0;
            public Dictionary<Inventory, List<StolenItemInfo>> stealDictionary;
            public List<ItemTransferOrb> orbsInFlight;

            public class StolenItemInfo
            {
                public ItemIndex itemIndex;
                public int count = 0;
                public NetworkIdentity ownerBodyNetworkIdentity;
            }

            public void Start()
            {
                stealDictionary = new Dictionary<Inventory, List<StolenItemInfo>>();
                orbsInFlight = new List<ItemTransferOrb>();
                droneMasters = new List<CharacterMaster>();
            }

            public void FixedUpdate()
            {
                if (NetworkServer.active)
                {
                    if (droneSpawner == null && isActiveAndEnabled)
                    {
                        droneSpawner = new DeployableMinionSpawner(body.master, deployableSlot, new Xoroshiro128Plus(Run.instance.seed ^ (ulong)GetInstanceID()))
                        {
                            respawnInterval = 60f,
                            minSpawnDistance = 10f,
                            maxSpawnDistance = 25f,
                            spawnCard = MysticsRisky2Utils.BaseAssetTypes.BaseCharacterMaster.characterSpawnCards.Find(x => x.name == "EliteVariety_cscTinkererDrone")
                        };
                        droneSpawner.onMinionSpawnedServer += OnMinionSpawnedServer;
                    }
                }
            }

            public void RecalculateDroneStatBonus()
            {
                int oldDroneStatBonus = droneStatBonus;
                droneStatBonus = 0;
                Inventory myInventory = body.inventory;
                if (myInventory)
                {
                    foreach (ItemIndex itemIndex in ItemCatalog.allItems)
                    {
                        ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
                        if (itemDef.ContainsTag(ItemTag.Scrap))
                        {
                            int scrapPower = 0;
                            switch (itemDef.tier)
                            {
                                case ItemTier.Tier1:
                                    scrapPower = 1;
                                    break;
                                case ItemTier.Tier2:
                                case ItemTier.Boss:
                                    scrapPower = 2;
                                    break;
                                case ItemTier.Tier3:
                                    scrapPower = 3;
                                    break;
                            }
                            if (scrapPower != 0) droneStatBonus += myInventory.GetItemCount(itemDef) * scrapPower;
                        }
                    }
                }
                if (oldDroneStatBonus != droneStatBonus)
                {
                    foreach (CharacterMaster droneMaster in droneMasters)
                    {
                        EliteVarietyAffixTinkererRecipientBehavior component = droneMaster.GetComponent<EliteVarietyAffixTinkererRecipientBehavior>();
                        if (component)
                        {
                            component.droneStatBonus = droneStatBonus;
                        }
                    }
                }
            }

            public void OnMinionSpawnedServer(SpawnCard.SpawnResult obj)
            {
                GameObject spawnedInstance = obj.spawnedInstance;
                if (spawnedInstance)
                {
                    CharacterMaster droneMaster = spawnedInstance.GetComponent<CharacterMaster>();
                    droneMasters.Add(droneMaster);
                    if (droneMaster && droneMaster.inventory)
                    {
                        droneMaster.inventory.GiveItem(EliteVarietyContent.Items.TinkererDroneStatBonus);
                    }
                }
            }

            public class EliteVarietyAffixTinkererRecipientBehavior : MonoBehaviour
            {
                private int _droneStatBonus = 0;
                public int droneStatBonus
                {
                    get
                    {
                        return _droneStatBonus;
                    }
                    set
                    {
                        if (_droneStatBonus != value)
                        {
                            _droneStatBonus = value;
                            if (NetworkServer.active) new SyncDroneStatBonus(gameObject.GetComponent<NetworkIdentity>().netId, value).Send(NetworkDestination.Clients);
                        }
                    }
                }

                public class SyncDroneStatBonus : INetMessage
                {
                    NetworkInstanceId objID;
                    int droneStatBonus;

                    public SyncDroneStatBonus()
                    {
                    }

                    public SyncDroneStatBonus(NetworkInstanceId objID, int droneStatBonus)
                    {
                        this.objID = objID;
                        this.droneStatBonus = droneStatBonus;
                    }

                    public void Deserialize(NetworkReader reader)
                    {
                        objID = reader.ReadNetworkId();
                        droneStatBonus = reader.ReadInt32();
                    }

                    public void OnReceived()
                    {
                        if (NetworkServer.active) return;
                        GameObject obj = Util.FindNetworkObject(objID);
                        if (obj)
                        {
                            EliteVarietyAffixTinkererRecipientBehavior component = obj.GetComponent<EliteVarietyAffixTinkererRecipientBehavior>();
                            if (component)
                            {
                                component.droneStatBonus = droneStatBonus;
                            }
                        }
                    }

                    public void Serialize(NetworkWriter writer)
                    {
                        writer.Write(objID);
                        writer.Write(droneStatBonus);
                    }
                }
            }

            public void OnDestroy()
            {
                orbsInFlight.ForEach(x => OrbManager.instance.ForceImmediateArrival(x));

                Vector3 emitPosition = transform.position;
                if (body) emitPosition = body.corePosition;

                foreach (KeyValuePair<Inventory, List<StolenItemInfo>> keyValuePair in stealDictionary)
                {
                    if (keyValuePair.Key)
                    {
                        foreach (StolenItemInfo stolenItemInfo in keyValuePair.Value)
                        {
                            ItemTransferOrb item = ItemTransferOrb.DispatchItemTransferOrb(emitPosition, keyValuePair.Key, stolenItemInfo.itemIndex, stolenItemInfo.count, null, stolenItemInfo.ownerBodyNetworkIdentity);
                        }
                    }
                }

                foreach (CharacterMaster droneMaster in droneMasters)
                {
                    droneMaster.TrueKill();
                }
            }

            public void StealFrom(Inventory inventory, Vector3 emitPosition, NetworkIdentity networkIdentity)
            {
                List<ItemIndex> itemsThatCanBeStolen = new List<ItemIndex>();
                foreach (ItemIndex itemIndex in ItemCatalog.allItems)
                {
                    ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
                    if (itemDef) {
                        if (itemDef.canRemove && itemDef.DoesNotContainTag(ItemTag.CannotSteal) && inventory.GetItemCount(itemIndex) > 0)
                        {
                            itemsThatCanBeStolen.Add(itemIndex);
                        }
                    }
                }
                if (itemsThatCanBeStolen.Count > 0)
                {
                    ItemIndex itemToSteal = RoR2Application.rng.NextElementUniform(itemsThatCanBeStolen);
                    ItemDef itemToStealDef = ItemCatalog.GetItemDef(itemToSteal);
                    int stealAmount = 1;
                    inventory.RemoveItem(itemToSteal, stealAmount);

                    ItemDef scrap = RoR2Content.Items.ScrapWhite;
                    switch (itemToStealDef.tier)
                    {
                        case ItemTier.Tier1:
                            scrap = RoR2Content.Items.ScrapWhite;
                            break;
                        case ItemTier.Tier2:
                            scrap = RoR2Content.Items.ScrapGreen;
                            break;
                        case ItemTier.Boss:
                            scrap = RoR2Content.Items.ScrapYellow;
                            break;
                        case ItemTier.Tier3:
                            scrap = RoR2Content.Items.ScrapRed;
                            break;
                    }
                    ItemIndex scrapIndex = scrap.itemIndex;

                    if (!stealDictionary.ContainsKey(inventory)) stealDictionary.Add(inventory, new List<StolenItemInfo>());
                    StolenItemInfo stolenItemInfo = stealDictionary[inventory].FirstOrDefault(x => x.itemIndex == itemToSteal);
                    if (stolenItemInfo == null)
                    {
                        stolenItemInfo = new StolenItemInfo
                        {
                            itemIndex = itemToSteal,
                            ownerBodyNetworkIdentity = networkIdentity
                        };
                        stealDictionary[inventory].Add(stolenItemInfo);
                    }
                    stolenItemInfo.count += stealAmount;

                    ItemTransferOrb item = ItemTransferOrb.DispatchItemTransferOrb(emitPosition, null, itemToSteal, stealAmount, (orb) =>
                    {
                        body.inventory.GiveItem(scrap, stealAmount);
                        orbsInFlight.Remove(orb);

                        foreach (CharacterMaster droneMaster in droneMasters)
                        {
                            CharacterBody droneBody = droneMaster.GetBody();
                            if (droneBody)
                            {
                                ItemTransferOrb item2 = ItemTransferOrb.DispatchItemTransferOrb(body.corePosition, null, scrapIndex, stealAmount, (orb2) =>
                                {
                                    orbsInFlight.Remove(orb2);
                                }, droneBody.networkIdentity);
                            }
                        }
                    }, body.networkIdentity);
                    orbsInFlight.Add(item);
                }
            }
        }

        private void GenericGameEvents_OnHitEnemy(DamageInfo damageInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo attackerInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo victimInfo)
        {
            if (damageInfo.procCoefficient > 0 && attackerInfo.body && attackerInfo.master && victimInfo.body && victimInfo.inventory && attackerInfo.body.HasBuff(buffDef) && Util.CheckRoll(40f * damageInfo.procCoefficient, attackerInfo.master))
            {
                EliteVarietyAffixTinkererBehavior component = attackerInfo.body.GetComponent<EliteVarietyAffixTinkererBehavior>();
                if (component) component.StealFrom(victimInfo.inventory, victimInfo.body.corePosition, victimInfo.body.networkIdentity);
            }
        }

        public int CharacterMaster_GetDeployableSameSlotLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            if (slot == deployableSlot)
            {
                int num = 1;
                if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef)) num *= 2;
                return 1 * num;
            }
            return orig(self, slot);
        }
    }
}
