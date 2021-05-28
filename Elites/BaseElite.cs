using RoR2;
using UnityEngine;

namespace EliteVariety.Elites
{
    public abstract class BaseElite : MysticsRisky2Utils.BaseAssetTypes.BaseElite
    {
        public override string TokenPrefix => Main.TokenPrefix;
        public override Texture LoadRecolorRamp(string assetName)
        {
            return Main.AssetBundle.LoadAsset<Texture>("Assets/EliteVariety/Elites/" + assetName + "/RecolorRamp.png");
        }
    }
}
