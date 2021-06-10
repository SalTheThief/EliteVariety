using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

namespace EliteVariety.Items
{
    public abstract class BaseItem : MysticsRisky2Utils.BaseAssetTypes.BaseItem
    {
        public override GameObject LoadModel(string assetName)
        {
            return Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Items/" + assetName + "/Model.prefab");
        }
        public override bool FollowerModelExists(string assetName)
        {
            return Main.AssetBundle.Contains("Assets/EliteVariety/Items/" + assetName + "/FollowerModel.prefab");
        }
        public override GameObject LoadFollowerModel(string assetName)
        {
            return Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Items/" + assetName + "/FollowerModel.prefab");
        }
        public override Sprite LoadIconSprite(string assetName)
        {
            return Main.AssetBundle.LoadAsset<Sprite>("Assets/EliteVariety/Items/" + assetName + "/Icon.png");
        }
        public override string TokenPrefix => Main.TokenPrefix;

        public override void SetUnlockable()
        {
            EliteVarietyContent.Resources.unlockableDefs.Add(GetUnlockableDef());
        }
    }
}
