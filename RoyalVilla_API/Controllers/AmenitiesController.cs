using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVIlla.DTO;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;

namespace RoyalVilla_API.Controllers
{
    [Route("api/amenities")]
    [ApiController]
    public class AmenitiesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _db;

        public AmenitiesController(IMapper mapper, ApplicationDbContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        #region GET ENDPOINTS

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<VillaAmenitiesDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<VillaAmenitiesDTO>>>> GetAmenities()
        {
            try
            {
                var amenities = await _db.VillaAmenities.Include(a => a.Villa).ToListAsync();

                if (!amenities.Any())
                {
                    return Ok(ApiResponse<List<VillaAmenitiesDTO>>.Ok(
                        new List<VillaAmenitiesDTO>(), "There are no amenities in the DB"));
                }

                var response = ApiResponse<List<VillaAmenitiesDTO>>.Ok(
                    _mapper.Map<List<VillaAmenitiesDTO>>(amenities), "Successfully retrieved amenities");

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError, 
                    ApiResponse<object>.Error(500, $"An error occurred while retrieving amenities", ex.Message));
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<VillaAmenitiesDTO>>> GetAmenityById([FromRoute] int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Invalid id parameter"));
                }

                var amenity = await _db.VillaAmenities.Include(a => a.Villa).FirstOrDefaultAsync(u => u.Id == id);

                if (amenity == null)
                {
                    return NotFound(ApiResponse<object>.NotFound("Villa amenity with id " + id + " not found"));
                }

                var response = ApiResponse<VillaAmenitiesDTO>.Ok(_mapper.Map<VillaAmenitiesDTO>(amenity), "Successfully retrieved amenity");

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Error(500, $"An error occurred while retrieving the amenity with id {id}", ex.Message));
            }

        }

        #endregion

        #region POST ENDPOINTS
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<VillaAmenitiesDTO>>> CreateAmenity([FromBody] VillaAmenitiesCreateDTO villaAmenitiesCreateDTO)
        {
            try
            {
                if (!IsValidCreateAmenityRequest(villaAmenitiesCreateDTO))
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Invalid request data"));
                }

                if (!(await VillaExists(villaAmenitiesCreateDTO.VillaId)))
                {
                    return BadRequest(ApiResponse<object>.BadRequest($"Villa with id '{villaAmenitiesCreateDTO.VillaId}' does not exist"));
                }

                if (await VillaAmenityNameExist(villaAmenitiesCreateDTO.Name))
                {
                    return Conflict(ApiResponse<object>.Conflict($"Villa amenity with the name '{villaAmenitiesCreateDTO.Name}' already exists"));
                }



                var villaAmenity = _mapper.Map<VillaAmenities>(villaAmenitiesCreateDTO);

                await _db.VillaAmenities.AddAsync(villaAmenity);

                await _db.SaveChangesAsync();

                var villaAmenityDTO = _mapper.Map<VillaAmenitiesDTO>(villaAmenity);

                villaAmenityDTO.VillaName = _db.Villas
                    .FirstOrDefault(v => v.Id == villaAmenitiesCreateDTO.VillaId)?.Name;

                var response = ApiResponse<VillaAmenitiesDTO>.CreatedAt(
                    villaAmenityDTO, "Successfully created amenity");

                return CreatedAtAction(
                    nameof(GetAmenityById), new { id = villaAmenityDTO.Id }, response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Error(500, $"An error occurred while creating the amenity", ex.Message));
            }

        }


        private async Task<bool> VillaExists(int villaId) => await _db.Villas.AnyAsync(v => v.Id == villaId);


        private async Task<bool> VillaAmenityNameExist(string name) => 
            await _db.VillaAmenities.AnyAsync(a => a.Name.ToLower() == name.ToLower());

        private bool IsValidCreateAmenityRequest(
            VillaAmenitiesCreateDTO villaAmenitiesCreateDTO) => villaAmenitiesCreateDTO == null ? false : true;


        #endregion

        #region PUT ENDPOINTS
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<VillaAmenitiesDTO>>> UpdateAmenity([FromRoute] int id, [FromBody] VillaAmenitiesUpdateDTO villaAmenitiesUpdateDTO)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.BadRequest($"Invalid villa amenity id '{id}'. Id must be greater than zero."));
                }
                var existingAmenity = await _db.VillaAmenities.FindAsync(id);

                if (existingAmenity == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Villa amenity with id '{id}' not found"));
                }
                if (await VillaAmenityNameExist(villaAmenitiesUpdateDTO.Name) && existingAmenity.Name.ToLower() != villaAmenitiesUpdateDTO.Name.ToLower())
                {
                    return Conflict(ApiResponse<object>.Conflict($"Villa amenity with the name '{villaAmenitiesUpdateDTO.Name}' already exists"));
                }

                _mapper.Map(villaAmenitiesUpdateDTO, existingAmenity);

                _db.VillaAmenities.Update(existingAmenity);
                await _db.SaveChangesAsync();

                var updatedAmenityDTO = _mapper.Map<VillaAmenitiesDTO>(existingAmenity);
                var response = ApiResponse<VillaAmenitiesDTO>.Ok(updatedAmenityDTO, "Successfully updated amenity");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Error(500, $"An error occurred while updating the amenity with id {id}", ex.Message));
            }
        }

        #endregion

        #region DELETE ENDPOINTS

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAmenity([FromRoute] int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.BadRequest($"Invalid villa amenity id '{id}'. Id must be greater than zero."));
                }
                var existingAmenity = await _db.VillaAmenities.FindAsync(id);
                if (existingAmenity == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Villa amenity with id '{id}' not found"));
                }
                _db.VillaAmenities.Remove(existingAmenity);
                await _db.SaveChangesAsync();
                return Ok(ApiResponse<object>.NoContent($"Successfully deleted amenity with id {id}"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Error(500, $"An error occurred while deleting the amenity with id {id}", ex.Message));
            }
        }

        #endregion
    }
}
