namespace Core
{
    /// <summary>
    /// Base class for all systems in the ECS architecture
    /// Systems contain the logic that operates on entities with specific components
    /// </summary>
    public abstract class System
    {
        protected EntityManager EntityManager;
        public System(EntityManager entityManager)
        {
            EntityManager = entityManager;
        }
        public abstract void Update(float deltaTime);
    }
}