using GameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI_Strategy
{
    class TMG_DefendingState : TMG_IState
    {
        private static Random random = new Random();

        public void DeployTowers(Player player, PlayerLane defendLane, int currentTurn)
        {
            int count = 0;
            while (count < 20) // deploys max 20 towers in a turn
            {
                if (player.Gold > Tower.COSTS)
                {
                    count++;

                    int deployX = 0;
                    int deployY = 3;

                    this._DecideTowerDeployment(out deployX, out deployY, defendLane);

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

        private void _DecideTowerDeployment(out int deployX, out int deployY, PlayerLane defendLane)
        {
            for (int rowIndex = PlayerLane.HEIGHT - 1; rowIndex > PlayerLane.HEIGHT_OF_SAFETY_ZONE; rowIndex = rowIndex - 2)
            {
                if (!TMG_Strategy.IsRowFullWithTowers(defendLane, rowIndex))
                {
                    deployX = _GetColumnToDeployTowerInRow(defendLane, rowIndex);
                    deployY = rowIndex;
                    return;
                }
            }

            deployX = 0;
            deployY = 0;
        }

        private int _GetColumnToDeployTowerInRow(PlayerLane defendLane, int rowIndex)
        {
            return _DecideRandomTowerPositionInRow(defendLane, rowIndex);
        }

        private int _DecideRandomTowerPositionInRow(PlayerLane defendLane, int rowIndex)
        {
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
            if (y % 2 != 0 && // y odd 
                y >= PlayerLane.HEIGHT_OF_SAFETY_ZONE && y < PlayerLane.HEIGHT) // and outside of safety zone
            {
                return true;
            }
            return false;
        }



        public void DeploySoldiers(Player player, PlayerLane attackLane, int currentTurn)
        {
            // Defending state focuses on deploying towers
        }
    }
}
