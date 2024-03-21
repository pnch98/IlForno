using System.ComponentModel.DataAnnotations.Schema;

namespace IlForno.Models
{
    public class Carrello
    {
        [NotMapped]
        public Prodotto ProdottoOrdine { get; set; }
        [NotMapped]
        public int Quantita { get; set; }
    }
}
