using GameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI_Strategy
{
    interface TMG_IState
    {
        void DeployTowers(Player player, PlayerLane defendLane, int currentTurn);
        void DeploySoldiers(Player player, PlayerLane attackLane, int currentTurn);
    }
}
