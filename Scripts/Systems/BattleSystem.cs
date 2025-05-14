using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;
using Systems.StatesMachine;
using UnityEngine;

namespace Systems
{
    /// <summary>
    /// System that handles battle logic with state machine integration
    /// Hệ thống xử lý logic trận đấu với tích hợp máy trạng thái
    /// </summary>
    public class BattleSystem : Core.System
    {
        // Tham chiếu đến các hệ thống khác
        // References to other systems
        private ElementInteractionSystem _elementInteractionSystem;
        private CardSystem _cardSystem;
        private SupportCardSystem _supportCardSystem;
        private ScoringSystem _scoringSystem;
        
        // Máy trạng thái
        // State machine
        private BattleStateMachine _stateMachine;
        
        // Trạng thái trận đấu
        // Battle state
        private Entity _playerEntity;
        private Entity _enemyEntity;
        private List<Entity> _selectedCards = new List<Entity>();
        private List<Entity> _playedCards = new List<Entity>();
        
        // Thống kê trận đấu
        // Battle stats
        private int _currentTurn = 1;
        private int _playerEnergy = 3;
        private int _maxPlayerEnergy = 3;
        
        // Thông báo ý định của kẻ địch
        // Enemy intent
        private string _enemyIntentType = "";
        private int _enemyIntentValue = 0;
        
        // Constructor
        public BattleSystem(EntityManager entityManager, ElementInteractionSystem elementInteractionSystem, 
                            CardSystem cardSystem, SupportCardSystem supportCardSystem, 
                            ScoringSystem scoringSystem) : base(entityManager)
        {
            _elementInteractionSystem = elementInteractionSystem;
            _cardSystem = cardSystem;
            _supportCardSystem = supportCardSystem;
            _scoringSystem = scoringSystem;
            
            // Tạo máy trạng thái
            // Create the state machine
            _stateMachine = new BattleStateMachine(this);
        }
        
        /// <summary>
        /// Initialize a battle
        /// Khởi tạo trận đấu
        /// </summary>
        public void InitializeBattle()
        {
            Debug.Log("Initializing battle...");
            
            // Đặt lại trạng thái trận đấu
            // Reset battle state
            _currentTurn = 1;
            _playerEnergy = _maxPlayerEnergy;
            _selectedCards.Clear();
            _playedCards.Clear();
            
            // Đặt lại số lần tái sử dụng bộ bài
            // Reset deck recycle count
            _cardSystem.ResetRecycleCount();
            
            // Rút bài ban đầu
            // Draw initial hand
            _cardSystem.FillHand();
            
            Debug.Log("Battle initialized!");
        }
        
        /// <summary>
        /// Update method - called every frame
        /// </summary>
        public override void Update(float deltaTime)
        {
            // Cập nhật máy trạng thái
            // Update the state machine
            _stateMachine.Update();
        }
        
        /// <summary>
        /// Start the battle
        /// Bắt đầu trận đấu
        /// </summary>
        public void StartBattle(Entity player, Entity enemy)
        {
            _playerEntity = player;
            _enemyEntity = enemy;
            
            // Khởi tạo ý định của kẻ địch
            // Initialize enemy intent
            GenerateEnemyIntent();
            
            // Bắt đầu máy trạng thái
            // Start the state machine
            _stateMachine.Start();
        }
        
        /// <summary>
        /// Start a new player turn
        /// Bắt đầu lượt mới của người chơi
        /// </summary>
        public void StartPlayerTurn()
        {
            Debug.Log($"Starting player turn {_currentTurn}");
            
            // Đặt lại năng lượng người chơi
            // Reset player energy
            _playerEnergy = _maxPlayerEnergy;
            
            // Điền đầy tay bài (lên tới 8 lá)
            // Fill hand up to 8 cards
            _cardSystem.FillHand();
            
            // Đặt lại thẻ đã chọn
            // Reset selected cards
            _selectedCards.Clear();
            
            // Đặt lại thẻ đã chơi
            // Clear played cards for this turn
            _playedCards.Clear();
            
            // Cập nhật cooldown của thẻ hỗ trợ
            // Update support card cooldowns
            List<Entity> supportZone = _cardSystem.GetSupportZone();
            foreach (var card in supportZone)
            {
                SupportCardComponent supportComponent = card.GetComponent<SupportCardComponent>();
                if (supportComponent != null && supportComponent.CurrentCooldown > 0)
                {
                    supportComponent.CurrentCooldown--;
                }
            }
        }
        
