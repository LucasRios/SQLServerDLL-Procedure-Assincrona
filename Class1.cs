using System;                        // Importa tipos básicos do .NET (Exception, String, etc.)
using System.Data.SqlClient;          // Fornece classes para conexão e execução de comandos SQL no SQL Server
using System.Data.SqlTypes;           // Contém tipos SQL nativos usados por procedimentos CLR (SqlString, SqlInt32, etc.)
using System.Threading;               // Necessário para criar e gerenciar threads manuais
using Microsoft.SqlServer.Server;     // Contém atributos e classes específicos para integração CLR no SQL Server

// Classe pública que define uma stored procedure CLR (Common Language Runtime)
// Essa procedure pode ser chamada diretamente a partir do SQL Server via CREATE PROCEDURE EXTERNAL NAME
public class SQLProcedureAssincrona
{
    // Atributo [SqlProcedure] indica ao SQL Server que este método é exposto como uma stored procedure CLR
    [SqlProcedure]
    public static void ExecuteAsync(SqlString databaseName, SqlString commandText)
    {
        // Converte os parâmetros do tipo SqlString (nativos do SQL Server) para strings .NET normais
        string db = databaseName.Value;     // Nome do banco de dados onde o comando será executado
        string cmd = commandText.Value;     // Texto do comando SQL a ser executado

        // Comentário: a execução será feita em uma thread separada para não bloquear o processo SQL Server.
        // Isso é útil para executar operações demoradas ou que podem falhar sem afetar a transação principal.

        Thread thread = new Thread(() =>
        {
            try
            {
                // Define a string de conexão para o servidor SQL remoto.
                string connString = $"-----";

                // Cria um objeto SqlConnection que gerencia a conexão com o banco especificado.
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // Abre a conexão com o banco.
                    conn.Open();

                    // Cria um comando SQL que será executado dentro da conexão aberta.
                    // Usa o texto recebido como parâmetro (cmd).
                    using (SqlCommand sqlCmd = new SqlCommand(cmd, conn))
                    {
                        // Executa o comando SQL (sem retorno de resultados — comando de ação, tipo INSERT/UPDATE/DELETE)
                        sqlCmd.ExecuteNonQuery();
                    } // Fecha o SqlCommand automaticamente (dispose) ao sair do bloco "using"
                } // Fecha a conexão (dispose) ao sair do bloco "using"
            }
            catch (Exception ex)
            {
                // Captura qualquer exceção que ocorra durante a execução na thread.
                // O bloco está vazio — o erro é engolido silenciosamente.
                // Recomendável registrar a exceção em log, arquivo ou tabela de auditoria.
            }
        });

        // Define a thread como "background", o que significa que ela não impedirá
        // o encerramento do processo se o SQL Server for finalizado.
        // Threads em background são encerradas automaticamente quando o processo termina.
        thread.IsBackground = true;

        // Inicia a execução da thread em segundo plano.
        thread.Start();

        // Ao sair deste método, o SQL Server considera a stored procedure concluída,
        // mesmo que a thread ainda esteja executando em paralelo.
    }
}
