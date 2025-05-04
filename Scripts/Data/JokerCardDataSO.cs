using Core.Utils;
using Data;
using Systems.StatesMachine;
using UnityEngine;

/// <summary>
/// Scriptable object for joker cards
/// </summary>
[CreateAssetMenu(fileName = "New Joker Card", menuName = "NguhanhGame/Joker Card Data")]
public class JokerCardDataSO : CardDataSO
{
    [Header("Joker Effect")]
    [TextArea(3, 5)]
    public string effectDescription;
        
    [Header("Activation Type")]
    public ActivationType activationType = ActivationType.Transformative;
        
    [Header("Activation Condition")]
    [TextArea(2, 3)]
    public string activationConditionDescription;
    public string conditionType; // "health", "element", "turn", etc.
    public float conditionValue; // Percentage, count, etc.
        
    [Header("Cooldown")]
    public int cooldownTime = 5; // Default cooldown for Joker cards
}