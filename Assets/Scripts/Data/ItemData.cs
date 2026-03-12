using System;
using System.Collections.Generic;
using UnityEngine;

namespace Smithy.Data
{
    [Serializable]
    public struct ItemStat
    {
        public string label;
        public float value;
    }

    [CreateAssetMenu(fileName = "New Item", menuName = "Smithy/Item Data")]
    public class ItemData : ScriptableObject
    {
        public const int StatsCount = 5;

        public string itemName = "Item";
        public Sprite icon;
        [SerializeField] private ItemStat[] stats = new ItemStat[StatsCount];

        public IReadOnlyList<(string label, float value)> GetDisplayStats()
        {
            var list = new List<(string, float)>();
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
