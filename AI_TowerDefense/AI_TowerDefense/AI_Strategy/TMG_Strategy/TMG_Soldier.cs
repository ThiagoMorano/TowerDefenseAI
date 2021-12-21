using GameFramework;
using System;

namespace AI_Strategy
{
    public class TMG_Soldier : Soldier
    {
        int previousHealth = -1;

        public TMG_Soldier(Player player, PlayerLane lane, int x) : base(player, lane, x)
        {
            previousHealth = health;
        }

        public override void Move()
        {
            if (speed > 0 && posY < PlayerLane.HEIGHT - 1)
            {

                if (_ShouldMoveForward())
                {
                    _MoveForward();
                }
            }
            previousHealth = health;
        }

        // Defines the conditions in which a soldier can move
        private bool _ShouldMoveForward()
        {
            return _IsCellSafeZone(posY + 1)
                || _IsThereASoldierBehind()
                || _HasOtherTwoSoldiersAround()
                || _IsBeingAttacked()
                || _AreReachableLanesFree();
        }

        private bool _IsCellSafeZone(int y)
        {
            return y < PlayerLane.HEIGHT_OF_SAFETY_ZONE;
        }

        private bool _IsThereASoldierBehind()
        {
            return lane.GetCellAt(posX, System.Math.Max(0, posY - 1)).Unit != null;
        }


        private bool _HasOtherTwoSoldiersAround()
        {
            return _GetNumberOfAdjacentSoldiers() >= 2;
        }

        private int _GetNumberOfAdjacentSoldiers()
        {
            int friendsCounter = 0;
            for (int i = posX - 1; i <= posX + 1; i++)
            {
                for (int j = posY - 1; j <= posY + 1; j++)
                {
                    if (i == posX && j == posY) continue;
                    if (i < 0 || i > PlayerLane.WIDTH - 1) continue;
                    if (j < 0 || j > PlayerLane.HEIGHT - 1) continue;


                    if (lane.GetCellAt(i, j).Unit != null && lane.GetCellAt(i, j).Unit.Type == "T")
                    {
                        friendsCounter++;
                    }
                }
            }
            return friendsCounter;
        }

        private bool _IsBeingAttacked()
        {
            return health < previousHealth;
        }

        private bool _AreReachableLanesFree()
        {
            for (int rowIndex = PlayerLane.HEIGHT - 1; rowIndex > posY; rowIndex = rowIndex - 2)
            {
                for (int i = posX - 1; i <= posX + 1; i++)
                {
                    if (i < 0 || i > PlayerLane.WIDTH - 1) continue;


                    if (lane.GetCellAt(i, rowIndex).Unit != null && lane.GetCellAt(i, rowIndex).Unit.Type == "T")
                    {
                        return false;
                    }
                }
            }
            return true; ;
        }


        private bool _MoveForward()
        {
            return MoveTo(posX, posY + 1);
        }
    }
}
