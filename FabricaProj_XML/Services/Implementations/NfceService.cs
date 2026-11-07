using FabricaProj_XML.Services.Interfaces;
using FabricaProj_XML.Models.Domain;

namespace FabricaProj_XML.Services.Implementations
{
    public class NfceService(IXmlParseService _xmlParseService) : INfceService
    {
        public List<Nfce> FiltrarNfceCompetencia(string? competencia)
        {
            List<Nfce> arquivosNfceValidas = _xmlParseService.XmlParse();
            
            if(string.IsNullOrWhiteSpace(competencia))
            {
               return arquivosNfceValidas.ToList(); 
            }
            return arquivosNfceValidas.Where(n => n.DataEmicao == competencia).ToList();
        }
    }
}