        /// <summary>
        /// End the player's turn
        /// Kết thúc lượt của người chơi
        /// </summary>
        public void EndPlayerTurn()
        {
            // Đưa thẻ trong khu vực chơi vào chồng bỏ
            // Discard cards in play zone
            _cardSystem.DiscardPlayZone();
            
            // Đặt lại danh sách thẻ đã chơi
            // Reset played cards list
            _playedCards.Clear();
        }
        
        /// <summary>
        /// Start the enemy's turn
        /// Bắt đầu lượt của kẻ địch
        /// </summary>
        public void StartEnemyTurn()
        {
            Debug.Log("Starting enemy turn");
            
            // Logic khởi tạo lượt kẻ địch
            // Enemy turn initialization logic
            
            // Tạo ý định mới cho kẻ địch
            // Generate new enemy intent for next turn
            GenerateEnemyIntent();
        }
        
        /// <summary>
        /// Enemy performs its action
        /// Kẻ địch thực hiện hành động của nó
        /// </summary>
        public void PerformEnemyAction()
        {
            Debug.Log("Enemy performing action");
            
            if (_playerEntity == null || _enemyEntity == null)
                return;
            
            StatsComponent playerStats = _playerEntity.GetComponent<StatsComponent>();
            StatsComponent enemyStats = _enemyEntity.GetComponent<StatsComponent>();
            
            if (playerStats == null || enemyStats == null)
                return;
            
            // Thực hiện hành động dựa trên ý định
            // Perform action based on intent
            switch (_enemyIntentType)
            {
                case "Attack":
                    // Tấn công
                    // Attack
                    float damage = _enemyIntentValue;
                    
                    // Áp dụng phòng thủ
                    // Apply defense
                    float damageReduction = playerStats.Defense / 100f;
                    damage = Mathf.Max(0, damage * (1 - damageReduction));
                    
                    // Gây sát thương
                    // Apply damage
                    playerStats.Health -= (int)damage;
                    if (playerStats.Health < 0)
                        playerStats.Health = 0;
                    
                    Debug.Log($"Enemy attacks for {(int)damage} damage!");
                    break;
                    
                case "Defend":
                    // Phòng thủ
                    // Defend
                    enemyStats.Defense += _enemyIntentValue;
                    
                    Debug.Log($"Enemy increases its defense by {_enemyIntentValue}!");
                    break;
                    
                case "Buff":
                    // Tăng cường
                    // Buff
                    enemyStats.Attack += _enemyIntentValue;
                    
                    Debug.Log($"Enemy increases its attack by {_enemyIntentValue}!");
                    break;
                    
                case "Heal":
                    // Hồi máu
                    // Heal
                    enemyStats.Health = Mathf.Min(enemyStats.Health + _enemyIntentValue, enemyStats.MaxHealth);
                    
                    Debug.Log($"Enemy heals for {_enemyIntentValue} health!");
                    break;
                    
                default:
                    // Tấn công mặc định
                    // Default attack
                    float defaultDamage = enemyStats.Attack;
                    
                    // Áp dụng phòng thủ
                    // Apply defense
                    float defaultDamageReduction = playerStats.Defense / 100f;
                    defaultDamage = Mathf.Max(0, defaultDamage * (1 - defaultDamageReduction));
                    
                    // Gây sát thương
                    // Apply damage
                    playerStats.Health -= (int)defaultDamage;
                    if (playerStats.Health < 0)
                        playerStats.Health = 0;
                    
                    Debug.Log($"Enemy attacks for {(int)defaultDamage} damage!");
                    break;
            }
        }
        
