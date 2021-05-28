using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using MysticsRisky2Utils;

namespace EliteVariety.Equipment
{
    public abstract class BaseEliteAffix : BaseEquipment
    {
        public override GameObject LoadModel(string assetName)
        {
            return Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Misc/GenericAffixPickup.prefab");
        }
        public override bool FollowerModelExists(string assetName)
        {
            return Main.AssetBundle.Contains("Assets/EliteVariety/Elites/" + assetName + "/FollowerModel.prefab");
        }
        public override GameObject LoadFollowerModel(string assetName)
        {
            return Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Elites/" + assetName + "/FollowerModel.prefab");
        }
        public override Sprite LoadIconSprite(string assetName)
        {
            return Main.AssetBundle.LoadAsset<Sprite>("Assets/EliteVariety/Elites/" + assetName + "/EquipmentIcon.png");
        }

        public override void PreLoad()
        {
            equipmentDef.canDrop = false;
            equipmentDef.enigmaCompatible = false;
        }

        public override void SetAssets(string eliteName)
        {
            base.SetAssets(eliteName);

            Material material = new Material(HopooShaderToMaterial.Standard.shader);
            HopooShaderToMaterial.Standard.DisableEverything(material);
            material.name = "mat" + equipmentDef.name + "Pickup";
            material.EnableKeyword("FORCE_SPEC");
            material.EnableKeyword("FRESNEL_EMISSION");
            material.SetFloat("_AOON", 0f);
            material.SetFloat("_BlueChannelBias", 0f);
            material.SetFloat("_BlueChannelSmoothness", 0f);
            material.SetFloat("_BumpScale", 1f);
            material.SetFloat("_ColorsOn", 0f);
            material.SetFloat("_Cull", 2f);
            material.SetFloat("_Cutoff", 0.5f);
            material.SetFloat("_DecalLayer", 0f);
            material.SetFloat("_Depth", 0.2f);
            material.SetFloat("_DetailNormalMapScale", 1f);
            material.SetFloat("_DitherOn", 0f);
            material.SetFloat("_DstBlend", 0f);
            material.SetFloat("_EliteBrightnessMax", 1f);
            material.SetFloat("_EliteBrightnessMin", 0f);
            material.SetFloat("_EliteIndex", 0f);
            material.SetFloat("_EmPower", 0f);
            material.SetFloat("_EnableCutout", 0f);
            material.SetFloat("_FEON", 1f);
            material.SetFloat("_Fade", 1f);
            material.SetFloat("_FadeBias", 0f);
            material.SetFloat("_FadeDistance", 0f);
            material.SetFloat("_FlowDiffuseStrength", 1f);
            material.SetFloat("_FlowEmissionStrength", 1f);
            material.SetFloat("_FlowHeightBias", 0f);
            material.SetFloat("_FlowHeightPower", 1f);
            material.SetFloat("_FlowMaskStrength", 0f);
            material.SetFloat("_FlowNormalStrength", 1f);
            material.SetFloat("_FlowSpeed", 1f);
            material.SetFloat("_FlowTextureScaleFactor", 1f);
            material.SetFloat("_FlowmapOn", 0f);
            material.SetFloat("_ForceSpecOn", 1f);
            material.SetFloat("_FresnelBoost", 20f);
            material.SetFloat("_FresnelPower", 4.11f);
            material.SetFloat("_GlossMapScale", 1f);
            material.SetFloat("_Glossiness", 0.5f);
            material.SetFloat("_GlossyReflections", 1f);
            material.SetFloat("_GreenChannelBias", 0f);
            material.SetFloat("_GreenChannelSmoothness", 0f);
            material.SetFloat("_LimbPrimeMask", 1f);
            material.SetFloat("_LimbRemovalOn", 0f);
            material.SetFloat("_Metallic", 0f);
            material.SetFloat("_Mode", 0f);
            material.SetFloat("_NormalStrength", 1f);
            material.SetFloat("_OcclusionStrength", 1f);
            material.SetFloat("_Parallax", 0.02f);
            material.SetFloat("_PrintBias", 0f);
            material.SetFloat("_PrintBoost", 1f);
            material.SetFloat("_PrintDirection", 0f);
            material.SetFloat("_PrintEmissionToAlbedoLerp", 0f);
            material.SetFloat("_PrintOn", 0f);
            material.SetFloat("_RampInfo", 0f);
            material.SetFloat("_SliceAlphaDepth", 0.1f);
            material.SetFloat("_SliceBandHeight", 1f);
            material.SetFloat("_SliceHeight", 5f);
            material.SetFloat("_Smoothness", 0f);
            material.SetFloat("_SmoothnessTextureChannel", 0f);
            material.SetFloat("_SpecularExponent", 9.26f);
            material.SetFloat("_SpecularHighlights", 1f);
            material.SetFloat("_SpecularStrength", 0.258f);
            material.SetFloat("_SplatmapOn", 0f);
            material.SetFloat("_SplatmapTileScale", 1f);
            material.SetFloat("_SrcBlend", 1f);
            material.SetFloat("_UVSec", 0f);
            material.SetFloat("_ZWrite", 1f);
            foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())
            {
                renderer.materials = new Material[]
                {
                    material
                };
            }
        }

        public void AdjustElitePickupMaterial(Color color, float fresnelPower, bool smoothFresnelRamp = true)
        {
            Material material = model.GetComponentInChildren<Renderer>().sharedMaterial;
            material.SetColor("_Color", color);
            material.SetFloat("_FresnelPower", fresnelPower);
            material.SetTexture("_FresnelRamp", Main.AssetBundle.LoadAsset<Texture>("Assets/EliteVariety/Misc/" + (smoothFresnelRamp ? "texElitePickupFresnelRampSmooth.png" : "texElitePickupFresnelRamp.png")));
        }

        public void AdjustElitePickupMaterial(Color color, float fresnelPower, Texture customFresnelRamp)
        {
            Material material = model.GetComponentInChildren<Renderer>().sharedMaterial;
            material.SetColor("_Color", color);
            material.SetFloat("_FresnelPower", fresnelPower);
            material.SetTexture("_FresnelRamp", customFresnelRamp);
        }
    }
}
