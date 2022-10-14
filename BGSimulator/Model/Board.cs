
﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static BGSimulator.Utils.RandomUtils;

namespace BGSimulator.Model
{
    public class Board
    {
        private const int BOARD_SIZE = 7;

        public Board()
        {
            Initialize();
        }

        public List<IMinion> Graveyard { get; set; }
        public bool IsEmpty { get { return PlayedMinions.Count == 0; } }
        public bool IsFull { get { return PlayedMinions.Count == BOARD_SIZE; } }
        public ObservableCollection<IMinion> PlayedMinions { get; set; }
        public Player Player { get; set; }

        private Dictionary<IMinion, AuraType> BoardAuras = new Dictionary<IMinion, AuraType>();
        private int NextAttacker { get; set; }

        public void HookEvents()
        {
            PlayedMinions.CollectionChanged += PlayedMinions_CollectionChanged;
            PlayedMinions_CollectionChanged();
        }

        public Board RivalBoard { get; set; }

        public void Attack(Board defenderBoard)
        {
            IMinion attackingMinion = GetNextAttacker();

            if (attackingMinion.Attack == 0)
            {
                return;
            }

            int attacks = attackingMinion.Attributes.HasFlag(Attribute.WindFury) ? 2 : 1;

            IMinion defendingMinion;
            for (int i = 0; i < attacks; i++)
            {
                if (!attackingMinion.Keywords.HasFlag(Keywords.SpecialAttack))
                {
                    defendingMinion = defenderBoard.GetRandomDefender();
                }
                else
                {
                    defendingMinion = attackingMinion.OnAquireTargets(new TriggerParams() { Activator = attackingMinion, Board = this, Player = Player, RivalBoard = RivalBoard });
                }

                if (defendingMinion == null)
                    break;

                Console.WriteLine(string.Format(@"{0} {1} Is Attacking {2} {3}", Player.Name, attackingMinion.ToString(), defenderBoard.Player.Name, defendingMinion.ToString()));

                MinionAttack(attacker: attackingMinion, defender: defendingMinion);
            }
        }

        public void ApplyAuras()
        {
            foreach (var minion in PlayedMinions)
            {
                minion.OnApplyAura(new TriggerParams() { Activator = minion, Board = this, Player = Player, RivalBoard = RivalBoard });
            }
        }

        public void Buff(IMinion buffer, IMinion minion, int attack = 0, int health = 0, Attribute attributes = Attribute.None, Action<TriggerParams> deathRattle = null, bool aura = false)
        {
            int amount = buffer?.Level ?? 1;
            attack *= amount;
            health *= amount;

            if (!aura)
            {
                minion.Attack += attack;
                minion.Health += health;
                minion.Attributes |= attributes;
            }
            else
            {
                minion.AddAura(buffer, new Buff() { Attack = attack, Health = health, Attribute = attributes });
            }

            if (deathRattle != null)
            {
                minion.Keywords |= Keywords.DeathRattle;
                minion.OnDeath += deathRattle;
            }

        }

        private void RemoveAuras(IMinion minion)
        {
            RemoveMinionAura(minion);

            if (BoardAuras.ContainsKey(minion))
                BoardAuras.Remove(minion);
        }

        private void RemoveMinionAura(IMinion minion)
        {
            foreach (var m in PlayedMinions)
            {
                m.RemoveAura(minion);
            }
        }

        public void BuffAdapt(Adapt adapt, int index)
        {
            int attack = 0;
            int health = 0;
            Attribute attr = Attribute.None;

            Action<TriggerParams> deathRattle = null;

            switch (adapt)
            {
                case Adapt.DeathRattle:
                    deathRattle = (tp) => { tp.Board.Summon("Plant", tp.Index, Direction.InPlace, 2); };
                    break;
                case Adapt.DivineShield:
                    attr |= Attribute.DivineShield;
                    break;

                case Adapt.OneOne:
                    attack++;
                    health++;
                    break;

                case Adapt.Poison:
                    attr |= Attribute.Poison;
                    break;

                case Adapt.Windfury:
                    attr |= Attribute.WindFury;
                    break;

                case Adapt.Taunt:
                    attr |= Attribute.Taunt;
                    break;

                case Adapt.ThreeAttack:
                    attack += 3;
                    break;

                case Adapt.ThreeHealth:
                    health += 3;
                    break;
            }

            BuffAllOfType(null, MinionType.Murloc, attack, health, attr, deathRattle);
        }

