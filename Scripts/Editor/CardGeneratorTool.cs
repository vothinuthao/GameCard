using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Core.Utils;
using Data;

namespace Editor
{
    /// <summary>
    /// Editor tool for generating cards automatically
    /// </summary>
    public partial class CardGeneratorTool : EditorWindow
    {
        #region Fields

        // Output folder
        private string _outputFolder = "Assets/Resources/Cards";

        // Default card values
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
        private int _napAmIndex = 0;

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
        private string[] _tabNames = { "Tạo Thẻ Đơn", "Tạo Thẻ Hàng Loạt", "Quản Lý Thẻ" };
        
        // Card preview
        private Texture2D _previewTexture;

        #endregion

        #region Editor Window

        /// <summary>
        /// Shows the editor window
        /// </summary>
        [MenuItem("Tools/Card Generator")]
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
            if (!Directory.Exists(_outputFolder))
            {
                Directory.CreateDirectory(_outputFolder);
            }

            GUILayout.Label("Công Cụ Tạo Thẻ Bài Ngũ Hành", EditorStyles.boldLabel);
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

        #endregion

        #region Single Card Tab

        /// <summary>
        /// Draws the tab for creating a single card
        /// </summary>
        private void DrawSingleCardTab()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.LabelField("Thông Tin Cơ Bản", EditorStyles.boldLabel);
            
            // Output folder
            EditorGUILayout.BeginHorizontal();
            _outputFolder = EditorGUILayout.TextField("Thư mục xuất:", _outputFolder);
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Chọn thư mục xuất", "Assets", "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // Convert absolute path to project-relative path
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        _outputFolder = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            // Basic card information
            _cardName = EditorGUILayout.TextField("Tên thẻ:", _cardName);
            _cardDescription = EditorGUILayout.TextArea(_cardDescription, GUILayout.Height(60));
            _cardType = (CardType)EditorGUILayout.EnumPopup("Loại thẻ:", _cardType);
            _cardRarity = (Rarity)EditorGUILayout.EnumPopup("Độ hiếm:", _cardRarity);
            _cardCost = EditorGUILayout.IntSlider("Chi phí:", _cardCost, 0, 5);

            // Stats
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Chỉ Số", EditorStyles.boldLabel);
            _cardAttack = EditorGUILayout.IntSlider("Tấn công:", _cardAttack, 0, 10);
            _cardDefense = EditorGUILayout.IntSlider("Phòng thủ:", _cardDefense, 0, 10);
            _cardHealth = EditorGUILayout.IntSlider("Máu:", _cardHealth, 1, 20);
            _cardSpeed = EditorGUILayout.IntSlider("Tốc độ:", _cardSpeed, 1, 10);

            // Element and NapAm
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Nguyên Tố & Nạp Âm", EditorStyles.boldLabel);
            _cardElement = (ElementType)EditorGUILayout.EnumPopup("Nguyên tố:", _cardElement);

            // Display different NapAm options based on the selected element
            switch (_cardElement)
            {
                case ElementType.Metal:
                    _napAmIndex = EditorGUILayout.Popup("Nạp âm:", _napAmIndex, GetMetalNapAmNames());
                    break;
                case ElementType.Wood:
                    _napAmIndex = EditorGUILayout.Popup("Nạp âm:", _napAmIndex, GetWoodNapAmNames());
                    break;
                case ElementType.Water:
                    _napAmIndex = EditorGUILayout.Popup("Nạp âm:", _napAmIndex, GetWaterNapAmNames());
                    break;
                case ElementType.Fire:
                    _napAmIndex = EditorGUILayout.Popup("Nạp âm:", _napAmIndex, GetFireNapAmNames());
                    break;
                case ElementType.Earth:
                    _napAmIndex = EditorGUILayout.Popup("Nạp âm:", _napAmIndex, GetEarthNapAmNames());
                    break;
            }

            // Special fields based on card type
            EditorGUILayout.Space();
            DrawSpecialFields();

