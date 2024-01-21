/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Army/CoreArmyLite.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class CoreUltra
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private CoreFarms Farm = new();
    private CoreAdvanced Adv => new();
    private CoreArmyLite Army = new();
    private static CoreArmyLite sArmy = new();

    public void ScriptMain(IScriptInterface bot)
    {
        Core.RunCore();
    }

    /// <summary>
    /// Roundabout way of using revitalize elixir. Use after equipping class or the buff will disappear
    /// </summary>
    public void UseRevitalize()
    {
        Bot.Options.AttackWithoutTarget = true;
        Core.Join("lair-999999");
        Core.Equip(new[]{ "Felicitous Philtre", "Potent Revitalize Elixir" });
        Bot.Skills.StartAdvanced("5");
        while (!Bot.Self.HasActiveAura("Potent Revitalize Elixir"))
            Bot.Combat.Attack("*");

        Bot.Options.AttackWithoutTarget = false;
    }

}
