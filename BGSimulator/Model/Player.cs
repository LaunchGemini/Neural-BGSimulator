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
        const int PLAYE