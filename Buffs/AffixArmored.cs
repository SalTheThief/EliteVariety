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
        public override Sprite LoadSprite(string assetName)
        {
            return Main.AssetBundle.LoadAsset<Sprite>("Assets/EliteVariety/Elites/Armored/BuffIcon.png");
        }

        public override void OnLoad()
        {
            base.OnLoad();
            buffDef.name = "AffixArmored";
            buffDef.eliteDef = EliteVarietyContent.Elites.Armored;
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            GenericGameEvents.OnHitEnemy += GenericGameEvents_OnHitEnemy;
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
                self.barrierDecayRate *= 0.1f;
            }
        }

        private void GenericGameEvents_OnHitEnemy(DamageInfo damageInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo attackerInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo victimInfo)
        {
            if (damageInfo.procCoefficient > 0 && attackerInfo.body && victimInfo.body && attackerInfo.healthComponent && attackerInfo.body.HasBuff(buffDef))
            {
                victimInfo.body.AddTimedBuff(EliteVarietyContent.Buffs.ArmoredHeavyStun, 0.75f * damageInfo.procCoefficient);
            }
        }
    }
}
