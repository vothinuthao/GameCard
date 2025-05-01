using System;
using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// Represents a basic entity in the ECS architecture
    /// An entity is essentially just an ID with a collection of components
    /// </summary>
    public class Entity
    {
        public int Id { get; private set; }
        private Dictionary<Type, Component> _components = new Dictionary<Type, Component>();
        
        // Constructor
        public Entity(int id)
        {
            Id = id;
        }
        
        public void AddComponent(Component component)
        {
            Type type = component.GetType();
            component.Entity = this;
            
            if (_components.ContainsKey(type))
                _components[type] = component;
            else
                _components.Add(type, component);
        }
        
        // Get a component of the specified type
        public T GetComponent<T>() where T : Component
        {
            Type type = typeof(T);
            if (_components.TryGetValue(type, out var component))
                return (T)component;
            return null;
        }
        
        // Check if the entity has a component of the specified type
        public bool HasComponent<T>() where T : Component
        {
            return _components.ContainsKey(typeof(T));
        }
        
        // Remove a component of the specified type
        public void RemoveComponent<T>() where T : Component
        {
            Type type = typeof(T);
            if (_components.ContainsKey(type))
                _components.Remove(type);
        }
    }
}