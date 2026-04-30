using System;

namespace Economy
{
    public class ResourceInventory
    {
        private int _wood;
        private int _metal;
        private int _crystal;
        private int _mystery;

        public event Action<ResourceType, int> ResourceChanged;

        public ResourceInventory(int wood = 0, int metal = 0, int crystal = 0, int mystery = 0)
        {
            _wood = Math.Max(0, wood);
            _metal = Math.Max(0, metal);
            _crystal = Math.Max(0, crystal);
            _mystery = Math.Max(0, mystery);
        }

        public int Get(ResourceType type) => type switch
        {
            ResourceType.Wood => _wood,
            ResourceType.Metal => _metal,
            ResourceType.Crystal => _crystal,
            ResourceType.Mystery => _mystery,
            _ => 0
        };

        public void Add(ResourceType type, int amount)
        {
            if (amount <= 0) return;

            switch (type)
            {
                case ResourceType.Wood: _wood += amount; break;
                case ResourceType.Metal: _metal += amount; break;
                case ResourceType.Crystal: _crystal += amount; break;
                case ResourceType.Mystery: _mystery += amount; break;
            }

            ResourceChanged?.Invoke(type, Get(type));
        }
    }
}

