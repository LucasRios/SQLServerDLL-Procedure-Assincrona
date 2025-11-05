# SQLProcedureAssincrona: Execute comandos SQL de forma assíncrona direto do SQL Server! ⚙️

Este projeto permite executar comandos SQL de maneira assíncrona diretamente a partir do SQL Server, utilizando procedimentos armazenados CLR (Common Language Runtime).
Ideal para disparar tarefas demoradas (como atualizações, logs ou integrações externas) sem bloquear a transação principal do banco. 🚀

---

## 🛠️ Funcionalidades

- ⚡ Executa comandos SQL em thread separada (não bloqueia o SQL Server)
- 🔁 Permite rodar instruções em outros bancos da mesma instância ou remotos
- 🧩 Suporte completo a SqlProcedure com parâmetros nativos do SQL Server
- 🧱 Código simples, robusto e facilmente extensível

---

## 📦 Requisitos

- SQL Server com **CLR Integration** habilitada
- Projeto compilado como **Assembly .NET Framework** com permissões `UNSAFE`
- Permissões de rede no servidor SQL para acessar APIs externas

---

## 📥 Instalação

1. **Habilite o CLR no SQL Server (se ainda não estiver ativo):**

```sql
sp_configure 'clr enabled', 1;
RECONFIGURE;
```
Compile o código como DLL.

Registre o assembly no banco de dados:

```sql
CREATE ASSEMBLY SQLProcedureAssincrona
FROM 'C:\caminho\para\SQLProcedureAssincrona.dll'
WITH PERMISSION_SET = UNSAFE;
```
Crie os procedimentos:

 ```sql  
CREATE PROCEDURE dbo.ExecuteAsync
    @DatabaseName NVARCHAR(255),
    @CommandText NVARCHAR(MAX)
AS EXTERNAL NAME SQLProcedureAssincrona.[SQLProcedureAssincrona].ExecuteAsync;
```

## 🚀 Como Usar
🔧 Exemplo de Execução Assíncrona
  ```sql
EXEC dbo.ExecuteAsync
    @DatabaseName = N'MeuBancoDeDados',
    @CommandText = N'UPDATE Logs SET Processado = 1 WHERE Data < GETDATE() - 30';
```

## ➡️ O comando acima será executado em uma thread separada, permitindo que o SQL continue outras operações normalmente sem esperar o término da execução.

## ⚠️ Observações Importantes

- 🔐 Atenção com credenciais: o exemplo contém usuário e senha no código-fonte. Use variáveis de ambiente ou arquivo de configuração seguro.
- 💣 Evite SQL Injection: o parâmetro @CommandText aceita qualquer SQL, então nunca use entrada direta de usuário.
- 🧱 Threads não são gerenciadas pelo SQL Server: use com moderação para evitar sobrecarga de threads simultâneas.
- 🧾 Adicione logs: recomenda-se registrar exceções e eventos em arquivo ou tabela de auditoria.
- 🌐 Configuração de servidor: ajuste IP, credenciais e parâmetros de conexão conforme seu ambiente.
 
## 📜 Licença

Distribuído gratuitamente para fins educacionais e profissionais.
Sinta-se livre para contribuir, adaptar ou melhorar conforme sua necessidade. 🤝

Feito com ⚙️ C#, 💾 SQL Server e curiosidade infinita 💡
