using GameFramework;

namespace AI_Strategy
{
    /*
     * This class derives from Soldier and provides a new move method. Your assignment should
     * do the same - but with your own movement strategy.
     */
    public class ThiagoSoldier : Soldier
    {
        public ThiagoSoldier(Player player, PlayerLane lane, int x) : base(player, lane, x)
        {
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
                }
            }
        }

        // Only moves foward when backed by another soldier
        private bool _ShouldMoveForward()
        {
            return lane.GetCellAt(posX, System.Math.Max(0, posY - 1)).Unit != null;
        }

        private bool _MoveForward()
        {
            return MoveTo(posX, posY + 1);
        }

        private bool _IsCellSafeZone(int y)
        {
            return y < PlayerLane.HEIGHT_OF_SAFETY_ZONE;
        }
    }
}
