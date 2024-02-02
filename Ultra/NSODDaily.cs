/*
name: Army Necrotic Sword of Doom Daily
description: uses an army to farm enroaching shadows daily.
tags: nsod, nsod daily, daily, army, enroaching shadows
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

public class NSoDDaily
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    public CoreFarms Farm = new();
    public CoreAdvanced Adv => new();
    private CoreArmyLite Army = new();
    private static CoreBots sCore = new();
    private static CoreArmyLite sArmy = new();

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
        Core.BankingBlackList.AddRange(new[] { "Glacial Pinion", "Hydra Eyeball", "Flibbitigiblets", "Void Aura" });
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

        Core.AddDrop("Glacial Pinion", "Hydra Eyeball", "Flibbitigiblets", "Void Aura");

        Core.EnsureAccept(9091); // Originul daily
        Core.EnsureAccept(8653);
        // Core.RegisterQuests(9091, 8653);

        // Turn off antilag
        Bot.Options.LagKiller = false;
        //Bot.Flash.SetGameObject("stage.frameRate", 30);
        if (Bot.Flash.GetGameObject<bool>("ui.monsterIcon.redX.visible"))
            Bot.Flash.CallGameFunction("world.toggleMonsters");

        //Army.waitForParty("Icestormarena", "Glacial Pinion");
        // while (!Bot.ShouldExit && !Core.CheckInventory("Glacial Pinion"))
        // {
        ArmyEquip();
        Army.waitForParty("icewing");
        
        Core.HuntMonster("icewing", "Warlord Icewing", "Glacial Pinion", isTemp: false, publicRoom: false);
        Bot.Sleep(1000);
        // }
            
        Core.JumpWait();
        Army.waitForParty("voidflibbi", "Glacial Pinion");
        // while (!Bot.ShouldExit && !Core.CheckInventory("Hydra Eyeball", 3))
        // {
        Adv.KillUltra("voidflibbi", "Enter", "Spawn", "Flibbitiestgibbet", publicRoom: false);
        // Core.HuntMonster("voidflibbi", "Flibbitiestgibbet", "Flibbitigiblets", 1, isTemp: false);
        // Core.HuntMonster("voidflibbi", "Flibbitiestgibbet");
        // }

        Core.JumpWait();
        Army.waitForParty("hydrachallenge", "Flibbitigiblets");
        Core.Equip("Void Highlord");
        
        // while (!Bot.ShouldExit && !Core.CheckInventory("Flibbitigiblets"))
        // {
        Core.HuntMonster("hydrachallenge", "Hydra Head 90", "Hydra Eyeball", 3, isTemp: false);
        //Adv.KillUltra("voidflibbi", "Enter", "Spawn", "Flibbitiestgibbet", "Flibbitigiblets", 1, publicRoom: false);
        // }

        Core.JumpWait();

        Core.EnsureComplete(8653);
    }

    private void ArmyEquip()
    {
        string[] players = Army.Players();
        if (Bot.Player.Username == players[0])
        {
            Core.Equip("Legion Revenant");
            Bot.Skills.StartAdvanced("3 | 4 | 1 | 2");
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
            Bot.Skills.StartAdvanced("2 | 4 | 1 | 3");
        }
        else if (Bot.Player.Username == players[3])
        {
            // Core.Equip("StoneCrusher");
            // Bot.Skills.StartAdvanced("3S | 2S | 1 | 4");
            Core.Equip("Chaos Avenger");
            Bot.Skills.StartAdvanced("4 | 3 | 1 | 2");
        }
    }

}