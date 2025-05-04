// File: Scripts/Editor/CardGeneratorTool.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Core.Utils;
using Data;
using Systems.StatesMachine;

namespace Editor
{
    /// <summary>
    /// Editor tool for generating cards automatically with enhanced organization and CSV support
    /// </summary>
    public partial class CardGeneratorTool : EditorWindow
    {
        #region Fields

        // Output folder
        private string _baseOutputFolder = "Assets/Resources/Cards";

        // Default card values
        private int _cardId = 0;
        private string _cardKeyName = "new_card";
        private string _cardName = "New Card";
        private string _cardDescription = "Card description";
        private CardType _cardType = CardType.ElementalCard;
        private SupportCardType _supportCardType = SupportCardType.DivineBeast;
        private Rarity _cardRarity = Rarity.Common;
        private int _cardCost = 1;
        private int _cardAttack = 2;
        private int _cardDefense = 2;
        private int _cardHealth = 5;
        private int _cardSpeed = 2;
        private int _napAmIndex = 1;
        private ElementType _cardElement = ElementType.Metal;

        // Nap Am selections (for ElementalCard only)
        // private MetalNapAm _metalNapAm = MetalNapAm.SwordQi;
        // private WoodNapAm _woodNapAm = WoodNapAm.Growth;
        // private WaterNapAm _waterNapAm = WaterNapAm.Adaptation;
        // private FireNapAm _fireNapAm = FireNapAm.Burning;
        // private EarthNapAm _earthNapAm = EarthNapAm.Solidity;

        // Batch generation
        private int _batchAmount = 5;
        private ElementType _batchElement = ElementType.Metal;
        private bool _randomizeStats = true;
        private float _minAttack = 1;
        private float _maxAttack = 5;
        private float _minDefense = 1;
        private float _maxDefense = 5;
        private float _minHealth = 3;
        private float _maxHealth = 10;
        private float _minSpeed = 1;
        private float _maxSpeed = 5;
        private float _minCost = 1;
        private float _maxCost = 3;

        // Support card fields
        private string _effectDescription = "";
        private string _effectTargetStat = "attack";
        private string _activationConditionDescription = "attack";
        private float _effectValue = 1.0f;
        private int _effectDuration = 3;
        private ActivationType _activationType = ActivationType.Persistent;
        private ActivationConditionType _conditionType = ActivationConditionType.None;
        private string _conditionParameter = "";
        private float _conditionValue = 0.3f;
        private float _conditionValue2 = 0.0f;
        private int _cooldownTime = 3;
        private string _zodiacAnimal = "";
        private EffectType _effectType = EffectType.None;
        private string _effectParameter = "";
        private float _effectValue2 = 0.0f;

        // List of generated cards and tabs
        private List<ScriptableObject> _generatedCards = new List<ScriptableObject>();
        private Vector2 _scrollPosition;
        private int _selectedTab = 0;
        private string[] _tabNames = { "Create Single Card", "Batch Create Cards", "Card Manager" };

        // Card preview
        private Texture2D _previewTexture;

        // Folder structure
        private bool _useSubfolders = true;

        #endregion

        #region Editor Window

        /// <summary>
        /// Shows the editor window
        /// </summary>
        [MenuItem("Tools/Enhanced Card Generator")]
        public static void ShowWindow()
        {
            var window = GetWindow<CardGeneratorTool>("Card Generator");
            window.minSize = new Vector2(500, 700);
            window.Show();
        }

