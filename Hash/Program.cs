using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Hash
{
    class Program
    {
        //Nome do arquivo de video
        static String _nomeVideo = String.Empty;
        
        //Tamanho do bloco em bytes
        static int _tamanhoBloco = 1024;

        static void Main(string[] args)
        {
            Console.Write("*** Programa HASH - SHA256 ***");
            Console.Write("\n");

            int opcao = -1;

            while(opcao != 3)
            {
                ExibeMenu();
                int tmp = 0;
                if (int.TryParse(Console.ReadLine(), out tmp))
                {
                    opcao = tmp;
                }

                if(opcao == 0)
                {
                    break;
                } else if (opcao == 1)
                {
                    _nomeVideo = "video03.mp4";
                    ExecutaHash();
                    Console.Write("\n");
                } else if(opcao == 2)
                {
                    _nomeVideo = "video05.mp4";
                    ExecutaHash();
                    Console.Write("\n");
                } else
                {
                    Console.Write("Opção invalida");
                    Console.Write("\n");
                }
            }
            Console.WriteLine("programa encerrado.");
            System.Environment.Exit(0);
        }

        private static void ExibeMenu()
        {
            Console.WriteLine("Escolha uma das opções:");
            Console.WriteLine("1 - Video 03");
            Console.WriteLine("2 - Video 05");
            Console.WriteLine("0 - Sair do programa");
        }

        private static void ExecutaHash()
        {
            //Carrega o arquivo do video (em bytes)
            var bytesVideo = File.ReadAllBytes(_nomeVideo);

            //Obtem o tamanho do video em bytes
            int tamanhoVideo = bytesVideo.Length;

            //Obtem o numero de blocos inteiros do video
            int numBlocosVideo = tamanhoVideo / _tamanhoBloco;

            //Obtem o numero de bytes do ultimo bloco
            int numBytesUltimoBlocoVideo = tamanhoVideo % _tamanhoBloco;

            //Atualiza o numero de blocos, contando o ultimo bloco
            if ((tamanhoVideo % _tamanhoBloco) > 0)
            {
                numBlocosVideo++;
            }

            //Cria um arquivo para armazenar o video em formato de bytes
            byte[][] video = new byte[numBlocosVideo][];

            //Inicializa o arquivo que vai receber o video
            for (int i = 0; i < video.Length - 1; i++)
            {
                //Inicializa os blocos inteiros
                video[i] = new byte[_tamanhoBloco];
            }

            //Inicializa o ultimo bloco
            video[video.Length - 1] = new byte[numBytesUltimoBlocoVideo];

            //Carrega o arquivo do video
            FileStream fs = new FileStream(_nomeVideo, FileMode.Open);

            //Faz a leitura do video
            for (int index = 0; index < video.Length; index++)
            {
                fs.Read(video[index]);
            }
            //Encerra o leitor de arquivos
            fs.Close();

            /**Hash*/
            //Inicializa a matriz de hashes e o SHA-256
            byte[][] hashes = new byte[numBlocosVideo][];
            SHA256 sha256 = SHA256Managed.Create();

            //Calcula a hash de cada bloco e
            //Armazena a hash de cada bloco em um array de bytes
            //Do final para o inicio do arquivo
            for (int index = video.Length - 1; index > 0; index--)
            {
                // Caso o trecho não tenha o numero de bytes minimo do SHA-256
                // ele e completado automaticamente
                hashes[index] = sha256.ComputeHash(video[index]);

                //Concatena o hash do bloco atual com o bloco anterior
                byte[] aux = new byte[video[index - 1].Length + hashes[index].Length];
                Array.Copy(video[index - 1], 0, aux, 0, video[index - 1].Length);
                Array.Copy(hashes[index], 0, aux, video[index - 1].Length, hashes[index].Length);

                //Substitui bloco anterior pelo bloco concatenado 
                video[index - 1] = aux;
            }
            //Calcula a hash do ultimo bloco (primeiro)
            hashes[0] = sha256.ComputeHash(video[0]);

            //Converte as hashes para hexadecimal
            String[] hashVideo = new String[video.Length];
            for (int index = 0; index < hashVideo.Length; index++)
            {
                StringBuilder stringBuilder = new StringBuilder();
                byte[] tmpByte = null;
                foreach (byte b in hashes[index])
                {
                    tmpByte = new byte[1];
                    tmpByte[0] = b;
                    stringBuilder.Append(BitConverter.ToString(tmpByte)).ToString();
                }
                hashVideo[index] = stringBuilder.ToString();
            }

            //Imprime a hash de cada bloco
            string arquivoSaida = _nomeVideo + "_saidaHash.txt";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(arquivoSaida, false))
            {
                for (int index = 0; index < hashVideo.Length; index++)
                {
                    file.WriteLine(index + "\t" + hashVideo[index]);
                }
            }
            Console.WriteLine("Arquivo de saída gerado: \n" + System.Environment.CurrentDirectory + "\\" + arquivoSaida);
        }
    }
}
