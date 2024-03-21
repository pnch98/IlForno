using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IlForno.Models
{
    public class DettagliOrdine
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("Ordine")]
        public int IdOrdine { get; set; }
        [Required]
        [ForeignKey("Prodotto")]
        public int IdProdotto { get; set; }
        [Required]
        public int Quantita { get; set; }
        public virtual Prodotto Prodotto { get; set; }
        public virtual Ordine Ordine { get; set; }
    }
}
