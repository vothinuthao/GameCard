using System.Collections.Generic;

namespace Core
{
    public class SystemManager
    {
        // List to store all systems
        private List<System> _systems = new List<System>();
        
        // Add a system
        public void AddSystem(System system)
        {
            _systems.Add(system);
        }
        
        // Update all systems
        public void UpdateAllSystems(float deltaTime)
        {
            foreach (var system in _systems)
            {
                system.Update(deltaTime);
            }
        }
    }
}