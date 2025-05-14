using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;
using UnityEngine;

namespace Systems
{
    /// <summary>
    /// System that handles card scoring calculations based on Ngũ Hành relationships
    /// Hệ thống xử lý tính điểm dựa trên mối quan hệ Ngũ Hành
    /// </summary>
    public class ScoringSystem : Core.System
    {
        // Tham chiếu đến các hệ thống khác
        // References to other systems
        private ElementInteractionSystem _elementInteractionSystem;
        
        // Mùa hiện tại
        // Current season
        private Season _currentSeason = Season.Spring;
        
        // Các hằng số cho tính toán điểm
        // Constants for score calculation
        private const float BASE_TUONG_SINH_BONUS = 0.3f;  // Tương Sinh: +30% bonus
        private const float BASE_TUONG_KHAC_BONUS = 0.5f;  // Tương Khắc: +50% bonus
        private const float TUONG_SINH_KEP_BONUS = 0.5f;   // Tương Sinh Kép: +50% bonus
        private const float TUONG_KHAC_KEP_BONUS = 0.8f;   // Tương Khắc Kép: +80% bonus
        
        // Constructor
        public ScoringSystem(EntityManager entityManager, ElementInteractionSystem elementInteractionSystem) : base(entityManager)
        {
            _elementInteractionSystem = elementInteractionSystem;
        }
        
        /// <summary>
        /// Update method - called every frame
        /// </summary>
        public override void Update(float deltaTime)
        {
            // This system doesn't need regular updates
        }
        
        /// <summary>
        /// Set the current season
        /// Đặt mùa hiện tại
        /// </summary>
        public void SetSeason(Season season)
        {
            _currentSeason = season;
        }
        
        /// <summary>
        /// Calculate total score for the played cards against a target
        /// Tính tổng điểm cho các lá bài đã chơi đối với mục tiêu
        /// </summary>
        public int CalculateScore(List<Entity> playedCards, Entity target)
        {
            if (playedCards == null || playedCards.Count == 0 || target == null)
                return 0;
                
            // 1. Tính điểm gốc (base score)
            // Calculate base score
            int baseScore = CalculateBaseScore(playedCards);
            
            // 2. Áp dụng hệ số Ngũ Hành (element interactions)
            // Apply element interaction modifiers
            float elementModifier = CalculateElementModifier(playedCards, target);
            float scoreAfterElements = baseScore * elementModifier;
            
            // 3. Tính điểm combo Nạp Âm (nap am combinations)
            // Calculate Nap Am combo points
            int napAmComboPoints = CalculateNapAmComboPoints(playedCards);
            
            // 4. Tính điểm từ hiệu ứng thẻ bài (card effects)
            // Calculate points from card effects
            int effectPoints = CalculateEffectPoints(playedCards, target);
            
            // 5. Áp dụng điểm từ thẻ hỗ trợ (support cards)
            // Apply support card points - This would typically come from SupportCardSystem
            int supportPoints = 0; // Will be implemented elsewhere
            
            // 6. Áp dụng hệ số môi trường (environment modifiers)
            // Apply environment modifiers
            float environmentModifier = CalculateEnvironmentModifier(playedCards);
            
            // 7. Áp dụng hệ số độ hiếm (rarity modifiers)
            // Apply rarity modifiers
            float rarityModifier = CalculateRarityModifier(playedCards);
            
            // 8. Tính điểm tạm thời (temporary score)
            // Calculate temporary score
            float temporaryScore = (scoreAfterElements + napAmComboPoints + effectPoints + supportPoints) 
                                  * environmentModifier * rarityModifier;
            
            // 9. Xét đến phòng thủ của đối thủ (target defense)
            // Consider target defense
            int targetDefense = GetTargetDefense(target);
            float damageReduction = CalculateDamageReduction(targetDefense);
            
            // 10. Tính điểm cuối cùng (final score/damage)
            // Calculate final score/damage
            int finalScore = Mathf.RoundToInt(temporaryScore * (1 - damageReduction));
            
            return finalScore;
        }
        
        /// <summary>
        /// Calculate base score from attack values of played cards
        /// Tính điểm gốc từ giá trị tấn công của các lá bài đã chơi
        /// </summary>
        private int CalculateBaseScore(List<Entity> playedCards)
        {
            int baseScore = 0;
            
            foreach (var card in playedCards)
            {
                StatsComponent stats = card.GetComponent<StatsComponent>();
                if (stats != null)
                {
                    baseScore += stats.Attack;
                }
            }
            
            return baseScore;
        }
        
