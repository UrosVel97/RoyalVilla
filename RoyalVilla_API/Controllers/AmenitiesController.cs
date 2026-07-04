using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Data.DTOs;
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
                    return Ok(ApiResponse<List<VillaAmenitiesDTO>>.Ok(new List<VillaAmenitiesDTO>(), "There are no amenities in the DB"));
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
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<VillaAmenitiesDTO>>> CreateAmenity([FromBody] VillaAmenitiesCreateDTO villaAmenitiesCreateDTO)
        {
            try
            {
                if (villaAmenitiesCreateDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Amenity data is null"));
                }

                var amenity = _mapper.Map<VillaAmenities>(villaAmenitiesCreateDTO);

                await _db.VillaAmenities.AddAsync(amenity);

                await _db.SaveChangesAsync();
                var response = ApiResponse<VillaAmenitiesDTO>.CreatedAt(_mapper.Map<VillaAmenitiesDTO>(amenity), "Successfully created amenity");
                return CreatedAtAction(nameof(GetAmenityById), new { id = amenity.Id }, response);


            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Error(500, $"An error occurred while creating the amenity", ex.Message));
            }

        }

        #endregion
    }
}
