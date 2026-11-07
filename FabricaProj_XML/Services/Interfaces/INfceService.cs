using FabricaProj_XML.Models.Domain; 

namespace FabricaProj_XML.Services.Interfaces
{
    public interface INfceService
    {
        List<Nfce> FiltrarNfceCompetencia(string? competencia);
    }
}