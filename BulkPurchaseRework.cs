using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using UnityEngine;

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

        Harmony = new Harmony("com.MarsPatrick.bulkpurchaserework");
        Harmony.PatchAll();
    }
}