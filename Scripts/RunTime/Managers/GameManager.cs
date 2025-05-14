using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;
using Factories;
using Systems;
using Systems.States;
using Systems.StatesMachine;
using UnityEngine;

namespace RunTime
{
    /// <summary>
    /// Main game manager that ties all the systems together and handles game flow
    /// GameManager chính quản lý tất cả hệ thống và luồng game
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        // Thành phần ECS
        // ECS components
        private EntityManager _entityManager;
        private SystemManager _systemManager;
        
        // Các hệ thống game
        // Game systems
        private CardSystem _cardSystem;
        private ElementInteractionSystem _elementInteractionSystem;
        private BattleSystem _battleSystem;
        private SupportCardSystem _supportCardSystem;
        private ScoringSystem _scoringSystem;
        
        // Factories 
        private CardFactory _cardFactory;
        private ScriptableObjectFactory _scriptableObjectFactory;
        
        // Trạng thái game
        // Game state
        private Entity _playerEntity;
        private Entity _enemyEntity;
        private bool _isBattleActive = false;
        private bool _isGameInProgress = false;
        
        // Mùa và môi trường hiện tại
        // Current season and environment
        private Season _currentSeason = Season.Spring;
        
        // Thống kê người chơi
        // Player stats
        private int _playerHealth = 50;
        private int _maxPlayerHealth = 50;
        private int _playerGold = 0;
        private int _playerEnergy = 3;
        private int _maxPlayerEnergy = 3;
        
        // Thống kê hành trình rogue-like
        // Rogue-like journey stats
        private int _currentFloor = 1;
        private int _maxFloor = 5;
        private int _encounterCount = 0;
        
        // Cờ
        // Flags
        private bool _isInitialized = false;
        
        [Header("References")]
        [SerializeField] private GameObject battleScreen;
        
        /// <summary>
        /// Initialize the manager on Awake
        /// Khởi tạo quản lý khi Awake
        /// </summary>
        protected override void OnInitialize()
        {
            InitializeGame();
        }
        
        /// <summary>
        /// Initialize the game systems
        /// This must be called before starting a game
        /// Khởi tạo các hệ thống game, phải được gọi trước khi bắt đầu game
        /// </summary>
        private void InitializeGame()
        {
            // Tránh khởi tạo trùng lặp
            // Avoid duplicate initialization
            if (_isInitialized)
            {
                Debug.Log("Game đã được khởi tạo, bỏ qua...");
                return;
            }
            
            Debug.Log("Đang khởi tạo các hệ thống game...");
            
            // Tạo thành phần ECS
            // Create ECS components
            _entityManager = new EntityManager();
            _systemManager = new SystemManager();
            
            // Khởi tạo ScriptableObjectFactory
            // Initialize ScriptableObjectFactory
            _scriptableObjectFactory = ScriptableObjectFactory.Instance;
            _scriptableObjectFactory.Initialize(_entityManager);
            
            // Tạo factories
            // Create factories
            _cardFactory = new CardFactory(_entityManager);
            
            // Tạo hệ thống game
            // Create game systems
            _cardSystem = new CardSystem(_entityManager);
            _elementInteractionSystem = new ElementInteractionSystem(_entityManager);
            _supportCardSystem = new SupportCardSystem(_entityManager);
            _scoringSystem = new ScoringSystem(_entityManager, _elementInteractionSystem);
            
            // BattleSystem phụ thuộc vào các hệ thống khác, tạo sau cùng
            // BattleSystem depends on other systems, so create it last
            _battleSystem = new BattleSystem(_entityManager, _elementInteractionSystem, 
                                          _cardSystem, _supportCardSystem, _scoringSystem);
            
            // Thêm hệ thống vào quản lý hệ thống
            // Add systems to system manager
            _systemManager.AddSystem(_cardSystem);
            _systemManager.AddSystem(_elementInteractionSystem);
            _systemManager.AddSystem(_supportCardSystem);
            _systemManager.AddSystem(_scoringSystem);
            _systemManager.AddSystem(_battleSystem);
            
            // Khởi tạo LoadCardData với EntityManager
            // Initialize LoadCardData with EntityManager
            if (LoadCardData.HasInstance && !LoadCardData.Instance.IsInitialized)
            {
                LoadCardData.Instance.Initialize(_entityManager);
            }
            
            // Đặt mùa ban đầu
            // Set initial season
            _elementInteractionSystem.SetSeason(_currentSeason);
            _scoringSystem.SetSeason(_currentSeason);
            
            _isInitialized = true;
            Debug.Log("Các hệ thống game đã được khởi tạo thành công!");
        }
        
