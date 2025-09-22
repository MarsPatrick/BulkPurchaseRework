using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulkPurchaseRework;

[BepInPlugin("com.MarsPatrick.BulkPurchaseRework", "BulkPurchaseRework", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    internal static Harmony Harmony;

    // Configuraciones públicas accesibles desde otros scripts
    public static ConfigEntry<int> ShelfThreshold { get; set; }
    public static ConfigEntry<int> StorageThreshold { get; set; }
    public static ConfigEntry<string> ProductBlacklist { get; set; }
    public static ConfigEntry<int> CurrentMode { get; set; }
    public static int specialmode = 1;
    public static Dictionary<(int, int), string> modeMappings = [];
    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin com.MarsPatrick.BulkPurchaseRework is loaded!");

        // Cargar configuraciones
        ShelfThreshold = Config.Bind(
            "Thresholds",
            "ShelfThreshold",
            40,
            "Cantidad mínima de unidades en Total antes de reordenar."
        );

        StorageThreshold = Config.Bind(
            "Thresholds",
            "StorageThreshold",
            40,
            "Cantidad mínima de unidades en Bodega antes de reordenar."
        );

        ProductBlacklist = Config.Bind(
            "Blacklist",
            "Item Blacklist",
            "",
            "Lista de IDs de productos en lista negra para no comprar"
        );

        CurrentMode = Config.Bind(
            "Modes",
            "CurrentFillMode",
            1,
            "Modo de rellenar."
        );

        NormalizeBlacklist();
        FillDictionary();
        Harmony = new Harmony("com.MarsPatrick.bulkpurchaserework");
        Harmony.PatchAll();
    }

    private void NormalizeBlacklist()
    {
        if (string.IsNullOrWhiteSpace(ProductBlacklist.Value))
        {
            ProductBlacklist.Value = "";
            return;
        }

        // Parsear a lista de enteros
        var numbers = ProductBlacklist.Value
            .Split([','], StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s, out var n) ? n : (int?)null)
            .Where(n => n.HasValue)
            .Select(n => n.Value)
            .Distinct()
            .OrderBy(n => n)
            .ToList();

        ProductBlacklist.Value = string.Join(",", numbers);
    }

    private void FillDictionary()
    {
        modeMappings.Add((1, 1), "Shelf Fill Threshold");
        modeMappings.Add((1, 2), "Shelf 1Box Threshold");
        modeMappings.Add((1, 3), "Storage Fill Threshold");
        modeMappings.Add((1, 4), "Storage 1Box Threshold");
        modeMappings.Add((1, 5), "Mixed Fill Threshold");
        modeMappings.Add((1, 6), "Mixed 1Box Threshold");
        modeMappings.Add((2, 1), "Fill Shelves w/o Storage");
        modeMappings.Add((2, 2), "Fill Shelves w/ Storage");
        modeMappings.Add((3, 1), "Boxes Storage");
        modeMappings.Add((3, 2), "Item Threshold");
    }
}