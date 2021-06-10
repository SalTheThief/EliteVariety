using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Audio;
using RoR2.Orbs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace EliteVariety.Buffs
{
    public class AffixPillaging : BaseBuff
    {
		public static GameObject moneyTransferOrbEffect;

        public override Sprite LoadSprite(string assetName)
        {
            return Main.AssetBundle.LoadAsset<Sprite>("Assets/EliteVariety/Elites/Pillaging/BuffIcon.png");
        }

		public override void OnLoad()
		{
			base.OnLoad();
			buffDef.name = "AffixPillaging";
			On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            GenericGameEvents.OnTakeDamage += GenericGameEvents_OnTakeDamage;

			moneyTransferOrbEffect = Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Elites/Pillaging/PillageOrbEffect.prefab");
			EffectComponent effectComponent = moneyTransferOrbEffect.AddComponent<EffectComponent>();
			effectComponent.positionAtReferencedTransform = false;
			effectComponent.parentToReferencedTransform = false;
			effectComponent.applyScale = true;
			VFXAttributes vfxAttributes = moneyTransferOrbEffect.AddComponent<VFXAttributes>();
			vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
			vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
			OrbEffect orbEffect = moneyTransferOrbEffect.AddComponent<OrbEffect>();
			orbEffect.startVelocity1 = new Vector3(-7f, 7f, -7f);
			orbEffect.startVelocity2 = new Vector3(10f, 10f, 10f);
			orbEffect.endVelocity1 = new Vector3(-4f, 4f, -4f);
			orbEffect.endVelocity2 = new Vector3(1f, 1f, 1f);
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
			DestroyOnTimer destroyOnTimer = moneyTransferOrbEffect.transform.Find("TrailParent/Trail").gameObject.AddComponent<DestroyOnTimer>();
			destroyOnTimer.duration = 0.4f;
			destroyOnTimer.enabled = false;
			MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsOrbEffectOnArrivalDefaults onArrivalDefaults = moneyTransferOrbEffect.AddComponent<MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsOrbEffectOnArrivalDefaults>();
			onArrivalDefaults.orbEffect = orbEffect;
			onArrivalDefaults.transformsToUnparentChildren = new Transform[] {
				moneyTransferOrbEffect.transform.Find("TrailParent")
			};
			onArrivalDefaults.componentsToEnable = new MonoBehaviour[]
			{
				destroyOnTimer
			};

			EliteVarietyContent.Resources.effectPrefabs.Add(moneyTransferOrbEffect);

            On.RoR2.DeathRewards.OnKilledServer += DeathRewards_OnKilledServer;
		}

		public override void AfterContentPackLoaded()
		{
			base.AfterContentPackLoaded();
			buffDef.eliteDef = EliteVarietyContent.Elites.Pillaging;
		}

		public void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (NetworkServer.active) self.AddItemBehavior<EliteVarietyAffixPillagingBehavior>(self.HasBuff(buffDef) ? 1 : 0);
        }

        public class EliteVarietyAffixPillagingBehavior : CharacterBody.ItemBehavior
        {
            public float allyGoldStealRadius = 25f;
            public float allyGoldStealInterval = 1f;
            public float allyGoldStealStopwatch = 0f;
            public uint allyGoldStealBaseAmount = 1u;
            public int allyGoldStealMaxAllies = 5;
			public EliteVarietyAffixPillagingDeathRewardsModifier deathRewardsModifier;

			public void Start()
            {
				deathRewardsModifier = GetComponent<EliteVarietyAffixPillagingDeathRewardsModifier>();
				if (!deathRewardsModifier) deathRewardsModifier = gameObject.AddComponent<EliteVarietyAffixPillagingDeathRewardsModifier>();
			}

            public void FixedUpdate()
            {
                allyGoldStealStopwatch += Time.fixedDeltaTime;
                if (allyGoldStealStopwatch >= allyGoldStealInterval)
                {
                    allyGoldStealStopwatch = 0f;

					uint stealAmount = (uint)(allyGoldStealBaseAmount * Run.instance.difficultyCoefficient);

					List<Collider> colliders = Physics.OverlapSphere(body.corePosition, allyGoldStealRadius, LayerIndex.entityPrecise.mask).ToList();
					int alliesToStealFrom = allyGoldStealMaxAllies;
					while (alliesToStealFrom > 0 && colliders.Count > 0)
                    {
						Collider collider = RoR2Application.rng.NextElementUniform(colliders);
						colliders.Remove(collider);
						CharacterBody colliderBody = Util.HurtBoxColliderToBody(collider);
						if (colliderBody && colliderBody != body && TeamComponent.GetObjectTeam(colliderBody.gameObject) == TeamComponent.GetObjectTeam(gameObject) && !colliderBody.HasBuff(EliteVarietyContent.Buffs.AffixPillaging))
						{
							HealthComponent healthComponent = colliderBody.healthComponent;
							if (healthComponent && healthComponent.alive)
							{
								StealGold(colliderBody, stealAmount, true, false);
								alliesToStealFrom--;
							}
						}
					}
                }
            }

            public bool StealGold(CharacterBody victim, uint stealAmount, bool dontGainGoldIfVictimHasNone, bool keepSameDeathRewardsOnVictim)
            {
				if (!body.master) return false;

				CharacterMaster victimMaster = victim.master;

				if (victimMaster)
				{
					if (victimMaster.money > 0)
					{
						uint goldToRemove = (uint)Mathf.Min(stealAmount, victimMaster.money);
						victimMaster.money -= goldToRemove;

						EliteVarietyAffixPillagingDeathRewardsModifier victimDeathRewardsModifier = victim.GetComponent<EliteVarietyAffixPillagingDeathRewardsModifier>();
						if (!victimDeathRewardsModifier) victimDeathRewardsModifier = victim.gameObject.AddComponent<EliteVarietyAffixPillagingDeathRewardsModifier>();
						if (keepSameDeathRewardsOnVictim) victimDeathRewardsModifier.addGold += goldToRemove;
                    }
                    else
                    {
						if (dontGainGoldIfVictimHasNone) return false;
                    }

					MoneyTransferOrb orb = new MoneyTransferOrb();
					orb.masterToGiveMoneyTo = body.master;
					orb.money = stealAmount;
					orb.origin = victim.corePosition;
					orb.target = body.mainHurtBox;
					OrbManager.instance.AddOrb(orb);

					deathRewardsModifier.removeGold += stealAmount; // don't drop stolen gold on death

					return true;
				}

				return false;
            }
        }

		private void GenericGameEvents_OnTakeDamage(DamageReport damageReport)
		{
			if (damageReport.damageInfo.procCoefficient > 0 && damageReport.attackerBody && damageReport.victimBody && damageReport.victimMaster && damageReport.attackerBody.HasBuff(buffDef))
            {
				EliteVarietyAffixPillagingBehavior component = damageReport.attackerBody.GetComponent<EliteVarietyAffixPillagingBehavior>();
				if (component)
				{
					uint stealAmount = (uint)Mathf.Max(damageReport.damageDealt / damageReport.victimBody.healthComponent.fullCombinedHealth * damageReport.victimMaster.money, 1u);
					bool stolen = component.StealGold(damageReport.victimBody, stealAmount, false, true);
					if (stolen)
					{
						EffectManager.SimpleImpactEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/CoinImpact"), damageReport.victimBody.corePosition, Vector3.up, true);
					}
				}
			}
		}

		public class MoneyTransferOrb : Orb
		{
			public CharacterMaster masterToGiveMoneyTo;
			public uint money;

			public override void Begin()
			{
				duration = 1.5f;
				EffectData effectData = new EffectData
				{
					origin = origin,
					genericFloat = duration
				};
				effectData.SetHurtBoxReference(target);
				EffectManager.SpawnEffect(moneyTransferOrbEffect, effectData, true);
			}

			public override void OnArrival()
			{
				if (masterToGiveMoneyTo)
				{
					masterToGiveMoneyTo.GiveMoney(money);
				}
			}
		}

		public class EliteVarietyAffixPillagingDeathRewardsModifier : MonoBehaviour
		{
			public uint addGold = 0;
			public uint removeGold = 0;
			public CharacterMaster master;
			public uint moneyLastFrame = 0;

			public void Awake()
            {
				CharacterBody body = GetComponent<CharacterBody>();
				if (body) master = body.master;
            }

			public void FixedUpdate()
            {
				if (master && master.money < moneyLastFrame)
                {
					removeGold -= moneyLastFrame - master.money;
					if (removeGold < 0) removeGold = 0;
                }
				moneyLastFrame = master.money;
            }
		}

		private void DeathRewards_OnKilledServer(On.RoR2.DeathRewards.orig_OnKilledServer orig, DeathRewards self, DamageReport damageReport)
		{
			if (self.goldReward >= 0)
			{
				EliteVarietyAffixPillagingDeathRewardsModifier deathRewardsModifier = self.GetComponent<EliteVarietyAffixPillagingDeathRewardsModifier>();
				if (deathRewardsModifier)
				{
					uint goldChange = deathRewardsModifier.addGold - deathRewardsModifier.removeGold;
					self.goldReward += (uint)(goldChange > 0 ? goldChange : -Mathf.Min(-goldChange, self.goldReward));
				}
			}
			orig(self, damageReport);
		}
	}
}