            // Generate button
            EditorGUILayout.Space();
            if (GUILayout.Button("Tạo Thẻ", GUILayout.Height(30)))
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
                    EditorGUILayout.LabelField("Thông Tin Thần Thú", EditorStyles.boldLabel);
                    _effectDescription = EditorGUILayout.TextArea(_effectDescription, GUILayout.Height(60));
                    _effectTargetStat = EditorGUILayout.TextField("Chỉ số ảnh hưởng:", _effectTargetStat);
                    _effectValue = EditorGUILayout.FloatField("Giá trị hiệu ứng:", _effectValue);
                    _effectDuration = EditorGUILayout.IntField("Thời gian hiệu ứng:", _effectDuration);
                    break;

                case CardType.Monster:
                    EditorGUILayout.LabelField("Thông Tin Yêu Quái", EditorStyles.boldLabel);
                    _effectDescription = EditorGUILayout.TextArea(_effectDescription, GUILayout.Height(60));
                    _effectTargetStat = EditorGUILayout.TextField("Loại hiệu ứng:", _effectTargetStat);
                    _effectValue = EditorGUILayout.FloatField("Giá trị hiệu ứng:", _effectValue);
                    _effectDuration = EditorGUILayout.IntField("Thời gian hiệu ứng:", _effectDuration);
                    break;

                case CardType.SpiritAnimal:
                    EditorGUILayout.LabelField("Thông Tin Linh Thú", EditorStyles.boldLabel);
                    _zodiacAnimal = EditorGUILayout.TextField("Con giáp:", _zodiacAnimal);
                    _effectDescription = EditorGUILayout.TextArea(_effectDescription, GUILayout.Height(60));
                    _activationType = (ActivationType)EditorGUILayout.EnumPopup("Loại kích hoạt:", _activationType);
                    _activationConditionDescription = EditorGUILayout.TextField("Mô tả điều kiện:", _activationConditionDescription);
                    _conditionType = EditorGUILayout.TextField("Loại điều kiện:", _conditionType);
                    _conditionValue = EditorGUILayout.FloatField("Giá trị điều kiện:", _conditionValue);
                    break;

                case CardType.Joker:
                    EditorGUILayout.LabelField("Thông Tin Joker", EditorStyles.boldLabel);
                    _effectDescription = EditorGUILayout.TextArea(_effectDescription, GUILayout.Height(60));
                    _activationType = (ActivationType)EditorGUILayout.EnumPopup("Loại kích hoạt:", _activationType);
                    _activationConditionDescription = EditorGUILayout.TextField("Mô tả điều kiện:", _activationConditionDescription);
                    _conditionType = EditorGUILayout.TextField("Loại điều kiện:", _conditionType);
                    _conditionValue = EditorGUILayout.FloatField("Giá trị điều kiện:", _conditionValue);
                    _cooldownTime = EditorGUILayout.IntField("Thời gian hồi:", _cooldownTime);
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
                // Generate a filename based on the card type and name
                string sanitizedName = SanitizeFileName(_cardName);
                string assetPath = $"{_outputFolder}/{_cardElement}_{sanitizedName}_Card.asset";

                // Create the asset
                AssetDatabase.CreateAsset(cardData, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // Add to generated cards list
                _generatedCards.Add(cardData);

                // Select the created asset
                Selection.activeObject = cardData;

                Debug.Log($"Card created: {assetPath}");
                EditorUtility.DisplayDialog("Thành Công", $"Đã tạo thẻ: {_cardName}", "OK");
            }
        }

        #endregion

        #region Batch Card Tab

        /// <summary>
        /// Draws the tab for batch card generation
        /// </summary>
        private void DrawBatchCardTab()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.LabelField("Tạo Thẻ Hàng Loạt", EditorStyles.boldLabel);
            
            // Output folder
            EditorGUILayout.BeginHorizontal();
            _outputFolder = EditorGUILayout.TextField("Thư mục xuất:", _outputFolder);
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Chọn thư mục xuất", "Assets", "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // Convert absolute path to project-relative path
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        _outputFolder = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            // Batch settings
            _batchAmount = EditorGUILayout.IntSlider("Số lượng thẻ:", _batchAmount, 1, 20);
            _cardType = (CardType)EditorGUILayout.EnumPopup("Loại thẻ:", _cardType);
            _cardRarity = (Rarity)EditorGUILayout.EnumPopup("Độ hiếm:", _cardRarity);
            _batchElement = (ElementType)EditorGUILayout.EnumPopup("Nguyên tố:", _batchElement);

