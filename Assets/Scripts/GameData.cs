using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    public static Dictionary<AllyType, Dictionary<AllyType, Dictionary<StatType, float>>> allyStats = new()
    {
        {
            AllyType.Apple, 
            new Dictionary<AllyType, Dictionary<StatType, float>>()
            {
                {
                    AllyType.Apple, new Dictionary<StatType, float>()
                    {
                        { StatType.MaxHealth, 6f },
                        { StatType.Attack, 3f },
                        { StatType.Unique, 1f }
                    }
                },
                {
                    AllyType.Strawberry, new Dictionary<StatType, float>()
                    {
                        { StatType.MaxHealth, 0f },
                        { StatType.Attack, 0f },
                        { StatType.Unique, 0f }
                    }
                },
                {
                    AllyType.Lemon, new Dictionary<StatType, float>()
                    {
                        { StatType.MaxHealth, 0f },
                        { StatType.Attack, 0f },
                        { StatType.Unique, 0f }
                    }
                },
                {
                    AllyType.Blueberry, new Dictionary<StatType, float>()
                    {
                        { StatType.MaxHealth, 0f },
                        { StatType.Attack, 0f },
                        { StatType.Unique, 0f }
                    }
                }
            }
        },
        {
            AllyType.Strawberry,
            new Dictionary<AllyType, Dictionary<StatType, float>>()
            {
                {
                    AllyType.Apple, new Dictionary<StatType, float>()
                    {
                        { StatType.MaxHealth, 0f },
                        { StatType.Attack, 0f },
                        { StatType.Unique, 0f }
                    }
                },
                {
                    AllyType.Strawberry, new Dictionary<StatType, float>()
                    {
                        { StatType.MaxHealth, 2f },
                        { StatType.Attack, 1f },
                        { StatType.Unique, 3f }
                    }
                },
                {
                    AllyType.Lemon, new Dictionary<StatType, float>()
                    {
                        { StatType.MaxHealth, 0f },
                        { StatType.Attack, 0f },
                        { StatType.Unique, 0f }
                    }
                },
                {
                    AllyType.Blueberry, new Dictionary<StatType, float>()
                    {
                        { StatType.MaxHealth, 0f },
                        { StatType.Attack, 0f },
                        { StatType.Unique, 0f }
                    }
                }
            }
        },
        {
            AllyType.Lemon,
            new Dictionary<AllyType, Dictionary<StatType, float>>()
            {
                {
                    AllyType.Apple, new Dictionary<StatType, float>()
                    {
                        { StatType.MaxHealth, 0f },
                        { StatType.Attack, 0f },
                        { StatType.Unique, 0f }
                    }
                },
                {
                    AllyType.Strawberry, new Dictionary<StatType, float>()
                    {
                        { StatType.MaxHealth, 0f },
                        { StatType.Attack, 0f },
                        { StatType.Unique, 0f }
                    }
                },
                {
                    AllyType.Lemon, new Dictionary<StatType, float>()
                    {
                        { StatType.MaxHealth, 4f },
                        { StatType.Attack, 1f },
                        { StatType.Unique, 1f }
                    }
                },
                {
                    AllyType.Blueberry, new Dictionary<StatType, float>()
                    {
                        { StatType.MaxHealth, 0f },
                        { StatType.Attack, 0f },
                        { StatType.Unique, 0f }
                    }
                }
            }
        },
        {
            AllyType.Blueberry,
            new Dictionary<AllyType, Dictionary<StatType, float>>()
            {
                {
                    AllyType.Apple, new Dictionary<StatType, float>()
                    {
                        { StatType.MaxHealth, 0f },
                        { StatType.Attack, 0f },
                        { StatType.Unique, 0f }
                    }
                },
                {
                    AllyType.Strawberry, new Dictionary<StatType, float>()
                    {
                        { StatType.MaxHealth, 0f },
                        { StatType.Attack, 0f },
                        { StatType.Unique, 0f }
                    }
                },
                {
                    AllyType.Lemon, new Dictionary<StatType, float>()
                    {
                        { StatType.MaxHealth, 0f },
                        { StatType.Attack, 0f },
                        { StatType.Unique, 0f }
                    }
                },
                {
                    AllyType.Blueberry, new Dictionary<StatType, float>()
                    {
                        { StatType.MaxHealth, 5f },
                        { StatType.Attack, 2f },
                        { StatType.Unique, 1f }
                    }
                }
            }
        }
    };

    public static List<TextAsset> usedAppleBlueberryStories = new();
    public static List<TextAsset> usedAppleLemonStories = new();
    public static List<TextAsset> usedAppleStrawberryStories = new();
    public static List<TextAsset> usedBlueberryLemonStories = new();
    public static List<TextAsset> usedBlueberryStrawberryStories = new();
    public static List<TextAsset> usedLemonStrawberryStories = new();

    public static float GetStatSum(AllyType thisAllyType, AllyType upAllyType, AllyType loAllyType, StatType statType)
    {
        return allyStats[thisAllyType][thisAllyType][statType]
            + allyStats[thisAllyType][upAllyType][statType]
            + allyStats[thisAllyType][loAllyType][statType];
    }
}
