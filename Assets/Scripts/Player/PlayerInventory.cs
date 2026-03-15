using System.Collections.Generic;
using UnityEngine;
using Smithy.Core;
using Smithy.Data;

namespace Smithy.Player
{
    [System.Serializable]
    public class ItemStack
    {
        public ItemData item;
        public int count = 1;
    }

    public class PlayerInventory : MonoBehaviour, IInventory
    {
        [SerializeField] private List<ItemStack> items = new List<ItemStack>();

        private List<ItemData> _flatCache = new List<ItemData>();

        public IReadOnlyList<ItemData> Items
        {
            get
            {
                _flatCache.Clear();
                foreach (var s in items)
                {
                    if (s?.item == null) continue;
                    for (int i = 0; i < s.count; i++)
                        _flatCache.Add(s.item);
                }
                return _flatCache;
            }
        }

        public int Count(ItemData item)
        {
            if (item == null) return 0;
            foreach (var s in items)
                if (s?.item == item) return s.count;
            return 0;
        }

        public bool HasItem(ItemData item)
        {
            return Count(item) > 0;
        }

        public bool RemoveItem(ItemData item)
        {
            if (item == null) return false;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i]?.item != item) continue;
                items[i].count--;
                if (items[i].count <= 0)
                    items.RemoveAt(i);
                return true;
            }
            return false;
        }

        public void AddItem(ItemData item)
        {
            if (item == null) return;
            foreach (var s in items)
            {
                if (s?.item == item)
                {
                    s.count++;
                    return;
                }
            }
            items.Add(new ItemStack { item = item, count = 1 });
        }
    }
}