            // Stat ranges
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Phạm Vi Chỉ Số", EditorStyles.boldLabel);
            
            _randomizeStats = EditorGUILayout.Toggle("Ngẫu nhiên hóa chỉ số:", _randomizeStats);
            
            if (_randomizeStats)
            {
                EditorGUILayout.LabelField("Chi phí:", $"{_minCost} - {_maxCost}");
                EditorGUILayout.MinMaxSlider(ref _minCost, ref _maxCost, 0, 5);
                _minCost = Mathf.FloorToInt(_minCost);
                _maxCost = Mathf.CeilToInt(_maxCost);

                EditorGUILayout.LabelField("Tấn công:", $"{_minAttack} - {_maxAttack}");
                EditorGUILayout.MinMaxSlider(ref _minAttack, ref _maxAttack, 0, 10);
                _minAttack = Mathf.FloorToInt(_minAttack);
                _maxAttack = Mathf.CeilToInt(_maxAttack);

                EditorGUILayout.LabelField("Phòng thủ:", $"{_minDefense} - {_maxDefense}");
                EditorGUILayout.MinMaxSlider(ref _minDefense, ref _maxDefense, 0, 10);
                _minDefense = Mathf.FloorToInt(_minDefense);
                _maxDefense = Mathf.CeilToInt(_maxDefense);

                EditorGUILayout.LabelField("Máu:", $"{_minHealth} - {_maxHealth}");
                EditorGUILayout.MinMaxSlider(ref _minHealth, ref _maxHealth, 1, 20);
                _minHealth = Mathf.FloorToInt(_minHealth);
                _maxHealth = Mathf.CeilToInt(_maxHealth);

                EditorGUILayout.LabelField("Tốc độ:", $"{_minSpeed} - {_maxSpeed}");
                EditorGUILayout.MinMaxSlider(ref _minSpeed, ref _maxSpeed, 1, 10);
                _minSpeed = Mathf.FloorToInt(_minSpeed);
                _maxSpeed = Mathf.CeilToInt(_maxSpeed);
            }
            else
            {
                _cardCost = EditorGUILayout.IntSlider("Chi phí:", _cardCost, 0, 5);
                _cardAttack = EditorGUILayout.IntSlider("Tấn công:", _cardAttack, 0, 10);
                _cardDefense = EditorGUILayout.IntSlider("Phòng thủ:", _cardDefense, 0, 10);
                _cardHealth = EditorGUILayout.IntSlider("Máu:", _cardHealth, 1, 20);
                _cardSpeed = EditorGUILayout.IntSlider("Tốc độ:", _cardSpeed, 1, 10);
            }

            // Name prefix
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Tiền tố tên thẻ:", EditorStyles.boldLabel);
            _cardName = EditorGUILayout.TextField("Tiền tố:", _cardName);
            EditorGUILayout.HelpBox("Các thẻ sẽ được đặt tên theo mẫu: [Tiền tố]_[Số]", MessageType.Info);

            // Generate button
            EditorGUILayout.Space();
            if (GUILayout.Button("Tạo Thẻ Hàng Loạt", GUILayout.Height(30)))
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
            EditorUtility.DisplayProgressBar("Đang tạo thẻ", "Chuẩn bị...", 0f);