        /// <summary>
        /// Calculate element interaction modifier based on Tương Sinh/Tương Khắc relationships
        /// Tính hệ số tương tác nguyên tố dựa trên mối quan hệ Tương Sinh/Tương Khắc
        /// </summary>
        private float CalculateElementModifier(List<Entity> playedCards, Entity target)
        {
            // Default modifier - no bonus
            float modifier = 1.0f;
            
            // Lấy thành phần nguyên tố của mục tiêu
            // Get target element
            ElementComponent targetElement = target.GetComponent<ElementComponent>();
            if (targetElement == null)
                return modifier;
                
            // Kiểm tra mối quan hệ Tương Sinh giữa các lá bài
            // Check for Tương Sinh relationships between played cards
            bool hasTuongSinh = false;
            
            // Kiểm tra mối quan hệ Tương Khắc đối với mục tiêu
            // Check for Tương Khắc relationships against target
            bool hasTuongKhac = false;
            
            // Kiểm tra chuỗi Tương Sinh (3 lá bài liên tiếp)
            // Check for Tương Sinh chain (3 consecutive cards)
            bool hasTuongSinhChain = false;
            
            // Kiểm tra chuỗi Tương Khắc (2 lá bài khắc 2 yếu tố của đối thủ)
            // Check for Tương Khắc chain (2 cards overcoming 2 elements of target)
            bool hasTuongKhacChain = false;
            
            if (playedCards.Count > 1)
            {
                // Kiểm tra quan hệ giữa các lá bài
                // Check relationships between played cards
                for (int i = 0; i < playedCards.Count - 1; i++)
                {
                    ElementComponent element1 = playedCards[i].GetComponent<ElementComponent>();
                    ElementComponent element2 = playedCards[i + 1].GetComponent<ElementComponent>();
                    
                    if (element1 != null && element2 != null)
                    {
                        if (_elementInteractionSystem.HasGeneratingRelationship(element1.Element, element2.Element))
                        {
                            hasTuongSinh = true;
                            
                            // Kiểm tra chuỗi Tương Sinh (3 lá bài)
                            // Check for Tương Sinh chain (3 cards)
                            if (i < playedCards.Count - 2)
                            {
                                ElementComponent element3 = playedCards[i + 2].GetComponent<ElementComponent>();
                                if (element3 != null && _elementInteractionSystem.HasGeneratingRelationship(element2.Element, element3.Element))
                                {
                                    hasTuongSinhChain = true;
                                }
                            }
                        }
                    }
                }
            }
            
            // Kiểm tra quan hệ với mục tiêu
            // Check relationships with target
            int tuongKhacCount = 0;
            foreach (var card in playedCards)
            {
                ElementComponent cardElement = card.GetComponent<ElementComponent>();
                if (cardElement != null)
                {
                    if (_elementInteractionSystem.HasOvercomingRelationship(cardElement.Element, targetElement.Element))
                    {
                        hasTuongKhac = true;
                        tuongKhacCount++;
                    }
                }
            }
            
            // Kiểm tra chuỗi Tương Khắc
            // Check for Tương Khắc chain
            if (tuongKhacCount >= 2)
            {
                hasTuongKhacChain = true;
            }
            
            // Áp dụng hệ số dựa trên mối quan hệ
            // Apply modifiers based on relationships
            if (hasTuongSinhChain)
            {
                modifier *= (1 + TUONG_SINH_KEP_BONUS);
            }
            else if (hasTuongSinh)
            {
                modifier *= (1 + BASE_TUONG_SINH_BONUS);
            }
            
            if (hasTuongKhacChain)
            {
                modifier *= (1 + TUONG_KHAC_KEP_BONUS);
            }
            else if (hasTuongKhac)
            {
                modifier *= (1 + BASE_TUONG_KHAC_BONUS);
            }
            
            return modifier;
        }
        
        /// <summary>
        /// Calculate points from Nap Am combinations
        /// Tính điểm từ các kết hợp Nạp Âm
        /// </summary>
        private int CalculateNapAmComboPoints(List<Entity> playedCards)
        {
            int comboPoints = 0;
            
            // TODO: Triển khai tính toán combo Nạp Âm chi tiết
            // Implement detailed Nap Am combo calculations
            
            // Giả lập cho mục đích demo
            // Simulation for demo purposes
            if (playedCards.Count >= 2)
            {
                // Combo đơn (2 lá bài)
                // Simple combo (2 cards)
                comboPoints += 10;
            }
            
            if (playedCards.Count >= 3)
            {
                // Combo tam hợp (3 lá bài)
                // Tam Hop combo (3 cards)
                comboPoints += 30;
            }
            
            return comboPoints;
        }
        
        /// <summary>
        /// Calculate points from card effects
        /// Tính điểm từ hiệu ứng thẻ bài
        /// </summary>
        private int CalculateEffectPoints(List<Entity> playedCards, Entity target)
        {
            int effectPoints = 0;
            
            foreach (var card in playedCards)
            {
                EffectComponent effectComp = card.GetComponent<EffectComponent>();
                if (effectComp != null)
                {
                    foreach (var effect in effectComp.Effects)
                    {
                        // Lấy điểm sát thương từ hiệu ứng
                        // Get damage points from effect
                        // effectPoints += effect.GetDamageValue();
                    }
                }
            }
            
            return effectPoints;
        }
        
