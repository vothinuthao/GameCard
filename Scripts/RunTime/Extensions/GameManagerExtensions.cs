using Core;
using Core.Utils;
using Systems;

namespace RunTime
{
    /// <summary>
    /// Extension methods for GameManager
    /// </summary>
    public static class GameManagerExtensions
    {
        /// <summary>
        /// Get the entity manager
        /// </summary>
        public static EntityManager GetEntityManager(this GameManager gameManager)
        {
            // Use reflection to get the private entityManager field
            // This is a workaround for the lack of proper access to private fields
            System.Reflection.FieldInfo field = typeof(GameManager).GetField("entityManager", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            return field?.GetValue(gameManager) as EntityManager;
        }
        
        /// <summary>
        /// Get the card system
        /// </summary>
        public static CardSystem GetCardSystem(this GameManager gameManager)
        {
            // Use reflection to get the private cardSystem field
            System.Reflection.FieldInfo field = typeof(GameManager).GetField("cardSystem", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            return field?.GetValue(gameManager) as CardSystem;
        }
        
        /// <summary>
        /// Get the battle system
        /// </summary>
        public static BattleSystem GetBattleSystem(this GameManager gameManager)
        {
            // Use reflection to get the private battleSystem field
            System.Reflection.FieldInfo field = typeof(GameManager).GetField("battleSystem", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            return field?.GetValue(gameManager) as BattleSystem;
        }
        
        /// <summary>
        /// Get the element interaction system
        /// </summary>
        public static ElementInteractionSystem GetElementInteractionSystem(this GameManager gameManager)
        {
            // Use reflection to get the private elementInteractionSystem field
            System.Reflection.FieldInfo field = typeof(GameManager).GetField("elementInteractionSystem", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            return field?.GetValue(gameManager) as ElementInteractionSystem;
        }
        
        /// <summary>
        /// Get the support card system
        /// </summary>
        public static SupportCardSystem GetSupportCardSystem(this GameManager gameManager)
        {
            // Use reflection to get the private supportCardSystem field
            System.Reflection.FieldInfo field = typeof(GameManager).GetField("supportCardSystem", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            return field?.GetValue(gameManager) as SupportCardSystem;
        }
        
        /// <summary>
        /// Get the player entity
        /// </summary>
        public static Entity GetPlayerEntity(this GameManager gameManager)
        {
            // Use reflection to get the private playerEntity field
            System.Reflection.FieldInfo field = typeof(GameManager).GetField("playerEntity", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            return field?.GetValue(gameManager) as Entity;
        }
        
        /// <summary>
        /// Get the enemy entity
        /// </summary>
        public static Entity GetEnemyEntity(this GameManager gameManager)
        {
            // Use reflection to get the private enemyEntity field
            System.Reflection.FieldInfo field = typeof(GameManager).GetField("enemyEntity", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            return field?.GetValue(gameManager) as Entity;
        }
        
        /// <summary>
        /// Get the current season
        /// </summary>
        public static Season GetCurrentSeason(this GameManager gameManager)
        {
            // Use reflection to get the private currentSeason field
            System.Reflection.FieldInfo field = typeof(GameManager).GetField("currentSeason", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            return (Season)(field?.GetValue(gameManager) ?? Season.Spring);
        }
    }
}