            try
            {
                for (int i = 0; i < _batchAmount; i++)
                {
                    // Update progress
                    EditorUtility.DisplayProgressBar("Đang tạo thẻ", $"Tạo thẻ {i + 1}/{_batchAmount}", (float)(i + 1) / _batchAmount);

                    // Randomize stats if needed
                    if (_randomizeStats)
                    {
                        _cardCost = (int)Random.Range(_minCost, _maxCost + 1);
                        _cardAttack = (int)Random.Range(_minAttack, _maxAttack + 1);
                        _cardDefense = (int)Random.Range(_minDefense, _maxDefense + 1);
                        _cardHealth = (int)Random.Range(_minHealth, _maxHealth + 1);
                        _cardSpeed = (int)Random.Range(_minSpeed, _maxSpeed + 1);
                    }

                    // Set card name with index
                    string originalName = _cardName;
                    _cardName = $"{originalName}_{i + 1}";
                    
                    // Randomize NapAm index
                    _napAmIndex = Random.Range(0, 6); // Assuming 6 NapAm types per element
                    
                    // Random description if empty
                    if (string.IsNullOrEmpty(_cardDescription))
                    {
                        _cardDescription = $"Thẻ {_cardElement} tự động tạo.";
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
                        // Generate a filename based on the card type and name
                        string sanitizedName = SanitizeFileName(_cardName);
                        string assetPath = $"{_outputFolder}/{_batchElement}_{sanitizedName}_Card.asset";

                        // Create the asset
                        AssetDatabase.CreateAsset(cardData, assetPath);
                        createdCards.Add(cardData);
                        successCount++;
                    }

                    // Reset name
                    _cardName = originalName;
                }

                // Save all assets at once
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // Add to generated cards list
                _generatedCards.AddRange(createdCards);

                EditorUtility.DisplayDialog("Thành Công", $"Đã tạo {successCount} thẻ bài.", "OK");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error creating batch cards: {e.Message}");
                EditorUtility.DisplayDialog("Lỗi", $"Có lỗi xảy ra: {e.Message}", "OK");
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

            EditorGUILayout.LabelField("Quản Lý Thẻ Bài", EditorStyles.boldLabel);

            // Refresh button
            if (GUILayout.Button("Làm Mới Danh Sách"))
            {
                RefreshCardList();
            }

            EditorGUILayout.Space();

            // Draw CSV import section
            DrawCsvImportSection();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Danh Sách Thẻ", EditorStyles.boldLabel);

            // Display all cards in the Resources folder
            if (_generatedCards.Count == 0)
            {
                EditorGUILayout.HelpBox("Không có thẻ nào. Hãy tạo thẻ hoặc làm mới danh sách.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.LabelField($"Tìm thấy {_generatedCards.Count} thẻ bài:", EditorStyles.boldLabel);
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
                        if (GUILayout.Button("Sửa", GUILayout.Width(60)))
                        {
                            Selection.activeObject = card;
                            EditorGUIUtility.PingObject(card);
                        }
                        
                        // Delete button
                        if (GUILayout.Button("Xóa", GUILayout.Width(60)))
                        {
                            if (EditorUtility.DisplayDialog("Xác nhận xóa", $"Bạn có chắc muốn xóa thẻ {card.name}?", "Xóa", "Hủy"))
                            {
                                string assetPath = AssetDatabase.GetAssetPath(card);
                                AssetDatabase.DeleteAsset(assetPath);
                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();
                                RefreshCardList();
                            }
                        }
                        
                        // Duplicate button
                        if (GUILayout.Button("Nhân bản", GUILayout.Width(80)))
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
            
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { _outputFolder });
            
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

        /// <summary>
        /// Exports the card list to a CSV file
        /// </summary>
        private void ExportCardList()
        {
            string path = EditorUtility.SaveFilePanel("Xuất danh sách thẻ", "", "CardList.csv", "csv");
            if (string.IsNullOrEmpty(path)) return;

            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    // Write header
                    writer.WriteLine("Name,Type,Rarity,Element,Cost,Attack,Defense,Health,Speed,Description");

                    // Write data
                    foreach (var card in _generatedCards)
                    {
                        if (card is CardDataSO cardData)
                        {
                            writer.WriteLine($"\"{cardData.cardName}\",{cardData.cardType},{cardData.rarity},{cardData.elementType},{cardData.cost},{cardData.attack},{cardData.defense},{cardData.health},{cardData.speed},\"{cardData.description.Replace("\"", "\"\"")}\"");
                        }
                    }
                }

                EditorUtility.DisplayDialog("Xuất Thành Công", $"Đã xuất {_generatedCards.Count} thẻ bài ra tệp CSV.", "OK");
                
