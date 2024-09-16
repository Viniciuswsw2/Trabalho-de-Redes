using System;
using System.Net.Sockets;
using System.Text;

class TicTacToeClient
{
    static void Main(string[] args)
    {
        TcpClient client = null;
        NetworkStream stream = null;
        string server = "127.0.0.1";
        int port = 13000;
        string startMsg = string.Empty;
        string playerSymbol = "O";
        string response = string.Empty;
        bool gameStarted = false;
        bool gameEnded = false;
        int bytesRead = 0;
        bool ignore = false;
        bool control = false;

        try
        {
            client = new TcpClient(server, port); // Conectando ao servidor local
            stream = client.GetStream();

            Console.WriteLine("Conectado ao servidor do Jogo da Velha!");

            byte[] buffer = new byte[512];

            while (!gameStarted)
            {
                // Ler resposta inicial do servidor (esperando outro jogador ou início do jogo)
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                startMsg = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                if (startMsg == "1" && !ignore)
                {
                    Console.WriteLine("Aguardando outro jogador...");
                    playerSymbol = "X";
                    ignore = true;
                }
                else if (startMsg == "0")
                {
                    Console.WriteLine("Iniciando Partida...");
                    gameStarted = true;
                }
            }

            // Recebe mensagem do Servidor
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            // Exibe o jogo da velha
            Console.WriteLine(response);
            ExibirJogoDaVelha(response);

            if (playerSymbol == "O")
            {
                control = true;
            }

            // Loop do jogo
            while (!gameEnded)
            {
                if (control)
                {
                    Console.WriteLine("Aguarde a Jogada do seu oponente...");
                }

                // Recebe mensagem do Servidor
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                if (response.Length == 1)
                {
                    while (true)
                    {
                        // Pede para o usuário digitar a jogada
                        Console.WriteLine("Sua vez! Digite sua Jogada (1-9):");

                        // Envia a jogada para o Servidor
                        string jogada = Console.ReadLine();
                        byte[] msg = Encoding.UTF8.GetBytes(jogada);
                        stream.Write(msg, 0, msg.Length);

                        // Receber a confirmação da jogada
                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                        response = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                        if (response == "-1")
                        {
                            Console.WriteLine("Jogada inválida, tente novamente.");
                        }
                        else
                        {
                            Console.WriteLine("Jogada registrada com sucesso!");
                            // Recebe jogo da velha atualizado do Servidor
                            bytesRead = stream.Read(buffer, 0, buffer.Length);
                            response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            break;
                        }
                    }
                }

                // Exibe o jogo da velha
                ExibirJogoDaVelha(response);

                if (response.Length == 10)
                {
                    // Verificar se houve vitória ou empate
                    char gameResult = response[9];
                    if (gameResult == '1' || gameResult == '2')
                    {
                        // Exibe mensagem de vitória/derrota para ambos os jogadores
                        if ((gameResult == '1' && playerSymbol == "X") || (gameResult == '2' && playerSymbol == "O"))
                        {
                            // Mensagem de vitória personalizada
                            Console.WriteLine();
                            Console.WriteLine("╔═══════════════════════════════╗");
                            Console.WriteLine("║     PARABÉNS! VOCÊ VENCEU!    ║");
                            Console.WriteLine("║ Você dominou o Jogo da Velha  ║");
                            Console.WriteLine($"║      com o jogador '{playerSymbol}'!       ║");
                            Console.WriteLine("╚═══════════════════════════════╝");
                            Console.WriteLine();
                        }
                        else
                        {
                            // Mensagem de derrota
                            Console.WriteLine("Infelizmente, o oponente venceu!");
                        }
                        gameEnded = true;
                    }
                    else if (gameResult == '3')
                    {
                        Console.WriteLine("Jogo Encerrado, Deu Velha! (Empate)");
                        gameEnded = true;
                    }
                }

                // Alterna o controle do jogo entre os jogadores
                control = !control;
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            stream?.Close();
            client?.Close();
        }

        Console.WriteLine("Conexão encerrada.");
    }

    static void ExibirJogoDaVelha(string board)
    {
        Console.WriteLine("");
        Console.WriteLine("Jogo da Velha Atual:");
        Console.WriteLine($"{board[0]} | {board[1]} | {board[2]}");
        Console.WriteLine("--+---+--");
        Console.WriteLine($"{board[3]} | {board[4]} | {board[5]}");
        Console.WriteLine("--+---+--");
        Console.WriteLine($"{board[6]} | {board[7]} | {board[8]}");
        Console.WriteLine("");
    }
}