        /// <summary>
        /// Update game state
        /// Cập nhật trạng thái game
        /// </summary>
        private void Update()
        {
            if (_isBattleActive)
            {
                // Cập nhật tất cả hệ thống
                // Update all systems
                _systemManager.UpdateAllSystems(Time.deltaTime);
                
                // Kiểm tra trận đấu đã kết thúc chưa
                // Check if battle is over
                if (_battleSystem.IsBattleOver())
                {
                    EndBattle();
                }
            }
        }
        
        /// <summary>
        /// Start a new game, initializing player and creating a deck
        /// Bắt đầu game mới, khởi tạo người chơi và tạo bộ bài
        /// </summary>
        public void StartNewGame()
        {
            Debug.Log("Đang bắt đầu game mới...");
            
            // Đảm bảo game đã được khởi tạo
            // Ensure game is initialized
            if (!_isInitialized)
            {
                InitializeGame();
            }
            
            // Tải tất cả thẻ bài sử dụng LoadCardData
            // Load all cards using LoadCardData
            if (LoadCardData.HasInstance && LoadCardData.Instance.IsInitialized)
            {
                LoadCardData.Instance.LoadAllCards();
            }
            
            // Đặt lại thống kê người chơi
            // Reset player stats
            _playerHealth = _maxPlayerHealth;
            _playerGold = 0;
            _playerEnergy = _maxPlayerEnergy;
            
            // Đặt lại thống kê hành trình
            // Reset journey stats
            _currentFloor = 1;
            _encounterCount = 0;
            
            // Tạo entity người chơi
            // Create player entity
            _playerEntity = CreatePlayerEntity();
            
            // Bắt đầu trận đấu với kích thước bộ bài khởi đầu là 30 lá
            // Start battle with a starter deck of 30 cards
            List<Entity> deck = CreateStarterDeck();
            
            if (deck != null && deck.Count > 0)
            {
                // Thêm thẻ vào bộ bài người chơi
                // Add cards to player's deck
                foreach (var card in deck)
                {
                    if (card != null)
                    {
                        _cardSystem.AddCardToDeck(card);
                    }
                }
                
                // Xáo trộn bộ bài
                // Shuffle the deck
                _cardSystem.ShuffleDeck();
                
                // Đặt game đang tiến hành
                // Set game in progress
                _isGameInProgress = true;
                
                Debug.Log("Game mới đã được bắt đầu thành công!");
                StartBattle("medium");
            }
            else
            {
                Debug.LogError("Không thể tạo bộ bài! Game không thể bắt đầu.");
            }
        }
        
