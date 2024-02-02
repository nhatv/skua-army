/*
name: Ultra Drakath
description: Army for Ultra Drakath weekly
tags: ultra, weekly, army
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Army/CoreArmyLite.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class UltraDrakath
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

        // Turn off antilag
        // Bot.Options.LagKiller = false;
        // Bot.Flash.SetGameObject("stage.frameRate", 30);
        // Bot.Flash.CallGameFunction("world.toggleMonsters");

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
            Core.Equip(new[]{"Void Highlord", "Cape of Awe", "Necrotic Sword of Doom" } );
            Core.Equip("Potent Revitalize Elixir");
            Core.Equip("Scroll of Enrage"); // Use at every 2 mil hp, skip 10,2 mil
            Bot.Skills.StartAdvanced("4S | 3S | 1S | 2S");
        } 
        else if (Bot.Player.Username == players[1])
        {
            Core.Equip(new[]{ "ArchPaladin", "Cape of Awe" });
            Core.Equip("Potent Revitalize Elixir");
            Core.Equip("Felicitous Philtre");
            // Bot.Skills.StartAdvanced("5S | 3 | 1 | 2");
            Bot.Skills.StartAdvanced("5S | 3 | 1 | 2 | 1 | 2 | 4 | 1");
        } 
        else if (Bot.Player.Username == players[2])
        {
            Core.Equip(new[]{ "Lord of Order", "Cape of Awe", "Malgor's ShadowFlame Blade" });
            Core.Equip("Potent Revitalize Elixir");
            Core.Equip("Felicitous Philtre");
            Bot.Skills.StartAdvanced("5S | 2S | 4S | 1S | 3S");
        }
        else if (Bot.Player.Username == players[3])
        {
            Core.Equip(new[]{ "Stonecrusher", "Cape of Awe" });
            Core.Equip("Potent Revitalize Elixir");
            Core.Equip("Felicitous Philtre");
            Bot.Skills.StartAdvanced("5S | 2S | 3S | 1S | 4S");
        }
        // Use Felicitous Philtre on all at 2.5m to burst

        Core.Join("championdrakath", "r2", "Left");
        Core.EnsureAccept(8300);
        Army.waitForParty("championdrakath");

        Monster? monster = Bot.Monsters.CurrentMonsters?.Find(m => m.MapID == 1);
        if (monster == null)
        {
            Core.Logger($"Monster not found. Something is wrong. Stopping bot", messageBox: true, stopBot: true);
            return;
        }
        while (!Bot.ShouldExit && !Core.CheckInventory("Champion Drakath Defeated"))
        {
            if (Bot.Player.Username == players[0])
            {
                monster = Bot.Monsters.CurrentMonsters?.Find(m => m.MapID == 1);
                if ((monster.HP <= 18200000 && monster.HP >= 18000000) || 
                    (monster.HP <= 16300000 && monster.HP >= 16000000) || 
                    (monster.HP <= 14300000 && monster.HP >= 14000000) || 
                    (monster.HP <= 12300000 && monster.HP >= 12000000) || 
                    (monster.HP <= 8150000 && monster.HP >= 8000000) || 
                    (monster.HP <= 6150000 && monster.HP >= 6000000) || 
                    (monster.HP <= 4150000 && monster.HP >= 4000000))
                {
                    Core.Logger($"Drakath HP: {monster.HP}, Preparing to Taunt");
                    Bot.Skills.StartAdvanced("5");
                }
                else
                {
                    Bot.Skills.StartAdvanced("4S | 3S | 1S | 2S");
                }
                
            }
            
            Bot.Combat.Attack("Champion Drakath");
            Bot.Sleep(1000);
        }

        Adv.KillUltra("championdrakath", "r2", "Left", "Champion Drakath", "Champion Drakath Defeated", publicRoom: false);
        Core.EnsureComplete(8300);
    }
}