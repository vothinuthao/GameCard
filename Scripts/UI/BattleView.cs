using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Components;
using Core;
using RunTime;
using UI;

namespace NguhanhGame.UI
{
    /// <summary>
    /// Manages the battle UI
    /// </summary>
    public class BattleView : MonoBehaviour
    {
        [Header("Player UI")]
        [SerializeField] private TextMeshProUGUI playerHealthText;
        [SerializeField] private Slider playerHealthSlider;
        
        [Header("Enemy UI")]
        [SerializeField] private TextMeshProUGUI enemyHealthText;
        [SerializeField] private Slider enemyHealthSlider;
        [SerializeField] private Image enemyImage;
        
        [Header("Card UI")]
        [SerializeField] private Transform handContainer;
        [SerializeField] private Transform playZoneContainer;
        [SerializeField] private Transform supportZoneContainer;
        [SerializeField] private GameObject cardPrefab;
        
        [Header("Battle UI")]
        [SerializeField] private Button endTurnButton;
        [SerializeField] private TextMeshProUGUI deckCountText;
        [SerializeField] private TextMeshProUGUI discardCountText;
        [SerializeField] private TextMeshProUGUI turnText;
        [SerializeField] private TextMeshProUGUI seasonText;
        
        // Internal references
        private List<CardView> handCardViews = new List<CardView>();
        private List<CardView> playZoneCardViews = new List<CardView>();
        private List<CardView> supportZoneCardViews = new List<CardView>();
        private int currentTurn = 1;
        
        // Start is called before the first frame update
        private void Start()
        {
            // Initialize UI
            UpdateUI();
            
            // Add listeners
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
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
            GameManager gameManager = GameManager.Instance;
            
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
            }
            
            // Update hand cards
            UpdateHandCards(gameManager.GetCardSystem().GetHand());
            
            // Update play zone
            UpdatePlayZone(gameManager.GetCardSystem().GetPlayZone());
            
            // Update support zone
            UpdateSupportZone(gameManager.GetCardSystem().GetSupportZone());
            
            // Update deck and discard counts
            deckCountText.text = $"Deck: {gameManager.GetCardSystem().GetDeck().Count}";
            discardCountText.text = $"Discard: {gameManager.GetCardSystem().GetDiscardPile().Count}";
            
            // Update turn and season
            turnText.text = $"Turn: {currentTurn}";
            seasonText.text = $"Season: {gameManager.GetCurrentSeason()}";
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
        /// Handle end turn button click
        /// </summary>
        private void OnEndTurnClicked()
        {
            GameManager.Instance.EndTurn();
            currentTurn++;
        }
        
        /// <summary>
        /// Display a battle result message
        /// </summary>
        public void ShowBattleResult(bool playerWon)
        {
            // In a real implementation, this would show a UI panel with the result
            if (playerWon)
            {
                Debug.Log("Battle Result: Player Won!");
            }
            else
            {
                Debug.Log("Battle Result: Player Lost!");
            }
        }
    }
}
