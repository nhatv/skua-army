/*
name: Ultra Drakath
description: Army for Champion Drakath weekly
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

public class ChampionDrakath
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
        new Option<int>("threshold", "HP Threshold", "Enter HP threshold to taunt (Default: 300000)", 300000),
        sArmy.player1,
        new Option<string>("player1class", "Account #1 Class", "Enter the name of class to equip.", "Legion Revenant"),
        sArmy.player2,
        new Option<string>("player2class", "Account #2 Class", "Enter the name of class to equip.", "ArchPaladin"),
        sArmy.player3,
        new Option<string>("player3class", "Account #3 Class", "Enter the name of class to equip.", "Lord of Order"),
        sArmy.player4,
        new Option<string>("player4class", "Account #4 Class", "Enter the name of class to equip.", "StoneCrusher"),
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
        Bot.Options.LagKiller = Bot.Options.LagKiller ? false : true;
        Bot.Flash.SetGameObject("stage.frameRate", 30);
        if (Bot.Flash.GetGameObject<bool>("ui.monsterIcon.redX.visible"))
            Bot.Flash.CallGameFunction("world.toggleMonsters");

    string[] players = Army.Players();

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
                if ((monster.HP <= 18300000 && monster.HP >= 18000000) || 
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