using IlForno.Models;
using Microsoft.EntityFrameworkCore;

namespace IlForno.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Utente> Utenti { get; set; }
        public DbSet<Prodotto> Prodotti { get; set; }
        public DbSet<Ordine> Ordini { get; set; }
        public DbSet<DettagliOrdine> DettagliOrdine { get; set; }
    }
}
