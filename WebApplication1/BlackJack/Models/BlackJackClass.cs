using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace BlackJack.Models
{
    public class BlackJackClass
    {
        public object GetMyDeck()
        {
            string url = "https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1";
            var client = new WebClient();
            var content = client.DownloadString(url);
            var serializer = new JavaScriptSerializer();
            var jsonContent = serializer.Deserialize<object>(content);

            return jsonContent;
        }

    }
}