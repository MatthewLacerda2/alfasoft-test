using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Alfasoft.Context;
using Alfasoft.Models;

namespace ContactManager.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ApplicationDbContext _context;

    public List<Contact> Contacts { get; set; } = new();
    [BindProperty]
    public Contact NewContact { get; set; } = new();

    public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public void OnGet()
    {
        Contacts = _context.Contacts
            .Where(c => !c.IsDeleted)
            .ToList();
    }

    public IActionResult OnPostAdd()
    {
        if (!ModelState.IsValid)
        {
            OnGet(); // repopulate list
            return Page();
        }
        // Check for duplicate email
        if (_context.Contacts.Any(c => c.Email == NewContact.Email && !c.IsDeleted))
        {
            ModelState.AddModelError("NewContact.Email", "Email already exists");
            OnGet();
            return Page();
        }
        _context.Contacts.Add(NewContact);
        _context.SaveChanges();
        NewContact = new();
        return RedirectToPage();
    }

    public IActionResult OnPostDelete(int id)
    {
        var contact = _context.Contacts.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
        if (contact != null)
        {
            contact.IsDeleted = true;
            _context.SaveChanges();
        }
        return RedirectToPage();
    }
}
