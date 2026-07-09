using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace task03
{
    public class CustomCollection<T> : IEnumerable<T>
    {
        private readonly List<T> _elements = new();

        public void Add(T item)
        {

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Элемент не может быть null");
            }
            _elements.Add(item);
        }
        public IEnumerator<T> GetEnumerator() => _elements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetReverseEnumerator()
        {
            for (int i = _elements.Count - 1; i >= 0; i--)
            {
                yield return _elements[i];
            }
        }

        public static IEnumerable<int> GenerateSequence(int start, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Количество элементов не может быть отрицательным");
            }

            for (int i = 0; i < count; i++)
            {
                yield return start + i;
            }
        }

        public IEnumerable<T> FilterAndSort(Func<T, bool> filter, Func<T, IComparable> keySelector)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter), "Функция фильтрации не может быть null");
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector), "Функция выбора ключа не может быть null");
            }
            return _elements.Where(filter).OrderBy(keySelector);
        }

    }
}
