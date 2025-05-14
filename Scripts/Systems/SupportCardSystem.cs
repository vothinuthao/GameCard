using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;
using Systems.StatesMachine;
using UnityEngine;

namespace Systems
{
    /// <summary>
    /// System that handles support cards (Thẻ Phụ)
    /// Hệ thống xử lý thẻ hỗ trợ (Thẻ Phụ)
    /// </summary>
    public class SupportCardSystem : Core.System
    {
        // Ngữ cảnh cho điều kiện kích hoạt
        // Context for activation conditions
        private List<Entity> _playedCards = new List<Entity>();
        
        // Danh sách thẻ đã kích hoạt trong lượt này để ngăn kích hoạt nhiều lần
        // List of activated cards this turn to prevent multiple activations
        private List<Entity> _activatedCardsThisTurn = new List<Entity>();

        // Constructor
        public SupportCardSystem(EntityManager entityManager) : base(entityManager)
        {
        }

        /// <summary>
        /// Update method - called every frame
        /// Phương thức cập nhật - được gọi mỗi khung hình
        /// </summary>
        public override void Update(float deltaTime)
        {
            // Hầu hết kiểm tra hiện đã được điều khiển bởi máy trạng thái trận đấu
            // Most checks are now driven by the battle state machine
            // Phương thức này được giữ để tương thích
            // This method is kept for compatibility
        }

        /// <summary>
        /// Set the played cards for this turn
        /// Đặt các thẻ đã chơi cho lượt này
        /// </summary>
        public void SetPlayedCards(List<Entity> playedCards)
        {
            _playedCards = new List<Entity>(playedCards);
        }

        /// <summary>
        /// Clear played cards and activated cards at the end of a turn
        /// Xóa thẻ đã chơi và thẻ đã kích hoạt ở cuối lượt
        /// </summary>
        public void ClearTurnData()
        {
            _playedCards.Clear();
            _activatedCardsThisTurn.Clear();
        }

        /// <summary>
        /// Check for OnEntry activation type support cards
        /// Kiểm tra thẻ hỗ trợ loại kích hoạt OnEntry
        /// </summary>
        public void CheckOnEntryActivations()
        {
            // Lấy tất cả thẻ hỗ trợ từ khu vực hỗ trợ
            // Get all support cards from the support zone
            var supportCards = GetSupportCards();

            foreach (var entity in supportCards)
            {
                var supportCard = entity.GetComponent<SupportCardComponent>();
                if (supportCard == null) continue;

                // Kiểm tra xem nó có phải là thẻ OnEntry và chưa hoạt động không
                // Check if it's an OnEntry card and not already active
                if (supportCard.ActivationType == ActivationType.OnEntry && 
                    !supportCard.IsActive &&
                    supportCard.CurrentCooldown <= 0)
                {
                    // Ghi nhật ký kích hoạt
                    // Log activation
                    CardInfoComponent cardInfo = entity.GetComponent<CardInfoComponent>();
                    Debug.Log($"Kích hoạt thẻ hỗ trợ OnEntry: {(cardInfo != null ? cardInfo.Name : "Không rõ")}");
                    
                    // Kích hoạt hiệu ứng thẻ
                    // Activate the card effect
                    if (supportCard.Effect != null)
                    {
                        supportCard.Effect.Apply(null);
                    }
                    
                    // Đánh dấu là đang hoạt động
                    // Mark as active
                    supportCard.IsActive = true;
                    supportCard.CurrentCooldown = supportCard.CooldownTime;
                    _activatedCardsThisTurn.Add(entity);
                }
            }
        }

