using System.Collections.Generic;
using Core;
using Core.Utils;
using Factories;
using RunTime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Manages the deck builder UI
    /// </summary>
    public class DeckBuilderView : MonoBehaviour
    {
        [Header("UI References")] [SerializeField]
        private Transform collectionContainer;

        [SerializeField] private Transform deckContainer;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TMP_Dropdown filterDropdown;
        [SerializeField] private TMP_InputField searchField;
        [SerializeField] private TextMeshProUGUI deckCountText;

        // Internal references
        private List<CardView> collectionCardViews = new List<CardView>();
        private List<CardView> deckCardViews = new List<CardView>();
        private List<Entity> availableCards = new List<Entity>();
        private List<Entity> deckCards = new List<Entity>();

        // Start is called before the first frame update
        private void Start()
        {
            // Initialize UI
            InitializeUI();

            // Add listeners
            saveButton.onClick.AddListener(OnSaveClicked);
            loadButton.onClick.AddListener(OnLoadClicked);
            backButton.onClick.AddListener(OnBackClicked);
            filterDropdown.onValueChanged.AddListener(OnFilterChanged);
            searchField.onValueChanged.AddListener(OnSearchChanged);
        }

        /// <summary>
        /// Initialize the UI
        /// </summary>
        private void InitializeUI()
        {
            // Load all available cards
            LoadAvailableCards();

            // Load current deck
            LoadCurrentDeck();

            // Update UI
            UpdateUI();
        }

        /// <summary>
        /// Load all available cards
        /// </summary>
        private void LoadAvailableCards()
        {
            // In a real implementation, this would load from a catalog or database
            // For now, we'll create some sample cards

            CardFactory cardFactory = new CardFactory(GameManager.Instance.GetEntityManager());

            // Add some Metal cards
            availableCards.Add(cardFactory.CreateMetalCard("Kim Chúc Kiếm",
                "A powerful metal sword that cuts through defenses", MetalNapAm.SwordQi, Rarity.Common, 1));
            availableCards.Add(cardFactory.CreateMetalCard("Thiết Bích Thuẫn",
                "A sturdy shield that provides strong defense", MetalNapAm.Hardness, Rarity.Common, 1));
            availableCards.Add(cardFactory.CreateMetalCard("Thanh Tịnh Chi Kim",
                "Pure metal that dispels negative effects", MetalNapAm.Purity, Rarity.Rare, 2));

            // Add some Wood cards
            availableCards.Add(cardFactory.CreateWoodCard("Thanh Mộc Châm",
                "Sharp wooden needles that can pierce enemies", WoodNapAm.Toxin, Rarity.Common, 1));
            availableCards.Add(cardFactory.CreateWoodCard("Sinh Mệnh Chi Mộc", "A tree of life that provides healing",
                WoodNapAm.Regeneration, Rarity.Rare, 2));
            availableCards.Add(cardFactory.CreateWoodCard("Linh Hoạt Chi Mộc", "Flexible wood that enhances agility",
                WoodNapAm.Flexibility, Rarity.Common, 1));

            // In a real implementation, we would also load cards from ScriptableObjects
            // using the ScriptableObjectFactory
        }

        /// <summary>
        /// Load the current deck
        /// </summary>
        private void LoadCurrentDeck()
        {
            // In a real implementation, this would load the saved deck
            // For now, we'll start with an empty deck
            deckCards.Clear();
        }

        /// <summary>
        /// Update the UI
        /// </summary>
        private void UpdateUI()
        {
            // Update collection cards
            UpdateCollectionCards();

            // Update deck cards
            UpdateDeckCards();

            // Update deck count
            deckCountText.text = $"Deck: {deckCards.Count} / 30"; // Assuming 30 is max deck size
        }

        /// <summary>
        /// Update collection cards display
        /// </summary>
        private void UpdateCollectionCards()
        {
            // Clear existing cards
            ClearContainer(collectionContainer);
            collectionCardViews.Clear();

            // Create new card views for collection
            foreach (var card in availableCards)
            {
                GameObject cardGO = Instantiate(cardPrefab, collectionContainer);
                CardView cardView = cardGO.GetComponent<CardView>();
                cardView.SetCard(card);

                // Add click listener
                Button cardButton = cardGO.GetComponent<Button>();
                if (cardButton != null)
                {
                    cardButton.onClick.AddListener(() => OnCollectionCardClicked(card));
                }

                collectionCardViews.Add(cardView);
            }
        }

        /// <summary>
        /// Update deck cards display
        /// </summary>
        private void UpdateDeckCards()
        {
            // Clear existing cards
            ClearContainer(deckContainer);
            deckCardViews.Clear();

            // Create new card views for deck
            foreach (var card in deckCards)
            {
                GameObject cardGO = Instantiate(cardPrefab, deckContainer);
                CardView cardView = cardGO.GetComponent<CardView>();
                cardView.SetCard(card);

                // Add click listener
                Button cardButton = cardGO.GetComponent<Button>();
                if (cardButton != null)
                {
                    cardButton.onClick.AddListener(() => OnDeckCardClicked(card));
                }

                deckCardViews.Add(cardView);
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
        /// Handle collection card click (add to deck)
        /// </summary>
        private void OnCollectionCardClicked(Entity card)
        {
            if (deckCards.Count < 30) // Assuming 30 is max deck size
            {
                // Copy the card to the deck
                // In a real implementation, we would create a new entity with the same properties
                deckCards.Add(card);
                UpdateUI();
            }
        }

        /// <summary>
        /// Handle deck card click (remove from deck)
        /// </summary>
        private void OnDeckCardClicked(Entity card)
        {
            deckCards.Remove(card);
            UpdateUI();
        }

        /// <summary>
        /// Handle save button click
        /// </summary>
        private void OnSaveClicked()
        {
            // Save the deck
            // In a real implementation, this would save to PlayerPrefs or a file
            Debug.Log("Deck saved!");
        }

        /// <summary>
        /// Handle load button click
        /// </summary>
        private void OnLoadClicked()
        {
            // Load a saved deck
            // In a real implementation, this would load from PlayerPrefs or a file
            Debug.Log("Deck loaded!");
        }

        /// <summary>
        /// Handle back button click
        /// </summary>
        private void OnBackClicked()
        {
            // Go back to the main menu
            // In a real implementation, this would use a scene manager
            Debug.Log("Going back to main menu");
        }

        /// <summary>
        /// Handle filter dropdown change
        /// </summary>
        private void OnFilterChanged(int value)
        {
            // Filter the collection based on dropdown value
            // 0: All, 1: Metal, 2: Wood, 3: Water, 4: Fire, 5: Earth
            // In a real implementation, this would apply filters
            Debug.Log($"Filter changed to: {value}");

            // Update UI
            UpdateUI();
        }

        /// <summary>
        /// Handle search field change
        /// </summary>
        private void OnSearchChanged(string searchText)
        {
            // Search the collection based on search text
            // In a real implementation, this would filter by name/description
            Debug.Log($"Search text: {searchText}");

            // Update UI
            UpdateUI();
        }
    }
}