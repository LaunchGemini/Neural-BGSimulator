using System.Collections.Generic;

namespace BGSimulator.Model
{
    public class BobsTavern : ITavern
    {
        public int Round { get; set; } = 1;
        public Pool Pool { get; set; }

        public IMinion CreateGolden(Player player, IEnumerable<IMinion> tripple)
        {
            return Pool.CreateGolden(tripple);
        }

        public void Mulligen(Player player)
        {
            Pool.Return(player.ShopOffer);
        }

        pu