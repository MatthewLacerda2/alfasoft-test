using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alfasoft.Models;

public class Contact
{
    public Contact()
    {
        Name = string.Empty;
        ContactNumber = string.Empty;
        Email = string.Empty;
    }

    public int Id { get; set; }

    [Required]
    [MinLength(5)]
    public string Name { get; set; }

    [Required]
    [Phone]
    public string ContactNumber { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public bool IsDeleted { get; set; } = false;

}
