namespace Core
{
    /// <summary>
    /// Base class for all components in the ECS architecture
    /// Components are pure data containers with no behavior
    /// </summary>
    public abstract class Component
    {
        public Entity Entity { get; set; }
    }
}