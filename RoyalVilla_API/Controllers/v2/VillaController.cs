using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVIlla.DTO;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;

namespace RoyalVilla_API.Controllers.v2;



[ApiController]
[Route("api/v{version:apiVersion}/villa")]
[ApiVersion("2.0")]
//[Authorize(Roles = "Customer,Admin")]
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
    public async Task<ActionResult<string>> GetVillas()
    {
        return "This is version 2 of the API";

    }


    [HttpGet("{id:int}")]
    public async Task<ActionResult<string>> GetVillaById([FromRoute] int id)
    {
        return $"This is version 2 of the API. You requested villa with ID: {id}";

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
        try
        {
            if (villaDTO == null)
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

            return CreatedAtAction(
                nameof(GetVillaById),
                new { id = villa.Id },
                ApiResponse<VillaDTO>.CreatedAt(
                    _mapper.Map<VillaDTO>(villa), $"Villa with ID {villa.Id} created successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.Error(500, $"An error occurred while creating the villa: {ex.Message}"));
        }
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
                return BadRequest(ApiResponse<object>.BadRequest("Invalid villa data or ID"));
            }

            var existingVilla = await _db.Villas.FindAsync(id);

            if (existingVilla == null)
            {
                return NotFound(ApiResponse<object>.NotFound($"Villa with ID {id} not found"));
            }

            // Check if the name already exists in the database
            if (await _db.Villas.AnyAsync(v => v.Name == villaDTO.Name && v.Id != id))
            {
                return Conflict(ApiResponse<object>.Conflict($"Villa with name {villaDTO.Name} already exists"));
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
                ApiResponse<object>.Error(500, $"An error occurred while updating the villa with ID {id}: {ex.Message}"));
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
        try
        {
            if (id <= 0)
            {
                return BadRequest(ApiResponse<object>.BadRequest("Invalid villa ID"));
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
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.Error(500, $"An error occurred while deleting the villa with ID {id}: {ex.Message}"));
        }
    }

    #endregion


}
