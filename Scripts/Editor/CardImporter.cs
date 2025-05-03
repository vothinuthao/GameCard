// File: Scripts/Editor/CardImporter.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Core.Utils;
using Data;
using System;
using System.Text.RegularExpressions;

namespace Editor
{
    /// <summary>
    /// Extension of EnhancedCardGeneratorTool to handle CSV import and export
    /// </summary>
    public partial class CardGeneratorTool : EditorWindow
    {
        private string _csvPath = "";
        private string _importMessage = "";
        private bool _showImportUI = false;
        private bool _overwriteExisting = false;
        private List<CardImportData> _cardsToImport = new List<CardImportData>();
        private Vector2 _importScrollPosition;
        
        // Structure to hold data from CSV
        private class CardImportData
        {
            public int Id;
            public string KeyName;
            public string CardName;
            public string Description;
            public CardType CardType;
            public Rarity Rarity;
            public int Cost;
            public int Attack;
            public int Defense;
            public int Health;
            public int Speed;
            public ElementType ElementType;
            
            // New Nap Am fields using enums
            public MetalNapAm MetalNapAm;
            public WoodNapAm WoodNapAm;
            public WaterNapAm WaterNapAm;
            public FireNapAm FireNapAm;
            public EarthNapAm EarthNapAm;
            
            public string EffectDescription;
            public string EffectTargetStat;
            public float EffectValue;
            public int EffectDuration;
            public string ZodiacAnimal;
            public ActivationType ActivationType;
            public string ActivationConditionDescription;
            public string ConditionType;
            public float ConditionValue;
            public int CooldownTime;
            public bool Selected = true; // For UI selection
        }
        
        /// <summary>
        /// Draws the CSV import section in the Card Manager tab
        /// </summary>
        private void DrawCsvImportSection()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Import/Export CSV", EditorStyles.boldLabel);
            
