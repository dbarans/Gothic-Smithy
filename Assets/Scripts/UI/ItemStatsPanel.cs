using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Smithy.Data;

namespace Smithy.UI
{

    public class ItemStatsPanel : MonoBehaviour
    {
        public static ItemStatsPanel Instance { get; private set; }


        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private Image iconImage;

        [SerializeField] private TMP_Text strengthValueText;
        [SerializeField] private TMP_Text damageValueText;
        [SerializeField] private TMP_Text fireDamageValueText;
        [SerializeField] private TMP_Text magicDamageValueText;

        [SerializeField] private TMP_Text valueValueText;

        [Tooltip("Odstęp panelu od kursora (w pikselach)")]
        [SerializeField] private float marginFromCursor = 12f;

        private RectTransform _rect;
        private Canvas _canvas;

        private void Awake()
        {
            Instance = this;
            _rect = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
            Hide();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private static void SetStatRow(TMP_Text valueField, int value)
        {
            if (valueField == null) return;
            GameObject row = valueField.transform.parent != null ? valueField.transform.parent.gameObject : valueField.gameObject;
            bool show = value != 0;
            row.SetActive(show);
            if (show)
                valueField.text = value.ToString();
        }

        public void Setup(ItemData item)
        {
            if (item == null)
            {
                if (itemNameText != null) itemNameText.text = "";
                if (iconImage != null) iconImage.enabled = false;
                if (strengthValueText != null && strengthValueText.transform.parent != null) strengthValueText.transform.parent.gameObject.SetActive(false);
                if (damageValueText != null && damageValueText.transform.parent != null) damageValueText.transform.parent.gameObject.SetActive(false);
                if (fireDamageValueText != null && fireDamageValueText.transform.parent != null) fireDamageValueText.transform.parent.gameObject.SetActive(false);
                if (magicDamageValueText != null && magicDamageValueText.transform.parent != null) magicDamageValueText.transform.parent.gameObject.SetActive(false);
                if (valueValueText != null && valueValueText.transform.parent != null) valueValueText.transform.parent.gameObject.SetActive(false);
                return;
            }

            if (itemNameText != null) itemNameText.text = item.itemName ?? "";
            if (iconImage != null)
            {
                iconImage.enabled = item.icon != null;
                if (item.icon != null) iconImage.sprite = item.icon;
            }

            SetStatRow(strengthValueText, item.strength);
            SetStatRow(damageValueText, item.damage);
            SetStatRow(fireDamageValueText, item.fireDamage);
            SetStatRow(magicDamageValueText, item.magicDamage);
            SetStatRow(valueValueText, item.value);
        }

        public void ShowAt(ItemData item, Vector2 screenPosition)
        {
            if (item == null) { Hide(); return; }
            if (_rect == null) _rect = GetComponent<RectTransform>();
            if (_canvas == null) _canvas = GetComponentInParent<Canvas>();
            if (_canvas == null || _rect == null) return;

            Setup(item);
            gameObject.SetActive(true);

            float scale = _canvas.rootCanvas.scaleFactor;
            float panelW = _rect.rect.width * scale;
            float panelH = _rect.rect.height * scale;
            float margin = marginFromCursor;

            bool cursorLeftHalf = screenPosition.x < Screen.width * 0.5f;
            bool cursorTopHalf = screenPosition.y > Screen.height * 0.5f;

            float centerX = cursorLeftHalf
                ? screenPosition.x + margin + panelW * 0.5f
                : screenPosition.x - margin - panelW * 0.5f;
            float centerY = cursorTopHalf
                ? screenPosition.y - margin - panelH * 0.5f
                : screenPosition.y + margin + panelH * 0.5f;

            centerX = Mathf.Clamp(centerX, panelW * 0.5f, Screen.width - panelW * 0.5f);
            centerY = Mathf.Clamp(centerY, panelH * 0.5f, Screen.height - panelH * 0.5f);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.rootCanvas.transform as RectTransform,
                new Vector2(centerX, centerY),
                _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
                out Vector2 localPoint);
            _rect.anchoredPosition = localPoint;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
