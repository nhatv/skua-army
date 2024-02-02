/*
name: Ultra Nulgath
description: Army for Ultra Nulgath weekly
tags: ultra, weekly, army
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Army/CoreArmyLite.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class UltraNulgath
{
    public IScriptInterface Bot => IScriptInterface.Instance;

    public CoreBots Core => CoreBots.Instance;
    public CoreAdvanced Adv = new();
    public CoreFarms Farm = new();
    private static CoreArmyLite sArmy = new();
    private CoreArmyLite Army = new();
    public string OptionsStorage = "Ultra";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new List<IOption>()
    {
        sArmy.player1,
        sArmy.player2,
        sArmy.player3,
        sArmy.player4,
        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DoWeekly();

        Core.SetOptions(false);
    }

    public void DoWeekly()
    {
        Core.OneTimeMessage("Only for army", "This is intended for use with an army, not for solo players.");

        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = Army.getRoomNr();

        // Turn off antilag
        Bot.Options.LagKiller = false;
        //Bot.Flash.SetGameObject("stage.frameRate", 30);
        if (Bot.Flash.GetGameObject<bool>("ui.monsterIcon.redX.visible"))
            Bot.Flash.CallGameFunction("world.toggleMonsters");

        string[] players = Army.Players();
        if (Bot.Player.Username == players[0])
        {
            Core.Equip(new[]{"Legion Revenant", "Cape of Awe", "Necrotic Sword of Doom" } );
            Core.Equip("Potent Revitalize Elixir");
            Core.Equip("Felicitous Philtre");
            Bot.Skills.StartAdvanced("5S | 3 | 4 | 1 | 2");
        } 
        else if (Bot.Player.Username == players[1])
        {
            // Try DoT
            Core.Equip(new[]{ "ArchPaladin", "Cape of Awe" });
            Core.Equip("Potent Revitalize Elixir");
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("3 | 1 | 2 | 5S");
        } 
        else if (Bot.Player.Username == players[2])
        {
            // Try DoT
            Core.Equip(new[]{ "Lord of Order", "Cape of Awe", "Malgor's ShadowFlame Blade" });
            Core.Equip("Potent Revitalize Elixir");
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("5S | 2 | 4 | 1 | 3");
        }
        else if (Bot.Player.Username == players[3])
        {
            // Healer gear
            // Core.Equip(new[]{ "Dragon of Time", "Awescended Omni Cowl", "Category Five Hurricane Cloud", "Exalted Apotheosis" });
            // Core.Equip("Potent Revitalize Elixir");
            // Core.Equip("Potent Honor Potion");
            // Bot.Skills.StartAdvanced("5S | 3 | 2 | 1 | 2 | 4");
            Core.Equip("Chaos Avenger");
            Core.Equip("Potent Revitalize Elixir");
            Core.Equip("Felicitous Philtre");
            Bot.Skills.StartAdvanced("5S | 4 | 3 | 1 | 2");
        }

        Core.EnsureAccept(8692);
        Army.waitForParty("whitemap");
        Core.Join("ultranulgath", "Boss", "Right");
        while (!Bot.ShouldExit && !Core.CheckInventory("Nulgath the Archfiend Defeated?"))
        {
            // DOT Focus sword when its up, everyone else target Nulgath
            if (Bot.Player.Username == players[3])
            {
                while (Core.IsMonsterAlive("Overfiend Blade"))
                {
                    Bot.Combat.Attack("Overfiend Blade");
                    if (!Core.IsMonsterAlive("Nulgath the Archfiend"))
                    {
                        Core.JumpWait();
                        break;
                    }
                }
                Bot.Combat.Attack("Nulgath the Archfiend");
                Bot.Sleep(1000);
            }
            if (!Core.IsMonsterAlive("Nulgath the Archfiend"))
            {
                Core.JumpWait();
                break;
            }
            Bot.Combat.Attack("Nulgath the Archfiend");
            Bot.Sleep(1000);
        }
        
        Core.EnsureComplete(8692);

    }

}