using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlackJack.Models
{
    public class Deck
    {
        public string deck_id { get; set; }
        public int remaining { get; set; }
        public bool shuffled { get; set; }
        public bool success { get; set; }

    }
}