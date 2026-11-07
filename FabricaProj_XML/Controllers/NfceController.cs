using Microsoft.AspNetCore.Mvc;
using FabricaProj_XML.Models.DTOs;
using FabricaProj_XML.Models.Domain;
using FabricaProj_XML.Services.Interfaces;

namespace FabricaProj_XML.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NfceController(INfceService _nfceService) : ControllerBase
    {
        // GET: /api/nfce?competencia=2026-10
        [HttpGet]
        public ActionResult<List<Nfce>> GetNfce([FromQuery] string? competencia)
        {
            var notasFiltradas = _nfceService.FiltrarNfceCompetencia(competencia);
            return Ok(notasFiltradas);
        } 
    }
}