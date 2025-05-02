using System.Collections.Generic;
using Components;
using Core;
using RunTime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UI;

namespace NguhanhGame.UI
{
    /// <summary>
    /// UI component for displaying and managing the support zone
    /// </summary>
    public class SupportZoneView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform supportCardContainer;
        [SerializeField] private GameObject supportCardPrefab;
        [SerializeField] private TextMeshProUGUI supportZoneTitle;
        [SerializeField] private GameObject supportZoneInfoPanel;
        [SerializeField] private TextMeshProUGUI supportZoneInfoText;
        
        [Header("Configuration")]
        [SerializeField] private int maxSupportCards = 6;
        [SerializeField] private float cardSpacing = 110f;
        
        // Internal references
        private List<CardView> _supportCardViews = new List<CardView>();
        private List<Entity> _supportCards = new List<Entity>();
        
        // Start is called before the first frame update
        private void Start()
        {
            // Initialize support zone
            UpdateUI();
            
            // Set title
            if (supportZoneTitle != null)
            {
                supportZoneTitle.text = "Support Cards (0/" + maxSupportCards + ")";
            }
            
            // Hide info panel initially
            if (supportZoneInfoPanel != null)
            {
                supportZoneInfoPanel.SetActive(false);
            }
        }
        
        // Update is called once per frame
        private void Update()
        {
            // Update UI periodically
            UpdateUI();
        }
        
        /// <summary>
        /// Update the UI to reflect current support cards
        /// </summary>
        private void UpdateUI()
        {
            if (GameManager.Instance == null || supportCardContainer == null) return;
            
            // Get current support cards from card system
            var cardSystem = GameManager.Instance.GetCardSystem();
            if (cardSystem == null) return;
            
            var currentSupportCards = cardSystem.GetSupportZone();
            
            // Check if support cards have changed
            bool cardsChanged = currentSupportCards.Count != _supportCards.Count;
            if (!cardsChanged)
            {
                for (int i = 0; i < currentSupportCards.Count; i++)
                {
                    if (i >= _supportCards.Count || currentSupportCards[i] != _supportCards[i])
                    {
                        cardsChanged = true;
                        break;
                    }
                }
            }
            
            // Update support cards if changed
            if (cardsChanged)
            {
                _supportCards = new List<Entity>(currentSupportCards);
                RefreshSupportCardViews();
                
                // Update title
                if (supportZoneTitle != null)
                {
                    supportZoneTitle.text = "Support Cards (" + _supportCards.Count + "/" + maxSupportCards + ")";
                }
            }
            
            // Update activation status
            for (int i = 0; i < _supportCardViews.Count; i++)
            {
                if (i < _supportCards.Count)
                {
                    Entity card = _supportCards[i];
                    SupportCardComponent supportComponent = card.GetComponent<SupportCardComponent>();
                    
                    if (supportComponent != null)
                    {
                        // Update cooldown display
                        _supportCardViews[i].UpdateCooldown(supportComponent.CurrentCooldown);
                        
                        // Update activation status
                        _supportCardViews[i].UpdateActivationStatus(supportComponent.IsActive);
                    }
                }
            }
        }
        
        /// <summary>
        /// Refresh support card views
        /// </summary>
        private void RefreshSupportCardViews()
        {
            // Clear existing views
            foreach (var cardView in _supportCardViews)
            {
                Destroy(cardView.gameObject);
            }
            _supportCardViews.Clear();
            
            // Create new views
            for (int i = 0; i < _supportCards.Count; i++)
            {
                GameObject cardGO = Instantiate(supportCardPrefab, supportCardContainer);
                
                // Position the card
                RectTransform rectTransform = cardGO.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = new Vector2(i * cardSpacing, 0);
                }
                
                // Set up card view
                CardView cardView = cardGO.GetComponent<CardView>();
                if (cardView != null)
                {
                    cardView.SetCard(_supportCards[i]);
                    
                    // Add click listener to show info
                    Button cardButton = cardGO.GetComponent<Button>();
                    if (cardButton != null)
                    {
                        int index = i; // Capture index for closure
                        cardButton.onClick.AddListener(() => OnSupportCardClicked(index));
                    }
                    
                    _supportCardViews.Add(cardView);
                }
            }
        }
        
        /// <summary>
        /// Handle support card click
        /// </summary>
        private void OnSupportCardClicked(int index)
        {
            if (index < 0 || index >= _supportCards.Count) return;
            
            // Get card
            Entity card = _supportCards[index];
            
            // Show info panel
            if (supportZoneInfoPanel != null && supportZoneInfoText != null)
            {
                // Get card components
                CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                SupportCardComponent supportComponent = card.GetComponent<SupportCardComponent>();
                
                if (cardInfo != null && supportComponent != null)
                {
                    // Build info text
                    string infoText = "<b>" + cardInfo.Name + "</b>\n\n";
                    infoText += cardInfo.Description + "\n\n";
                    
                    // Add activation type
                    infoText += "<b>Activation Type:</b> " + supportComponent.ActivationType.ToString() + "\n";
                    
                    // Add activation condition
                    if (supportComponent.Condition != null)
                    {
                        infoText += "<b>Condition:</b> " + supportComponent.Condition.Description + "\n";
                    }
                    
                    // Add effect
                    if (supportComponent.Effect != null)
                    {
                        infoText += "<b>Effect:</b> " + supportComponent.Effect.Description + "\n";
                    }
                    
                    // Add cooldown
                    if (supportComponent.CooldownTime > 0)
                    {
                        infoText += "<b>Cooldown:</b> " + supportComponent.CooldownTime + " turns\n";
                        
                        if (supportComponent.CurrentCooldown > 0)
                        {
                            infoText += "<b>Current Cooldown:</b> " + supportComponent.CurrentCooldown + " turns\n";
                        }
                    }
                    
                    // Add activation status
                    infoText += "<b>Status:</b> " + (supportComponent.IsActive ? "Active" : "Inactive");
                    
                    // Set info text
                    supportZoneInfoText.text = infoText;
                    
                    // Show panel
                    supportZoneInfoPanel.SetActive(true);
                }
            }
        }
        
        /// <summary>
        /// Hide info panel
        /// </summary>
        public void HideInfoPanel()
        {
            if (supportZoneInfoPanel != null)
            {
                supportZoneInfoPanel.SetActive(false);
            }
        }
    }
}