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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VillaCreateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.CreateAsync<ApiResponse<VillaDTO>>(model,"");
                if (response != null && response.Success)
                {
                    TempData["success"] = "Villa created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "Error occurred while creating villa.";
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var response = await _villaService.GetAsync<ApiResponse<VillaDTO>>(id, "");
            if (response != null && response.Success && response.Data != null)
            {
                var villaUpdateDto = _mapper.Map<VillaUpdateDTO>(response.Data);
                return View(villaUpdateDto);
            }
            TempData["error"] = "Villa not found.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(VillaUpdateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.UpdateAsync<ApiResponse<VillaDTO>>(model, "");
                if (response != null && response.Success)
                {
                    TempData["success"] = "Villa updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "Error occurred while updating villa.";
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var villa = _villaService.GetAsync<ApiResponse<VillaDTO>>(id, "").Result;
            if (villa != null && villa.Success && villa.Data != null)
            {
                return View(villa.Data);
            }
            TempData["error"] = "Villa not found.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(VillaDTO villa)
        {
            var response = await _villaService.DeleteAsync<ApiResponse<VillaDTO>>(villa.Id, "");
            if (response != null && response.Success)
            {
                TempData["success"] = "Villa deleted successfully!";
            }
            else
            {
                TempData["error"] = "Error occurred while deleting villa.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