        /// <summary>
        /// Calculate environment modifier based on season and terrain
        /// Tính hệ số môi trường dựa trên mùa và địa hình
        /// </summary>
        private float CalculateEnvironmentModifier(List<Entity> playedCards)
        {
            float environmentModifier = 1.0f;
            
            // Tính hệ số mùa
            // Calculate season modifier
            foreach (var card in playedCards)
            {
                ElementComponent element = card.GetComponent<ElementComponent>();
                if (element != null)
                {
                    float seasonBonus = _elementInteractionSystem.GetSeasonBonus(element.Element, _currentSeason);
                    environmentModifier += seasonBonus / playedCards.Count; // Chia trung bình cho số lá bài
                }
            }
            
            // TODO: Thêm tính toán cho địa hình nếu cần
            // Add terrain calculations if needed
            
            return environmentModifier;
        }
        
        /// <summary>
        /// Calculate rarity modifier based on card rarities
        /// Tính hệ số độ hiếm dựa trên độ hiếm của thẻ bài
        /// </summary>
        private float CalculateRarityModifier(List<Entity> playedCards)
        {
            float totalRarityModifier = 0f;
            
            foreach (var card in playedCards)
            {
                CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                if (cardInfo != null)
                {
                    // Hệ số độ hiếm
                    // Rarity modifier
                    switch (cardInfo.Rarity)
                    {
                        case Rarity.Common:
                            totalRarityModifier += 1.0f;
                            break;
                        case Rarity.Rare:
                            totalRarityModifier += 1.1f;
                            break;
                        case Rarity.Epic:
                            totalRarityModifier += 1.2f;
                            break;
                        case Rarity.Legendary:
                            totalRarityModifier += 1.3f;
                            break;
                    }
                }
            }
            
            // Lấy trung bình hệ số độ hiếm
            // Get average rarity modifier
            return totalRarityModifier / playedCards.Count;
        }
        
        /// <summary>
        /// Get target defense value
        /// Lấy giá trị phòng thủ của mục tiêu
        /// </summary>
        private int GetTargetDefense(Entity target)
        {
            StatsComponent stats = target.GetComponent<StatsComponent>();
            return stats != null ? stats.Defense : 0;
        }
        
        /// <summary>
        /// Calculate damage reduction based on defense
        /// Tính giảm sát thương dựa trên phòng thủ
        /// </summary>
        private float CalculateDamageReduction(int defense)
        {
            // Sử dụng công thức có giá trị giảm dần
            // Use diminishing returns formula
            return (float)defense / (defense + 50);
        }
        
        /// <summary>
        /// Check if the scoring would result in a critical hit
        /// Kiểm tra xem điểm số có dẫn đến đòn chí mạng không
        /// </summary>
        public bool IsCriticalHit(List<Entity> playedCards, Entity target)
        {
            // Logic cho đòn chí mạng, có thể dựa trên các yếu tố như:
            // Logic for critical hits, could be based on factors like:
            // - Mối quan hệ Tương Khắc mạnh (Tương Khắc Kép)
            // - Strong Tương Khắc relationship (Double Tương Khắc) 
            // - Combo Nạp Âm đặc biệt
            // - Special Nap Am combos
            // - Yếu tố ngẫu nhiên
            // - Random factors
            
            // Triển khai đơn giản cho demo
            // Simple implementation for demo
            float critChance = 0.05f; // 5% cơ bản
            
            // Tăng cơ hội chí mạng nếu có Tương Khắc
            // Increase crit chance if there's Tương Khắc
            foreach (var card in playedCards)
            {
                ElementComponent cardElement = card.GetComponent<ElementComponent>();
                ElementComponent targetElement = target.GetComponent<ElementComponent>();
                
                if (cardElement != null && targetElement != null)
                {
                    if (_elementInteractionSystem.HasOvercomingRelationship(cardElement.Element, targetElement.Element))
                    {
                        critChance += 0.1f; // +10% mỗi lá bài có Tương Khắc
                    }
                }
            }
            
            // Kiểm tra xác suất
            // Check probability
            return Random.value < critChance;
        }
        
        /// <summary>
        /// Get score details for UI display
        /// Lấy chi tiết điểm số để hiển thị trên UI
        /// </summary>
        public Dictionary<string, float> GetScoreDetails(List<Entity> playedCards, Entity target)
        {
            Dictionary<string, float> details = new Dictionary<string, float>();
            
            details["BaseScore"] = CalculateBaseScore(playedCards);
            details["ElementModifier"] = CalculateElementModifier(playedCards, target);
            details["NapAmCombo"] = CalculateNapAmComboPoints(playedCards);
            details["EffectPoints"] = CalculateEffectPoints(playedCards, target);
            details["EnvironmentModifier"] = CalculateEnvironmentModifier(playedCards);
            details["RarityModifier"] = CalculateRarityModifier(playedCards);
            details["TargetDefense"] = GetTargetDefense(target);
            details["DamageReduction"] = CalculateDamageReduction(GetTargetDefense(target));
            
            return details;
        }
    }
}