        /// <summary>
        /// Create a starter deck for the player
        /// Tạo bộ bài khởi đầu cho người chơi
        /// </summary>
        private List<Entity> CreateStarterDeck()
        {
            Debug.Log("Đang tạo bộ bài khởi đầu cho game mới...");
            
            // Lấy tùy chọn của người chơi cho chủ đề bộ bài nếu có (mặc định là "balanced")
            // Get player preference for deck theme if available (default to "balanced")
            string deckTheme = "balanced";
            if (PlayerPrefs.HasKey("DeckTheme"))
            {
                deckTheme = PlayerPrefs.GetString("DeckTheme");
            }
            
            // Lấy tùy chọn của người chơi cho kích thước bộ bài nếu có (mặc định là 24)
            // Get player preference for deck size if available (default to 24)
            int deckSize = 24;
            if (PlayerPrefs.HasKey("DeckSize"))
            {
                deckSize = PlayerPrefs.GetInt("DeckSize");
            }
            
            List<Entity> deck = null;
            
            // Cố gắng tạo bộ bài theo chủ đề dựa trên tùy chọn
            // Try to create themed deck based on preference
            switch (deckTheme.ToLower())
            {
                case "metal":
                    deck = _cardFactory.CreateThemedDeck(ElementType.Metal, deckSize);
                    break;
                case "wood":
                    deck = _cardFactory.CreateThemedDeck(ElementType.Wood, deckSize);
                    break;
                case "water":
                    deck = _cardFactory.CreateThemedDeck(ElementType.Water, deckSize);
                    break;
                case "fire":
                    deck = _cardFactory.CreateThemedDeck(ElementType.Fire, deckSize);
                    break;
                case "earth":
                    deck = _cardFactory.CreateThemedDeck(ElementType.Earth, deckSize);
                    break;
                case "balanced":
                default:
                    if (LoadCardData.HasInstance && LoadCardData.Instance.IsInitialized)
                    {
                        deck = LoadCardData.Instance.CreateRandomStarterDeck("balanced", deckSize);
                    }
                    
                    // Nếu LoadCardData thất bại, sử dụng CardFactory
                    // If LoadCardData failed, use CardFactory
                    if (deck == null || deck.Count == 0)
                    {
                        deck = _cardFactory.CreateSampleDeck(deckSize);
                    }
                    break;
            }
            
            // Đảm bảo bộ bài có ít nhất một số thẻ
            // Ensure deck has at least some cards
            if (deck == null || deck.Count == 0)
            {
                Debug.LogError("Không thể tạo bất kỳ thẻ nào cho bộ bài!");
                deck = _cardFactory.CreateSampleDeck(24); // Fallback to basic deck
            }
            
            Debug.Log($"Đã tạo bộ bài với {deck.Count} thẻ cho game mới");
            return deck;
        }
        
        /// <summary>
        /// Start a battle with an enemy
        /// Bắt đầu trận đấu với kẻ địch
        /// </summary>
        public void StartBattle(string enemyType)
        {
            if (_playerEntity == null)
            {
                Debug.LogError("Không thể bắt đầu trận đấu - entity người chơi chưa được tạo!");
                return;
            }
            
            Debug.Log($"Bắt đầu trận đấu với kẻ địch: {enemyType}");
            
            // Tạo entity kẻ địch
            // Create enemy entity
            _enemyEntity = CreateEnemyEntity(enemyType);
            
            // Đặt cờ trận đấu đang diễn ra
            // Set battle active flag
            _isBattleActive = true;
            
            // Đặt lại số lần tái sử dụng bộ bài cho trận đấu mới
            // Reset deck recycle count for new battle
            _cardSystem.ResetRecycleCount();
            
            // Bắt đầu trận đấu trong hệ thống trận đấu
            // Start battle in battle system
            _battleSystem.StartBattle(_playerEntity, _enemyEntity);
            
            // Hiện màn hình trận đấu
            // Show battle screen
            if (battleScreen != null)
            {
                battleScreen.SetActive(true);
            }
            
            Debug.Log("Trận đấu đã bắt đầu thành công!");
        }
        
