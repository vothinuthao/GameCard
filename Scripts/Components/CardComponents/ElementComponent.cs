using Core;
using Core.Utils;

namespace Components
{
    /// <summary>
    /// Represents the elemental aspect of a card (Ngũ Hành) with integrated Nạp Âm
    /// </summary>
    public class ElementComponent : Component
    {
        // Element related data
        public ElementType Element { get; set; }
        public int NapAmIndex { get; private set; }
        
        /// <summary>
        /// Default constructor with element only
        /// </summary>
        public ElementComponent(ElementType element)
        {
            Element = element;
            NapAmIndex = 1; // Default to first nap am type
        }
        
        /// <summary>
        /// Constructor with specified Nạp Âm index
        /// </summary>
        public ElementComponent(ElementType element, int napAmIndex)
        {
            Element = element;
            SetNapAmIndex(napAmIndex);
        }

        /// <summary>
        /// Returns the bonus damage against another element based on Wu Xing rules
        /// </summary>
        public float GetElementBonus(ElementType targetElement)
        {
            // Tương khắc: +50% damage
            switch (Element)
            {
                case ElementType.Metal when targetElement == ElementType.Wood:
                case ElementType.Wood when targetElement == ElementType.Earth:
                case ElementType.Earth when targetElement == ElementType.Water:
                case ElementType.Water when targetElement == ElementType.Fire:
                case ElementType.Fire when targetElement == ElementType.Metal:
                    return 0.5f; // 50% bonus damage
                default:
                    return 0f;
            }
        }
        
        /// <summary>
        /// Returns the synergy bonus when combined with another element
        /// </summary>
        public float GetElementSynergy(ElementType targetElement)
        {
            // Tương sinh: +30% effectiveness
            switch (Element)
            {
                case ElementType.Metal when targetElement == ElementType.Water:
                case ElementType.Water when targetElement == ElementType.Wood:
                case ElementType.Wood when targetElement == ElementType.Fire:
                case ElementType.Fire when targetElement == ElementType.Earth:
                case ElementType.Earth when targetElement == ElementType.Metal:
                    return 0.3f; // 30% synergy bonus
                default:
                    return 0f;
            }
        }
        
        /// <summary>
        /// Gets the Vietnamese name for this element
        /// </summary>
        public string GetElementName()
        {
            switch (Element)
            {
                case ElementType.Metal:
                    return "Kim";
                case ElementType.Wood:
                    return "Mộc";
                case ElementType.Water:
                    return "Thủy";
                case ElementType.Fire:
                    return "Hỏa";
                case ElementType.Earth:
                    return "Thổ";
                default:
                    return "Unknown";
            }
        }
        
        /// <summary>
        /// Set Nạp Âm index for this element
        /// </summary>
        public void SetNapAmIndex(int napAmIndex)
        {
            // Ensure index is between 1 and 6
            if (napAmIndex < 1) NapAmIndex = 1;
            else if (napAmIndex > 6) NapAmIndex = 6;
            else NapAmIndex = napAmIndex;
        }
    }
    
    /// <summary>
    /// Data structure for element combo effects
    /// </summary>
    public class ComboData
    {
        public string Name { get; private set; }
        public int BonusPoints { get; private set; }
        public string Effect { get; private set; }
        
        public ComboData(string name, int bonusPoints, string effect)
        {
            Name = name;
            BonusPoints = bonusPoints;
            Effect = effect;
        }
    }
}