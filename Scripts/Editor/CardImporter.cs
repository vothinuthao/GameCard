// File: Scripts/Editor/CardImporter.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Core.Utils;
using Data;
using System;
using System.Text.RegularExpressions;
using Systems.StatesMachine;

namespace Editor
{
    /// <summary>
    /// Extension of CardGeneratorTool to handle CSV import and export
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
            public SupportCardType SupportCardType;
            public Rarity Rarity;
            public int Cost;
            public int Attack;
            public int Defense;
            public int Health;
            public int Speed;
            public int NapAmIndex;
            public ElementType ElementType;
            
            // Nap Am fields using enums
            public MetalNapAm MetalNapAm;
            public WoodNapAm WoodNapAm;
            public WaterNapAm WaterNapAm;
            public FireNapAm FireNapAm;
            public EarthNapAm EarthNapAm;
            
            // Support card fields
            public string EffectDescription;
            public string EffectParameter;
            public float EffectValue;
            public float EffectValue2;
            public int EffectDuration;
            public string ZodiacAnimal;
            public ActivationType ActivationType;
            public string ActivationConditionDescription;
            public ActivationConditionType ConditionType;
            public string ConditionParameter;
            public float ConditionValue;
            public float ConditionValue2;
            public int CooldownTime;
            public EffectType EffectType;
            
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
                string typeInfo = card.CardType == CardType.ElementalCard 
                    ? $"{card.CardType}, {card.ElementType}, {GetNapAmNameForImport(card)}" 
                    : $"{card.CardType}, {card.SupportCardType}";
                
                EditorGUILayout.LabelField($"{card.Id}: {card.CardName} ({typeInfo})", EditorStyles.boldLabel);
                
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
                    // Write updated header with the new structure
                    writer.WriteLine("Id,KeyName,CardName,Description,CardType,SupportCardType,Rarity,Cost,Attack,Defense,Health,Speed,ElementType,MetalNapAm,WoodNapAm,WaterNapAm,FireNapAm,EarthNapAm,EffectDescription,EffectType,EffectParameter,EffectValue,EffectValue2,EffectDuration,ZodiacAnimal,ActivationType,ActivationConditionDescription,ConditionType,ConditionParameter,ConditionValue,ConditionValue2,CooldownTime");
                    
                    // Write example rows for different card types
                    
                    // Metal Card Example
                    writer.WriteLine("1,metal_sword,Kim Chúc Kiếm,Thanh kiếm bằng kim loại sắc bén có khả năng chém xuyên giáp,ElementalCard,,Common,1,4,1,3,2,Metal,SwordQi,,,,,,,,,,,,,,,,,,");
                    
                    // Wood Card Example
                    writer.WriteLine("2,wood_shield,Sinh Mệnh Chi Mộc,Cây đem lại sức mạnh tái sinh và hồi phục,ElementalCard,,Common,1,1,3,5,2,Wood,,Regeneration,,,,,,,,,,,,,,,,,");
                    
                    // Water Card Example
                    writer.WriteLine("3,water_ice,Phong Tỏa Băng Hà,Dòng nước đóng băng kẻ địch làm chậm mọi chuyển động,ElementalCard,,Rare,2,2,4,4,3,Water,,,Ice,,,,,,,,,,,,,,,,");
                    
                    // Divine Beast Example
                    writer.WriteLine("9,white_tiger,Bạch Hổ Thần Thú,Thần thú phương Tây chúa tể của Kim tăng sức mạnh cho các thẻ Kim,SupportCard,DivineBeast,Epic,3,6,4,12,5,,,,,,Tăng sức tấn công của tất cả thẻ Kim,StatBuff,attack,2,0,3,,Persistent,Luôn kích hoạt khi có thẻ Kim trên sân,ElementType,Metal,0,0,0");
                    
                    // Monster Example
                    writer.WriteLine("11,nine_tailed_fox,Cửu Vĩ Hồ,Hồ ly chín đuôi với sức mạnh mê hoặc có thể khiến đối thủ tấn công đồng đội,SupportCard,Monster,Epic,3,5,3,10,6,,,,,,30% cơ hội làm đối thủ đánh nhầm vào đồng đội,Charm,chance,0.3,0,2,,Reactive,Khi bị tấn công trực tiếp,None,,,,,5");
                    