        /// <summary>
        /// End the current battle
        /// Kết thúc trận đấu hiện tại
        /// </summary>
        private void EndBattle()
        {
            _isBattleActive = false;
            
            // Lấy người chiến thắng
            // Get winner
            Entity winner = _battleSystem.GetWinner();
            
            if (winner == _playerEntity)
            {
                Debug.Log("Người chơi đã thắng trận đấu!");
                
                // Thêm phần thưởng
                // Add rewards
                _playerGold += 25;
                
                // Hồi một chút máu cho người chơi
                // Heal player a bit
                StatsComponent playerStats = _playerEntity.GetComponent<StatsComponent>();
                if (playerStats != null)
                {
                    playerStats.Health = Mathf.Min(playerStats.Health + 5, playerStats.MaxHealth);
                }
                
                // Tăng số lượt gặp gỡ
                // Increment encounter count
                _encounterCount++;
                
                // Tăng tầng nếu đủ số lượt gặp gỡ
                // Increase floor if enough encounters
                if (_encounterCount >= 3)
                {
                    _currentFloor++;
                    _encounterCount = 0;
                    
                    if (_currentFloor > _maxFloor)
                    {
                        // Người chơi đã hoàn thành hành trình
                        // Player has completed the journey
                        Debug.Log("Người chơi đã hoàn thành hành trình!");
                        _isGameInProgress = false;
                    }
                }
                
                // Tạo trận đấu tiếp theo nếu hành trình chưa kết thúc
                // Create next battle if journey not over
                if (_isGameInProgress)
                {
                    string enemyType = "medium";
                    
                    // Tăng mức độ khó theo tầng
                    // Increase difficulty based on floor
                    if (_currentFloor == 2) enemyType = "medium";
                    else if (_currentFloor == 3) enemyType = "strong";
                    else if (_currentFloor == 4) enemyType = "elite";
                    else if (_currentFloor >= 5) enemyType = "boss";
                    
                    // Bắt đầu trận đấu tiếp theo sau một khoảng thời gian
                    // Start next battle after a delay
                    Invoke("StartNextBattle", 2f);
                }
            }
            else
            {
                Debug.Log("Người chơi đã thua trận đấu!");
                
                // Logic game over
                // Game over logic
                _isGameInProgress = false;
                
                // Tự động khởi động lại để đơn giản
                // Automatically restart for simplicity
                Invoke("StartNewGame", 2f);
            }
        }
        
        /// <summary>
        /// Start the next battle
        /// Bắt đầu trận đấu tiếp theo
        /// </summary>
        private void StartNextBattle()
        {
            string enemyType = "medium";
            
            // Tăng mức độ khó theo tầng
            // Increase difficulty based on floor
            if (_currentFloor == 2) enemyType = "medium";
            else if (_currentFloor == 3) enemyType = "strong";
            else if (_currentFloor == 4) enemyType = "elite";
            else if (_currentFloor >= 5) enemyType = "boss";
            
            StartBattle(enemyType);
        }
        
        /// <summary>
        /// Create the player entity
        /// Tạo entity cho người chơi
        /// </summary>
        private Entity CreatePlayerEntity()
        {
            Entity player = _entityManager.CreateEntity();
            
            // Thêm thành phần thống kê
            // Add stats component
            StatsComponent stats = new StatsComponent
            {
                Attack = 5,
                Defense = 5,
                Health = _playerHealth,
                MaxHealth = _maxPlayerHealth,
                Speed = 5
            };
            player.AddComponent(stats);
            
            return player;
        }
        
        /// <summary>
        /// Create an enemy entity
        /// Tạo entity cho kẻ địch
        /// </summary>
        private Entity CreateEnemyEntity(string enemyType)
        {
            Entity enemy = _entityManager.CreateEntity();
            
            // Thêm thành phần thống kê
            // Add stats component
            StatsComponent stats = new StatsComponent();
            
            // Đặt thống kê dựa trên loại kẻ địch
            // Set stats based on enemy type
            switch (enemyType.ToLower())
            {
                case "weak":
                    stats.Attack = 3;
                    stats.Defense = 2;
                    stats.Health = 15;
                    stats.MaxHealth = 15;
                    stats.Speed = 3;
                    break;
                case "medium":
                    stats.Attack = 5;
                    stats.Defense = 4;
                    stats.Health = 25;
                    stats.MaxHealth = 25;
                    stats.Speed = 4;
                    break;
                case "strong":
                    stats.Attack = 8;
                    stats.Defense = 6;
                    stats.Health = 40;
                    stats.MaxHealth = 40;
                    stats.Speed = 5;
                    break;
                case "elite":
                    stats.Attack = 10;
                    stats.Defense = 8;
                    stats.Health = 60;
                    stats.MaxHealth = 60;
                    stats.Speed = 6;
                    break;
                case "boss":
                    stats.Attack = 12;
                    stats.Defense = 10;
                    stats.Health = 100;
                    stats.MaxHealth = 100;
                    stats.Speed = 7;
                    break;
                default:
                    stats.Attack = 4;
                    stats.Defense = 3;
                    stats.Health = 20;
                    stats.MaxHealth = 20;
                    stats.Speed = 4;
                    break;
            }
            
            enemy.AddComponent(stats);

            ElementComponent element = new ElementComponent((ElementType)Random.Range(1, 6));
            enemy.AddComponent(element);
            
            CardInfoComponent cardInfo = new CardInfoComponent
            {
                Name = $"{enemyType.ToUpper()} Enemy",
                Description = "Kẻ địch muốn đánh bại bạn.",
                Type = CardType.SupportCard,
                Rarity = Rarity.Common
            };
            enemy.AddComponent(cardInfo);
            
            return enemy;
        }
        