        /// <summary>
        /// Check for support card activations at the start of a turn
        /// Kiểm tra kích hoạt thẻ hỗ trợ ở đầu lượt
        /// </summary>
        public void CheckOnTurnStartActivations(Entity entity)
        {
            var supportCards = GetSupportCards();

            foreach (var supportCard in supportCards)
            {
                var supportComponent = supportCard.GetComponent<SupportCardComponent>();
                if (supportComponent == null) continue;

                // Hiệu ứng Persistent luôn hoạt động
                // Persistent effects are always active
                if (supportComponent.ActivationType == ActivationType.Persistent)
                {
                    if (!_activatedCardsThisTurn.Contains(supportCard))
                    {
                        // Ghi nhật ký kích hoạt
                        // Log activation
                        CardInfoComponent cardInfo = supportCard.GetComponent<CardInfoComponent>();
                        Debug.Log($"Kích hoạt thẻ hỗ trợ Persistent: {(cardInfo != null ? cardInfo.Name : "Không rõ")}");
                        
                        // Áp dụng hiệu ứng persistent
                        // Apply persistent effect
                        if (supportComponent.Effect != null)
                        {
                            supportComponent.Effect.Apply(entity);
                        }
                        
                        _activatedCardsThisTurn.Add(supportCard);
                    }
                }

                // Hiệu ứng Recurring ở đầu lượt
                // Recurring effects at turn start
                if (supportComponent.ActivationType == ActivationType.Recurring &&
                    supportComponent.CurrentCooldown <= 0 &&
                    !_activatedCardsThisTurn.Contains(supportCard))
                {
                    // Kiểm tra điều kiện đầu lượt - cần triển khai phù hợp
                    // dựa trên loại điều kiện
                    // Check turn start condition - this would need proper implementation
                    // based on the type of condition
                    bool isTurnStartCondition = IsTurnStartCondition(supportComponent.Condition);
                    
                    if (isTurnStartCondition && supportComponent.Condition.IsMet(supportCard, entity, null))
                    {
                        // Ghi nhật ký kích hoạt
                        // Log activation
                        CardInfoComponent cardInfo = supportCard.GetComponent<CardInfoComponent>();
                        Debug.Log($"Kích hoạt thẻ hỗ trợ Recurring ở đầu lượt: {(cardInfo != null ? cardInfo.Name : "Không rõ")}");
                        
                        // Áp dụng hiệu ứng
                        // Apply the effect
                        if (supportComponent.Effect != null)
                        {
                            supportComponent.Effect.Apply(entity);
                        }
                        
                        supportComponent.CurrentCooldown = supportComponent.CooldownTime;
                        _activatedCardsThisTurn.Add(supportCard);
                    }
                }
            }
        }

        /// <summary>
        /// Check for support card activations at the end of a turn
        /// Kiểm tra kích hoạt thẻ hỗ trợ ở cuối lượt
        /// </summary>
        public void CheckOnTurnEndActivations(Entity entity)
        {
            var supportCards = GetSupportCards();

            foreach (var supportCard in supportCards)
            {
                var supportComponent = supportCard.GetComponent<SupportCardComponent>();
                if (supportComponent == null) continue;

                // Kiểm tra kích hoạt cuối lượt
                // Check for turn end activations
                if (supportComponent.ActivationType == ActivationType.Recurring &&
                    supportComponent.CurrentCooldown <= 0 &&
                    !_activatedCardsThisTurn.Contains(supportCard))
                {
                    // Kiểm tra điều kiện cuối lượt
                    // Check turn end condition
                    bool isTurnEndCondition = IsTurnEndCondition(supportComponent.Condition);
                    
                    if (isTurnEndCondition && supportComponent.Condition.IsMet(supportCard, entity, null))
                    {
                        // Ghi nhật ký kích hoạt
                        // Log activation
                        CardInfoComponent cardInfo = supportCard.GetComponent<CardInfoComponent>();
                        Debug.Log($"Kích hoạt thẻ hỗ trợ Recurring ở cuối lượt: {(cardInfo != null ? cardInfo.Name : "Không rõ")}");
                        
                        // Áp dụng hiệu ứng
                        // Apply the effect
                        if (supportComponent.Effect != null)
                        {
                            supportComponent.Effect.Apply(entity);
                        }
                        
                        supportComponent.CurrentCooldown = supportComponent.CooldownTime;
                        _activatedCardsThisTurn.Add(supportCard);
                    }
                }
            }
        }

