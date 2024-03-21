using IlForno.Data;
using IlForno.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IlForno.Controllers
{
    public class ProdottoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProdottoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Prodottoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Prodotti.ToListAsync());
        }

        // GET: Prodottoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prodotto = await _context.Prodotti
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prodotto == null)
            {
                return NotFound();
            }

            return View(prodotto);
        }

        [Authorize(Roles = "Admin")]
        // GET: Prodottoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Prodottoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Nome,Img,Prezzo,TempoConsegna,Ingredienti")] Prodotto prodotto)
        {
            ModelState.Remove("ListaDettagliOrdine");
            if (ModelState.IsValid)
            {
                _context.Add(prodotto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(prodotto);
        }

        [Authorize(Roles = "Admin")]
        // GET: Prodottoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prodotto = await _context.Prodotti.FindAsync(id);
            if (prodotto == null)
            {
                return NotFound();
            }
            return View(prodotto);
        }

        // POST: Prodottoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Img,Prezzo,TempoConsegna,Ingredienti")] Prodotto prodotto)
        {
            if (id != prodotto.Id)
            {
                return NotFound();
            }

            ModelState.Remove("ListaDettagliOrdine");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prodotto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdottoExists(prodotto.Id))
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
            return View(prodotto);
        }

        [Authorize(Roles = "Admin")]
        // GET: Prodottoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prodotto = await _context.Prodotti
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prodotto == null)
            {
                return NotFound();
            }

            return View(prodotto);
        }

        // POST: Prodottoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prodotto = await _context.Prodotti.FindAsync(id);
            if (prodotto != null)
            {
                _context.Prodotti.Remove(prodotto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProdottoExists(int id)
        {
            return _context.Prodotti.Any(e => e.Id == id);
        }
    }
}
