using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlackJack.Models
{
    public class GameViewModel
    {
        public Deck newDeck { get; set; }

        public person player { get; set; }

        public person dealer { get; set; }

        public CardView cvm { get; set; }

        public string GameOver = "";
    }
}