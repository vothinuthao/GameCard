using Components;
using Core;
using Core.Utils;
using RunTime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    /// <summary>
    /// Visual representation of a card in Unity
    /// Updated to support support cards with cooldown and activation status
    /// </summary>
    public class CardView : MonoBehaviour
    {
        [Header("Card References")]
        [SerializeField] private Image cardBackground;
        [SerializeField] private Image cardArtwork;
        [SerializeField] private Image cardFrame;
        [SerializeField] private TextMeshProUGUI cardName;
        [SerializeField] private TextMeshProUGUI cardDescription;
        [SerializeField] private TextMeshProUGUI elementText;
        [SerializeField] private TextMeshProUGUI napAmText;
        [SerializeField] private TextMeshProUGUI costText;
        
        [Header("Stat References")]
        [SerializeField] private TextMeshProUGUI attackText;
        [SerializeField] private TextMeshProUGUI defenseText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI speedText;
        
        [Header("Support Card References")]
        [SerializeField] private GameObject cooldownPanel;
        [SerializeField] private TextMeshProUGUI cooldownText;
        [SerializeField] private GameObject activationIndicator;
        [SerializeField] private TextMeshProUGUI activationTypeText;
        
        [Header("Visual Customization")]
        [SerializeField] private Color metalColor = new Color(0.7f, 0.7f, 0.7f);
        [SerializeField] private Color woodColor = new Color(0.0f, 0.6f, 0.0f);
        [SerializeField] private Color waterColor = new Color(0.0f, 0.0f, 0.8f);
        [SerializeField] private Color fireColor = new Color(0.8f, 0.0f, 0.0f);
        [SerializeField] private Color earthColor = new Color(0.6f, 0.4f, 0.2f);
        
        [Header("Rarity Frames")]
        [SerializeField] private Sprite commonFrame;
        [SerializeField] private Sprite rareFrame;
        [SerializeField] private Sprite epicFrame;
        [SerializeField] private Sprite legendaryFrame;
        
        [Header("Support Card Type Indicators")]
        [SerializeField] private Sprite divineBeastIndicator;
        [SerializeField] private Sprite monsterIndicator;
        [SerializeField] private Sprite spiritAnimalIndicator;
        [SerializeField] private Sprite artifactIndicator;
        [SerializeField] private Sprite talismanIndicator;
        [SerializeField] private Sprite divineWeaponIndicator;
        
        // Reference to the entity
        private Entity _cardEntity;
        private int _handIndex = -1;
        
        /// <summary>
        /// Set the card entity to display
        /// </summary>
        public void SetCard(Entity cardEntity, int index = -1)
        {
            _cardEntity = cardEntity;
            _handIndex = index;
            UpdateVisuals();
        }
        
        /// <summary>
        /// Update the visual representation of the card
        /// </summary>
        private void UpdateVisuals()
        {
            if (_cardEntity == null)
            {
                Debug.LogError("CardView: Card entity is null!");
                return;
            }
            
            // Get components
            CardInfoComponent cardInfo = _cardEntity.GetComponent<CardInfoComponent>();
            ElementComponent element = _cardEntity.GetComponent<ElementComponent>();
            StatsComponent stats = _cardEntity.GetComponent<StatsComponent>();
            SupportCardComponent supportCard = _cardEntity.GetComponent<SupportCardComponent>();
            
            if (cardInfo == null)
            {
                Debug.LogError("CardView: Card info component is null!");
                return;
            }
            
            // Set basic info
            if (cardName != null) cardName.text = cardInfo.Name;
            if (cardDescription != null) cardDescription.text = cardInfo.Description;
            if (costText != null) costText.text = cardInfo.Cost.ToString();
            
            // Set artwork
            if (cardArtwork != null)
            {
                if (cardInfo.Artwork != null)
                {
                    cardArtwork.sprite = cardInfo.Artwork;
                    cardArtwork.gameObject.SetActive(true);
                }
                else
                {
                    // Use placeholder artwork based on card type
                    cardArtwork.gameObject.SetActive(true);
                }
            }
            
            // Set frame based on rarity
            if (cardFrame != null)
            {
                switch (cardInfo.Rarity)
                {
                    case Rarity.Common:
                        cardFrame.sprite = commonFrame;
                        break;
                    case Rarity.Rare:
                        cardFrame.sprite = rareFrame;
                        break;
                    case Rarity.Epic:
                        cardFrame.sprite = epicFrame;
                        break;
                    case Rarity.Legendary:
                        cardFrame.sprite = legendaryFrame;
                        break;
                }
            }
            
            // Set element
            if (element != null && elementText != null)
            {
                elementText.text = element.GetElementName();
                
                // Set color based on element
                if (cardBackground != null)
                {
                    switch (element.Element)
                    {
                        case ElementType.Metal:
                            cardBackground.color = metalColor;
                            break;
                        case ElementType.Wood:
                            cardBackground.color = woodColor;
                            break;
                        case ElementType.Water:
                            cardBackground.color = waterColor;
                            break;
                        case ElementType.Fire:
                            cardBackground.color = fireColor;
                            break;
                        case ElementType.Earth:
                            cardBackground.color = earthColor;
                            break;
                    }
                }
            }
            else if (elementText != null)
            {
                elementText.text = string.Empty;
            }
            
            // Set NapAm
            // if (napAm != null && napAmText != null)
            // {
            //     napAmText.text = napAm.GetNapAmVietnameseName();
            // }
            else if (napAmText != null)
            {
                napAmText.text = string.Empty;
            }
            
            // Set stats
            if (stats != null)
            {
                if (attackText != null) attackText.text = stats.Attack.ToString();
                if (defenseText != null) defenseText.text = stats.Defense.ToString();
                if (healthText != null) healthText.text = stats.Health.ToString();
                if (speedText != null) speedText.text = stats.Speed.ToString();
            }
            else
            {
                if (attackText != null) attackText.text = "0";
                if (defenseText != null) defenseText.text = "0";
                if (healthText != null) healthText.text = "0";
                if (speedText != null) speedText.text = "0";
            }
            
            // Set support card info
            if (supportCard != null)
            {
                // Show support card-specific UI elements
                if (cooldownPanel != null) cooldownPanel.SetActive(supportCard.CooldownTime > 0);
                if (cooldownText != null) cooldownText.text = supportCard.CurrentCooldown.ToString();
                if (activationIndicator != null) activationIndicator.SetActive(supportCard.IsActive);
                
                // Set activation type text
                if (activationTypeText != null)
                {
                    activationTypeText.text = GetShortActivationType(supportCard.ActivationType);
                }
                
                // Add a support card type indicator based on the card type
                // if (cardArtwork != null && cardInfo.Type != CardType.ElementalCard)
                // {
                //     switch (cardInfo.Type)
                //     {
                //         case CardType.DivineBeast:
                //             cardArtwork.sprite = divineBeastIndicator;
                //             break;
                //         case CardType.Monster:
                //             cardArtwork.sprite = monsterIndicator;
                //             break;
                //         case CardType.SpiritAnimal:
                //             cardArtwork.sprite = spiritAnimalIndicator;
                //             break;
                //         // For now, we're using SpiritAnimal type for artifacts, talismans, and divine weapons
                //         // In a full implementation, these would have their own CardType
                //     }
                // }
            }
            else
            {
                // Hide support card-specific UI elements
                if (cooldownPanel != null) cooldownPanel.SetActive(false);
                if (activationIndicator != null) activationIndicator.SetActive(false);
                if (activationTypeText != null) activationTypeText.text = string.Empty;
            }
        }
        
        /// <summary>
        /// Update the cooldown display
        /// </summary>
        public void UpdateCooldown(int cooldown)
        {
            if (cooldownPanel != null) cooldownPanel.SetActive(cooldown > 0);
            if (cooldownText != null) cooldownText.text = cooldown.ToString();
        }
        
        /// <summary>
        /// Update the activation status
        /// </summary>
        public void UpdateActivationStatus(bool isActive)
        {
            if (activationIndicator != null) activationIndicator.SetActive(isActive);
        }
        
        /// <summary>
        /// Get a short form of the activation type
        /// </summary>
        private string GetShortActivationType(ActivationType type)
        {
            switch (type)
            {
                case ActivationType.OnEntry:
                    return "Entry";
                case ActivationType.Persistent:
                    return "Persist";
                case ActivationType.Recurring:
                    return "Recur";
                case ActivationType.Triggered:
                    return "Trigger";
                case ActivationType.Reactive:
                    return "React";
                case ActivationType.Transformative:
                    return "Trans";
                default:
                    return type.ToString();
            }
        }
        
        /// <summary>
        /// Handle card being clicked
        /// </summary>
        public void OnCardClicked()
        {
            if (_handIndex >= 0)
            {
                // Call GameManager to play the card
                GameManager.Instance.PlayCard(_handIndex);
            }
        }
        
        /// <summary>
        /// Handle right-click to play as support
        /// </summary>
        public void OnCardRightClicked()
        {
            if (_handIndex >= 0)
            {
                // Check if card has SupportCardComponent
                if (_cardEntity != null && _cardEntity.HasComponent<SupportCardComponent>())
                {
                    // Call GameManager to play as support
                    GameManager.Instance.PlayAsSupport(_handIndex);
                }
            }
        }
    }
}