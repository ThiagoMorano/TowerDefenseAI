using GameFramework;
using System;

namespace AI_Strategy
{
    /*
     * This class derives from Soldier and provides a new move method. Your assignment should
     * do the same - but with your own movement strategy.
     */
    public class ThiagoSoldier : Soldier
    {
        int previousHealth = -1;

        public ThiagoSoldier(Player player, PlayerLane lane, int x) : base(player, lane, x)
        {
            previousHealth = health;
        }

        public override void Move()
        {
            if (speed > 0 && posY < PlayerLane.HEIGHT - 1)
            {
                if (_IsCellSafeZone(posY + 1))
                {
                    _MoveForward();
                }
                else
                {
                    if (_ShouldMoveForward())
                    {
                        _MoveForward();
                    }
                    else
                    {
                        _MoveBackwards();
                    }
                }
            }
        }

        private bool _IsCellSafeZone(int y)
        {
            return y < PlayerLane.HEIGHT_OF_SAFETY_ZONE;
        }


        // Only moves foward when backed by another soldier
        private bool _ShouldMoveForward()
        {
            bool isBacked = lane.GetCellAt(posX, System.Math.Max(0, posY - 1)).Unit != null;
            bool hasOtherThreeSoldiersAround = _GetNumberOfAdjacentSoldiers() >= 3;

            return isBacked || hasOtherThreeSoldiersAround || _IsBeingAttacked() || _AreReachableLanesFree();
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


        private bool _MoveBackwards()
        {
            return MoveTo(posX, posY - 1);
        }
    }
}
