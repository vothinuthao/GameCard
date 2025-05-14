using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;

namespace Systems
{
    /// <summary>
    /// System that manages cards, decks, hands, etc.
    /// </summary>
    public class CardSystem : Core.System
    {
        // Tham chiếu tới bộ bài, tay bài, chồng bỏ, và khu vực chơi bài
        // Reference to deck, hand, discard pile, play zone
        private List<Entity> _deck = new List<Entity>();
        private List<Entity> _hand = new List<Entity>();
        private List<Entity> _discardPile = new List<Entity>();
        private List<Entity> _playZone = new List<Entity>();
        private List<Entity> _supportZone = new List<Entity>();
        
        // Số lần bộ bài đã được tái sử dụng
        // Number of times the deck has been recycled
        private int _recycleCount = 0;
        
        // Hằng số cho kích thước tay bài và số lá bài có thể chơi mỗi lượt
        // Constants for hand size and max cards playable per turn
        private const int MAX_HAND_SIZE = 8;
        private const int MAX_CARDS_PER_TURN = 3;
        
        // Constructor
        public CardSystem(EntityManager entityManager) : base(entityManager)
        {
        }
        
        /// <summary>
        /// Update method - called every frame
        /// </summary>
        public override void Update(float deltaTime)
        {
            // Update card states, animations, etc.
            // This would be more complex in a real implementation
        }
        
        /// <summary>
        /// Draw a card from the deck to the hand
        /// Rút một lá bài từ bộ bài vào tay
        /// </summary>
        public Entity DrawCard()
        {
            // Nếu bộ bài trống, kiểm tra chồng bỏ
            // If deck is empty, check discard pile
            if (_deck.Count == 0)
            {
                // Nếu cả chồng bỏ cũng trống, trả về null
                // If discard pile is also empty, return null
                if (_discardPile.Count == 0)
                    return null;
                
                // Tái sử dụng chồng bỏ thành bộ bài mới
                // Recycle discard pile into new deck
                RecycleDiscardPile();
            }
            
            // Nếu tay bài đã đạt kích thước tối đa, không rút thêm
            // If hand is at max size, don't draw more
            if (_hand.Count >= MAX_HAND_SIZE)
                return null;
            
            Entity card = _deck[0];
            _deck.RemoveAt(0);
            _hand.Add(card);
            
            CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
            if (cardInfo != null)
            {
                cardInfo.State = CardState.InHand;
            }
            
            return card;
        }
        
        /// <summary>
        /// Draw multiple cards
        /// Rút nhiều lá bài cùng lúc
        /// </summary>
        public List<Entity> DrawCards(int count)
        {
            List<Entity> drawnCards = new List<Entity>();
            for (int i = 0; i < count; i++)
            {
                // Chỉ rút thêm bài nếu tay bài chưa đầy
                // Only draw more cards if hand is not full
                if (_hand.Count < MAX_HAND_SIZE)
                {
                    Entity card = DrawCard();
                    if (card != null)
                        drawnCards.Add(card);
                    else
                        break; // Deck is empty
                }
                else
                {
                    break; // Hand is full
                }
            }
            return drawnCards;
        }
        
        /// <summary>
        /// Fill hand to maximum size by drawing cards
        /// Rút bài cho đến khi tay bài đầy
        /// </summary>
        public List<Entity> FillHand()
        {
            int cardsToDraw = MAX_HAND_SIZE - _hand.Count;
            if (cardsToDraw <= 0)
                return new List<Entity>();
                
            return DrawCards(cardsToDraw);
        }
        
        /// <summary>
        /// Play a card from the hand to the play zone
        /// Đánh một lá bài từ tay vào khu vực chơi bài
        /// </summary>
        public void PlayCard(Entity card)
        {
            if (_hand.Contains(card))
            {
                _hand.Remove(card);
                _playZone.Add(card);
                
                // Update the card state
                CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                if (cardInfo != null)
                {
                    cardInfo.State = CardState.InPlay;
                }
                
                // Logic for playing the card would be implemented in BattleSystem
            }
        }
        
        /// <summary>
        /// Move a card to the support zone
        /// Đưa một lá bài vào khu vực hỗ trợ
        /// </summary>
        public void PlayAsSupport(Entity card)
        {
            if (_hand.Contains(card) && _supportZone.Count < 6) // Maximum 6 support cards
            {
                _hand.Remove(card);
                _supportZone.Add(card);
                
                // Update the card state
                CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                if (cardInfo != null)
                {
                    cardInfo.State = CardState.InSupportZone;
                }
            }
        }
        
        /// <summary>
        /// Discard a card from the hand
        /// Bỏ một lá bài từ tay vào chồng bỏ
        /// </summary>
        public void DiscardCard(Entity card)
        {
            if (_hand.Contains(card))
            {
                _hand.Remove(card);
                _discardPile.Add(card);
                
                // Update the card state
                CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                if (cardInfo != null)
                {
                    cardInfo.State = CardState.InDiscardPile;
                }
            }
        }
        
        /// <summary>
        /// Discard all cards from the play zone
        /// Bỏ tất cả lá bài từ khu vực chơi bài vào chồng bỏ
        /// </summary>
        public void DiscardPlayZone()
        {
            foreach (var card in _playZone)
            {
                _discardPile.Add(card);
                CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                if (cardInfo != null)
                {
                    cardInfo.State = CardState.InDiscardPile;
                }
            }
            
            _playZone.Clear();
        }
        
        /// <summary>
        /// Discard multiple cards from hand
        /// Bỏ nhiều lá bài từ tay vào chồng bỏ
        /// </summary>
        public void DiscardCards(List<Entity> cards)
        {
            foreach (var card in cards)
            {
                DiscardCard(card);
            }
        }
        
