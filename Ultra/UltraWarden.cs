/*
name: Ultra Warden
description: Army for Ultra Warden daily
tags: ultra, daily, army
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Army/CoreArmyLite.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class UltraWarden
{
    public IScriptInterface Bot => IScriptInterface.Instance;

    public CoreBots Core => CoreBots.Instance;
    public CoreAdvanced Adv = new();
    public CoreFarms Farm = new();
    public CoreDailies Daily = new();
    private static CoreArmyLite sArmy = new();
    private CoreArmyLite Army = new();
    public string OptionsStorage = "UltraTimeInn";
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
            Core.Equip("Potent Revitalize Elixir");
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("5S | 3 | 4S | 1 | 2");
        } 
        else if (Bot.Player.Username == players[1])
        {
            // Core.Equip("ArchPaladin");
            // Bot.Skills.StartAdvanced("3S | 1S | 2S");
            //Bot.Skills.StartAdvanced("3 | 1 | 2 | 1 | 2 | 4 | 1", 250, SkillUseMode.WaitForCooldown);
            Core.Equip("StoneCrusher");
            Bot.Skills.StartAdvanced("3S | 2S | 1S | 4S");
        } 
        else if (Bot.Player.Username == players[2])
        {
            // Core.Equip("Lord of Order");
            // Bot.Skills.StartAdvanced("2S | 4S | 1S | 3S");
            Core.Equip("LightCaster");
            Bot.Skills.StartAdvanced("3S | 1S | 2S | 4S");
        }
        else if (Bot.Player.Username == players[3])
        {
            Core.Equip("Chaos Avenger");
            Core.Equip("Potent Revitalize Elixir");
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("5S | 4 | 3 | 1 | 2");
            // Core.Equip("Verus DoomKnight");
            // Core.Equip("Awescended Omni Armblades");
            // Bot.Skills.StartAdvanced("1 | 2 | 3 | 1 | 2 | 4 | 3 | 2 | 1 | 3 | 2 | 4", 250, SkillUseMode.WaitForCooldown);
        }
        Core.Equip("Exalted Apotheosis");

        //Core.Join("ultrawarden", "r2", "Left");
        Core.EnsureAccept(8153);
        Army.waitForParty("ultrawarden");

        while (!Bot.ShouldExit && !Core.CheckInventory("Ultra Warden Defeated"))
        {
            Core.HuntMonster("ultrawarden", "Ultra Warden", "Ultra Warden Defeated", isTemp: true, publicRoom: false);
        }
        Core.EnsureComplete(8153);

    }
}