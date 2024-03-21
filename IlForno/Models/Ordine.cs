using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IlForno.Models
{
    public class Ordine
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("Utente")]
        public int IdUtente { get; set; }
        [Display(Name = "Prezzo totale")]
        public double PrezzoTotale { get; set; }
        [Required]
        [Display(Name = "Indirizzo di consegna")]
        public string IndirizzoDiConsegna { get; set; }
        [Required]
        [Display(Name = "Data ordine")]
        public DateTime DataOrdine { get; set; } = DateTime.Now;
        [Required]
        public bool IsEvaso { get; set; } = false;
        public string Nota { get; set; } = "";
        public virtual Utente Utente { get; set; }
        public virtual ICollection<DettagliOrdine> ListaDettagliOrdine { get; set; }

    }
}
