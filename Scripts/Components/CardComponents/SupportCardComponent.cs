using System.Collections.Generic;
using Core;
using Core.Utils;
using Systems.StatesMachine;
using UnityEngine;

namespace Components
{
    /// <summary>
    /// Component for cards that can be played as support cards (Thẻ Phụ)
    /// Component cho thẻ hỗ trợ
    /// </summary>
    public class SupportCardComponent : Core.Component
    {
        // Properties
        private ActivationType _activationType;
        private ActivationCondition _condition;
        private Effect _effect;
        private int _cooldownTime;
        private int _currentCooldown;
        private bool _isActive;
        private string _effectDescription;
        
        // Constructor
        public SupportCardComponent() 
        {
            _activationType = ActivationType.Persistent;
            _cooldownTime = 0;
            _currentCooldown = 0;
            _isActive = false;
            _effectDescription = "";
        }
        
        // Getters and setters
        public ActivationType ActivationType 
        { 
            get { return _activationType; } 
            set { _activationType = value; }
        }
        
        public ActivationCondition Condition 
        { 
            get { return _condition; } 
            set { _condition = value; }
        }
        
        public Effect Effect 
        { 
            get { return _effect; } 
            set { _effect = value; }
        }
        
        public int CooldownTime 
        { 
            get { return _cooldownTime; } 
            set { _cooldownTime = value; }
        }
        
        public int CurrentCooldown 
        { 
            get { return _currentCooldown; } 
            set { _currentCooldown = value; }
        }
        
        public bool IsActive 
        { 
            get { return _isActive; } 
            set { _isActive = value; }
        }
        
        public string EffectDescription 
        { 
            get { return _effectDescription; } 
            set { _effectDescription = value; }
        }
        
        /// <summary>
        /// Initialize from ScriptableObject data
        /// Khởi tạo từ dữ liệu ScriptableObject
        /// </summary>
        public void Initialize(ActivationType activationType, ActivationCondition condition, 
                               Effect effect, int cooldownTime, string effectDescription)
        {
            _activationType = activationType;
            _condition = condition;
            _effect = effect;
            _cooldownTime = cooldownTime;
            _currentCooldown = 0;
            _isActive = false;
            _effectDescription = effectDescription;
        }
        
        /// <summary>
        /// Check if this support card can be activated
        /// Kiểm tra xem thẻ hỗ trợ này có thể kích hoạt không
        /// </summary>
        public bool CanActivate(Entity target, object context)
        {
            // Card cannot activate if on cooldown
            if (_currentCooldown > 0)
                return false;
            
            // If already active and not recurring/reactive, cannot activate again
            if (_isActive && _activationType != ActivationType.Recurring && _activationType != ActivationType.Reactive)
                return false;
            
            // If no condition, always can activate
            if (_condition == null)
                return true;
            
            // Check if condition is met
            return _condition.IsMet(null, target, context);
        }
        
        /// <summary>
        /// Activate the support card
        /// Kích hoạt thẻ hỗ trợ
        /// </summary>
        public void Activate(Entity target)
        {
            // Apply the effect
            if (_effect != null)
            {
                _effect.Apply(target);
            }
            
            // Mark as active for non-recurring effects
            if (_activationType != ActivationType.Recurring && _activationType != ActivationType.Reactive)
            {
                _isActive = true;
            }
            
            // Set cooldown
            _currentCooldown = _cooldownTime;
            
            Debug.Log($"Support card activated: {_effect?.Name ?? "Unknown effect"}");
        }
        
        /// <summary>
        /// Update the support card state at end of turn
        /// Cập nhật trạng thái thẻ hỗ trợ ở cuối lượt
        /// </summary>
        public void UpdateState()
        {
            // Reduce cooldown
            if (_currentCooldown > 0)
            {
                _currentCooldown--;
                
                // If cooldown reached 0 and this is a persistent effect, reactivate
                if (_currentCooldown == 0 && _activationType == ActivationType.Persistent)
                {
                    _isActive = true;
                }
            }
        }
    }
    
    /// <summary>
    /// Base class for all activation conditions
    /// Lớp cơ sở cho tất cả các điều kiện kích hoạt
    /// </summary>
    public abstract class ActivationCondition
    {
        public string Description { get; set; }
        
        // Check if the condition is met
        public abstract bool IsMet(Entity entity, Entity target, object context);
    }
    
    /// <summary>
    /// Condition: Based on health percentage
    /// Điều kiện: Dựa trên phần trăm máu
    /// </summary>
    public class HealthPercentCondition : ActivationCondition
    {
        public float HealthPercent { get; private set; }
        public bool BelowThreshold { get; private set; }
        
