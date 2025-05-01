using Core;

namespace Components
{
    /// <summary>
    /// Contains the stats for a card
    /// </summary>
    public class StatsComponent : Component
    {
        // Basic stats
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Speed { get; set; }

        // Advanced stats
        public float Accuracy { get; set; } = 1.0f;
        public float Evasion { get; set; } = 0.0f;
        public float Penetration { get; set; } = 0.0f;
        public float CriticalChance { get; set; } = 0.05f;
        public float CriticalDamage { get; set; } = 1.5f;
        public float Resistance { get; set; } = 0.0f;

        // Constructor with default values
        public StatsComponent()
        {
            Attack = 1;
            Defense = 1;
            Health = 5;
            MaxHealth = 5;
            Speed = 1;
        }

        // Constructor with custom values
        public StatsComponent(int attack, int defense, int health, int speed)
        {
            Attack = attack;
            Defense = defense;
            Health = health;
            MaxHealth = health;
            Speed = speed;
        }
    }
}