/*
name: Ultra Speaker
description: Army for Ultra Speaker weekly
tags: ultra, weekly, army
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

public class UltraSpeaker
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
            Core.Equip("Legion Revenant");
            Core.Equip("Necrotic Sword of Doom"); // arcana
            Core.Equip("Potent Revitalize Elixir");
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("5S | 3 | 4S | 1S | 2S");
        } 
        else if (Bot.Player.Username == players[1])
        {
            Core.Equip("ArchPaladin");
            Core.Equip("Necrotic Sword of the Abyss"); // lacerate
            Core.Equip("Potent Revitalize Elixir");
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("3 | 1 | 5S | 2");

            // Core.Equip("Chaos Avenger");
            // Core.Equip("Potent Revitalize Elixir");
            // Core.Equip("Felicitous Philtre");
            // Bot.Skills.StartAdvanced("5S | 4 | 3 | 1 | 2");
        } 
        else if (Bot.Player.Username == players[2])
        {
            Core.Equip("Lord of Order");
            Core.Equip("Malgor's ShadowFlame Blade"); // awe blast
            Core.Equip("Potent Revitalize Elixir");
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("5S | 2S | 4S | 1S | 3S");
        }
        else if (Bot.Player.Username == players[3])
        {
            // Core.Equip("Stonecrusher");
            // Core.Equip("Scroll of Enrage");
            // Bot.Skills.StartAdvanced("5S | 2 | 3 | 1 | 4", 250, SkillUseMode.UseIfAvailable);
            // Core.Equip("Chaos Avenger");
            Core.Equip("Void Highlord");
            // Core.Equip("Sin of the Abyss");
            Core.Equip("Potent Revitalize Elixir");
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("5S | 4 | 3 | 1 | 2");
            // Bot.Skills.StartAdvanced("5S | 1 | 2 | 3 | 1 | 2 | 5S | 4 | 3 | 2 | 1 | 3 | 2 | 5S | 4");
        }
        Core.Equip("Cape of Awe"); // Penitence Cape

        Core.Join("ultraspeaker", "Boss", "Left");
        Core.EnsureAccept(9173);
        Army.waitForParty("ultraspeaker");
        if (Bot.Player.CurrentClass.Name == "Legion Revenant")
        {
            Bot.Events.ExtensionPacketReceived += MalgorListener;
        }
        while (!Bot.ShouldExit && !Core.CheckInventory("The First Speaker Silenced"))
        {
            // Walk top left
            Bot.Flash.Call("walkTo", 63, 325, 10); // x, y, speed
            Bot.Combat.Attack("The First Speaker");
            Bot.Sleep(2000);
        }
        
        Core.EnsureComplete(9173);
        if (Bot.Player.CurrentClass.Name == "Legion Revenant")
        {
            Bot.Events.ExtensionPacketReceived -= MalgorListener;
        }


        async void MalgorListener(dynamic packet)
        {
            string type = packet["params"].type;
            dynamic data = packet["params"].dataObj;
            if (type is not null and "json")
            {
                string cmd = data.cmd.ToString();
                switch (cmd)
                {
                    case "ct":
                        if (data.anims is not null)
                            {
                                foreach (var a in data.anims)
                                {
                                    if (a is not null && (string)a["msg"] is "All stand equal beneath the eyes of the Eternal."){
                                        Bot.Skills.UseSkill(1);
                                    }
                                }
                            }
                            break;
                }
            }
        }
    }

    

}