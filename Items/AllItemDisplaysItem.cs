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
    public class AllItemDisplaysItem : BaseItem
    {
        public override void PreLoad()
        {
            itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = "AllItemDisplaysItem";
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

        public override void OnLoad()
        {
            base.OnLoad();

            onSetupIDRS += () => {
                AddDisplayRule("BeetleBody", "Head", GetEliteFollowerPrefab("Armored"), new Vector3(0F, 0.34403F, 0.37159F), new Vector3(0F, 90F, 112.8398F), new Vector3(0.25041F, 0.25041F, 0.25041F));
                AddDisplayRule("BeetleBody", "Chest", GetEliteFollowerPrefab("Armored"), new Vector3(0F, 0.19652F, -0.61841F), new Vector3(0F, 90F, 2.51592F), new Vector3(0.25041F, 0.25041F, 0.25041F));
                AddDisplayRule("BeetleBody", "Chest", GetEliteFollowerPrefab("Buffing"), new Vector3(0F, 0.13316F, -0.65895F), new Vector3(0F, 270F, 37.27843F), new Vector3(0.80709F, 0.80709F, 0.80709F));
                AddDisplayRule("BeetleBody", "Head", GetEliteFollowerPrefab("Pillaging"), new Vector3(0F, 0.60674F, 0.47416F), new Vector3(27.76727F, 0F, 0F), new Vector3(0.48181F, 0.48181F, 0.48181F));
                AddDisplayRule("BeetleBody", "Head", GetEliteFollowerPrefab("Sandstorm"), new Vector3(0.00703F, 0.43691F, 0.35467F), new Vector3(28.91702F, 319.116F, 341.4145F), new Vector3(0.62749F, 0.62749F, 0.62749F));
                AddDisplayRule("BeetleBody", "Head", GetEliteFollowerPrefab("Tinkerer"), new Vector3(0.08712F, 0.48435F, -0.71049F), new Vector3(333.6425F, 178.0887F, 357.6602F), new Vector3(0.43294F, 0.43294F, 0.43294F));
                AddDisplayRule("BeetleBody", "Head", GetEliteFollowerPrefab("ImpPlane"), new Vector3(0.01635F, 0.78858F, 0.44616F), new Vector3(333.6425F, 178.0887F, 357.6602F), new Vector3(2.17542F, 2.17542F, 1.86206F));
            };
        }

        public static GameObject GetEliteFollowerPrefab(string eliteName)
        {
            return Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Elites/" + eliteName + "/FollowerModel.prefab");
        }
    }
}
