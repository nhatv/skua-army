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

        // Turn off antilag
        Bot.Options.LagKiller = false;
        Bot.Flash.SetGameObject("stage.frameRate", 10);
        if (Bot.Flash.GetGameObject<bool>("ui.monsterIcon.redX.visible"))
            Bot.Flash.CallGameFunction("world.toggleMonsters");

        Core.AddDrop("Sliver of Moonlight", "Sliver of Sunlight", "Ecliptic Offering", "Rite of Ascension");
        string[] players = Army.Players();
        // Ultra.UseRevitalize();
        Bot.Events.ExtensionPacketReceived += Army.PartyManagement;
        Army.waitForParty("whitemap");
        
        if (Bot.Player.Username == players[0])
        {
            for (int i = 1; i < 4; i++)
            {
                Army.PartyInvite(players[i]);
                Core.Sleep();
            }
        }
        else
            Core.Sleep(5000);

        // Cannot hit boss without killing all the enemies
        //Night Falls (Daily Bonus) - Sliver of Moonlight
        if (Daily.CheckDaily(9303))
        {
            EquipSideDungeon();
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
            EquipSideDungeon();
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
            EquipMidDungeon();
            Core.Equip("Potent Revitalize Elixir");
            Core.Equip("Scroll of Enrage");
            Core.EnsureAccept(9305);

            Core.Join("ascendeclipse");
            // Core.SendPackets($"%xt%zm%dungeonQueue%{Bot.Map.RoomID}%ascendeclipse%");

            // Army.waitForParty("ascendeclipse");

            // private rooms makes them attack too fast
            // work on party invites
            if (Bot.Player.Username == players[1] || Bot.Player.Username == players[3])
                KillUltrav2("ascendeclipse", "Enter", "Spawn", "Fallen Star");
            KillUltrav2("ascendeclipse", "Enter", "Spawn", "Blessless Deer");

            ////////////////////////////////////////////////////////////////////////////////////////
            while (!Bot.ShouldExit && !Core.CheckInventory("Ecliptic Offering"))
            {
                if (Bot.Player.Username == players[0] || Bot.Player.Username == players[3]) // LR and Cav
                {
                    while (Core.IsMonsterAlive("Ascended Solstice"))
                    {
                        if (Bot.Player.Username == players[0] && !Bot.Target.HasActiveAura("Focus"))
                            Bot.Skills.UseSkill(5);
                        Bot.Combat.Attack("Ascended Solstice");
                        Bot.Sleep(1000);
                    }
                    
                }
                if (Bot.Player.Username == players[2] && !Bot.Target.HasActiveAura("Focus"))
                    Bot.Skills.UseSkill(5);
                Bot.Combat.Attack("Ascended Midnight");
                Bot.Sleep(1000);
            }
            Core.EnsureComplete(9305);
            Bot.Wait.ForPickup("Ecliptic Offering");
        }
        Bot.Events.ExtensionPacketReceived -= Army.PartyManagement;
        
        // async void SkyListener(dynamic packet)
        // {
        //     string type = packet["params"].type;
        //     dynamic data = packet["params"].dataObj;
        //     if (type is not null and "json")
        //     {
        //         string cmd = data.cmd.ToString();
        //         switch (cmd)
        //         {
        //             case "ct":
        //                 if (data.anims is not null)
        //                     {
        //                         foreach (var a in data.anims)
        //                         {
        //                             if (a is not null && (string)a["msg"] is "The sky begins to burn."){
        //                                 Bot.Sleep(1000);
        //                                 Core.Logger("Attacking deer");
        //                                 Bot.Combat.Attack("Blessless Deer");
        //                                 Bot.Sleep(5000);
        //                                 Bot.Combat.Attack("Fallen Star");
        //                             }
        //                         }
        //                     }
        //                     break;
        //         }
        //     }
        // }
        

    }

        /// <summary>
    /// Joins a map, jump & set the spawn point and kills the specified monster with the best available race gear. But also listens for Counter Attacks
    /// </summary>
    /// <param name="map">Map to join</param>
    /// <param name="cell">Cell to jump to</param>
    /// <param name="pad">Pad to jump to</param>
    /// <param name="monster">Name of the monster to kill</param>
    /// <param name="item">Item to kill the monster for, if null will just kill the monster 1 time</param>
    /// <param name="quant">Desired quantity of the item</param>
    /// <param name="isTemp">Whether the item is temporary</param>
    /// <param name="log">Whether it will log that it is killing the monster</param>
    public void KillUltrav2(string map, string cell, string pad, string monster)
    {
        Core.Join(map, cell, pad);
        // if (!forAuto)
        //     _RaceGear(monster);
        Core.Jump(cell, pad);

        Core.Logger($"Killing Ultra-Boss {monster}");
        bool ded = false;
        Bot.Events.MonsterKilled += b => ded = true;
        while (!Bot.ShouldExit && !ded)
        {
            if (monster == "Fallen Star")
            {
                while (Bot.Self.HasActiveAura("Solar Flare"))
                    Bot.Combat.Attack("Blessless Deer");
            }
            if (!Bot.Combat.StopAttacking)
                Bot.Combat.Attack(monster);
            Bot.Sleep(1000);
        }
    }

    private void EquipSideDungeon()
    {
        string[] players = Army.Players();
        if (Bot.Player.Username == players[0])
        {
            Core.Equip("Legion Revenant");
            Core.Equip("Cape of Awe"); // Penitence Cape
            Bot.Skills.StartAdvanced("3S | 4S | 1S | 2S");
            Bot.Options.LagKiller = false;
        } 
        else if (Bot.Player.Username == players[1])
        {
            Core.Equip("ArchPaladin");
            Core.Equip("Cape of Awe"); // Penitence Cape
            // Bot.Skills.StartAdvanced("3S | 2S | 1S");
            Bot.Skills.StartAdvanced("3 | 1 | 2 | 1 | 2 | 4 | 1", 250, SkillUseMode.WaitForCooldown);
        } 
        else if (Bot.Player.Username == players[2])
        {
            Core.Equip("Lord of Order");
            Core.Equip("Cape of Awe"); // Penitence Cape
            Bot.Skills.StartAdvanced("2 | 4 | 1 | 3");
        }
        else if (Bot.Player.Username == players[3])
        {
            // // Healer gear
            // Core.Equip("Dragon of Time");
            // Core.Equip("Awescended Omni Cowl");
            // Core.Equip("Category Five Hurricane Cloud");
            // Core.Equip("Exalted Apotheosis");
            // Core.Equip("Scroll of Enrage");
            // Bot.Skills.StartAdvanced("5S | 3 | 2 | 1 | 2 | 4", 250, SkillUseMode.UseIfAvailable);

            Core.Equip("Chaos Avenger");
            Bot.Skills.StartAdvanced("4 | 3 | 1 | 2");
        }
    }

    private void EquipMidDungeon()
    {
        string[] players = Army.Players();
        if (Bot.Player.Username == players[0])
        {
            Core.Equip("Legion Revenant");
            Core.Equip("Cape of Awe"); // Penitence Cape
            Core.Equip("Necrotic Sword of Doom");
            Bot.Skills.StartAdvanced("3S | 4S | 1S | 2S");
            Bot.Options.LagKiller = false;
        } 
        else if (Bot.Player.Username == players[1])
        {
            Core.Equip("ArchPaladin");
            Core.Equip("Cape of Awe"); // Penitence Cape
            Core.Equip("Dual Exalted Apotheosis");
            // Bot.Skills.StartAdvanced("3S | 2S | 1S");
            Bot.Skills.StartAdvanced("3 | 1 | 2 | 1 | 2 | 4 | 1", 250, SkillUseMode.WaitForCooldown);
        } 
        else if (Bot.Player.Username == players[2])
        {
            Core.Equip("Lord of Order");
            Core.Equip("Cape of Awe"); // Penitence Cape
            Core.Equip("Dual Exalted Apotheosis");
            Bot.Skills.StartAdvanced("2 | 4 | 1 | 3");
        }
        else if (Bot.Player.Username == players[3])
        {
            // Healer gear
            Core.Equip("Dragon of Time");
            Core.Equip("Awescended Omni Cowl");
            Core.Equip("Category Five Hurricane Cloud");
            Core.Equip("Exalted Apotheosis");
            Bot.Skills.StartAdvanced("1 | 2 | 4 | 2 | 3 | 2");

            // Core.Equip("Chaos Avenger");
            // Bot.Skills.StartAdvanced("4 | 3 | 1 | 2");
            // Core.Equip("StoneCrusher");
            // Core.Equip("Cape of Awe"); // Penitence Cape
            // Bot.Skills.StartAdvanced("2 | 3 | 1 | 4");
        }
    }
}