using System;
using System.Collections.Generic;

namespace BGSimulator.Model
{
    public class MinionBase : CardBase, IMinion
    {
        public MinionBase()
        {
            damageTaken = 0;
        }

        private int damageTaken;

        public MinionType MinionType { get; set; }
        public int NumberOfCopies { get; set; }
        public MinionTier MinionTier { get; set; } = MinionTier.Ranks[1];
        public Attribute Attributes { get; set; } = Attribute.None;
        public Keywords Keywords { get; set; } = Keywords.None;
        public MinionType ValidTargets { get; set; } = MinionType.All;
        public int Level { get; set; } = 1;
        public int Health { get; set; }
        public int Attack { get; set; }

        public int CurrentHealth
        {
            get
            {
                int currHealth = Health;
                foreach (var buff in TempBuffs.Values)
                {
                    currHealth += buff.Health;
                }
                return currHealth - damageTaken;
            }
        }

        public int CurrentAttack
        {
            get
            {
                int currAttack = Attack;
                foreach (var buff in TempBuffs.Values)
    