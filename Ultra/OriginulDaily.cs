/*
name: Originul Daily
description: Army for Originul daily quest
tags: ultra, army, daily
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Army/Ultra/CoreUltra.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class OriginulDaily
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
        new Option<OriginulRewards>("Reward", "What reward to pick?", "Choose your reward", OriginulRewards.DarkCrystalShard),
        sArmy.player1,
        sArmy.player2,
        sArmy.player3,
        sArmy.player4,
        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions
    };
    public enum OriginulRewards
    {
        TaintedGem = 4769,
        DarkCrystalShard = 4770,
        DiamondofNulgath = 4771,
        GemofNulgath = 6136,
        BloodGemoftheArchfiend = 22332,
        TotemofNulgath = 5357
    }

    public void ScriptMain(IScriptInterface bot)
    {
        // Core.BankingBlackList.AddRange(new[] { "Tainted Gem", "Dark Crystal Shard", "Diamond of Nulgath", "Gem of Nulgath", "Blood Gem of the Archfiend" });
        Core.BankingBlackList.AddRange(new[] { "Dark Crystal Shard", "Blood Gem of the Archfiend", "Totem of Nulgath" });
        Core.SetOptions();

        DoDaily(Bot.Config.Get<OriginulRewards>("OriginulRewards"));

        Core.SetOptions(false);
    }

    public void DoDaily(OriginulRewards RewardChoice = OriginulRewards.TotemofNulgath)
    {
        Core.OneTimeMessage("Only for army", "This is intended for use with an army, not for solo players.");

        Core.AddDrop("Tainted Gem", "Dark Crystal Shard", "Diamond of Nulgath", "Gem of Nulgath", "Blood Gem of the Archfiend", "Totem of Nulgath");
        Core.AddDrop("Xyfrag's ??? Essence", "Nightbane's ??? Essence", "Flibbitiestgibbet's ??? Essence");
        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = Army.getRoomNr();

        // Turn off antilag
        Bot.Options.LagKiller = false;
        //Bot.Flash.SetGameObject("stage.frameRate", 30);
        if (Bot.Flash.GetGameObject<bool>("ui.monsterIcon.redX.visible"))
            Bot.Flash.CallGameFunction("world.toggleMonsters");

        Ultra.UseRevitalize();
        Core.EnsureAccept(9091);

        // Shaman (t), LoO, DoT (honor), LR (t)
        //Adv.KillUltra("voidnerfkitten", "Enter", "Spawn", "Sarah the NerfKitten", "Nerfkitten's Fang", 3, publicRoom: false);
        Core.Join("voidxyfrag");
        EquipXyfrag();
        Army.waitForParty("voidxyfrag");
        if(Bot.Player.CurrentClass.Name == "Legion Revenant")
        {
            Bot.Events.ExtensionPacketReceived += XyfragListener;
        }
        // while (!Bot.ShouldExit && !Core.CheckInventory("Xyfrag's ??? Essence"))
        // {
        //     Core.Join("voidxyfrag");
        //     Bot.Combat.Attack("Xyfrag");
        // Adv.KillUltra("voidxyfrag", "Enter", "Spawn", "Xyfrag", "Xyfrag's ??? Essence", 1, isTemp: false, publicRoom: false);
        Adv.KillUltra("voidxyfrag", "Enter", "Spawn", "Xyfrag", publicRoom: false);
        // }
        if(Bot.Player.CurrentClass.Name == "Legion Revenant")
        {
            Bot.Events.ExtensionPacketReceived -= XyfragListener;
        }
    

        // while (!Bot.ShouldExit && !Core.CheckInventory("Nightbane's ??? Essence"))
        // {
        Core.Join("voidnightbane");
        EquipNightBane();
            
        Adv.KillUltra("voidnightbane", "Enter", "Spawn", "Nightbane", publicRoom: false);
        // Core.HuntMonster("voidnightbane", "Nightbane", "Nightbane's ??? Essence", isTemp: false, publicRoom: false);

        // Already killed flibbi for NSOD daily
        if (Daily.CheckDaily(8653))
        {
            Adv.KillUltra("voidflibbi", "Enter", "Spawn", "Flibbitiestgibbet", publicRoom: false);
        }
        // Core.HuntMonster("voidnightbane", "Nightbane", "Nightbane's ??? Essence", isTemp: false, publicRoom: false);
        // }

        // while (!Bot.ShouldExit && !Core.CheckInventory("Flibbitiestgibbet's ??? Essence"))
        // {
            //EquipFlibbi(); 
            // Nightbane comp works too
            
            //Core.HuntMonster("voidflibbi", "Flibbitiestgibbet", "Flibbitiestgibbet's ??? Essence", isTemp: false, publicRoom: false);
        
        // }


        Core.EnsureComplete(9091, (int)RewardChoice);
        Core.CancelRegisteredQuests();
        Core.ToBank("Tainted Gem", "Dark Crystal Shard", "Diamond of Nulgath", "Gem of Nulgath", "Blood Gem of the Archfiend", "Totem of Nulgath");

        async void XyfragListener(dynamic packet)
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
                                    if (a is not null && (string)a["msg"] is "BLEEEEEEEEEEEECCH"){
                                        Bot.Sleep(1500);
                                        Bot.Skills.UseSkill(5);
                                    }
                                }
                            }
                            break;
                }
            }
        }

    }

    private void EquipXyfrag()
    {
        string[] players = Army.Players();
        if (Bot.Player.Username == players[0])
        {
            Core.Equip("Legion Revenant");
            Core.Equip("Dual Exalted Apotheosis"); // arcana
            Core.Equip("Potent Revitalize Elixir");
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("3 | 4 | 1 | 2");
        } 
        else if (Bot.Player.Username == players[1])
        {
            Core.Equip("Verus DoomKnight");
            Core.Equip("Divine Veil"); // anima
            Core.Equip("Scroll of Enrage");
            Core.Equip("Felicitous Philtre");
            Bot.Skills.StartAdvanced("5S | 1 | 2 | 3 | 1 | 2 | 4 | 3 | 2 | 1 | 3 | 2 | 4");
        } 
        else if (Bot.Player.Username == players[2])
        {
            Core.Equip("Lord of Order");
            Core.Equip("Malgor's ShadowFlame Blade"); // awe blast
            Core.Equip("Scroll of Enrage");
            Core.Equip("Felicitous Philtre");
            Bot.Skills.StartAdvanced("5S | 2S | 4S | 1S | 3S");
        }
        else if (Bot.Player.Username == players[3])
        {
            Core.Equip("Void Highlord");
            Core.Equip("Divine Veil"); // anima
            Core.Equip("Fate Tonic");
            Core.Equip("Felicitous Philtre");
            Bot.Skills.StartAdvanced("5S | 3S | 4S | 1S | 2S");
        }
    }

    private void EquipNightBane()
    {
        string[] players = Army.Players();
        if (Bot.Player.Username == players[0])
        {
            Core.Equip(new[]{ "Legion Revenant", "Head of the Legion Beast", "Awescended Omni Wings", "Exalted Apotheosis" });
            Bot.Skills.StartAdvanced("3 | 4 | 1 | 2");
        } 
        else if (Bot.Player.Username == players[1])
        {
            Core.Equip(new[]{ "Dragon of Time", "Awescended Omni Cowl", "Corrupted BattleMage Cape", "Exalted Apotheosis" });
            Bot.Skills.StartAdvanced("3 | 2 | 1 | 2 | 4");
        } 
        else if (Bot.Player.Username == players[2])
        {
            Core.Equip(new[]{ "Lord of Order", "Head of the Legion Beast", "Awescended Omni Wings", "Dual Exalted Apotheosis" });
            Bot.Skills.StartAdvanced("3 | 2 | 1 | 2 | 4");
        }
        else if (Bot.Player.Username == players[3])
        {
            Core.Equip(new[]{ "Dragon of Time", "Awescended Omni Cowl", "Category Five Hurricane Cloud", "Exalted Apotheosis" });
            Bot.Skills.StartAdvanced("3 | 2 | 1 | 2 | 4");
        }
    }

    private void EquipFlibbi()
    {
        string[] players = Army.Players();
        if (Bot.Player.Username == players[0])
        {
            Core.Equip("Legion Revenant");
            Core.Equip("Potent Revitalize Elixir");
            Core.Equip("Scroll of Enrage");
            Bot.Skills.StartAdvanced("5S | 3S | 4S | 1S | 2S");
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
            Bot.Skills.StartAdvanced("2S | 4S | 1S | 3S");
        }
        else if (Bot.Player.Username == players[3])
        {
            Core.Equip("StoneCrusher");
            Bot.Skills.StartAdvanced("3S | 2S | 1 | 4");
        }
    }

}