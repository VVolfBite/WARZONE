using System;
using System.Collections.Generic;

namespace Warzone.Foundation
{
    public sealed class SimpleObjectPool<T> where T : class
    {
        private readonly Stack<T> _items = new Stack<T>();
        private readonly Func<T> _factory;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;

        public SimpleObjectPool(Func<T> factory, Action<T> onGet = null, Action<T> onRelease = null, int preloadCount = 0)
        {
            _factory = factory;
            _onGet = onGet;
            _onRelease = onRelease;

            for (int i = 0; i < preloadCount; i++)
            {
                _items.Push(_factory());
            }
        }

        public T Get()
        {
            T item = _items.Count > 0 ? _items.Pop() : _factory();
            _onGet?.Invoke(item);
            return item;
        }

        public void Release(T item)
        {
            if (item == null)
            {
                return;
            }

            _onRelease?.Invoke(item);
            _items.Push(item);
        }
    }
}