                    // Spirit Animal Example
                    writer.WriteLine("12,rat_swift,Tý - Thần Tốc,Chuột nhanh nhẹn tăng tốc độ và khả năng né tránh,SupportCard,SpiritAnimal,Rare,2,2,1,4,7,,,,,,Tăng tốc độ và né tránh mỗi khi chơi 2 lá bài,StatBuff,speed,2,0,2,Rat,Recurring,Mỗi khi chơi 2 lá bài,Threshold,cards_played,2,0,1");
                    
                    // Joker Example
                    writer.WriteLine("13,heaven_earth_reversal,Càn Khôn Đảo Chuyển,Đảo ngược tất cả hiệu ứng tương sinh thành tương khắc và ngược lại,SupportCard,Joker,Legendary,4,0,0,8,0,,,,,,Đảo ngược tất cả hiệu ứng tương sinh thành tương khắc và ngược lại trong 3 lượt,ElementReversal,all,1,0,3,,Transformative,Kích hoạt khi có ít nhất 3 nguyên tố khác nhau trong tay,ElementCount,different,3,0,5");
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
                if (values.Count < 7) // We need at least basic info
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
                card.SupportCardType = ParseEnum<SupportCardType>(values[5]);
                card.Rarity = ParseEnum<Rarity>(values[6]);
                card.Cost = ParseInt(values[7]);
                card.Attack = ParseInt(values[8]);
                card.Defense = ParseInt(values[9]);
                card.Health = ParseInt(values[10]);
                card.Speed = ParseInt(values[11]);
                
                
                // Only parse ElementType for ElementalCard
                if (card.CardType == CardType.ElementalCard)
                {
                    if (values.Count > 12) card.NapAmIndex = ParseInt(values[12]);
                    if (values.Count > 13) card.MetalNapAm = ParseEnum<MetalNapAm>(values[13]);
                    if (values.Count > 14) card.WoodNapAm = ParseEnum<WoodNapAm>(values[14]);
                    if (values.Count > 15) card.WaterNapAm = ParseEnum<WaterNapAm>(values[15]);
                    if (values.Count > 16) card.FireNapAm = ParseEnum<FireNapAm>(values[16]);
                    if (values.Count > 17) card.EarthNapAm = ParseEnum<EarthNapAm>(values[17]);
                }
                
