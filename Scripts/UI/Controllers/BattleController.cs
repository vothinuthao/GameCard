using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;
using RunTime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NguhanhGame.UI;
using Systems;
using Systems.States;
using Systems.StatesMachine;

namespace UI
{
    /// <summary>
    /// Controls the battle screen 
    /// </summary>
    public class BattleController : MonoBehaviour
    {
        [Header("Player UI")]
        [SerializeField] private TextMeshProUGUI playerHealthText;
        [SerializeField] private Slider playerHealthSlider;
        [SerializeField] private TextMeshProUGUI playerEnergyText;
        
        [Header("Enemy UI")]
        [SerializeField] private TextMeshProUGUI enemyHealthText;
        [SerializeField] private Slider enemyHealthSlider;
        [SerializeField] private Image enemyImage;
        [SerializeField] private TextMeshProUGUI enemyNameText;
        [SerializeField] private TextMeshProUGUI enemyIntentText;
        
        [Header("Card Areas")]
        [SerializeField] private Transform handContainer;
        [SerializeField] private Transform playZoneContainer;
        [SerializeField] private SupportZoneView supportZoneView;
        [SerializeField] private GameObject cardPrefab;
        
        [Header("Battle Controls")]
        [SerializeField] private Button endTurnButton;
        [SerializeField] private TextMeshProUGUI deckCountText;
        [SerializeField] private TextMeshProUGUI discardCountText;
        [SerializeField] private TextMeshProUGUI turnText;
        [SerializeField] private TextMeshProUGUI seasonText;
        
        [Header("Card Selection")]
        [SerializeField] private GameObject cardSelectionPanel;
        [SerializeField] private TextMeshProUGUI selectionTitleText;
        [SerializeField] private Transform cardSelectionContainer;
        [SerializeField] private Button cancelSelectionButton;
        
        [Header("Battle Log")]
        [SerializeField] private TextMeshProUGUI battleLogText;
        [SerializeField] private ScrollRect battleLogScrollRect;
        
        [Header("Season Controls")]
        [SerializeField] private Button springButton;
        [SerializeField] private Button summerButton;
        [SerializeField] private Button autumnButton;
        [SerializeField] private Button winterButton;
        
        [Header("Restart")]
        [SerializeField] private Button restartButton;
        
        // Internal references
        private List<CardView> handCardViews = new List<CardView>();
        private List<CardView> playZoneCardViews = new List<CardView>();
        private int currentTurn = 1;
        
        // Current selected cards
        private Entity selectedCard1 = null;
        private Entity selectedCard2 = null;
        private int selectedCardIndex1 = -1;
        private int selectedCardIndex2 = -1;
        
        // Game references
        private GameManager _gameManager;
        private CardSystem _cardSystem;
        private BattleSystem _battleSystem;
        private ElementInteractionSystem _elementSystem;
        private SupportCardSystem _supportCardSystem;
        
        /// <summary>
        /// Initialize the controller
        /// </summary>
        private void Start()
        {
            // Get references
            _gameManager = GameManager.Instance;
            if (_gameManager != null)
            {
                _cardSystem = _gameManager.GetCardSystem();
                _battleSystem = _gameManager.GetBattleSystem();
                _elementSystem = _gameManager.GetElementInteractionSystem();
                _supportCardSystem = _gameManager.GetSupportCardSystem();
            }
            
            if (supportZoneView != null)
            {
                supportZoneView.SetBattleController(this);
            }
            
            // Add button listeners
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
            cancelSelectionButton.onClick.AddListener(OnCancelSelectionClicked);
            
            // Add season button listeners
            springButton.onClick.AddListener(() => OnSeasonClicked(Season.Spring));
            summerButton.onClick.AddListener(() => OnSeasonClicked(Season.Summer));
            autumnButton.onClick.AddListener(() => OnSeasonClicked(Season.Autumn));
            winterButton.onClick.AddListener(() => OnSeasonClicked(Season.Winter));
            
            // Add restart button listener
            restartButton.onClick.AddListener(OnRestartClicked);
            
            // Hide card selection panel
            cardSelectionPanel.SetActive(false);
            
            // Start a new game if needed
            if (_gameManager != null && !_gameManager.IsGameInProgress())
            {
                _gameManager.StartNewGame();
            }
            
            // Reset battle state
            ResetBattleState();
            
            // Add first battle log entry
            AddBattleLog("Battle started! Select cards to play.");
        }
        
