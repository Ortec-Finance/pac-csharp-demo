using System.ComponentModel.DataAnnotations;

namespace TaskListAPI
{
    public class EditCustomerDto
    {
        [Required]
        [MaxLength(256)]
        public string Name { get; set; } = null!;
    }
}