/*
name: Temple Shrine Daily
description: Army for Temple Shrine daily
tags: ultra, daily, army
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Army/Ultra/CoreUltra.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Models.Auras;
using Skua.Core.Models.Players;
using Skua.Core.Models.Monsters;
using Skua.Core.Options;

public class TempleShrineFinalBoss
{
    public IScriptInterface Bot => IScriptInterface.Instance;

    public CoreBots Core => CoreBots.Instance;
    public CoreAdvanced Adv = new();
    public CoreFarms Farm = new();
    public CoreDailies Daily = new();
    private static CoreArmyLite sArmy = new();
    private CoreArmyLite Army = new();
    public CoreUltra Ultra = new();
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
        Core.BankingBlackList.AddRange(new[] { "Sliver of Moonlight", "Sliver of Sunlight", "Ecliptic Offering", "Rite of Ascension" });
        Core.SetOptions(disableClassSwap: true);
        // Core.SetOptions();
        Core.Sleep(2000);
        Ultra.AntiLagOff();

        DoDaily();

        Core.SetOptions(false);
    }

    bool start = false;
    public void DoDaily()
    {
        Core.OneTimeMessage("Only for army", "This is intended for use with an army, not for solo players.");

        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = Army.getRoomNr();

        Core.AddDrop("Sliver of Moonlight", "Sliver of Sunlight", "Ecliptic Offering", "Rite of Ascension");

        string[] players = Army.Players();
        // Ultra.UseRevitalize();
        // Ultra.PartySetup(players);
        EquipDungeon(players);
        Bot.Options.AttackWithoutTarget = true;

        // Core.ActivateDungeonMonsterListener();
        
        // Cannot hit boss without killing all the enemies
        //Night Falls (Daily Bonus) - Sliver of Moonlight
        if (!Core.CheckInventory("Rite of Ascension"));
            Core.BuyItem("templeshrine", 2303, "Rite of Ascension");

        //Frozen Cycle (Daily Bonus) - Ecliptic Offering
        if (Daily.CheckDaily(9305))
        {
            Core.EnsureAccept(9305);

            // Army.waitForParty("ascendeclipse");
            Core.Equip(new[] {"Potent Revitalize Elixir", "Scroll of Enrage"});
            // if (Bot.Player.Username == players[0] || Bot.Player.Username == players[2]) // LR and loo whenever its up
            Ultra.SkillsConfig(true);

            Core.Jump("r3", "Left");
            Ultra.SkillsConfig(); // Turn off taunt spam for the listeners
            if (Bot.Player.Username == players[0] || Bot.Player.Username == players[2]) // LR and loo
                Core.Sleep(7000);
            if (Bot.Player.Username == players[0] || Bot.Player.Username == players[3]) // LR and dps
            {
                // Bot.Events.ExtensionPacketReceived += SunListener;
                KillUltrav2("ascendeclipse", "r3", "Left", "Ascended Solstice");
            }
            // else
                // Bot.Events.ExtensionPacketReceived += MoonListener;
            Core.Sleep();
            KillUltrav2("ascendeclipse", "r3", "Left", "Ascended Midnight");
            // Bot.Target.GetAura("Focus").SecondsRemaining() < 2

            Core.EnsureComplete(9305);
            Bot.Wait.ForPickup("Ecliptic Offering");
        }

        // Unsub to all listeners
        // Core.ActivateDungeonMonsterListener(false);
        Bot.Events.ExtensionPacketReceived -= Army.PartyManagement;
        if (Bot.Player.Username == players[0] || Bot.Player.Username == players[2]) // LR and loo
            Bot.Events.ExtensionPacketReceived -= SunListener;
        else
            Bot.Events.ExtensionPacketReceived -= MoonListener;


        async void SunListener(dynamic packet)
        {
            string type = packet["params"].type;
            dynamic data = packet["params"].dataObj;
            if (type is not null and "json")
            {
                string cmd = data.cmd.ToString();
                switch (cmd)
                {
                    case "ct":
                        dynamic anims = data.anims?[0]!;
                        if (anims is not null)
                        {
                            string msg = anims["msg"];
                            if (msg is not null && msg.Contains("sun converges")){
                                if (start)
                                {
                                    Core.Logger($"{msg}");
                                    Bot.Sleep(800);
                                    Bot.Skills.UseSkill(5);
                                }
                                else
                                    Core.Logger("Not my turn");
                                start = !start;
                            }
                        }
                        break;
                }
            }
        }
        async void MoonListener(dynamic packet)
        {
            string type = packet["params"].type;
            dynamic data = packet["params"].dataObj;
            if (type is not null and "json")
            {
                string cmd = data.cmd.ToString();
                switch (cmd)
                {
                    case "ct":
                        dynamic anims = data.anims?[0]!;
                        if (anims is not null)
                        {
                            string msg = anims["msg"];
                            if (msg is not null && msg.Contains("moon converges")){
                                if (start)
                                {
                                    Core.Logger($"{msg}");
                                    Bot.Sleep(800);
                                    Bot.Skills.UseSkill(5);
                                }
                                else
                                    Core.Logger("Not my turn");
                                start = !start;
                            }
                        }
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Modified version of Adv.KillUltra. Joins a map, jump & set the spawn point and kills the specified monster. But also listens for Counter Attacks
    /// </summary>
    /// <param name="map">Map to join</param>
    /// <param name="cell">Cell to jump to</param>
    /// <param name="pad">Pad to jump to</param>
    /// <param name="monster">Name of the monster to kill</param>
    public void KillUltrav2(string map, string cell, string pad, string monsterName)
    {
        string[] players = Army.Players();
        Core.Join(map, cell, pad);
        Core.Jump(cell, pad);

        Monster? monster = Bot.Monsters.CurrentMonsters?.Find(m => m.Name == monsterName);
        Core.Logger($"Killing Ultra-Boss {monster}");
        bool ded = false;
        Bot.Events.MonsterKilled += b => ded = true;
        while (!Bot.ShouldExit && !ded)
        {
            monster = Bot.Monsters.CurrentMonsters?.Find(m => m.Name == monsterName);
            
            Bot.Combat.Attack(monsterName);
            Bot.Sleep(1000);
            if (monsterName == "Fallen Star")
            {
                while (Bot.Self.HasActiveAura("Solar Flare"))
                    Bot.Combat.Attack("Blessless Deer");
            }
            if (monster is null)
                return;
        }
    }

    private void EquipDungeon(string[] players)
    {
        if (Bot.Player.Username == players[0])
        {
            Core.Equip("Legion Revenant");
            Core.Equip("Cape of Awe"); // Penitence Cape
        } 
        else if (Bot.Player.Username == players[1])
        {
            Core.Equip("ArchPaladin");
            Core.Equip("Cape of Awe"); // Penitence Cape
        } 
        else if (Bot.Player.Username == players[2])
        {
            Core.Equip("Lord of Order");
        }
        else if (Bot.Player.Username == players[3])
        {
            // Healer gear
            // Core.Equip("Dragon of Time");
            // Core.Equip("Awescended Omni Cowl");
            // Core.Equip("Category Five Hurricane Cloud");
            // Core.Equip("Exalted Apotheosis");

            Core.Equip("Chaos Avenger");
            // Bot.Skills.StartAdvanced("4 | 3 | 1 | 2");
        }
        Ultra.SkillsConfig();
    }
}