using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Xml;

namespace ConsoleApp1
{
    internal class Program
    {

        public static string[] ExtrairChaveXML()
        {

            string pasta = @"C:\XmlNfce";
            string[] arquivos = Directory.GetFiles(pasta);

            List<string> notas = new List<string>();

            foreach (string arquivo in arquivos)
            {
                string xml = Path.GetFileNameWithoutExtension(arquivo);
                notas.Add(xml);
            }
            return notas.ToArray();
        }
        public static string[] ValidarModeloNota(List<string> notas)
        {
            List<string> notas_modelo_65 = new List<string>();

            foreach (string nota in notas)
            {
                if (nota.Length < 22) continue;
                string modelo_da_nota = nota.Substring(20, 2);

                if (modelo_da_nota == "65")
                {
                    Console.WriteLine("Nota modelo 65");
                    notas_modelo_65.Add(nota);
                }
                else if (modelo_da_nota == "55")
                {
                    Console.WriteLine("Nota modelo 55");
                }
                else
                {
                    Console.WriteLine("Modelo de nota inválido");
                }
            }

            return notas_modelo_65.ToArray();
        }

        public static List<string> ValidarDestNota(List<string> notasModelo65)
        {
            var notasSemDest = new List<string>();


            foreach (var notaModelo65 in notasModelo65)
            {
                string caminhoXml = Path.Combine(@"C:\XmlNfce", notaModelo65 + ".xml");
                string conteudoDoXml = File.ReadAllText(caminhoXml);
                if (!conteudoDoXml.Contains("<dest>"))
                {
                    notasSemDest.Add(notaModelo65);
                }
            }
            return notasSemDest;
        }

        public static List<string> FiltrarPorCompetencia(string dataBusca, List<string> notasSemDest)
        {
            IEnumerable<string> query = notasSemDest;

            if (!string.IsNullOrWhiteSpace(dataBusca))
            {
                query = query.Where(chaveNota =>
                {
                    string caminhoCompleto = Path.Combine(@"C:\XmlNfce", chaveNota + ".xml");

                    if (!File.Exists(caminhoCompleto))
                        return false;

                    string conteudoDoXml = File.ReadAllText(caminhoCompleto);

                    int inicio = conteudoDoXml.IndexOf("<dhEmi>");
                    int fim = conteudoDoXml.IndexOf("</dhEmi>");

                    if (inicio != -1 && fim > inicio)
                    {
                        string dhEmi = conteudoDoXml.Substring(inicio + 7, fim - (inicio + 7));
                        return dhEmi.Contains(dataBusca);
                    }

                    return false;
                });
            }

            return query.ToList();
        }

        public static List<Dictionary<string, string>> ExtrairDadosNotas(List<string> notasFiltradas)
        {
            var resultado = new List<Dictionary<string, string>>();

            foreach (string chaveNota in notasFiltradas)
            {
                string chaveNotaXml = Path.Combine(@"C:\XmlNfce", chaveNota + ".xml");
                string conteudoDoXml = File.ReadAllText(chaveNotaXml);

                string data = conteudoDoXml.Substring(conteudoDoXml.IndexOf("<dhEmi>") + 7, 7);
                string serie = chaveNota.Substring(22, 3); // Série na chave tem 3 dígitos
                string primeiro5DigitosCnpj = chaveNota.Substring(6, 5);

                var item = new Dictionary<string, string>
                {
                    { "Chave", chaveNota },
                    { "Serie", serie },
                    { "Data", data },
                    { "CnpjCurto", primeiro5DigitosCnpj }
                };
                resultado.Add(item);
            }

            return resultado;
        }

        public static void SalvarRelatorioNotas(List<Dictionary<string, string>> dadosNotasFiltradas)
        {
            
            if (dadosNotasFiltradas.Count == 0)
            {
                Console.WriteLine("\nNenhuma nota encontrada para salvar.");
                return;
            }

            string data = dadosNotasFiltradas[0]["Data"];
            string cnpj = dadosNotasFiltradas[0]["CnpjCurto"];

            string ano = data.Substring(0, 4);
            string mes = data.Substring(5, 2);
            string caminhoSaida = $@"C:\XmlNfce\{mes}{ano}{cnpj}.txt";

            var relatorio = new
            {
                TotalNotas = dadosNotasFiltradas.Count,
                Notas = dadosNotasFiltradas
            };
 
            var opcoesJson = new JsonSerializerOptions { WriteIndented = true };
            string conteudoJson = JsonSerializer.Serialize(relatorio, opcoesJson);

            File.WriteAllText(caminhoSaida, conteudoJson);

            Console.WriteLine($"Resultado salvo em: {caminhoSaida}");
        }

        static void Main(string[] args)
        {
            var chavesXml = ExtrairChaveXML();
            var notasModelo65 = ValidarModeloNota(new List<string>(chavesXml));
            var notasSemDest = ValidarDestNota(new List<string>(notasModelo65));

            Console.Write("\nDigite a competência para filtrar (Enter para ignorar): ");
            string dataCompetencia = Console.ReadLine();

            var notasFiltradas = FiltrarPorCompetencia(dataCompetencia, new List<string>(notasSemDest));

            var dadosNotasFiltradas = ExtrairDadosNotas(new List<string>(notasFiltradas));

            SalvarRelatorioNotas(dadosNotasFiltradas);
        }
    }
}