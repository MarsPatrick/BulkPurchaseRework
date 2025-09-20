using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Linq;

namespace BulkPurchaseRework;

[BepInPlugin("com.MarsPatrick.BulkPurchaseRework", "BulkPurchaseRework", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    internal static Harmony Harmony;

    // Configuraciones públicas accesibles desde otros scripts
    public static ConfigEntry<int> ShelveThreshold { get; set; }
    public static ConfigEntry<int> StorageThreshold { get; set; }
    public static ConfigEntry<string> ProductBlacklist { get; set; }
    public static ConfigEntry<int> CurrentMode { get; set; }

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin com.MarsPatrick.BulkPurchaseRework is loaded!");

        // Cargar configuraciones
        ShelveThreshold = Config.Bind(
            "Thresholds",
            "ShelveThreshold",
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
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s, out var n) ? n : (int?)null)
            .Where(n => n.HasValue)
            .Select(n => n.Value)
            .Distinct()
            .OrderBy(n => n)
            .ToList();

        // Guardar de vuelta como string
        ProductBlacklist.Value = string.Join(",", numbers);
    }
}