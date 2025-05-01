using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;

namespace Systems
{
    /// <summary>
    /// System that handles element interactions such as Tương Sinh (generating) and Tương Khắc (overcoming)
    /// </summary>
    public class ElementInteractionSystem : Core.System
    {
        private Season _currentSeason = Season.Spring;
        
        // Constructor
        public ElementInteractionSystem(EntityManager entityManager) : base(entityManager)
        {
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
        /// </summary>
        public void SetSeason(Season season)
        {
            _currentSeason = season;
        }
        
        /// <summary>
        /// Calculate the element interactions for a set of cards
        /// </summary>
        public float CalculateElementInteraction(List<Entity> cards)
        {
            float totalBonus = 0f;
            
            // Check for element synergies between cards
            for (int i = 0; i < cards.Count; i++)
            {
                for (int j = 0; j < cards.Count; j++)
                {
                    if (i == j) continue;
                    
                    ElementComponent element1 = cards[i].GetComponent<ElementComponent>();
                    ElementComponent element2 = cards[j].GetComponent<ElementComponent>();
                    
                    if (element1 != null && element2 != null)
                    {
                        totalBonus += element1.GetElementSynergy(element2.Element);
                    }
                }
            }
            
            // Apply season effects
            foreach (var card in cards)
            {
                ElementComponent element = card.GetComponent<ElementComponent>();
                if (element != null)
                {
                    totalBonus += GetSeasonBonus(element.Element, _currentSeason);
                }
            }
            
            return totalBonus;
        }
        
        /// <summary>
        /// Calculate the element advantage for an attack
        /// </summary>
        public float CalculateElementAdvantage(Entity attacker, Entity defender)
        {
            ElementComponent attackerElement = attacker.GetComponent<ElementComponent>();
            ElementComponent defenderElement = defender.GetComponent<ElementComponent>();
            
            if (attackerElement == null || defenderElement == null)
                return 0f;
            
            return attackerElement.GetElementBonus(defenderElement.Element);
        }
        
        /// <summary>
        /// Get the bonus/penalty from the current season for an element
        /// </summary>
        public float GetSeasonBonus(ElementType element, Season season)
        {
            switch (season)
            {
                case Season.Spring:
                    // Spring: Wood +20%, Metal -10%
                    if (element == ElementType.Wood) return 0.2f;
                    if (element == ElementType.Metal) return -0.1f;
                    break;
                case Season.Summer:
                    // Summer: Fire +20%, Water -10%
                    if (element == ElementType.Fire) return 0.2f;
                    if (element == ElementType.Water) return -0.1f;
                    break;
                case Season.Autumn:
                    // Autumn: Metal +20%, Wood -10%
                    if (element == ElementType.Metal) return 0.2f;
                    if (element == ElementType.Wood) return -0.1f;
                    break;
                case Season.Winter:
                    // Winter: Water +20%, Fire -10%
                    if (element == ElementType.Water) return 0.2f;
                    if (element == ElementType.Fire) return -0.1f;
                    break;
            }
            
            return 0f;
        }
        
        /// <summary>
        /// Check if two elements have a generating relationship (tương sinh)
        /// </summary>
        public bool HasGeneratingRelationship(ElementType source, ElementType target)
        {
            return (source == ElementType.Metal && target == ElementType.Water) ||
                   (source == ElementType.Water && target == ElementType.Wood) ||
                   (source == ElementType.Wood && target == ElementType.Fire) ||
                   (source == ElementType.Fire && target == ElementType.Earth) ||
                   (source == ElementType.Earth && target == ElementType.Metal);
        }
        
        /// <summary>
        /// Check if two elements have an overcoming relationship (tương khắc)
        /// </summary>
        public bool HasOvercomingRelationship(ElementType source, ElementType target)
        {
            return (source == ElementType.Metal && target == ElementType.Wood) ||
                   (source == ElementType.Wood && target == ElementType.Earth) ||
                   (source == ElementType.Earth && target == ElementType.Water) ||
                   (source == ElementType.Water && target == ElementType.Fire) ||
                   (source == ElementType.Fire && target == ElementType.Metal);
        }
    }
}