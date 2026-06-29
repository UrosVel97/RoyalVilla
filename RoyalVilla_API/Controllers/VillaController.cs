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
    public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
    {
        var villas = await _db.Villas.ToListAsync();
        return Ok(_mapper.Map<List<VillaDTO>>(villas));
    }


    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<VillaDTO>>> GetVillaById(int id)
    {
        try
        {
            if(id <= 0)
            {
                return new ApiResponse<VillaDTO>()
                {
                    StatusCode = 400,
                    Errors = "Villa ID must be greater than 0",
                    Success = false,
                    Message = "Bad request"
                };

            }    

            var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);
            if (villa == null)
            {
                return NotFound($"Villa with ID {id} not found");
            }

            var villaDTO = _mapper.Map<VillaDTO>(villa);

            return new ApiResponse<VillaDTO>()
            {
                StatusCode = 200,
                Data = villaDTO,
                Success = true,
                Message = "Record retrieved successfully"
            };

        }
        catch (Exception ex)
        {
            return new ApiResponse<VillaDTO>()
            {
                StatusCode = 500,
                Errors = ex.Message,
                Success = false,
                Message = "A server side error occurred while processing the request"
            };

        }
        
    }

    #endregion

    #region POST ENDPOINTS

    [HttpPost]
    public async Task<ActionResult<VillaDTO>> CreateVilla(VillaCreateDTO villaDTO)
    {
        if(villaDTO == null)
        {
            return BadRequest("Villa data is null");
        }

        var duplicateVilla = await _db.Villas.FirstOrDefaultAsync(v => v.Name.ToLower() == villaDTO.Name.ToLower());
        if (duplicateVilla != null)
        {
            return Conflict($"Villa with name {villaDTO.Name} already exists");
        }

        var villa = _mapper.Map<Villa>(villaDTO);

        await _db.Villas.AddAsync(villa);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVillaById), new { id = villa.Id }, _mapper.Map<VillaDTO>(villa));
    }

    #endregion

    #region PUT ENDPOINTS

    [HttpPut("{id:int}")]
    public async Task<ActionResult<VillaUpdateDTO>> UpdateVilla([FromRoute] int id, [FromBody] VillaUpdateDTO villaDTO)
    {
        try
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

            // Check if the name already exists in the database
            if (await _db.Villas.AnyAsync(v => v.Name == villaDTO.Name && v.Id != id))
            {
                return Conflict($"Villa with name {villaDTO.Name} already exists");
            }


            _mapper.Map(villaDTO, existingVilla);
            existingVilla.UpdatedDate = DateTime.Now;

            _db.Villas.Update(existingVilla);

            await _db.SaveChangesAsync();

            return Ok(_mapper.Map<VillaDTO>(existingVilla));
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                $"An error occurred while updating the villa with id {id}");
        }
    }

    #endregion

    #region DELETE ENDPOINTS

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteVilla([FromRoute] int id)
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
