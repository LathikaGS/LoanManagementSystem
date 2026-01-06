using LoanManagementSystem.Data;
using LoanManagementSystem.DTOs;
using LoanManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/loan-types")]
public class LoanTypeController : ControllerBase
{
    private readonly AppDbContext _context;

    public LoanTypeController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/loan-types
    [HttpGet]
    public async Task<IActionResult> GetLoanTypes()
    {
        return Ok(await _context.LoanTypes.ToListAsync());
    }

    // POST: api/loan-types
    [HttpPost]
    public async Task<IActionResult> AddLoanType([FromBody] LoanTypeDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var loanType = new LoanType
        {
            LoanName = dto.LoanName,
            ROI = dto.ROI,
            MaxTenure = dto.MaxTenure,
            MinAmount = dto.MinAmount,
            MaxAmount = dto.MaxAmount
        };

        _context.LoanTypes.Add(loanType);
        await _context.SaveChangesAsync();

        return Ok(loanType);
    }

    // PUT: api/loan-types/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLoanType(int id, [FromBody] LoanTypeDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var loanType = await _context.LoanTypes.FindAsync(id);
        if (loanType == null)
            return NotFound();

        loanType.LoanName = dto.LoanName;
        loanType.ROI = dto.ROI;
        loanType.MaxTenure = dto.MaxTenure;
        loanType.MinAmount = dto.MinAmount;
        loanType.MaxAmount = dto.MaxAmount;

        await _context.SaveChangesAsync();
        return Ok(loanType);
    }

    // DELETE: api/loan-types/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLoanType(int id)
    {
        var loanType = await _context.LoanTypes.FindAsync(id);
        if (loanType == null)
            return NotFound();

        _context.LoanTypes.Remove(loanType);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Loan type deleted successfully" });
    }
}