        /// <summary>
        /// Generate enemy intent for next turn
        /// Tạo ý định của kẻ địch cho lượt tiếp theo
        /// </summary>
        private void GenerateEnemyIntent()
        {
            if (_enemyEntity == null)
                return;
                
            StatsComponent enemyStats = _enemyEntity.GetComponent<StatsComponent>();
            if (enemyStats == null)
                return;
                
            // Lựa chọn ngẫu nhiên ý định dựa trên tình trạng
            // Randomly choose intent based on state
            float healthPercent = (float)enemyStats.Health / enemyStats.MaxHealth;
            
            // Danh sách ý định có thể
            // List of possible intents
            List<string> possibleIntents = new List<string>();
            
            // Luôn có thể tấn công
            // Always can attack
            possibleIntents.Add("Attack");
            
            // Thêm phòng thủ nếu máu thấp
            // Add defend if low health
            if (healthPercent < 0.5f)
            {
                possibleIntents.Add("Defend");
                possibleIntents.Add("Defend");  // Thêm 2 lần để tăng xác suất - Add twice to increase probability
            }
            
            // Thêm hồi máu nếu máu rất thấp
            // Add heal if very low health
            if (healthPercent < 0.3f)
            {
                possibleIntents.Add("Heal");
                possibleIntents.Add("Heal");  // Thêm 2 lần để tăng xác suất - Add twice to increase probability
            }
            
            // Thêm tăng cường nếu máu cao
            // Add buff if high health
            if (healthPercent > 0.7f)
            {
                possibleIntents.Add("Buff");
            }
            
            // Chọn ý định ngẫu nhiên
            // Choose random intent
            int randomIndex = Random.Range(0, possibleIntents.Count);
            _enemyIntentType = possibleIntents[randomIndex];
            
            // Xác định giá trị ý định
            // Determine intent value
            switch (_enemyIntentType)
            {
                case "Attack":
                    _enemyIntentValue = enemyStats.Attack;
                    break;
                case "Defend":
                    _enemyIntentValue = Random.Range(2, 5);
                    break;
                case "Buff":
                    _enemyIntentValue = Random.Range(1, 3);
                    break;
                case "Heal":
                    _enemyIntentValue = Random.Range(3, 7);
                    break;
                default:
                    _enemyIntentValue = enemyStats.Attack;
                    break;
            }
        }
        
        /// <summary>
        /// End the enemy's turn
        /// Kết thúc lượt của kẻ địch
        /// </summary>
        public void EndEnemyTurn()
        {
            Debug.Log("Ending enemy turn");
            
            // Tăng bộ đếm lượt
            // Increment turn counter
            _currentTurn++;
        }
        
        /// <summary>
        /// Select a card to play
        /// Chọn một lá bài để chơi
        /// </summary>
        public void SelectCard(Entity card)
        {
            if (card == null)
                return;
                
            // Kiểm tra xem lá bài đã được chọn chưa
            // Check if card is already selected
            if (_selectedCards.Contains(card))
            {
                _selectedCards.Remove(card);
                Debug.Log("Card deselected");
                return;
            }
            
            // Kiểm tra xem đã chọn đủ số lá bài tối đa chưa
            // Check if we already have the maximum number of cards selected
            int maxCardsPerTurn = _cardSystem.GetMaxCardsPerTurn();
            if (_selectedCards.Count >= maxCardsPerTurn)
            {
                Debug.Log($"Already have {maxCardsPerTurn} cards selected, deselect one first");
                return;
            }
            
            // Lấy chi phí lá bài
            // Get card cost
            int cardCost = 1; // Chi phí mặc định - Default cost
            CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
            if (cardInfo != null)
            {
                cardCost = cardInfo.Cost;
            }
            
            // Kiểm tra đủ năng lượng
            // Check if we have enough energy
            int totalCost = 0;
            foreach (var selectedCard in _selectedCards)
            {
                CardInfoComponent selectedCardInfo = selectedCard.GetComponent<CardInfoComponent>();
                if (selectedCardInfo != null)
                {
                    totalCost += selectedCardInfo.Cost;
                }
            }
            
            totalCost += cardCost;
            
            if (_playerEnergy < totalCost)
            {
                Debug.Log("Not enough energy to select this card");
                return;
            }
            
            // Thêm vào danh sách thẻ đã chọn
            // Add to selected cards
            _selectedCards.Add(card);
            Debug.Log("Card selected");
        }
        
        /// <summary>
        /// Discard selected cards
        /// Bỏ các lá bài đã chọn
        /// </summary>
        public void DiscardSelectedCards()
        {
            if (_selectedCards.Count == 0)
                return;
                
            // Bỏ tất cả lá bài đã chọn
            // Discard all selected cards
            _cardSystem.DiscardCards(_selectedCards);
            
            // Giảm năng lượng
            // Deduct energy
            foreach (var card in _selectedCards)
            {
                CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                if (cardInfo != null)
                {
                    _playerEnergy -= cardInfo.Cost;
                }
            }
            
            Debug.Log($"Discarded {_selectedCards.Count} cards");
            
            // Đặt lại danh sách đã chọn
            // Reset selected cards
            _selectedCards.Clear();
        }
        
