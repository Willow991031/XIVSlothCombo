﻿using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using ImGuiNET;
using XIVSlothComboX.Core;
using XIVSlothComboX.Services;
using XIVSlothComboX.Window.Functions;

namespace XIVSlothComboX.Window.Tabs
{
    internal class PvPFeatures : ConfigWindow
    {
        internal static new void Draw()
        {
            ImGui.Text("此选项卡允许您选择要启用的PvP连击和功能。");

            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.Text($"{FontAwesomeIcon.SkullCrossbones.ToIconString()}");
            ImGui.PopFont();
            ImGui.SameLine();
            ImGui.TextUnformatted("PVP功能只能在PVP用");
            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.Text($"{FontAwesomeIcon.SkullCrossbones.ToIconString()}");
            ImGui.PopFont();

            ImGui.BeginChild("scrolling", new Vector2(0, 0), true);

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 5));

            int i = 1;

            foreach (string? jobName in groupedPresets.Keys)
            {
                if (!groupedPresets[jobName].Any(x => PluginConfiguration.IsSecret(x.Preset))) 
                    continue;
                string header = jobName;
                if (jobName == groupedPresets.First().Key)
                {
                    header = "All Jobs";
                }
                if (ImGui.CollapsingHeader(header))
                {
                    foreach (var otherJob in groupedPresets.Keys.Where(x => x != jobName))
                    {
                        ImGui.GetStateStorage().SetInt(ImGui.GetID(otherJob), 0);
                    }
                    
                    if (jobName != groupedPresets.First().Key)
                    {
                        ImGui.GetStateStorage().SetInt(ImGui.GetID("All Jobs"), 0);
                    }

                    DrawHeadingContents(jobName, i);
                }

                else
                {
                    i += groupedPresets[jobName].Where(x => PluginConfiguration.IsSecret(x.Preset)).Count();
                    foreach (var preset in groupedPresets[jobName].Where(x => PluginConfiguration.IsSecret(x.Preset)))
                    {
                        i += Presets.AllChildren(presetChildren[preset.Preset]);
                    }
                }
            }
            ImGui.PopStyleVar();
            ImGui.EndChild();
        }

        private static void DrawHeadingContents(string jobName, int i)
        {
            foreach (var (preset, info) in groupedPresets[jobName].Where(x => PluginConfiguration.IsSecret(x.Preset)))
            {
                InfoBox presetBox = new() { Color = Colors.Grey, BorderThickness = 1f, CurveRadius = 8f, ContentsAction = () => { Presets.DrawPreset(preset, info, ref i); } };

                if (Service.Configuration.HideConflictedCombos)
                {
                    var conflictOriginals = Service.Configuration.GetConflicts(preset); // Presets that are contained within a ConflictedAttribute
                    var conflictsSource = Service.Configuration.GetAllConflicts();      // Presets with the ConflictedAttribute

                    if (!conflictsSource.Where(x => x == preset).Any() || conflictOriginals.Length == 0)
                    {
                        presetBox.Draw();
                        ImGuiHelpers.ScaledDummy(12.0f);
                        continue;
                    }

                    if (conflictOriginals.Any(x => Service.Configuration.IsEnabled(x)))
                    {
                        Service.Configuration.EnabledActions.Remove(preset);
                        Service.Configuration.Save();
                    }

                    else
                    {
                        presetBox.Draw();
                        ImGuiHelpers.ScaledDummy(12.0f);
                        continue;
                    }
                }

                else
                {
                    presetBox.Draw();
                    ImGuiHelpers.ScaledDummy(12.0f);
                }
            }
        }
    }
}
