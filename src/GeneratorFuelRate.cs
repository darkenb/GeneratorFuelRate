using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GeneratorFuelRate
{
    public enum Language { English, Chinese, Turkish }

    [BepInPlugin("bug.dsp.generatorfuelrate", "GeneratorFuelRate", "1.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        private static Text rateText;
        private static ConfigEntry<bool> showCurrent, showMax;
        private static ConfigEntry<Language> language;
        private static readonly string[] perMin = { "/ min", "/ 分钟", "/ dk" }, maxWord = { "max", "最大", "maks" };

        private void Awake()
        {
            showCurrent = Config.Bind("Display", "ShowCurrent", true, "Show fuel items per minute at the plant's current output.");
            showMax = Config.Bind("Display", "ShowMax", true, "Show fuel items per minute the plant would burn at full load (max).");
            language = Config.Bind("General", "Language", Language.English, "Text language: English, Chinese or Turkish.");
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        // Same drain as PowerGeneratorComponent.GenEnergyByFuel for `energy` J/tick, scaled to items per minute.
        public static double FuelPerMinute(in PowerGeneratorComponent g, long energy, long heatValue)
        {
            if (heatValue <= 0 || g.genEnergyPerTick <= 0) return 0;
            double drain = g.productive
                ? (double)energy * g.useFuelPerTick * 40 / ((double)g.genEnergyPerTick * Cargo.incFastDivisionNumerator[g.fuelIncLevel])
                : (double)energy * g.useFuelPerTick / g.genEnergyPerTick;
            return drain * 3600 / heatValue;
        }

        // Covers every fuel-burning generator (Fuel Cell, Fission, Artificial Star, Artificial Photonix...): group2 is the game's fuel panel.
        [HarmonyPostfix, HarmonyPatch(typeof(UIPowerGeneratorWindow), "_OnUpdate")]
        private static void OnUpdate(UIPowerGeneratorWindow __instance)
        {
            if (__instance.generatorId == 0 || __instance.powerSystem == null || !__instance.group2.gameObject.activeSelf) return;
            if (rateText == null)
            {
                // Clone the efficiency label so font and colour match; left-aligned under it so it clears the centred alarm button.
                rateText = Instantiate(__instance.fuelText2, __instance.fuelText2.transform.parent.parent);
                rateText.name = "fuel-rate-text";
                rateText.alignment = TextAnchor.MiddleLeft;
                rateText.horizontalOverflow = HorizontalWrapMode.Overflow;
                rateText.raycastTarget = false;
                var rt = rateText.rectTransform;
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0f, 0.5f);
                rt.sizeDelta = new Vector2(180f, 20f);
                rt.anchoredPosition = new Vector2(-40f, -70f);
            }
            ref var g = ref __instance.powerSystem.genPool[__instance.generatorId];
            var fuel = LDB.items.Select(g.curFuelId != 0 ? g.curFuelId : g.fuelId);
            if (fuel == null) { rateText.text = ""; return; }
            string cur = FuelPerMinute(g, g.generateCurrentTick, fuel.HeatValue).ToString("0.##");
            // capacityCurrentTick already holds proliferator, boost, antimatter and GenesisBook reactor bonuses.
            string max = FuelPerMinute(g, g.capacityCurrentTick, fuel.HeatValue).ToString("0.##");
            int l = (int)language.Value;
            rateText.text = showCurrent.Value && showMax.Value ? cur + " " + perMin[l] + "  (" + maxWord[l] + " " + max + ")"
                : showCurrent.Value ? cur + " " + perMin[l]
                : showMax.Value ? maxWord[l] + " " + max + " " + perMin[l] : "";
        }
    }
}