        /// <summary>
        /// Resolve the selected cards as an attack
        /// Giải quyết các lá bài đã chọn như một đòn tấn công
        /// </summary>
        public void ResolveSelectedCards()
        {
            if (_selectedCards.Count == 0)
                return;
                
            // Kiểm tra thẻ hỗ trợ
            // Check for support cards
            List<Entity> attackCards = new List<Entity>();
            
            foreach (var card in _selectedCards)
            {
                if (card.HasComponent<SupportCardComponent>())
                {
                    // Chơi như thẻ hỗ trợ
                    // Play as support
                    _cardSystem.PlayAsSupport(card);
                    
                    Debug.Log("Played card as support");
                }
                else
                {
                    // Chơi thẻ
                    // Play the card
                    _cardSystem.PlayCard(card);
                    
                    // Thêm vào danh sách tấn công
                    // Add to attack cards
                    attackCards.Add(card);
                    
                    // Thêm vào danh sách đã chơi
                    // Add to played cards
                    _playedCards.Add(card);
                    
                    Debug.Log("Played card for attack");
                }
                
                // Giảm năng lượng
                // Deduct energy
                CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                if (cardInfo != null)
                {
                    _playerEnergy -= cardInfo.Cost;
                }
            }
            
            // Nếu có thẻ tấn công, xử lý tấn công
            // If there are attack cards, process attack
            if (attackCards.Count > 0)
            {
                // Tính điểm sát thương dựa trên các lá bài đã chơi
                // Calculate damage based on played cards
                int damage = _scoringSystem.CalculateScore(attackCards, _enemyEntity);
                
                // Áp dụng sát thương cho kẻ địch
                // Apply damage to enemy
                ApplyDamage(_enemyEntity, damage);
                
                Debug.Log($"Dealt {damage} damage to enemy!");
                
                // Kiểm tra chí mạng
                // Check for critical hit
                if (_scoringSystem.IsCriticalHit(attackCards, _enemyEntity))
                {
                    int critDamage = damage / 2; // 50% sát thương phụ - 50% extra damage
                    ApplyDamage(_enemyEntity, critDamage);
                    Debug.Log($"Critical hit! Extra {critDamage} damage!");
                }
            }
            
            // Cập nhật hệ thống thẻ hỗ trợ với các lá bài đã chơi
            // Update support card system with played cards
            _supportCardSystem.SetPlayedCards(_playedCards);
            
            // Đặt lại danh sách đã chọn
            // Clear selected cards
            _selectedCards.Clear();
        }
        
        /// <summary>
        /// Apply damage to a target
        /// Gây sát thương cho mục tiêu
        /// </summary>
        private void ApplyDamage(Entity target, int damage)
        {
            StatsComponent stats = target.GetComponent<StatsComponent>();
            if (stats != null)
            {
                stats.Health -= damage;
                if (stats.Health < 0)
                    stats.Health = 0;
                
                Debug.Log($"Applied {damage} damage to target");
            }
        }
        
        /// <summary>
        /// Check for support card activations
        /// Kiểm tra kích hoạt thẻ hỗ trợ
        /// </summary>
        public void CheckSupportCards()
        {
            // Lấy trạng thái trận đấu hiện tại
            // Get current battle state
            BattleStateType currentState = _stateMachine.GetCurrentStateType();
            
            // Kiểm tra kích hoạt thẻ hỗ trợ dựa trên trạng thái
            // Check for specific support card activations based on the state
            switch (currentState)
            {
                case BattleStateType.PlayerTurnStart:
                    _supportCardSystem.CheckOnTurnStartActivations(_playerEntity);
                    break;
                case BattleStateType.PlayerTurnEnd:
                    _supportCardSystem.CheckOnTurnEndActivations(_playerEntity);
                    break;
                case BattleStateType.EnemyTurnStart:
                    // Có thể kiểm tra kích hoạt riêng cho kẻ địch
                    // Could check enemy-specific activations here
                    break;
                default:
                    // Kiểm tra mặc định cho tất cả trạng thái
                    // Default checks for all states
                    _supportCardSystem.CheckOnEntryActivations();
                    break;
            }
        }
        
