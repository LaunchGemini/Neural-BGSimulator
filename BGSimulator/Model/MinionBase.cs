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
        public Keywords Keywords { get; set; } = Keyw