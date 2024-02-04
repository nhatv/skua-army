/*
name: Do all ultra
description: Does all the ultra dailies with an army
tags: ultra, daily, army
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Army/Ultra/NSODDaily.cs
//cs_include Scripts/Army/Ultra/UltraEngineer.cs
//cs_include Scripts/Army/Ultra/UltraEzrajal.cs
//cs_include Scripts/Army/Ultra/UltraWarden.cs
//cs_include Scripts/Army/Ultra/UltraTyndarius.cs
//cs_include Scripts/Army/Ultra/OriginulDaily.cs
//cs_include Scripts/Army/Ultra/CoreUltra.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class DoAllUltra
{
    public IScriptInterface Bot => IScriptInterface.Instance;

    public CoreBots Core => CoreBots.Instance;
    public CoreAdvanced Adv = new();
    public CoreFarms Farm = new();
    public CoreDailies Daily = new();
    private static CoreArmyLite sArmy = new();
    private CoreArmyLite Army = new();
    public string OptionsStorage = "Ultra";
    public bool DontPreconfigure = true;
    private NSoDDaily NSODD = new();
    private UltraEzrajal UEzra = new();
    private UltraWarden UWard = new();
    private UltraEngineer UEngi = new();
    private UltraTyndarius UTynd = new();
    private OriginulDaily OrigD = new();
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

        DoAll();

        Core.SetOptions(false);
    }

    public void DoAll()
    {
        // if (Daily.CheckDaily(8245))
        //     UTynd.DoDaily();

        // if (Daily.CheckDaily(8153))
        //     UWard.DoDaily();

        if (Daily.CheckDaily(8653))
            NSODD.DoDaily();

        if (Daily.CheckDaily(9091))
            OrigD.DoDaily();

        // if (Daily.CheckDaily(8154))
        //     UEngi.DoDaily();

        // if (Daily.CheckDaily(8152))
        //     UEzra.DoDaily();

    }

}