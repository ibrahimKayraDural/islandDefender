using System.Collections;
using System.Collections.Generic;

namespace LimitedQueue
{
    public enum EnqueueMode { DeleteLast, DontAllowNew };

    public class LimitedQueue<T>
    {
        public int Limit { get; private set; }
        public int Count { get { return _queue.Count; } }

        Queue<T> _queue;
        EnqueueMode _mode;


        public LimitedQueue(int limit, EnqueueMode mode = EnqueueMode.DeleteLast)
        {
            _queue = new Queue<T>();
            Limit = limit;
            _mode = mode;
        }

        public T[] ToArray() => _queue.ToArray();

        public void Enqueue(T item)
        {
            if (_queue.Count == Limit)
            {
                switch (_mode)
                {
                    case EnqueueMode.DeleteLast:
                        _queue.Dequeue();
                        break;

                    default: return;
                }
            }

            _queue.Enqueue(item);
        }

        public void Dequeue()
        {
            if (_queue.Count == 0)
                return;
            _queue.Dequeue();
        }

        public bool Contains(T item)
        {
            return _queue.Contains(item);
        }

        public void CopyTo(T[] array, int index)
        {
            _queue.CopyTo(array, index);
        }

        public Queue<T>.Enumerator GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        public void Clear()
        {
            _queue.Clear();
        }
    }
}