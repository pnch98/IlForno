using IlForno.Data;
using IlForno.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace IlForno.Controllers
{
    public class CarrelloController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CarrelloController> _logger;

        public CarrelloController(ApplicationDbContext context, ILogger<CarrelloController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            List<Carrello> cartList = new List<Carrello>();
            var cartSession = HttpContext.Session.GetString("cartList");
            if (!string.IsNullOrEmpty(cartSession))
            {
                cartList = JsonConvert.DeserializeObject<List<Carrello>>(cartSession);
            }
            return View(cartList);
        }

        //Metodo che prende id prodotto e quantità, trova il prodotto tramite l'id e li manda come parametri a AddProdotto
        [HttpGet]
        public IActionResult AggiornaCarrello(int productId, int quantity)
        {
            try
            {
                // Verifica se productId e quantity sono diversi da zero
                if (productId != 0 && quantity != 0)
                {
                    // Puoi mantenere la logica esistente qui
                    Prodotto product = _context.Prodotti.Find(productId);
                    if (product == null)
                    {
                        TempData["error"] = "Prodotto non trovato";
                        return BadRequest("Prodotto non trovato");
                    }
                    AddProdotto(product, quantity);

                    return Ok();
                }
                else
                {
                    return BadRequest("Dati del carrello non validi");
                }
            }
            catch (Exception ex)
            {
                // Log dell'eccezione
                _logger.LogError(ex, "Errore durante l'aggiornamento del carrello");
                return StatusCode(500, "Errore durante l'aggiornamento del carrello: " + ex.Message);
            }
        }

        //Metodo che riprende il carrello dalla sessione e aggiorna la quantità del prodotto nel carrello
        private void AddProdotto(Prodotto prodotto, int quantita)
        {
            bool isFound = false;

            var cartSession = HttpContext.Session.GetString("cartList");
            if (cartSession != null)
            {
                List<Carrello> cart = JsonConvert.DeserializeObject<List<Carrello>>(cartSession);
                foreach (var item in cart)
                {
                    //controlla se il prodotto è già presente nel carrello, in caso aggiorna la quantità
                    if (item.ProdottoOrdine.Id == prodotto.Id)
                    {
                        item.Quantita += quantita;
                        HttpContext.Session.SetString("cartList", JsonConvert.SerializeObject(cart));
                        isFound = true;
                    }

                }
            }
            //se il prodotto non è presente, lo aggiunge con la rispettiva quantità
            if (isFound == false)
            {

                Carrello carrello = new Carrello();
                carrello.ProdottoOrdine = prodotto;
                carrello.Quantita = quantita;
                List<Carrello> cartList = new List<Carrello>();
                if (!string.IsNullOrEmpty(cartSession))
                {
                    cartList = JsonConvert.DeserializeObject<List<Carrello>>(cartSession);
                }
                cartList.Add(carrello);
                HttpContext.Session.SetString("cartList", JsonConvert.SerializeObject(cartList));
            }
        }

        //Metodo che prende il prodotto dal carrello e aggiorna la quantità aggiungendo 1
        [HttpPost]
        public IActionResult AddOne(int id)
        {
            try
            {
                var cartSession = HttpContext.Session.GetString("cartList");
                List<Carrello> cartList = string.IsNullOrEmpty(cartSession)
                    ? new List<Carrello>()
                    : JsonConvert.DeserializeObject<List<Carrello>>(cartSession);

                Carrello cartProd = cartList.FirstOrDefault(pr => pr.ProdottoOrdine.Id == id);
                if (cartProd != null)
                {
                    // Incrementa la quantità del prodotto nel carrello
                    cartProd.Quantita++;

                    // Aggiorna la sessione con il carrello aggiornato
                    HttpContext.Session.SetString("cartList", JsonConvert.SerializeObject(cartList));

                    return Ok();
                }
                else
                {
                    return NotFound("Prodotto non trovato nel carrello");
                }
            }
            catch (Exception ex)
            {
                // Log dell'eccezione
                _logger.LogError(ex, "Errore durante l'aggiornamento del carrello");
                return StatusCode(500, "Errore durante l'aggiornamento del carrello: " + ex.Message);
            }
        }

        //Metodo che prende il prodotto dal carrello e aggiorna la quantità rimuovendo 1
        [HttpPost]
        public IActionResult RemoveOne(int id)
        {
            try
            {
                var cartSession = HttpContext.Session.GetString("cartList");
                List<Carrello> cartList = string.IsNullOrEmpty(cartSession)
                    ? new List<Carrello>()
                    : JsonConvert.DeserializeObject<List<Carrello>>(cartSession);

                Carrello cartProd = cartList.FirstOrDefault(pr => pr.ProdottoOrdine.Id == id);
                if (cartProd != null)
                {
                    // Incrementa la quantità del prodotto nel carrello
                    cartProd.Quantita--;

                    // Aggiorna la sessione con il carrello aggiornato
                    HttpContext.Session.SetString("cartList", JsonConvert.SerializeObject(cartList));

                    return Ok();
                }
                else
                {
                    return NotFound("Prodotto non trovato nel carrello");
                }
            }
            catch (Exception ex)
            {
                // Log dell'eccezione
                _logger.LogError(ex, "Errore durante l'aggiornamento del carrello");
                return StatusCode(500, "Errore durante l'aggiornamento del carrello: " + ex.Message);
            }
        }

        //Metodo che prende il prodotto dal carrello e lo rimuove
        [HttpPost]
        public IActionResult RemoveProduct(int id)
        {
            try
            {
                var cartSession = HttpContext.Session.GetString("cartList");
                List<Carrello> cartList = string.IsNullOrEmpty(cartSession)
                    ? new List<Carrello>()
                    : JsonConvert.DeserializeObject<List<Carrello>>(cartSession);

                Carrello cartProd = cartList.FirstOrDefault(pr => pr.ProdottoOrdine.Id == id);
                if (cartProd != null)
                {
                    // Incrementa la quantità del prodotto nel carrello
                    cartList.Remove(cartProd);

                    // Aggiorna la sessione con il carrello aggiornato
                    HttpContext.Session.SetString("cartList", JsonConvert.SerializeObject(cartList));

                    return Ok();
                }
                else
                {
                    return NotFound("Prodotto non trovato nel carrello");
                }
            }
            catch (Exception ex)
            {
                // Log dell'eccezione
                _logger.LogError(ex, "Errore durante l'aggiornamento del carrello");
                return StatusCode(500, "Errore durante l'aggiornamento del carrello: " + ex.Message);
            }
        }

        //Metodo che svuota la sessione contenente il carrello
        [HttpPost]
        public IActionResult RemoveAll()
        {
            try
            {
                HttpContext.Session.Remove("cartList");
                return Ok();
            }
            catch (Exception ex)
            {
                // Log dell'eccezione
                _logger.LogError(ex, "Errore durante la rimozione della sessione");
                return StatusCode(500, "Errore durante la rimozione della sessione: " + ex.Message);
            }
        }

        public IActionResult RiepilogoOrdine()
        {
            return View();
        }

        //Metodo che crea un nuovo ordine e i dettagli ordine associati
        [HttpPost, ActionName("RiepilogoOrdine")]
        public async Task<IActionResult> RiepilogoOrdinePOST([Bind("IndirizzoDiConsegna", "Nota")] Ordine ordine)
        {
            List<Carrello> cartList = new List<Carrello>();
            var cartSession = HttpContext.Session.GetString("cartList");
            if (!string.IsNullOrEmpty(cartSession))
            {
                cartList = JsonConvert.DeserializeObject<List<Carrello>>(cartSession);
            }

            ModelState.Remove("Utente");
            ModelState.Remove("ListaDettagliOrdine");
            if (ModelState.IsValid)
            {
                //costruisco l'ordine a partire dal carrello e aggiungendo indirizzo e nota
                ordine.IdUtente = Convert.ToInt16(User.FindFirstValue(ClaimTypes.NameIdentifier));
                double prezzoTotale = 0;
                foreach (var item in cartList)
                {
                    prezzoTotale += item.ProdottoOrdine.Prezzo * item.Quantita;
                }
                ordine.PrezzoTotale = prezzoTotale;

                //aggiungo l'ordine al carrello e aggiorno il database
                _context.Ordini.Add(ordine);
                await _context.SaveChangesAsync();

                //creo dettagli ordine per ogni tipo di prodotto presente nel carrello e aggiorno il database
                foreach (var item in cartList)
                {
                    DettagliOrdine dettagliOrdine = new DettagliOrdine();
                    dettagliOrdine.IdOrdine = ordine.Id;
                    dettagliOrdine.IdProdotto = item.ProdottoOrdine.Id;
                    dettagliOrdine.Quantita = item.Quantita;

                    _context.DettagliOrdine.Add(dettagliOrdine);
                }

                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("cartList");
            }

            return View("AcquistoEffettuato");
        }

        public IActionResult AcquistoEffettuato()
        {
            return View();
        }

        //Metodo che ritorna il numero di prodotti presenti nel carrello
        [HttpGet]
        public IActionResult GetCartData()
        {
            List<Carrello> cartList = new List<Carrello>();
            var cartSession = HttpContext.Session.GetString("cartList");
            if (!string.IsNullOrEmpty(cartSession))
            {
                cartList = JsonConvert.DeserializeObject<List<Carrello>>(cartSession);
            }

            int totalProducts = cartList.Sum(item => item.Quantita);
            return Json(new { totalProducts });
        }
    }
}