        /// <summary>
        /// Select a card from the player's hand
        /// Chọn một lá bài từ tay người chơi
        /// </summary>
        public void SelectCard(int cardIndex)
        {
            if (!_isBattleActive)
            {
                Debug.LogWarning("Không thể chọn thẻ - không có trận đấu đang diễn ra!");
                return;
            }
            
            // Xử lý lựa chọn thông qua máy trạng thái của BattleSystem
            // This now needs to handle selection through BattleSystem
            List<Entity> hand = _cardSystem.GetHand();
            
            if (cardIndex < 0 || cardIndex >= hand.Count)
            {
                Debug.LogError($"Chỉ số thẻ không hợp lệ: {cardIndex}");
                return;
            }
            
            Entity card = hand[cardIndex];
            
            // Sử dụng phương thức này để chọn thẻ thay vì chơi nó trực tiếp
            // Use this method to select a card rather than play it directly
            _battleSystem.SelectCard(card);
            
            Debug.Log($"Đã chọn thẻ ở chỉ số {cardIndex}");
        }
        
        /// <summary>
        /// Play a card as a support card
        /// Chơi một lá bài như thẻ hỗ trợ
        /// </summary>
        public void PlayAsSupport(int cardIndex)
        {
            if (!_isBattleActive)
            {
                Debug.LogWarning("Không thể chơi thẻ hỗ trợ - không có trận đấu đang diễn ra!");
                return;
            }
            
            List<Entity> hand = _cardSystem.GetHand();
            
            if (cardIndex < 0 || cardIndex >= hand.Count)
            {
                Debug.LogError($"Chỉ số thẻ không hợp lệ: {cardIndex}");
                return;
            }
            
            // Kiểm tra xem thẻ có thể chơi như thẻ hỗ trợ không
            // Check if the card can be played as a support
            Entity card = hand[cardIndex];
            if (card.HasComponent<SupportCardComponent>())
            {
                _cardSystem.PlayAsSupport(card);
                Debug.Log($"Đã chơi thẻ ở chỉ số {cardIndex} như thẻ hỗ trợ");
            }
            else
            {
                Debug.LogWarning("Thẻ này không thể chơi như thẻ hỗ trợ!");
            }
        }
        
        /// <summary>
        /// Play selected cards as an attack
        /// Chơi các thẻ đã chọn như một đòn tấn công
        /// </summary>
        public void PlaySelectedCards()
        {
            if (!_isBattleActive)
            {
                Debug.LogWarning("Không thể chơi thẻ - không có trận đấu đang diễn ra!");
                return;
            }
            
            // Lấy các thẻ đã chọn từ BattleSystem
            // Get selected cards from BattleSystem
            List<Entity> selectedCards = _battleSystem.GetSelectedCards();
            
            if (selectedCards.Count == 0)
            {
                Debug.LogWarning("Không có thẻ nào được chọn để chơi!");
                return;
            }
            
            // Giải quyết các thẻ đã chọn
            // Resolve selected cards
            _battleSystem.ResolveSelectedCards();
            
            Debug.Log($"Đã chơi {selectedCards.Count} thẻ cho đòn tấn công");
        }
        
