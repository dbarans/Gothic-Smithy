using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Smithy.Data;

namespace Smithy.UI
{
    public class RequiredIngredientRow : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text countLabel;

        public void Setup(ItemData item, int requiredCount, int playerCount)
        {
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
            if (icon != null)
            {
                icon.enabled = item != null && item.icon != null;
                if (item != null && item.icon != null)
                    icon.sprite = item.icon;
            }
            if (countLabel != null)
                countLabel.text = count > 1 ? count.ToString() : (count == 1 ? "1" : "0");
        }
    }
}
