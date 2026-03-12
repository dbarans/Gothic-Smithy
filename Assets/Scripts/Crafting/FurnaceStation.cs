using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Smithy.Core;
using Smithy.Data;
using Smithy.UI;

namespace Smithy.Crafting
{
    public class FurnaceStation : MonoBehaviour, IInteractable
    {
        [SerializeField] private List<RecipeData> recipes = new List<RecipeData>();
        [SerializeField] private CraftingManager craftingManager;
        [SerializeField] private CraftingStationUI craftingUI;
        [SerializeField] private InventoryUI inventoryUI;
        [SerializeField] private float heatingDuration = 3f;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private GameObject progressUI;

        private bool _isHeating;
        private ItemData _outputToGive;

        public string GetPromptText() => "[E] - Piec";

        public void Interact(InteractContext context)
        {
            if (_isHeating) return;
            if (craftingUI != null && craftingManager != null)
            {
                var playerCtrl = context.Player != null ? context.Player.GetComponent<Smithy.Player.FirstPersonController>() : null;
                craftingUI.Show(recipes, TryStartCraft, playerCtrl);
                if (inventoryUI != null) inventoryUI.Show();
            }
            else if (craftingManager != null)
            {
                var playerInv = craftingManager.PlayerInventory;
                if (playerInv != null) StartCoroutine(TakeAndHeatFirst(playerInv));
            }
        }

        public bool TryStartCraft(RecipeData recipe, IInventory playerInv)
        {
            if (recipe == null || recipe.outputItem == null || playerInv == null || _isHeating) return false;
            if (!CraftingManager.HasAllIngredients(recipe, playerInv)) return false;
            CraftingManager.ConsumeIngredients(recipe, playerInv);
            _outputToGive = recipe.outputItem;
            _isHeating = true;
            StartCoroutine(HeatCurrentItem(playerInv));
            return true;
        }

        private IEnumerator HeatCurrentItem(IInventory playerInv)
        {
            if (progressSlider != null) progressSlider.value = 0f;
            if (progressUI != null) progressUI.SetActive(true);

            float t = 0f;
            while (t < heatingDuration)
            {
                t += Time.deltaTime;
                if (progressSlider != null) progressSlider.value = t / heatingDuration;
                yield return null;
            }

            if (progressUI != null) progressUI.SetActive(false);

            if (_outputToGive != null && playerInv != null)
                playerInv.AddItem(_outputToGive);
            _outputToGive = null;
            _isHeating = false;
        }

        private IEnumerator TakeAndHeatFirst(IInventory playerInv)
        {
            var recipe = CraftingManager.GetFirstCraftable(recipes, playerInv);
            if (recipe == null) yield break;
            CraftingManager.ConsumeIngredients(recipe, playerInv);
            _outputToGive = recipe.outputItem;
            _isHeating = true;
            yield return HeatCurrentItem(playerInv);
        }
    }
}
