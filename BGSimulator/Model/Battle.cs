using System;
using System.Linq;
using static BGSimulator.Utils.RandomUtils;

namespace BGSimulator.Model
{
    public class Battle
    {
        public int Round { get; set; }
        public Player PlayerA { get; set; }
        public Player PlayerB { get; set; }
        public Board PlayerABoard { get; set; }
        public Board PlayerBBoard { get; set; }

        public void Start()
        {

            var attacker = FirstAttacker();
            var defender = attacker.CurrentMatch;

            //clone again because we want to save the state at the start of the fight.
            var attackerBoard = attacker.Board.Clone();
            var defenderBoard = defender.Board.Clone();

            attackerBoard.RivalBoard = defenderBoard;
            defenderBoard.RivalBoard = attackerBoard;

            attackerBoard.HookEvents();
            defenderBoard.HookEvents();

            attackerBoard.ApplyA