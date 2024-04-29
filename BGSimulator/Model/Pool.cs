
﻿using BGSimulator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static BGSimulator.Utils.RandomUtils;

namespace BGSimulator.Model
{
    public class Pool
    {
        private List<IMinion> allMinions;
        private List<IMinion> poolMinions;

        private const int DISCOVER_MINIONS = 3;

        public IMinion CreateGolden(IEnumerable<IMinion> tripple)
        {
            var baseMinion = GetFreshCopy(tripple.First().Name, forGolden: true);

            foreach (var minion in tripple)
            {
                baseMinion.Attack += minion.Attack;
                baseMinion.Health += minion.Health;
                baseMinion.Attributes &= minion.Attributes;
            }


            Action<TriggerParams> onPlay = (tp) =>
            {
                ICard card = new MinionFromTierCard();
                Action<TriggerParams> discover = (_tp) =>
                {

                    var tier = _tp.Player.ShopLevel == 6 ? 6 : _tp.Player.ShopLevel + 1;
                    _tp.Player.ChooseDiscover(Discover(m => m.MinionTier.Tier == tier));
                };
                card.OnPlayed += discover;

                tp.Player.AddToHand(card);
            };

            baseMinion.OnPlayed += onPlay;


            return baseMinion;
        }

        private static Pool instance;

        public static Pool Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Pool();
                }