        /// <summary>
        /// OnGUI is called for rendering and handling GUI events
        /// </summary>
        private void OnGUI()
        {
            // Check if the output folder exists, create if it doesn't
            if (!Directory.Exists(_baseOutputFolder))
            {
                Directory.CreateDirectory(_baseOutputFolder);
            }

            // Create type subfolders if needed
            if (_useSubfolders)
            {
                CreateTypeSubfolders();
            }

            GUILayout.Label("Card Generator Tool", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _selectedTab = GUILayout.Toolbar(_selectedTab, _tabNames);

            EditorGUILayout.Space();

            // Draw the selected tab
            switch (_selectedTab)
            {
                case 0:
                    DrawSingleCardTab();
                    break;
                case 1:
                    DrawBatchCardTab();
                    break;
                case 2:
                    DrawCardManagerTab();
                    break;
            }
        }

        /// <summary>
        /// Creates subfolders for each card type
        /// </summary>
        private void CreateTypeSubfolders()
        {
            string[] folders =
            {
                $"{_baseOutputFolder}/ElementalCards",
                $"{_baseOutputFolder}/SupportCards"
            };

            foreach (string folder in folders)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }

            // Create subfolders for support card types
            string supportCardsFolder = $"{_baseOutputFolder}/SupportCards";

            string[] supportFolders =
            {
                $"{supportCardsFolder}/DivineBeasts",
                $"{supportCardsFolder}/Monsters",
                $"{supportCardsFolder}/SpiritAnimals",
                $"{supportCardsFolder}/Jokers",
                $"{supportCardsFolder}/Artifacts",
                $"{supportCardsFolder}/Talismans"
            };

            foreach (string folder in supportFolders)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
        }

        /// <summary>
        /// Gets the output folder for a specific card type
        /// </summary>
        private string GetOutputFolderForType(CardType cardType,
            SupportCardType supportType = SupportCardType.DivineBeast)
        {
            if (!_useSubfolders)
                return _baseOutputFolder;

            switch (cardType)
            {
                case CardType.ElementalCard:
                    return $"{_baseOutputFolder}/ElementalCards";
                case CardType.SupportCard:
                    string supportBaseFolder = $"{_baseOutputFolder}/SupportCards";

                    switch (supportType)
                    {
                        case SupportCardType.DivineBeast:
                            return $"{supportBaseFolder}/DivineBeasts";
                        case SupportCardType.Monster:
                            return $"{supportBaseFolder}/Monsters";
                        case SupportCardType.DivineWeapon:
                            return $"{supportBaseFolder}/SpiritAnimals";
                        // case SupportCardType.DivineWeapon:
                        //     return $"{supportBaseFolder}/Jokers";
                        case SupportCardType.Artifact:
                            return $"{supportBaseFolder}/Artifacts";
                        case SupportCardType.Talisman:
                            return $"{supportBaseFolder}/Talismans";
                        default:
                            return supportBaseFolder;
                    }
                default:
                    return _baseOutputFolder;
            }
        }

        #endregion

        #region Single Card Tab

        /// <summary>
        /// Draws the tab for creating a single card
        /// </summary>
        private void DrawSingleCardTab()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.LabelField("Basic Information", EditorStyles.boldLabel);

            // Output folder
            EditorGUILayout.BeginHorizontal();
            _baseOutputFolder = EditorGUILayout.TextField("Output folder:", _baseOutputFolder);
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select output folder", "Assets", "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // Convert absolute path to project-relative path
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        _baseOutputFolder = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            _useSubfolders = EditorGUILayout.Toggle("Use type subfolders:", _useSubfolders);

            // Card identification
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Card Identification", EditorStyles.boldLabel);
            _cardId = EditorGUILayout.IntField("Card ID:", _cardId);
            _cardKeyName = EditorGUILayout.TextField("Card Key (English):", _cardKeyName);

            // Basic card information
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Card Details", EditorStyles.boldLabel);
            _cardName = EditorGUILayout.TextField("Card Name:", _cardName);
            _cardDescription = EditorGUILayout.TextArea(_cardDescription, GUILayout.Height(60));

            // Card Type Selection with conditional Support Card Type
            EditorGUILayout.BeginHorizontal();
            _cardType = (CardType)EditorGUILayout.EnumPopup("Card Type:", _cardType);

            if (_cardType == CardType.SupportCard)
            {
                _supportCardType = (SupportCardType)EditorGUILayout.EnumPopup("Support Type:", _supportCardType);
            }

            EditorGUILayout.EndHorizontal();

            _cardRarity = (Rarity)EditorGUILayout.EnumPopup("Rarity:", _cardRarity);
            _cardCost = EditorGUILayout.IntSlider("Cost:", _cardCost, 0, 5);