        /// <summary>
        /// Check for support card activations based on health
        /// Kiểm tra kích hoạt thẻ hỗ trợ dựa trên máu
        /// </summary>
        public void CheckHealthBasedActivations(Entity entity)
        {
            var supportCards = GetSupportCards();

            foreach (var supportCard in supportCards)
            {
                var supportComponent = supportCard.GetComponent<SupportCardComponent>();
                if (supportComponent == null) continue;

                // Kiểm tra xem nó có phải là thẻ Triggered với điều kiện health không
                // Check if it's a Triggered card with a health condition
                if ((supportComponent.ActivationType == ActivationType.Triggered ||
                     supportComponent.ActivationType == ActivationType.Reactive) &&
                    supportComponent.Condition is HealthPercentCondition &&
                    supportComponent.CurrentCooldown <= 0 &&
                    !_activatedCardsThisTurn.Contains(supportCard))
                {
                    // Kiểm tra xem điều kiện có được đáp ứng không
                    // Check if the condition is met
                    if (supportComponent.Condition.IsMet(supportCard, entity, null))
                    {
                        // Ghi nhật ký kích hoạt
                        // Log activation
                        CardInfoComponent cardInfo = supportCard.GetComponent<CardInfoComponent>();
                        Debug.Log($"Kích hoạt thẻ hỗ trợ dựa trên máu: {(cardInfo != null ? cardInfo.Name : "Không rõ")}");
                        
                        // Kích hoạt thẻ
                        // Activate the card
                        if (supportComponent.Effect != null)
                        {
                            supportComponent.Effect.Apply(entity);
                        }
                        
                        // Đối với thẻ Triggered, kích hoạt một lần và đặt cooldown
                        // For Triggered cards, activate once and set cooldown
                        if (supportComponent.ActivationType == ActivationType.Triggered)
                        {
                            supportComponent.IsActive = true;
                            supportComponent.CurrentCooldown = supportComponent.CooldownTime;
                        }
                        
                        _activatedCardsThisTurn.Add(supportCard);
                    }
                }
            }
        }

        /// <summary>
        /// Check for element based activations
        /// Kiểm tra kích hoạt dựa trên nguyên tố
        /// </summary>
        public void CheckElementBasedActivations(Entity target)
        {
            if (_playedCards.Count == 0) return;
            
            var supportCards = GetSupportCards();

            foreach (var supportCard in supportCards)
            {
                var supportComponent = supportCard.GetComponent<SupportCardComponent>();
                if (supportComponent == null) continue;

                // Kiểm tra kích hoạt dựa trên nguyên tố
                // Check for element-based activations
                if ((supportComponent.ActivationType == ActivationType.Recurring ||
                     supportComponent.ActivationType == ActivationType.Triggered) &&
                    supportComponent.Condition is ElementPlayedCondition &&
                    supportComponent.CurrentCooldown <= 0 &&
                    !_activatedCardsThisTurn.Contains(supportCard))
                {
                    // Kiểm tra xem điều kiện có được đáp ứng không
                    // Check if the condition is met
                    if (supportComponent.Condition.IsMet(supportCard, null, _playedCards))
                    {
                        // Ghi nhật ký kích hoạt
                        // Log activation
                        CardInfoComponent cardInfo = supportCard.GetComponent<CardInfoComponent>();
                        Debug.Log($"Kích hoạt thẻ hỗ trợ dựa trên nguyên tố: {(cardInfo != null ? cardInfo.Name : "Không rõ")}");
                        
                        // Áp dụng hiệu ứng
                        // Apply the effect
                        if (supportComponent.Effect != null)
                        {
                            supportComponent.Effect.Apply(target ?? _playedCards[0]);
                        }
                        
                        // Đối với thẻ Triggered, kích hoạt một lần và đặt cooldown
                        // For Triggered cards, activate once and set cooldown
                        if (supportComponent.ActivationType == ActivationType.Triggered)
                        {
                            supportComponent.IsActive = true;
                        }
                        
                        supportComponent.CurrentCooldown = supportComponent.CooldownTime;
                        _activatedCardsThisTurn.Add(supportCard);
                    }
                }
            }
        }

