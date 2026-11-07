namespace FabricaProj_XML.Repositories.Interfaces
{
    public interface IFileSystemXmlRepository
    {
        string[] BuscarArquivosNfe();
        string ObterConteudoXml(string arquivoNfe);
        string ObterChaveXml(string arquivoNfce);
        string ObterModeloNota(string chaveXml);
        string ObterTag(string tag, string arquivoNfce);

    }

}