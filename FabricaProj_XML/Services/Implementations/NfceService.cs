using FabricaProj_XML.Services.Interfaces;
using FabricaProj_XML.Models.Domain;

namespace FabricaProj_XML.Services.Implementations
{
    public class NfceService(IXmlParseService _xmlParseService) : INfceService
    {
        public List<Nfce> FiltrarNfceCompetencia(string competencia)
        {
            List<Nfce> arquivosNfceValidas = _xmlParseService.XmlParse();

            if(arquivosNfceValidas == null || string.IsNullOrWhiteSpace(competencia))
            {
                return new List<Nfce>();
            }

            return arquivosNfceValidas.Where(n => n.DataEmicao == competencia).ToList();
        }
    }
}