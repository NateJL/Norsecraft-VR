using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public enum Faction
    {
        None,
        Stormstad,
        Bjornheim
    };
    public static readonly string[] factionNames = { "Stormstad", "Bjornheim" };
    public static readonly int factionCount = 2;

    public static readonly string[] craftingSkills = { "Woodcutting", "Mining", "Crafting", "Fishing", "Farming", "Cooking" };
    public static readonly int skillCount = 6;
    public static readonly float maxSkill = 99;

    public Color stormstadColor;
    public Color bjornheimColor;
    public Color rumRunnerColor;

    public static int hour;
    public static int minute;

    public static readonly int MaxNameCharacters = 16;

    public static readonly int MaxFactionReputation = 4096;
    public static readonly int MinFactionReputation = -4096;
    public static readonly int FactionHostileThreshold = -100;

    public static readonly int TileSize = 500;

    /// <summary>
    /// Safetly destroy an object from the editor or at runtime.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T SafeDestroy<T>(T obj) where T : Object
    {
        if (Application.isEditor)
            Object.DestroyImmediate(obj);
        else
            Object.Destroy(obj);

        return null;
    }
}
