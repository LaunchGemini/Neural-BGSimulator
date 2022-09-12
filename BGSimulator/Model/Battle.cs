using System;
using System.Linq;
using static BGSimulator.Utils.RandomUtils;

namespace BGSimulator.Model
{
    public class Battle
    {
        public int Round { get; set; }
        public Player PlayerA { 