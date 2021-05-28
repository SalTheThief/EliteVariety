using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

namespace EliteVariety.Equipment
{
    public abstract class BaseEquipment : MysticsRisky2Utils.BaseAssetTypes.BaseEquipment
    {
        public override GameObject LoadModel(string assetName)
        {
            return Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Equipment/" + assetName + "/Model.prefab");
        }
        public override bool FollowerModelExists(string assetName)
        {
            return Main.AssetBundle.Contains("Assets/EliteVariety/Equipment/" + assetName + "/FollowerModel.prefab");
        }
        public override GameObject LoadFollowerModel(string assetName)
        {
            return Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Equipment/" + assetName + "/FollowerModel.prefab");
        }
        public override Sprite LoadIconSprite(string assetName)
        {
            return Main.AssetBundle.LoadAsset<Sprite>("Assets/EliteVariety/Equipment/" + assetName + "/Icon.png");
        }
        public override string TokenPrefix => Main.TokenPrefix;

        public override void SetUnlockable()
        {
            EliteVarietyContent.Resources.unlockableDefs.Add(GetUnlockableDef());
        }

        public abstract void AspectAbilitiesSupport();
    }
}
