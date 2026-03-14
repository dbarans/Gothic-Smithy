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
        private string stationPrompt;
        [SerializeField] private float craftDurationFallback = 2f;
        [SerializeField] private List<AudioClip> craftSounds = new List<AudioClip>();
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private GameObject progressUI;
        [SerializeField] private List<RecipeData> recipes = new List<RecipeData>();
        [SerializeField] private CraftingManager craftingManager;
        [SerializeField] private CraftingStationUI craftingUI;
        [SerializeField] private InventoryUI inventoryUI;

        private bool _isCrafting;
        private ItemData _outputToGive;

        public virtual string GetPromptText() => string.IsNullOrEmpty(stationPrompt) ? "" : stationPrompt;

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
            float duration = GetCraftDuration();
            if (progressSlider != null) progressSlider.value = 0f;
            if (progressUI != null) progressUI.SetActive(true);

            var source = audioSource != null ? audioSource : GetComponent<AudioSource>();
            int soundIndex = 0;
            float nextSoundTime = 0f;

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                if (progressSlider != null) progressSlider.value = t / duration;

                if (source != null && craftSounds != null && soundIndex < craftSounds.Count && t >= nextSoundTime)
                {
                    AudioClip clip = craftSounds[soundIndex];
                    if (clip != null)
                    {
                        source.PlayOneShot(clip);
                        nextSoundTime += clip.length;
                    }
                    soundIndex++;
                }

                yield return null;
            }

            if (progressUI != null) progressUI.SetActive(false);

            if (_outputToGive != null && playerInv != null)
                playerInv.AddItem(_outputToGive);
            _outputToGive = null;
            _isCrafting = false;
        }

        private float GetCraftDuration()
        {
            if (craftSounds != null && craftSounds.Count > 0)
            {
                float total = 0f;
                foreach (var clip in craftSounds)
                    if (clip != null)
                        total += clip.length;
                if (total > 0f) return total;
            }
            return Mathf.Max(0.01f, craftDurationFallback);
        }
    }
}
