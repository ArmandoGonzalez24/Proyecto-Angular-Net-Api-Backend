using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProductCategoryCrud.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [NotMapped] // Este campo no se almacenará en la base de datos
        public string Password { get; set; }

        [JsonIgnore] // Oculta este campo en Swagger
        public string PasswordHash { get; set; }

        [Required]
        public int RoleId { get; set; }

        [JsonIgnore] // Oculta este campo en Swagger
        public Role Role { get; set; }
    }
}
