using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public static List<Dictionary<string, string>> ValidarDestNota(List<string> notasModelo65)
        {
            var resultado = new List<Dictionary<string, string>>();

            foreach (var notaModelo65 in notasModelo65)
            {
                string chaveModelo65 = Path.Combine(@"C:\XmlNfce", notaModelo65 + ".xml");

                string conteudoDoXml = File.ReadAllText(chaveModelo65);




                if (!conteudoDoXml.Contains("<dest>"))
                {
                    string data = conteudoDoXml.Substring(conteudoDoXml.IndexOf("<dhEmi>") + 7, 7);
                    string serie = notaModelo65.Substring(22, 4);
                    string primeiro5DigitosCnpj = notaModelo65.Substring(6, 5);
                    Console.WriteLine(primeiro5DigitosCnpj);
                    var item = new Dictionary<string, string>
                        {
                            { "Chave",  notaModelo65    },
                            { "Serie",  serie   },
                            { "Data",   data    },
                            { "CnpjCurto",   primeiro5DigitosCnpj    }
                        };

                    resultado.Add(item);
                }
            }
            return resultado;
        }

        static void Main(string[] args)
        {
            var chavesXml = ExtrairChaveXML();
            var notasModelo65 = ValidarModeloNota(new List<string>(chavesXml));
            var notasSemDest = ValidarDestNota(new List<string>(notasModelo65));

            Console.WriteLine($"Total de notas sem destinatário: {notasSemDest.Count}\n");
            foreach (var nota in notasSemDest)
            {
                Console.WriteLine($"Chave: {nota["Chave"]} | Série: {nota["Serie"]} | Data: {nota["Data"]} | Primeiros 5 dígitos do CNPJ: {nota["CnpjCurto"]}");
            }

            Console.Write("\nDigite a série para filtrar (Enter para ignorar): ");
            string serieBusca = Console.ReadLine();

            Console.Write("Digite a data para filtrar (ex.: 2024, 2024-01, 2024-01-15) (Enter para ignorar): ");
            string dataBusca = Console.ReadLine();

            IEnumerable<Dictionary<string, string>> query = notasSemDest;

            if (!string.IsNullOrWhiteSpace(serieBusca))
                query = query.Where(n => n["Serie"].Contains(serieBusca));

            if (!string.IsNullOrWhiteSpace(dataBusca))
                query = query.Where(n => n["Data"].Contains(dataBusca));

            var filtradas = query.ToList();

            Console.WriteLine($"\n{filtradas.Count} nota(s) encontrada(s):");
            foreach (var n in filtradas)
                Console.WriteLine($"  Série {n["Serie"]} | Data {n["Data"]} | Chave {n["Chave"]}");

            string cnpjCurto = filtradas.Count > 0 ? filtradas[0]["CnpjCurto"] : "00000";
            string dataPrimeiraNota = filtradas.Count > 0 ? filtradas[0]["Data"] : "";

            string mes = "00";
            string ano = "0000";

            if (!string.IsNullOrWhiteSpace(dataPrimeiraNota) && dataPrimeiraNota.Length >= 7)
            {
                ano = dataPrimeiraNota.Substring(0, 4);
                mes = dataPrimeiraNota.Substring(5, 2);
            }

            string nomeArquivo = $"{mes}{ano}{cnpjCurto}.txt";

            string caminhoSaida = Path.Combine(@"C:\XmlNfce", nomeArquivo);

            var linhas = new List<string>();
            linhas.Add($"Total de notas encontradas: {filtradas.Count}");
            linhas.Add("");

            foreach (var n in filtradas)
            {
                linhas.Add($"Série {n["Serie"]} | Data {n["Data"]} | Chave {n["Chave"]}");
            }

            File.WriteAllLines(caminhoSaida, linhas);

            Console.WriteLine($"\nResultado salvo em: {caminhoSaida}");
        }
    }
}