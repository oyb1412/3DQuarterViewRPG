using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#region Stat
[Serializable]
public class Stat
{
    public int level;
    public int hp;
    public int attack;
}

[Serializable]
public class StatData : ILorder<int, Stat>
{
    public List<Stat> stats = new List<Stat>();
    public Dictionary<int, Stat> MakeDict()
    {
        Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
        foreach (var stat in stats)
        {
            dict.Add(stat.level, stat);
        }

        return dict;
    }
}
#endregion
