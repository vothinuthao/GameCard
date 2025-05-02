// BattleController.cs

using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;
using RunTime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private Transform supportZoneContainer;
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
        private List<CardView> supportZoneCardViews = new List<CardView>();
        private int currentTurn = 1;
        private int playerEnergy = 3;
        private int maxPlayerEnergy = 3;
        
        // Current selected cards
        private Entity selectedCard1 = null;
        private Entity selectedCard2 = null;
        private int selectedCardIndex1 = -1;
        private int selectedCardIndex2 = -1;
        
        // Enemy intent
        private string currentEnemyIntent = "Attack";
        private int currentEnemyIntentValue = 5;
        
        // Start is called before the first frame update
        private void Start()
        {
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
            
            // Start a new game
            GameManager.Instance.StartNewGame();
            
            // Reset battle state
            ResetBattleState();
        }
        
        // OnEnable is called when the gameObject becomes enabled and active
        private void OnEnable()
        {
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
            playerEnergy = maxPlayerEnergy;
            selectedCard1 = null;
            selectedCard2 = null;
            selectedCardIndex1 = -1;
            selectedCardIndex2 = -1;
            battleLogText.text = "";
        }
        
        // Update is called once per frame
        private void Update()
        {
            // Update UI periodically
            UpdateUI();
        }
        
        /// <summary>
        /// Update all UI elements
        /// </summary>
        private void UpdateUI()
        {
            // Get references
            var gameManager = GameManager.Instance;
            var cardSystem = gameManager.GetCardSystem();
            
            // Update player stats
            Entity playerEntity = gameManager.GetPlayerEntity();
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
            playerEnergyText.text = $"Energy: {playerEnergy} / {maxPlayerEnergy}";
            
            // Update enemy stats
            Entity enemyEntity = gameManager.GetEnemyEntity();
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
                    switch (enemyElement.Element)
                    {
                        case ElementType.Metal:
                            enemyImage.color = new Color(0.7f, 0.7f, 0.7f);
                            break;
                        case ElementType.Wood:
                            enemyImage.color = new Color(0.0f, 0.6f, 0.0f);
                            break;
                        case ElementType.Water:
                            enemyImage.color = new Color(0.0f, 0.0f, 0.8f);
                            break;
                        case ElementType.Fire:
                            enemyImage.color = new Color(0.8f, 0.0f, 0.0f);
                            break;
                        case ElementType.Earth:
                            enemyImage.color = new Color(0.6f, 0.4f, 0.2f);
                            break;
                    }
                }
            }
            
            // Update enemy intent
            enemyIntentText.text = $"Intent: {currentEnemyIntent} {currentEnemyIntentValue}";
            
            // Update hand cards
            UpdateHandCards(cardSystem.GetHand());
            
            // Update play zone
            UpdatePlayZone(cardSystem.GetPlayZone());
            
            // Update support zone
            UpdateSupportZone(cardSystem.GetSupportZone());
            
            // Update deck and discard counts
            deckCountText.text = $"Deck: {cardSystem.GetDeck().Count}";
            discardCountText.text = $"Discard: {cardSystem.GetDiscardPile().Count}";
            
            // Update turn and season
            turnText.text = $"Turn: {currentTurn}";
            seasonText.text = $"Season: {gameManager.GetCurrentSeason()}";
            
            // Disable end turn button if battle is over
            bool battleOver = gameManager.GetBattleSystem().IsBattleOver();
            endTurnButton.interactable = !battleOver;
            
            // Show restart button if battle is over
            restartButton.gameObject.SetActive(battleOver);
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
        /// Update support zone display
        /// </summary>
        private void UpdateSupportZone(List<Entity> supportZoneCards)
        {
            // Clear existing cards if count mismatch
            if (supportZoneCardViews.Count != supportZoneCards.Count)
            {
                ClearContainer(supportZoneContainer);
                supportZoneCardViews.Clear();
                
                // Create new card views
                for (int i = 0; i < supportZoneCards.Count; i++)
                {
                    GameObject cardGO = Instantiate(cardPrefab, supportZoneContainer);
                    CardView cardView = cardGO.GetComponent<CardView>();
                    cardView.SetCard(supportZoneCards[i]);
                    supportZoneCardViews.Add(cardView);
                }
            }
            else
            {
                // Update existing card views
                for (int i = 0; i < supportZoneCards.Count; i++)
                {
                    supportZoneCardViews[i].SetCard(supportZoneCards[i]);
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
            var cardSystem = GameManager.Instance.GetCardSystem();
            var hand = cardSystem.GetHand();
            
            if (cardIndex < 0 || cardIndex >= hand.Count)
                return;
            
            Entity card = hand[cardIndex];
            
            // Get card cost
            int cardCost = 1; // Default cost
            CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
            if (cardInfo != null)
            {
                cardCost = cardInfo.Cost;
            }
            
            // Check if we have enough energy
            if (playerEnergy < cardCost)
            {
                AddBattleLog("Not enough energy to play this card!");
                return;
            }
            
            // Check if this is a support card
            if (card.HasComponent<SupportCardComponent>())
            {
                // Play as support
                if (cardSystem.GetSupportZone().Count < 6) // Maximum 6 support cards
                {
                    cardSystem.PlayAsSupport(card);
                    playerEnergy -= cardCost;
                    AddBattleLog($"Played {cardInfo.Name} as support card.");
                }
                else
                {
                    AddBattleLog("Support zone is full!");
                }
                return;
            }
            
            // If no card is selected yet, select this card
            if (selectedCard1 == null)
            {
                selectedCard1 = card;
                selectedCardIndex1 = cardIndex;
                AddBattleLog($"Selected {cardInfo.Name}.");
                
                // If this is the only card we want to play, show confirmation
                ShowCardSelectionPanel("Play this card?", new List<Entity> { card }, cardIndex => {
                    // Play the card
                    PlaySelectedCards();
                });
            }
            // If a card is already selected, select this as second card if different
            else if (selectedCardIndex1 != cardIndex)
            {
                selectedCard2 = card;
                selectedCardIndex2 = cardIndex;
                AddBattleLog($"Selected {cardInfo.Name} as second card.");
                
                // Show confirmation
                ShowCardSelectionPanel("Play these cards?", new List<Entity> { selectedCard1, selectedCard2 }, cardIndex => {
                    // Play the cards
                    PlaySelectedCards();
                });
            }
            // If this card is already selected, deselect it
            else
            {
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
            // Reset selection
            selectedCard1 = null;
            selectedCard2 = null;
            selectedCardIndex1 = -1;
            selectedCardIndex2 = -1;
            
            // Hide panel
            cardSelectionPanel.SetActive(false);
            
            AddBattleLog("Card selection canceled.");
        }
        
        /// <summary>
        /// Play selected cards
        /// </summary>
        private void PlaySelectedCards()
        {
            var gameManager = GameManager.Instance;
            var cardSystem = gameManager.GetCardSystem();
            var battleSystem = gameManager.GetBattleSystem();
            var enemyEntity = gameManager.GetEnemyEntity();
            
            // Calculate total cost
            int totalCost = 0;
            
            if (selectedCard1 != null)
            {
                CardInfoComponent cardInfo = selectedCard1.GetComponent<CardInfoComponent>();
                if (cardInfo != null)
                {
                    totalCost += cardInfo.Cost;
                }
            }
            
            if (selectedCard2 != null)
            {
                CardInfoComponent cardInfo = selectedCard2.GetComponent<CardInfoComponent>();
                if (cardInfo != null)
                {
                    totalCost += cardInfo.Cost;
                }
            }
            
            // Check if we have enough energy
            if (playerEnergy < totalCost)
            {
                AddBattleLog("Not enough energy to play these cards!");
                return;
            }
            
            // Play first card
            if (selectedCard1 != null && enemyEntity != null)
            {
                // battleSystem.PlayCard(selectedCard1, enemyEntity);
                playerEnergy -= GetCardCost(selectedCard1);
                
                CardInfoComponent cardInfo = selectedCard1.GetComponent<CardInfoComponent>();
                AddBattleLog($"Played {cardInfo.Name}.");
                
                // Log element interaction
                LogElementInteraction(selectedCard1, enemyEntity);
            }
            
            // Play second card
            if (selectedCard2 != null && enemyEntity != null)
            {
                // battleSystem.PlayCard(selectedCard2, enemyEntity);
                playerEnergy -= GetCardCost(selectedCard2);
                
                CardInfoComponent cardInfo = selectedCard2.GetComponent<CardInfoComponent>();
                AddBattleLog($"Played {cardInfo.Name}.");
                
                // Log element interaction
                LogElementInteraction(selectedCard2, enemyEntity);
            }
            
            // Check if enemy is defeated
            if (battleSystem.IsBattleOver())
            {
                Entity winner = battleSystem.GetWinner();
                
                if (winner == gameManager.GetPlayerEntity())
                {
                    AddBattleLog("Victory! You defeated the enemy.");
                }
                else
                {
                    AddBattleLog("Defeat! You were defeated by the enemy.");
                }
            }
            
            // Reset selection
            selectedCard1 = null;
            selectedCard2 = null;
            selectedCardIndex1 = -1;
            selectedCardIndex2 = -1;
        }
        
        /// <summary>
        /// Log element interaction details
        /// </summary>
        private void LogElementInteraction(Entity card, Entity target)
        {
            ElementComponent cardElement = card.GetComponent<ElementComponent>();
            ElementComponent targetElement = target.GetComponent<ElementComponent>();
            var elementSystem = GameManager.Instance.GetElementInteractionSystem();
            
            if (cardElement != null && targetElement != null)
            {
                string interaction = $"{cardElement.GetElementName()} vs {targetElement.GetElementName()}: ";
                
                if (elementSystem.HasGeneratingRelationship(cardElement.Element, targetElement.Element))
                {
                    interaction += "+30% effectiveness (Tương Sinh)";
                    AddBattleLog(interaction);
                }
                else if (elementSystem.HasOvercomingRelationship(cardElement.Element, targetElement.Element))
                {
                    interaction += "+50% damage (Tương Khắc)";
                    AddBattleLog(interaction);
                }
                else if (elementSystem.HasOvercomingRelationship(targetElement.Element, cardElement.Element))
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
            // Generate enemy intent for next turn
            GenerateEnemyIntent();
            
            // End turn in game manager
            GameManager.Instance.EndTurn();
            
            // Update turn counter
            currentTurn++;
            
            // Reset energy
            playerEnergy = maxPlayerEnergy;
            
            // Reset selection
            selectedCard1 = null;
            selectedCard2 = null;
            selectedCardIndex1 = -1;
            selectedCardIndex2 = -1;
            
            AddBattleLog($"Started turn {currentTurn}.");
        }
        
        /// <summary>
        /// Generate enemy intent for next turn
        /// </summary>
        private void GenerateEnemyIntent()
        {
            // Simple AI: Alternate between attack and defense
            if (currentEnemyIntent == "Attack")
            {
                currentEnemyIntent = "Defense";
                currentEnemyIntentValue = Random.Range(3, 8);
            }
            else
            {
                currentEnemyIntent = "Attack";
                currentEnemyIntentValue = Random.Range(4, 10);
            }
            
            AddBattleLog($"Enemy's intent: {currentEnemyIntent} {currentEnemyIntentValue}");
        }
        
        /// <summary>
        /// Handle season button click
        /// </summary>
        private void OnSeasonClicked(Season season)
        {
            GameManager.Instance.SetSeason(season);
            AddBattleLog($"Changed season to {season}.");
        }
        
        /// <summary>
        /// Handle restart button click
        /// </summary>
        private void OnRestartClicked()
        {
            // Reset battle state
            ResetBattleState();
            
            // Start a new game
            GameManager.Instance.StartNewGame();
            
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
    }
}