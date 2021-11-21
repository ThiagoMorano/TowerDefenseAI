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
        private int EARLY_GAME_THRESHOLD = 300;
        private int EARLY_DEFENSE = 11;
        private int MID_DEFENSE = 9;
        private int END_GAME_THRESHOLD = 800;
        private int END_DEFENSE = 7;


        private static Random random = new Random();
        private bool _isInDefensiveState = false;

        private int _turnCounter = 0;


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

            if (player.Gold > Tower.COSTS)
            {
                int deployX = 0;
                int deployY = 3;

                this._DecideTowerDeployment(out deployX, out deployY);

                DebugLoger.Log(String.Format("Deploy at ({0}, {1})", deployX, deployY), true);

                if (defendLane.GetCellAt(deployX, deployY).Unit == null)
                {
                    Tower tower = player.BuyTower(defendLane, deployX, deployY);
                    if (tower != null)
                    {
                        DebugLoger.Log(String.Format("Tower at ({0}, {1}) has been deployed", deployX, deployY), true);
                    }
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
                if (ValidTowerX(x) && defendLane.GetCellAt(x, y).Unit == null)
                {
                    return x;
                }
            }
            return 0;
        }


        /*
         * example strategy for deploying Soldiers based on random placement and budget.
         * Yours should be better!
         */
        public override void DeploySoldiers()
        {
            if (_isInDefensiveState) return;

            int round = 0;
            while (player.Gold > 5 && round < 5)
            {
                round++;
                Boolean positioned = false;
                int count = 0;
                while (!positioned && count < 10)
                {
                    count++;
                    int x = 1;
                    int y = 0;
                    if (attackLane.GetCellAt(x, y).Unit == null)
                    {
                        positioned = true;
                        Soldier soldier = player.BuySoldier(attackLane, x);
                    }
                }
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


        bool ValidTowerPosition(int x, int y)
        {
            if (ValidTowerX(x) && ValidTowerY(y))
            {
                return true;
            }
            return false;
        }

        bool ValidTowerX(int x)
        {
            if (x % 2 == 0 && // x even
                x >= 0 && x < PlayerLane.WIDTH)
            {
                return true;
            }
            return false;
        }

        bool ValidTowerY(int y)
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
