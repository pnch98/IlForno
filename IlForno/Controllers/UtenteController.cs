using IlForno.Data;
using IlForno.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace IlForno.Controllers
{
    public class UtenteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CarrelloController> _logger;

        public UtenteController(ApplicationDbContext context, ILogger<CarrelloController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        // GET: Utentes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Utenti.ToListAsync());
        }

        // GET: Utentes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utente = await _context.Utenti
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utente == null)
            {
                return NotFound();
            }

            return View(utente);
        }

        // GET: Utentes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Utentes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Password,Role")] Utente utente)
        {
            ModelState.Remove("Ordini");
            if (ModelState.IsValid)
            {
                _context.Add(utente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(utente);
        }

        [Authorize(Roles = "Admin")]
        // GET: Utentes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utente = await _context.Utenti.FindAsync(id);
            if (utente == null)
            {
                return NotFound();
            }
            return View(utente);
        }

        // POST: Utentes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,Role")] Utente utente)
        {
            if (id != utente.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Ordini");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(utente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtenteExists(utente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(utente);
        }

        [Authorize(Roles = "Admin")]
        // GET: Utentes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utente = await _context.Utenti
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utente == null)
            {
                return NotFound();
            }

            return View(utente);
        }

        // POST: Utentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utente = await _context.Utenti.FindAsync(id);
            if (utente != null)
            {
                _context.Utenti.Remove(utente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtenteExists(int id)
        {
            return _context.Utenti.Any(e => e.Id == id);
        }

        //View che presenta lo storico ordini
        [Authorize]
        public IActionResult Profile()
        {
            int idUtente = Convert.ToInt16(User.FindFirstValue(ClaimTypes.NameIdentifier));
            //Ripreso l'id dalla sessione, grazie alle proprietà di navigazione ritorno una lista
            //che comprende oltre agli ordini anche i dettagli ordine e i prodotti associati
            var listaOrdiniEffettuati = _context.Ordini.Include(ord => ord.ListaDettagliOrdine)
                                                            .ThenInclude(dett => dett.Prodotto)
                                                       .Where(ord => ord.IdUtente == idUtente)
                                                       .ToList();
            return View(listaOrdiniEffettuati);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult BackOffice()
        {
            return View();
        }

        //Metodo che prende in input i valori di anno mese e giorno, ricostruisce la data e cerca nel database il totale ordini,
        //il totale ordini evasi e il totale incassi di quella data
        [HttpPost]
        public IActionResult DateStatus(int year, int month, int day)
        {
            try
            {
                DateOnly date = new DateOnly(year, month, day);

                int totaleOrdini = _context.Ordini.Where(ord => DateOnly.FromDateTime(ord.DataOrdine) == date).Count();
                int totaleOrdiniEvasi = _context.Ordini
                                                .Where(ord => DateOnly.FromDateTime(ord.DataOrdine) == date && ord.IsEvaso == true)
                                                .Count();
                double totaleIncassi = _context.Ordini.Where(ord => DateOnly.FromDateTime(ord.DataOrdine) == date).Sum(ord => ord.PrezzoTotale);

                return Json(new { totaleOrdini, totaleOrdiniEvasi, totaleIncassi });
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while processing the date status request.");

                // Return a generic error message
                return StatusCode(500, "An error occurred while processing the date status request.");
            }
        }
    }
}
