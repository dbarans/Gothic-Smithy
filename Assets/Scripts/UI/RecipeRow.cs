using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Smithy.Data;

namespace Smithy.UI
{
    [RequireComponent(typeof(Button))]
    public class RecipeRow : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;

        public void Setup(RecipeData recipe)
        {
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
    }
}
