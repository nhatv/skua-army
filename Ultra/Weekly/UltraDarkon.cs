/*
name: Ultra Darkon
description: Army for Ultra Darkon weekly
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

public class UltraDarkon
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
            // arcana
            Core.Equip(new[]{"Legion Revenant", "Necrotic Sword of Doom" } );
            UseRevitalize();
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("5S | 3S | 4S | 1S | 2S");
        } 
        else if (Bot.Player.Username == players[1])
        {
            // Healer gear w/ health vamp
            Core.Equip(new[]{"Lord of Order", "Awescended Omni Cowl", "Corrupted BattleMage Cape", "Dual Exalted Apotheosis" } );
            UseRevitalize();
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("4S | 2S | 1S | 3S | 5S");
            // Manual: Stop using 4th skill below 9m
            // Use 4th skill between 4.5m and 4.7m
        } 
        else if (Bot.Player.Username == players[2])
        {
            // elysium
            Core.Equip(new[]{"LightCaster", "Exalted Apotheosis" } );
            UseRevitalize();
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("3S | 5S | 4S | 1S | 2S");
        }
        else if (Bot.Player.Username == players[3])
        {
            // valiance
            Core.Equip("Void Highlord");
            UseRevitalize();
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("5S | 3S | 4S | 1S | 2S");
        }
        // must use revitalize to survive

        Core.EnsureAccept(8746);
        
        Core.Join("ultradarkon", "r2", "Left");
        Army.waitForParty("ultradarkon");

        Monster? monster = Bot.Monsters.CurrentMonsters?.Find(m => m.ID == 5167);
        if (monster == null)
        {
            Core.Logger($"Monster not found. Something is wrong. Stopping bot", messageBox: true, stopBot: true);
            return;
        }

        bool thirdPhase = false;
        while (!Bot.ShouldExit && !Core.CheckInventory("Darkon the Conductor Defeated"))
        {
            if (Bot.Player.Username == players[1])
            {
                monster = Bot.Monsters.CurrentMonsters?.Find(m => m.ID == 5167);
                if (monster.HP < 9500000 && monster.HP != 0 && !thirdPhase)
                {
                    Core.Logger($"Darkon HP: {monster.HP}, Stopping 4th skill");
                    Bot.Skills.StartAdvanced("2S | 1S | 3S | 5S");
                }
                if (monster.HP < 4800000 && monster.HP != 0 && !thirdPhase)
                {
                    Core.Logger($"Darkon HP: {monster.HP}, Resuming 4th skill");
                    Bot.Skills.StartAdvanced("4S | 2S | 1S | 3S | 5S");
                    thirdPhase = true;
                }
                Core.Logger($"Darkon HP: {monster.HP}");
            }
            
            Bot.Combat.Attack("Darkon the Conductor");
            Bot.Sleep(1000);
        }

        // Adv.KillUltra("ultradarkon", "r2", "Left", "Darkon the Conductor", "Darkon the Conductor Defeated", publicRoom: false);
        
        Core.EnsureComplete(8746);

    }

    private void UseRevitalize()
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

}