using UnityEngine;
using MysticsRisky2Utils;
using MysticsRisky2Utils.MonoBehaviours;
using RoR2;
using UnityEngine.Rendering.PostProcessing;
using System.Linq;
using R2API;

namespace EliteVariety.Buffs
{
    public class ImpPlaneStare : BaseBuff
    {
        public override Sprite LoadSprite(string assetName)
        {
            return Resources.Load<Sprite>("Textures/BuffIcons/texBuffDeathMarkIcon");
        }

        public override void OnLoad()
        {
            base.OnLoad();
            buffDef.name = "ImpPlaneStare";
            buffDef.canStack = true;
            buffDef.isDebuff = true;
            buffDef.buffColor = new Color32(142, 27, 59, 255);

            GenericGameEvents.OnApplyDamageIncreaseModifiers += GenericGameEvents_OnApplyDamageIncreaseModifiers;
        }

        public void GenericGameEvents_OnApplyDamageIncreaseModifiers(DamageInfo damageInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo attackerInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo victimInfo, ref float damage)
        {
            if (victimInfo.body && victimInfo.body.HasBuff(buffDef))
            {
                damage *= 1f + 0.1f * victimInfo.body.GetBuffCount(buffDef);
            }
        }
    }
}
