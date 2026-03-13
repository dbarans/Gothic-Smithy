using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Smithy.Core;
using Smithy.Data;
using Smithy.UI;

namespace Smithy.Crafting
{
    public class CraftStation : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private string stationPrompt = "E - Stacja";
        [SerializeField] private float craftDuration = 2f;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private GameObject progressUI;
        [SerializeField] private List<RecipeData> recipes = new List<RecipeData>();
        [SerializeField] private CraftingManager craftingManager;
        [SerializeField] private CraftingStationUI craftingUI;
        [SerializeField] private InventoryUI inventoryUI;

        private bool _isCrafting;
        private ItemData _outputToGive;

        public virtual string GetPromptText() => string.IsNullOrEmpty(stationPrompt) ? "E - Stacja" : stationPrompt;

        public virtual void Interact(InteractContext context)
        {
            if (_isCrafting) return;
            if (craftingManager == null) return;
            var inv = craftingManager.PlayerInventory;
            if (inv == null) return;
            if (craftingUI != null)
            {
                var playerCtrl = context.Player != null ? context.Player.GetComponent<Smithy.Player.FirstPersonController>() : null;
                craftingUI.Show(recipes, TryStartCraft, playerCtrl);
                if (inventoryUI != null) inventoryUI.Show();
            }
        }

        public bool TryStartCraft(RecipeData recipe, IInventory playerInv)
        {
            if (recipe == null || recipe.outputItem == null || playerInv == null || _isCrafting) return false;
            if (!CraftingManager.HasAllIngredients(recipe, playerInv)) return false;
            CraftingManager.ConsumeIngredients(recipe, playerInv);
            _outputToGive = recipe.outputItem;
            _isCrafting = true;
            StartCoroutine(RunCraftProgress(playerInv));
            return true;
        }

        private IEnumerator RunCraftProgress(IInventory playerInv)
        {
            float duration = Mathf.Max(0.01f, craftDuration);
            if (progressSlider != null) progressSlider.value = 0f;
            if (progressUI != null) progressUI.SetActive(true);

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                if (progressSlider != null) progressSlider.value = t / duration;
                yield return null;
            }

            if (progressUI != null) progressUI.SetActive(false);

            if (_outputToGive != null && playerInv != null)
                playerInv.AddItem(_outputToGive);
            _outputToGive = null;
            _isCrafting = false;
        }
    }
}
