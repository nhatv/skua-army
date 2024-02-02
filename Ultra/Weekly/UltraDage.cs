/*
name: Ultra Dage
description: Army for Ultra Dage weekly
tags: ultra, weekly, army, legion
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Tools/AutoAttackWithMove.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Auras;
using Skua.Core.Interfaces.Auras;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class UltraDage
{
    public IScriptInterface Bot => IScriptInterface.Instance;

    public CoreBots Core => CoreBots.Instance;
    public CoreAdvanced Adv = new();
    public CoreFarms Farm = new();
    private static CoreArmyLite sArmy = new();
    private CoreArmyLite Army = new();
    private AAWithMove AAWMove = new();
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
        //Core.SetOptions();

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
        Bot.Flash.SetGameObject("stage.frameRate", 30);
        if (Bot.Flash.GetGameObject<bool>("ui.monsterIcon.redX.visible"))
            Bot.Flash.CallGameFunction("world.toggleMonsters");

        string[] players = Army.Players();
        if (Bot.Player.Username == players[0])
        {
            //Core.Equip(new[]{"Legion Revenant", "Dazzling Daredevil Visage", "ShadowFiend Cloak", "Necrotic Sword of the Abyss" } );
            Core.Equip(new[]{ "Legion Revenant", "Cape of Awe", "Necrotic Blade of the Underworld" });
            Core.Equip("Felicitous Philtre");
            Core.Equip("Scroll of Life Steal");
            Bot.Skills.StartAdvanced("5 H<70S | 4S | 1S | 2S");
        } 
        else if (Bot.Player.Username == players[1])
        {
            Core.Equip(new[]{ "Chaos Avenger", "Cape of Awe", "Sin of the Abyss" });
            Core.Equip("Felicitous Philtre");
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("4S | 1S | 2S");
        } 
        else if (Bot.Player.Username == players[2])
        {
            Core.Equip(new[]{ "Lord of Order", "Awescended Omni Cowl", "Cape of Awe", "Dual Exalted Apotheosis" });
            Core.Equip("Felicitous Philtre");
            Core.Equip("Scroll of Life Steal");
            Bot.Skills.StartAdvanced("5 H<70S | 4S | 3S | 1S");
        }
        else if (Bot.Player.Username == players[3])
        {
            Core.Equip(new[]{ "Verus DoomKnight", "Cape of Awe", "Sin of the Abyss" });
            Core.Equip("Scroll of Enrage");
            Bot.Sleep(3000);
            Core.Equip("Felicitous Philtre");
            Bot.Skills.StartAdvanced("5S | 1S | 2S | 3S | 1S | 2S | 5S | 4 | 3S | 2S | 1S | 3S | 2S | 5S | 4");
            //Bot.Skills.StartAdvanced("5S | 2S | 3S | 1S | 4S");
        }

        Core.EnsureAccept(8547);
        Army.waitForParty("ultradage");

        AttackMoveDage(players);

        //Adv.KillUltra("ultradage", "Boss", "Right", "Dage the Dark Lord", "Dage the Dark Lord Defeated", publicRoom: false);
        Core.EnsureComplete(8547);
    }

    public void AttackMoveDage(string[] players)
    {
        Core.AddDrop("Dage the Evil Insignia");
        Core.Jump("Boss", "Right");
        Bot.Player.SetSpawnPoint();
        Bot.Events.RunToArea += moveUltraDage;

        while (!Bot.ShouldExit && !Core.CheckInventory("Dage the Dark Lord Defeated"))
        {
            if (Bot.Player.Username == players[2])
            {
                if (!Bot.Self.HasActiveAura("Noxious Decay") && Bot.Player.Health != Bot.Player.MaxHealth)
                {
                    Bot.Skills.StartAdvanced("2");
                }
                else
                {
                    Bot.Skills.StartAdvanced("5 H<70S | 4S | 3S | 1S");
                }
            }
            Bot.Combat.Attack("*");
            Core.Sleep(1000);
        }

        void moveUltraDage(string zone)
        {
            switch (zone.ToLower())
            {
                case "a":
                    //Move to the left
                    // _walk((40, 175), (400, 410));
                    // _walk((110, 111), (403, 408));
                    Bot.Flash.Call("walkTo", 111, 405, 10);
                    break;
                case "b":
                    //Move to the right
                    // _walk((760, 930), (410, 415));
                    // _walk((830, 831), (408, 408));
                    Bot.Flash.Call("walkTo", 830, 405, 10);
                    break;
                default:
                    //Move to the center
                    // _walk((480, 500), (300, 420));
                    // _walk((440, 464), (400, 420));
                    Bot.Flash.Call("walkTo", 450, 405, 10);
                    break;
            }
        }
    }
}