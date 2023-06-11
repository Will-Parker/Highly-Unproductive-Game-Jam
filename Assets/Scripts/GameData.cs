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

    public static Dictionary<int, int[]> levelRankRequirements = new() // key=levelID, value=(index=rank, element=turnRequirement)
    {
        { 0, new int[3] { 0, 0, 0 } },
        { 1, new int[3] { 12, 20, 30 } },
        { 2, new int[3] { 10, 25, 40 } },
        { 3, new int[3] { 6, 15, 30 } },
        { 4, new int[3] { 20, 40, 80 } }
    };

    public static Dictionary<int, int> bestLevelClear = new() // key=levelID, value=turnsTaken
    {
        { 0, int.MaxValue },
        { 1, int.MaxValue },
        { 2, int.MaxValue },
        { 3, int.MaxValue },
        { 4, int.MaxValue },
    };

    public static float GetStatSum(AllyType thisAllyType, AllyType upAllyType, AllyType loAllyType, StatType statType)
    {
        return allyStats[thisAllyType][thisAllyType][statType]
            + allyStats[thisAllyType][upAllyType][statType]
            + allyStats[thisAllyType][loAllyType][statType];
    }

    public static void ResetAllyStats()
    {
        allyStats = new()
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
    }

}