        /// <summary>
        /// Discard selected cards
        /// Bỏ các thẻ đã chọn
        /// </summary>
        public void DiscardSelectedCards()
        {
            if (!_isBattleActive)
            {
                Debug.LogWarning("Không thể bỏ thẻ - không có trận đấu đang diễn ra!");
                return;
            }
            
            // Bỏ các thẻ đã chọn
            // Discard selected cards
            _battleSystem.DiscardSelectedCards();
            
            Debug.Log("Đã bỏ các thẻ đã chọn");
        }
        
        /// <summary>
        /// End the player's turn
        /// Kết thúc lượt của người chơi
        /// </summary>
        public void EndTurn()
        {
            if (!_isBattleActive)
            {
                Debug.LogWarning("Không thể kết thúc lượt - không có trận đấu đang diễn ra!");
                return;
            }
            
            // Sử dụng máy trạng thái của BattleSystem để chuyển sang trạng thái PlayerTurnEnd
            // Use BattleSystem's state machine to change to PlayerTurnEnd state
            BattleStateMachine stateMachine = _battleSystem.GetStateMachine();
            if (stateMachine != null)
            {
                stateMachine.ChangeState(BattleStateType.PlayerTurnEnd);
                Debug.Log("Đã kết thúc lượt người chơi");
            }
            else
            {
                // Dự phòng nếu máy trạng thái không khả dụng
                // Fallback if state machine is not available
                Debug.LogWarning("Máy trạng thái trận đấu không khả dụng, sử dụng phương thức trực tiếp");
                _battleSystem.EndPlayerTurn();
            }
        }
        
        /// <summary>
        /// Check if a game is in progress
        /// Kiểm tra xem game có đang tiến hành không
        /// </summary>
        public bool IsGameInProgress()
        {
            return _isGameInProgress;
        }
        
        /// <summary>
        /// Set the current season
        /// Đặt mùa hiện tại
        /// </summary>
        public void SetSeason(Season season)
        {
            _currentSeason = season;
            _elementInteractionSystem.SetSeason(season);
            _scoringSystem.SetSeason(season);
            Debug.Log($"Đã thay đổi mùa thành: {season}");
        }
        
        /// <summary>
        /// Get player energy
        /// Lấy năng lượng người chơi
        /// </summary>
        public int GetPlayerEnergy()
        {
            return _playerEnergy;
        }
        
        /// <summary>
        /// Set player energy
        /// Đặt năng lượng người chơi
        /// </summary>
        public void SetPlayerEnergy(int energy)
        {
            _playerEnergy = Mathf.Clamp(energy, 0, _maxPlayerEnergy);
        }
        
        /// <summary>
        /// Reset player energy to max
        /// Đặt lại năng lượng người chơi về tối đa
        /// </summary>
        public void ResetPlayerEnergy()
        {
            _playerEnergy = _maxPlayerEnergy;
        }
        
        /// <summary>
        /// Get current floor in the rogue-like journey
        /// Lấy tầng hiện tại trong hành trình rogue-like
        /// </summary>
        public int GetCurrentFloor()
        {
            return _currentFloor;
        }
        
        /// <summary>
        /// Get encounter count on the current floor
        /// Lấy số lượt gặp gỡ trên tầng hiện tại
        /// </summary>
        public int GetEncounterCount()
        {
            return _encounterCount;
        }
        
        /// <summary>
        /// Get player gold
        /// Lấy số vàng của người chơi
        /// </summary>
        public int GetPlayerGold()
        {
            return _playerGold;
        }
        
        // Getters cho các hệ thống và entities
        // Getters for systems and entities
        public EntityManager GetEntityManager() => _entityManager;
        public CardSystem GetCardSystem() => _cardSystem;
        public BattleSystem GetBattleSystem() => _battleSystem;
        public ElementInteractionSystem GetElementInteractionSystem() => _elementInteractionSystem;
        public SupportCardSystem GetSupportCardSystem() => _supportCardSystem;
        public ScoringSystem GetScoringSystem() => _scoringSystem;
        public Entity GetPlayerEntity() => _playerEntity;
        public Entity GetEnemyEntity() => _enemyEntity;
        public Season GetCurrentSeason() => _currentSeason;
    }
}