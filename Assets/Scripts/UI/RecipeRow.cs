using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Smithy.Data;

namespace Smithy.UI
{
    [RequireComponent(typeof(Button))]
    public class RecipeRow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;

        private RecipeData _currentRecipe;

        public void Setup(RecipeData recipe)
        {
            _currentRecipe = recipe;
            var item = recipe != null ? recipe.outputItem : null;
            if (icon != null)
            {
                icon.enabled = item != null && item.icon != null;
                if (item != null && item.icon != null)
                    icon.sprite = item.icon;
            }
            if (nameText != null)
                nameText.text = item != null ? item.itemName : "—";
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            var item = _currentRecipe != null ? _currentRecipe.outputItem : null;
            ItemStatsPanel.Instance?.ShowAt(item, eventData.position);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ItemStatsPanel.Instance?.Hide();
        }
    }
}
