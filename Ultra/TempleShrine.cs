/*
name: Temple Shrine Daily
description: Army for Temple Shrine daily
tags: ultra, daily, army
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Army/Ultra/CoreUltra.cs
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
    public CoreDailies Daily = new();
    private static CoreArmyLite sArmy = new();
    private CoreArmyLite Army = new();
    public CoreUltra Ultra = new();
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
        EquipSideDungeon();
        // Ultra.UseRevitalize();

        //Night Falls (Daily Bonus) - Sliver of Moonlight
        if (Daily.CheckDaily(9303))
        {
            Core.EnsureAccept(9303);
            Adv.KillUltra("solsticemoon", "Enter", "Spawn", "Faithless Deer");
            Adv.KillUltra("solsticemoon", "Enter", "Spawn", "Shackled Fairy");
            Core.Jump("r1", "Left");
            Adv.KillUltra("solsticemoon", "r1", "Left", "Lunar Haze");
            Adv.KillUltra("solsticemoon", "r1", "Left", "Faithless Deer");
            Core.Jump("r2", "Right");
            Adv.KillUltra("solsticemoon", "r2", "Right", "Lunar Haze");
            Adv.KillUltra("solsticemoon", "r2", "Right", "Shackled Fairy");
            Core.Jump("r3", "Right");
            Adv.KillUltra("solsticemoon", "r3", "Right", "Hollow Midnight");
            Core.EnsureComplete(9303);
            Bot.Wait.ForPickup("Sliver of Moonlight");
        }

        //Dawn Breaks (Daily Bonus) - Sliver of Sunlight
        if (Daily.CheckDaily(9304))
        {
            Core.EnsureAccept(9304);
            Adv.KillUltra("midnightsun", "Enter", "Spawn", "Dying Light");
            Adv.KillUltra("midnightsun", "Enter", "Spawn", "Shining Star");
            Core.Jump("r1", "Left");
            Adv.KillUltra("midnightsun", "r1", "Left", "Dawn Knight"); // Sun mechanics don't matter, supps will sometimes die and that's fine
            Adv.KillUltra("midnightsun", "r1", "Left", "Shining Star");
            Core.Jump("r2", "Right");
            Adv.KillUltra("midnightsun", "r2", "Right", "Dawn Knight");
            Adv.KillUltra("midnightsun", "r2", "Right", "Dying Light");
            Core.Jump("r3", "Left");
            Adv.KillUltra("midnightsun", "r3", "Left", "Hollow Solstice");
            Core.EnsureComplete(9304);
            Bot.Wait.ForPickup("Sliver of Sunlight");
        }

        //Frozen Cycle (Daily Bonus) - Ecliptic Offering
        if (Daily.CheckDaily(9305))
        {
            Core.Equip("Scroll of Enrage");
            Core.EnsureAccept(9305);
            Core.Join("templeshrine");
            while (!Bot.ShouldExit && !Core.CheckInventory("Ecliptic Offering"))
            {
                if (Bot.Player.Username == players[0] || Bot.Player.Username == players[3]) // LR and Cav
                {
                    while (Core.IsMonsterAlive("Ascended Solstice"))
                    {
                        if (Bot.Player.Username == players[0] && !Bot.Target.HasActiveAura("Focus"))
                            Bot.Skills.UseSkill(5);
                        Bot.Combat.Attack("Ascended Solstice");
                        Bot.Sleep(1000);
                    }
                    
                }
                if (Bot.Player.Username == players[2] && !Bot.Target.HasActiveAura("Focus"))
                    Bot.Skills.UseSkill(5);
                Bot.Combat.Attack("Ascended Midnight");
                Bot.Sleep(1000);
            }
            Core.EnsureComplete(9305);
            Bot.Wait.ForPickup("Ecliptic Offering");
        }
        // while (!Bot.ShouldExit && !Core.CheckInventory("Ultra Avatar Tyndarius Defeated"))
        // {
        //     Bot.Combat.Attack(2);
        //     Bot.Sleep(1000);
        // }
        

    }

    private void EquipSideDungeon()
    {
        string[] players = Army.Players();
        if (Bot.Player.Username == players[0])
        {
            Core.Equip("Legion Revenant");
            Core.Equip("Cape of Awe"); // Penitence Cape
            Bot.Skills.StartAdvanced("3S | 4S | 1S | 2S");
            Bot.Options.LagKiller = false;
        } 
        else if (Bot.Player.Username == players[1])
        {
            Core.Equip("ArchPaladin");
            Core.Equip("Cape of Awe"); // Penitence Cape
            // Bot.Skills.StartAdvanced("3S | 2S | 1S");
            Bot.Skills.StartAdvanced("3 | 1 | 2 | 1 | 2 | 4 | 1", 250, SkillUseMode.WaitForCooldown);
        } 
        else if (Bot.Player.Username == players[2])
        {
            Core.Equip("Lord of Order");
            Core.Equip("Cape of Awe"); // Penitence Cape
            Bot.Skills.StartAdvanced("2 | 4 | 1 | 3");
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
            Bot.Skills.StartAdvanced("4 | 3 | 1 | 2");
        }
    }

}