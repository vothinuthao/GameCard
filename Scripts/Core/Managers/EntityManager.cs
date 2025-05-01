using System.Collections.Generic;
using System.Linq;
using Core;

public class EntityManager
{
    // Dictionary to store all entities, indexed by their ID
    private Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
    private int nextEntityId = 0;
        
    // Create a new entity
    public Entity CreateEntity()
    {
        int id = nextEntityId++;
        Entity entity = new Entity(id);
        entities.Add(id, entity);
        return entity;
    }
        
    // Get an entity by ID
    public Entity GetEntity(int id)
    {
        if (entities.ContainsKey(id))
            return entities[id];
        return null;
    }
        
    // Get all entities
    public IEnumerable<Entity> GetAllEntities()
    {
        return entities.Values;
    }
        
    // Get entities with specific components
    public IEnumerable<Entity> GetEntitiesWithComponents<T>() where T : Component
    {
        return entities.Values.Where(e => e.HasComponent<T>());
    }
        
    // Get entities with multiple specific components
    public IEnumerable<Entity> GetEntitiesWithComponents<T1, T2>() 
        where T1 : Component 
        where T2 : Component
    {
        return entities.Values.Where(e => e.HasComponent<T1>() && e.HasComponent<T2>());
    }
        
    // Get entities with three specific components
    public IEnumerable<Entity> GetEntitiesWithComponents<T1, T2, T3>() 
        where T1 : Component 
        where T2 : Component
        where T3 : Component
    {
        return entities.Values.Where(e => 
            e.HasComponent<T1>() && 
            e.HasComponent<T2>() &&
            e.HasComponent<T3>());
    }
        
    // Remove an entity
    public void RemoveEntity(int id)
    {
        if (entities.ContainsKey(id))
            entities.Remove(id);
    }
}