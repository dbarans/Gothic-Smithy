using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Smithy.Core;
using Smithy.Data;

namespace Smithy.Crafting
{
    public class FurnaceStation : MonoBehaviour, IInteractable
    {
        [SerializeField] private CraftingManager craftingManager;
        [SerializeField] private float heatingDuration = 3f;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private GameObject progressUI;

        private bool _isHeating;
        private ItemData _itemInFurnace;

        public string GetPromptText() => "E - Use furnace";

        public void Interact(InteractContext context)
        {
            if (_isHeating) return;
            if (craftingManager == null) return;
            var playerInv = craftingManager.PlayerInventory;
            if (playerInv == null) return;
            StartCoroutine(TakeAndHeat(playerInv));
        }

        private IEnumerator TakeAndHeat(IInventory playerInv)
        {
            _isHeating = true;
            ItemData input = null;
            foreach (var item in playerInv.Items)
            {
                var output = craftingManager.TryGetFurnaceOutput(item, playerInv);
                if (output != null) { input = item; break; }
            }

            if (input == null)
            {
                _isHeating = false;
                yield break;
            }

            playerInv.RemoveItem(input);
            _itemInFurnace = input;

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

            var result = craftingManager.GetFurnaceOutput(_itemInFurnace);
            if (result != null && playerInv != null)
                playerInv.AddItem(result);
            _itemInFurnace = null;
            _isHeating = false;
        }
    }
}
