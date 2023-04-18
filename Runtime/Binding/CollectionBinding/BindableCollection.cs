using System;
using System.Collections;
using System.Collections.Generic;

namespace MVVMToolkit.Binding.CollectionBinding
{
    internal class BindableCollection<T> : IList<T>, IList
    {
        private readonly List<T> _list = new();
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static void CheckAndThrow(object value, out T item)
        {
            if (value is not T match)
            {
                throw new InvalidOperationException();
            }

            item = match;
        }

        public void Add(T item)
        {
            _list.Add(item);
        }


        /// <inheritdoc />
        public int Add(object value)
        {
            CheckAndThrow(value, out var item);
            Add(item);
            return _list.Count - 1;
        }

        public void Clear()
        {
            _list.Clear();
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public void Insert(int index, object value)
        {
            CheckAndThrow(value, out var item);
            _list.Insert(index, item);
        }

        public void Remove(object value)
        {
            CheckAndThrow(value, out var item);
            _list.Remove(item);
        }

        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
        }

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        object IList.this[int index]
        {
            get => this[index];
            set
            {
                CheckAndThrow(value, out var item);
                this[index] = item;
            }
        }


        public bool Contains(object value)
        {
            CheckAndThrow(value, out var item);
            return _list.Contains(item);
        }

        public int IndexOf(object value)
        {
            CheckAndThrow(value, out var item);
            return _list.IndexOf(item);
        }


        public bool IsFixedSize => false;


        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }


        public int Count => _list.Count;


        public bool IsReadOnly => false;

        public void CopyTo(Array array, int index)
        {
            for (var i = 0; i < _list.Count; i++)
            {
                array.SetValue(_list[i], i);
            }
        }

        public bool IsSynchronized => false;
        public object SyncRoot => this;


        public int IndexOf(T item) => _list.IndexOf(item);
    }
}