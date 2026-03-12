using System.Collections.Generic;
using UnityEngine;
using Smithy.Core;
using Smithy.Data;

namespace Smithy.Player
{
    public class PlayerInventory : MonoBehaviour, IInventory
    {
        [SerializeField] private List<ItemData> items = new List<ItemData>();

        public IReadOnlyList<ItemData> Items => items;

        public bool HasItem(ItemData item)
        {
            if (item == null) return false;
            return items.Contains(item);
        }

        public bool RemoveItem(ItemData item)
        {
            return items.Remove(item);
        }

        public void AddItem(ItemData item)
        {
            if (item != null)
                items.Add(item);
        }
    }
}