            // Stats
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
            _cardAttack = EditorGUILayout.IntSlider("Attack:", _cardAttack, 0, 10);
            _cardDefense = EditorGUILayout.IntSlider("Defense:", _cardDefense, 0, 10);
            _cardHealth = EditorGUILayout.IntSlider("Health:", _cardHealth, 1, 20);
            _cardSpeed = EditorGUILayout.IntSlider("Speed:", _cardSpeed, 1, 10);

            // Element and NapAm (only for ElementalCard)
            if (_cardType == CardType.ElementalCard)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Element & NapAm", EditorStyles.boldLabel);
                _cardElement = (ElementType)EditorGUILayout.EnumPopup("Element:", _cardElement);

            }

            // Special fields based on card type
            if (_cardType == CardType.SupportCard)
            {
                EditorGUILayout.Space();
                DrawSupportCardFields();
            }

            // Generate button
            EditorGUILayout.Space();
            if (GUILayout.Button("Create Card", GUILayout.Height(30)))
            {
                GenerateSingleCard();
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Draws fields specific to support card types
        /// </summary>
        private void DrawSupportCardFields()
        {
            EditorGUILayout.LabelField($"{_supportCardType} Configuration", EditorStyles.boldLabel);

            // Common support card fields
            _activationType = (ActivationType)EditorGUILayout.EnumPopup("Activation Type:", _activationType);

            EditorGUILayout.LabelField("Activation Condition", EditorStyles.miniBoldLabel);
            _conditionType = (ActivationConditionType)EditorGUILayout.EnumPopup("Condition Type:", _conditionType);
            _conditionParameter = EditorGUILayout.TextField("Condition Parameter:", _conditionParameter);
            _conditionValue = EditorGUILayout.FloatField("Condition Value:", _conditionValue);

            if (_conditionType == ActivationConditionType.Threshold ||
                _conditionType == ActivationConditionType.ElementCount)
            {
                _conditionValue2 = EditorGUILayout.FloatField("Condition Value 2:", _conditionValue2);
            }

            _activationConditionDescription = EditorGUILayout.TextArea("Condition Description:",
                _activationConditionDescription, GUILayout.Height(40));

            EditorGUILayout.LabelField("Effect Configuration", EditorStyles.miniBoldLabel);
            _effectType = (EffectType)EditorGUILayout.EnumPopup("Effect Type:", _effectType);
            _effectParameter = EditorGUILayout.TextField("Effect Parameter:", _effectParameter);
            _effectValue = EditorGUILayout.FloatField("Effect Value:", _effectValue);

            if (_effectType == EffectType.StatBuff ||
                _effectType == EffectType.DamageOverTime)
            {
                _effectValue2 = EditorGUILayout.FloatField("Effect Value 2:", _effectValue2);
            }

            _effectDescription =
                EditorGUILayout.TextArea("Effect Description:", _effectDescription, GUILayout.Height(60));
            _effectDuration = EditorGUILayout.IntField("Effect Duration (turns):", _effectDuration);
            _cooldownTime = EditorGUILayout.IntField("Cooldown Time (turns):", _cooldownTime);

            // Type-specific fields
            // switch (_supportCardType)
            // {
            //     case SupportCardType.SpiritAnimal:
            //         _zodiacAnimal = EditorGUILayout.TextField("Zodiac Animal:", _zodiacAnimal);
            //         break;
            //         
            // }
        }

        /// <summary>
        /// Generates a single card with the current settings
        /// </summary>
        private void GenerateSingleCard()
        {
            ScriptableObject cardData = null;

            // Create the appropriate card type
            switch (_cardType)
            {
                case CardType.ElementalCard:
                    cardData = CreateElementalCard();
                    break;
                case CardType.SupportCard:
                    cardData = CreateSupportCard();
                    break;
            }

            if (cardData != null)
            {
                // Get the output folder for this card type
                string outputFolder = GetOutputFolderForType(_cardType, _supportCardType);

                // Sanitize card key name to ensure it's valid for a filename
                string sanitizedKeyName = SanitizeFileName(_cardKeyName);

                // Generate filename based on key name
                string assetPath = $"{outputFolder}/{sanitizedKeyName}.asset";

                // Create the asset
                AssetDatabase.CreateAsset(cardData, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // Add to generated cards list
                _generatedCards.Add(cardData);

                // Select the created asset
                Selection.activeObject = cardData;

                Debug.Log($"Card created: {assetPath}");
                EditorUtility.DisplayDialog("Success", $"Card created: {_cardName}", "OK");
            }
        }

        #endregion

        #region Card Creation Methods

        /// <summary>
        /// Creates an Elemental Card with appropriate Nap Am
        /// </summary>
        private ElementalCardDataSO CreateElementalCard()
        {
            ElementalCardDataSO card = CreateInstance<ElementalCardDataSO>();

            // Set basic properties
            card.cardId = _cardId;
            card.cardKeyName = _cardKeyName;
            card.cardName = _cardName;
            card.description = _cardDescription;
            card.cardType = CardType.ElementalCard;
            card.rarity = _cardRarity;
            card.cost = _cardCost;
            card.attack = _cardAttack;
            card.defense = _cardDefense;
            card.health = _cardHealth;
            card.speed = _cardSpeed;
            card.elementType = _cardElement;
            card.napAmIndex = _napAmIndex;

            return card;
        }

        /// <summary>
        /// Creates a Support Card
        /// </summary>
        private SupportCardDataSO CreateSupportCard()
        {
            SupportCardDataSO card = CreateInstance<SupportCardDataSO>();

            // Set basic properties
            card.cardId = _cardId;
            card.cardKeyName = _cardKeyName;
            card.cardName = _cardName;
            card.description = _cardDescription;
            card.cardType = CardType.SupportCard;
            card.supportCardType = _supportCardType;
            card.rarity = _cardRarity;
            card.cost = _cardCost;
            card.attack = _cardAttack;
            card.defense = _cardDefense;
            card.health = _cardHealth;
            card.speed = _cardSpeed;

            // Set support card specific properties
            card.activationType = _activationType;
            card.conditionType = _conditionType;
            card.conditionParameter = _conditionParameter;
            card.conditionValue = _conditionValue;
            card.conditionValue2 = _conditionValue2;
            card.effectType = _effectType;
            card.effectParameter = _effectParameter;
            card.effectValue = _effectValue;
            card.effectValue2 = _effectValue2;
            card.effectDuration = _effectDuration;
            card.cooldownTime = _cooldownTime;
            card.effectDescription = _effectDescription;
            card.activationConditionDescription = _activationConditionDescription;

            return card;
        }

        #endregion

        #region Batch Card Tab

        /// <summary>
        /// Draws the tab for batch card generation
        /// </summary>
        private void DrawBatchCardTab()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.LabelField("Batch Card Creation", EditorStyles.boldLabel);

            // Output folder
            EditorGUILayout.BeginHorizontal();
            _baseOutputFolder = EditorGUILayout.TextField("Output folder:", _baseOutputFolder);
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select output folder", "Assets", "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // Convert absolute path to project-relative path
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        _baseOutputFolder = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            _useSubfolders = EditorGUILayout.Toggle("Use type subfolders:", _useSubfolders);

            // Batch settings
            _batchAmount = EditorGUILayout.IntSlider("Number of cards:", _batchAmount, 1, 20);

            // Card Type Selection with conditional Support Card Type
            EditorGUILayout.BeginHorizontal();
            _cardType = (CardType)EditorGUILayout.EnumPopup("Card Type:", _cardType);

            if (_cardType == CardType.SupportCard)
            {
                _supportCardType = (SupportCardType)EditorGUILayout.EnumPopup("Support Type:", _supportCardType);
            }

            EditorGUILayout.EndHorizontal();

            _cardRarity = (Rarity)EditorGUILayout.EnumPopup("Rarity:", _cardRarity);

            // Element selection only for ElementalCard
            if (_cardType == CardType.ElementalCard)
            {
                _batchElement = (ElementType)EditorGUILayout.EnumPopup("Element:", _batchElement);
            }

            // Starting ID
            _cardId = EditorGUILayout.IntField("Starting ID:", _cardId);

            // Stat ranges
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Stat Ranges", EditorStyles.boldLabel);

            _randomizeStats = EditorGUILayout.Toggle("Randomize stats:", _randomizeStats);

            if (_randomizeStats)
            {
                EditorGUILayout.LabelField("Cost:", $"{_minCost} - {_maxCost}");
                EditorGUILayout.MinMaxSlider(ref _minCost, ref _maxCost, 0, 5);
                _minCost = Mathf.FloorToInt(_minCost);
                _maxCost = Mathf.CeilToInt(_maxCost);

                EditorGUILayout.LabelField("Attack:", $"{_minAttack} - {_maxAttack}");
                EditorGUILayout.MinMaxSlider(ref _minAttack, ref _maxAttack, 0, 10);
                _minAttack = Mathf.FloorToInt(_minAttack);
                _maxAttack = Mathf.CeilToInt(_maxAttack);

                EditorGUILayout.LabelField("Defense:", $"{_minDefense} - {_maxDefense}");
                EditorGUILayout.MinMaxSlider(ref _minDefense, ref _maxDefense, 0, 10);
                _minDefense = Mathf.FloorToInt(_minDefense);
                _maxDefense = Mathf.CeilToInt(_maxDefense);

                EditorGUILayout.LabelField("Health:", $"{_minHealth} - {_maxHealth}");
                EditorGUILayout.MinMaxSlider(ref _minHealth, ref _maxHealth, 1, 20);
                _minHealth = Mathf.FloorToInt(_minHealth);
                _maxHealth = Mathf.CeilToInt(_maxHealth);

                EditorGUILayout.LabelField("Speed:", $"{_minSpeed} - {_maxSpeed}");
                EditorGUILayout.MinMaxSlider(ref _minSpeed, ref _maxSpeed, 1, 10);
                _minSpeed = Mathf.FloorToInt(_minSpeed);
                _maxSpeed = Mathf.CeilToInt(_maxSpeed);
            }
            else
            {
                _cardCost = EditorGUILayout.IntSlider("Cost:", _cardCost, 0, 5);
                _cardAttack = EditorGUILayout.IntSlider("Attack:", _cardAttack, 0, 10);
                _cardDefense = EditorGUILayout.IntSlider("Defense:", _cardDefense, 0, 10);
                _cardHealth = EditorGUILayout.IntSlider("Health:", _cardHealth, 1, 20);
                _cardSpeed = EditorGUILayout.IntSlider("Speed:", _cardSpeed, 1, 10);
            }

            // Name prefix
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Name Settings", EditorStyles.boldLabel);
            _cardKeyName = EditorGUILayout.TextField("Key prefix:", _cardKeyName);
            _cardName = EditorGUILayout.TextField("Name prefix:", _cardName);
            EditorGUILayout.HelpBox("Cards will be named in format: [Prefix]_[Number]", MessageType.Info);

            // Generate button
            EditorGUILayout.Space();
            if (GUILayout.Button("Create Batch Cards", GUILayout.Height(30)))
            {
                GenerateBatchCards();
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Generates multiple cards with the batch settings
        /// </summary>
        private void GenerateBatchCards()
        {
            List<ScriptableObject> createdCards = new List<ScriptableObject>();
            int successCount = 0;

            // Create progress bar
            EditorUtility.DisplayProgressBar("Creating Cards", "Preparing...", 0f);

            try
            {
                for (int i = 0; i < _batchAmount; i++)
                {
                    // Update progress
                    EditorUtility.DisplayProgressBar("Creating Cards", $"Creating card {i + 1}/{_batchAmount}",
                        (float)(i + 1) / _batchAmount);

                    // Randomize stats if needed
                    if (_randomizeStats)
                    {
                        _cardCost = (int)Random.Range(_minCost, _maxCost + 1);
                        _cardAttack = (int)Random.Range(_minAttack, _maxAttack + 1);
                        _cardDefense = (int)Random.Range(_minDefense, _maxDefense + 1);
                        _cardHealth = (int)Random.Range(_minHealth, _maxHealth + 1);
                        _cardSpeed = (int)Random.Range(_minSpeed, _maxSpeed + 1);
                    }

                    // Set card Id with index
                    int cardId = _cardId + i;

                    // Set card key and name with index
                    string originalKeyName = _cardKeyName;
                    string originalName = _cardName;
                    _cardKeyName = $"{originalKeyName}_{i + 1}";
                    _cardName = $"{originalName}_{i + 1}";

                    // Set the element from batch settings (if ElementalCard)
                    if (_cardType == CardType.ElementalCard)
                    {
                        _cardElement = _batchElement;

                    }

                    // Create the card based on type
                    ScriptableObject cardData = null;

                    switch (_cardType)
                    {
                        case CardType.ElementalCard:
                            cardData = CreateElementalCard();
                            break;
                        case CardType.SupportCard:
                            cardData = CreateSupportCard();
                            break;
                    }

                    if (cardData != null)
                    {
                        // Get output folder for the card type
                        string outputFolder = GetOutputFolderForType(_cardType, _supportCardType);

                        // Generate a filename based on the card key name
                        string sanitizedKeyName = SanitizeFileName(_cardKeyName);
                        string assetPath = $"{outputFolder}/{sanitizedKeyName}.asset";

                        // Create the asset
                        AssetDatabase.CreateAsset(cardData, assetPath);
                        createdCards.Add(cardData);
                        successCount++;
                    }

                    // Reset names
                    _cardKeyName = originalKeyName;
                    _cardName = originalName;
                }

                // Save all assets at once
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // Add to generated cards list
                _generatedCards.AddRange(createdCards);

                EditorUtility.DisplayDialog("Success", $"Created {successCount} cards.", "OK");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error creating batch cards: {e.Message}");
                EditorUtility.DisplayDialog("Error", $"An error occurred: {e.Message}", "OK");
            }
            finally
            {
                // Clear progress bar
                EditorUtility.ClearProgressBar();
            }
        }

        #endregion

        #region Card Manager Tab

        /// <summary>
        /// Draws the tab for managing existing cards
        /// </summary>
        private void DrawCardManagerTab()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.LabelField("Card Manager", EditorStyles.boldLabel);

            // Refresh button
            if (GUILayout.Button("Refresh Card List"))
            {
                RefreshCardList();
            }

            EditorGUILayout.Space();

            // CSV Import/Export section
            DrawCsvImportSection();

            EditorGUILayout.Space();

            // Display all cards in the Resources folder
            if (_generatedCards.Count == 0)
            {
                EditorGUILayout.HelpBox("No cards found. Create some cards or refresh the list.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.LabelField($"Found {_generatedCards.Count} cards:", EditorStyles.boldLabel);
                EditorGUILayout.Space();

                // Display cards grouped by type
                Dictionary<CardType, Dictionary<SupportCardType, List<ScriptableObject>>> cardsByType =
                    GroupCardsByType();

                // Display Elemental Cards
                if (cardsByType.ContainsKey(CardType.ElementalCard))
                {
                    var elementalCards =
                        cardsByType[CardType.ElementalCard][SupportCardType.DivineBeast]; // Placeholder, not used
                    EditorGUILayout.LabelField($"Elemental Cards ({elementalCards.Count}):", EditorStyles.boldLabel);

                    foreach (var card in elementalCards)
                    {
                        DisplayCardEntry(card);
                    }

                    EditorGUILayout.Space();
                }

                // Display Support Cards by type
                if (cardsByType.ContainsKey(CardType.SupportCard))
                {
                    var supportTypes = cardsByType[CardType.SupportCard];
                    EditorGUILayout.LabelField($"Support Cards:", EditorStyles.boldLabel);

                    foreach (var supportType in supportTypes.Keys)
                    {
                        var cards = supportTypes[supportType];
                        if (cards.Count > 0)
                        {
                            EditorGUILayout.LabelField($"  {supportType} ({cards.Count}):", EditorStyles.boldLabel);

                            foreach (var card in cards)
                            {
                                DisplayCardEntry(card);
                            }

                            EditorGUILayout.Space();
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Displays a single card entry in the manager list
        /// </summary>
        private void DisplayCardEntry(ScriptableObject card)
        {
            if (card == null) return;

            EditorGUILayout.BeginHorizontal("box");

            // Card name
            EditorGUILayout.LabelField(card.name, EditorStyles.boldLabel, GUILayout.Width(200));

            // Edit button
            if (GUILayout.Button("Edit", GUILayout.Width(60)))
            {
                Selection.activeObject = card;
                EditorGUIUtility.PingObject(card);
            }

            // Delete button
            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                if (EditorUtility.DisplayDialog("Confirm deletion",
                        $"Are you sure you want to delete the card {card.name}?", "Delete", "Cancel"))
                {
                    string assetPath = AssetDatabase.GetAssetPath(card);
                    AssetDatabase.DeleteAsset(assetPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    RefreshCardList();
                }
            }

            // Duplicate button
            if (GUILayout.Button("Duplicate", GUILayout.Width(80)))
            {
                string assetPath = AssetDatabase.GetAssetPath(card);
                string newPath = assetPath.Replace(".asset", "_Copy.asset");

                if (AssetDatabase.CopyAsset(assetPath, newPath))
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    RefreshCardList();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Refreshes the list of cards from the Resources folder
        /// </summary>
        private void RefreshCardList()
        {
            _generatedCards.Clear();

            List<string> folderPaths = new List<string> { _baseOutputFolder };

            // Add subfolders if enabled
            if (_useSubfolders)
            {
                folderPaths.Add($"{_baseOutputFolder}/ElementalCards");

                string supportCardsFolder = $"{_baseOutputFolder}/SupportCards";
                folderPaths.Add(supportCardsFolder);

                folderPaths.Add($"{supportCardsFolder}/DivineBeasts");
                folderPaths.Add($"{supportCardsFolder}/Monsters");
                folderPaths.Add($"{supportCardsFolder}/SpiritAnimals");
                folderPaths.Add($"{supportCardsFolder}/Jokers");
                folderPaths.Add($"{supportCardsFolder}/Artifacts");
                folderPaths.Add($"{supportCardsFolder}/Talismans");
            }

            foreach (string folderPath in folderPaths)
            {
                if (!Directory.Exists(folderPath))
                    continue;

                string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { folderPath });

                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

                    if (obj is CardDataSO || obj is ElementalCardDataSO || obj is SupportCardDataSO)
                    {
                        _generatedCards.Add(obj);
                    }
                }
            }
        }

        /// <summary>
        /// Groups cards by their type and subtype
        /// </summary>
        private Dictionary<CardType, Dictionary<SupportCardType, List<ScriptableObject>>> GroupCardsByType()
        {
            var result = new Dictionary<CardType, Dictionary<SupportCardType, List<ScriptableObject>>>();

            // Initialize dictionaries
            result[CardType.ElementalCard] = new Dictionary<SupportCardType, List<ScriptableObject>>();
            result[CardType.ElementalCard][SupportCardType.DivineBeast] = new List<ScriptableObject>(); // Placeholder

            result[CardType.SupportCard] = new Dictionary<SupportCardType, List<ScriptableObject>>();
            foreach (SupportCardType supportType in System.Enum.GetValues(typeof(SupportCardType)))
            {
                result[CardType.SupportCard][supportType] = new List<ScriptableObject>();
            }

            foreach (var card in _generatedCards)
            {
                if (card is ElementalCardDataSO elementalCard)
                {
                    result[CardType.ElementalCard][SupportCardType.DivineBeast].Add(card);
                }
                else if (card is SupportCardDataSO supportCard)
                {
                    result[CardType.SupportCard][supportCard.supportCardType].Add(card);
                }
                else if (card is CardDataSO cardData)
                {
                    // Handle legacy cards or other card types
                    if (cardData.cardType == CardType.ElementalCard)
                    {
                        result[CardType.ElementalCard][SupportCardType.DivineBeast].Add(card);
                    }
                    else if (cardData.cardType == CardType.SupportCard)
                    {
                        // If we don't know the support type, default to DivineBeast
                        result[CardType.SupportCard][SupportCardType.DivineBeast].Add(card);
                    }
                }
            }

            return result;
        }

        #endregion

        #region Utility Methods


        /// <summary>
        /// Sanitizes a filename by removing invalid characters
        /// </summary>
        private string SanitizeFileName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string result = fileName;

            foreach (char c in invalidChars)
            {
                result = result.Replace(c, '_');
            }

            return result;
        }
        #endregion
    }
}