        public void BuffAdjacent(IMinion minion, int attack = 0, int health = 0, Attribute attributes = Attribute.None, bool aura = false)
        {
            if (aura)
            {
                RemoveMinionAura(minion);
            }

            var adjacents = GetAdjacentMinions(minion);

            foreach (var adj in adjacents)
            {
                Buff(minion, adj, attack, health, attributes, aura: aura);
            }
        }

        public void BuffAllOfType(IMinion buffer, MinionType type, int attack = 0, int health = 0, Attribute attributes = Attribute.None, Action<TriggerParams> deathRattle = null, bool aura = false)
        {
            foreach (var minion in PlayedMinions.Where(m => m != buffer))
            {
                if ((type & minion.MinionType) != 0)
                {
                    Buff(buffer, minion, attack, health, attributes, deathRattle, aura);
                }
            }
        }

        public void BuffAllWithAttribute(IMinion buffer, Attribute attribute, int attack, int health, bool aura = false)
        {
            foreach (var minion in PlayedMinions)
            {
                if (minion.Attributes.HasFlag(attribute))
                {
                    Buff(buffer, minion, attack, health, aura: aura);
                }
            }
        }

        public void PlayerTookDamage()
        {
            foreach (var minion in PlayedMinions)
            {
                minion.OnPlayerDamage(new TriggerParams() { Activator = minion, Board = this, Player = Player });
            }
        }

        public void BuffRandom(IMinion buffer, int attack = 0, int health = 0, Attribute attributes = Attribute.None, MinionType type = MinionType.All)
        {
            var buffee = GetRandomMinion(type);
            if (buffee == null)
                return;

            Buff(buffer, buffee, attack, health, attributes);
        }

        public void BuffRandomUnique(IMinion buffer, List<MinionType> buffedTypes, int attack, int health)
        {
            var uniqueBuffed = new List<IMinion>();

            foreach (var type in buffedTypes)
            {
                var buffed = GetRandomMinion(type, uniqueBuffed);
                if (buffed != null)
                    uniqueBuffed.Add(buffed);
            }

            foreach (var buffed in uniqueBuffed)
            {
                Buff(buffer, buffed, attack, health);
            }
        }

        public void CleaveAttack(IMinion activator, IMinion target)
        {
            var adjacent = GetAdjacentMinions(target);
            foreach (var minion in adjacent)
            {
                MinionTakeDamage(minion, activator.CurrentAttack);
            }
        }

        public Board Clone()
        {
            var board = this.MemberwiseClone() as Board;
            board.PlayedMinions = new ObservableCollection<IMinion>(PlayedMinions.Select(m => m.Clone()).ToList());
            board.BoardAuras.Clear();
            board.Graveyard = new List<IMinion>();
            return board;
        }

        public bool Controls(MinionType murloc, IMinion exclude = null)
        {
            return PlayedMinions.Any(m => (m.MinionType & MinionType.Murloc) != 0 && (exclude == null || m != exclude));
        }

        public IMinion GetNextAttacker()
        {
            if (NextAttacker >= PlayedMinions.Count)
                NextAttacker = 0;
            var minion = PlayedMinions[NextAttacker];
            NextAttacker++;
            return minion;
        }

        public IMinion GetRandomDefender()
        {
            if (IsEmpty)
                return null;

            var taunters = PlayedMinions.Where(m => m.Attributes.HasFlag(Attribute.Taunt)).ToArray();
            if (taunters.Any())
            {
                return taunters[RandomNumber(0, taunters.Length)];
            }

            return PlayedMinions[RandomNumber(0, PlayedMinions.Count)];
        }

        public IMinion GetRandomMinion(MinionType type = MinionType.All, List<IMinion> excludes = null)
        {
            var minions = PlayedMinions.Except(excludes ?? new List<IMinion>()).Where(m => (m.MinionType & type) != 0).ToArray();

            if (!minions.Any())
                return null;

            var minion = minions[RandomNumber(0, minions.Length)];
            return minion;
        }

        public List<IMinion> GetValidTargets(MinionType validTargets)
        {
            return PlayedMinions.Where(m => (validTargets & m.MinionType) != 0).ToList();
        }

        public void MinionAttack(IMinion attacker, IMinion defender)
        {
            var defenderResult = RivalBoard.MinionTakeDamage(defender, attacker.CurrentAttack);
            var attackerResult = MinionTakeDamage(attacker, defender.CurrentAttack);

            attacker.OnAttack(new TriggerParams() { Activator = attacker, Target = defender, Board = this, RivalBoard = RivalBoard, Overkill = defenderResult.overkill, Killed = defenderResult.killed });
            OnMinionAttacked(attacker);

            ClearDeaths();
            RivalBoard.ClearDeaths();
        }