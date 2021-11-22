using System;
using GameFramework;
using System.Collections.Generic;

namespace AI_Strategy
{
    /*
     * very simple example strategy based on random placement of units.
     */
    public class ThiagoStrategy : AbstractStrategy
    {
        // State machine attributes
        private int EARLY_GAME_THRESHOLD = 300;
        private int END_GAME_THRESHOLD = 800;
        private bool _isInDefensiveState = false;

        // Defending attributes
        private int EARLY_DEFENSE = 11;
        private int MID_DEFENSE = 9;
        private int END_DEFENSE = 7;

        // Attacking attributes
        private int SOLDIER_COST = 2;
        int ATTACK_PIVOT_LEFT = 0;
        int ATTACK_PIVOT_RIGHT = PlayerLane.WIDTH - 2;
        

        private int _turnCounter = 0;
        private static Random random = new Random();


        public ThiagoStrategy(PlayerLane defendLane, PlayerLane attackLane, Player player) : base(defendLane, attackLane, player)
        {
            _EvaluateState();
        }

        private void _EvaluateState()
        {
            _isInDefensiveState = !_isDefenseSet(_turnCounter);
        }

        private bool _isDefenseSet(int currentTurn)
        {
            for (int rowIndex = PlayerLane.HEIGHT - 1; rowIndex > PlayerLane.HEIGHT - _GetMaxDefenseRow(currentTurn); rowIndex = rowIndex - 2)
            {
                if (!_isRowFull(rowIndex))
                {
                    return false;
                }
            }
            return true;
        }

        private int _GetMaxDefenseRow(int currentTurn)
        {
            if (_turnCounter < EARLY_GAME_THRESHOLD)
            {
                return EARLY_DEFENSE;
            }
            else if (_turnCounter < END_GAME_THRESHOLD)
            {
                return MID_DEFENSE;
            }
            else
            {
                return END_DEFENSE;
            }
        }


        public override void DeployTowers()
        {
            _turnCounter++;
            _EvaluateState();
            if (!_isInDefensiveState) return;

            int count = 0;
            while (count < 20) // deploys max 20 towers in a turn
            {
                if (player.Gold > Tower.COSTS)
                {
                    count++;

                    int deployX = 0;
                    int deployY = 3;

                    this._DecideTowerDeployment(out deployX, out deployY);

                    //DebugLoger.Log(String.Format("Deploy at ({0}, {1})", deployX, deployY), true);

                    if (defendLane.GetCellAt(deployX, deployY).Unit == null)
                    {
                        Tower tower = player.BuyTower(defendLane, deployX, deployY);
                        if (tower != null)
                        {
                            //DebugLoger.Log(String.Format("Tower at ({0}, {1}) has been deployed", deployX, deployY), true);
                        }
                    }
                }
                else
                {
                    break;
                }
            }

        }

        private void _DecideTowerDeployment(out int deployX, out int deployY)
        {
            for (int rowIndex = PlayerLane.HEIGHT - 1; rowIndex > PlayerLane.HEIGHT_OF_SAFETY_ZONE; rowIndex = rowIndex - 2)
            {
                if (!_isRowFull(rowIndex))
                {
                    deployX = _DecideRandomTowerPositionInRow(rowIndex);
                    deployY = rowIndex;
                    return;
                }
            }

            deployX = 0;
            deployY = 0;
        }

        private bool _isRowFull(int rowIndex)
        {
            for (int i = 0; i < PlayerLane.WIDTH; i = i + 2)
            {
                if (defendLane.GetCellAt(i, rowIndex).Unit == null)
                {
                    return false;
                }
            }
            return true;
        }

        private int _DecideRandomTowerPositionInRow(int rowIndex)
        {
            _EvaluateState();
            int count = 0;
            while (count < 20)
            {
                count++;
                int x = random.Next(PlayerLane.WIDTH);
                int y = rowIndex;
                if (_ValidTowerX(x) && defendLane.GetCellAt(x, y).Unit == null)
                {
                    return x;
                }
            }
            return 0;
        }



        public override void DeploySoldiers()
        {
            if (_isInDefensiveState) return;

            // decide what corner to attack
            int pivotPosition = _GetAttackPivot();

            if (player.Gold > SOLDIER_COST * 2) // always spawns in pairs
            {
                for (int i = 0; i < 2; i++)
                {
                    int x = pivotPosition + i;
                    int y = 0;
                    if (attackLane.GetCellAt(x, y).Unit == null)
                    {
                        Soldier soldier = player.BuySoldier(attackLane, x);
                    }
                }
            }
        }

        private int _GetAttackPivot()
        {
            int countPivotLeft = 0;
            int countPivotRight = 0;

            // scan two relevant tower lanes in each corner:
            // select tower lane
            // iterate through lane counting towers
            for (int rowIndex = PlayerLane.HEIGHT - 1; rowIndex > PlayerLane.HEIGHT_OF_SAFETY_ZONE; rowIndex = rowIndex - 2)
            {
                if (attackLane.GetCellAt(ATTACK_PIVOT_LEFT, rowIndex).Unit != null && attackLane.GetCellAt(ATTACK_PIVOT_LEFT, rowIndex).Unit.Type == "T")
                {
                    countPivotLeft++;
                }
                if (attackLane.GetCellAt(ATTACK_PIVOT_LEFT + 2, rowIndex).Unit != null && attackLane.GetCellAt(ATTACK_PIVOT_LEFT + 2, rowIndex).Unit.Type == "T")
                {
                    countPivotLeft++;
                }

                if (attackLane.GetCellAt(ATTACK_PIVOT_RIGHT - 1, rowIndex).Unit != null && attackLane.GetCellAt(ATTACK_PIVOT_RIGHT - 1, rowIndex).Unit.Type == "T")
                {
                    countPivotRight++;
                }
                if (attackLane.GetCellAt(ATTACK_PIVOT_RIGHT + 1, rowIndex).Unit != null && attackLane.GetCellAt(ATTACK_PIVOT_RIGHT + 1, rowIndex).Unit.Type == "T")
                {
                    countPivotRight++;
                }
            }

            //DebugLoger.Log(String.Format("TowerCountLeft = {0}; TowerCountRight = {1}", countPivotLeft, countPivotRight), true);
            if (countPivotLeft < countPivotRight)
            {
                return ATTACK_PIVOT_LEFT;
            }
            else
            {
                return ATTACK_PIVOT_RIGHT;

            }
        }

        /*
         * called by the game play environment. The order in which the array is returned here is
         * the order in which soldiers will plan and perform their movement.
         *
         * The default implementation does not change the order. Do better!
         */
        public override List<Soldier> SortedSoldierArray(List<Soldier> unsortedList)
        {
            return unsortedList;
        }


        bool _ValidTowerPosition(int x, int y)
        {
            if (_ValidTowerX(x) && _ValidTowerY(y))
            {
                return true;
            }
            return false;
        }

        bool _ValidTowerX(int x)
        {
            if (x % 2 == 0 && // x even
                x >= 0 && x < PlayerLane.WIDTH)
            {
                return true;
            }
            return false;
        }

        bool _ValidTowerY(int y)
        {
            if (y % 2 != 0 &&
                y >= PlayerLane.HEIGHT_OF_SAFETY_ZONE && y < PlayerLane.HEIGHT) // y odd and leavin
            {
                return true;
            }
            return false;
        }

    }
}
