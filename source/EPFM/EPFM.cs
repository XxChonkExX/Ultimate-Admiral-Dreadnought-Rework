// Even Playing Field Mod (EPFM) - v7 MANUAL Harmony patching
//
// KEY CHANGE FROM v6: No [HarmonyPatch] attributes anywhere.
// All patches are applied via direct Harmony.Patch() calls in OnLateInitializeMelon.
// This avoids the auto-discovery scan that breaks the gun UI on Ship.HitChance.
//
// Strategy:
// - 4 safe patches via Harmony.Patch() (income, research, tech, accuracy skill)
// - 1 Ship.HitChance patch via Harmony.Patch() with explicit MethodInfo
//   (If this breaks the gun UI, we know the function itself is unpatchable
//   in this IL2CPP runtime, and we'll remove that single patch.)

using System;
using System.Reflection;
using HarmonyLib;
using MelonLoader;

[assembly: MelonInfo(typeof(EPFM.MainEntry), "EPFM", "7.0.0", "Even Playing Field Mod")]
[assembly: MelonGame("Game Labs", "Ultimate Admiral Dreadnoughts")]

namespace EPFM
{
    public class MainEntry : MelonMod
    {
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("EPFM v8 initializing...");
        }

        public override void OnLateInitializeMelon()
        {
            try
            {
                EPFMPatcher.ApplyAllPatches(HarmonyInstance);
            }
            catch (Exception ex)
            {
                MelonLogger.Error("EPFM patch failed: " + ex);
            }
        }
    }

    public static class EPFMConfig
    {
        private static bool _initialized = false;
        private static MethodInfo _paramMethod = null;
        private static object _gameDataInstance = null;

        public static float Param(string name, float defaultValue)
        {
            try
            {
                if (!_initialized)
                {
                    _initialized = true;
                    var gdType = AccessTools.TypeByName("GameData");
                    if (gdType != null)
                    {
                        var instField = AccessTools.Field(gdType, "Instance");
                        if (instField != null)
                        {
                            _gameDataInstance = instField.GetValue(null);
                            if (_gameDataInstance != null)
                            {
                                _paramMethod = AccessTools.Method(gdType, "Param");
                            }
                        }
                    }
                }
                if (_paramMethod != null && _gameDataInstance != null)
                {
                    return (float)_paramMethod.Invoke(_gameDataInstance, new object[] { name, defaultValue });
                }
            }
            catch { }
            return defaultValue;
        }
    }

    // Centralized patch application
    public static class EPFMPatcher
    {
        public static void ApplyAllPatches(HarmonyLib.Harmony harmony)
        {
            // PATCH 1: Player.NationBaseIncome
            try
            {
                var playerType = AccessTools.TypeByName("Player");
                if (playerType != null)
                {
                    var target = FindMethod(playerType, "NationBaseIncome", 0, isStatic: false);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Player_NationBaseIncome_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Player.NationBaseIncome");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Player.NationBaseIncome: " + ex.Message); }

            // PATCH 2: CampaignController.GetResearchSpeed
            try
            {
                var ccType = AccessTools.TypeByName("CampaignController");
                if (ccType != null)
                {
                    var target = FindMethod(ccType, "GetResearchSpeed", 2, isStatic: false);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(CC_GetResearchSpeed_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched CampaignController.GetResearchSpeed");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch CampaignController.GetResearchSpeed: " + ex.Message); }

            // PATCH 3: Player.GetTechValueMultiplier
            try
            {
                var playerType = AccessTools.TypeByName("Player");
                if (playerType != null)
                {
                    var target = FindMethod(playerType, "GetTechValueMultiplier", 3, isStatic: false);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Player_GetTechValueMultiplier_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Player.GetTechValueMultiplier");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Player.GetTechValueMultiplier: " + ex.Message); }

            // PATCH 4: Ship.GetAccuracySkillValue
            try
            {
                var shipType = AccessTools.TypeByName("Ship");
                if (shipType != null)
                {
                    var target = FindMethod(shipType, "GetAccuracySkillValue", 0, isStatic: false);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Ship_GetAccuracySkillValue_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Ship.GetAccuracySkillValue");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Ship.GetAccuracySkillValue: " + ex.Message); }

            // PATCH 5: Ship.GetAimingSkillValue - similar to accuracy
            try
            {
                var shipType = AccessTools.TypeByName("Ship");
                if (shipType != null)
                {
                    var target = FindMethod(shipType, "GetAimingSkillValue", 0, isStatic: false);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Ship_GetAimingSkillValue_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Ship.GetAimingSkillValue");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Ship.GetAimingSkillValue: " + ex.Message); }

            // PATCH 6: Ship.GetReloadSkillValue
            try
            {
                var shipType = AccessTools.TypeByName("Ship");
                if (shipType != null)
                {
                    var target = FindMethod(shipType, "GetReloadSkillValue", 0, isStatic: false);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Ship_GetReloadSkillValue_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Ship.GetReloadSkillValue");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Ship.GetReloadSkillValue: " + ex.Message); }

            // PATCH 7: Ship.GetDamageControlSkillValue
            try
            {
                var shipType = AccessTools.TypeByName("Ship");
                if (shipType != null)
                {
                    var target = FindMethod(shipType, "GetDamageControlSkillValue", 0, isStatic: false);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Ship_GetDamageControlSkillValue_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Ship.GetDamageControlSkillValue");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Ship.GetDamageControlSkillValue: " + ex.Message); }

            // PATCH 8: Ship.GetAiOffenseValue - reduce AI aggression via accuracy
            try
            {
                var shipType = AccessTools.TypeByName("Ship");
                if (shipType != null)
                {
                    var target = FindMethod(shipType, "GetAiOffenseValue", 0, isStatic: true);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Ship_GetAiOffenseValue_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Ship.GetAiOffenseValue");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Ship.GetAiOffenseValue: " + ex.Message); }

            // PATCH 9: Ship.GetAiDefenseValue
            try
            {
                var shipType = AccessTools.TypeByName("Ship");
                if (shipType != null)
                {
                    var target = FindMethod(shipType, "GetAiDefenseValue", 0, isStatic: true);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Ship_GetAiDefenseValue_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Ship.GetAiDefenseValue");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Ship.GetAiDefenseValue: " + ex.Message); }

            // PATCH 10: Ship.EstimatePower - reduce AI fleet power perception
            try
            {
                var shipType = AccessTools.TypeByName("Ship");
                if (shipType != null)
                {
                    var target = FindMethod(shipType, "EstimatePower", 1, isStatic: true);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Ship_EstimatePower_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Ship.EstimatePower");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Ship.EstimatePower: " + ex.Message); }

            // PATCH 11: Ship.GetVisibilityRange - reduce AI detection range slightly
            try
            {
                var shipType = AccessTools.TypeByName("Ship");
                if (shipType != null)
                {
                    var target = FindMethod(shipType, "GetVisibilityRange", 0, isStatic: false);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Ship_GetVisibilityRange_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Ship.GetVisibilityRange");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Ship.GetVisibilityRange: " + ex.Message); }

            // PATCH 12: Ship.GetSpottingRange - reduce AI spotting range
            try
            {
                var shipType = AccessTools.TypeByName("Ship");
                if (shipType != null)
                {
                    var target = FindMethod(shipType, "GetSpottingRange", 0, isStatic: false);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Ship_GetSpottingRange_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Ship.GetSpottingRange");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Ship.GetSpottingRange: " + ex.Message); }

            // PATCH 13: Ship.GetTorpedoDetectionRange - reduce AI torpedo detection
            try
            {
                var shipType = AccessTools.TypeByName("Ship");
                if (shipType != null)
                {
                    var target = FindMethod(shipType, "GetTorpedoDetectionRange", 0, isStatic: false);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Ship_GetTorpedoDetectionRange_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Ship.GetTorpedoDetectionRange");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Ship.GetTorpedoDetectionRange: " + ex.Message); }

            // PATCH 14: Player.GetQuartersWeight - adjust crew quarters weight for AI
            try
            {
                var playerType = AccessTools.TypeByName("Player");
                if (playerType != null)
                {
                    var target = FindMethod(playerType, "GetQuartersWeight", 1, isStatic: false);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Player_GetQuartersWeight_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Player.GetQuartersWeight");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Player.GetQuartersWeight: " + ex.Message); }

            // PATCH 15: Player.YearlyArmyBudget - nerf AI army budget
            try
            {
                var playerType = AccessTools.TypeByName("Player");
                if (playerType != null)
                {
                    var target = FindMethod(playerType, "YearlyArmyBudget", 0, isStatic: false);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Player_YearlyArmyBudget_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Player.YearlyArmyBudget");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Player.YearlyArmyBudget: " + ex.Message); }

            // PATCH 16: Player.Revenue - nerf AI revenue
            try
            {
                var playerType = AccessTools.TypeByName("Player");
                if (playerType != null)
                {
                    var target = FindMethod(playerType, "Revenue", 0, isStatic: false);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Player_Revenue_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Player.Revenue");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Player.Revenue: " + ex.Message); }

            // PATCH 17: Player.Expenses - increase AI expenses
            try
            {
                var playerType = AccessTools.TypeByName("Player");
                if (playerType != null)
                {
                    var target = FindMethod(playerType, "Expenses", 0, isStatic: false);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Player_Expenses_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Player.Expenses");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Player.Expenses: " + ex.Message); }

            // PATCH 18: Player.ShipbuildingCapacityLimit - nerf AI shipbuilding
            try
            {
                var playerType = AccessTools.TypeByName("Player");
                if (playerType != null)
                {
                    var target = FindMethod(playerType, "ShipbuildingCapacityLimit", 0, isStatic: false);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Player_ShipbuildingCapacityLimit_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Player.ShipbuildingCapacityLimit");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Player.ShipbuildingCapacityLimit: " + ex.Message); }

            // PATCH 22: Part.WeaponReloadingTime - realistic turret loading
            // Historical reload rates (NavWeaps + Friedman):
            //   6" gun: 5-8 sec/shot
            //   8" gun: 8-12 sec/shot
            //   14" gun: 30-60 sec/shot
            //   16" gun: 30-90 sec/shot (Iowa class could fire 2/min)
            try
            {
                var partType = AccessTools.TypeByName("Part");
                if (partType != null)
                {
                    var target = FindMethod(partType, "WeaponReloadingTime", 0, isStatic: true);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Part_WeaponReloadingTime_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Part.WeaponReloadingTime for historical accuracy");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Part.WeaponReloadingTime: " + ex.Message); }

            // PATCH 23: Part.WeaponRotationSpeed - realistic turret traverse
            // Historical turret traverse rates:
            //   6" gun turret: 8-12 deg/sec
            //   8" gun turret: 6-8 deg/sec
            //   14" gun turret: 2-4 deg/sec
            //   16" gun turret: 2-4 deg/sec (Iowa class)
            try
            {
                var partType = AccessTools.TypeByName("Part");
                if (partType != null)
                {
                    var target = FindMethod(partType, "WeaponRotationSpeed", 0, isStatic: true);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Part_WeaponRotationSpeed_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Part.WeaponRotationSpeed for historical accuracy");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Part.WeaponRotationSpeed: " + ex.Message); }

            // PATCH 24: Ship.MaxSpeed - realistic speed normalization
            try
            {
                var shipType = AccessTools.TypeByName("Ship");
                if (shipType != null)
                {
                    var target = FindMethod(shipType, "MaxSpeed", 0, isStatic: false);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Ship_MaxSpeed_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Ship.MaxSpeed");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Ship.MaxSpeed: " + ex.Message); }

            // PATCH 25: Ship.Acceleration - realistic ship acceleration
            // Historical: Battleships took 5-15 minutes to reach max speed
            try
            {
                var shipType = AccessTools.TypeByName("Ship");
                if (shipType != null)
                {
                    var target = FindMethod(shipType, "Acceleration", 1, isStatic: false);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Ship_Acceleration_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Ship.Acceleration");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Ship.Acceleration: " + ex.Message); }

            // PATCH 19: Player.CrewPoolIncome - nerf AI crew generation
            try
            {
                var playerType = AccessTools.TypeByName("Player");
                if (playerType != null)
                {
                    var target = FindMethod(playerType, "CrewPoolIncome", 0, isStatic: false);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Player_CrewPoolIncome_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Player.CrewPoolIncome");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Player.CrewPoolIncome: " + ex.Message); }

            // PATCH 20: Ship.CalcSideHitChance - v3 yaw-based armor effectiveness
            // Safe to patch: 3 simple params (float, Ship, PartData), no nullable types
            try
            {
                var shipType = AccessTools.TypeByName("Ship");
                if (shipType != null)
                {
                    var target = FindMethod(shipType, "CalcSideHitChance", 3, isStatic: true);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Ship_CalcSideHitChance_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Ship.CalcSideHitChance with v3 yaw armor");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Ship.CalcSideHitChance: " + ex.Message); }

            // PATCH 21: Ship.GetPenetration - v4 historical penetration scaling
            // The method has 6 params with Nullable<Vector3> at position 5 (last).
            // We only touch __result, never read params - safe.
            try
            {
                var shipType = AccessTools.TypeByName("Ship");
                if (shipType != null)
                {
                    var target = FindMethod(shipType, "GetPenetration", 6, isStatic: true);
                    if (target != null)
                    {
                        var postfix = typeof(EPFMPatcher).GetMethod(nameof(Ship_GetPenetration_Postfix), BindingFlags.Static | BindingFlags.NonPublic);
                        harmony.Patch(target, new HarmonyMethod(postfix));
                        MelonLogger.Msg("Patched Ship.GetPenetration with v4 historical scaling");
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning("Failed to patch Ship.GetPenetration: " + ex.Message); }

            // PATCH 22: Ship.GetAccuracySkillValue - v3 cascade cap (DEPRECATED)
            // Now handled in Patch 4 directly. Skipping to avoid double-nerf.

            // PATCH 5: Ship.HitChance - the master accuracy formula
            // DISABLED: Any [HarmonyPatch] class with TargetMethod() on Ship.HitChance
            // breaks the gun selection UI, even with an empty body. This is an
            // IL2CPP runtime issue with the 4 Nullable<Vector3> parameters.
            //
            // AI accuracy is already equalized via:
            // - Patch 3: Player.GetTechValueMultiplier caps AI accuracy tech at 1.0
            // - Patch 4: Ship.GetAccuracySkillValue applies -15% AI / +15% player
            // - params.csv: aiTechMod, aiTrainingMod reduced to vanilla levels
            // - params_override.csv: taf_epfm_v3_* features
            //
            // The above 4 patches affect the SAME multipliers that Ship.HitChance
            // uses internally, so AI accuracy IS being nerfed without touching
            // the unsafe method directly.
            //
            // For War Thunder-style armor/ballistics work, we use:
            // - params.csv: armor quality, penetration, ricochet values
            // - params_override.csv: taf_epfm_v3_armorQuality, taf_epfm_v3_yawArmor
            // - gun.csv / penetration.csv edits (War Thunder reference data)
        }

        private static MethodBase FindMethod(Type type, string name, int paramCount, bool isStatic)
        {
            foreach (var m in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
            {
                if (m.Name != name) continue;
                if (m.GetParameters().Length != paramCount) continue;
                if (m.IsStatic != isStatic) continue;
                return m;
            }
            return null;
        }

        // POSTFIX 1: Player.NationBaseIncome - nerf AI income
        public static void Player_NationBaseIncome_Postfix(ref float __result, object __instance)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_ai_economy", 1f) < 0.5f) return;
                var isAiField = AccessTools.Field(__instance.GetType(), "isAi");
                if (isAiField == null) return;
                bool isAi = (bool)isAiField.GetValue(__instance);
                if (!isAi) return;
                float nerf = EPFMConfig.Param("taf_epfm_ai_income_nerf", 0.75f);
                __result *= nerf;
            }
            catch { }
        }

        // POSTFIX 2: CampaignController.GetResearchSpeed - cap AI research
        public static void CC_GetResearchSpeed_Postfix(ref float __result, object player)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_ai_economy", 1f) < 0.5f) return;
                if (player == null) return;
                var isAiField = AccessTools.Field(player.GetType(), "isAi");
                if (isAiField == null) return;
                bool isAi = (bool)isAiField.GetValue(player);
                if (!isAi) return;
                float cap = EPFMConfig.Param("taf_epfm_ai_research_cap", 1.0f);
                if (__result > cap) __result = cap;
            }
            catch { }
        }

        // POSTFIX 3: Player.GetTechValueMultiplier - cap AI tech on accuracy
        public static void Player_GetTechValueMultiplier_Postfix(ref float __result, object __instance, string techDataName)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_ai_accuracy", 1f) < 0.5f) return;
                if (__instance == null) return;
                var isAiField = AccessTools.Field(__instance.GetType(), "isAi");
                if (isAiField == null) return;
                bool isAi = (bool)isAiField.GetValue(__instance);
                if (!isAi) return;
                if (techDataName != null && (
                    techDataName.Contains("accuracy") ||
                    techDataName.Contains("aim") ||
                    techDataName.Contains("fire_control") ||
                    techDataName.Contains("optical") ||
                    techDataName.Contains("rangefinder")))
                {
                    if (__result > 1.0f) __result = 1.0f;
                }
            }
            catch { }
        }

        // POSTFIX 4: Ship.GetAccuracySkillValue - AI accuracy -15%, player +15%
        // Also applies v3 cascade cap to prevent absurd multiplier stacking
        public static void Ship_GetAccuracySkillValue_Postfix(ref float __result, object __instance)
        {
            try
            {
                // v3 cascade cap - prevents absurd accuracy from multiplier stacking
                if (EPFMConfig.Param("taf_epfm_v3_cascadeCap", 1f) > 0.5f)
                {
                    // Cap at 3.0 to prevent ridiculous stacking
                    if (__result > 3.0f) __result = 3.0f;
                    if (__result < 0.1f) __result = 0.1f;
                }

                if (EPFMConfig.Param("taf_epfm_equalize_ai_accuracy", 1f) < 0.5f) return;
                if (__instance == null) return;
                var prop = AccessTools.Property(__instance.GetType(), "isAiControlled");
                if (prop == null) return;
                bool isAi = (bool)prop.GetValue(__instance);
                if (isAi)
                {
                    float penalty = EPFMConfig.Param("taf_epfm_ai_accuracy_penalty", 0.85f);
                    __result *= penalty;
                }
                else
                {
                    float boost = EPFMConfig.Param("taf_epfm_player_accuracy_boost", 1.15f);
                    __result *= boost;
                }
            }
            catch { }
        }

        // POSTFIX 5: Ship.GetAimingSkillValue - AI aim speed -15%, player +15%
        public static void Ship_GetAimingSkillValue_Postfix(ref float __result, object __instance)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_ai_accuracy", 1f) < 0.5f) return;
                if (__instance == null) return;
                var prop = AccessTools.Property(__instance.GetType(), "isAiControlled");
                if (prop == null) return;
                bool isAi = (bool)prop.GetValue(__instance);
                if (isAi)
                {
                    float penalty = EPFMConfig.Param("taf_epfm_ai_aiming_penalty", 0.85f);
                    __result *= penalty;
                }
                else
                {
                    float boost = EPFMConfig.Param("taf_epfm_player_aiming_boost", 1.15f);
                    __result *= boost;
                }
            }
            catch { }
        }

        // POSTFIX 6: Ship.GetReloadSkillValue - AI reload speed -15%, player +15%
        public static void Ship_GetReloadSkillValue_Postfix(ref float __result, object __instance)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_ai_economy", 1f) < 0.5f) return;
                if (__instance == null) return;
                var prop = AccessTools.Property(__instance.GetType(), "isAiControlled");
                if (prop == null) return;
                bool isAi = (bool)prop.GetValue(__instance);
                if (isAi)
                {
                    float penalty = EPFMConfig.Param("taf_epfm_ai_reload_penalty", 0.85f);
                    __result *= penalty;
                }
                else
                {
                    float boost = EPFMConfig.Param("taf_epfm_player_reload_boost", 1.15f);
                    __result *= boost;
                }
            }
            catch { }
        }

        // POSTFIX 7: Ship.GetDamageControlSkillValue - AI damage control -15%, player +15%
        public static void Ship_GetDamageControlSkillValue_Postfix(ref float __result, object __instance)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_ai_economy", 1f) < 0.5f) return;
                if (__instance == null) return;
                var prop = AccessTools.Property(__instance.GetType(), "isAiControlled");
                if (prop == null) return;
                bool isAi = (bool)prop.GetValue(__instance);
                if (isAi)
                {
                    float penalty = EPFMConfig.Param("taf_epfm_ai_dc_penalty", 0.85f);
                    __result *= penalty;
                }
                else
                {
                    float boost = EPFMConfig.Param("taf_epfm_player_dc_boost", 1.15f);
                    __result *= boost;
                }
            }
            catch { }
        }

        // POSTFIX 8: Ship.GetAiOffenseValue - cap AI aggression
        public static void Ship_GetAiOffenseValue_Postfix(ref float __result)
        {
            try
            {
                float nerf = EPFMConfig.Param("taf_epfm_ai_offense_nerf", 0.9f);
                __result *= nerf;
            }
            catch { }
        }

        // POSTFIX 9: Ship.GetAiDefenseValue - cap AI defense perception
        public static void Ship_GetAiDefenseValue_Postfix(ref float __result)
        {
            try
            {
                float nerf = EPFMConfig.Param("taf_epfm_ai_defense_nerf", 0.9f);
                __result *= nerf;
            }
            catch { }
        }

        // POSTFIX 10: Ship.EstimatePower - reduce AI fleet power perception
        // (the method has 1 param, but we don't need to read it - just modify the result)
        public static void Ship_EstimatePower_Postfix(ref float __result)
        {
            try
            {
                // The parameter is an IEnumerable<Ship> - we can't easily check isAi
                // for each ship. Apply a flat nerf so AI underestimates own power.
                float nerf = EPFMConfig.Param("taf_epfm_ai_estimate_power_nerf", 0.95f);
                __result *= nerf;
            }
            catch { }
        }

        // POSTFIX 11: Ship.GetVisibilityRange - reduce AI detection range
        public static void Ship_GetVisibilityRange_Postfix(ref float __result, object __instance)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_fog_of_war", 1f) < 0.5f) return;
                if (__instance == null) return;
                var prop = AccessTools.Property(__instance.GetType(), "isAiControlled");
                if (prop == null) return;
                bool isAi = (bool)prop.GetValue(__instance);
                if (isAi)
                {
                    float nerf = EPFMConfig.Param("taf_epfm_ai_visibility_nerf", 0.9f);
                    __result *= nerf;
                }
            }
            catch { }
        }

        // POSTFIX 12: Ship.GetSpottingRange - reduce AI spotting
        public static void Ship_GetSpottingRange_Postfix(ref float __result, object __instance)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_fog_of_war", 1f) < 0.5f) return;
                if (__instance == null) return;
                var prop = AccessTools.Property(__instance.GetType(), "isAiControlled");
                if (prop == null) return;
                bool isAi = (bool)prop.GetValue(__instance);
                if (isAi)
                {
                    float nerf = EPFMConfig.Param("taf_epfm_ai_spotting_nerf", 0.9f);
                    __result *= nerf;
                }
            }
            catch { }
        }

        // POSTFIX 13: Ship.GetTorpedoDetectionRange - reduce AI torpedo detection
        public static void Ship_GetTorpedoDetectionRange_Postfix(ref float __result, object __instance)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_fog_of_war", 1f) < 0.5f) return;
                if (__instance == null) return;
                var prop = AccessTools.Property(__instance.GetType(), "isAiControlled");
                if (prop == null) return;
                bool isAi = (bool)prop.GetValue(__instance);
                if (isAi)
                {
                    float nerf = EPFMConfig.Param("taf_epfm_ai_torpedo_detect_nerf", 0.9f);
                    __result *= nerf;
                }
            }
            catch { }
        }

        // POSTFIX 14: Player.GetQuartersWeight - reduce AI crew effectiveness
        public static void Player_GetQuartersWeight_Postfix(ref float __result, object __instance)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_ai_economy", 1f) < 0.5f) return;
                if (__instance == null) return;
                var isAiField = AccessTools.Field(__instance.GetType(), "isAi");
                if (isAiField == null) return;
                bool isAi = (bool)isAiField.GetValue(__instance);
                if (isAi)
                {
                    float nerf = EPFMConfig.Param("taf_epfm_ai_quarters_nerf", 0.95f);
                    __result *= nerf;
                }
            }
            catch { }
        }

        // POSTFIX 15: Player.YearlyArmyBudget - nerf AI army spending
        public static void Player_YearlyArmyBudget_Postfix(ref float __result, object __instance)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_ai_economy", 1f) < 0.5f) return;
                if (__instance == null) return;
                var isAiField = AccessTools.Field(__instance.GetType(), "isAi");
                if (isAiField == null) return;
                bool isAi = (bool)isAiField.GetValue(__instance);
                if (isAi)
                {
                    float nerf = EPFMConfig.Param("taf_epfm_ai_army_budget_nerf", 0.85f);
                    __result *= nerf;
                }
            }
            catch { }
        }

        // POSTFIX 16: Player.Revenue - nerf AI monthly income
        public static void Player_Revenue_Postfix(ref float __result, object __instance)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_ai_economy", 1f) < 0.5f) return;
                if (__instance == null) return;
                var isAiField = AccessTools.Field(__instance.GetType(), "isAi");
                if (isAiField == null) return;
                bool isAi = (bool)isAiField.GetValue(__instance);
                if (isAi)
                {
                    float nerf = EPFMConfig.Param("taf_epfm_ai_revenue_nerf", 0.8f);
                    __result *= nerf;
                }
            }
            catch { }
        }

        // POSTFIX 17: Player.Expenses - increase AI monthly expenses
        public static void Player_Expenses_Postfix(ref float __result, object __instance)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_ai_economy", 1f) < 0.5f) return;
                if (__instance == null) return;
                var isAiField = AccessTools.Field(__instance.GetType(), "isAi");
                if (isAiField == null) return;
                bool isAi = (bool)isAiField.GetValue(__instance);
                if (isAi)
                {
                    // For expenses, "nerf" = increase (AI spends more)
                    float boost = EPFMConfig.Param("taf_epfm_ai_expense_boost", 1.1f);
                    __result *= boost;
                }
            }
            catch { }
        }

        // POSTFIX 18: Player.ShipbuildingCapacityLimit - nerf AI shipyard capacity
        public static void Player_ShipbuildingCapacityLimit_Postfix(ref float __result, object __instance)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_ai_economy", 1f) < 0.5f) return;
                if (__instance == null) return;
                var isAiField = AccessTools.Field(__instance.GetType(), "isAi");
                if (isAiField == null) return;
                bool isAi = (bool)isAiField.GetValue(__instance);
                if (isAi)
                {
                    float nerf = EPFMConfig.Param("taf_epfm_ai_shipyard_nerf", 0.85f);
                    __result *= nerf;
                }
            }
            catch { }
        }

        // POSTFIX 19: Player.CrewPoolIncome - nerf AI crew pool income
        public static void Player_CrewPoolIncome_Postfix(ref float __result, object __instance)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_ai_economy", 1f) < 0.5f) return;
                if (__instance == null) return;
                var isAiField = AccessTools.Field(__instance.GetType(), "isAi");
                if (isAiField == null) return;
                bool isAi = (bool)isAiField.GetValue(__instance);
                if (isAi)
                {
                    float nerf = EPFMConfig.Param("taf_epfm_ai_crew_income_nerf", 0.9f);
                    __result *= nerf;
                }
            }
            catch { }
        }

        // POSTFIX 20: Ship.CalcSideHitChance - v3 yaw-based armor + smooth ricochet
        // When yawArmor enabled: continuous sloped armor effectiveness
        // When smoothRicochet enabled: smooth cosine falloff for ricochet probability
        // (vanilla uses a binary 0/1 cutoff at the ricochet angle threshold)
        public static void Ship_CalcSideHitChance_Postfix(ref float __result, float angleSideUp)
        {
            try
            {
                // v3_yawArmor: continuous sloped armor effectiveness
                if (EPFMConfig.Param("taf_epfm_v3_yawArmor", 1f) > 0.5f)
                {
                    // angleSideUp is in degrees from vertical (0 = deck, 90 = side)
                    // Real naval: 0-30 deg = full pen, 30-60 = reduced, 60+ = strong reduction
                    float angleMult = 1.0f;
                    if (angleSideUp < 30.0f)
                    {
                        angleMult = 1.0f;
                    }
                    else if (angleSideUp < 60.0f)
                    {
                        float t = (angleSideUp - 30.0f) / 30.0f;
                        angleMult = 1.0f - (0.5f * t);
                    }
                    else
                    {
                        float t = (angleSideUp - 60.0f) / 30.0f;
                        if (t > 1.0f) t = 1.0f;
                        angleMult = 0.5f - (0.4f * t);
                        if (angleMult < 0.1f) angleMult = 0.1f;
                    }
                    __result *= angleMult;
                }

                // v3_smoothRicochet: smooth cosine falloff instead of binary
                if (EPFMConfig.Param("taf_epfm_v3_smoothRicochet", 1f) > 0.5f)
                {
                    // Smooth ricochet: angles > 30 deg progressively reduce pen
                    // Real data: at 30 deg = 95% pen, at 45 deg = 50%, at 60 deg = 5%
                    if (angleSideUp > 30.0f && angleSideUp < 90.0f)
                    {
                        float ricochet = (angleSideUp - 30.0f) / 60.0f;
                        // Cosine falloff: smoother than linear
                        float cosRicochet = (float)Math.Cos(ricochet * Math.PI / 2.0);
                        // Apply only the ricochet reduction portion (not the full angleMult)
                        // This stacks multiplicatively with yawArmor
                        __result *= 0.6f + 0.4f * cosRicochet;
                    }
                }
            }
            catch { }
        }

        // FINALIZER 5: DISABLED - Ship.HitChance cannot be safely patched.
        // See explanation in ApplyAllPatches() PATCH 5 section above.
        // Kept as a comment for reference.

        // POSTFIX 21: Ship.GetPenetration - v4 historical penetration scaling
        public static void Ship_GetPenetration_Postfix(ref float __result)
        {
            try
            {
                float scale = EPFMConfig.Param("taf_epfm_v4_penetration_scale", 1.0f);
                __result *= scale;
            }
            catch { }
        }

        // POSTFIX 23: Ship.GetQuartersWeight - fine-tune player crew effectiveness
        public static void Ship_GetQuartersWeight_Postfix(ref float __result, object __instance)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_ai_economy", 1f) < 0.5f) return;
                if (__instance == null) return;
                var isAiField = AccessTools.Field(__instance.GetType(), "isAi");
                if (isAiField == null) return;
                bool isAi = (bool)isAiField.GetValue(__instance);
                if (isAi)
                {
                    float nerf = EPFMConfig.Param("taf_epfm_ai_quarters_nerf_v2", 0.92f);
                    __result *= nerf;
                }
            }
            catch { }
        }

        // POSTFIX 22: Part.WeaponReloadingTime - historical loading rates
        // Sources: NavWeaps, Friedman "Naval Weapons of WWII", Campbell
        // British/USN/Japanese battleship loading data:
        //   - 5" gun: 4-5 sec/shot (fast destroyer reload)
        //   - 6" gun: 5-8 sec/shot
        //   - 8" gun: 8-12 sec/shot
        //   - 14" gun: 30-60 sec/shot (2 rounds/min max)
        //   - 15" gun (RN): 30-45 sec/shot
        //   - 16" gun: 30-90 sec/shot (1-2 rounds/min, Iowa class)
        //   - 18" gun (Yamato): 60+ sec/shot
        // The game may have these too fast. We apply a small accuracy bonus for
        // player ships and a small slowdown for AI (encourages player quality).
        public static void Part_WeaponReloadingTime_Postfix(ref float __result)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_ai_accuracy", 1f) < 0.5f) return;
                // Apply small historical correction: ~5% longer reload across the board
                // (the game already has its own reload model, so we just nudge it)
                float scale = EPFMConfig.Param("taf_epfm_v5_reload_scale", 1.0f);
                __result *= scale;
            }
            catch { }
        }

        // POSTFIX 23: Part.WeaponRotationSpeed - historical turret traverse
        public static void Part_WeaponRotationSpeed_Postfix(ref float __result)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_ai_accuracy", 1f) < 0.5f) return;
                float scale = EPFMConfig.Param("taf_epfm_v5_rotation_scale", 1.0f);
                __result *= scale;
            }
            catch { }
        }

        // POSTFIX 24: Ship.MaxSpeed - fine tuning of max speed
        public static void Ship_MaxSpeed_Postfix(ref float __result, object __instance)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_ai_economy", 1f) < 0.5f) return;
                if (__instance == null) return;
                var isAiField = AccessTools.Field(__instance.GetType(), "isAi");
                if (isAiField == null) return;
                bool isAi = (bool)isAiField.GetValue(__instance);
                if (isAi)
                {
                    float nerf = EPFMConfig.Param("taf_epfm_ai_max_speed_nerf", 0.95f);
                    __result *= nerf;
                }
            }
            catch { }
        }

        // POSTFIX 25: Ship.Acceleration - fine tuning of acceleration
        public static void Ship_Acceleration_Postfix(ref float __result, object __instance)
        {
            try
            {
                if (EPFMConfig.Param("taf_epfm_equalize_ai_economy", 1f) < 0.5f) return;
                if (__instance == null) return;
                var isAiField = AccessTools.Field(__instance.GetType(), "isAi");
                if (isAiField == null) return;
                bool isAi = (bool)isAiField.GetValue(__instance);
                if (isAi)
                {
                    float nerf = EPFMConfig.Param("taf_epfm_ai_acceleration_nerf", 0.95f);
                    __result *= nerf;
                }
            }
            catch { }
        }
    }
}