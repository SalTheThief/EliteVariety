using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using MysticsRisky2Utils;

namespace EliteVariety.Items
{
    public class TinkererDroneStatBonus : BaseItem
    {
        public override void PreLoad()
        {
            itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = "TinkererDroneStatBonus";
            itemDef.canRemove = false;
            itemDef.hidden = true;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.CannotCopy,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            itemDef.tier = ItemTier.NoTier;
        }

        public float TinkererDroneStatBonusModifierTimes(MysticsRisky2UtilsPlugin.GenericCharacterInfo genericCharacterInfo)
        {
            if (genericCharacterInfo.inventory && genericCharacterInfo.master)
            {
                int itemCount = genericCharacterInfo.inventory.GetItemCount(itemDef);
                if (itemCount > 0)
                {
                    Buffs.AffixTinkerer.EliteVarietyAffixTinkererBehavior.EliteVarietyAffixTinkererRecipientBehavior component = genericCharacterInfo.master.GetComponent<Buffs.AffixTinkerer.EliteVarietyAffixTinkererBehavior.EliteVarietyAffixTinkererRecipientBehavior>();
                    if (component)
                    {
                        return component.droneStatBonus * itemCount;
                    }
                }
            }
            return 0f;
        }

        public override void OnLoad()
        {
            base.OnLoad();

            CharacterStats.armorModifiers.Add(new CharacterStats.FlatStatModifier {
                amount = 5f,
                times = (genericCharacterInfo) => TinkererDroneStatBonusModifierTimes(genericCharacterInfo)
            });
            CharacterStats.attackSpeedModifiers.Add(new CharacterStats.StatModifier
            {
                flat = 0.1f,
                times = (genericCharacterInfo) => TinkererDroneStatBonusModifierTimes(genericCharacterInfo)
            });
            CharacterStats.cooldownModifiers.Add(new CharacterStats.StatModifier
            {
                multiplier = 0.95f,
                times = (genericCharacterInfo) => TinkererDroneStatBonusModifierTimes(genericCharacterInfo)
            });
            CharacterStats.damageModifiers.Add(new CharacterStats.StatModifier
            {
                multiplier = 0.05f,
                times = (genericCharacterInfo) => TinkererDroneStatBonusModifierTimes(genericCharacterInfo)
            });
            CharacterStats.healthModifiers.Add(new CharacterStats.StatModifier
            {
                flat = 10,
                times = (genericCharacterInfo) => TinkererDroneStatBonusModifierTimes(genericCharacterInfo)
            });
            CharacterStats.moveSpeedModifiers.Add(new CharacterStats.StatModifier
            {
                multiplier = 0.1f,
                times = (genericCharacterInfo) => TinkererDroneStatBonusModifierTimes(genericCharacterInfo)
            });
            CharacterStats.regenModifiers.Add(new CharacterStats.FlatStatModifier
            {
                amount = 0.1f,
                times = (genericCharacterInfo) => TinkererDroneStatBonusModifierTimes(genericCharacterInfo)
            });
        }
    }
}