        /// <summary>
        /// Check for reactive activations when player is targeted
        /// Kiểm tra kích hoạt phản ứng khi người chơi bị nhắm mục tiêu
        /// </summary>
        public void CheckReactiveActivations(Entity entity, Entity attacker, int damage)
        {
            var supportCards = GetSupportCards();

            foreach (var supportCard in supportCards)
            {
                var supportComponent = supportCard.GetComponent<SupportCardComponent>();
                if (supportComponent == null) continue;

                // Kiểm tra xem nó có phải là thẻ Reactive không
                // Check if it's a Reactive card
                if (supportComponent.ActivationType == ActivationType.Reactive &&
                    supportComponent.CurrentCooldown <= 0 &&
                    !_activatedCardsThisTurn.Contains(supportCard))
                {
                    // Ngữ cảnh bổ sung cho điều kiện phản ứng
                    // Additional context for reactive conditions
                    Dictionary<string, object> context = new Dictionary<string, object>
                    {
                        { "Attacker", attacker },
                        { "Damage", damage }
                    };

                    // Kiểm tra xem điều kiện có được đáp ứng không
                    // Check if the condition is met
                    if (supportComponent.Condition.IsMet(supportCard, entity, context))
                    {
                        // Ghi nhật ký kích hoạt
                        // Log activation
                        CardInfoComponent cardInfo = supportCard.GetComponent<CardInfoComponent>();
                        Debug.Log($"Kích hoạt thẻ hỗ trợ Reactive: {(cardInfo != null ? cardInfo.Name : "Không rõ")}");
                        
                        // Áp dụng hiệu ứng - đối với thẻ phản ứng, thường nhắm mục tiêu kẻ tấn công
                        // Apply the effect - for reactive cards, usually target the attacker
                        if (supportComponent.Effect != null)
                        {
                            supportComponent.Effect.Apply(attacker);
                        }
                        
                        supportComponent.CurrentCooldown = supportComponent.CooldownTime;
                        _activatedCardsThisTurn.Add(supportCard);
                    }
                }
            }
        }

        /// <summary>
        /// Check for transformative activations
        /// Kiểm tra kích hoạt biến đổi
        /// </summary>
        public void CheckTransformativeActivations(Entity entity)
        {
            var supportCards = GetSupportCards();

            foreach (var supportCard in supportCards)
            {
                var supportComponent = supportCard.GetComponent<SupportCardComponent>();
                if (supportComponent == null) continue;

                // Kiểm tra xem nó có phải là thẻ Transformative không
                // Check if it's a Transformative card
                if (supportComponent.ActivationType == ActivationType.Transformative &&
                    supportComponent.CurrentCooldown <= 0 &&
                    !_activatedCardsThisTurn.Contains(supportCard))
                {
                    // Kiểm tra xem điều kiện có được đáp ứng không
                    // Check if the condition is met
                    if (supportComponent.Condition.IsMet(supportCard, entity, null))
                    {
                        // Ghi nhật ký kích hoạt
                        // Log activation
                        CardInfoComponent cardInfo = supportCard.GetComponent<CardInfoComponent>();
                        Debug.Log($"Kích hoạt thẻ hỗ trợ Transformative: {(cardInfo != null ? cardInfo.Name : "Không rõ")}");
                        
                        // Áp dụng hiệu ứng
                        // Apply the effect
                        if (supportComponent.Effect != null)
                        {
                            supportComponent.Effect.Apply(entity);
                        }
                        
                        supportComponent.IsActive = true;
                        supportComponent.CurrentCooldown = supportComponent.CooldownTime;
                        _activatedCardsThisTurn.Add(supportCard);
                    }
                }
            }
        }
        
        /// <summary>
        /// Get all support cards in the support zone
        /// Lấy tất cả thẻ hỗ trợ trong khu vực hỗ trợ
        /// </summary>
        private List<Entity> GetSupportCards()
        {
            // Điều này lý tưởng sẽ được lấy từ CardSystem
            // This would ideally be retrieved from the CardSystem
            // Hiện tại, chúng tôi sẽ chỉ truy vấn tất cả các entity có SupportCardComponent
            // For now, we'll just query all entities with SupportCardComponent
            return new List<Entity>(EntityManager.GetEntitiesWithComponents<SupportCardComponent>());
        }

        /// <summary>
        /// Determine if a condition is a turn start condition
        /// Xác định xem điều kiện có phải là điều kiện bắt đầu lượt không
        /// </summary>
        private bool IsTurnStartCondition(ActivationCondition condition)
        {
            // Điều này cần triển khai phù hợp dựa trên loại điều kiện
            // This would need proper implementation based on the type of condition
            // Hiện tại, chỉ là giữ chỗ
            // For now, just a placeholder
            return condition.GetType().Name.Contains("TurnStart");
        }

        /// <summary>
        /// Determine if a condition is a turn end condition
        /// Xác định xem điều kiện có phải là điều kiện kết thúc lượt không
        /// </summary>
        private bool IsTurnEndCondition(ActivationCondition condition)
        {
            // Điều này cần triển khai phù hợp dựa trên loại điều kiện
            // This would need proper implementation based on the type of condition
            // Hiện tại, chỉ là giữ chỗ
            // For now, just a placeholder
            return condition.GetType().Name.Contains("TurnEnd");
        }
    }
}