using System;

namespace Economy
{
    public class ResourceInventory
    {
        private int _eye;
        private int _book;
        private int _void;
        private int _mystery;

        public event Action<ResourceType, int> ResourceChanged;

        public ResourceInventory(int eye = 0, int book = 0, int @void = 0, int mystery = 0)
        {
            _eye = Math.Max(0, eye);
            _book = Math.Max(0, book);
            _void = Math.Max(0, @void);
            _mystery = Math.Max(0, mystery);
        }

        public int Get(ResourceType type) => type switch
        {
            ResourceType.Eye => _eye,
            ResourceType.Book => _book,
            ResourceType.Void => _void,
            ResourceType.Mystery => _mystery,
            _ => 0
        };

        public void Add(ResourceType type, int amount)
        {
            if (amount <= 0) return;

            switch (type)
            {
                case ResourceType.Eye: _eye += amount; break;
                case ResourceType.Book: _book += amount; break;
                case ResourceType.Void: _void += amount; break;
                case ResourceType.Mystery: _mystery += amount; break;
            }

            ResourceChanged?.Invoke(type, Get(type));
        }
    }
}

