using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace BulkPurchaseRework;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    internal static Harmony Harmony;

    // Configuraciones públicas accesibles desde otros scripts
    public static ConfigEntry<int> StorageThreshold { get; set; }
    public static ConfigEntry<int> StorageBoxThreshold { get; set; }

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        // Cargar configuraciones
        StorageThreshold = Config.Bind(
            "Thresholds",
            "StorageThreshold",
            40,
            "Cantidad mínima de unidades en Total antes de reordenar."
        );

        StorageBoxThreshold = Config.Bind(
            "Thresholds",
            "StorageBoxThreshold",
            40,
            "Cantidad mínima de unidades en Bodega antes de reordenar."
        );

        Harmony = new Harmony("mod.supermarkettogether.buyallbutton");
        Harmony.PatchAll();
    }
}