        public HealthPercentCondition(float healthPercent, bool belowThreshold, string description)
        {
            HealthPercent = healthPercent;
            BelowThreshold = belowThreshold;
            Description = description;
        }
        
        public override bool IsMet(Entity entity, Entity target, object context)
        {
            if (target == null)
                return false;
            
            StatsComponent stats = target.GetComponent<StatsComponent>();
            if (stats == null)
                return false;
            
            float currentPercent = (float)stats.Health / stats.MaxHealth;
            
            if (BelowThreshold)
                return currentPercent <= HealthPercent;
            else
                return currentPercent >= HealthPercent;
        }
    }
    
    /// <summary>
    /// Condition: Based on played cards of a specific element
    /// Điều kiện: Dựa trên các lá bài đã chơi của một nguyên tố cụ thể
    /// </summary>
    public class ElementPlayedCondition : ActivationCondition
    {
        public ElementType Element { get; private set; }
        public int Count { get; private set; }
        
        public ElementPlayedCondition(ElementType element, int count, string description)
        {
            Element = element;
            Count = count;
            Description = description;
        }
        
        public override bool IsMet(Entity entity, Entity target, object context)
        {
            // Context should be a list of played cards this turn
            if (!(context is List<Entity> playedCards))
                return false;
            
            int elementCount = 0;
            foreach (var card in playedCards)
            {
                ElementComponent element = card.GetComponent<ElementComponent>();
                if (element != null && element.Element == Element)
                    elementCount++;
            }
            
            return elementCount >= Count;
        }
    }
    
    /// <summary>
    /// Condition: Based on element count in hand
    /// Điều kiện: Dựa trên số lượng nguyên tố trên tay
    /// </summary>
    public class ElementCountCondition : ActivationCondition
    {
        public ElementType Element { get; private set; }
        public int Count { get; private set; }
        
        public ElementCountCondition(ElementType element, int count, string description)
        {
            Element = element;
            Count = count;
            Description = description;
        }
        
        public override bool IsMet(Entity entity, Entity target, object context)
        {
            // Context should be a list of cards in hand
            if (!(context is List<Entity> handCards))
                return false;
            
            int elementCount = 0;
            foreach (var card in handCards)
            {
                ElementComponent element = card.GetComponent<ElementComponent>();
                if (element != null && element.Element == Element)
                    elementCount++;
            }
            
            return elementCount >= Count;
        }
    }
    
    /// <summary>
    /// Condition: Based on having all elements in hand
    /// Điều kiện: Dựa trên việc có tất cả các nguyên tố trên tay
    /// </summary>
    public class AllElementsInHandCondition : ActivationCondition
    {
        public AllElementsInHandCondition(string description)
        {
            Description = description;
        }
        
        public override bool IsMet(Entity entity, Entity target, object context)
        {
            // Context should be a list of cards in hand
            if (!(context is List<Entity> handCards))
                return false;
            
            HashSet<ElementType> elements = new HashSet<ElementType>();
            
            foreach (var card in handCards)
            {
                ElementComponent element = card.GetComponent<ElementComponent>();
                if (element != null && element.Element != ElementType.None)
                {
                    elements.Add(element.Element);
                }
            }
            
            // Check if all 5 elements are present
            return elements.Count >= 5;
        }
    }
    
    /// <summary>
    /// Condition: Based on having played all elements
    /// Điều kiện: Dựa trên việc đã chơi tất cả các nguyên tố
    /// </summary>
    public class AllElementsPlayedCondition : ActivationCondition
    {
        public AllElementsPlayedCondition(string description)
        {
            Description = description;
        }
        
        public override bool IsMet(Entity entity, Entity target, object context)
        {
            // Context should be a list of all cards played in the battle
            if (!(context is List<Entity> playedCards))
                return false;
            
            HashSet<ElementType> elements = new HashSet<ElementType>();
            
            foreach (var card in playedCards)
            {
                ElementComponent element = card.GetComponent<ElementComponent>();
                if (element != null && element.Element != ElementType.None)
                {
                    elements.Add(element.Element);
                }
            }
            
            // Check if all 5 elements are present
            return elements.Count >= 5;
        }
    }
    
    /// <summary>
    /// Condition: Based on a threshold value (damage, health lost, etc.)
    /// Điều kiện: Dựa trên một ngưỡng giá trị (sát thương, máu đã mất, v.v.)
    /// </summary>
    public class ThresholdCondition : ActivationCondition
    {
        public float Threshold { get; private set; }
        public ThresholdType Type { get; private set; }
        
        public ThresholdCondition(float threshold, ThresholdType type, string description)
        {
            Threshold = threshold;
            Type = type;
            Description = description;
        }
        
