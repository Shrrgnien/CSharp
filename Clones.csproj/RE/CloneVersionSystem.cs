using System;
using System.Collections;
using System.Collections.Generic;

namespace Clones
{
    public class SingleTypeStack<Type>
    {
        List<Type> singleTypeStack = new List<Type>();

        public Type Pop()
        {
            var result = singleTypeStack[singleTypeStack.Count - 1];
            singleTypeStack.RemoveAt(singleTypeStack.Count - 1);
            return result;
        }

        public void Push(Type item)
        {
            singleTypeStack.Add(item);
        }

        public Type Last()
        {
            return singleTypeStack[Count() - 1];
        }

        public int Count()
        {
            return singleTypeStack.Count;
        }

        public SingleTypeStack<Type> CopySingleTypeStack()
        {
            var result = new SingleTypeStack<Type>();
            foreach (var e in singleTypeStack)
                result.Push(e);
            return result;
        }

    }

    /*
 learn ci pi. Обучить клона с номером ci по программе pi.
rollback ci. Откатить последнюю программу у клона с номером ci.
relearn ci. Переусвоить последний откат у клона с номером ci.
clone ci. Клонировать клона с номером ci.
check ci. Вернуть программу, которой клон с номером ci владеет и при этом усвоил последней. Если клон владеет только базовыми знаниями, верните "basic".
Выполнение команды check должно возвращать имя программы. Выполнение остальных команд должно возвращать null.
*/

    public class Clone
    {
        public SingleTypeStack<string> LearnedProgramms = new SingleTypeStack<string>();
        public SingleTypeStack<string> RolledBackProgramms = new SingleTypeStack<string>();

        public Clone(Clone clone)
        {
            LearnedProgramms = clone.LearnedProgramms.CopySingleTypeStack();
            RolledBackProgramms = clone.RolledBackProgramms.CopySingleTypeStack();
        }

        public Clone()
        {
           // LearnedProgramms = new SingleTypeStack<string>();
           // RolledBackProgramms = new SingleTypeStack<string>();
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
            if (LearnedProgramms.Count() != 0)
                return LearnedProgramms.Last();
            return "basic";
        }
    }

	public class CloneVersionSystem : ICloneVersionSystem
	{
        List<Clone> Clones = new List<Clone>();

		public string Execute(string query)
		{

            var str = query.Split(' ');
            if (Clones.Count < int.Parse(str[1]))
                Clones.Add(new Clone());
                switch (str[0])
            {
                case "rollback":                  
                    Clones[int.Parse(str[1])-1].RollBack();
                    return null;
                case "learn":
                    Clones[int.Parse(str[1])-1].Learn(str[2]);
                    return null;
                case "relearn":
                    Clones[int.Parse(str[1])-1].Relearn();
                    return null;
                case "clone":
                    Clones.Add(new Clone(Clones[int.Parse(str[1])-1]));
                    return null;
                case "check":
                    return Clones[int.Parse(str[1])-1].Check();
            }
			return null;
		}
	}
}


