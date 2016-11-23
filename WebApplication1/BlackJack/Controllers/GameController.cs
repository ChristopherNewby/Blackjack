using BlackJack.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BlackJack.Controllers
{
    public class GameController : Controller
    {
        HttpClient client = new HttpClient();
        string url = "https://deckofcardsapi.com/api/deck";

        Deck currentDeck;

        CardView cvm = new CardView();
        person player = new person();
        person dealer = new person();

        GameViewModel gvm = new GameViewModel();


        public GameController()
        {
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }

        // GET: Game
        public async Task<ActionResult> Index()
        {
            HttpResponseMessage response = await client.GetAsync(url + "/new/shuffle/?deck_count=1");
            if (response.IsSuccessStatusCode)
            {
                var responseData = response.Content.ReadAsStringAsync().Result;
                currentDeck = JsonConvert.DeserializeObject<Deck>(responseData);
                gvm.newDeck = currentDeck;


                return View(gvm);
            }
            return View("Error");
        }

        public async Task<Card> DrawACard(string deck_id)
        {
            var deckId = Request.QueryString["deck_id"];

            HttpResponseMessage response = await client.GetAsync(url + "/" + deckId + "/draw/?count=1");

            if (response.IsSuccessStatusCode)
            {
                var responseData = response.Content.ReadAsStringAsync().Result;

                var drawnCard = JsonConvert.DeserializeObject<CardView>(responseData);

                if (drawnCard.remaining == 0)
                    RedirectToAction("Index");

                return drawnCard.cards[0];          
            }
            return null;
        }

        public async Task<ActionResult> PlayGame()
        {
            GameViewModel game = new Models.GameViewModel();
            game.newDeck = new Deck();
            game.newDeck.deck_id = Request.QueryString["deck_id"];
            game.dealer = new person();
            game.player = new person();
            game.dealer.Hand = new List<Card>();
            game.player.Hand = new List<Card>();

            for (int i = 0; i < 2; i++)
            {
                game.dealer.Hand.Add(await DrawACard(game.newDeck.deck_id));
                game.player.Hand.Add(await DrawACard(game.newDeck.deck_id));
            }

            Session["gameSession"] = game;

            return View(game);
        }

        [HttpPost]
        public async Task<ActionResult> PlayGame(string playerStayButton, string playerHitButton)
        {
            GameViewModel game = (GameViewModel)Session["gameSession"];

            if (playerStayButton != null)
            {
                while (game.dealer.HandValue() < 15 || game.dealer.HandValue() == 15)
                {
                    game.dealer.Hand.Add(await DrawACard(game.newDeck.deck_id));
                }
                {
                    if (game.dealer.HandValue() > 21)
                    {
                        Response.Write("<script>alert('Dealer Busted!')</script>");
                    }
                    else if (game.player.HandValue() > game.dealer.HandValue())
                    {
                        Response.Write("<script>alert('Player Wins!')</script>");
                        //game.GameOver = "true";
                    }
                    else if (game.player.HandValue() <= game.dealer.HandValue())
                    {
                        Response.Write("<script>alert('Dealer Wins!')</script>");
                        //game.GameOver = "false";
                    }
                   
                    return View(game);
                }
            }

            else if (playerHitButton != null)
            {
                while (game.player.HandValue() < 21)
                {
                    game.player.Hand.Add(await DrawACard(game.newDeck.deck_id));
                    return View(game);
                }
                if (game.player.HandValue() > 21)
                {
                    Response.Write("<script>alert('Player Busted!')</script>");
                }

                else if (game.player.HandValue() == 21)
                {
                    Response.Write("<script>alert('21! Player Wins!')</script>");
                }
                game.player.Hand.Add(await DrawACard(game.newDeck.deck_id));
                return View(game);
            }
            else
                return View(game);
        }
    }
}