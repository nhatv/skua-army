/*
name: Ultra Ezrajal
description: Army for Ultra Ezrajal daily
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

public class UltraEzrajal
{
    public IScriptInterface Bot => IScriptInterface.Instance;

    public CoreBots Core => CoreBots.Instance;
    public CoreAdvanced Adv = new();
    public CoreFarms Farm = new();
    private static CoreArmyLite sArmy = new();
    private CoreArmyLite Army = new();
    public CoreDailies Daily = new();
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
            Bot.Skills.StartAdvanced("3 | 4 | 1 | 2", 250, SkillUseMode.UseIfAvailable);
        } 
        else if (Bot.Player.Username == players[1])
        {
            Core.Equip("ArchPaladin");
            //Bot.Skills.StartAdvanced("3 | 1 | 2");
            Bot.Skills.StartAdvanced("3 | 1 | 2 | 1 | 2 | 4 | 1", 250, SkillUseMode.WaitForCooldown);
        } 
        else if (Bot.Player.Username == players[2])
        {
            Core.Equip("Lord of Order");
            Bot.Skills.StartAdvanced("2 | 4 | 1 | 3", 250, SkillUseMode.UseIfAvailable);
        }
        else if (Bot.Player.Username == players[3])
        {
            Core.Equip("Verus DoomKnight");
            Bot.Skills.StartAdvanced("1 | 2 | 3 | 1 | 2 | 4 | 3 | 2 | 1 | 3 | 2 | 4", 250, SkillUseMode.WaitForCooldown);
        }
        Core.Equip("Cape of Awe"); // Penitence Cape

        //Core.Join("ultraezrajal", "r2", "Left");
        Core.EnsureAccept(8152);
        Army.waitForParty("ultraezrajal");
        Adv.KillUltra("ultraezrajal", "r2", "Left", "Ultra Ezrajal", "Ultra Ezrajal Defeated", publicRoom: false);
        Core.EnsureComplete(8152);
    }
}