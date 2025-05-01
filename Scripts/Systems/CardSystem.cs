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
        // Reference to the deck of cards
        private List<Entity> _deck = new List<Entity>();
        private List<Entity> _hand = new List<Entity>();
        private List<Entity> _discardPile = new List<Entity>();
        private List<Entity> _playZone = new List<Entity>();
        private List<Entity> _supportZone = new List<Entity>();
        
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
        /// </summary>
        public Entity DrawCard()
        {
            if (_deck.Count == 0)
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
        /// </summary>
        public List<Entity> DrawCards(int count)
        {
            List<Entity> drawnCards = new List<Entity>();
            for (int i = 0; i < count; i++)
            {
                Entity card = DrawCard();
                if (card != null)
                    drawnCards.Add(card);
                else
                    break; // Deck is empty
            }
            return drawnCards;
        }
        
        /// <summary>
        /// Play a card from the hand to the play zone
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
        /// Shuffle the discard pile back into the deck
        /// </summary>
        public void ShuffleDeck()
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
                }
            }
        }
        
        /// <summary>
        /// Add a card to the deck
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
        
        // Getters for different card zones
        public List<Entity> GetDeck() => new List<Entity>(_deck);
        public List<Entity> GetHand() => new List<Entity>(_hand);
        public List<Entity> GetDiscardPile() => new List<Entity>(_discardPile);
        public List<Entity> GetPlayZone() => new List<Entity>(_playZone);
        public List<Entity> GetSupportZone() => new List<Entity>(_supportZone);
    }
}