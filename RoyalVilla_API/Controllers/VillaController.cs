using Microsoft.AspNetCore.Mvc;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data.DTOs;
using AutoMapper;

namespace RoyalVilla_API.Controllers;



[ApiController]
[Route("api/villa")]
public class VillaController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public VillaController(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    #region GET ENDPOINTS

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Villa>>> GetVillas()
    {
        return Ok(await _db.Villas.ToListAsync());
    }


    [HttpGet("{id:int}")]
    public async Task<ActionResult<Villa>> GetVilla(int id)
    {
        try
        {
            if(id <= 0)
            {
                return BadRequest("Villa ID must be greater than 0");
            }    

            var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);
            if (villa == null)
            {
                return NotFound($"Villa with ID {id} not found");
            }

            return Ok(villa);
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError, 
                $"An error occurred while retrieving the villa with id {id}");
        }
        
    }

    #endregion

    #region POST ENDPOINTS

    [HttpPost]
    public async Task<ActionResult<Villa>> CreateVilla(VillaCreateDTO villaDTO)
    {
        if(villaDTO == null)
        {
            return BadRequest("Villa data is null");
        }

        var villa = _mapper.Map<Villa>(villaDTO);

        await _db.Villas.AddAsync(villa);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVilla), new { id = villa.Id }, villa);
    }

    #endregion

    #region PUT ENDPOINTS

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Villa>> UpdateVilla([FromRoute] int id, [FromBody] VillaCreateDTO villaDTO)
    {
        if (villaDTO == null || id <= 0)
        {
            return BadRequest("Invalid villa data or ID");
        }

        var existingVilla = await _db.Villas.FindAsync(id);

        if (existingVilla == null)
        {
            return NotFound($"Villa with ID {id} not found");
        }

        _mapper.Map(villaDTO, existingVilla);

        _db.Villas.Update(existingVilla);

        await _db.SaveChangesAsync();

        return Ok();
    }

    #endregion

    #region DELETE ENDPOINTS

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Villa>> DeleteVilla([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid villa data or ID");
        }

        var existingVilla = await _db.Villas.FindAsync(id);

        if (existingVilla == null)
        {
            return NotFound($"Villa with ID {id} not found");
        }

    
        _db.Villas.Remove(existingVilla);


        await _db.SaveChangesAsync();

        return NoContent();
    }

    #endregion


}
