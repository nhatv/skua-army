/*
name: Ultra Tyndarius
description: Army for Ultra Avatar Tyndarius weekly
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

public class UltraTyndarius
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

        // Delay so that AP and LoO enters first
        if (Bot.Player.Username == players[0] || Bot.Player.Username == players[3])
        {
            Bot.Sleep(1000);
        }
        Core.Jump("Boss", "Left");
        Core.Join("ultratyndarius", "Boss", "Left"); // Set spawn at boss room
        while (!Bot.ShouldExit && !Core.CheckInventory("Ultra Avatar Tyndarius Defeated"))
        {
            // AP and LoO focus Tyndarius, DoT targets left orb, LR targets right orb when they are up
            if (Bot.Player.Username == players[1] || Bot.Player.Username == players[2])
            {
                Bot.Combat.Attack("Ultra Avatar Tyndarius");
            }
            else if (Bot.Player.Username == players[3])
            {
                while (Core.IsMonsterAlive(1, useMapID: true))
                {
                    Bot.Combat.Attack(1);
                    if (!Core.IsMonsterAlive(2, useMapID: true))
                    {
                        Core.JumpWait();
                        break;
                    }
                }
                while (Core.IsMonsterAlive(3, useMapID: true))
                {
                    Bot.Combat.Attack(3);
                    if (!Core.IsMonsterAlive(2, useMapID: true))
                    {
                        Core.JumpWait();
                        break;
                    }
                }
                Bot.Combat.Attack(2);
                Bot.Sleep(1000);
            }
            else
            {
                while (Core.IsMonsterAlive(3, useMapID: true))
                    Bot.Combat.Attack(3);
                    if (!Core.IsMonsterAlive(2, useMapID: true))
                    {
                        Core.JumpWait();
                        break;
                    }
                Bot.Combat.Attack(2);
                Bot.Sleep(1000);
            }
            if (!Core.IsMonsterAlive(2, useMapID: true))
                break;
        }

        Core.JumpWait();
        Core.EnsureComplete(8245);
        

    }

}