        /// <summary>
        /// Discard a random card from hand
        /// Bỏ một lá bài ngẫu nhiên từ tay vào chồng bỏ
        /// </summary>
        public Entity DiscardRandomCard()
        {
            if (_hand.Count == 0)
                return null;
                
            int randomIndex = UnityEngine.Random.Range(0, _hand.Count);
            Entity card = _hand[randomIndex];
            DiscardCard(card);
            return card;
        }
        
        /// <summary>
        /// Recycle the discard pile back into the deck
        /// Tái sử dụng chồng bỏ thành bộ bài mới
        /// </summary>
        public void RecycleDiscardPile()
        {
            _deck.AddRange(_discardPile);
            _discardPile.Clear();
            
            // Fisher-Yates shuffle
            System.Random random = new System.Random();
            int n = _deck.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (_deck[k], _deck[n]) = (_deck[n], _deck[k]);
            }
            
            // Update all card states
            foreach (var card in _deck)
            {
                CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                if (cardInfo != null)
                {
                    cardInfo.State = CardState.InDeck;
                    
                    // Giảm sức mạnh của bài sau mỗi lần tái sử dụng bộ bài
                    // Reduce card power after each deck recycle
                    if (_recycleCount > 0)
                    {
                        StatsComponent stats = card.GetComponent<StatsComponent>();
                        if (stats != null)
                        {
                            // Giảm sức tấn công và phòng thủ mỗi lần tái sử dụng
                            // Reduce attack and defense each recycle
                            float reductionFactor = 0.1f * _recycleCount; // 10% reduction per recycle
                            stats.Attack = System.Math.Max(1, (int)(stats.Attack * (1 - reductionFactor)));
                            stats.Defense = System.Math.Max(0, (int)(stats.Defense * (1 - reductionFactor)));
                        }
                    }
                }
            }
            
            // Increment recycle count
            _recycleCount++;
        }
        
        /// <summary>
        /// Add a card to the deck
        /// Thêm một lá bài vào bộ bài
        /// </summary>
        public void AddCardToDeck(Entity card)
        {
            _deck.Add(card);
            
            // Update the card state
            CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
            if (cardInfo != null)
            {
                cardInfo.State = CardState.InDeck;
            }
        }
        
        /// <summary>
        /// Reset deck to initial state (helpful for new battles)
        /// Đặt lại bộ bài về trạng thái ban đầu (hữu ích cho trận đấu mới)
        /// </summary>
        public void ResetDeck()
        {
            // Trả tất cả thẻ từ tay, khu vực chơi, và chồng bỏ về bộ bài
            // Return all cards from hand, play zone, and discard pile to deck
            _deck.AddRange(_hand);
            _deck.AddRange(_playZone);
            _deck.AddRange(_discardPile);
            
            // Xóa từ các vùng khác
            // Clear from other zones
            _hand.Clear();
            _playZone.Clear();
            _discardPile.Clear();
            
            // Đặt lại trạng thái thẻ
            // Reset card states
            foreach (var card in _deck)
            {
                CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                if (cardInfo != null)
                {
                    cardInfo.State = CardState.InDeck;
                }
            }
            
            // Xáo trộn bộ bài
            // Shuffle the deck
            ShuffleDeck();
            
            // Đặt lại số lần tái sử dụng
            // Reset recycle count
            ResetRecycleCount();
        }
        
        /// <summary>
        /// Clear all cards from hand back to deck
        /// Trả lại tất cả lá bài trên tay vào bộ bài
        /// </summary>
        public void ReturnHandToDeck()
        {
            foreach (var card in _hand)
            {
                CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                if (cardInfo != null)
                {
                    cardInfo.State = CardState.InDeck;
                }
                _deck.Add(card);
            }
            _hand.Clear();
            ShuffleDeck();
        }
        
        /// <summary>
        /// Get the maximum number of cards that can be played per turn
        /// Lấy số lá bài tối đa có thể chơi trong một lượt
        /// </summary>
        public int GetMaxCardsPerTurn()
        {
            return MAX_CARDS_PER_TURN;
        }
        
        /// <summary>
        /// Get the maximum hand size
        /// Lấy kích thước tối đa của tay bài
        /// </summary>
        public int GetMaxHandSize()
        {
            return MAX_HAND_SIZE;
        }
        
        /// <summary>
        /// Get the number of times the deck has been recycled
        /// Lấy số lần bộ bài đã được tái sử dụng
        /// </summary>
        public int GetRecycleCount()
        {
            return _recycleCount;
        }
        
        /// <summary>
        /// Reset the recycle count (e.g. at the start of a new battle)
        /// Đặt lại số lần tái sử dụng (ví dụ: khi bắt đầu trận đấu mới)
        /// </summary>
        public void ResetRecycleCount()
        {
            _recycleCount = 0;
        }
        /// <summary>
        /// Shuffle the deck
        /// Xáo trộn bộ bài
        /// </summary>
        public void ShuffleDeck()
        {
            // Fisher-Yates shuffle
            System.Random random = new System.Random();
            int n = _deck.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (_deck[k], _deck[n]) = (_deck[n], _deck[k]);
            }
        }
        
        // Getters for different card zones
        public List<Entity> GetDeck() => new List<Entity>(_deck);
        public List<Entity> GetHand() => new List<Entity>(_hand);
        public List<Entity> GetDiscardPile() => new List<Entity>(_discardPile);
        public List<Entity> GetPlayZone() => new List<Entity>(_playZone);
        public List<Entity> GetSupportZone() => new List<Entity>(_supportZone);
    }
}