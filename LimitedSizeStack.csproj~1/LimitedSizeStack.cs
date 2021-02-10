using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApplication
{
    public class LimitedSizeStack<T>
    {
        LinkedList<T> stack = new LinkedList<T>();
        int Limit { get; set; }

        public LimitedSizeStack(int limit)
        {
            Limit = limit;
        }

        public void Push(T item)
        {
            if (Limit == stack.Count)
                stack.RemoveFirst();
            stack.AddLast(item);
        }

        public T Pop()
        {
            T result = stack.Last.Value;
            stack.Remove(stack.Last);
            return result;
        }

        public int Count
        {
            get
            {
                return stack.Count;
            }
        }
    }
}