        /// <summary>
        /// Reset battle state
        /// </summary>
        private void ResetBattleState()
        {
            currentTurn = 1;
            selectedCard1 = null;
            selectedCard2 = null;
            selectedCardIndex1 = -1;
            selectedCardIndex2 = -1;
            battleLogText.text = "";
        }
        
        /// <summary>
        /// Update UI each frame
        /// </summary>
        private void Update()
        {
            UpdateUI();
        }
        
        /// <summary>
        /// Update all UI elements
        /// </summary>
        private void UpdateUI()
        {
            if (_gameManager == null || _cardSystem == null) return;
            
            // Update player stats
            Entity playerEntity = _gameManager.GetPlayerEntity();
            if (playerEntity != null)
            {
                StatsComponent playerStats = playerEntity.GetComponent<StatsComponent>();
                if (playerStats != null)
                {
                    playerHealthText.text = $"{playerStats.Health} / {playerStats.MaxHealth}";
                    playerHealthSlider.maxValue = playerStats.MaxHealth;
                    playerHealthSlider.value = playerStats.Health;
                }
            }
            
            // Update player energy
            int playerEnergy = _gameManager.GetPlayerEnergy();
            int maxPlayerEnergy = _battleSystem.GetMaxPlayerEnergy();
            playerEnergyText.text = $"Energy: {playerEnergy} / {maxPlayerEnergy}";
            
            // Update enemy stats
            Entity enemyEntity = _gameManager.GetEnemyEntity();
            if (enemyEntity != null)
            {
                StatsComponent enemyStats = enemyEntity.GetComponent<StatsComponent>();
                if (enemyStats != null)
                {
                    enemyHealthText.text = $"{enemyStats.Health} / {enemyStats.MaxHealth}";
                    enemyHealthSlider.maxValue = enemyStats.MaxHealth;
                    enemyHealthSlider.value = enemyStats.Health;
                }
                
                // Update enemy name
                CardInfoComponent enemyInfo = enemyEntity.GetComponent<CardInfoComponent>();
                if (enemyInfo != null)
                {
                    enemyNameText.text = enemyInfo.Name;
                }
                
                // Update enemy element color
                ElementComponent enemyElement = enemyEntity.GetComponent<ElementComponent>();
                if (enemyElement != null)
                {
                    // Get color based on element
                    enemyImage.color = GetElementColor(enemyElement.Element);
                }
            }
            
            // Update enemy intent
            // string intentType = _battleSystem.GetEnemyIntentType();
            // int intentValue = _battleSystem.GetEnemyIntentValue();
            // enemyIntentText.text = $"Intent: {intentType} {intentValue}";
            
            // Update hand cards
            UpdateHandCards(_cardSystem.GetHand());
            
            // Update play zone
            UpdatePlayZone(_cardSystem.GetPlayZone());
            
            // Update support zone
            if (supportZoneView != null)
            {
                supportZoneView.SetSupportCards(_cardSystem.GetSupportZone());
                supportZoneView.UpdateSupportCardStatus();
            }
            
            // Update deck and discard counts
            deckCountText.text = $"Deck: {_cardSystem.GetDeck().Count}";
            discardCountText.text = $"Discard: {_cardSystem.GetDiscardPile().Count}";
            
            // Update turn and season
            turnText.text = $"Turn: {currentTurn}";
            seasonText.text = $"Season: {_gameManager.GetCurrentSeason()}";
            
            // Disable end turn button if battle is over
            bool battleOver = _battleSystem.IsBattleOver();
            endTurnButton.interactable = !battleOver;
            
            // Show restart button if battle is over
            restartButton.gameObject.SetActive(battleOver);
        }
        
