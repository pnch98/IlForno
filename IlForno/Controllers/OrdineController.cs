using IlForno.Data;
using IlForno.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IlForno.Controllers
{
    public class OrdineController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CarrelloController> _logger;

        public OrdineController(ApplicationDbContext context, ILogger<CarrelloController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Ordines
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ordini.Include(o => o.Utente);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Ordines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordine = await _context.Ordini
                .Include(o => o.Utente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordine == null)
            {
                return NotFound();
            }

            return View(ordine);
        }

        // GET: Ordines/Create
        public IActionResult Create()
        {
            ViewData["IdUtente"] = new SelectList(_context.Utenti, "Id", "Password");
            return View();
        }

        // POST: Ordines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUtente,PrezzoTotale,IndirizzoDiConsegna,DataOrdine,IsEvaso,Nota")] Ordine ordine)
        {
            ModelState.Remove("Utente");
            ModelState.Remove("ListaDettagliOrdine");
            if (ModelState.IsValid)
            {
                _context.Add(ordine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdUtente"] = new SelectList(_context.Utenti, "Id", "Password", ordine.IdUtente);
            return View(ordine);
        }

        // GET: Ordines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordine = await _context.Ordini.FindAsync(id);
            if (ordine == null)
            {
                return NotFound();
            }
            ViewData["IdUtente"] = new SelectList(_context.Utenti, "Id", "Password", ordine.IdUtente);
            return View(ordine);
        }

        // POST: Ordines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdUtente,PrezzoTotale,IndirizzoDiConsegna,DataOrdine,IsEvaso,Nota")] Ordine ordine)
        {
            if (id != ordine.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Utente");
            ModelState.Remove("ListaDettagliOrdine");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdineExists(ordine.Id))
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
            ViewData["IdUtente"] = new SelectList(_context.Utenti, "Id", "Password", ordine.IdUtente);
            return View(ordine);
        }

        // GET: Ordines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordine = await _context.Ordini
                .Include(o => o.Utente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordine == null)
            {
                return NotFound();
            }

            return View(ordine);
        }

        // POST: Ordines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ordine = await _context.Ordini.FindAsync(id);
            if (ordine != null)
            {
                _context.Ordini.Remove(ordine);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdineExists(int id)
        {
            return _context.Ordini.Any(e => e.Id == id);
        }

        //Metodo che ritorna una lista di ordini dove IsEvaso = false
        [Authorize(Roles = "Admin")]
        public IActionResult OrdiniDaEvadere()
        {
            var ordiniDaEvadere = _context.Ordini.Where(ord => ord.IsEvaso == false);
            return View(ordiniDaEvadere);
        }

        //Metodo che imposta IsEvaso del prodotto selezionato a true
        [HttpPost]
        public IActionResult Evaso(int id)
        {
            try
            {
                var ordine = _context.Ordini.FirstOrDefault(ord => ord.Id == id);

                if (ordine == null)
                {
                    return StatusCode(500, "An error occurred while processing the date status request.");
                }

                ordine.IsEvaso = true;
                _context.Ordini.Update(ordine);
                _context.SaveChanges();

                return Ok();
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
