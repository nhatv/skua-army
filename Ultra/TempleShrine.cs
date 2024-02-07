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
using Skua.Core.Options;

public class TempleShrineDaily
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

        DoDaily();

        Core.SetOptions(false);
    }

    public void DoDaily()
    {
        Core.OneTimeMessage("Only for army", "This is intended for use with an army, not for solo players.");

        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = Army.getRoomNr();

        Core.AddDrop("Sliver of Moonlight", "Sliver of Sunlight", "Ecliptic Offering", "Rite of Ascension");

        string[] players = Army.Players();
        // Ultra.UseRevitalize();
        Ultra.PartySetup(players);
        EquipDungeon(players);
        Ultra.AntiLagOff();
        
        // Cannot hit boss without killing all the enemies
        //Night Falls (Daily Bonus) - Sliver of Moonlight
        if (Daily.CheckDaily(9303))
        {
            if (Army.isPartyLeader())
                Core.SendPackets($"%xt%zm%dungeonQueue%{Bot.Map.RoomID}%solsticemoon%");
            Core.EnsureAccept(9303);
            Adv.KillUltra("solsticemoon", "Enter", "Spawn", "Faithless Deer");
            Adv.KillUltra("solsticemoon", "Enter", "Spawn", "Shackled Fairy");
            Core.Jump("r1", "Left");
            Adv.KillUltra("solsticemoon", "r1", "Left", "Lunar Haze");
            Adv.KillUltra("solsticemoon", "r1", "Left", "Faithless Deer");
            Core.Jump("r2", "Right");
            Adv.KillUltra("solsticemoon", "r2", "Right", "Lunar Haze");
            Adv.KillUltra("solsticemoon", "r2", "Right", "Shackled Fairy");
            Core.Jump("r3", "Right");
            Adv.KillUltra("solsticemoon", "r3", "Right", "Hollow Midnight");
            Core.EnsureComplete(9303);
            Bot.Wait.ForPickup("Sliver of Moonlight");
        }

        //Dawn Breaks (Daily Bonus) - Sliver of Sunlight
        if (Daily.CheckDaily(9304))
        {
            if (Army.isPartyLeader())
                Core.SendPackets($"%xt%zm%dungeonQueue%{Bot.Map.RoomID}%midnightsun%");
            Core.EnsureAccept(9304);
            Adv.KillUltra("midnightsun", "Enter", "Spawn", "Dying Light");
            Adv.KillUltra("midnightsun", "Enter", "Spawn", "Shining Star");
            Core.Jump("r1", "Left");
            Adv.KillUltra("midnightsun", "r1", "Left", "Dawn Knight");
            Adv.KillUltra("midnightsun", "r1", "Left", "Shining Star");
            Core.Jump("r2", "Right");
            Adv.KillUltra("midnightsun", "r2", "Right", "Dawn Knight");
            Adv.KillUltra("midnightsun", "r2", "Right", "Dying Light");
            Core.Jump("r3", "Left");
            Adv.KillUltra("midnightsun", "r3", "Left", "Hollow Solstice");
            Core.EnsureComplete(9304);
            Bot.Wait.ForPickup("Sliver of Sunlight");
        }

        if (!Core.CheckInventory("Rite of Ascension"));
            Core.BuyItem("templeshrine", 2303, "Rite of Ascension");

        //Frozen Cycle (Daily Bonus) - Ecliptic Offering
        if (Daily.CheckDaily(9305))
        {
            Core.EnsureAccept(9305);

            // Core.Join("ascendeclipse");
            if (Army.isPartyLeader())
                Core.SendPackets($"%xt%zm%dungeonQueue%{Bot.Map.RoomID}%ascendeclipse%");

            // Army.waitForParty("ascendeclipse");
            Core.Equip(new[] {"Potent Revitalize Elixir", "Scroll of Enrage"});
            if (Bot.Player.Username == players[0])
                Ultra.SkillsConfig(true);

            // doing dungeon without party makes them attack fast
            KillUltrav2("ascendeclipse", "Enter", "Spawn", "Fallen Star");
            KillUltrav2("ascendeclipse", "Enter", "Spawn", "Blessless Deer");

            Core.Jump("r1", "Left");
            Ultra.SkillsConfig(true);
            KillUltrav2("ascendeclipse", "r1", "Left", "Suffocated Light");
            KillUltrav2("ascendeclipse", "r1", "Left", "Imprisoned Fairy");

            if (Bot.Player.Username == players[1] || Bot.Player.Username == players[2])
                Bot.Events.ExtensionPacketReceived += SunListener;
            Core.Jump("r2", "Left");
            if (Bot.Player.Username == players[0] || Bot.Player.Username == players[2]) // LR and Loo
                KillUltrav2("ascendeclipse", "r2", "Left", "Moon Haze");
            KillUltrav2("ascendeclipse", "r2", "Left", "Sunset Knight");
            if (Bot.Player.Username == players[1] || Bot.Player.Username == players[2])
                Bot.Events.ExtensionPacketReceived -= SunListener;

            Core.Jump("r3", "Left");
            if (Bot.Player.Username == players[0] || Bot.Player.Username == players[3]) // LR and other dps
                KillUltrav2("ascendeclipse", "r3", "Left", "Ascended Solstice");
            KillUltrav2("ascendeclipse", "r3", "Left", "Ascended Midnight");
            ////////////////////////////////////////////////////////////////////////////////////////
            // while (!Bot.ShouldExit && !Core.CheckInventory("Ecliptic Offering"))
            // {
            //     if (Bot.Player.Username == players[0] || Bot.Player.Username == players[3]) // LR and Cav
            //     {
            //         while (Core.IsMonsterAlive("Ascended Solstice"))
            //         {
            //             if (Bot.Player.Username == players[0] && (!Bot.Target.HasActiveAura("Focus") || Bot.Target.GetAura("Focus").SecondsRemaining() < 2))
            //                 Bot.Skills.UseSkill(5);
            //             Bot.Combat.Attack("Ascended Solstice");
            //             Bot.Sleep(1000);
            //         }
                    
            //     }
            //     if (Bot.Player.Username == players[2] && !Bot.Target.HasActiveAura("Focus"))
            //         Bot.Skills.UseSkill(5);
            //     Bot.Combat.Attack("Ascended Midnight");
            //     Bot.Sleep(1000);
            // }
            Core.EnsureComplete(9305);
            Bot.Wait.ForPickup("Ecliptic Offering");
        }
        Bot.Events.ExtensionPacketReceived -= Army.PartyManagement;

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
                        if (data.anims is not null)
                        {
                            foreach (var a in data.anims)
                            {
                                if (a is not null && (string)a["msg"] is "The sun's heat radiates!"){
                                    Core.Logger($"{(string)a["msg"]}");
                                    Bot.Sleep(1500);
                                    Bot.Skills.StartAdvanced("3 | 1 | 4");
                                }
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
    public void KillUltrav2(string map, string cell, string pad, string monster, bool tauntLoop = true)
    {
        string[] players = Army.Players();
        Core.Join(map, cell, pad);
        Core.Jump(cell, pad);

        Core.Logger($"Killing Ultra-Boss {monster}");
        bool ded = false;
        Bot.Events.MonsterKilled += b => ded = true;
        while (!Bot.ShouldExit && !ded)
        {
            Bot.Combat.Attack(monster);
            if (!Core.IsMonsterAlive(monster))
                return;
            if (monster == "Fallen Star")
            {
                while (Bot.Self.HasActiveAura("Solar Flare"))
                    Bot.Combat.Attack("Blessless Deer");
            }
            if (tauntLoop)
            {
                if (Bot.Target is not null)
                {
                    while (!Bot.Target.HasActiveAura("Focus"))
                    {
                        Ultra.SkillsConfig(true);
                    }
                    Ultra.SkillsConfig(false);
                }
            }
            Bot.Sleep(1000);
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

    // private void EquipMidDungeon()
    // {
    //     string[] players = Army.Players();
    //     if (Bot.Player.Username == players[0])
    //     {
    //         Core.Equip("Legion Revenant");
    //         Core.Equip("Cape of Awe"); // Penitence Cape
    //         Core.Equip("Necrotic Sword of Doom");
    //         Bot.Skills.StartAdvanced("3S | 4S | 1S | 2S");
    //         Bot.Options.LagKiller = false;
    //     } 
    //     else if (Bot.Player.Username == players[1])
    //     {
    //         Core.Equip("ArchPaladin");
    //         Core.Equip("Cape of Awe"); // Penitence Cape
    //         Core.Equip("Dual Exalted Apotheosis");
    //         // Bot.Skills.StartAdvanced("3S | 2S | 1S");
    //         Bot.Skills.StartAdvanced("3 | 1 | 2 | 1 | 2 | 4 | 1", 250, SkillUseMode.WaitForCooldown);
    //     } 
    //     else if (Bot.Player.Username == players[2])
    //     {
    //         Core.Equip("Lord of Order");
    //         Core.Equip("Cape of Awe"); // Penitence Cape
    //         Core.Equip("Dual Exalted Apotheosis");
    //         Bot.Skills.StartAdvanced("2 | 4 | 1 | 3");
    //     }
    //     else if (Bot.Player.Username == players[3])
    //     {
    //         // Healer gear
    //         Core.Equip("Dragon of Time");
    //         Core.Equip("Awescended Omni Cowl");
    //         Core.Equip("Category Five Hurricane Cloud");
    //         Core.Equip("Exalted Apotheosis");
    //         Bot.Skills.StartAdvanced("1 | 2 | 4 | 2 | 3 | 2");

    //         // Core.Equip("Chaos Avenger");
    //         // Bot.Skills.StartAdvanced("4 | 3 | 1 | 2");
    //         // Core.Equip("StoneCrusher");
    //         // Core.Equip("Cape of Awe"); // Penitence Cape
    //         // Bot.Skills.StartAdvanced("2 | 3 | 1 | 4");
    //     }
    // }
}