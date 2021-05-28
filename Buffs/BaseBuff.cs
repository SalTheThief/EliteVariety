using UnityEngine;
namespace EliteVariety.Buffs
{
    public abstract class BaseBuff : MysticsRisky2Utils.BaseAssetTypes.BaseBuff
    {
        public override string TokenPrefix => Main.TokenPrefix;
        public override Sprite LoadSprite(string assetName)
        {
            return Main.AssetBundle.LoadAsset<Sprite>("Assets/EliteVariety/Buffs/" + assetName + ".png");
        }
    }
}