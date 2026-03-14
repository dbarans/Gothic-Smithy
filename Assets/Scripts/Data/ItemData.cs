using System;
using System.Collections.Generic;
using UnityEngine;

namespace Smithy.Data
{
    [Serializable]
    public struct ItemStat
    {
        public string label;
        public string value;
    }

    [CreateAssetMenu(fileName = "New Item", menuName = "Smithy/Item Data")]
    public class ItemData : ScriptableObject
    {
        public const int StatsCount = 5;

        public string itemName;
        public Sprite icon;
        [SerializeField] private ItemStat[] stats = new ItemStat[StatsCount];

        public IReadOnlyList<(string label, string value)> GetDisplayStats()
        {
            var list = new List<(string, string)>();
            if (stats == null) return list;
            foreach (var s in stats)
            {
                if (!string.IsNullOrEmpty(s.label))
                    list.Add((s.label, s.value));
            }
            return list;
        }
    }
}
