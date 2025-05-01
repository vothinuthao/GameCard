using Core;
using Core.Utils;

namespace Components
{
    /// <summary>
    /// Represents the elemental aspect of a card (Ngũ Hành)
    /// </summary>
    public class ElementComponent : Component
    {
        // Element related data
        public ElementType Element { get; set; }
        
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
    }
}