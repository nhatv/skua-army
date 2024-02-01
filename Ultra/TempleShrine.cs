/*
name: Temple Shrine Daily
description: Army for Temple Shrine daily
tags: ultra, daily, army
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

public class TempleShrineDaily
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
        //Core.SetOptions(disableClassSwap: true);
        Core.SetOptions();

        DoDaily();

        Core.SetOptions(false);
    }

    public void DoDaily()
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
            Core.Equip("Legion Revenant");
            Core.Equip("Cape of Awe"); // Penitence Cape
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("5S | 3S | 4S | 1S | 2S");
            Bot.Options.LagKiller = false;
        } 
        else if (Bot.Player.Username == players[1])
        {
            Core.Equip("ArchPaladin");
            Core.Equip("Cape of Awe"); // Penitence Cape
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("3S | 5S | 2S | 1S");
        } 
        else if (Bot.Player.Username == players[2])
        {
            // Awe Blast
            Core.Equip("Lord of Order");
            Core.Equip("Cape of Awe"); // Penitence Cape
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("2S | 4S | 1S | 3S | 5S");
        }
        else if (Bot.Player.Username == players[3])
        {
            // // Healer gear
            // Core.Equip("Dragon of Time");
            // Core.Equip("Awescended Omni Cowl");
            // Core.Equip("Category Five Hurricane Cloud");
            // Core.Equip("Exalted Apotheosis");
            // Core.Equip("Scroll of Enrage");
            // Bot.Skills.StartAdvanced("5S | 3 | 2 | 1 | 2 | 4", 250, SkillUseMode.UseIfAvailable);

            Core.Equip("Chaos Avenger");
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("4 | 3 | 1S | 2 | 5S");
        }

        Army.waitForParty("ultratyndarius"); // Use LoO's buffs here
        Core.EnsureAccept(8245);

        Core.Jump("Boss", "Left");
        Core.Join("ultratyndarius", "Boss", "Left"); // Set spawn at boss room
        while (!Bot.ShouldExit && !Core.CheckInventory("Ultra Avatar Tyndarius Defeated"))
        {
            Bot.Combat.Attack(2);
            Bot.Sleep(1000);
        }

        Core.JumpWait();
        Core.EnsureComplete(8245);
        

    }

    public void templeshrineDailies()
    {
        if (!CheckDaily(9303) && !CheckDaily(9304) && !CheckDaily(9305))
            return;

        //Night Falls (Daily Bonus) - Sliver of Moonlight
        if (CheckDaily(9303))
        {
            Core.EnsureAccept(9303);
            Core.HuntMonster("midnightsun", "*", "Midnight Moondrop");
            Core.EnsureComplete(9303);
            Bot.Wait.ForPickup("Sliver of Moonlight");
        }

        //Dawn Breaks (Daily Bonus) - Sliver of Sunlight
        if (CheckDaily(9304))
        {
            Core.EnsureAccept(9304);
            Core.HuntMonster("solsticemoon", "*", "Solstice Sundew");
            Core.EnsureComplete(9304);
            Bot.Wait.ForPickup("Sliver of Sunlight");
        }

        //boss 3 requires taunting, not doable for skua atm.
        //Frozen Cycle (Daily Bonus) - Ecliptic Offering
        if (CheckDaily(9305))
        {
            Core.EnsureAccept(9305);
            Core.Join("templeshrine");
            Core.HuntMonster("ascendeclipse", "monster", "Midnight's Shadow");
            Core.HuntMonster("ascendeclipse", "monster", "Solstice's Shadow");
            Core.EnsureComplete(9305);
            Bot.Wait.ForPickup("Ecliptic Offering");
        }
    }

}