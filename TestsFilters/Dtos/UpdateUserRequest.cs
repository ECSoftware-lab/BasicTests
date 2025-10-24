using System.ComponentModel.DataAnnotations;

namespace TestsFilters.Dtos
{
    public class UpdateUserRequest
    {
        [StringLength(100, MinimumLength = 3)]
        public string? Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Range(18, 100)]
        public int? Age { get; set; }
    }
}
