using UnityEngine;

namespace Data
{
    /// <summary>
    /// Scriptable object for divine beast cards
    /// </summary>
    [CreateAssetMenu(fileName = "New Divine Beast Card", menuName = "NguhanhGame/Divine Beast Card Data")]
    public class DivineBeastCardDataSO : CardDataSO
    {
        [Header("Divine Beast Effect")]
        [TextArea(3, 5)]
        public string effectDescription;
        
        [Header("Effect Parameters")]
        public string effectTargetStat;
        public float effectValue;
        public int effectDuration;
        
    }
}