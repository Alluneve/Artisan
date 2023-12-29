﻿using Artisan.Autocraft;
using Artisan.RawInformation;
using ClickLib.Clicks;
using ECommons;
using ECommons.Automation;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;

namespace Artisan.GameInterop;

public static unsafe class Operations
{
    public unsafe static void RepeatTrialCraft()
    {
        try
        {
            if (Throttler.Throttle(500))
            {
                if (GenericHelpers.TryGetAddonByName<AddonRecipeNote>("RecipeNote", out var recipenote))
                {
                    Callback.Fire(&recipenote->AtkUnitBase, true, 10);
                    Endurance.Tasks.Clear();
                }
            }
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "RepeatTrialCraft");
        }
    }

    public unsafe static void QuickSynthItem(int crafts)
    {
        try
        {
            var recipeWindow = Svc.GameGui.GetAddonByName("RecipeNote", 1);
            if (recipeWindow == nint.Zero)
                return;

            GenericHelpers.TryGetAddonByName<AddonRecipeNoteFixed>("RecipeNote", out var addon);

            if (!int.TryParse(addon->SelectedRecipeQuantityCraftableFromMaterialsInInventory->NodeText.ToString(), out int trueNumberCraftable) || trueNumberCraftable == 0)
            {
                return;
            }

            var addonPtr = (AddonRecipeNote*)recipeWindow;
            if (addonPtr == null)
                return;

            try
            {
                if (Throttler.Throttle(500))
                {
                    ClickRecipeNote.Using(recipeWindow).QuickSynthesis();

                    var quickSynthPTR = Svc.GameGui.GetAddonByName("SynthesisSimpleDialog", 1);
                    if (quickSynthPTR == nint.Zero)
                        return;

                    var quickSynthWindow = (AtkUnitBase*)quickSynthPTR;
                    if (quickSynthWindow == null)
                        return;

                    var values = stackalloc AtkValue[2];
                    values[0] = new()
                    {
                        Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                        Int = Math.Min(trueNumberCraftable, Math.Min(crafts, 99)),
                    };
                    values[1] = new()
                    {
                        Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Bool,
                        Byte = 1,
                    };

                    quickSynthWindow->FireCallback(3, values);

                }


            }
            catch (Exception e)
            {
                e.Log();
            }

        }
        catch (Exception ex)
        {
            ex.Log();
        }
    }

    public unsafe static void CloseQuickSynthWindow()
    {
        try
        {
            var quickSynthPTR = Svc.GameGui.GetAddonByName("SynthesisSimple", 1);
            if (quickSynthPTR == nint.Zero)
                return;

            var quickSynthWindow = (AtkUnitBase*)quickSynthPTR;
            if (quickSynthWindow == null)
                return;

            var qsynthButton = (AtkComponentButton*)quickSynthWindow->UldManager.NodeList[2];
            AtkResNodeFunctions.ClickButton(quickSynthWindow, qsynthButton, 0);
        }
        catch (Exception e)
        {
            e.Log();
        }
    }

    public unsafe static void RepeatActualCraft()
    {
        try
        {
            if (GenericHelpers.TryGetAddonByName<AddonRecipeNote>("RecipeNote", out var recipenote))
            {
                ClickRecipeNote.Using(new nint(&recipenote->AtkUnitBase)).Synthesize();
                Endurance.Tasks.Clear();
            }
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "RepeatActualCraft");
        }
    }
}