        /// <summary>
        /// Get color based on element type
        /// </summary>
        private Color GetElementColor(ElementType elementType)
        {
            switch (elementType)
            {
                case ElementType.Metal:
                    return new Color(0.7f, 0.7f, 0.7f);
                case ElementType.Wood:
                    return new Color(0.0f, 0.6f, 0.0f);
                case ElementType.Water:
                    return new Color(0.0f, 0.0f, 0.8f);
                case ElementType.Fire:
                    return new Color(0.8f, 0.0f, 0.0f);
                case ElementType.Earth:
                    return new Color(0.6f, 0.4f, 0.2f);
                default:
                    return Color.gray;
            }
        }
        
        /// <summary>
        /// Update hand cards display
        /// </summary>
        private void UpdateHandCards(List<Entity> handCards)
        {
            // Clear existing cards if count mismatch
            if (handCardViews.Count != handCards.Count)
            {
                ClearContainer(handContainer);
                handCardViews.Clear();
                
                // Create new card views
                for (int i = 0; i < handCards.Count; i++)
                {
                    GameObject cardGO = Instantiate(cardPrefab, handContainer);
                    CardView cardView = cardGO.GetComponent<CardView>();
                    cardView.SetCard(handCards[i], i);
                    
                    int index = i;
                    Button cardButton = cardGO.GetComponent<Button>();
                    cardButton.onClick.AddListener(() => OnHandCardClicked(index));
                    
                    handCardViews.Add(cardView);
                }
            }
            else
            {
                // Update existing card views
                for (int i = 0; i < handCards.Count; i++)
                {
                    handCardViews[i].SetCard(handCards[i], i);
                }
            }
        }
        
        /// <summary>
        /// Update play zone display
        /// </summary>
        private void UpdatePlayZone(List<Entity> playZoneCards)
        {
            // Clear existing cards if count mismatch
            if (playZoneCardViews.Count != playZoneCards.Count)
            {
                ClearContainer(playZoneContainer);
                playZoneCardViews.Clear();
                
                // Create new card views
                for (int i = 0; i < playZoneCards.Count; i++)
                {
                    GameObject cardGO = Instantiate(cardPrefab, playZoneContainer);
                    CardView cardView = cardGO.GetComponent<CardView>();
                    cardView.SetCard(playZoneCards[i]);
                    playZoneCardViews.Add(cardView);
                }
            }
            else
            {
                // Update existing card views
                for (int i = 0; i < playZoneCards.Count; i++)
                {
                    playZoneCardViews[i].SetCard(playZoneCards[i]);
                }
            }
        }
        
        /// <summary>
        /// Clear all child objects from a container
        /// </summary>
        private void ClearContainer(Transform container)
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
        }
        