                // Parse support card fields (only used for SupportCard type)
                if (card.CardType == CardType.SupportCard)
                {
                    if (values.Count > 18) card.EffectDescription = values[18];
                    if (values.Count > 19) card.EffectType = ParseEnum<EffectType>(values[19]);
                    if (values.Count > 20) card.EffectParameter = values[20];
                    if (values.Count > 21) card.EffectValue = ParseFloat(values[21]);
                    if (values.Count > 22) card.EffectValue2 = ParseFloat(values[22]);
                    if (values.Count > 23) card.EffectDuration = ParseInt(values[23]);
                    if (values.Count > 24) card.ZodiacAnimal = values[24];
                    if (values.Count > 25) card.ActivationType = ParseEnum<ActivationType>(values[25]);
                    if (values.Count > 26) card.ActivationConditionDescription = values[26];
                    if (values.Count > 27) card.ConditionType = ParseEnum<ActivationConditionType>(values[27]);
                    if (values.Count > 28) card.ConditionParameter = values[28];
                    if (values.Count > 29) card.ConditionValue = ParseFloat(values[29]);
                    if (values.Count > 30) card.ConditionValue2 = ParseFloat(values[30]);
                    if (values.Count > 31) card.CooldownTime = ParseInt(values[31]);
                }
                
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
        private string GetNapAmNameForImport(CardImportData cardData)
        {
            if (cardData.CardType != CardType.ElementalCard)
                return "N/A";
                
            switch (cardData.ElementType)
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
                    _supportCardType = cardData.SupportCardType;
                    _cardRarity = cardData.Rarity;
                    _cardCost = cardData.Cost;
                    _cardAttack = cardData.Attack;
                    _cardDefense = cardData.Defense;
                    _cardHealth = cardData.Health;
                    _cardSpeed = cardData.Speed;
                    _napAmIndex = cardData.NapAmIndex;
                    
                    if (cardData.CardType == CardType.ElementalCard)
                    {
                        _cardElement = cardData.ElementType;
                        
                    }
                    
                    // Copy support card fields
                    if (cardData.CardType == CardType.SupportCard)
                    {
                        _effectDescription = cardData.EffectDescription;
                        _effectType = cardData.EffectType;
                        _effectParameter = cardData.EffectParameter;
                        _effectValue = cardData.EffectValue;
                        _effectValue2 = cardData.EffectValue2;
                        _effectDuration = cardData.EffectDuration;
                        _zodiacAnimal = cardData.ZodiacAnimal;
                        _activationType = cardData.ActivationType;
                        _activationConditionDescription = cardData.ActivationConditionDescription;
                        _conditionType = cardData.ConditionType;
                        _conditionParameter = cardData.ConditionParameter;
                        _conditionValue = cardData.ConditionValue;
                        _conditionValue2 = cardData.ConditionValue2;
                        _cooldownTime = cardData.CooldownTime;
                    }
                    
                    // Create card based on type
                    ScriptableObject card = null;
                    switch (cardData.CardType)
                    {
                        case CardType.ElementalCard:
                            card = CreateElementalCard();
                            break;
                        case CardType.SupportCard:
                            card = CreateSupportCard();
                            break;
                    }
                    
                    if (card != null)
                    {
                        // Get the output folder for this card type
                        string outputFolder = GetOutputFolderForType(cardData.CardType, cardData.SupportCardType);
                        
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
                    // Write header with new structure
                    writer.WriteLine("Id,KeyName,CardName,Description,CardType,SupportCardType,Rarity,Cost,Attack,Defense,Health,Speed,ElementType,MetalNapAm,WoodNapAm,WaterNapAm,FireNapAm,EarthNapAm,EffectDescription,EffectType,EffectParameter,EffectValue,EffectValue2,EffectDuration,ZodiacAnimal,ActivationType,ActivationConditionDescription,ConditionType,ConditionParameter,ConditionValue,ConditionValue2,CooldownTime");
                    
                    // Write card data
                    foreach (var cardObj in _generatedCards)
                    {
                        if (cardObj is ElementalCardDataSO elementalCard)
                        {
                            // Base card data
                            string line = $"{elementalCard.cardId},\"{EscapeCsvField(elementalCard.cardKeyName)}\",\"{EscapeCsvField(elementalCard.cardName)}\",\"{EscapeCsvField(elementalCard.description)}\",{elementalCard.cardType},,{elementalCard.rarity},{elementalCard.cost},{elementalCard.attack},{elementalCard.defense},{elementalCard.health},{elementalCard.speed},{elementalCard.elementType}";
                            
                            // Empty fields for other details
                            line += ",,,,,,,,,,,,,,";
                            
                            writer.WriteLine(line);
                        }
                        else if (cardObj is SupportCardDataSO supportCard)
                        {
                            string line = $"{supportCard.cardId},\"{EscapeCsvField(supportCard.cardKeyName)}\",\"{EscapeCsvField(supportCard.cardName)}\",\"{EscapeCsvField(supportCard.description)}\",{supportCard.cardType},{supportCard.supportCardType},{supportCard.rarity},{supportCard.cost},{supportCard.attack},{supportCard.defense},{supportCard.health},{supportCard.speed}";
                            
                            // Add empty fields for element and NapAm
                            line += ",,,,,";
                            
                            // Add support card specific fields
                            line += $",\"{EscapeCsvField(supportCard.effectDescription)}\",{supportCard.effectType},\"{EscapeCsvField(supportCard.effectParameter)}\",{supportCard.effectValue},{supportCard.effectValue2},{supportCard.effectDuration}";
                            
                            // Add zodiac animal (for SpiritAnimal type, empty otherwise)
                            // string zodiacAnimal = supportCard.supportCardType == SupportCardType.SpiritAnimal ? "" : ""; // Update if you add this field
                            // line += $",\"{zodiacAnimal}\"";
                            
                            // Add activation details
                            line += $",{supportCard.activationType},\"{EscapeCsvField(supportCard.activationConditionDescription)}\",{supportCard.conditionType},\"{EscapeCsvField(supportCard.conditionParameter)}\",{supportCard.conditionValue},{supportCard.conditionValue2},{supportCard.cooldownTime}";
                            
                            writer.WriteLine(line);
                        }
                        else if (cardObj is CardDataSO cardData)
                        {
                            // Handle legacy cards or basic CardDataSO
                            string line = $"{cardData.cardId},\"{EscapeCsvField(cardData.cardKeyName)}\",\"{EscapeCsvField(cardData.cardName)}\",\"{EscapeCsvField(cardData.description)}\",{cardData.cardType},,{cardData.rarity},{cardData.cost},{cardData.attack},{cardData.defense},{cardData.health},{cardData.speed}";
                            
                            // Add empty fields for the rest
                            line += ",,,,,,,,,,,,,,,,,,";
                            
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