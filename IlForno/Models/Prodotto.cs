using System.ComponentModel.DataAnnotations;

namespace IlForno.Models
{
    public class Prodotto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public string Img { get; set; }
        [Required]
        public double Prezzo { get; set; }
        [Required]
        public int TempoConsegna { get; set; }
        [Required]
        public string Ingredienti { get; set; }
        public virtual ICollection<DettagliOrdine> ListaDettagliOrdine { get; set; }
    }
}
