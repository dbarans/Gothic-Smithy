using System.Collections.Generic;
using UnityEngine;
using Smithy.Core;
using Smithy.Data;
using Smithy.UI;

namespace Smithy.Crafting
{
    public class AnvilStation : MonoBehaviour, IInteractable
    {
        [SerializeField] private List<RecipeData> recipes = new List<RecipeData>();
        [SerializeField] private CraftingManager craftingManager;
        [SerializeField] private CraftingStationUI craftingUI;
        [SerializeField] private InventoryUI inventoryUI;

        public string GetPromptText() => "[E] - Kowadło";

        public void Interact(InteractContext context)
        {
            if (craftingManager == null) return;
            var inv = craftingManager.PlayerInventory;
            if (inv == null) return;
            if (craftingUI != null)
            {
                var playerCtrl = context.Player != null ? context.Player.GetComponent<Smithy.Player.FirstPersonController>() : null;
                craftingUI.Show(recipes, (recipe, inventory) => craftingManager.ExecuteAnvilCraft(recipe, inventory), playerCtrl);
                if (inventoryUI != null) inventoryUI.Show();
            }
        }
    }
}