            // CSV Import
            EditorGUILayout.BeginHorizontal();
            _csvPath = EditorGUILayout.TextField("CSV Path:", _csvPath);
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string selectedPath = EditorUtility.OpenFilePanel("Select CSV File", "", "csv");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    _csvPath = selectedPath;
                    _showImportUI = false;
                    _importMessage = "";
                }
            }
            EditorGUILayout.EndHorizontal();
            
            // Import options
            _overwriteExisting = EditorGUILayout.Toggle("Overwrite existing cards:", _overwriteExisting);
            _useSubfolders = EditorGUILayout.Toggle("Use type subfolders:", _useSubfolders);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Read CSV", GUILayout.Width(100)))
            {
                ReadCsvFile();
            }
            
            if (GUILayout.Button("Create CSV Template", GUILayout.Width(150)))
            {
                CreateCsvTemplate();
            }
            
            if (GUILayout.Button("Export All Cards to CSV", GUILayout.Width(180)))
            {
                ExportAllCardsToCSV();
            }
            EditorGUILayout.EndHorizontal();
            
            // Show import message
            if (!string.IsNullOrEmpty(_importMessage))
            {
                EditorGUILayout.HelpBox(_importMessage, _importMessage.Contains("error") ? MessageType.Error : MessageType.Info);
            }
            
            // Draw import UI if data is loaded
            if (_showImportUI && _cardsToImport.Count > 0)
            {
                DrawImportUI();
            }
        }
        
        /// <summary>
        /// Draws the UI for selecting which cards to import
        /// </summary>
        private void DrawImportUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Found {_cardsToImport.Count} cards in CSV:", EditorStyles.boldLabel);
            
            // Select/Deselect All
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select All", GUILayout.Width(100)))
            {
                foreach (var card in _cardsToImport)
                {
                    card.Selected = true;
                }
            }
            
            if (GUILayout.Button("Deselect All", GUILayout.Width(120)))
            {
                foreach (var card in _cardsToImport)
                {
                    card.Selected = false;
                }
            }
            EditorGUILayout.EndHorizontal();
            
            // Display cards to import
            _importScrollPosition = EditorGUILayout.BeginScrollView(_importScrollPosition, GUILayout.Height(300));
            
            for (int i = 0; i < _cardsToImport.Count; i++)
            {
                CardImportData card = _cardsToImport[i];
                
                EditorGUILayout.BeginHorizontal("box");
                
                // Checkbox for selection
                card.Selected = EditorGUILayout.Toggle(card.Selected, GUILayout.Width(20));
                
                // Card info
                string napAmName = GetNapAmName(card.ElementType, card);
                EditorGUILayout.LabelField($"{card.Id}: {card.CardName} ({card.CardType}, {card.ElementType}, {napAmName})", EditorStyles.boldLabel);
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
            
            // Import button
            if (GUILayout.Button("Import Selected Cards", GUILayout.Height(30)))
            {
                ImportSelectedCards();
            }
        }
        
        /// <summary>
        /// Creates a template CSV file
        /// </summary>
        private void CreateCsvTemplate()
        {
            string path = EditorUtility.SaveFilePanel("Save CSV Template", "", "Card_Template.csv", "csv");
            if (string.IsNullOrEmpty(path)) return;

            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    // Write header
                    writer.WriteLine("Id,KeyName,CardName,Description,CardType,Rarity,Cost,Attack,Defense,Health,Speed,ElementType,MetalNapAm,WoodNapAm,WaterNapAm,FireNapAm,EarthNapAm,EffectDescription,EffectTargetStat,EffectValue,EffectDuration,ZodiacAnimal,ActivationType,ActivationConditionDescription,ConditionType,ConditionValue,CooldownTime");
                    
                    // Write example rows for different card types
                    
                    // Metal Card Example
                    writer.WriteLine("1,metal_sword,Kim Chúc Kiếm,Thanh kiếm bằng kim loại sắc bén có khả năng chém xuyên giáp,ElementalCard,Common,1,4,1,3,2,Metal,SwordQi,,,,,,,,,,,,,,,");
                    
                    // Wood Card Example
                    writer.WriteLine("2,wood_shield,Sinh Mệnh Chi Mộc,Cây đem lại sức mạnh tái sinh và hồi phục,ElementalCard,Common,1,1,3,5,2,Wood,,Regeneration,,,,,,,,,,,,,,");
                    
                    // Water Card Example
                    writer.WriteLine("3,water_ice,Phong Tỏa Băng Hà,Dòng nước đóng băng kẻ địch làm chậm mọi chuyển động,ElementalCard,Rare,2,2,4,4,3,Water,,,Ice,,,,,,,,,,,,,");
                    
                    // Fire Card Example
                    writer.WriteLine("4,fire_explosion,Bùng Nổ Nhiệt Bạo,Ngọn lửa bùng cháy tạo ra vụ nổ mạnh mẽ,ElementalCard,Rare,2,5,1,3,4,Fire,,,,Explosion,,,,,,,,,,,,");
                    
                    // Earth Card Example
                    writer.WriteLine("5,earth_crystal,Tinh Thể Kỳ Diệu,Tinh thể từ lòng đất với sức mạnh kỳ diệu,ElementalCard,Epic,3,3,5,6,2,Earth,,,,,Crystal,,,,,,,,,,,");
                    
                    // Divine Beast Example
                    writer.WriteLine("9,white_tiger,Bạch Hổ Thần Thú,Thần thú phương Tây chúa tể của Kim tăng sức mạnh cho các thẻ Kim,DivineBeast,Epic,3,6,4,12,5,Metal,SwordQi,,,,,,Tăng sức tấn công của tất cả thẻ Kim,attack,2,3,,,,,,");
                    
                    // Monster Example
                    writer.WriteLine("11,nine_tailed_fox,Cửu Vĩ Hồ,Hồ ly chín đuôi với sức mạnh mê hoặc có thể khiến đối thủ tấn công đồng đội,Monster,Epic,3,5,3,10,6,Fire,,,,Passion,,30% cơ hội làm đối thủ đánh nhầm vào đồng đội,control,0.3,2,,,,,,");
                    
                    // Spirit Animal Example
                    writer.WriteLine("12,rat_swift,Tý - Thần Tốc,Chuột nhanh nhẹn tăng tốc độ và khả năng né tránh,SpiritAnimal,Rare,2,2,1,4,7,Water,,,Adaptation,,,Tăng tốc độ và né tránh mỗi khi chơi 2 lá bài,,,,,Rat,Recurring,Mỗi khi chơi 2 lá bài,card_count,2,");
                    
                    // Joker Example
                    writer.WriteLine("13,heaven_earth_reversal,Càn Khôn Đảo Chuyển,Đảo ngược tất cả hiệu ứng tương sinh thành tương khắc và ngược lại,Joker,Legendary,4,0,0,8,0,None,,,,,,,Đảo ngược tất cả hiệu ứng tương sinh thành tương khắc và ngược lại trong 3 lượt,,,,,,Transformative,Kích hoạt khi có ít nhất 3 nguyên tố khác nhau trong tay,element_diversity,3,5");
                }

                _importMessage = "CSV template created successfully!";
                EditorUtility.RevealInFinder(path);
            }
            catch (Exception e)
            {
                _importMessage = $"Error creating CSV template: {e.Message}";
                Debug.LogError(_importMessage);
            }
        }
        
        /// <summary>
        /// Reads the CSV file and parses card data
        /// </summary>
        private void ReadCsvFile()
        {
            if (string.IsNullOrEmpty(_csvPath) || !File.Exists(_csvPath))
            {
                _importMessage = "Invalid CSV path!";
                _showImportUI = false;
                return;
            }

            try
            {
                _cardsToImport.Clear();
                
                // Read all lines
                string[] lines = File.ReadAllLines(_csvPath);
                
                // Check if file is not empty
                if (lines.Length <= 1)
                {
                    _importMessage = "CSV file is empty or only contains a header!";
                    _showImportUI = false;
                    return;
                }
                
                // Skip header row
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;
                    
                    // Parse CSV row
                    CardImportData cardData = ParseCsvLine(line);
                    if (cardData != null)
                    {
                        _cardsToImport.Add(cardData);
                    }
                }
                
                if (_cardsToImport.Count > 0)
                {
                    _importMessage = $"Read {_cardsToImport.Count} cards from CSV.";
                    _showImportUI = true;
                }
                else
                {
                    _importMessage = "No valid card data found in the CSV.";
                    _showImportUI = false;
                }
            }
            catch (Exception e)
            {
                _importMessage = $"Error reading CSV file: {e.Message}";
                Debug.LogError(_importMessage);
                _showImportUI = false;
            }
        }
        
        /// <summary>
        /// Parses a CSV line into CardImportData
        /// </summary>
        private CardImportData ParseCsvLine(string line)
        {
            try
            {
                // CSV can contain commas inside quotes, so we need a proper parser
                List<string> values = ParseCSVRow(line);
                
                // Check if we have enough values
                if (values.Count < 13) // We need at least basic info plus Id and KeyName
                {
                    Debug.LogWarning($"Not enough data in line: {line}");
                    return null;
                }
                
                CardImportData card = new CardImportData();
                
                // Parse basic info
                card.Id = ParseInt(values[0]);
                card.KeyName = values[1];
                card.CardName = values[2];
                card.Description = values[3];
                card.CardType = ParseEnum<CardType>(values[4]);
                card.Rarity = ParseEnum<Rarity>(values[5]);
                card.Cost = ParseInt(values[6]);
                card.Attack = ParseInt(values[7]);
                card.Defense = ParseInt(values[8]);
                card.Health = ParseInt(values[9]);
                card.Speed = ParseInt(values[10]);
                card.ElementType = ParseEnum<ElementType>(values[11]);
                
                // Parse Nap Am fields based on the new schema
                if (values.Count > 12) card.MetalNapAm = ParseEnum<MetalNapAm>(values[12]);
                if (values.Count > 13) card.WoodNapAm = ParseEnum<WoodNapAm>(values[13]);
                if (values.Count > 14) card.WaterNapAm = ParseEnum<WaterNapAm>(values[14]);
                if (values.Count > 15) card.FireNapAm = ParseEnum<FireNapAm>(values[15]);
                if (values.Count > 16) card.EarthNapAm = ParseEnum<EarthNapAm>(values[16]);
                
                // Parse optional fields
                if (values.Count > 17) card.EffectDescription = values[17];
                if (values.Count > 18) card.EffectTargetStat = values[18];
                if (values.Count > 19) card.EffectValue = ParseFloat(values[19]);
                if (values.Count > 20) card.EffectDuration = ParseInt(values[20]);
                if (values.Count > 21) card.ZodiacAnimal = values[21];
                if (values.Count > 22) card.ActivationType = ParseEnum<ActivationType>(values[22]);
                if (values.Count > 23) card.ActivationConditionDescription = values[23];
                if (values.Count > 24) card.ConditionType = values[24];
                if (values.Count > 25) card.ConditionValue = ParseFloat(values[25]);
                if (values.Count > 26) card.CooldownTime = ParseInt(values[26]);
                
                return card;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error parsing CSV line: {line}, Error: {e.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Get the NapAm name based on element type and import data
        /// </summary>
        private string GetNapAmName(ElementType elementType, CardImportData cardData)
        {
            switch (elementType)
            {
                case ElementType.Metal:
                    return cardData.MetalNapAm.ToString();
                case ElementType.Wood:
                    return cardData.WoodNapAm.ToString();
                case ElementType.Water:
                    return cardData.WaterNapAm.ToString();
                case ElementType.Fire:
                    return cardData.FireNapAm.ToString();
                case ElementType.Earth:
                    return cardData.EarthNapAm.ToString();
                default:
                    return "Unknown";
            }
        }
        
        /// <summary>
        /// Imports the selected cards from CSV
        /// </summary>
        private void ImportSelectedCards()
        {
            int successCount = 0;
            
            // Create progress bar
            EditorUtility.DisplayProgressBar("Importing cards", "Preparing...", 0f);
            
            try
            {
                List<ScriptableObject> createdCards = new List<ScriptableObject>();
                
                // Process each selected card
                for (int i = 0; i < _cardsToImport.Count; i++)
                {
                    if (!_cardsToImport[i].Selected) continue;
                    
                    CardImportData cardData = _cardsToImport[i];
                    
                    // Update progress
                    EditorUtility.DisplayProgressBar("Importing cards", 
                        $"Creating card: {cardData.CardName}", 
                        (float)(i + 1) / _cardsToImport.Count);
                    
                    // Copy data to field variables for creation methods
                    _cardId = cardData.Id;
                    _cardKeyName = cardData.KeyName;
                    _cardName = cardData.CardName;
                    _cardDescription = cardData.Description;
                    _cardType = cardData.CardType;
                    _cardRarity = cardData.Rarity;
                    _cardCost = cardData.Cost;
                    _cardAttack = cardData.Attack;
                    _cardDefense = cardData.Defense;
                    _cardHealth = cardData.Health;
                    _cardSpeed = cardData.Speed;
                    _cardElement = cardData.ElementType;
                    
                    // Set the appropriate NapAm based on element type
                    switch (_cardElement)
                    {
                        case ElementType.Metal:
                            _metalNapAm = cardData.MetalNapAm;
                            break;
                        case ElementType.Wood:
                            _woodNapAm = cardData.WoodNapAm;
                            break;
                        case ElementType.Water:
                            _waterNapAm = cardData.WaterNapAm;
                            break;
                        case ElementType.Fire:
                            _fireNapAm = cardData.FireNapAm;
                            break;
                        case ElementType.Earth:
                            _earthNapAm = cardData.EarthNapAm;
                            break;
                    }
                    
                    _effectDescription = cardData.EffectDescription;
                    _effectTargetStat = cardData.EffectTargetStat;
                    _effectValue = cardData.EffectValue;
                    _effectDuration = cardData.EffectDuration;
                    _zodiacAnimal = cardData.ZodiacAnimal;
                    _activationType = cardData.ActivationType;
                    _activationConditionDescription = cardData.ActivationConditionDescription;
                    _conditionType = cardData.ConditionType;
                    _conditionValue = cardData.ConditionValue;
                    _cooldownTime = cardData.CooldownTime;
                    
                    // Create card based on type
                    ScriptableObject card = null;
                    switch (cardData.CardType)
                    {
                        case CardType.ElementalCard:
                            card = CreateElementalCard();
                            break;
                        case CardType.DivineBeast:
                            card = CreateDivineBeastCard();
                            break;
                        case CardType.Monster:
                            card = CreateMonsterCard();
                            break;
                        case CardType.SpiritAnimal:
                            card = CreateSpiritAnimalCard();
                            break;
                        case CardType.Joker:
                            card = CreateJokerCard();
                            break;
                    }
                    
                    if (card != null)
                    {
                        // Get the output folder for this card type
                        string outputFolder = GetOutputFolderForType(cardData.CardType);
                        
                        // Sanitize card key name to ensure it's valid for a filename
                        string sanitizedKeyName = SanitizeFileName(cardData.KeyName);
                        
                        // Generate filename
                        string assetPath = $"{outputFolder}/{sanitizedKeyName}.asset";
                        
                        // Check if asset already exists
                        bool exists = File.Exists(assetPath);
                        if (exists && !_overwriteExisting)
                        {
                            assetPath = $"{outputFolder}/{sanitizedKeyName}_Import.asset";
                        }
                        
                        // Create/update asset
                        if (exists && _overwriteExisting)
                        {
                            AssetDatabase.DeleteAsset(assetPath);
                        }
                        
                        AssetDatabase.CreateAsset(card, assetPath);
                        createdCards.Add(card);
                        successCount++;
                    }
                }
                
                // Save all assets
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                // Add to generated cards list
                _generatedCards.AddRange(createdCards);
                
                _importMessage = $"Successfully imported {successCount} cards from CSV.";
                _showImportUI = false;
                
                // Refresh card list
                RefreshCardList();
            }
            catch (Exception e)
            {
                _importMessage = $"Error importing cards: {e.Message}";
                Debug.LogError(_importMessage);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        
        /// <summary>
        /// Export all cards to CSV
        /// </summary>
        private void ExportAllCardsToCSV()
        {
            // Refresh card list to make sure we have all cards
            RefreshCardList();
            
            if (_generatedCards.Count == 0)
            {
                _importMessage = "No cards to export!";
                return;
            }
            
            string path = EditorUtility.SaveFilePanel("Export cards to CSV", "", "Cards_Export.csv", "csv");
            if (string.IsNullOrEmpty(path)) return;
            
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    // Write header
                    writer.WriteLine("Id,KeyName,CardName,Description,CardType,Rarity,Cost,Attack,Defense,Health,Speed,ElementType,MetalNapAm,WoodNapAm,WaterNapAm,FireNapAm,EarthNapAm,EffectDescription,EffectTargetStat,EffectValue,EffectDuration,ZodiacAnimal,ActivationType,ActivationConditionDescription,ConditionType,ConditionValue,CooldownTime");
                    
                    // Write card data
                    foreach (var cardObj in _generatedCards)
                    {
                        if (cardObj is ElementalCardDataSO elementalCard)
                        {
                            // Base card data
                            string line = $"{elementalCard.cardId},\"{EscapeCsvField(elementalCard.cardKeyName)}\",\"{EscapeCsvField(elementalCard.cardName)}\",\"{EscapeCsvField(elementalCard.description)}\",{elementalCard.cardType},{elementalCard.rarity},{elementalCard.cost},{elementalCard.attack},{elementalCard.defense},{elementalCard.health},{elementalCard.speed},{elementalCard.elementType}";
                            
                            // Add Nap Am data based on element type
                            string metalNapAm = elementalCard.elementType == ElementType.Metal ? elementalCard.metalNapAm.ToString() : "";
                            string woodNapAm = elementalCard.elementType == ElementType.Wood ? elementalCard.woodNapAm.ToString() : "";
                            string waterNapAm = elementalCard.elementType == ElementType.Water ? elementalCard.waterNapAm.ToString() : "";
                            string fireNapAm = elementalCard.elementType == ElementType.Fire ? elementalCard.fireNapAm.ToString() : "";
                            string earthNapAm = elementalCard.elementType == ElementType.Earth ? elementalCard.earthNapAm.ToString() : "";
                            
                            line += $",{metalNapAm},{woodNapAm},{waterNapAm},{fireNapAm},{earthNapAm}";
                            
                            // Empty fields for other details
                            line += ",,,,,,,,,,";
                            
                            writer.WriteLine(line);
                        }
                        else if (cardObj is DivineBeastCardDataSO divineBeast)
                        {
                            string line = $"{divineBeast.cardId},\"{EscapeCsvField(divineBeast.cardKeyName)}\",\"{EscapeCsvField(divineBeast.cardName)}\",\"{EscapeCsvField(divineBeast.description)}\",{divineBeast.cardType},{divineBeast.rarity},{divineBeast.cost},{divineBeast.attack},{divineBeast.defense},{divineBeast.health},{divineBeast.speed},{divineBeast.elementType}";
                            
                            // Add empty NapAm fields
                            line += ",,,,,";
                            
                            // Add Divine Beast specific fields
                            line += $",\"{EscapeCsvField(divineBeast.effectDescription)}\",{divineBeast.effectTargetStat},{divineBeast.effectValue},{divineBeast.effectDuration}";
                            
                            // Empty fields for other details
                            line += ",,,,,,";
                            
                            writer.WriteLine(line);
                        }
                        else if (cardObj is SpiritAnimalCardDataSO spiritAnimal)
                        {
                            string line = $"{spiritAnimal.cardId},\"{EscapeCsvField(spiritAnimal.cardKeyName)}\",\"{EscapeCsvField(spiritAnimal.cardName)}\",\"{EscapeCsvField(spiritAnimal.description)}\",{spiritAnimal.cardType},{spiritAnimal.rarity},{spiritAnimal.cost},{spiritAnimal.attack},{spiritAnimal.defense},{spiritAnimal.health},{spiritAnimal.speed},{spiritAnimal.elementType}";
                            
                            // Add empty NapAm fields
                            line += ",,,,,";
                            
                            // Add Spirit Animal specific fields
                            line += $",\"{EscapeCsvField(spiritAnimal.supportEffectDescription)}\",,,,\"{spiritAnimal.zodiacAnimal}\",{spiritAnimal.activationType},\"{EscapeCsvField(spiritAnimal.activationConditionDescription)}\",{spiritAnimal.conditionType},{spiritAnimal.conditionValue},";
                            
                            writer.WriteLine(line);
                        }
                        else if (cardObj is JokerCardDataSO joker)
                        {
                            string line = $"{joker.cardId},\"{EscapeCsvField(joker.cardKeyName)}\",\"{EscapeCsvField(joker.cardName)}\",\"{EscapeCsvField(joker.description)}\",{joker.cardType},{joker.rarity},{joker.cost},{joker.attack},{joker.defense},{joker.health},{joker.speed},{joker.elementType}";
                            
                            // Add empty NapAm fields
                            line += ",,,,,";
                            
                            // Add Joker specific fields
                            line += $",\"{EscapeCsvField(joker.effectDescription)}\",,,,,,{joker.activationType},\"{EscapeCsvField(joker.activationConditionDescription)}\",{joker.conditionType},{joker.conditionValue},{joker.cooldownTime}";
                            
                            writer.WriteLine(line);
                        }
                        else if (cardObj is MonsterCardDataSO monster && monster.effects.Length > 0)
                        {
                            string line = $"{monster.cardId},\"{EscapeCsvField(monster.cardKeyName)}\",\"{EscapeCsvField(monster.cardName)}\",\"{EscapeCsvField(monster.description)}\",{monster.cardType},{monster.rarity},{monster.cost},{monster.attack},{monster.defense},{monster.health},{monster.speed},{monster.elementType}";
                            
                            // Add empty NapAm fields
                            line += ",,,,,";
                            
                            // Add Monster specific fields
                            var effect = monster.effects[0];
                            line += $",\"{EscapeCsvField(effect.effectDescription)}\",{effect.effectType},{effect.effectValue},{effect.effectDuration}";
                            
                            // Empty fields for other details
                            line += ",,,,,,";
                            
                            writer.WriteLine(line);
                        }
                    }
                }
                
                _importMessage = $"Successfully exported {_generatedCards.Count} cards to CSV!";
                EditorUtility.RevealInFinder(path);
            }
            catch (Exception e)
            {
                _importMessage = $"Error exporting CSV: {e.Message}";
                Debug.LogError(_importMessage);
            }
        }
        
        /// <summary>
        /// Escapes a field for CSV output
        /// </summary>
        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";
                
            // Replace double quotes with two double quotes
            return field.Replace("\"", "\"\"");
        }
        
        /// <summary>
        /// Parse CSV row that may contain quoted fields
        /// </summary>
        private List<string> ParseCSVRow(string line)
        {
            List<string> result = new List<string>();
            bool inQuotes = false;
            int startIndex = 0;
            
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (line[i] == ',' && !inQuotes)
                {
                    // Add field to result
                    string field = line.Substring(startIndex, i - startIndex).Trim();
                    
                    // Remove surrounding quotes if present
                    if (field.StartsWith("\"") && field.EndsWith("\"") && field.Length >= 2)
                    {
                        field = field.Substring(1, field.Length - 2);
                        field = field.Replace("\"\"", "\""); // Replace double quotes with single quotes
                    }
                    
                    result.Add(field);
                    startIndex = i + 1;
                }
            }
            
            // Add the last field
            if (startIndex < line.Length)
            {
                string field = line.Substring(startIndex).Trim();
                
                // Remove surrounding quotes if present
                if (field.StartsWith("\"") && field.EndsWith("\"") && field.Length >= 2)
                {
                    field = field.Substring(1, field.Length - 2);
                    field = field.Replace("\"\"", "\""); // Replace double quotes with single quotes
                }
                
                result.Add(field);
            }
            
            return result;
        }
        
        /// <summary>
        /// Safely parse an enum value
        /// </summary>
        private T ParseEnum<T>(string value) where T : Enum
        {
            if (string.IsNullOrEmpty(value))
                return default(T);
                
            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch
            {
                return default(T);
            }
        }
        
        /// <summary>
        /// Safely parse an integer
        /// </summary>
        private int ParseInt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
                
            int result;
            if (int.TryParse(value, out result))
                return result;
                
            return 0;
        }
        
        /// <summary>
        /// Safely parse a float
        /// </summary>
        private float ParseFloat(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0f;
                
            float result;
            if (float.TryParse(value, out result))
                return result;
                
            return 0f;
        }
    }
}