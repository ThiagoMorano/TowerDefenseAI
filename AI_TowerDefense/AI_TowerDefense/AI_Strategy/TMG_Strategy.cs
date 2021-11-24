using System;
using GameFramework;
using System.Collections.Generic;

namespace AI_Strategy
{
    public class TMG_Strategy : AbstractStrategy
    {
        private const int _EARLY_GAME_TURN_THRESHOLD = 300;
        private const int _END_GAME_TURN_THRESHOLD = 800;

        private const int _AMOUNT_OF_EARLY_DEFENSE = 5;
        private const int _AMOUNT_OF_MID_DEFENSE = 4;
        private const int _AMOUNT_OF_END_DEFENSE = 3;

        private int _turnCounter = 0;

        private TMG_IState _currentState;
        private TMG_IState _defendingState;
        private TMG_IState _attackingState;


        public TMG_Strategy(PlayerLane defendLane, PlayerLane attackLane, Player player) : base(defendLane, attackLane, player)
        {
            _defendingState = new TMG_DefendingState();
            _attackingState = new TMG_AttackingState();

            _EvaluateState();
        }

        private void _EvaluateState()
        {
            if (_IsDefenseSet(_turnCounter))
            {
                _currentState = _attackingState;
            }
            else
            {
                _currentState = _defendingState;
            }
        }

        private bool _IsDefenseSet(int currentTurn)
        {
            for (int rowIndex = PlayerLane.HEIGHT - 1; rowIndex > _GetMaxDefenseLineIndex(currentTurn); rowIndex = rowIndex - 2)
            {
                if (!IsRowFullWithTowers(defendLane, rowIndex))
                {
                    return false;
                }
            }
            return true;
        }

        private int _GetMaxDefenseLineIndex(int currentTurn)
        {
            if (_turnCounter < _EARLY_GAME_TURN_THRESHOLD)
            {
                return PlayerLane.HEIGHT - 2 * _AMOUNT_OF_EARLY_DEFENSE;
            }
            else if (_turnCounter < _END_GAME_TURN_THRESHOLD)
            {
                return PlayerLane.HEIGHT - 2 * _AMOUNT_OF_MID_DEFENSE;
            }
            else
            {
                return PlayerLane.HEIGHT - 2 * _AMOUNT_OF_END_DEFENSE;
            }
        }

        public static bool IsRowFullWithTowers(PlayerLane defendLane, int rowIndex)
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


        public override void DeployTowers()
        {
            _turnCounter++;
            _EvaluateState();

            _currentState.DeployTowers(player, defendLane, _turnCounter);
        }


        public override void DeploySoldiers()
        {
            _currentState.DeploySoldiers(player, attackLane, _turnCounter);
        }


        public override List<Soldier> SortedSoldierArray(List<Soldier> unsortedList)
        {
            return unsortedList;
        }
    }
}
