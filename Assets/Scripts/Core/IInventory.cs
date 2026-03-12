using System.Collections.Generic;
using Smithy.Data;

namespace Smithy.Core
{
    public interface IInventory
    {
        IReadOnlyList<ItemData> Items { get; }
        int Count(ItemData item);
        bool HasItem(ItemData item);
        bool RemoveItem(ItemData item);
        void AddItem(ItemData item);
    }
}
