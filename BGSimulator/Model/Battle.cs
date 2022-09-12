﻿using System;
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

            attackerBoard.ApplyAuras();
            defenderBoard.ApplyAuras();

            while (!attackerBoard.IsEmpty && !defenderBoard.IsEmpty)
            {
                PrintBoardState(attackerBoard, defenderBoard);

                attackerBoard.Attack(defenderBoard);

                var temp = attackerBoard;
                attackerBoard = defenderBoard;
                defenderBoard = temp;
            }

            int attackerDamage = attackerBoard.PlayedMinions.Select(m => m.MinionTier.Tier).Sum();
            int defenderDamage = defenderBoard.PlayedMinions.Select(m => m.MinionTier.Tier).Sum();
            attackerBoard.Player.TakeDamage(defenderDamage, always: true);
            defenderBoard.Player.TakeDamage(attackerDamage, alwa