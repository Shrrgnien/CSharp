using System;
using System.Collections;
using System.Collections.Generic;

namespace Clones
{
    public class StackItem<Type>
    {
        public Type Value { get; set; }
        public StackItem<Type> Next { get; set; }
        public StackItem<Type> Previous { get; set; }
    }

    public class LinkedStack<Type>
    {
        StackItem<Type> head;
        StackItem<Type> tail;

        public void CreateLinkedStack(Type value)
        {
            head = tail = new StackItem<Type> { Value = value, Next = null, Previous = null };
        }

        public void Push(Type value)
        {
            if (head == null)
                head = tail = new StackItem<Type> { Value = value, Next = null, Previous = null };
            else
            {
                var item = new StackItem<Type> { Value = value, Next = null, Previous = tail };
                tail.Next = item;
                tail = item;
            }
        }

        public Type Pop()
        {
            Type popedItem = tail.Value;
            //if (tail.Previous == null)
             //   tail = head = null;
           // else
           // {
                tail = tail.Previous;
                tail.Next = null;
            //}
            return popedItem;
        }

        public Type Last()
        {
            if (tail == null)
                return default(Type);
            return tail.Value;
        }

        public void Clone(LinkedStack<Type> clone)
        {
            head = clone.head;
            tail = clone.tail;
        }
    }

    public class Clone
    {
        public LinkedStack<string> LearnedProgramms = new LinkedStack<string>();
        public LinkedStack<string> RolledBackProgramms = new LinkedStack<string>();

        public Clone(Clone clone)
        {
            LearnedProgramms.Clone(clone.LearnedProgramms);
            RolledBackProgramms.Clone(clone.RolledBackProgramms);
        }

        public Clone()
        {
            LearnedProgramms.CreateLinkedStack("basic");
            RolledBackProgramms = new LinkedStack<string>();
        }

        public void RollBack()
        {
            RolledBackProgramms.Push(LearnedProgramms.Pop());
        }

        public void Relearn()
        {
            LearnedProgramms.Push(RolledBackProgramms.Pop());
        }

        public void Learn(string program)
        {
            LearnedProgramms.Push(program);
        }

        public string Check()
        {
            if (LearnedProgramms.Last() == null)
                return "basic";
            return LearnedProgramms.Last();
        }
    }

	public class CloneVersionSystem : ICloneVersionSystem
	{
        List<Clone> clones = new List<Clone>();

		public string Execute(string query)
		{
            var str = query.Split(' ');
            if (clones.Count < int.Parse(str[1]))
                clones.Add(new Clone());
            switch (str[0])
            {
                case "rollback":                  
                    clones[int.Parse(str[1])-1].RollBack();
                    return null;
                case "learn":
                    clones[int.Parse(str[1])-1].Learn(str[2]);
                    return null;
                case "relearn":
                    clones[int.Parse(str[1])-1].Relearn();
                    return null;
                case "clone":
                    clones.Add(new Clone(clones[int.Parse(str[1])-1]));
                    return null;
                case "check":
                    return clones[int.Parse(str[1])-1].Check();
            }
			return null;
		}
	}
}