        public override bool IsMet(Entity entity, Entity target, object context)
        {
            switch (Type)
            {
                case ThresholdType.DamageReceived:
                    // Context contains damage info
                    if (context is Dictionary<string, object> contextDict &&
                        contextDict.TryGetValue("Damage", out object damageObj) &&
                        damageObj is int damage)
                    {
                        return damage > Threshold;
                    }
                    break;
                    
                case ThresholdType.HealthLostPercent:
                    if (target != null)
                    {
                        StatsComponent stats = target.GetComponent<StatsComponent>();
                        if (stats != null)
                        {
                            // Calculate health lost percentage
                            float healthLostPercent = 1f - ((float)stats.Health / stats.MaxHealth);
                            return healthLostPercent > Threshold;
                        }
                    }
                    break;
                    
                case ThresholdType.DamageDealt:
                    // Context contains damage info
                    if (context is Dictionary<string, object> damageDict &&
                        damageDict.TryGetValue("DamageDealt", out object damageDealtObj) &&
                        damageDealtObj is int damageDealt)
                    {
                        return damageDealt > Threshold;
                    }
                    break;
                    
                case ThresholdType.CardsPlayed:
                    // Context contains played cards count
                    if (context is Dictionary<string, object> cardsDict &&
                        cardsDict.TryGetValue("CardsPlayed", out object cardsPlayedObj) &&
                        cardsPlayedObj is int cardsPlayed)
                    {
                        return cardsPlayed > Threshold;
                    }
                    break;
            }
            
            return false;
        }
    }
    
    /// <summary>
    /// Condition: Based on hand size
    /// Điều kiện: Dựa trên số lượng thẻ trên tay
    /// </summary>
    public class HandSizeCondition : ActivationCondition
    {
        public int Size { get; private set; }
        public bool GreaterThan { get; private set; }
        
        public HandSizeCondition(int size, bool greaterThan, string description)
        {
            Size = size;
            GreaterThan = greaterThan;
            Description = description;
        }
        
        public override bool IsMet(Entity entity, Entity target, object context)
        {
            // Context should be a list of cards in hand
            if (!(context is List<Entity> handCards))
                return false;
            
            if (GreaterThan)
                return handCards.Count > Size;
            else
                return handCards.Count < Size;
        }
    }
    
    /// <summary>
    /// Condition: Based on being targeted by an effect
    /// Điều kiện: Dựa trên việc bị nhắm bởi một hiệu ứng
    /// </summary>
    public class EffectTargetedCondition : ActivationCondition
    {
        public EffectTargetedCondition(string description)
        {
            Description = description;
        }
        
        public override bool IsMet(Entity entity, Entity target, object context)
        {
            // Context contains targeted info
            if (context is Dictionary<string, object> contextDict &&
                contextDict.TryGetValue("Targeted", out object targetedObj) &&
                targetedObj is bool targeted)
            {
                return targeted;
            }
            
            return false;
        }
    }
    
    /// <summary>
    /// Condition: Based on dealing damage
    /// Điều kiện: Dựa trên việc gây sát thương
    /// </summary>
    public class DamageDealtCondition : ActivationCondition
    {
        public DamageDealtCondition(string description)
        {
            Description = description;
        }
        
        public override bool IsMet(Entity entity, Entity target, object context)
        {
            // Context contains damage dealt info
            if (context is Dictionary<string, object> contextDict &&
                contextDict.TryGetValue("DamageDealt", out object damageObj) &&
                damageObj is int damage)
            {
                return damage > 0;
            }
            
            // For now, assume true if target exists and has StatsComponent
            return target != null && target.GetComponent<StatsComponent>() != null;
        }
    }
    
    /// <summary>
    /// Condition: Based on playing combo of element cards
    /// Điều kiện: Dựa trên việc chơi combo thẻ nguyên tố
    /// </summary>
    public class ElementComboCondition : ActivationCondition
    {
        public int ComboRequired { get; private set; }
        
        public ElementComboCondition(int comboRequired, string description)
        {
            ComboRequired = comboRequired;
            Description = description;
        }
        
        public override bool IsMet(Entity entity, Entity target, object context)
        {
            // Context should be a list of played cards
            if (!(context is List<Entity> playedCards) || playedCards.Count < ComboRequired)
                return false;
            
            Dictionary<ElementType, int> elementCounts = new Dictionary<ElementType, int>();
            
            foreach (var card in playedCards)
            {
                ElementComponent element = card.GetComponent<ElementComponent>();
                if (element != null && element.Element != ElementType.None)
                {
                    if (!elementCounts.ContainsKey(element.Element))
                    {
                        elementCounts[element.Element] = 0;
                    }
                    
                    elementCounts[element.Element]++;
                    
                    // If we have enough cards of the same element, condition is met
                    if (elementCounts[element.Element] >= ComboRequired)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
    }
}