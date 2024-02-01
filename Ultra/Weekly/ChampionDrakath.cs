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
//cs_include Scripts/Army/Ultra/CoreUltra.cs
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
    public CoreUltra Ultra = new();
    public string OptionsStorage = "Ultra";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new List<IOption>()
    {
        new Option<int>("threshold", "HP Threshold", "Player 1 uses taunt when HP is within the threshold, value is halved < 10 mil HP. (Default: 300000)", 300000),
        new Option<bool>("spamFeli", "Spam Felicitous Philtre?", "Yes(True) / No(False)", true),
        sArmy.player1,
        new Option<string>("player1Class", "Account #1 Class", "Enter the name of class to equip. (Default: Void Highlord)", "Void Highlord"),
        sArmy.player2,
        new Option<string>("player2Class", "Account #2 Class", "Enter the name of class to equip. (Default: ArchPaladin)", "ArchPaladin"),
        sArmy.player3,
        new Option<string>("player3Class", "Account #3 Class", "Enter the name of class to equip. (Default: Void Lord of Order)", "Lord of Order"),
        sArmy.player4,
        new Option<string>("player4Class", "Account #4 Class", "Enter the name of class to equip. (Default: Void StoneCrusher)", "StoneCrusher"),
        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions
    };

    public void ScriptMain(IScriptInterface bot)
    {
        //Core.SetOptions(disableClassSwap: true);
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
        Bot.Options.LagKiller = Bot.Options.LagKiller ? false : true;
        Bot.Flash.SetGameObject("stage.frameRate", 30);
        if (Bot.Flash.GetGameObject<bool>("ui.monsterIcon.redX.visible"))
            Bot.Flash.CallGameFunction("world.toggleMonsters");

        string[] players = Army.Players();
        if (Bot.Player.Username == players[0])
        {
            Core.Equip(new[]{ Bot.Config.Get<string>("player1Class"), "Potent Revitalize Elixir", "Scroll of Enrage" });
            if (Bot.Player.CurrentClass.Name == "Void Highlord")
                Bot.Skills.StartAdvanced("3 | 4 | 1 | 2");
            else
                Bot.Skills.StartAdvanced(Bot.Player.CurrentClass?.Name ?? "generic", false);
        } 
        else if (Bot.Player.Username == players[1])
        {
            Core.Equip(new[]{ Bot.Config.Get<string>("player2Class"), "Potent Revitalize Elixir", "Felicitous Philtre" });
            // Bot.Skills.StartAdvanced("3 | 1 | 2 | 1 | 2 | 4 | 1", 250, SkillUseMode.WaitForCooldown);
            if (Bot.Player.CurrentClass.Name == "ArchPaladin")
                Bot.Skills.StartAdvanced("3 | 1 | 2");
            else
                Bot.Skills.StartAdvanced(Bot.Player.CurrentClass?.Name ?? "generic", false);
        } 
        else if (Bot.Player.Username == players[2])
        {
            Core.Equip(new[]{ Bot.Config.Get<string>("player3Class"), "Potent Revitalize Elixir", "Felicitous Philtre" });
            if (Bot.Player.CurrentClass.Name == "Lord of Order")
                Bot.Skills.StartAdvanced("2 | 4 | 1 | 3");
            else
                Bot.Skills.StartAdvanced(Bot.Player.CurrentClass?.Name ?? "generic", false);
        }
        else if (Bot.Player.Username == players[3])
        {
            Core.Equip(new[]{ Bot.Config.Get<string>("player4Class"), "Potent Revitalize Elixir", "Felicitous Philtre" });
            if (Bot.Player.CurrentClass.Name == "StoneCrusher")
                Bot.Skills.StartAdvanced("2 | 3 | 1 | 4");
            else
                Bot.Skills.StartAdvanced(Bot.Player.CurrentClass?.Name ?? "generic", false);
        }

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
                if (monster == null)
                    continue;
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