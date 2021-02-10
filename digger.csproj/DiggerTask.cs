using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

namespace Digger
{
    public class Terrain : ICreature
    {
        static string fileName = "Terrain.png";
        
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand { DeltaX = 0, DeltaY = 0 };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return conflictedObject.GetImageFileName() == "Digger.png";
        }

        public int GetDrawingPriority()
        {
            return 0;
        }

        public string GetImageFileName()
        {
            return fileName;
        }
    }

    public class Player : ICreature
    {
        static string fileName = "Digger.png"; 

        public CreatureCommand MoveLeft(int x, int y)
        {
            if(x !=0 && !IsASack(x - 1, y))
                return new CreatureCommand { DeltaX = -1, DeltaY = 0 };
            return new CreatureCommand { DeltaX = 0, DeltaY = 0 };
        }

        public CreatureCommand MoveRight(int x, int y)
        {
            if(x != Game.MapWidth - 1 && !IsASack(x + 1, y))
                return new CreatureCommand { DeltaX = 1, DeltaY = 0 };
            return new CreatureCommand { DeltaX = 0, DeltaY = 0 };
        }

        public CreatureCommand MoveUp(int x, int y)
        {
            if(y != 0 && !IsASack(x, y - 1))
                return new CreatureCommand { DeltaX = 0, DeltaY = -1 };
            return new CreatureCommand { DeltaX = 0, DeltaY = 0 };
        }

        public CreatureCommand MoveDown(int x, int y)
        {
            if(y != Game.MapHeight - 1 && !IsASack(x, y + 1))
                return new CreatureCommand { DeltaX = 0, DeltaY = 1 };
            return new CreatureCommand { DeltaX = 0, DeltaY = 0 };
        }

        public bool IsASack(int x, int y)
        {
            if (Game.Map[x, y] != null)
                return Game.Map[x, y].GetImageFileName() == "Sack.png";
            return false;
        }

        public CreatureCommand Act(int x, int y)
        {
            switch (Game.KeyPressed)
            {
                case Keys.A:
                    return MoveLeft(x,y);
                case Keys.S:
                    return MoveDown(x,y);
                case Keys.D:
                    return MoveRight(x,y);
                case Keys.W:
                    return MoveUp(x,y);
                case Keys.Left:
                    return MoveLeft(x,y);
                case Keys.Down:
                    return MoveDown(x,y);
                case Keys.Right:
                    return MoveRight(x,y);
                case Keys.Up:
                    return MoveUp(x,y);
                case Keys.L:
                    return MoveLeft(x,y);
            } 
            return new CreatureCommand { DeltaX = 0, DeltaY = 0 };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return conflictedObject.GetImageFileName() == "Monster.png";
        }

        public int GetDrawingPriority()
        {
            return -1;
        }

        public string GetImageFileName()
        {
            return fileName;
        }
    }

    public class Sack : ICreature
    {
        static readonly string fileName = "Sack.png";
        public int MovementDistance = 0;

        public CreatureCommand Act(int x, int y)
        {
            if (y + 1 < Game.MapHeight)
            {
                if (Game.Map[x, y + 1] == null)
                {
                    MovementDistance++;
                    return new CreatureCommand { DeltaX = 0, DeltaY = 1 };
                }
                if (Game.Map[x, y + 1].GetImageFileName() == "Digger.png")
                {
                    if (MovementDistance > 0)
                    {
                        Game.Map[x, y + 1] = null;
                        return new CreatureCommand { DeltaX = 0, DeltaY = 1};
                    }
                }
                if (Game.Map[x, y + 1].GetImageFileName() == "Monster.png")
                {
                    if (MovementDistance > 0)
                    {
                        Game.Map[x, y + 1] = null;
                        return new CreatureCommand { DeltaX = 0, DeltaY = 1, TransformTo = new Gold() };
                    }
                }
                if (MovementDistance > 1)
                    return new CreatureCommand { DeltaX = 0, DeltaY = 0, TransformTo = new Gold() };
                MovementDistance = 0;
            }
            if (MovementDistance > 1)
                return new CreatureCommand { DeltaX = 0, DeltaY = 0, TransformTo = new Gold() };
            return new CreatureCommand { DeltaX = 0, DeltaY = 0};
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return false;
        }

        public int GetDrawingPriority()
        {
            return 2;
        }

        public string GetImageFileName()
        {
            return fileName;
        }
    }

    public class Gold : ICreature
    {
        static string fileName = "Gold.png";

        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand { DeltaX = 0, DeltaY = 0 };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject.GetImageFileName() == "Digger.png")
                Game.Scores += 10;
            return conflictedObject.GetImageFileName() == "Digger.png" || conflictedObject.GetImageFileName() == "Monster.png";
        }

        public int GetDrawingPriority()
        {
            return 1;
        }

        public string GetImageFileName()
        {
            return fileName;
        }
    }

    public class Monster : ICreature
    {
        static readonly string fileName = "Monster.png";
        public int diggerPositionX = 0;
        public int diggerPositionY = 0;
        public bool isDiggerOnMap = false;

        public void FindDiggerOnMap()
        {
            isDiggerOnMap = false;
            for (int x=0; x<Game.MapWidth; x++)
                for(int y=0; y<Game.MapHeight; y++)
                    if (Game.Map[x, y] != null && Game.Map[x, y].GetImageFileName() == "Digger.png")
                    {
                        isDiggerOnMap = true;
                        diggerPositionX = x;
                        diggerPositionY = y;
                     }
        }
        
        public bool IsThereAnObstacle(int x, int y)
        {
            return (Game.Map[x, y] != null && (Game.Map[x, y].GetImageFileName() == "Terrain.png" 
                || Game.Map[x, y].GetImageFileName() == "Sack.png" || Game.Map[x, y].GetImageFileName() == "Monster.png"));
        }

        public CreatureCommand Act(int x, int y)
        {
            FindDiggerOnMap();
            if (isDiggerOnMap)
            {
                if(diggerPositionX > x && !IsThereAnObstacle(x + 1, y))
                    return new CreatureCommand { DeltaX = 1, DeltaY = 0 };
                if (diggerPositionX < x && !IsThereAnObstacle(x - 1, y))
                    return new CreatureCommand { DeltaX = -1, DeltaY = 0 };
                if (diggerPositionY > y && !IsThereAnObstacle(x, y + 1))
                    return new CreatureCommand { DeltaX = 0, DeltaY = 1 };
                if (diggerPositionY < y && !IsThereAnObstacle(x, y - 1))
                    return new CreatureCommand { DeltaX = 0, DeltaY = -1 };
            }
            return new CreatureCommand { DeltaX = 0, DeltaY = 0 };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return conflictedObject.GetImageFileName() == "Monster.png" || conflictedObject.GetImageFileName() == "Sack.png";
        }

        public int GetDrawingPriority()
        {
            return -50;
        }

        public string GetImageFileName()
        {
            return fileName;
        }
    }
}
