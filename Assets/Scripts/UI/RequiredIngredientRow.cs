using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Smithy.Data;

namespace Smithy.UI
{
    public class RequiredIngredientRow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text countLabel;

        private ItemData _currentItem;

        public void Setup(ItemData item, int requiredCount, int playerCount)
        {
            _currentItem = item;
            if (icon != null)
            {
                icon.enabled = item != null && item.icon != null;
                if (item != null && item.icon != null)
                    icon.sprite = item.icon;
            }
            if (countLabel != null)
                countLabel.text = $"{playerCount}/{requiredCount}";
        }

        public void Setup(ItemData item, int count)
        {
            _currentItem = item;
            if (icon != null)
            {
                icon.enabled = item != null && item.icon != null;
                if (item != null && item.icon != null)
                    icon.sprite = item.icon;
            }
            if (countLabel != null)
                countLabel.text = count > 1 ? count.ToString() : (count == 1 ? "1" : "0");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ItemStatsPanel.Instance?.ShowAt(_currentItem, eventData.position);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ItemStatsPanel.Instance?.Hide();
        }
    }
}
