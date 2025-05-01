using UnityEngine;

namespace Data
{
    /// <summary>
    /// Scriptable object for monster cards
    /// </summary>
    [CreateAssetMenu(fileName = "New Monster Card", menuName = "NguhanhGame/Monster Card Data")]
    public class MonsterCardDataSO : CardDataSO
    {
        [System.Serializable]
        public class EffectData
        {
            public string effectName;
            [TextArea(2, 3)]
            public string effectDescription;
            public string effectType; // "damage", "buff", "debuff", etc.
            public float effectValue;
            public int effectDuration; // in turns, -1 for permanent
        }
        
        [Header("Monster Effects")]
        public EffectData[] effects;
    }
}