using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BGSimulator.Utils.RandomUtils;

namespace BGSimulator.Model
{
    public class Player
    {

        public Action<Player> OnDeath = delegate { };
        const int MAX_HAND_SIZE = 10;
        const int MAX_SHOP_LEVEL = 6;
        const int PLAYER_MAX_HEALTH = 40;

        public Player()
        {
            Board = new Board() { Player = this };
        }

        public Board Board { get; set; }
        public ITavern BobsTavern { get; set; }
        public IBrain Brain { get; set; }
        public Player CurrentMatch { get; set; }
        public int Gold { get; set; } = 3;
        public List<ICard> Hand { get; set; } = new List<ICard>();
        public int Health { get; set; } = PLAYER_MAX_HEALTH;
        public bool IsDead { get => Health <= 0; }
        public Player LastMatch { get; set; }
        public List<string> MinionsPlayedThisGame { get; private set; } = new List<string>();
        public int MissingHealth { get { return PLAYER_MAX_HEALTH - Health; } }
        publi