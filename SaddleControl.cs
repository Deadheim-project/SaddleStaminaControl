using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;

namespace SaddleControl
{
    [BepInPlugin(PluginGUID, PluginGUID, Version)]
    [HarmonyPatch]
    public class SaddleControl : BaseUnityPlugin
    {
        public const string PluginGUID = "Detalhes.SaddleControl";
        public const string Name = "SaddleControl";
        public const string Version = "1.0.0";

        ConfigSync configSync = new ConfigSync("Detalhes.SaddleControl") { DisplayName = "SaddleControl", CurrentVersion = Version, MinimumRequiredVersion = Version };

        Harmony _harmony = new Harmony(PluginGUID);

        public static ConfigEntry<float> maxStamina;
        public static ConfigEntry<float> runStaminaDrain;
        public static ConfigEntry<float> swimStaminaDrain;
        public static ConfigEntry<float> staminaRegen;
        public static ConfigEntry<float> staminaRegenHungry;

        public void Awake()
        {
            maxStamina = config("Server config", "maxStamina", 240f,
                   new ConfigDescription("maxStamina", null));

            runStaminaDrain = config("Server config", "runStaminaDrain", 1f,
         new ConfigDescription("runStaminaDrain", null));

            swimStaminaDrain = config("Server config", "swimStaminaDrain", 1f,
         new ConfigDescription("swimStaminaDrain", null));

            staminaRegen = config("Server config", "staminaRegen", 5f,
         new ConfigDescription("staminaRegen", null));

            staminaRegenHungry = config("Server config", "staminaRegenHungry", 3f,
         new ConfigDescription("staminaRegenHungry", null));
            _harmony.PatchAll();
        }

        ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true)
        {
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        ConfigEntry<T> config<T>(string group, string name, T value, string description, bool synchronizedSetting = true) => config(group, name, value, new ConfigDescription(description), synchronizedSetting);

        [HarmonyPatch(typeof(Sadle), nameof(Sadle.Awake))]
        public static class Awake_Postfix
        {
            private static void Postfix(Sadle __instance)
            {    
                __instance.m_maxStamina = maxStamina.Value;
                __instance.m_runStaminaDrain = runStaminaDrain.Value;
                __instance.m_swimStaminaDrain = swimStaminaDrain.Value;
                __instance.m_staminaRegen = staminaRegen.Value;
                __instance.m_staminaRegenHungry = staminaRegenHungry.Value;
            }
        }
    }
}
