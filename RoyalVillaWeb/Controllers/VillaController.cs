using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RoyalVIlla.DTO;
using RoyalVillaWeb.Services.IServices;

namespace RoyalVillaWeb.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;

        public VillaController(IVillaService villaService, IMapper mapper)
        {
            _villaService = villaService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            List<VillaDTO> villaList = new();

            try
            {
                var response = await _villaService.GetAllAsync<ApiResponse<List<VillaDTO>>>("");

                if (response != null && response.Success && response.Data != null)
                {
                    villaList = response.Data;
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error occurred while fetching villa data: {ex.Message}";
            }

            return View(villaList);
        }
    }
}
