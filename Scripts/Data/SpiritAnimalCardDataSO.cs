using Core.Utils;
using Data;
using Systems.StatesMachine;
using UnityEngine;

/// <summary>
/// Scriptable object for spirit animal cards
/// </summary>
[CreateAssetMenu(fileName = "New Spirit Animal Card", menuName = "NguhanhGame/Spirit Animal Card Data")]
public class SpiritAnimalCardDataSO : CardDataSO
{
    [Header("Spirit Animal Information")]
    public string zodiacAnimal; // e.g., "Rat", "Ox", etc.
        
    [Header("Support Effect")]
    [TextArea(3, 5)]
    public string supportEffectDescription;
        
    [Header("Activation Type")]
    public ActivationType activationType;
        
    [Header("Activation Condition")]
    [TextArea(2, 3)]
    public string activationConditionDescription;
    public string conditionType; // "health", "element", "turn", etc.
    public float conditionValue; // Percentage, count, etc.
}