        /// <summary>
        /// Handle hand card clicked
        /// </summary>
        private void OnHandCardClicked(int cardIndex)
        {
            if (_cardSystem == null) return;
            
            var hand = _cardSystem.GetHand();
            if (cardIndex < 0 || cardIndex >= hand.Count) return;
            
            Entity card = hand[cardIndex];
            
            // Get card cost
            int cardCost = GetCardCost(card);
            
            // Check if we have enough energy
            int playerEnergy = _gameManager.GetPlayerEnergy();
            if (playerEnergy < cardCost)
            {
                AddBattleLog("Not enough energy to play this card!");
                return;
            }
            
            // Check if this is a support card
            if (card.HasComponent<SupportCardComponent>())
            {
                // Check if support zone has space
                if (supportZoneView != null && supportZoneView.CanPlaySupportCard())
                {
                    // Play as support
                    _cardSystem.PlayAsSupport(card);
                    _gameManager.SetPlayerEnergy(playerEnergy - cardCost);
                    
                    CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                    AddBattleLog($"Played {cardInfo?.Name ?? "card"} as support card.");
                    
                    // Reset selection if this card was selected
                    if (cardIndex == selectedCardIndex1 || cardIndex == selectedCardIndex2)
                    {
                        ResetCardSelection();
                    }
                    return;
                }
                else
                {
                    AddBattleLog("Support zone is full!");
                    return;
                }
            }
            
            // Card selection logic
            if (selectedCard1 == null)
            {
                // First card selected
                selectedCard1 = card;
                selectedCardIndex1 = cardIndex;
                
                CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                AddBattleLog($"Selected {cardInfo?.Name ?? "card"}.");
                
                // Show confirmation for single card play
                ShowCardSelectionPanel("Play this card?", new List<Entity> { card }, _ => {
                    PlaySelectedCards();
                });
            }
            else if (selectedCardIndex1 != cardIndex)
            {
                // Second card selected
                selectedCard2 = card;
                selectedCardIndex2 = cardIndex;
                
                CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                AddBattleLog($"Selected {cardInfo?.Name ?? "card"} as second card.");
                
                // Check if total cost is affordable
                int totalCost = GetCardCost(selectedCard1) + GetCardCost(selectedCard2);
                if (playerEnergy < totalCost)
                {
                    AddBattleLog("Not enough energy to play both cards!");
                    selectedCard2 = null;
                    selectedCardIndex2 = -1;
                    return;
                }
                
                // Show confirmation for combo
                ShowCardSelectionPanel("Play these cards?", new List<Entity> { selectedCard1, selectedCard2 }, _ => {
                    PlaySelectedCards();
                });
            }
            else
            {
                // Deselect first card
                selectedCard1 = null;
                selectedCardIndex1 = -1;
                AddBattleLog("Deselected card.");
                
                // Hide confirmation
                cardSelectionPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// Show card selection panel
        /// </summary>
        private void ShowCardSelectionPanel(string title, List<Entity> cards, System.Action<int> onCardSelected)
        {
            // Set title
            selectionTitleText.text = title;
            
            // Clear container
            ClearContainer(cardSelectionContainer);
            
            // Add cards
            for (int i = 0; i < cards.Count; i++)
            {
                GameObject cardGO = Instantiate(cardPrefab, cardSelectionContainer);
                CardView cardView = cardGO.GetComponent<CardView>();
                cardView.SetCard(cards[i]);
                
                // Add click listener
                int index = i; // Capture index
                Button cardButton = cardGO.GetComponent<Button>();
                cardButton.onClick.AddListener(() => {
                    onCardSelected?.Invoke(index);
                    cardSelectionPanel.SetActive(false);
                });
            }
            
            // Show panel
            cardSelectionPanel.SetActive(true);
        }
        
        /// <summary>
        /// Handle cancel selection clicked
        /// </summary>
        private void OnCancelSelectionClicked()
        {
            ResetCardSelection();
            cardSelectionPanel.SetActive(false);
            AddBattleLog("Card selection canceled.");
        }
        
        /// <summary>
        /// Reset card selection
        /// </summary>
        private void ResetCardSelection()
        {
            selectedCard1 = null;
            selectedCard2 = null;
            selectedCardIndex1 = -1;
            selectedCardIndex2 = -1;
        }
        
        /// <summary>
        /// Play selected cards
        /// </summary>
        private void PlaySelectedCards()
        {
            if (_gameManager == null || _battleSystem == null || _cardSystem == null) return;
            
            Entity enemyEntity = _gameManager.GetEnemyEntity();
            if (enemyEntity == null) return;
            
            // Calculate total cost
            int totalCost = 0;
            
            // Play first card
            if (selectedCard1 != null)
            {
                int cardCost = GetCardCost(selectedCard1);
                totalCost += cardCost;
                
                // Add to battle system's selected cards
                _battleSystem.SelectCard(selectedCard1);
                
                CardInfoComponent cardInfo = selectedCard1.GetComponent<CardInfoComponent>();
                AddBattleLog($"Played {cardInfo?.Name ?? "card"}.");
                
                // Log element interaction
                LogElementInteraction(selectedCard1, enemyEntity);
            }
            
            // Play second card
            if (selectedCard2 != null)
            {
                int cardCost = GetCardCost(selectedCard2);
                totalCost += cardCost;
                
                // Add to battle system's selected cards
                _battleSystem.SelectCard(selectedCard2);
                
                CardInfoComponent cardInfo = selectedCard2.GetComponent<CardInfoComponent>();
                AddBattleLog($"Played {cardInfo?.Name ?? "card"}.");
                
                // Log element interaction
                LogElementInteraction(selectedCard2, enemyEntity);
            }
            
            // Deduct energy
            int playerEnergy = _gameManager.GetPlayerEnergy();
            _gameManager.SetPlayerEnergy(playerEnergy - totalCost);
            
            // Change state to card resolution
            _battleSystem.GetStateMachine().ChangeState(BattleStateType.CardResolution);
            
            // Check if battle is over
            if (_battleSystem.IsBattleOver())
            {
                Entity winner = _battleSystem.GetWinner();
                
                if (winner == _gameManager.GetPlayerEntity())
                {
                    AddBattleLog("Victory! You defeated the enemy.");
                }
                else
                {
                    AddBattleLog("Defeat! You were defeated by the enemy.");
                }
            }
            
            // Reset selection
            ResetCardSelection();
        }
        
        /// <summary>
        /// Log element interaction details
        /// </summary>
        private void LogElementInteraction(Entity card, Entity target)
        {
            if (_elementSystem == null) return;
            
            ElementComponent cardElement = card.GetComponent<ElementComponent>();
            ElementComponent targetElement = target.GetComponent<ElementComponent>();
            
            if (cardElement != null && targetElement != null)
            {
                string interaction = $"{cardElement.GetElementName()} vs {targetElement.GetElementName()}: ";
                
                if (_elementSystem.HasGeneratingRelationship(cardElement.Element, targetElement.Element))
                {
                    interaction += "+30% effectiveness (Tương Sinh)";
                    AddBattleLog(interaction);
                }
                else if (_elementSystem.HasOvercomingRelationship(cardElement.Element, targetElement.Element))
                {
                    interaction += "+50% damage (Tương Khắc)";
                    AddBattleLog(interaction);
                }
                else if (_elementSystem.HasOvercomingRelationship(targetElement.Element, cardElement.Element))
                {
                    interaction += "Disadvantage! Enemy has element advantage";
                    AddBattleLog(interaction);
                }
            }
        }
        
        /// <summary>
        /// Get card cost
        /// </summary>
        private int GetCardCost(Entity card)
        {
            CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
            return cardInfo != null ? cardInfo.Cost : 1;
        }
        
        /// <summary>
        /// Handle end turn button click
        /// </summary>
        private void OnEndTurnClicked()
        {
            if (_battleSystem == null) return;
            
            // End turn in game manager
            _gameManager.EndTurn();
            
            // Update turn counter
            currentTurn++;
            
            // Reset card selection
            ResetCardSelection();
            
            AddBattleLog($"Started turn {currentTurn}.");
        }
        
        /// <summary>
        /// Handle season button click
        /// </summary>
        private void OnSeasonClicked(Season season)
        {
            if (_gameManager == null) return;
            
            _gameManager.SetSeason(season);
            AddBattleLog($"Changed season to {season}.");
        }
        
        /// <summary>
        /// Handle restart button click
        /// </summary>
        private void OnRestartClicked()
        {
            if (_gameManager == null) return;
            
            // Reset battle state
            ResetBattleState();
            
            // Start a new game
            _gameManager.StartNewGame();
            
            // Add first battle log entry
            AddBattleLog("Started a new battle!");
        }
        
        /// <summary>
        /// Add entry to battle log
        /// </summary>
        private void AddBattleLog(string message)
        {
            battleLogText.text += $"\n{message}";
            
            // Scroll to bottom
            Canvas.ForceUpdateCanvases();
            battleLogScrollRect.verticalNormalizedPosition = 0f;
        }
        
        /// <summary>
        /// Add a method for BattleSystem to call
        /// </summary>
        public void OnBattleEvent(string eventType, string message)
        {
            AddBattleLog(message);
            
            // Update the UI immediately
            UpdateUI();
        }
    }
}