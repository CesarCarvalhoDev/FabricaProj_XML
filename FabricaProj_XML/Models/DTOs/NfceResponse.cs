namespace FabricaProj_XML.Models.DTOs
{
    using Models.Domain;

    public class NfceResponse
    {
        public string ChaveNota {get; set;}
        public string Serie {get; set;}
        public string Data {get; set;}
        public string CnpjCurto {get; set;}

        public NfceResponse(Nfce nfce)
        {
            ChaveNota = nfce.ChaveXml;
            Serie = nfce.Serie;
            Data = nfce.DataEmicao;
            CnpjCurto = nfce.CnpjEmitente.Length >= 5 ? nfce.CnpjEmitente[..5] : nfce.CnpjEmitente;
        }
    }
}