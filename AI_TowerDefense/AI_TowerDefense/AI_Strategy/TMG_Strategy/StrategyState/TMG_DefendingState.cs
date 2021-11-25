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
            return _DecidePositionToDeployTowerInRow(defendLane, rowIndex);
            //return _PickTowerPositionInRowAccordingToPrecedence(defendLane, rowIndex);
            //return _DecideRandomTowerPositionInRow(defendLane, rowIndex);
        }

        private int _DecidePositionToDeployTowerInRow(PlayerLane defendLane, int rowIndex)
        {
            if (defendLane.SoldierCount() > 0)
            {
                return _PickTowerXAccordingToSoldierDensity(defendLane, rowIndex);
            }
            else
            {
                return _PickTowerXAccordingToDefaultPrecedence(defendLane, rowIndex);
            }
        }

        // Returns a column based on which side of the lane has more soldiers
        // Left side with more soldiers uses precedence rule 2 0 4 6
        // Right side with more soldiers uses precedence rule 4 6 2 0
        private static int _PickTowerXAccordingToSoldierDensity(PlayerLane defendLane, int rowIndex)
        {
            // Counts soldier in both sides of lane
            int leftSideCounter = 0;
            int rightSideCounter = 0;
            for (int j = 0; j < PlayerLane.HEIGHT - 1; j++)
            {
                for (int i = 0; i < PlayerLane.WIDTH - 1; i++)
                {
                    if (defendLane.GetCellAt(i, j).Unit != null && defendLane.GetCellAt(i, j).Unit.Type == "S")
                    {
                        if (i > (PlayerLane.WIDTH - 1) / 2)
                        {
                            rightSideCounter++;
                        }
                        else
                        {
                            // Middle lane is accounted for left side
                            leftSideCounter++;
                        }
                    }
                }
            }

            // Decide which precedence rule to use according to
            // the number of soldiers in each side
            if (leftSideCounter > rightSideCounter)
            {
                // spawn left side with preference for inner lanes
                // precedence: 2, 0, 4, 6
                if (defendLane.GetCellAt(2, rowIndex).Unit == null)
                {
                    return 2;
                }
                else if (defendLane.GetCellAt(0, rowIndex).Unit == null)
                {
                    return 0;
                }
                else if (defendLane.GetCellAt(4, rowIndex).Unit == null)
                {
                    return 4;
                }
                else
                {
                    return 6;
                }
            }
            else
            {
                // spawn right side with preference for inner lanes
                // precedence: 4, 6, 2, 0
                if (defendLane.GetCellAt(4, rowIndex).Unit == null)
                {
                    return 4;
                }
                else if (defendLane.GetCellAt(4, rowIndex).Unit == null)
                {
                    return 6;
                }
                else if (defendLane.GetCellAt(2, rowIndex).Unit == null)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
        }

        // Returns a column position following rule of precedence 2, 4, 0, 6
        // Inner lanes first
        private int _PickTowerXAccordingToDefaultPrecedence(PlayerLane defendLane, int rowIndex)
        {

            int x = 2;
            if (_ValidTowerX(x) && defendLane.GetCellAt(x, rowIndex).Unit == null)
            {
                return x;
            }

            x = 4;
            if (_ValidTowerX(x) && defendLane.GetCellAt(x, rowIndex).Unit == null)
            {
                return x;
            }

            x = 0;
            if (_ValidTowerX(x) && defendLane.GetCellAt(x, rowIndex).Unit == null)
            {
                return x;
            }

            // defaults to 6;
            x = 6;
            return x;

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
