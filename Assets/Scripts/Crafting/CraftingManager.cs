using System.Collections.Generic;
using UnityEngine;
using Smithy.Core;
using Smithy.Data;

namespace Smithy.Crafting
{
    public class CraftingManager : MonoBehaviour
    {
        [System.Serializable]
        public class Recipe
        {
            public ItemData inputItem;
            public ItemData outputItem;
        }

        [SerializeField] private Smithy.Player.PlayerInventory playerInventory;
        [SerializeField] private List<Recipe> furnaceRecipes = new List<Recipe>();
        [SerializeField] private List<Recipe> anvilRecipes = new List<Recipe>();

        public IInventory PlayerInventory => playerInventory;

        public ItemData GetFurnaceOutput(ItemData inputItem)
        {
            if (inputItem == null) return null;
            foreach (var r in furnaceRecipes)
                if (r.inputItem == inputItem) return r.outputItem;
            return null;
        }

        public ItemData TryGetFurnaceOutput(ItemData inputItem, IInventory inventory)
        {
            if (inputItem == null || inventory == null) return null;
            foreach (var r in furnaceRecipes)
                if (r.inputItem == inputItem && inventory.HasItem(inputItem))
                    return r.outputItem;
            return null;
        }

        public ItemData TryGetAnvilOutput(ItemData inputItem, IInventory inventory)
        {
            if (inputItem == null || inventory == null) return null;
            foreach (var r in anvilRecipes)
                if (r.inputItem == inputItem && inventory.HasItem(inputItem))
                    return r.outputItem;
            return null;
        }

        public bool ExecuteAnvilCraft(ItemData inputItem, IInventory inventory)
        {
            var output = TryGetAnvilOutput(inputItem, inventory);
            if (output == null) return false;
            if (!inventory.RemoveItem(inputItem)) return false;
            inventory.AddItem(output);
            return true;
        }
    }
}
