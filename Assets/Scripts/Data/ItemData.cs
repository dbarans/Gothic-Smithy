using UnityEngine;

namespace Smithy.Data
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Smithy/Item Data")]
    public class ItemData : ScriptableObject
    {
        public Sprite icon;
        public string itemName;

        public int strength;
        public int damage;
        public int fireDamage;
        public int magicDamage;
        public int value;
    }
}
