using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Smithy.Core;
using Smithy.Crafting;
using Smithy.Data;

namespace Smithy.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private CraftingManager craftingManager;
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Transform content;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private float refreshInterval = 0.15f;

        private IInventory _inventory;
        private float _nextRefresh;
        private readonly List<GameObject> _spawnedSlots = new List<GameObject>();

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
        }

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
            if (_inventory == null) return;
            if (Time.time < _nextRefresh) return;
            _nextRefresh = Time.time + refreshInterval;
            Refresh();
        }

        private void OnEnable()
        {
            _nextRefresh = 0f;
        }

        private void ClearSlots()
        {
            foreach (var go in _spawnedSlots)
            {
                if (go != null) Destroy(go);
            }
            _spawnedSlots.Clear();
        }

        public void Refresh()
        {
            if (content == null || slotPrefab == null)
                return;
            if (_inventory == null)
            {
                ClearSlots();
                return;
            }

            var counts = new Dictionary<ItemData, int>();
            foreach (var item in _inventory.Items)
            {
                if (item == null) continue;
                counts[item] = counts.GetValueOrDefault(item, 0) + 1;
            }

            ClearSlots();
            foreach (var kv in counts)
            {
                var row = Instantiate(slotPrefab, content);
                _spawnedSlots.Add(row);
                var slot = row.GetComponent<RequiredIngredientRow>();
                if (slot != null)
                    slot.Setup(kv.Key, kv.Value);
            }
        }

        public void Show() { if (panelRoot != null) panelRoot.SetActive(true); }
        public void Hide() { if (panelRoot != null) panelRoot.SetActive(false); }
    }
}
