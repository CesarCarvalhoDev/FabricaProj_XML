namespace FabricaProj_XML.Models.Domain
{
    public class Nfce
    {
        public string ChaveXml { get; set; }
        public string ModeloNfe { get; set; }
        public string DataEmicao { get; set; }
        public string CnpjEmitente { get; set; }
        public string Serie { get; set; }

        public Nfce(string chaveXml, string modeloNfe, string dataEmicao, string cnpjEmitente, string serie)
        {
            ChaveXml = chaveXml;
            ModeloNfe = modeloNfe;
            DataEmicao = dataEmicao;
            CnpjEmitente = cnpjEmitente;
            Serie = serie;
        }
    }
}