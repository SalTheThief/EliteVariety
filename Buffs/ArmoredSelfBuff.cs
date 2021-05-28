using UnityEngine;

namespace EliteVariety.Buffs
{
    public class ArmoredSelfBuff : BaseBuff
    {
        public override Sprite LoadSprite(string assetName)
        {
            return Resources.Load<Sprite>("Textures/BuffIcons/texBuffGenericShield");
        }

        public override void OnLoad()
        {
            base.OnLoad();
            buffDef.name = "ArmoredSelfBuff";
            buffDef.canStack = false;
            buffDef.buffColor = new Color32(255, 209, 209, 255);
            AddArmorModifier(50f);

            MysticsRisky2Utils.Overlays.CreateOverlay(Main.AssetBundle.LoadAsset<Material>("Assets/EliteVariety/Elites/Armored/matEliteArmoredBuffOverlay.mat"), (characterModel) =>
            {
                return characterModel.body.HasBuff(buffDef);
            });
            MysticsRisky2Utils.Overlays.CreateOverlay(Main.AssetBundle.LoadAsset<Material>("Assets/EliteVariety/Elites/Armored/matEliteArmoredBuffOverlay2.mat"), (characterModel) =>
            {
                return characterModel.body.HasBuff(buffDef);
            });
        }
    }
}