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
    /// Roundabout way of using revitalize elixir. Use after equipping class or all auras will disappear.
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

    /// <summary>
    /// Configures class skillset for ultras.
    /// Doesn't use wait for cooldown because it also waits for potion cd, so it's better to just spam skills.
    /// </summary>
    public void SkillsConfig(bool spamPot = false)
    {
        string currentClass = Bot.Player.CurrentClass.Name;
        string skillString = "";

        switch(currentClass)
        {
            case "Void Highlord":
                skillString = "3H>50S | 4 | 2 | 1H>30S | 3H>50S | 2 | 1H>30S | 3H>50S | 2 | 1H>30S";
                break;
            case "ArchPaladin":
                skillString = "3 | 1 | 2 | 1 | 2 | 4 | 1";
                break;
            case "Lord Of Order":
                skillString = "2 | 4 | 1 | 3";
                break;
            case "StoneCrusher":
                skillString = "2 | 3 | 1 | 4";
                break;

            default:
                skillString = "1 | 2 | 3 | 4";
                break;
        }
        if (spamPot)
            skillString = "5S | " + skillString;
        Bot.Skills.StartAdvanced(skillString);
    }

    /// <summary>
    /// Player 1 invites the other 3 players to make a party.
    /// </summary>
    /// <param name="players">List of player names in the army</param>
    public void PartySetup(string[] players)
    {
        Bot.Events.ExtensionPacketReceived += Army.PartyManagement;
        Army.waitForParty("templeshrine");
        Core.Sleep();
        if (Bot.Player.Username == players[0])
        {
            for (int i = 1; i < 4; i++)
            {
                Army.PartyInvite(players[i]);
                Core.Sleep();
            }
        }
        else
            Core.Sleep();
    }

    /// <summary>
    /// Turns off antilag from SetOptions()
    /// </summary>
    public void AntiLagOff()
    {
        Bot.Options.LagKiller = Bot.Options.LagKiller ? false : true;
        Bot.Flash.SetGameObject("stage.frameRate", 30);
        if (Bot.Flash.GetGameObject<bool>("ui.monsterIcon.redX.visible"))
            Bot.Flash.CallGameFunction("world.toggleMonsters");
    }
}