                // Open the file
                System.Diagnostics.Process.Start(path);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error exporting card list: {e.Message}");
                EditorUtility.DisplayDialog("Lỗi", $"Có lỗi xảy ra khi xuất danh sách: {e.Message}", "OK");
            }
        }

        #endregion

        #region Card Creation Methods

        /// <summary>
        /// Creates an Elemental Card
        /// </summary>
        private ElementalCardDataSO CreateElementalCard()
        {
            ElementalCardDataSO card = CreateInstance<ElementalCardDataSO>();
            
            // Set basic properties
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
        /// Creates a Divine Beast Card
        /// </summary>
        private DivineBeastCardDataSO CreateDivineBeastCard()
        {
            DivineBeastCardDataSO card = CreateInstance<DivineBeastCardDataSO>();
            
            // Set basic properties
            card.cardName = _cardName;
            card.description = _cardDescription;
            card.cardType = CardType.DivineBeast;
            card.rarity = _cardRarity;
            card.cost = _cardCost;
            card.attack = _cardAttack;
            card.defense = _cardDefense;
            card.health = _cardHealth;
            card.speed = _cardSpeed;
            card.elementType = _cardElement;
            card.napAmIndex = _napAmIndex;
            
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
            card.cardName = _cardName;
            card.description = _cardDescription;
            card.cardType = CardType.Monster;
            card.rarity = _cardRarity;
            card.cost = _cardCost;
            card.attack = _cardAttack;
            card.defense = _cardDefense;
            card.health = _cardHealth;
            card.speed = _cardSpeed;
            card.elementType = _cardElement;
            card.napAmIndex = _napAmIndex;
            
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
            card.cardName = _cardName;
            card.description = _cardDescription;
            card.cardType = CardType.SpiritAnimal;
            card.rarity = _cardRarity;
            card.cost = _cardCost;
            card.attack = _cardAttack;
            card.defense = _cardDefense;
            card.health = _cardHealth;
            card.speed = _cardSpeed;
            card.elementType = _cardElement;
            card.napAmIndex = _napAmIndex;
            
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
            card.cardName = _cardName;
            card.description = _cardDescription;
            card.cardType = CardType.Joker;
            card.rarity = _cardRarity;
            card.cost = _cardCost;
            card.attack = _cardAttack;
            card.defense = _cardDefense;
            card.health = _cardHealth;
            card.speed = _cardSpeed;
            card.elementType = _cardElement;
            card.napAmIndex = _napAmIndex;
            
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

        #region Utility Methods

        /// <summary>
        /// Gets the names of Metal NapAm
        /// </summary>
        private string[] GetMetalNapAmNames()
        {
            return new string[]
            {
                "Kiếm Khí (Sword Qi)",
                "Cương Nghị (Hardness)",
                "Thanh Tịnh (Purity)",
                "Phản Chiếu (Reflection)",
                "Linh Khí (Spirit)",
                "Trầm Tĩnh (Calmness)"
            };
        }

        /// <summary>
        /// Gets the names of Wood NapAm
        /// </summary>
        private string[] GetWoodNapAmNames()
        {
            return new string[]
            {
                "Sinh Trưởng (Growth)",
                "Linh Hoạt (Flexibility)",
                "Cộng Sinh (Symbiosis)",
                "Tái Sinh (Regeneration)",
                "Độc Tố (Toxin)",
                "Che Chắn (Shelter)"
            };
        }

        /// <summary>
        /// Gets the names of Water NapAm
        /// </summary>
        private string[] GetWaterNapAmNames()
        {
            return new string[]
            {
                "Thích Nghi (Adaptation)",
                "Băng Giá (Ice)",
                "Dòng Chảy (Flow)",
                "Sương Mù (Mist)",
                "Phản Ánh (Reflection)",
                "Thanh Tẩy (Purification)"
            };
        }

        /// <summary>
        /// Gets the names of Fire NapAm
        /// </summary>
        private string[] GetFireNapAmNames()
        {
            return new string[]
            {
                "Thiêu Đốt (Burning)",
                "Bùng Nổ (Explosion)",
                "Nhiệt Huyết (Passion)",
                "Ánh Sáng (Light)",
                "Rèn Luyện (Forging)",
                "Thiêu Rụi (Incineration)"
            };
        }

        /// <summary>
        /// Gets the names of Earth NapAm
        /// </summary>
        private string[] GetEarthNapAmNames()
        {
            return new string[]
            {
                "Kiên Cố (Solidity)",
                "Trọng Lực (Gravity)",
                "Màu Mỡ (Fertility)",
                "Núi Lửa (Volcano)",
                "Tinh Thể (Crystal)",
                "Đại Địa (Terra)"
            };
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