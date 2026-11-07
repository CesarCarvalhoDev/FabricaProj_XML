using FabricaProj_XML.Models.Domain;
using FabricaProj_XML.Repositories.Interfaces;
using FabricaProj_XML.Services.Interfaces;

namespace FabricaProj_XML.Services.Implementations
{
    public class XmlParseService(IFileSystemXmlRepository _fileSystemXmlRepository) : IXmlParseService
    {
        public List<Nfce> XmlParse()
        {
            List<Nfce> dadosNfce = new List<Nfce>();
            string[] arquivosNfe = _fileSystemXmlRepository.BuscarArquivosNfe();

            if (arquivosNfe == null || !arquivosNfe.Any())
                return new List<Nfce>();

            string[] arquivosValidosNfce = arquivosNfe
                .Where(arquivoNfe => EhModeloValido(arquivoNfe) && EhNotaAutorizada(arquivoNfe) && PossuiDestinatarioInformado(arquivoNfe))
                .ToArray();

            foreach (string arquivoValidoNfce in arquivosValidosNfce)
            {
                dadosNfce.Add(ExtrairDadosNfce(arquivoValidoNfce));
            }
            return dadosNfce;
        }

        private bool EhNotaAutorizada(string arquivoNfce)
        {
            string tagEncontrada = _fileSystemXmlRepository.ObterTag("<cStat>100</cStat>", arquivoNfce);
            return tagEncontrada == "<cStat>100</cStat>";
        }

        private bool EhModeloValido(string arquivoNfe)
        {
            string chaveXml = _fileSystemXmlRepository.ObterChaveXml(arquivoNfe);
            string modeloNota = _fileSystemXmlRepository.ObterModeloNota(chaveXml);

            return modeloNota == "65";
        }

        private bool PossuiDestinatarioInformado(string arquivoNfce)
        {
            string tagEncontrada = _fileSystemXmlRepository.ObterTag("<dest>", arquivoNfce);
            return tagEncontrada == "<dest>";
        }

        public Nfce ExtrairDadosNfce(string arquivoValidoNfce)
        {
            string chaveXml = _fileSystemXmlRepository.ObterChaveXml(arquivoValidoNfce);
            string conteudoDoXml = _fileSystemXmlRepository.ObterConteudoXml(arquivoValidoNfce);

            string modeloNota = _fileSystemXmlRepository.ObterModeloNota(chaveXml);
            int indexDhEmi = conteudoDoXml.IndexOf("<dhEmi>");
            string data = indexDhEmi >= 0 ? conteudoDoXml.Substring(indexDhEmi + 7, 7) : "";
            string serie = chaveXml.Substring(22, 3);
            string cnpjEmitente = chaveXml.Substring(6, 14);

            return new Nfce(chaveXml, modeloNota, data, cnpjEmitente, serie);
        }
    }
}