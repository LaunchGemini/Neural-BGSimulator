using System;
using System.Collections.Generic;

namespace BGSimulator.Model
{
    public interface IMinion : ICard
    {
        int Attack { get; set; }
        Attribute Attributes { get; set; }
        int CurrentHealth { get; }
        int CurrentAttack { get; }
        int Health { get; set; }
        bool IsDead { get; }
        int Level { get; set; }
        MinionTier MinionTier { get; set; }
        MinionType MinionType { get; set; }
        int NumberOfCopies { get; set; }
        Action<TriggerParams> OnAttack { get; set; }
        Action<TriggerParams> OnBattlefieldChanged { get; set; 