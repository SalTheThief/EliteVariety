using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using MysticsRisky2Utils;

namespace EliteVariety.Buffs
{
    public class AffixArmored : BaseBuff
    {
        public static Sprite barrierBarSprite;

        public override Sprite LoadSprite(string assetName)
        {
            return Main.AssetBundle.LoadAsset<Sprite>("Assets/EliteVariety/Elites/Armored/BuffIcon.png");
        }

        public override void OnLoad()
        {
            base.OnLoad();
            buffDef.name = "AffixArmored";
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            GenericGameEvents.OnHitEnemy += GenericGameEvents_OnHitEnemy;

            IL.RoR2.UI.HealthBar.UpdateBarInfos += HealthBar_UpdateBarInfos;

            barrierBarSprite = Main.AssetBundle.LoadAsset<Sprite>("Assets/EliteVariety/Elites/Armored/texEliteArmoredBarrierRecolor.png");
        }

        public override void AfterContentPackLoaded()
        {
            base.AfterContentPackLoaded();
            buffDef.eliteDef = EliteVarietyContent.Elites.Armored;
        }

        public void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (NetworkServer.active) self.AddItemBehavior<EliteVarietyAffixArmoredBehavior>(self.HasBuff(buffDef) ? 1 : 0);
        }

        public class EliteVarietyAffixArmoredBehavior : CharacterBody.ItemBehavior
        {
            public bool firstTimeBarrierGain = false;

            public void FixedUpdate()
            {
                if (!firstTimeBarrierGain && body.maxBarrier > 0)
                {
                    firstTimeBarrierGain = true;
                    body.healthComponent.AddBarrier(body.maxBarrier);
                }
            }
        }

        public void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (self.HasBuff(buffDef))
            {
                self.barrierDecayRate *= 0f;
            }
        }

        private void GenericGameEvents_OnHitEnemy(DamageInfo damageInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo attackerInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo victimInfo)
        {
            if (damageInfo.procCoefficient > 0 && attackerInfo.body && victimInfo.body && attackerInfo.healthComponent && attackerInfo.body.HasBuff(buffDef))
            {
                victimInfo.body.AddTimedBuff(EliteVarietyContent.Buffs.ArmoredHeavyStun, 1f * damageInfo.procCoefficient);
            }
        }

        private void HealthBar_UpdateBarInfos(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(
                MoveType.Before,
                x => x.MatchDup(),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<RoR2.UI.HealthBar>("style"),
                x => x.MatchLdflda(typeof(RoR2.UI.HealthBarStyle.BarStyle), "barrierBarStyle")
            ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<System.Action<RoR2.UI.HealthBar>>((healthBar) =>
                {
                    if (healthBar.source.body && healthBar.source.body.HasBuff(buffDef))
                    {
                        ref RoR2.UI.HealthBar.BarInfo barrierBarStyle = ref healthBar.barInfoCollection.barrierBarInfo;
                        barrierBarStyle.color = new Color32(255, 209, 209, 255);
                        barrierBarStyle.sprite = barrierBarSprite;
                    }
                });
            }
        }
    }
}
