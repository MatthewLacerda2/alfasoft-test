using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Alfasoft.Context;
using Alfasoft.Models;

namespace ContactManager.Pages;

public class EditModel : PageModel
{
    private readonly ILogger<EditModel> _logger;
    private readonly ApplicationDbContext _context;

    [BindProperty]
    public Contact Contact { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public EditModel(ILogger<EditModel> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult OnGet(int id)
    {
        var contact = _context.Contacts.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
        if (contact == null)
        {
            return RedirectToPage("/Index");
        }

        Contact = contact;
        return Page();
    }

    public IActionResult OnPostSave()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Check for duplicate email (excluding current contact)
        if (_context.Contacts.Any(c => c.Email == Contact.Email && c.Id != Contact.Id && !c.IsDeleted))
        {
            ErrorMessage = "Email already exists";
            return Page();
        }

        var existingContact = _context.Contacts.FirstOrDefault(c => c.Id == Contact.Id && !c.IsDeleted);
        if (existingContact == null)
        {
            ErrorMessage = "Contact not found";
            return Page();
        }

        existingContact.Name = Contact.Name;
        existingContact.ContactNumber = Contact.ContactNumber;
        existingContact.Email = Contact.Email;

        _context.SaveChanges();
        return RedirectToPage("/Index");
    }

    public IActionResult OnPostCancel()
    {
        // Re-fetch the original values
        var contact = _context.Contacts.FirstOrDefault(c => c.Id == Contact.Id && !c.IsDeleted);
        if (contact != null)
        {
            Contact = contact;
        }
        return Page();
    }
}
