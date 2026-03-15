using System.Collections.Generic;
using UnityEngine;
using Smithy.Core;
using Smithy.Data;

namespace Smithy.Crafting
{
    public class CraftingManager : MonoBehaviour
    {
        [SerializeField] private Smithy.Player.PlayerInventory playerInventory;
        [SerializeField] private List<RecipeData> furnaceRecipes = new List<RecipeData>();
        [SerializeField] private List<RecipeData> anvilRecipes = new List<RecipeData>();

        public IInventory PlayerInventory => playerInventory;
        public IReadOnlyList<RecipeData> FurnaceRecipes => furnaceRecipes;
        public IReadOnlyList<RecipeData> AnvilRecipes => anvilRecipes;

        public static bool HasAllIngredients(RecipeData recipe, IInventory inv)
        {
            if (recipe == null || recipe.ingredients == null || inv == null) return false;
            foreach (var ing in recipe.ingredients)
            {
                if (ing?.item == null) continue;
                if (inv.Count(ing.item) < ing.count) return false;
            }
            return true;
        }

        public static void ConsumeIngredients(RecipeData recipe, IInventory inv)
        {
            if (recipe?.ingredients == null || inv == null) return;
            foreach (var ing in recipe.ingredients)
            {
                if (ing?.item == null) continue;
                for (int i = 0; i < ing.count; i++)
                    inv.RemoveItem(ing.item);
            }
        }

        public static RecipeData GetFirstCraftable(IReadOnlyList<RecipeData> recipes, IInventory inv)
        {
            if (inv == null || recipes == null) return null;
            foreach (var r in recipes)
                if (r != null && HasAllIngredients(r, inv)) return r;
            return null;
        }

        public RecipeData GetFirstCraftableFurnaceRecipe(IInventory inv)
        {
            if (inv == null) return null;
            foreach (var r in furnaceRecipes)
                if (r != null && HasAllIngredients(r, inv)) return r;
            return null;
        }

        public ItemData GetFurnaceOutput(ItemData inputItem)
        {
            if (inputItem == null) return null;
            foreach (var r in furnaceRecipes)
                if (r != null && r.ingredients != null && r.ingredients.Length == 1 && r.ingredients[0].item == inputItem)
                    return r.outputItem;
            return null;
        }

        public ItemData TryGetFurnaceOutput(ItemData inputItem, IInventory inventory)
        {
            if (inputItem == null || inventory == null) return null;
            foreach (var r in furnaceRecipes)
                if (r != null && r.ingredients != null && r.ingredients.Length == 1 && r.ingredients[0].item == inputItem && inventory.Count(inputItem) >= r.ingredients[0].count)
                    return r.outputItem;
            return null;
        }

        public RecipeData GetFirstCraftableAnvilRecipe(IInventory inv)
        {
            if (inv == null) return null;
            foreach (var r in anvilRecipes)
                if (r != null && HasAllIngredients(r, inv)) return r;
            return null;
        }

        public ItemData TryGetAnvilOutput(ItemData inputItem, IInventory inventory)
        {
            if (inputItem == null || inventory == null) return null;
            foreach (var r in anvilRecipes)
                if (r != null && r.ingredients != null && r.ingredients.Length == 1 && r.ingredients[0].item == inputItem && inventory.Count(inputItem) >= r.ingredients[0].count)
                    return r.outputItem;
            return null;
        }

        public bool ExecuteAnvilCraft(RecipeData recipe, IInventory inventory)
        {
            if (recipe == null || inventory == null || !HasAllIngredients(recipe, inventory)) return false;
            ConsumeIngredients(recipe, inventory);
            if (recipe.outputItem != null) inventory.AddItem(recipe.outputItem);
            return true;
        }

        public bool ExecuteAnvilCraft(ItemData inputItem, IInventory inventory)
        {
            if (inputItem == null || inventory == null) return false;
            foreach (var r in anvilRecipes)
                if (r != null && r.ingredients != null && r.ingredients.Length == 1 && r.ingredients[0].item == inputItem)
                    return ExecuteAnvilCraft(r, inventory);
            return false;
        }
    }
}
