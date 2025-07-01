using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alfasoft.Models;

namespace ContactManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ContactsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET api/contacts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
    {
        return await _context.Contacts
            .Where(c => !c.IsDeleted)
            .ToListAsync();
    }
    
    // GET api/contacts/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Contact>> GetContact(int id)
    {
        var contact = await _context.Contacts
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (contact == null)
        {
            return NotFound();
        }

        return contact;
    }
    
    // POST api/contacts
    [HttpPost]
    public async Task<ActionResult<Contact>> CreateContact(Contact contact)
    {
        // Check if email already exists
        var existingContact = await _context.Contacts
            .FirstOrDefaultAsync(c => c.Email == contact.Email && !c.IsDeleted);
        
        if (existingContact != null)
        {
            return BadRequest(new { message = "Email already exists" });
        }

        _context.Contacts.Add(contact);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("IX_Contacts_Email") == true)
            {
                return BadRequest(new { message = "Email already exists" });
            }
            throw;
        }

        return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contact);
    }

    // PATCH api/contacts/5
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateContact(int id, [FromBody] ContactUpdateDto updateDto)
    {
        var contact = await _context.Contacts
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (contact == null)
        {
            return NotFound();
        }

        // Check email uniqueness if email is being updated
        if (!string.IsNullOrEmpty(updateDto.Email) && updateDto.Email != contact.Email)
        {
            var existingContact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Email == updateDto.Email && c.Id != id && !c.IsDeleted);
            
            if (existingContact != null)
            {
                return BadRequest(new { message = "Email already exists" });
            }
        }

        // Update only the provided fields
        if (!string.IsNullOrEmpty(updateDto.Name))
            contact.Name = updateDto.Name;
        
        if (!string.IsNullOrEmpty(updateDto.ContactNumber))
            contact.ContactNumber = updateDto.ContactNumber;
        
        if (!string.IsNullOrEmpty(updateDto.Email))
            contact.Email = updateDto.Email;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Handle database constraint violations
            if (ex.InnerException?.Message.Contains("IX_Contacts_Email") == true)
            {
                return BadRequest(new { message = "Email already exists" });
            }
            throw;
        }

        return NoContent();
    }
    
    // DELETE api/contacts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContact(int id)
    {
        var contact = await _context.Contacts
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (contact == null)
        {
            return NotFound();
        }

        // Soft delete
        contact.IsDeleted = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ContactExists(int id)
    {
        return _context.Contacts.Any(e => e.Id == id && !e.IsDeleted);
    }
}

// DTO for partial updates
public class ContactUpdateDto
{
    public string? Name { get; set; }
    public string? ContactNumber { get; set; }
    public string? Email { get; set; }
}
