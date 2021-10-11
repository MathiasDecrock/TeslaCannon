using BepInEx;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;

namespace TeslaCannon
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class TeslaCannon : BaseUnityPlugin
    {
        public const string PluginGUID = "com.zarboz.TeslaCannon";
        public const string PluginName = "TeslaCannon";
        public const string PluginVersion = "0.0.1";
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        private void Awake()
        {
            LoadAssets();
        }

        private void LoadAssets()
        {
            AssetBundle assetBundle =
                AssetUtils.LoadAssetBundleFromResources("teslacoil", typeof(TeslaCannon).Assembly);
            var cannon = assetBundle.LoadAsset<GameObject>("TeslaCoil");
            var CP = new CustomPiece(cannon, false, new PieceConfig
            {
                AllowedInDungeons = false,
                Category = "Tesla Coil",
                CraftingStation = "piece_workbench",
                PieceTable = "Hammer",
                Name = "Tesla Cannon",
                Description = "A handy zapper for those deathsquitos ;) ",
                Requirements = new RequirementConfig[]
                {
                    new RequirementConfig { Amount = 1, Item = "Wood", Recover = false }
                }
            });
            PieceManager.Instance.AddPiece(CP);
        }
        
    }
}