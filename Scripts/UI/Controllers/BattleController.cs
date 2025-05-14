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
    /// Controls the battle screen UI and interactions
    /// Điều khiển màn hình trận đấu và tương tác người dùng
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
        [SerializeField] private Button attackButton;
        [SerializeField] private Button discardButton;
        [SerializeField] private Button cancelSelectionButton;
        [SerializeField] private TextMeshProUGUI deckCountText;
        [SerializeField] private TextMeshProUGUI discardCountText;
        [SerializeField] private TextMeshProUGUI turnText;
        [SerializeField] private TextMeshProUGUI seasonText;
        [SerializeField] private TextMeshProUGUI recycleCountText;
        
        [Header("Card Selection")]
        [SerializeField] private GameObject cardSelectionPanel;
        [SerializeField] private TextMeshProUGUI selectionTitleText;
        [SerializeField] private Transform cardSelectionContainer;
        
        [Header("Battle Log")]
        [SerializeField] private TextMeshProUGUI battleLogText;
        [SerializeField] private ScrollRect battleLogScrollRect;
        
        [Header("Score Display")]
        [SerializeField] private GameObject scorePanel;
        [SerializeField] private TextMeshProUGUI baseScoreText;
        [SerializeField] private TextMeshProUGUI elementBonusText;
        [SerializeField] private TextMeshProUGUI comboPointsText;
        [SerializeField] private TextMeshProUGUI effectPointsText;
        [SerializeField] private TextMeshProUGUI environmentBonusText;
        [SerializeField] private TextMeshProUGUI totalScoreText;
        
        [Header("Season Controls")]
        [SerializeField] private Button springButton;
        [SerializeField] private Button summerButton;
        [SerializeField] private Button autumnButton;
        [SerializeField] private Button winterButton;
        
        [Header("Restart")]
        [SerializeField] private Button restartButton;
        
        // Tham chiếu nội bộ
        // Internal references
        private List<CardView> handCardViews = new List<CardView>();
        private List<CardView> playZoneCardViews = new List<CardView>();
        private int currentTurn = 1;
        
        // Trạng thái lựa chọn thẻ
        // Card selection state
        private List<Entity> selectedCards = new List<Entity>();
        private List<int> selectedCardIndices = new List<int>();
        
        // Tham chiếu game
        // Game references
        private GameManager _gameManager;
        private CardSystem _cardSystem;
        private BattleSystem _battleSystem;
        private ElementInteractionSystem _elementSystem;
        private SupportCardSystem _supportCardSystem;
        private ScoringSystem _scoringSystem;
        
        /// <summary>
        /// Initialize the controller
        /// Khởi tạo controller
        /// </summary>
        private void Start()
        {
            // Lấy tham chiếu
            // Get references
            _gameManager = GameManager.Instance;
            if (_gameManager != null)
            {
                _cardSystem = _gameManager.GetCardSystem();
                _battleSystem = _gameManager.GetBattleSystem();
                _elementSystem = _gameManager.GetElementInteractionSystem();
                _supportCardSystem = _gameManager.GetSupportCardSystem();
                _scoringSystem = _gameManager.GetScoringSystem();
            }
            
            if (supportZoneView != null)
            {
                supportZoneView.SetBattleController(this);
            }
            
            // Thêm lắng nghe sự kiện nút
            // Add button listeners
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
            attackButton.onClick.AddListener(OnAttackClicked);
            discardButton.onClick.AddListener(OnDiscardClicked);
            cancelSelectionButton.onClick.AddListener(OnCancelSelectionClicked);
            
            // Thêm lắng nghe sự kiện nút mùa
            // Add season button listeners
            springButton.onClick.AddListener(() => OnSeasonClicked(Season.Spring));
            summerButton.onClick.AddListener(() => OnSeasonClicked(Season.Summer));
            autumnButton.onClick.AddListener(() => OnSeasonClicked(Season.Autumn));
            winterButton.onClick.AddListener(() => OnSeasonClicked(Season.Winter));
            
            // Thêm lắng nghe sự kiện nút khởi động lại
            // Add restart button listener
            restartButton.onClick.AddListener(OnRestartClicked);
            
            // Ban đầu ẩn các panel
            // Initially hide panels
            cardSelectionPanel.SetActive(false);
            scorePanel.SetActive(false);
            
            // Vô hiệu hóa các nút hành động
            // Disable action buttons initially
            UpdateActionButtonStates();
            
            // Bắt đầu game mới nếu cần
            // Start a new game if needed
            if (_gameManager != null && !_gameManager.IsGameInProgress())
            {
                _gameManager.StartNewGame();
            }
            
            // Đặt lại trạng thái trận đấu
            // Reset battle state
            ResetBattleState();
            
            // Thêm mục nhật ký đầu tiên
            // Add first battle log entry
            AddBattleLog("Trận đấu bắt đầu! Chọn tối đa 3 lá bài để tấn công hoặc bỏ.");
        }
        
        /// <summary>
        /// Reset battle state
        /// Đặt lại trạng thái trận đấu
        /// </summary>
        private void ResetBattleState()
        {
            currentTurn = 1;
            selectedCards.Clear();
            selectedCardIndices.Clear();
            battleLogText.text = "";
            
            UpdateActionButtonStates();
        }
        
        /// <summary>
        /// Update UI each frame
        /// Cập nhật UI mỗi khung hình
        /// </summary>
        private void Update()
        {
            UpdateUI();
        }
        
        /// <summary>
        /// Update all UI elements
        /// Cập nhật tất cả các phần tử UI
        /// </summary>
        private void UpdateUI()
        {
            if (_gameManager == null || _cardSystem == null) return;
            
            // Cập nhật thống kê người chơi
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
            
            // Cập nhật năng lượng người chơi
            // Update player energy
            int playerEnergy = _gameManager.GetPlayerEnergy();
            int maxPlayerEnergy = _battleSystem.GetMaxPlayerEnergy();
            playerEnergyText.text = $"Năng lượng: {playerEnergy} / {maxPlayerEnergy}";
            
            // Cập nhật thống kê kẻ địch
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
                
                // Cập nhật tên kẻ địch
                // Update enemy name
                CardInfoComponent enemyInfo = enemyEntity.GetComponent<CardInfoComponent>();
                if (enemyInfo != null)
                {
                    enemyNameText.text = enemyInfo.Name;
                }
                
                // Cập nhật màu nguyên tố kẻ địch
                // Update enemy element color
                ElementComponent enemyElement = enemyEntity.GetComponent<ElementComponent>();
                if (enemyElement != null)
                {
                    // Lấy màu dựa trên nguyên tố
                    // Get color based on element
                    enemyImage.color = GetElementColor(enemyElement.Element);
                }
            }
            
            // Cập nhật ý định kẻ địch
            // Update enemy intent
            string intentType = _battleSystem.GetEnemyIntentType();
            int intentValue = _battleSystem.GetEnemyIntentValue();
            
            string intentText = "Ý định: ";
            switch (intentType)
            {
                case "Attack":
                    intentText += $"Tấn công {intentValue}";
                    break;
                case "Defend":
                    intentText += $"Phòng thủ +{intentValue}";
                    break;
                case "Buff":
                    intentText += $"Tăng sức tấn công +{intentValue}";
                    break;
                case "Heal":
                    intentText += $"Hồi máu {intentValue}";
                    break;
                default:
                    intentText += "Không rõ";
                    break;
            }
            
            enemyIntentText.text = intentText;
            
            // Cập nhật thẻ trên tay
            // Update hand cards
            UpdateHandCards(_cardSystem.GetHand());
            
            // Cập nhật khu vực chơi
            // Update play zone
            UpdatePlayZone(_cardSystem.GetPlayZone());
            
            // Cập nhật khu vực hỗ trợ
            // Update support zone
            if (supportZoneView != null)
            {
                supportZoneView.SetSupportCards(_cardSystem.GetSupportZone());
                supportZoneView.UpdateSupportCardStatus();
            }
            
            // Cập nhật số lượng bộ bài và chồng bỏ
            // Update deck and discard counts
            deckCountText.text = $"Bộ bài: {_cardSystem.GetDeck().Count}";
            discardCountText.text = $"Chồng bỏ: {_cardSystem.GetDiscardPile().Count}";
            
            // Cập nhật lượt và mùa
            // Update turn and season
            turnText.text = $"Lượt: {currentTurn}";
            seasonText.text = $"Mùa: {_gameManager.GetCurrentSeason()}";
            
            // Cập nhật số lần tái sử dụng
            // Update recycle count
            int recycleCount = _cardSystem.GetRecycleCount();
            recycleCountText.text = recycleCount > 0 ? $"Tái sử dụng: {recycleCount}" : "";
            
            // Vô hiệu hóa nút kết thúc lượt nếu trận đấu kết thúc
            // Disable end turn button if battle is over
            bool battleOver = _battleSystem.IsBattleOver();
            endTurnButton.interactable = !battleOver;
            
            // Hiện nút khởi động lại nếu trận đấu kết thúc
            // Show restart button if battle is over
            restartButton.gameObject.SetActive(battleOver);
            
            // Cập nhật trạng thái nút hành động
            // Update action button states
            UpdateActionButtonStates();
        }
        
        /// <summary>
        /// Update action button states based on selection
        /// Cập nhật trạng thái nút hành động dựa trên lựa chọn
        /// </summary>
        private void UpdateActionButtonStates()
        {
            bool hasSelection = selectedCards.Count > 0;
            
            attackButton.interactable = hasSelection;
            discardButton.interactable = hasSelection;
            cancelSelectionButton.interactable = hasSelection;
            
            // Vô hiệu hóa tất cả các nút nếu trận đấu kết thúc
            // Disable all buttons if battle is over
            bool battleOver = _battleSystem != null && _battleSystem.IsBattleOver();
            if (battleOver)
            {
                attackButton.interactable = false;
                discardButton.interactable = false;
                cancelSelectionButton.interactable = false;
                endTurnButton.interactable = false;
            }
        }
        
        /// <summary>
        /// Get color based on element type
        /// Lấy màu dựa trên loại nguyên tố
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
        /// Cập nhật hiển thị thẻ trên tay
        /// </summary>
        private void UpdateHandCards(List<Entity> handCards)
        {
            // Xóa thẻ hiện có nếu số lượng không khớp
            // Clear existing cards if count mismatch
            if (handCardViews.Count != handCards.Count)
            {
                ClearContainer(handContainer);
                handCardViews.Clear();
                
                // Tạo CardView mới
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
                // Cập nhật CardView hiện có
                // Update existing card views
                for (int i = 0; i < handCards.Count; i++)
                {
                    handCardViews[i].SetCard(handCards[i], i);
                    
                    // Cập nhật màu nền của thẻ nếu đã được chọn
                    // Update card background color if selected
                    Image cardBg = handCardViews[i].GetComponent<Image>();
                    if (cardBg != null)
                    {
                        if (selectedCardIndices.Contains(i))
                        {
                            cardBg.color = new Color(1f, 0.8f, 0.8f); // Màu đỏ nhạt - Light red
                        }
                        else
                        {
                            cardBg.color = Color.white;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Update play zone display
        /// Cập nhật hiển thị khu vực chơi
        /// </summary>
        private void UpdatePlayZone(List<Entity> playZoneCards)
        {
            // Xóa thẻ hiện có nếu số lượng không khớp
            // Clear existing cards if count mismatch
            if (playZoneCardViews.Count != playZoneCards.Count)
            {
                ClearContainer(playZoneContainer);
                playZoneCardViews.Clear();
                
                // Tạo CardView mới
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
                // Cập nhật CardView hiện có
                // Update existing card views
                for (int i = 0; i < playZoneCards.Count; i++)
                {
                    playZoneCardViews[i].SetCard(playZoneCards[i]);
                }
            }
        }
        
        /// <summary>
        /// Clear all child objects from a container
        /// Xóa tất cả đối tượng con từ một container
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
        /// Xử lý sự kiện nhấp vào thẻ trên tay
        /// </summary>
        private void OnHandCardClicked(int cardIndex)
        {
            if (_cardSystem == null) return;
            
            var hand = _cardSystem.GetHand();
            if (cardIndex < 0 || cardIndex >= hand.Count) return;
            
            Entity card = hand[cardIndex];
            
            // Kiểm tra số lượng thẻ tối đa có thể chọn
            // Check maximum number of cards that can be selected
            int maxCardsPerTurn = _cardSystem.GetMaxCardsPerTurn();
            
            // Lấy chi phí thẻ
            // Get card cost
            int cardCost = GetCardCost(card);
            
            // Kiểm tra xem đã có đủ năng lượng không
            // Check if we have enough energy
            int playerEnergy = _gameManager.GetPlayerEnergy();
            
            // Kiểm tra xem thẻ đã được chọn chưa
            // Check if card is already selected
            int existingIndex = selectedCardIndices.IndexOf(cardIndex);
            if (existingIndex >= 0)
            {
                // Bỏ chọn thẻ
                // Deselect card
                selectedCards.RemoveAt(existingIndex);
                selectedCardIndices.RemoveAt(existingIndex);
                
                AddBattleLog($"Bỏ chọn lá bài {GetCardName(card)}.");
                UpdateActionButtonStates();
                return;
            }
            
            // Kiểm tra xem số lượng thẻ đã chọn có đạt tối đa chưa
            // Check if maximum card selection has been reached
            if (selectedCards.Count >= maxCardsPerTurn)
            {
                AddBattleLog($"Đã chọn tối đa {maxCardsPerTurn} lá bài. Bỏ chọn một lá để chọn lá khác.");
                return;
            }
            
            // Tính tổng chi phí của các thẻ đã chọn
            // Calculate total cost of selected cards
            int totalCost = 0;
            foreach (var selectedCard in selectedCards)
            {
                totalCost += GetCardCost(selectedCard);
            }
            
            // Kiểm tra xem có đủ năng lượng để chơi tất cả thẻ đã chọn không
            // Check if enough energy to play all selected cards
            if (playerEnergy < totalCost + cardCost)
            {
                AddBattleLog("Không đủ năng lượng để chọn lá bài này!");
                return;
            }
            
            // Thêm thẻ vào danh sách đã chọn
            // Add card to selected list
            selectedCards.Add(card);
            selectedCardIndices.Add(cardIndex);
            
            // Thông báo thẻ đã được chọn
            // Notify that card has been selected
            AddBattleLog($"Đã chọn lá bài {GetCardName(card)}.");
            
            // Cập nhật trạng thái nút
            // Update button states
            UpdateActionButtonStates();
        }
        
        /// <summary>
        /// Handle attack button click
        /// Xử lý nhấp vào nút tấn công
        /// </summary>
        private void OnAttackClicked()
        {
            if (_battleSystem == null || selectedCards.Count == 0) return;
            
            // Kiểm tra thẻ hỗ trợ
            // Check for support cards
            for (int i = selectedCards.Count - 1; i >= 0; i--)
            {
                Entity card = selectedCards[i];
                if (card.HasComponent<SupportCardComponent>())
                {
                    // Chuyển thẻ hỗ trợ sang khu vực hỗ trợ
                    // Transfer support cards to support zone
                    _cardSystem.PlayAsSupport(card);
                    AddBattleLog($"Đã chơi {GetCardName(card)} như thẻ hỗ trợ.");
                    
                    // Xóa khỏi danh sách đã chọn
                    // Remove from selected list
                    selectedCards.RemoveAt(i);
                    selectedCardIndices.RemoveAt(i);
                }
            }
            
            // Nếu còn thẻ tấn công, xử lý tấn công
            // If there are attack cards, process attack
            if (selectedCards.Count > 0)
            {
                // Lấy chi tiết tính điểm trước khi tấn công
                // Get score details before attacking
                Dictionary<string, float> scoreDetails = _scoringSystem.GetScoreDetails(selectedCards, _gameManager.GetEnemyEntity());
                
                // Hiển thị bảng điểm
                // Display score panel
                DisplayScorePanel(scoreDetails);
                
                // Chuyển tất cả thẻ đã chọn sang khu vực chơi
                // Transfer all selected cards to play zone
                foreach (Entity card in selectedCards)
                {
                    _cardSystem.PlayCard(card);
                    AddBattleLog($"Đã chơi {GetCardName(card)} để tấn công.");
                    
                    // Giảm năng lượng
                    // Deduct energy
                    int cardCost = GetCardCost(card);
                    _gameManager.SetPlayerEnergy(_gameManager.GetPlayerEnergy() - cardCost);
                }
                
                // Tính điểm tấn công
                // Calculate attack score
                int damage = _scoringSystem.CalculateScore(selectedCards, _gameManager.GetEnemyEntity());
                
                // Gây sát thương cho kẻ địch
                // Apply damage to enemy
                Entity enemy = _gameManager.GetEnemyEntity();
                if (enemy != null)
                {
                    StatsComponent enemyStats = enemy.GetComponent<StatsComponent>();
                    if (enemyStats != null)
                    {
                        enemyStats.Health -= damage;
                        if (enemyStats.Health < 0) enemyStats.Health = 0;
                        AddBattleLog($"Gây {damage} sát thương cho kẻ địch!");
                        
                        // Kiểm tra đòn chí mạng
                        // Check for critical hit
                        if (_scoringSystem.IsCriticalHit(selectedCards, enemy))
                        {
                            int critDamage = damage / 2; // 50% sát thương phụ
                            enemyStats.Health -= critDamage;
                            if (enemyStats.Health < 0) enemyStats.Health = 0;
                            AddBattleLog($"Đòn chí mạng! Thêm {critDamage} sát thương!");
                        }
                    }
                }
                
                // Kiểm tra kết thúc trận đấu
                // Check for battle end
                if (_battleSystem.IsBattleOver())
                {
                    Entity winner = _battleSystem.GetWinner();
                    
                    if (winner == _gameManager.GetPlayerEntity())
                    {
                        AddBattleLog("Chiến thắng! Bạn đã đánh bại kẻ địch.");
                    }
                    else
                    {
                        AddBattleLog("Thất bại! Bạn đã bị đánh bại.");
                    }
                }
            }
            
            // Đặt lại danh sách đã chọn
            // Reset selected list
            selectedCards.Clear();
            selectedCardIndices.Clear();
            
            // Cập nhật trạng thái nút
            // Update button states
            UpdateActionButtonStates();
        }
        
        /// <summary>
        /// Handle discard button click
        /// Xử lý nhấp vào nút bỏ bài
        /// </summary>
        private void OnDiscardClicked()
        {
            if (_cardSystem == null || selectedCards.Count == 0) return;
            
            // Bỏ tất cả thẻ đã chọn
            // Discard all selected cards
            foreach (Entity card in selectedCards)
            {
                _cardSystem.DiscardCard(card);
                AddBattleLog($"Đã bỏ lá bài {GetCardName(card)}.");
                
                // Giảm năng lượng (tùy chọn - có thể bỏ qua chi phí năng lượng khi bỏ bài)
                // Deduct energy (optional - can skip energy cost when discarding)
                // int cardCost = GetCardCost(card);
                // _gameManager.SetPlayerEnergy(_gameManager.GetPlayerEnergy() - cardCost);
            }
            
            // Đặt lại danh sách đã chọn
            // Reset selected list
            selectedCards.Clear();
            selectedCardIndices.Clear();
            
            // Cập nhật trạng thái nút
            // Update button states
            UpdateActionButtonStates();
        }
        
        /// <summary>
        /// Display score panel with calculation details
        /// Hiển thị bảng điểm với chi tiết tính toán
        /// </summary>
        private void DisplayScorePanel(Dictionary<string, float> scoreDetails)
        {
            // Hiện bảng điểm
            // Show score panel
            scorePanel.SetActive(true);
            
            // Đặt các giá trị
            // Set values
            baseScoreText.text = $"Điểm cơ bản: {scoreDetails["BaseScore"]}";
            elementBonusText.text = $"Hệ số nguyên tố: x{scoreDetails["ElementModifier"]:F2}";
            comboPointsText.text = $"Điểm combo: +{scoreDetails["NapAmCombo"]}";
            effectPointsText.text = $"Điểm hiệu ứng: +{scoreDetails["EffectPoints"]}";
            environmentBonusText.text = $"Hệ số môi trường: x{scoreDetails["EnvironmentModifier"]:F2}";
            
            // Tính tổng điểm
            // Calculate total score
            float totalScore = scoreDetails["BaseScore"] * scoreDetails["ElementModifier"] + 
                             scoreDetails["NapAmCombo"] + 
                             scoreDetails["EffectPoints"];
            totalScore *= scoreDetails["EnvironmentModifier"] * scoreDetails["RarityModifier"];
            totalScore *= (1 - scoreDetails["DamageReduction"]);
            
            totalScoreText.text = $"Tổng điểm: {Mathf.RoundToInt(totalScore)}";
            
            // Ẩn bảng điểm sau 3 giây
            // Hide score panel after 3 seconds
            Invoke("HideScorePanel", 3f);
        }
        
        /// <summary>
        /// Hide score panel
        /// Ẩn bảng điểm
        /// </summary>
        private void HideScorePanel()
        {
            scorePanel.SetActive(false);
        }
        
        /// <summary>
        /// Handle cancel selection clicked
        /// Xử lý nhấp vào nút hủy lựa chọn
        /// </summary>
        private void OnCancelSelectionClicked()
        {
            selectedCards.Clear();
            selectedCardIndices.Clear();
            AddBattleLog("Đã hủy lựa chọn thẻ.");
            
            // Cập nhật trạng thái nút
            // Update button states
            UpdateActionButtonStates();
        }
        
        /// <summary>
        /// Get card cost
        /// Lấy chi phí thẻ
        /// </summary>
        private int GetCardCost(Entity card)
        {
            CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
            return cardInfo != null ? cardInfo.Cost : 1;
        }
        
        /// <summary>
        /// Get card name
        /// Lấy tên thẻ
        /// </summary>
        private string GetCardName(Entity card)
        {
            CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
            return cardInfo != null ? cardInfo.Name : "Không rõ";
        }
        
        /// <summary>
        /// Handle end turn button click
        /// Xử lý nhấp vào nút kết thúc lượt
        /// </summary>
        private void OnEndTurnClicked()
        {
            if (_battleSystem == null) return;
            
            // Kết thúc lượt trong game manager
            // End turn in game manager
            _gameManager.EndTurn();
            
            // Cập nhật bộ đếm lượt
            // Update turn counter
            currentTurn++;
            
            // Đặt lại lựa chọn thẻ
            // Reset card selection
            selectedCards.Clear();
            selectedCardIndices.Clear();
            
            // Kẻ địch thực hiện hành động trong máy trạng thái
            // Enemy performs action in state machine
            AddBattleLog($"Lượt {currentTurn} bắt đầu.");
            
            // Cập nhật trạng thái nút
            // Update button states
            UpdateActionButtonStates();
        }
        
        /// <summary>
        /// Handle season button click
        /// Xử lý nhấp vào nút mùa
        /// </summary>
        private void OnSeasonClicked(Season season)
        {
            if (_gameManager == null) return;
            
            _gameManager.SetSeason(season);
            AddBattleLog($"Đã thay đổi mùa thành {GetSeasonName(season)}.");
        }
        
        /// <summary>
        /// Get season name in Vietnamese
        /// Lấy tên mùa bằng tiếng Việt
        /// </summary>
        private string GetSeasonName(Season season)
        {
            switch (season)
            {
                case Season.Spring:
                    return "Mùa Xuân";
                case Season.Summer:
                    return "Mùa Hạ";
                case Season.Autumn:
                    return "Mùa Thu";
                case Season.Winter:
                    return "Mùa Đông";
                default:
                    return season.ToString();
            }
        }
        
        /// <summary>
        /// Handle restart button click
        /// Xử lý nhấp vào nút khởi động lại
        /// </summary>
        private void OnRestartClicked()
        {
            if (_gameManager == null) return;
            
            // Đặt lại trạng thái trận đấu
            // Reset battle state
            ResetBattleState();
            
            // Bắt đầu game mới
            // Start a new game
            _gameManager.StartNewGame();
            
            // Thêm mục nhật ký đầu tiên
            // Add first battle log entry
            AddBattleLog("Đã bắt đầu trận đấu mới!");
        }
        
        /// <summary>
        /// Add entry to battle log
        /// Thêm mục vào nhật ký trận đấu
        /// </summary>
        private void AddBattleLog(string message)
        {
            battleLogText.text += $"\n{message}";
            
            // Cuộn xuống cuối
            // Scroll to bottom
            Canvas.ForceUpdateCanvases();
            battleLogScrollRect.verticalNormalizedPosition = 0f;
        }
        
        /// <summary>
        /// Add a method for BattleSystem to call
        /// Thêm phương thức để BattleSystem gọi
        /// </summary>
        public void OnBattleEvent(string eventType, string message)
        {
            AddBattleLog(message);
            
            // Cập nhật UI ngay lập tức
            // Update the UI immediately
            UpdateUI();
        }
    }
}