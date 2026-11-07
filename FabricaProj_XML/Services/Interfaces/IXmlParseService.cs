using FabricaProj_XML.Models.Domain;

namespace FabricaProj_XML.Services.Interfaces
{
    public interface IXmlParseService
    {
        List<Nfce> XmlParse();
    }
}