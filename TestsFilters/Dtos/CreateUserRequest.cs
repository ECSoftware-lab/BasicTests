using System.ComponentModel.DataAnnotations;

namespace TestsFilters.Dtos
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        public string Email { get; set; } = string.Empty;

        [Range(18, 100, ErrorMessage = "La edad debe estar entre 18 y 100")]
        public int Age { get; set; }

        [Phone(ErrorMessage = "El teléfono no tiene un formato válido")]
        public string? PhoneNumber { get; set; }
    }
}
