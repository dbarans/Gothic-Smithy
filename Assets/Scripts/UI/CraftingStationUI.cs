using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Smithy.Core;
using Smithy.Data;
using Smithy.Crafting;

namespace Smithy.UI
{
    public class CraftingStationUI : MonoBehaviour
    {
        [SerializeField] private CraftingManager craftingManager;
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Transform recipeListContent;
        [SerializeField] private GameObject recipeRowPrefab;
        [SerializeField] private Image outputItemIcon;
        [SerializeField] private TMP_Text outputItemName;
        [SerializeField] private TMP_Text requiredText;
        [SerializeField] private Transform requiredIngredientsContent;
        [SerializeField] private GameObject requiredIngredientRowPrefab;
        [SerializeField] private Button craftButton;
        [SerializeField] private InventoryUI inventoryUI;

        private int _selectedIndex;
        private List<RecipeData> _recipes = new List<RecipeData>();
        private Func<RecipeData, IInventory, bool> _tryCraft;
        private readonly List<GameObject> _spawnedRecipeRows = new List<GameObject>();
        private readonly List<GameObject> _spawnedIngredientRows = new List<GameObject>();
        private Smithy.Player.FirstPersonController _currentPlayerController;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            if (craftButton != null) craftButton.onClick.AddListener(OnCraftClicked);
        }

        private void Update()
        {
            if (panelRoot != null && panelRoot.activeSelf && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                Hide();
        }

        public void Show(IReadOnlyList<RecipeData> recipes, Func<RecipeData, IInventory, bool> tryCraft, Smithy.Player.FirstPersonController playerController = null)
        {
            _currentPlayerController = playerController;
            _tryCraft = tryCraft;
            _recipes.Clear();
            if (recipes != null)
                foreach (var r in recipes)
                    if (r != null) _recipes.Add(r);

            ClearRecipeRows();
            if (recipeListContent != null && recipeRowPrefab != null)
            {
                for (int i = 0; i < _recipes.Count; i++)
                {
                    var recipe = _recipes[i];
                    var row = Instantiate(recipeRowPrefab, recipeListContent);
                    _spawnedRecipeRows.Add(row);
                    var rowScript = row.GetComponent<RecipeRow>();
                    if (rowScript != null)
                        rowScript.Setup(recipe);
                    var btn = row.GetComponent<Button>();
                    if (btn != null)
                    {
                        int index = i;
                        btn.onClick.AddListener(() => SelectRecipe(index));
                    }
                }
            }

            _selectedIndex = -1;
            RefreshDetail();

            if (panelRoot != null) panelRoot.SetActive(true);

            if (_currentPlayerController != null)
            {
                _currentPlayerController.SetMovementAndLookEnabled(false);
                _currentPlayerController.SetCursorVisible(true);
            }
        }

        private void ClearRecipeRows()
        {
            foreach (var go in _spawnedRecipeRows)
            {
                if (go != null) Destroy(go);
            }
            _spawnedRecipeRows.Clear();
        }

        public void Hide()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            _tryCraft = null;
            ClearRecipeRows();
            ClearRequiredIngredientsPanel();
            if (inventoryUI != null) inventoryUI.Hide();

            if (_currentPlayerController != null)
            {
                _currentPlayerController.SetMovementAndLookEnabled(true);
                _currentPlayerController.SetCursorVisible(false);
                _currentPlayerController = null;
            }
        }

        private void SelectRecipe(int index)
        {
            if (index >= 0 && index < _recipes.Count)
            {
                _selectedIndex = index;
                RefreshDetail();
            }
        }

        private void RefreshDetail()
        {
            var inv = craftingManager != null ? craftingManager.PlayerInventory : null;
            ClearRequiredIngredientsPanel();

            if (_selectedIndex < 0 || _selectedIndex >= _recipes.Count)
            {
                if (outputItemIcon != null) outputItemIcon.enabled = false;
                if (outputItemName != null) outputItemName.text = "—";
                if (requiredText != null) requiredText.text = "";
                if (craftButton != null) craftButton.interactable = false;
                return;
            }

            var recipe = _recipes[_selectedIndex];
            if (recipe == null) return;
            var item = recipe.outputItem;
            if (outputItemIcon != null)
            {
                outputItemIcon.enabled = item != null && item.icon != null;
                if (item != null && item.icon != null)
                    outputItemIcon.sprite = item.icon;
            }
            if (outputItemName != null)
                outputItemName.text = item != null ? item.itemName : "—";
            if (requiredText != null)
                requiredText.text = BuildRequiredString(recipe);

            var required = GetRequiredCounts(recipe);
            if (requiredIngredientsContent != null && requiredIngredientRowPrefab != null)
            {
                foreach (var (ingredientItem, requiredCount) in required)
                {
                    var row = Instantiate(requiredIngredientRowPrefab, requiredIngredientsContent);
                    _spawnedIngredientRows.Add(row);
                    var rowScript = row.GetComponent<RequiredIngredientRow>();
                    if (rowScript != null)
                        rowScript.Setup(ingredientItem, requiredCount, inv != null ? inv.Count(ingredientItem) : 0);
                }
            }

            if (craftButton != null)
                craftButton.interactable = inv != null && CraftingManager.HasAllIngredients(recipe, inv);
        }

        private void ClearRequiredIngredientsPanel()
        {
            foreach (var go in _spawnedIngredientRows)
            {
                if (go != null) Destroy(go);
            }
            _spawnedIngredientRows.Clear();
        }

        private static List<(ItemData item, int count)> GetRequiredCounts(RecipeData recipe)
        {
            var list = new List<(ItemData, int)>();
            if (recipe?.ingredients == null) return list;
            foreach (var ing in recipe.ingredients)
            {
                if (ing?.item == null) continue;
                list.Add((ing.item, ing.count));
            }
            return list;
        }

        private static string BuildRequiredString(RecipeData recipe)
        {
            if (recipe?.ingredients == null || recipe.ingredients.Length == 0) return "";
            var parts = new List<string>();
            foreach (var ing in recipe.ingredients)
            {
                if (ing?.item == null) continue;
                parts.Add(ing.count > 1 ? $"{ing.item.itemName} x{ing.count}" : ing.item.itemName);
            }
            return "Wymagane: " + string.Join(", ", parts);
        }

        private void OnCraftClicked()
        {
            if (_tryCraft == null || _selectedIndex < 0 || _selectedIndex >= _recipes.Count) return;
            var inv = craftingManager != null ? craftingManager.PlayerInventory : null;
            if (inv == null) return;
            var recipe = _recipes[_selectedIndex];
            if (!CraftingManager.HasAllIngredients(recipe, inv)) return;
            if (_tryCraft(recipe, inv))
                RefreshDetail();
        }
    }
}
