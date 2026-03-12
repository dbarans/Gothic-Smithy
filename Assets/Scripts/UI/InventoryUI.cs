using UnityEngine;
using TMPro;
using Smithy.Core;
using Smithy.Crafting;

namespace Smithy.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private CraftingManager craftingManager;
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text listText;
        [SerializeField] private float refreshInterval = 0.15f;

        private IInventory _inventory;
        private float _nextRefresh;

        private void Start()
        {
            if (craftingManager != null)
                _inventory = craftingManager.PlayerInventory;
            if (_inventory == null)
            {
                var pi = FindObjectOfType<Smithy.Player.PlayerInventory>();
                if (pi != null) _inventory = pi;
            }
        }

        private void Update()
        {
            if (_inventory == null || listText == null) return;
            if (Time.time < _nextRefresh) return;
            _nextRefresh = Time.time + refreshInterval;
            Refresh();
        }

        private void OnEnable()
        {
            _nextRefresh = 0f;
        }

        public void Refresh()
        {
            if (listText == null) return;
            if (_inventory == null)
            {
                listText.text = "—";
                return;
            }
            var items = _inventory.Items;
            if (items.Count == 0)
            {
                listText.text = "(empty)";
                return;
            }
            var lines = new System.Collections.Generic.List<string>();
            foreach (var item in items)
                lines.Add(item != null ? item.itemName : "?");
            listText.text = string.Join("\n", lines);
        }

        public void Show() { if (panelRoot != null) panelRoot.SetActive(true); }
        public void Hide() { if (panelRoot != null) panelRoot.SetActive(false); }
    }
}