                return instance;
            }
        }

        private Pool()
        {
            Initialize();
        }

        private void Initialize()
        {
            allMinions = new List<IMinion>()
            {
                //Tier 1
                new MinionBase() { MinionType = MinionType.Beast, Name = "Alleycat", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], Keywords = Keywords.BattleCry, OnPlayed = (tp) =>  { tp.Board.Summon("Tabbycat",tp.Index, golden: tp.Activator.Level == 2); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Tabbycat", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Dire Wolf Alpha", Cost = 2, Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[1], OnBoardChanged = (tp) => { tp.Board.BuffAdjacent(tp.Activator, attack: 1, aura: true); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Mecharoo", Attack = 1, Health = 1, Keywords = Keywords.DeathRattle, MinionTier = MinionTier.Ranks[1], OnDeath = (tp) =>  { tp.Board.Summon("Jo-E Bot",tp.Index, Direction.InPlace, golden: tp.Activator.Level == 2); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Jo-E Bot", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Micro Machine", Cost = 2, Attack = 1, Health = 2, MinionTier = MinionTier.Ranks[1], OnTurnStart = (tp) => { tp.Board.Buff(tp.Activator, minion: tp.Activator, attack: 1); } },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Murloc Tidecaller", Attack = 1, Health = 2, MinionTier = MinionTier.Ranks[1], OnMinionSummon = (tp) => { if(tp.Summon.MinionType == MinionType.Murloc) { tp.Board.Buff(tp.Activator, minion: tp.Activator, attack: 1); } } },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Murloc Tidehunter", Cost = 2, Attack = 2, Health = 1, MinionTier = MinionTier.Ranks[1], Keywords = Keywords.BattleCry, OnPlayed = (tp) =>  { tp.Board.Summon("Murloc Scout",tp.Index, golden: tp.Activator.Level == 2); } },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Murloc Scout", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Righteous Protector", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], Attributes = Attribute.Taunt | Attribute.DivineShield },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Rockpool Hunter", Cost = 2, Attack = 2, Health = 3, MinionTier = MinionTier.Ranks[1], Keywords = Keywords.Targeted | Keywords.BattleCry, ValidTargets = MinionType.Murloc, OnPlayed =(tp) => { if(tp.Target == null) return; tp.Board.Buff(tp.Activator, minion: tp.Target, attack: 1, health: 1); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Selfles s Hero", Attack = 2, Health = 1, MinionTier = MinionTier.Ranks[1], OnDeath = (tp) => { for (int i = 0; i < tp.Activator.Level; i++) { tp.Board.BuffRandom(null, attributes: Attribute.DivineShield); } } },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Voidwalker", Attack = 1, Health = 3 , MinionTier = MinionTier.Ranks[1], Attributes = Attribute.Taunt },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Vulgar Homunculus", Cost = 2, Attack = 2,Health = 4 , MinionTier = MinionTier.Ranks[1], Attributes = Attribute.Taunt, Keywords = Keywords.BattleCry, OnPlayed = (tp) => { tp.Player.TakeDamage(2 * tp.Activator.Level); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Wrath Weaver", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], OnMinionSummon = (tp) => { if (tp.Summon.MinionType.HasFlag(MinionType.Demon)) { tp.Player.TakeDamage(tp.Activator.Level); } } },

                //Tier 2
                new MinionBase() { MinionType = MinionType.Mech, Name = "Annoy-o-Tron", Cost = 2, Attack = 1, Health = 2, Attributes = Attribute.Taunt | Attribute.DivineShield, MinionTier = MinionTier.Ranks[2] },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Harvest Golem", Cost = 3, Attack = 2, Health = 3, MinionTier = MinionTier.Ranks[2], Keywords = Keywords.DeathRattle, OnDeath = (tp) => { tp.Board.Summon("Damaged Golem", tp.Index, Direction.InPlace, golden: tp.Activator.Level == 2); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Damaged Golem", Attack = 2, Health = 1, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Kaboom Bot", Cost = 3, Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[2], OnDeath = (tp) => { tp.Board.ShootRandomMinion(4, tp.Activator.Level); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Kindly Grandmother", Cost = 2, Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[2], Keywords = Keywords.DeathRattle, OnDeath = (tp) => { tp.Board.Summon("Big Bad Wolf", tp.Index, Direction.InPlace, golden: tp.Activator.Level == 2); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Big Bad Wolf", Attack = 3, Health = 2, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Metaltooth Leaper", Cost = 3, Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[2], Keywords = Keywords.BattleCry, OnPlayed = (tp) => { tp.Board.BuffAllOfType(tp.Activator, MinionType.Mech, 2); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Mounted Raptor", Cost = 3, Attack = 3, Health = 2, MinionTier = MinionTier.Ranks[2], Keywords = Keywords.DeathRattle, OnDeath = (tp) => { for(int i = 0; i < tp.Activator.Level; i++) { tp.Board.Summon(GetRandomMinions(m => m.Cost == 1 && m.PoolMinion), tp.Index, Direction.InPlace, golden: tp.Activator.Level == 2); } } },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Murloc Warleader", Cost = 3, Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[2], OnApplyAura = (tp) => { tp.Board.BuffAllOfType(tp.Activator, MinionType.Murloc,attack: 2, aura: true); }  },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Nathrezim Overseer", Cost = 3, Attack = 2, Health = 4 , MinionTier = MinionTier.Ranks[2], Keywords = Keywords.Targeted | Keywords.BattleCry, ValidTargets = MinionType.Demon, OnPlayed = (tp) => { if(tp.Target ==null) return; tp.Board.Buff(tp.Activator, tp.Target,2,2); } },

                //new MinionBase() { MinionType = MinionType.Amalgam, Name = "Nightmare Amalgam", Attack = 3,Health = 4 , MinionTier = MinionTier.Ranks[2]},

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Old Murk-Eye", Cost = 4, Rarity = Rarity.Legendary, Attack = 2, Health = 4, MinionTier = MinionTier.Ranks[2], OnBattlefieldChanged = (tp) => { int attack = tp.Board.PlayedMinions.Concat(tp.RivalBoard?.PlayedMinions ?? Enumerable.Empty<IMinion>()).Where(m => m.MinionType == MinionType.Murloc && m != tp.Activator).Count(); tp.Board.Buff(tp.Activator, tp.Activator, attack: attack, aura: true); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Pogo-Hopper", Cost = 1, Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[2], OnPlayed = (tp) => { var count = tp.Player.MinionsPlayedThisGame.Where(m => m == tp.Activator.Name).Count() - 1; tp.Board.Buff(null, tp.Activator, count * 2 * tp.Activator.Level,count * 2 * tp.Activator.Level); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Rat Pack", Cost = 3, Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[2], Keywords = Keywords.DeathRattle, OnDeath = (tp) => { tp.Board.Summon("Rat", tp.Index, Direction.InPlace, tp.Activator.Attack, golden: tp.Activator.Level == 2); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Rat", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Scavenging Hyena", Cost = 2, Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[2], OnMinionDied = (tp) => { if(tp.Target.MinionType == MinionType.Beast) { tp.Board.Buff(tp.Activator, tp.Activator, 2, 1); } } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Shielded Minibot", Cost = 2, Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[2], Attributes = Attribute.DivineShield },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Spawn of N'Zoth", Cost = 3, Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[2], Keywords = Keywords.DeathRattle, OnDeath = (tp) => { tp.Board.BuffAllOfType(tp.Activator, MinionType.All, 1, 1); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Zoobot", Cost = 3, Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[2], Keywords = Keywords.BattleCry, OnPlayed = (tp) => { tp.Board.BuffRandomUnique(tp.Activator, new List<MinionType>(){ MinionType.Murloc, MinionType.Beast, MinionType.Dragon }, 1, 1); } },

                ////Tier 3
                new MinionBase() { MinionType = MinionType.Mech, Name = "Cobalt Guardian", Cost = 5, Attack = 6, Health = 3, MinionTier = MinionTier.Ranks[3], OnMinionSummon = (tp) => { if(tp.Summon == null || tp.Summon.MinionType != MinionType.Mech) return; tp.Board.Buff(tp.Activator, tp.Activator, attributes: Attribute.DivineShield); } },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Coldlight Seer", Cost = 3, Attack = 2, Health = 3, MinionTier = MinionTier.Ranks[3], Keywords = Keywords.BattleCry, ValidTargets = MinionType.Murloc, OnPlayed = (tp) => { tp.Board.BuffAllOfType(tp.Activator, type:tp.Activator.ValidTargets, health: 2); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Crowd Favorite", Cost = 4, Attack = 4, Health = 4, MinionTier = MinionTier.Ranks[3], OnMinionSummon = (tp) => { if(tp.Summon == null || !tp.Summon.Keywords.HasFlag(Keywords.BattleCry)) return; tp.Board.Buff(tp.Activator, tp.Activator, 1, 1); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Crystalweaver", Cost = 4, Attack = 5, Health = 4, MinionTier = MinionTier.Ranks[3], Keywords = Keywords.Targeted | Keywords.BattleCry, ValidTargets = MinionType.Demon, OnPlayed = (tp) => { if(tp.Target == null) return; tp.Board.Buff(tp.Activator, tp.Target, 1, 1); } },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Floating Watcher", Cost = 5, Attack = 4, Health = 4 , MinionTier = MinionTier.Ranks[3], OnPlayerDamage = (tp) => { tp.Board.Buff(tp.Activator,tp.Activator,2 ,2); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Houndmaster", Cost = 4, Attack = 4, Health = 3, MinionTier = MinionTier.Ranks[3], Keywords = Keywords.Targeted | Keywords.BattleCry, ValidTargets = MinionType.Beast, OnPlayed = (tp) => { if(tp.Target == null) return; tp.Board.Buff(tp.Activator, tp.Target, 2, 2); } },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Imp Gang Boss", Cost = 3, Attack = 2, Health = 4 , MinionTier = MinionTier.Ranks[3], OnDamage = (tp) => { tp.Board.Summon("Imp", tp.Index, golden: tp.Activator.Level == 2); } },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Imp", Attack = 1, Health = 1 , MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Infested Wolf", Cost = 4, Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[3], Keywords = Keywords.DeathRattle, OnDeath = (tp) => { tp.Board.Summon("Spider", tp.Index, Direction.InPlace, 2, golden: tp.Activator.Level == 2); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Spider", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Khadgar", Cost = 2, Rarity = Rarity.Legendary, Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[3], OnApplyAura = (tp) => { tp.Board.AddAura(tp.Activator, AuraType.Summon);  } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Pack Leader", Cost = 3, Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[3], OnMinionSummon = (tp) => { if(tp.Summon == null || tp.Summon.MinionType != MinionType.Beast) return; tp.Board.Buff(tp.Activator, tp.Summon, 3); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Phalanx Commander", Cost = 5, Attack = 4, Health = 5, MinionTier = MinionTier.Ranks[3], OnApplyAura = (tp) => { tp.Board.BuffAllWithAttribute(tp.Activator, Attribute.Taunt, 2, 0, aura: true); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Piloted Shredder", Cost = 4, Attack = 4, Health = 3, MinionTier = MinionTier.Ranks[3], Keywords = Keywords.DeathRattle, OnDeath = (tp) => { for(int i = 0; i < tp.Activator.Level; i++) { tp.Board.Summon(GetRandomMinions(m => m.Cost == 2 && m.PoolMinion), tp.Index, Direction.InPlace ); } } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Psych-o-Tron", Cost = 5, Attack = 3, Health = 4, MinionTier = MinionTier.Ranks[3], Attributes = Attribute.DivineShield | Attribute.Taunt },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Replicating Menace", Cost = 4, Attack = 3, Health = 1, MinionTier = MinionTier.Ranks[3], Keywords = Keywords.Magnetic, OnPlayed = (tp) => { tp.Board.TryMagnet(tp.Activator, tp.Index); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Screwjank Clunker", Cost = 4, Attack = 2, Health = 5, MinionTier = MinionTier.Ranks[3], Keywords = Keywords.Targeted | Keywords.BattleCry, ValidTargets = MinionType.Mech, OnPlayed = (tp) => { if(tp.Target == null) return; tp.Board.Buff(tp.Activator, tp.Target, 2, 2); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Shifter Zerus", Cost = 1, Rarity = Rarity.Legendary, Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[3], OnTurnStart = (tp) => { if(tp.Activator.Contained != null) Return(tp.Activator.Contained); var minion = PullRandomMinion(6); tp.Activator.Contained = minion; } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Soul Juggler", Cost = -1, Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[3], OnMinionDied = (tp) => { if(tp.Target.MinionType != MinionType.Demon) return;  tp.Board.ShootRandomMinion(3); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "The Beast", Cost = 6, Attack = 9, Rarity = Rarity.Legendary, Health = 7, MinionTier = MinionTier.Ranks[3], Keywords = Keywords.DeathRattle, OnDeath = (tp) => { tp.RivalBoard.Summon("Finkle Einhorn", tp.RivalBoard.PlayedMinions.Count - 1); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Finkle Einhorn", Attack = 3, Health = 3, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Tortollan Shellraiser", Cost = 4, Attack = 2, Health = 6, MinionTier = MinionTier.Ranks[3], Keywords = Keywords.DeathRattle, OnDeath = (tp) => { tp.Board.BuffRandom(tp.Activator, 1, 1); } },

                ////Tier 4
                new MinionBase() { MinionType = MinionType.Mech, Name = "Annoy-o-Module", Cost = 4, Attack = 2, Health = 4, MinionTier = MinionTier.Ranks[4], Attributes = Attribute.Taunt | Attribute.DivineShield, Keywords = Keywords.Magnetic, ValidTargets = MinionType.Mech, OnPlayed = (tp) => { tp.Board.TryMagnet(tp.Activator, tp.Index); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Bolvar, Fireblood", Cost = 5, Rarity = Rarity.Legendary, Attack = 1, Health = 7, MinionTier = MinionTier.Ranks[4], Attributes = Attribute.DivineShield, OnMinionLostDivineShield = (tp) => { tp.Board.Buff(tp.Activator, tp.Activator, 2); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Cave Hydra", Cost = 3, Attack = 2, Health = 4, MinionTier = MinionTier.Ranks[4], OnAttack = (tp) => { tp.RivalBoard.CleaveAttack(tp.Activator, tp.Target); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Defender of Argus", Cost = 4, Attack = 2, Health = 3, MinionTier = MinionTier.Ranks[4], Keywords = Keywords.BattleCry, ValidTargets = MinionType.All, OnPlayed = (tp) => { tp.Board.BuffAdjacent(tp.Activator, 1, 1, Attribute.Taunt); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Festeroot Hulk", Cost = 5, Attack = 2, Health = 7, MinionTier = MinionTier.Ranks[4], OnMinionAttacked = (tp) => { tp.Board.Buff(tp.Activator, tp.Activator, 1, 0); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Iron Sensei", Cost = 3, Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[4], OnTurnEnd = (tp) => { tp.Board.BuffRandom(tp.Activator, 2, 2, type: MinionType.Mech); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Mechano-Egg", Cost = 5, Attack = 0, Health = 5, MinionTier = MinionTier.Ranks[4], Keywords = Keywords.DeathRattle, OnDeath = (tp) => { tp.Board.Summon("Robosaur", tp.Index, Direction.InPlace, golden: tp.Activator.Level == 2); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Menagerie Magician", Cost = 5, Attack = 4, Health = 4, MinionTier = MinionTier.Ranks[4], Keywords = Keywords.Targeted | Keywords.BattleCry, OnPlayed = (tp) => { tp.Board.BuffRandomUnique(tp.Activator, new List<MinionType>(){ MinionType.Beast, MinionType.Dragon, MinionType.Murloc }, 2, 2); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Piloted Sky Golem", Cost = 6, Attack = 6, Health = 4, MinionTier = MinionTier.Ranks[4], OnDeath = (tp) => { for (int i = 0; i < tp.Activator.Level; i++) { tp.Board.Summon(GetRandomMinions(m => m.Cost == 4 && m.PoolMinion), tp.Index, Direction.InPlace, golden: tp.Activator.Level == 2); } } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Security Rover", Cost = 6, Attack = 2, Health = 6, MinionTier = MinionTier.Ranks[4], OnDamage = (tp) => {  tp.Board.Summon("Guard Bot", tp.Index, golden: tp.Activator.Level == 2); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Guard Bot", Attack = 2, Health = 6, MinionTier = MinionTier.Ranks[1], Attributes = Attribute.Taunt, PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Siegebreaker", Cost = 7, Attack = 5,Health = 8 , MinionTier = MinionTier.Ranks[4], Attributes = Attribute.Taunt, OnApplyAura = (tp) => { tp.Board.BuffAllOfType(tp.Activator, MinionType.Demon, attack: 1, aura: true); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "The Boogeymonster", Cost = 8, Rarity = Rarity.Legendary, Attack = 6, Health = 7, MinionTier = MinionTier.Ranks[4], OnAttack = (tp) => {if (!tp.Killed) return; tp.Board.Buff(tp.Activator,tp.Activator, 2, 2); } },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Toxfin", Cost = 1, Attack = 1, Health = 2, MinionTier = MinionTier.Ranks[4] , Keywords = Keywords.Targeted | Keywords.BattleCry, ValidTargets = MinionType.Murloc, OnPlayed = (tp) => { if(tp.Target ==null) return; tp.Board.Buff(tp.Activator, tp.Target, attributes: Attribute.Poison); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Virmen Sensei", Cost = 5, Attack = 4, Health = 5, MinionTier = MinionTier.Ranks[4], Keywords = Keywords.Targeted | Keywords.BattleCry, ValidTargets = MinionType.Beast, OnPlayed = (tp) => { if(tp.Target ==null) return; tp.Board.Buff(tp.Activator, tp.Target, 2 , 2); } },

                ////Tier 5
                new MinionBase() { MinionType = MinionType.Demon, Name = "Annihilan Battlemaster", Cost = 8, Attack = 3, Health = 1 , MinionTier = MinionTier.Ranks[5], Keywords = Keywords.BattleCry, OnPlayed = (tp) => { tp.Board.Buff(tp.Activator, tp.Activator, 0, tp.Player.MissingHealth); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Baron Rivendare", Cost = 4, Rarity = Rarity.Legendary, Attack = 1, Health = 7, MinionTier = MinionTier.Ranks[5], OnApplyAura = (tp) => { tp.Board.AddAura(tp.Activator, AuraType.Deathrattle); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Brann Bronzebeard", Cost = 3, Rarity = Rarity.Legendary, Attack = 2, Health = 4, MinionTier = MinionTier.Ranks[5], OnApplyAura = (tp) => { tp.Board.AddAura(tp.Activator, AuraType.BattleCry); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Goldrinn, the Great Wolf", Cost = 8, Rarity = Rarity.Legendary, Attack = 4, Health = 4, MinionTier = MinionTier.Ranks[5], Keywords = Keywords.DeathRattle, OnDeath = (tp) => { tp.Board.BuffAllOfType(tp.Activator, MinionType.Beast, 4, 4); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Ironhide Direhorn", Cost = 7, Attack = 7, Health = 7, MinionTier = MinionTier.Ranks[5], OnAttack = (tp) => { if(!tp.Overkill) return; tp.Board.Summon("Ironhide Runt", tp.Index, golden: tp.Activator.Level == 2); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Ironhide Runt", Cost = 1, Attack = 5, Health = 5, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Junkbot", Cost = 5, Attack = 1, Health = 5, MinionTier = MinionTier.Ranks[5], OnMinionDied = (tp) => { if(tp.Target.MinionType != MinionType.Mech) return; tp.Board.Buff(tp.Activator, tp.Activator, 2, 2); } },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "King Bagurgle", Cost = 6, Rarity = Rarity.Legendary, Attack = 6, Health = 3, MinionTier = MinionTier.Ranks[5], OnPlayed = (tp) => { tp.Board.BuffAllOfType(tp.Activator, MinionType.Murloc, 2, 2); }, OnDeath = (tp) => {tp.Board.BuffAllOfType(tp.Activator, MinionType.Murloc, 2, 2); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Lightfang Enforcer", Cost = 6, Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[5], OnTurnEnd = (tp) => { tp.Board.BuffRandomUnique(tp.Activator, new List<MinionType>(){ MinionType.Beast, MinionType.Demon, MinionType.Mech, MinionType.Murloc }, 2, 1);  } },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Mal'Ganis", Cost = 9, Rarity = Rarity.Legendary, Attack = 9, Health = 7, MinionTier = MinionTier.Ranks[5], Keywords = Keywords.PlayerImmunity , OnApplyAura = (tp) => { tp.Board.BuffAllOfType(tp.Activator, MinionType.Demon, 2, 2, aura: true); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Robosaur", Attack = 8, Health = 8, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Primalfin Lookout", Cost = 3, Attack = 3, Health = 2, MinionTier = MinionTier.Ranks[5], Keywords = Keywords.BattleCry, OnPlayed = (tp) => { for(int i = 0; i < tp.Activator.Level; i++) { if(!tp.Board.Controls(MinionType.Murloc, tp.Activator)) return; tp.Player.ChooseDiscover(Discover((m) => m.MinionType == MinionType.Murloc && m.Name != tp.Activator.Name)); } } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Sated Threshadon", Cost = 7, Attack = 5, Health = 7, MinionTier = MinionTier.Ranks[5], Keywords = Keywords.DeathRattle, OnDeath = (tp) => { tp.Board.Summon("Primalfin", tp.Index, Direction.InPlace, 3, golden: tp.Activator.Level == 2); } },

                new MinionBase() { MinionType = MinionType.Murloc, Name = "Primalfin", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Savannah Highmane", Cost = 6, Attack = 6, Health = 5, MinionTier = MinionTier.Ranks[5], Keywords = Keywords.DeathRattle, OnDeath = (tp) => { tp.Board.Summon("Hyena", tp.Index, Direction.InPlace, golden: tp.Activator.Level == 2); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Hyena", Attack = 2, Health = 2, MinionTier = MinionTier.Ranks[1], PoolMinion = false },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Strongshell Scavenger", Cost = 4, Attack = 2, Health = 3, MinionTier = MinionTier.Ranks[5], Keywords = Keywords.BattleCry, OnPlayed = (tp) => { tp.Board.BuffAllWithAttribute(tp.Activator, Attribute.Taunt, 2, 2); } },

                new MinionBase() { MinionType = MinionType.Demon, Name = "Voidlord", Cost = 9, Attack = 3,Health =9 , MinionTier = MinionTier.Ranks[5], Keywords = Keywords.DeathRattle, OnDeath = (tp) => { tp.Board.Summon("Voidwalker", tp.Index, Direction.InPlace, 3, golden: tp.Activator.Level == 2); } },

                ////Tier 6
                new MinionBase() { MinionType = MinionType.Mech, Name = "Foe Reaper 4000", Cost = 8, Rarity = Rarity.Legendary, Attack = 6, Health = 9, MinionTier = MinionTier.Ranks[6], OnAttack = (tp) => { tp.RivalBoard.CleaveAttack(tp.Activator, tp.Target); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Gentle Megasaur", Cost = 4, Attack = 5, Health = 4, MinionTier = MinionTier.Ranks[6], Keywords = Keywords.BattleCry, OnPlayed = (tp) => { var adapt = tp.Player.ChooseAdapt(); tp.Board.BuffAdapt(adapt, tp.Index); } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Plant", Attack = 1, Health = 1, MinionTier = MinionTier.Ranks[1], },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Ghastcoiler", Cost = 6, Attack = 7, Health = 7, MinionTier = MinionTier.Ranks[6], Keywords = Keywords.DeathRattle, OnDeath = (tp) => { var minions = GetRandomMinions(m => m.Keywords.HasFlag(Keywords.DeathRattle) && m.Name != tp.Activator.Name, 2); for(int i = 0; i < tp.Activator.Level; i++) { tp.Board.Summon(minions, tp.Index, Direction.InPlace, golden: tp.Activator.Level == 2); } } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Kangor's Apprentice", Cost = 9, Attack = 3, Health = 6, MinionTier = MinionTier.Ranks[6], Keywords = Keywords.DeathRattle, OnDeath = (tp) => { tp.Board.SummonFromGraveyard(MinionType.Mech, tp.Index, amount: 2 * tp.Activator.Level); } },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Maexxna", Cost = 6, Rarity = Rarity.Legendary, Attack = 2, Health = 8, MinionTier = MinionTier.Ranks[6], Attributes = Attribute.Poison },

                new MinionBase() { MinionType = MinionType.Beast, Name = "Mama Bear", Cost = 8, Attack = 4, Health = 4, MinionTier = MinionTier.Ranks[6], OnMinionSummon = (tp) => { if(tp.Summon == null || tp.Summon.MinionType != MinionType.Beast) return; tp.Board.Buff(tp.Activator, tp.Summon, 4, 4); } },

                new MinionBase() { MinionType = MinionType.Mech, Name = "Sneed's Old Shredder", Cost = 8, Rarity = Rarity.Legendary, Attack = 5, Health = 7, MinionTier = MinionTier.Ranks[6], Keywords = Keywords.DeathRattle, OnDeath = (tp) => { for(int i = 0; i < tp.Activator.Level; i++) { tp.Board.Summon(GetRandomMinions(m => m.Rarity == Rarity.Legendary && m.Name != tp.Activator.Name).First().Name, tp.Index, Direction.InPlace); } } },

                new MinionBase() { MinionType = MinionType.Neutral, Name = "Zapp Slywick", Cost = 8, Rarity = Rarity.Legendary, Attack = 7, Health = 10, MinionTier = MinionTier.Ranks[6],Keywords = Keywords.SpecialAttack, Attributes = Attribute.WindFury, OnAquireTargets = (tp) => { return tp.RivalBoard.GetMinionWithMinAttack();  } },
            };

            BuildPool();
        }

        private List<IMinion> Discover(Func<IMinion, bool> predicate)
        {
            lock (poolMinions)
            {
                var minions = GetMinionsByPredicate(predicate, true);
                minions.Shuffle();
                var discover = minions.DistinctBy(m => m.Name).Take(DISCOVER_MINIONS).ToList();

                foreach (var d in discover)
                {
                    poolMinions.Remove(d);
                }

                return discover;
            }
        }

        private List<IMinion> GetMinionsByPredicate(Func<IMinion, bool> predicate, bool fromPool = false)
        {
            List<IMinion> minions;

            if (!fromPool)
            {
                minions = allMinions.Where(m => predicate(m)).ToList();
            }
            else
            {
                minions = poolMinions.Where(m => predicate(m)).ToList();
            }
            return minions;
        }

        private List<IMinion> GetRandomMinions(Func<IMinion, bool> predicate, int amount = 1)
        {
            var minions = allMinions.Where(m => predicate(m)).ToList();
            minions.Shuffle();
            return minions.Take(amount).ToList();
        }

        public List<IMinion> Roll(int amount, int maxRank)
        {
            List<IMinion> rollOffer = new List<IMinion>();
            for (int i = 0; i < amount; i++)
            {
                rollOffer.Add(PullRandomMinion(maxRank));
            }

            return rollOffer;
        }

        private IMinion PullRandomMinion(int maxRank)
        {
            int rank = GetRandomRank(maxRank);
            lock (poolMinions)
            {
                var minionsFromRank = poolMinions.Where(m => m.MinionTier.Tier <= rank).ToList();
                minionsFromRank.Shuffle();
                var minion = minionsFromRank[rank];
                poolMinions.Remove(minion);
                return minion;
            }
        }

        public void Return(IMinion minion)
        {
            if (!minion.PoolMinion)
                return;

            lock (poolMinions)
            {
                if (minion.Level == 1)
                {
                    poolMinions.Add(GetFreshCopy(minion.Name));
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        poolMinions.Add(GetFreshCopy(minion.Name));
                    }
                }
            }
        }

        public void Return(List<IMinion> lastOffer)
        {
            lock (poolMinions)
            {
                poolMinions.AddRange(lastOffer);
            }
        }

        private int GetRandomRank(int maxRank)
        {
            int rank = RandomNumber(1, maxRank + 1);

            return rank;
        }

        public void BuildPool()
        {
            poolMinions = new List<IMinion>();

            foreach (var minion in allMinions)
            {
                if (!minion.PoolMinion)
                    continue;
                for (int i = 0; i < minion.MinionTier.NumberOfCopies; i++)
                {
                    poolMinions.Add(minion.Clone());
                }
            }
        }

        public IMinion GetFreshCopy(string minionName, bool forGolden = false)
        {
            var clone = allMinions.First(m => m.Name == minionName)?.Clone();

            if (forGolden)
            {
                clone.Attack = 0;
                clone.Health = 0;
                clone.Level = 2;
            }

            return clone;
        }
    }
}