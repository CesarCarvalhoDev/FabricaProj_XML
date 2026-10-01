using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
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
                    string data = conteudoDoXml.Substring(conteudoDoXml.IndexOf("<dhEmi>") + 7, 10);
                    string serie= notaModelo65.Substring(22, 4);
                    var item = new Dictionary<string, string>
                        {
                            { "Chave",  notaModelo65    },
                            { "Serie",  serie   },
                            { "Data",   data    }
                        };

                    resultado.Add(item);
                }
            }
            return resultado;
        }

        public static void FiltrarPorSerie(List<string> notasSemDest, int serie)
        {
            List<string> notaComSerie = new List<string>();
            foreach (var nota in notasSemDest)
            {
                string chaveXml = Path.Combine(@"C:\XmlNfce", nota + ".xml");

                string conteudoDoXml = File.ReadAllText(chaveXml);

                

            }
        }

        static void Main(string[] args)
        {
            var chavesXml = ExtrairChaveXML();
            var notasModelo65 = ValidarModeloNota(new List<string>(chavesXml));
            var notasSemDest = ValidarDestNota(new List<string>(notasModelo65));

            // 1) Listar tudo
            Console.WriteLine($"Total de notas sem destinatário: {notasSemDest.Count}\n");
            foreach (var nota in notasSemDest)
            {
                Console.WriteLine($"Chave: {nota["Chave"]} | Série: {nota["Serie"]} | Data: {nota["Data"]}");
            }

            // 2) Buscar por série
            Console.Write("\nDigite a série para filtrar (Enter para ignorar): ");
            string serieBusca = Console.ReadLine();

            // 3) Buscar por data
            Console.Write("Digite a data para filtrar (ex.: 2024, 2024-01, 2024-01-15) (Enter para ignorar): ");
            string dataBusca = Console.ReadLine();

            // 4) Aplicar filtros condicionalmente
            IEnumerable<Dictionary<string, string>> query = notasSemDest;

            if (!string.IsNullOrWhiteSpace(serieBusca))
            {
                query = query.Where(n => n["Serie"].Contains(serieBusca));
            }

            if (!string.IsNullOrWhiteSpace(dataBusca))
            {
                query = query.Where(n => n["Data"].Contains(dataBusca));
            }

            var filtradas = query.ToList();

            // 5) Exibir resultado
            Console.WriteLine($"\n{filtradas.Count} nota(s) encontrada(s):");
            foreach (var n in filtradas)
                Console.WriteLine($"  Série {n["Serie"]} | Data {n["Data"]} | Chave {n["Chave"]}");

            Console.ReadKey();
        }
    }
}