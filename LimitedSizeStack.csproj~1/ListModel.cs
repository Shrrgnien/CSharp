using System;
using System.Collections.Generic;

namespace TodoApplication
{
    public class ListModel<TItem>
    {
        public enum  ActionMade{
            Pushed,
            Poped
        };

        public List<TItem> Items { get; }
        public int Limit;
        public LimitedSizeStack<TItem> ItemsStack;
        public LimitedSizeStack<ActionMade> ActionStack;
        public LimitedSizeStack<int> IndexStack;

        public ListModel(int limit)
        {
            Items = new List<TItem>();
            Limit = limit;
            ItemsStack = new LimitedSizeStack<TItem>(limit);
            ActionStack = new LimitedSizeStack<ActionMade>(limit);
            IndexStack = new LimitedSizeStack<int>(limit);
        }

        public void AddItem(TItem item)
        {
            Items.Add(item);
            ItemsStack.Push(item);
            ActionStack.Push(ActionMade.Pushed);
            IndexStack.Push(Items.IndexOf(item));
        }

        public void RemoveItem(int index)
        {
            ItemsStack.Push(Items[index]);
            ActionStack.Push(ActionMade.Poped);
            IndexStack.Push(index);
            Items.RemoveAt(index);
        }

        public bool CanUndo()
        {
            return ItemsStack.Count > 0;
        }

        public void Undo()
        {
            if (CanUndo())
            {
                var action = ActionStack.Pop();
                switch (action)
                {
                    case ActionMade.Pushed:
                        Items.Remove(ItemsStack.Pop());
                        IndexStack.Pop();
                        break;
                    case ActionMade.Poped:
                        Items.Insert(IndexStack.Pop(), ItemsStack.Pop());
                        break;

                }
            }
        }
    }
}
