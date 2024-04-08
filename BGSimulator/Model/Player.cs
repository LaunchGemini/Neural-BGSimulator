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
        public string Name { get; set; }
        public int ShopLevel { get; set; } = 1;
        public int ShopLevelupPrice { get; set; }
        public List<IMinion> ShopOffer { get; set; } = new List<IMinion>();
        public int Top { get; set; }
        public bool Freeze { get; set; }

        public void AddToHand(ICard minion)
        {
            if (Hand.Count == MAX_HAND_SIZE)
                return;

            Hand.Add(minion);
        }

        public Adapt ChooseAdapt()
        {
            var adaptOptions = GetAdaptOptions();
            return adaptOptions.First();
        }

        public void ChooseDiscover(List<IMinion> minions)
        {
            if (Hand.Count == MAX_HAND_SIZE)
            {
                Pool.Instance.Return(minions);
                return;
            }

            var minion = ChooseRandomMinion(minions);
            minions.Remove(minion);
            Pool.Instance.Return(minions);
            Hand.Add(minion);
        }

        public void PlayRound()
        {
            RoundStart();
            Board.RoundStart();
            bool done = false;

            while (!done)
            {
                bool canBuy = Gold > 2 && Hand.Count < MAX_HAND_SIZE;
                bool canLevel = ShopLevel < MAX_SHOP_LEVEL && Gold >= ShopLevelupPrice;
                bool canBuyAndLevel = canBuy && ShopLevel < MAX_SHOP_LEVEL && Gold > ShopLevelupPrice + 2;
                bool canRoll = Gold > 0;
                bool tableFull = Board.IsFull;
                bool shopHasDouble = ShopOffer.GroupBy(m => m.Name).Any(g => g.Count() > 1);
                bool canMakeTriple = Hand.Concat(Board.PlayedMinions).Concat(ShopOffer).GroupBy(m => m.Name).Any(g => g.Count() > 2);
                bool canSell = Hand.Any() || Board.PlayedMinions.Any();
                bool canPlay = Hand.Any();
                bool shopOfferEmpty = !ShopOffer.Any();

                if (Simulation.Instance.Round == 2)
                {
                    LevelUp();
                    break;
                }

                if (canBuyAndLevel)
                {
                    LevelUp();
                    continue;
                }

                if(canRoll && shopOfferEmpty)
                {
                    Roll();
                    continue;
                }

                if (canBuy)
                {
                    Buy();
                    continue;
                }

                if (canRoll)
                {
                    Roll();
                    continue;
                }

                if (canPlay)
                {
                    if(tableFull)
                        Sell();
                    PlayHand();
                    continue;
                }


                done = !canBuy && !canLevel;
            }

            TryFreeze();

     