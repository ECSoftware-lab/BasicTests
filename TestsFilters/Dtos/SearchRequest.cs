using System.ComponentModel.DataAnnotations;

namespace TestsFilters.Dtos
{
    public class SearchRequest
    {
        [Required(ErrorMessage = "El término de búsqueda es obligatorio")]
        [MinLength(2, ErrorMessage = "El término debe tener al menos 2 caracteres")]
        public string Query { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "El límite debe estar entre 1 y 100")]
        public int Limit { get; set; } = 10;

        public int Page { get; set; } = 1;
    }
}
