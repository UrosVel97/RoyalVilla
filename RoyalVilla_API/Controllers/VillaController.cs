using Microsoft.AspNetCore.Mvc;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

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
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<VillaDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<VillaDTO>>>> GetVillas()
    {
        var villas = await _db.Villas.ToListAsync();

        var villaDTOs = _mapper.Map<List<VillaDTO>>(villas);

        return Ok(ApiResponse<IEnumerable<VillaDTO>>.Ok(villaDTOs, "Villas retrieved successfully"));
    }


    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<VillaDTO>>> GetVillaById(int id)
    {
        try
        {
            if(id <= 0)
            {
                return NotFound(ApiResponse<object>.NotFound("Villa ID must be greater than 0"));
            }    

            var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);
            if (villa == null)
            {
                return NotFound(ApiResponse<object>.NotFound($"Villa with ID {id} not found"));
            }

            var villaDTO = _mapper.Map<VillaDTO>(villa);


            return Ok(ApiResponse<VillaDTO>.Ok(villaDTO, $"Villa with ID {id} retrieved successfully"));

        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<VillaDTO>.Error(500, $"An error occurred while retrieving the villa with ID {id}: {ex.Message}"));
        }
        
    }

    #endregion

    #region POST ENDPOINTS

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<VillaDTO>>> CreateVilla(VillaCreateDTO villaDTO)
    {
        if(villaDTO == null)
        {
            return BadRequest(ApiResponse<object>.BadRequest("Villa data is null"));
        }

        var duplicateVilla = await _db.Villas.FirstOrDefaultAsync(v => v.Name.ToLower() == villaDTO.Name.ToLower());

        if (duplicateVilla != null)
        {
            return Conflict(ApiResponse<object>.Conflict($"Villa with name {villaDTO.Name} already exists"));
        }

        var villa = _mapper.Map<Villa>(villaDTO);

        await _db.Villas.AddAsync(villa);

        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVillaById), new { id = villa.Id }, ApiResponse<VillaDTO>.CreatedAt(_mapper.Map<VillaDTO>(villa), $"Villa with ID {villa.Id} created successfully"));
    }

    #endregion

    #region PUT ENDPOINTS

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<VillaDTO>>> UpdateVilla([FromRoute] int id, [FromBody] VillaUpdateDTO villaDTO)
    {
        try
        {
            if (villaDTO == null || id <= 0)
            {
                return BadRequest(ApiResponse<VillaUpdateDTO>.BadRequest("Invalid villa data or ID"));
            }

            var existingVilla = await _db.Villas.FindAsync(id);

            if (existingVilla == null)
            {
                return NotFound(ApiResponse<VillaUpdateDTO>.NotFound($"Villa with ID {id} not found"));
            }

            // Check if the name already exists in the database
            if (await _db.Villas.AnyAsync(v => v.Name == villaDTO.Name && v.Id != id))
            {
                return Conflict(ApiResponse<VillaUpdateDTO>.Conflict($"Villa with name {villaDTO.Name} already exists"));
            }


            _mapper.Map(villaDTO, existingVilla);
            existingVilla.UpdatedDate = DateTime.Now;


            _db.Villas.Update(existingVilla);

            await _db.SaveChangesAsync();

            return Ok(ApiResponse<VillaDTO>.Ok(_mapper.Map<VillaDTO>(existingVilla), $"Villa with ID {id} updated successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<VillaDTO>.Error(500, $"An error occurred while updating the villa with ID {id}: {ex.Message}"));
        }
    }

    #endregion

    #region DELETE ENDPOINTS

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteVilla([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest(ApiResponse<object>.BadRequest("Invalid villa data or ID"));
        }

        var existingVilla = await _db.Villas.FindAsync(id);

        if (existingVilla == null)
        {
            return NotFound(ApiResponse<object>.NotFound($"Villa with ID {id} not found"));
        }

    
        _db.Villas.Remove(existingVilla);


        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.NoContent($"Villa with ID {id} deleted successfully"));
    }

    #endregion


}
