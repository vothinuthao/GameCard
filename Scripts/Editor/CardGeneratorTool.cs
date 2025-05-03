// File: Scripts/Editor/CardGeneratorTool.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Core.Utils;
using Data;

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
        private Rarity _cardRarity = Rarity.Common;
        private int _cardCost = 1;
        private int _cardAttack = 2;
        private int _cardDefense = 2;
        private int _cardHealth = 5;
        private int _cardSpeed = 2;
        private ElementType _cardElement = ElementType.Metal;
        
        // Nap Am selections (for ElementalCard only)
        private MetalNapAm _metalNapAm = MetalNapAm.SwordQi;
        private WoodNapAm _woodNapAm = WoodNapAm.Growth;
        private WaterNapAm _waterNapAm = WaterNapAm.Adaptation;
        private FireNapAm _fireNapAm = FireNapAm.Burning;
        private EarthNapAm _earthNapAm = EarthNapAm.Solidity;

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
        
        // Special card fields
        private string _effectDescription = "";
        private string _effectTargetStat = "attack";
        private float _effectValue = 1.0f;
        private int _effectDuration = 3;
        private ActivationType _activationType = ActivationType.Persistent;
        private string _activationConditionDescription = "";
        private string _conditionType = "health";
        private float _conditionValue = 0.3f;
        private int _cooldownTime = 3;
        private string _zodiacAnimal = "";
        
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
            string[] folders = {
                $"{_baseOutputFolder}/ElementalCards",
                $"{_baseOutputFolder}/DivineBeasts",
                $"{_baseOutputFolder}/Monsters",
                $"{_baseOutputFolder}/SpiritAnimals",
                $"{_baseOutputFolder}/Jokers"
            };

            foreach (string folder in folders)
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
        private string GetOutputFolderForType(CardType cardType)
        {
            if (!_useSubfolders)
                return _baseOutputFolder;

            switch (cardType)
            {
                case CardType.ElementalCard:
                    return $"{_baseOutputFolder}/ElementalCards";
                case CardType.DivineBeast:
                    return $"{_baseOutputFolder}/DivineBeasts";
                case CardType.Monster:
                    return $"{_baseOutputFolder}/Monsters";
                case CardType.SpiritAnimal:
                    return $"{_baseOutputFolder}/SpiritAnimals";
                case CardType.Joker:
                    return $"{_baseOutputFolder}/Jokers";
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
            _cardType = (CardType)EditorGUILayout.EnumPopup("Card Type:", _cardType);
            _cardRarity = (Rarity)EditorGUILayout.EnumPopup("Rarity:", _cardRarity);
            _cardCost = EditorGUILayout.IntSlider("Cost:", _cardCost, 0, 5);

            // Stats
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
            _cardAttack = EditorGUILayout.IntSlider("Attack:", _cardAttack, 0, 10);
            _cardDefense = EditorGUILayout.IntSlider("Defense:", _cardDefense, 0, 10);
            _cardHealth = EditorGUILayout.IntSlider("Health:", _cardHealth, 1, 20);
            _cardSpeed = EditorGUILayout.IntSlider("Speed:", _cardSpeed, 1, 10);

            // Element and NapAm
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Element & NapAm", EditorStyles.boldLabel);
            _cardElement = (ElementType)EditorGUILayout.EnumPopup("Element:", _cardElement);

            // Display different NapAm options based on the selected element
            switch (_cardElement)
            {
                case ElementType.Metal:
                    _metalNapAm = (MetalNapAm)EditorGUILayout.EnumPopup("Metal Nap Am:", _metalNapAm);
                    break;
                case ElementType.Wood:
                    _woodNapAm = (WoodNapAm)EditorGUILayout.EnumPopup("Wood Nap Am:", _woodNapAm);
                    break;
                case ElementType.Water:
                    _waterNapAm = (WaterNapAm)EditorGUILayout.EnumPopup("Water Nap Am:", _waterNapAm);
                    break;
                case ElementType.Fire:
                    _fireNapAm = (FireNapAm)EditorGUILayout.EnumPopup("Fire Nap Am:", _fireNapAm);
                    break;
                case ElementType.Earth:
                    _earthNapAm = (EarthNapAm)EditorGUILayout.EnumPopup("Earth Nap Am:", _earthNapAm);
                    break;
            }

            // Special fields based on card type
            EditorGUILayout.Space();
            DrawSpecialFields();

            // Generate button
            EditorGUILayout.Space();
            if (GUILayout.Button("Create Card", GUILayout.Height(30)))
            {
                GenerateSingleCard();
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Draws special fields based on card type
        /// </summary>
        private void DrawSpecialFields()
        {
            switch (_cardType)
            {
                case CardType.DivineBeast:
                    EditorGUILayout.LabelField("Divine Beast Information", EditorStyles.boldLabel);
                    _effectDescription = EditorGUILayout.TextArea(_effectDescription, GUILayout.Height(60));
                    _effectTargetStat = EditorGUILayout.TextField("Target Stat:", _effectTargetStat);
                    _effectValue = EditorGUILayout.FloatField("Effect Value:", _effectValue);
                    _effectDuration = EditorGUILayout.IntField("Effect Duration:", _effectDuration);
                    break;

                case CardType.Monster:
                    EditorGUILayout.LabelField("Monster Information", EditorStyles.boldLabel);
                    _effectDescription = EditorGUILayout.TextArea(_effectDescription, GUILayout.Height(60));
                    _effectTargetStat = EditorGUILayout.TextField("Effect Type:", _effectTargetStat);
                    _effectValue = EditorGUILayout.FloatField("Effect Value:", _effectValue);
                    _effectDuration = EditorGUILayout.IntField("Effect Duration:", _effectDuration);
                    break;

                case CardType.SpiritAnimal:
                    EditorGUILayout.LabelField("Spirit Animal Information", EditorStyles.boldLabel);
                    _zodiacAnimal = EditorGUILayout.TextField("Zodiac Animal:", _zodiacAnimal);
                    _effectDescription = EditorGUILayout.TextArea(_effectDescription, GUILayout.Height(60));
                    _activationType = (ActivationType)EditorGUILayout.EnumPopup("Activation Type:", _activationType);
                    _activationConditionDescription = EditorGUILayout.TextField("Condition Description:", _activationConditionDescription);
                    _conditionType = EditorGUILayout.TextField("Condition Type:", _conditionType);
                    _conditionValue = EditorGUILayout.FloatField("Condition Value:", _conditionValue);
                    break;

                case CardType.Joker:
                    EditorGUILayout.LabelField("Joker Information", EditorStyles.boldLabel);
                    _effectDescription = EditorGUILayout.TextArea(_effectDescription, GUILayout.Height(60));
                    _activationType = (ActivationType)EditorGUILayout.EnumPopup("Activation Type:", _activationType);
                    _activationConditionDescription = EditorGUILayout.TextField("Condition Description:", _activationConditionDescription);
                    _conditionType = EditorGUILayout.TextField("Condition Type:", _conditionType);
                    _conditionValue = EditorGUILayout.FloatField("Condition Value:", _conditionValue);
                    _cooldownTime = EditorGUILayout.IntField("Cooldown Time:", _cooldownTime);
                    break;
            }
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
                case CardType.DivineBeast:
                    cardData = CreateDivineBeastCard();
                    break;
                case CardType.Monster:
                    cardData = CreateMonsterCard();
                    break;
                case CardType.SpiritAnimal:
                    cardData = CreateSpiritAnimalCard();
                    break;
                case CardType.Joker:
                    cardData = CreateJokerCard();
                    break;
            }

            if (cardData != null)
            {
                // Get the output folder for this card type
                string outputFolder = GetOutputFolderForType(_cardType);
                
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
            
            // Set the appropriate Nap Am based on element type
            switch (_cardElement)
            {
                case ElementType.Metal:
                    card.metalNapAm = _metalNapAm;
                    break;
                case ElementType.Wood:
                    card.woodNapAm = _woodNapAm;
                    break;
                case ElementType.Water:
                    card.waterNapAm = _waterNapAm;
                    break;
                case ElementType.Fire:
                    card.fireNapAm = _fireNapAm;
                    break;
                case ElementType.Earth:
                    card.earthNapAm = _earthNapAm;
                    break;
            }
            
            return card;
        }

        /// <summary>
        /// Creates a Divine Beast Card
        /// </summary>
        private DivineBeastCardDataSO CreateDivineBeastCard()
        {
            DivineBeastCardDataSO card = CreateInstance<DivineBeastCardDataSO>();
            
            // Set basic properties
            card.cardId = _cardId;
            card.cardKeyName = _cardKeyName;
            card.cardName = _cardName;
            card.description = _cardDescription;
            card.cardType = CardType.DivineBeast;
            card.rarity = _cardRarity;
            card.cost = _cardCost;
            card.attack = _cardAttack;
            card.defense = _cardDefense;
            card.health = _cardHealth;
            card.speed = _cardSpeed;
            
            // Set Divine Beast specific properties
            card.effectDescription = _effectDescription;
            card.effectTargetStat = _effectTargetStat;
            card.effectValue = _effectValue;
            card.effectDuration = _effectDuration;
            
            return card;
        }

        /// <summary>
        /// Creates a Monster Card
        /// </summary>
        private MonsterCardDataSO CreateMonsterCard()
        {
            MonsterCardDataSO card = CreateInstance<MonsterCardDataSO>();
            
            // Set basic properties
            card.cardId = _cardId;
            card.cardKeyName = _cardKeyName;
            card.cardName = _cardName;
            card.description = _cardDescription;
            card.cardType = CardType.Monster;
            card.rarity = _cardRarity;
            card.cost = _cardCost;
            card.attack = _cardAttack;
            card.defense = _cardDefense;
            card.health = _cardHealth;
            card.speed = _cardSpeed;
            
            // Set Monster specific properties
            card.effects = new MonsterCardDataSO.EffectData[1];
            card.effects[0] = new MonsterCardDataSO.EffectData
            {
                effectName = _cardName + " Effect",
                effectDescription = _effectDescription,
                effectType = _effectTargetStat,
                effectValue = _effectValue,
                effectDuration = _effectDuration
            };
            
            return card;
        }

        /// <summary>
        /// Creates a Spirit Animal Card
        /// </summary>
        private SpiritAnimalCardDataSO CreateSpiritAnimalCard()
        {
            SpiritAnimalCardDataSO card = CreateInstance<SpiritAnimalCardDataSO>();
            
            // Set basic properties
            card.cardId = _cardId;
            card.cardKeyName = _cardKeyName;
            card.cardName = _cardName;
            card.description = _cardDescription;
            card.cardType = CardType.SpiritAnimal;
            card.rarity = _cardRarity;
            card.cost = _cardCost;
            card.attack = _cardAttack;
            card.defense = _cardDefense;
            card.health = _cardHealth;
            card.speed = _cardSpeed;
            
            // Set Spirit Animal specific properties
            card.zodiacAnimal = _zodiacAnimal;
            card.supportEffectDescription = _effectDescription;
            card.activationType = _activationType;
            card.activationConditionDescription = _activationConditionDescription;
            card.conditionType = _conditionType;
            card.conditionValue = _conditionValue;
            
            return card;
        }

        /// <summary>
        /// Creates a Joker Card
        /// </summary>
        private JokerCardDataSO CreateJokerCard()
        {
            JokerCardDataSO card = CreateInstance<JokerCardDataSO>();
            
            // Set basic properties
            card.cardId = _cardId;
            card.cardKeyName = _cardKeyName;
            card.cardName = _cardName;
            card.description = _cardDescription;
            card.cardType = CardType.Joker;
            card.rarity = _cardRarity;
            card.cost = _cardCost;
            card.attack = _cardAttack;
            card.defense = _cardDefense;
            card.health = _cardHealth;
            card.speed = _cardSpeed;
            
            // Set Joker specific properties
            card.effectDescription = _effectDescription;
            card.activationType = _activationType;
            card.activationConditionDescription = _activationConditionDescription;
            card.conditionType = _conditionType;
            card.conditionValue = _conditionValue;
            card.cooldownTime = _cooldownTime;
            
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
            _cardType = (CardType)EditorGUILayout.EnumPopup("Card Type:", _cardType);
            _cardRarity = (Rarity)EditorGUILayout.EnumPopup("Rarity:", _cardRarity);
            _batchElement = (ElementType)EditorGUILayout.EnumPopup("Element:", _batchElement);

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
                    EditorUtility.DisplayProgressBar("Creating Cards", $"Creating card {i + 1}/{_batchAmount}", (float)(i + 1) / _batchAmount);

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
                    
                    // Set the element from batch settings
                    _cardElement = _batchElement;
                    
                    // If ElementalCard, randomly select Nap Am based on element
                    if (_cardType == CardType.ElementalCard)
                    {
                        switch (_cardElement)
                        {
                            case ElementType.Metal:
                                _metalNapAm = (MetalNapAm)Random.Range(0, 6); // 0-5 for 6 types
                                break;
                            case ElementType.Wood:
                                _woodNapAm = (WoodNapAm)Random.Range(0, 6);
                                break;
                            case ElementType.Water:
                                _waterNapAm = (WaterNapAm)Random.Range(0, 6);
                                break;
                            case ElementType.Fire:
                                _fireNapAm = (FireNapAm)Random.Range(0, 6);
                                break;
                            case ElementType.Earth:
                                _earthNapAm = (EarthNapAm)Random.Range(0, 6);
                                break;
                        }
                    }
                    
                    // Random description if empty
                    if (string.IsNullOrEmpty(_cardDescription))
                    {
                        _cardDescription = $"Auto-generated {_cardElement} card with {GetNapAmName(_cardElement)} specialization.";
                    }

                    // Create the card based on type
                    ScriptableObject cardData = null;
                    
                    switch (_cardType)
                    {
                        case CardType.ElementalCard:
                            cardData = CreateElementalCard();
                            break;
                        case CardType.DivineBeast:
                            cardData = CreateDivineBeastCard();
                            break;
                        case CardType.Monster:
                            cardData = CreateMonsterCard();
                            break;
                        case CardType.SpiritAnimal:
                            cardData = CreateSpiritAnimalCard();
                            break;
                        case CardType.Joker:
                            cardData = CreateJokerCard();
                            break;
                    }

                    if (cardData != null)
                    {
                        // Get output folder for the card type
                        string outputFolder = GetOutputFolderForType(_cardType);
                        
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
                Dictionary<CardType, List<ScriptableObject>> cardsByType = GroupCardsByType();
                
                foreach (var group in cardsByType)
                {
                    EditorGUILayout.LabelField($"{group.Key} ({group.Value.Count}):", EditorStyles.boldLabel);
                    
                    foreach (var card in group.Value)
                    {
                        if (card == null) continue;
                        
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
                            if (EditorUtility.DisplayDialog("Confirm deletion", $"Are you sure you want to delete the card {card.name}?", "Delete", "Cancel"))
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
                    
                    EditorGUILayout.Space();
                }
            }

            EditorGUILayout.EndScrollView();
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
                folderPaths.Add($"{_baseOutputFolder}/DivineBeasts");
                folderPaths.Add($"{_baseOutputFolder}/Monsters");
                folderPaths.Add($"{_baseOutputFolder}/SpiritAnimals");
                folderPaths.Add($"{_baseOutputFolder}/Jokers");
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
                    
                    if (obj is CardDataSO || obj is ElementalCardDataSO || obj is DivineBeastCardDataSO || 
                        obj is MonsterCardDataSO || obj is SpiritAnimalCardDataSO || obj is JokerCardDataSO)
                    {
                        _generatedCards.Add(obj);
                    }
                }
            }
        }

        /// <summary>
        /// Groups cards by their type
        /// </summary>
        private Dictionary<CardType, List<ScriptableObject>> GroupCardsByType()
        {
            var result = new Dictionary<CardType, List<ScriptableObject>>();
            
            foreach (var card in _generatedCards)
            {
                if (card is CardDataSO cardData)
                {
                    if (!result.ContainsKey(cardData.cardType))
                    {
                        result[cardData.cardType] = new List<ScriptableObject>();
                    }
                    
                    result[cardData.cardType].Add(card);
                }
            }
            
            return result;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Get the Nap Am name based on selected element
        /// </summary>
        private string GetNapAmName(ElementType elementType)
        {
            switch (elementType)
            {
                case ElementType.Metal:
                    return _metalNapAm.ToString();
                case ElementType.Wood:
                    return _woodNapAm.ToString();
                case ElementType.Water:
                    return _waterNapAm.ToString();
                case ElementType.Fire:
                    return _fireNapAm.ToString();
                case ElementType.Earth:
                    return _earthNapAm.ToString();
                default:
                    return "Unknown";
            }
        }

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