using FabricaProj_XML.Repositories.Interfaces;

namespace FabricaProj_XML.Repositories.Implementations
{
    public class FileSystemXmlRepository : IFileSystemXmlRepository
    {
        public string[] BuscarArquivosNfe()
        {
            string caminhoArquivos = "C:\\XmlNfce";
            if (!Directory.Exists(caminhoArquivos))
            {
                return Array.Empty<string>();
            }
            return Directory.EnumerateFiles(caminhoArquivos, "*.xml", SearchOption.TopDirectoryOnly).ToArray();
        }
        public string ObterConteudoXml(string arquivoNfe)
        {
            return File.ReadAllText(arquivoNfe);
        }
        public string ObterChaveXml(string arquivoNfe)
        {
            return Path.GetFileNameWithoutExtension(arquivoNfe);
        }
        public string ObterModeloNota(string chaveXml)
        {
            if (chaveXml.Length < 25)
            {
                return "";
            }
            return chaveXml.Substring(20, 2);
        }

        public string ObterTag(string tag, string arquivoNfce)
        {
            string conteudoXml = ObterConteudoXml(arquivoNfce);
            int indexTag = conteudoXml.IndexOf(tag);
            if (indexTag >= 0)
            {
                string tagEncontrada = conteudoXml.Substring(indexTag, tag.Length);
                return tagEncontrada;
            }
            return "";
        }
    }
}