using GameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI_Strategy
{
    class TMG_AttackingState : TMG_IState
    {
        private int SOLDIER_COST = 2;
        int ATTACK_PIVOT_LEFT = 0;
        int ATTACK_PIVOT_RIGHT = PlayerLane.WIDTH - 2;


        public void DeploySoldiers(Player player, PlayerLane attackLane, int currentTurn)
        {
            // decide what corner to attack
            int pivotPosition = _GetAttackPivot(attackLane);

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

        private int _GetAttackPivot(PlayerLane attackLane)
        {
            int countPivotLeft = 0;
            int countPivotRight = 0;

            // scan the two farthest lanes in each side
            for (int rowIndex = PlayerLane.HEIGHT - 1; rowIndex > PlayerLane.HEIGHT_OF_SAFETY_ZONE; rowIndex = rowIndex - 2)
            {
                // left side
                if (attackLane.GetCellAt(ATTACK_PIVOT_LEFT, rowIndex).Unit != null && attackLane.GetCellAt(ATTACK_PIVOT_LEFT, rowIndex).Unit.Type == "T")
                {
                    countPivotLeft++;
                }
                if (attackLane.GetCellAt(ATTACK_PIVOT_LEFT + 2, rowIndex).Unit != null && attackLane.GetCellAt(ATTACK_PIVOT_LEFT + 2, rowIndex).Unit.Type == "T")
                {
                    countPivotLeft++;
                }

                // right side
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
            
            // Chooses the side with fewer towers
            if (countPivotLeft < countPivotRight)
            {
                return ATTACK_PIVOT_LEFT;
            }
            else
            {
                // Defaults to attacking in the right side if the number of towers are the same
                return ATTACK_PIVOT_RIGHT;
            }
        }

        public void DeployTowers(Player player, PlayerLane defendLane, int currentTurn)
        {
            // Attack state focuses on deploying soldiers
        }
    }
}
