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
        
        // Reference to the entity
        private Entity cardEntity;
        private int handIndex = -1;
        
        /// <summary>
        /// Set the card entity to display
        /// </summary>
        public void SetCard(Entity cardEntity, int index = -1)
        {
            this.cardEntity = cardEntity;
            this.handIndex = index;
            UpdateVisuals();
        }
        
        /// <summary>
        /// Update the visual representation of the card
        /// </summary>
        private void UpdateVisuals()
        {
            if (cardEntity == null)
            {
                Debug.LogError("CardView: Card entity is null!");
                return;
            }
            
            // Get components
            CardInfoComponent cardInfo = cardEntity.GetComponent<CardInfoComponent>();
            ElementComponent element = cardEntity.GetComponent<ElementComponent>();
            NapAmComponent napAm = cardEntity.GetComponent<NapAmComponent>();
            StatsComponent stats = cardEntity.GetComponent<StatsComponent>();
            
            if (cardInfo == null)
            {
                Debug.LogError("CardView: Card info component is null!");
                return;
            }
            
            // Set basic info
            cardName.text = cardInfo.Name;
            cardDescription.text = cardInfo.Description;
            costText.text = cardInfo.Cost.ToString();
            
            // Set artwork
            if (cardInfo.Artwork != null)
            {
                cardArtwork.sprite = cardInfo.Artwork;
                cardArtwork.gameObject.SetActive(true);
            }
            else
            {
                cardArtwork.gameObject.SetActive(false);
            }
            
            // Set frame based on rarity
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
            
            // Set element
            if (element != null)
            {
                elementText.text = element.GetElementName();
                
                // Set color based on element
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
            else
            {
                elementText.text = string.Empty;
            }
            
            // Set NapAm
            if (napAm != null)
            {
                napAmText.text = napAm.GetNapAmVietnameseName();
            }
            else
            {
                napAmText.text = string.Empty;
            }
            
            // Set stats
            if (stats != null)
            {
                attackText.text = stats.Attack.ToString();
                defenseText.text = stats.Defense.ToString();
                healthText.text = stats.Health.ToString();
                speedText.text = stats.Speed.ToString();
            }
            else
            {
                attackText.text = "0";
                defenseText.text = "0";
                healthText.text = "0";
                speedText.text = "0";
            }
        }
        
        /// <summary>
        /// Handle card being clicked
        /// </summary>
        public void OnCardClicked()
        {
            if (handIndex >= 0)
            {
                // Call GameManager to play the card
                GameManager.Instance.PlayCard(handIndex);
            }
        }
        
        /// <summary>
        /// Handle right-click to play as support
        /// </summary>
        public void OnCardRightClicked()
        {
            if (handIndex >= 0)
            {
                // Call GameManager to play as support
                GameManager.Instance.PlayAsSupport(handIndex);
            }
        }
    }
}