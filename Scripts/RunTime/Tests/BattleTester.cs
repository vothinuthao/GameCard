using Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RunTime.Tests
{
    /// <summary>
    /// Simple component to test the battle system
    /// </summary>
    public class BattleTester : MonoBehaviour
    {
        [Header("Test Controls")]
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button startBattleButton;
        [SerializeField] private Button playCardButton;
        [SerializeField] private Button playSupportButton;
        [SerializeField] private Button endTurnButton;
        [SerializeField] private Button getStateButton;
        
        [Header("Season Controls")]
        [SerializeField] private Button springButton;
        [SerializeField] private Button summerButton;
        [SerializeField] private Button autumnButton;
        [SerializeField] private Button winterButton;
        
        [Header("Output")]
        [SerializeField] private TextMeshProUGUI outputText;
        
        // Start is called before the first frame update
        private void Start()
        {
            // Add listeners
            startGameButton.onClick.AddListener(OnStartGameClicked);
            startBattleButton.onClick.AddListener(OnStartBattleClicked);
            playCardButton.onClick.AddListener(OnPlayCardClicked);
            playSupportButton.onClick.AddListener(OnPlaySupportClicked);
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
            getStateButton.onClick.AddListener(OnGetStateClicked);
            
            springButton.onClick.AddListener(() => OnSeasonClicked(Season.Spring));
            summerButton.onClick.AddListener(() => OnSeasonClicked(Season.Summer));
            autumnButton.onClick.AddListener(() => OnSeasonClicked(Season.Autumn));
            winterButton.onClick.AddListener(() => OnSeasonClicked(Season.Winter));
            
            // Initialize output
            outputText.text = "Welcome to the Battle Tester!\nClick 'Start Game' to begin.";
        }
        
        /// <summary>
        /// Handle start game button click
        /// </summary>
        private void OnStartGameClicked()
        {
            GameManager.Instance.StartNewGame();
            outputText.text = "Game started! Click 'Start Battle' to begin a battle.";
        }
        
        /// <summary>
        /// Handle start battle button click
        /// </summary>
        private void OnStartBattleClicked()
        {
            GameManager.Instance.StartBattle("medium");
            outputText.text = "Battle started! Click 'Play Card' to play a card or 'End Turn' to end your turn.";
        }
        
        /// <summary>
        /// Handle play card button click
        /// </summary>
        private void OnPlayCardClicked()
        {
            // Play the first card in hand if available
            var hand = GameManager.Instance.GetCardSystem().GetHand();
            if (hand.Count > 0)
            {
                GameManager.Instance.PlayCard(0);
                outputText.text = "Card played!";
            }
            else
            {
                outputText.text = "No cards in hand!";
            }
        }
        
        /// <summary>
        /// Handle play support button click
        /// </summary>
        private void OnPlaySupportClicked()
        {
            // Play the first card in hand as support if available
            var hand = GameManager.Instance.GetCardSystem().GetHand();
            if (hand.Count > 0)
            {
                GameManager.Instance.PlayAsSupport(0);
                outputText.text = "Card played as support!";
            }
            else
            {
                outputText.text = "No cards in hand!";
            }
        }
        
        /// <summary>
        /// Handle end turn button click
        /// </summary>
        private void OnEndTurnClicked()
        {
            GameManager.Instance.EndTurn();
            outputText.text = "Turn ended!";
        }
        
        /// <summary>
        /// Handle get state button click
        /// </summary>
        private void OnGetStateClicked()
        {
            GameManager.Instance.GetGameState();
            outputText.text = "Check console for game state info.";
        }
        
        /// <summary>
        /// Handle season button click
        /// </summary>
        private void OnSeasonClicked(Season season)
        {
            GameManager.Instance.SetSeason(season);
            outputText.text = $"Season changed to {season}!";
        }
    }
}