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
using Skua.Core.Models.Monsters;
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
    /// Roundabout way of using revitalize elixir. Use after equipping class or all auras will disappear
    /// </summary>
    public void UseRevitalize()
    {
        if (!Core.CheckInventory("Scroll of Enrage") && !Core.CheckInventory("Potent Revitalize Elixir"))
            return;
        Core.Join("battleontown-999999");
        Core.Equip(new[]{ "Scroll of Enrage", "Potent Revitalize Elixir" });
        do
        {
            Bot.Combat.Attack("*");
            Bot.Skills.UseSkill(5);
        } while (!Bot.ShouldExit && !Bot.Self.HasActiveAura("Potent Revitalize Elixir"));
    }

    public void KillChampDrakath()
    {
        string[] players = Army.Players();

        Core.EnsureAccept(8300);
        Core.Join("championdrakath", "r2", "Left");
        Army.waitForParty("championdrakath");

        Monster? monster = Bot.Monsters.CurrentMonsters?.Find(m => m.MapID == 1);
        if (monster == null)
        {
            Core.Logger($"Monster not found. Something is wrong. Stopping bot", messageBox: true, stopBot: true);
            return;
        }
        Core.Jump("r2", "Left");
        while (!Bot.ShouldExit && !Core.CheckInventory("Champion Drakath Defeated"))
        {
            if (Bot.Player.Username == players[0])
            {
                monster = Bot.Monsters.CurrentMonsters?.Find(m => m.MapID == 1);
                if (((monster.HP <= (18000000 + (int)(Bot.Config.Get<int>("threshold"))) && monster.HP >= 18000000) || 
                    (monster.HP <= (16000000 + (int)(Bot.Config.Get<int>("threshold"))) && monster.HP >= 16000000) || 
                    (monster.HP <= (14000000 + (int)(Bot.Config.Get<int>("threshold"))) && monster.HP >= 14000000) || 
                    (monster.HP <= (12000000 + (int)(Bot.Config.Get<int>("threshold"))) && monster.HP >= 12000000) || 
                    (monster.HP <= (8000000 + (int)(Bot.Config.Get<int>("threshold"))/2) && monster.HP >= 8000000) || 
                    (monster.HP <= (6000000 + (int)(Bot.Config.Get<int>("threshold"))/2) && monster.HP >= 6000000) || 
                    (monster.HP <= (4000000 + (int)(Bot.Config.Get<int>("threshold"))/2) && monster.HP >= 4000000)) && !Bot.Target.HasActiveAura("Focus"))
                {
                    Core.Logger($"Drakath HP: {monster.HP}, Taunting");
                    Bot.Skills.StartAdvanced("5");
                }
                else
                {
                    if (Bot.Player.CurrentClass.Name == "Void Highlord")
                        Bot.Skills.StartAdvanced("3 | 4 | 1 | 2");
                    else
                        Bot.Skills.StartAdvanced(Bot.Player.CurrentClass?.Name ?? "generic", false);
                }
                
            }
            
            if (Bot.Player.Username != players[0] && Bot.Config.Get<bool>("spamFeli") && Bot.Skills.CanUseSkill(5))
            {
                Bot.Skills.UseSkill(5);
            }
            Bot.Combat.Attack("Champion Drakath");
        }

        Core.Join("championdrakath", "r2", "Left");
        Adv.KillUltra("championdrakath", "r2", "Left", "Champion Drakath", "Champion Drakath Defeated", publicRoom: false);
        Core.EnsureComplete(8300);
    }

}