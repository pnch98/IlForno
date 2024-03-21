using System.ComponentModel.DataAnnotations;

namespace IlForno.Models
{
    public class Utente
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string Role { get; set; } = "User";
        public virtual ICollection<Ordine> Ordini { get; set; }
    }
}