        /// <summary>
        /// End the battle
        /// Kết thúc trận đấu
        /// </summary>
        public void EndBattle()
        {
            Debug.Log("Battle ended");
            
            // Đặt lại trạng thái trận đấu
            // Reset battle state
            _currentTurn = 1;
            _playerEnergy = _maxPlayerEnergy;
            _selectedCards.Clear();
            _playedCards.Clear();
            
            // Đặt lại số lần tái sử dụng bộ bài
            // Reset deck recycle count
            _cardSystem.ResetRecycleCount();
        }
        
        /// <summary>
        /// Give rewards to the player after a successful battle
        /// Trao phần thưởng cho người chơi sau khi chiến thắng
        /// </summary>
        public void GiveRewards()
        {
            Debug.Log("Giving rewards to player");
            
            // Logic phần thưởng sẽ được đặt ở đây
            // Reward logic would go here
        }
        
        /// <summary>
        /// Check if battle is over
        /// Kiểm tra trận đấu đã kết thúc chưa
        /// </summary>
        public bool IsBattleOver()
        {
            StatsComponent playerStats = _playerEntity?.GetComponent<StatsComponent>();
            StatsComponent enemyStats = _enemyEntity?.GetComponent<StatsComponent>();
            
            return (playerStats != null && playerStats.Health <= 0) || 
                   (enemyStats != null && enemyStats.Health <= 0);
        }
        
        /// <summary>
        /// Get the winner of the battle
        /// Lấy người chiến thắng của trận đấu
        /// </summary>
        public Entity GetWinner()
        {
            if (!IsBattleOver())
                return null;
            
            StatsComponent playerStats = _playerEntity?.GetComponent<StatsComponent>();
            StatsComponent enemyStats = _enemyEntity?.GetComponent<StatsComponent>();
            
            if (playerStats != null && playerStats.Health <= 0)
                return _enemyEntity;
            else
                return _playerEntity;
        }
        
        /// <summary>
        /// Get battle statistics for display
        /// Lấy thống kê trận đấu để hiển thị
        /// </summary>
        public Dictionary<string, object> GetBattleStats()
        {
            Dictionary<string, object> stats = new Dictionary<string, object>();
            
            // Thông tin cơ bản
            // Basic info
            stats["CurrentTurn"] = _currentTurn;
            stats["PlayerEnergy"] = _playerEnergy;
            stats["MaxPlayerEnergy"] = _maxPlayerEnergy;
            stats["DeckCount"] = _cardSystem.GetDeck().Count;
            stats["DiscardCount"] = _cardSystem.GetDiscardPile().Count;
            stats["HandCount"] = _cardSystem.GetHand().Count;
            stats["RecycleCount"] = _cardSystem.GetRecycleCount();
            
            // Thông tin người chơi
            // Player info
            if (_playerEntity != null)
            {
                StatsComponent playerStats = _playerEntity.GetComponent<StatsComponent>();
                if (playerStats != null)
                {
                    stats["PlayerHealth"] = playerStats.Health;
                    stats["PlayerMaxHealth"] = playerStats.MaxHealth;
                    stats["PlayerAttack"] = playerStats.Attack;
                    stats["PlayerDefense"] = playerStats.Defense;
                }
            }
            
            // Thông tin kẻ địch
            // Enemy info
            if (_enemyEntity != null)
            {
                StatsComponent enemyStats = _enemyEntity.GetComponent<StatsComponent>();
                if (enemyStats != null)
                {
                    stats["EnemyHealth"] = enemyStats.Health;
                    stats["EnemyMaxHealth"] = enemyStats.MaxHealth;
                    stats["EnemyAttack"] = enemyStats.Attack;
                    stats["EnemyDefense"] = enemyStats.Defense;
                }
                
                // Ý định kẻ địch
                // Enemy intent
                stats["EnemyIntentType"] = _enemyIntentType;
                stats["EnemyIntentValue"] = _enemyIntentValue;
            }
            
            return stats;
        }
        
        // Getters
        public Entity GetPlayerEntity() => _playerEntity;
        public Entity GetEnemyEntity() => _enemyEntity;
        public BattleStateMachine GetStateMachine() => _stateMachine;
        public int GetPlayerEnergy() => _playerEnergy;
        public int GetMaxPlayerEnergy() => _maxPlayerEnergy;
        public int GetCurrentTurn() => _currentTurn;
        public string GetEnemyIntentType() => _enemyIntentType;
        public int GetEnemyIntentValue() => _enemyIntentValue;
        public List<Entity> GetSelectedCards() => new List<Entity>(_selectedCards);
    }
}