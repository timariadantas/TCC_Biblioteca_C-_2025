using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using Biblioteca.Domain;
using Npgsql;
using Npgsql.Replication.PgOutput.Messages;

namespace Biblioteca.Storage
{
    public interface ILoanStorage
    {
        void Create(Loan loan);
        void Delete(string id);
        List<Loan> GetAll();
        Loan? GetById(string id);
        void ReturnLoanAndRestock(string loanId, DateTime? returnDate = null);
        void Update(Loan loan);
    }

    public class LoanStorage : ILoanStorage
    {
        public void Create(Loan loan)
        {
            using var conn = DataBase.Instance.GetConnection();
            if (conn.State != ConnectionState.Open) conn.Open();

            using var tx = conn.BeginTransaction();

            try
            {
                var cmd = new NpgsqlCommand(@"
                INSERT INTO loan 
                (id, client_id, inventory_id, loan_date, due_date, return_date, status, created_at, updated_at) 
                VALUES 
                (@id, @client_id, @inventory_id, @loan_date, @due_date, @return_date, @status, @created_at, @updated_at)", conn);

                cmd.Parameters.AddWithValue("id", loan.Id);
                cmd.Parameters.AddWithValue("client_id", loan.ClientId);
                cmd.Parameters.AddWithValue("inventory_id", loan.InventoryId);
                cmd.Parameters.AddWithValue("loan_date", loan.LoanDate);
                cmd.Parameters.AddWithValue("due_date", loan.DueDate);
                cmd.Parameters.AddWithValue("return_date", (object?)loan.ReturnDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("status", loan.Status ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("created_at", loan.CreatedAt ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("updated_at", loan.UpdatedAt ?? (object)DBNull.Value);

                cmd.Transaction = tx;

                cmd.ExecuteNonQuery();

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public List<Loan> GetAll()
        {
            var loans = new List<Loan>();

            using var conn = DataBase.Instance.GetConnection();


            var cmd = new NpgsqlCommand("SELECT * FROM loan", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                loans.Add(new Loan
                {
                    Id = reader.GetString(reader.GetOrdinal("id")),
                    ClientId = reader.GetString(reader.GetOrdinal("client_id")),
                    InventoryId = reader.GetString(reader.GetOrdinal("inventory_id")),
                    LoanDate = reader.GetDateTime(reader.GetOrdinal("loan_date")),
                    DueDate = reader.GetDateTime(reader.GetOrdinal("due_date")),
                    ReturnDate = reader.IsDBNull(reader.GetOrdinal("return_date")) ? null : reader.GetDateTime(reader.GetOrdinal("return_date")),
                    Status = reader.GetString(reader.GetOrdinal("status")),
                    CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? null : reader.GetDateTime(reader.GetOrdinal("created_at")),
                    UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : reader.GetDateTime(reader.GetOrdinal("updated_at"))
                });
            }

            return loans;
        }

        public Loan? GetById(string id)
        {
            using var conn = DataBase.Instance.GetConnection();


            var cmd = new NpgsqlCommand("SELECT * FROM loan WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Loan
                {
                    Id = reader.GetString(reader.GetOrdinal("id")),
                    ClientId = reader.GetString(reader.GetOrdinal("client_id")),
                    InventoryId = reader.GetString(reader.GetOrdinal("inventory_id")),
                    LoanDate = reader.GetDateTime(reader.GetOrdinal("loan_date")),
                    DueDate = reader.GetDateTime(reader.GetOrdinal("due_date")),
                    ReturnDate = reader.IsDBNull(reader.GetOrdinal("return_date")) ? null : reader.GetDateTime(reader.GetOrdinal("return_date")),
                    Status = reader.GetString(reader.GetOrdinal("status")),
                    CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? null : reader.GetDateTime(reader.GetOrdinal("created_at")),
                    UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : reader.GetDateTime(reader.GetOrdinal("updated_at"))
                };
            }

            return null;
        }

        public void Update(Loan loan)
        {
            using var conn = DataBase.Instance.GetConnection();
            if (conn.State != ConnectionState.Open) conn.Open();

            var tx = conn.BeginTransaction();

            try
            {

                var cmd = new NpgsqlCommand(@"
                UPDATE loan SET
                    client_id = @client_id,
                    inventory_id = @inventory_id,
                    loan_date = @loan_date,
                    due_date = @due_date,
                    return_date = @return_date,
                    status = @status,
                    updated_at = @updated_at
                WHERE id = @id", conn);

                cmd.Parameters.AddWithValue("id", loan.Id);
                cmd.Parameters.AddWithValue("client_id", loan.ClientId);
                cmd.Parameters.AddWithValue("inventory_id", loan.InventoryId);
                cmd.Parameters.AddWithValue("loan_date", loan.LoanDate);
                cmd.Parameters.AddWithValue("due_date", loan.DueDate);
                cmd.Parameters.AddWithValue("return_date", (object?)loan.ReturnDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("status", loan.Status);
                cmd.Parameters.AddWithValue("updated_at", loan.UpdatedAt ?? DateTime.UtcNow);

                cmd.Transaction = tx;

                cmd.ExecuteNonQuery();

                tx.Commit();

            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public void Delete(string id)
        {
            using var conn = DataBase.Instance.GetConnection();


            var cmd = new NpgsqlCommand("DELETE FROM loan WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);

            cmd.ExecuteNonQuery();
        }


        //---- devolução ---

        public void ReturnLoanAndRestock(string loanId, DateTime? returnDate = null)
        {
            using var conn = DataBase.Instance.GetConnection();
            if (conn.State != ConnectionState.Open) conn.Open();

            using var tx = conn.BeginTransaction();

            try
            {
                var selectCmd = new NpgsqlCommand("SELECT inventory_id, status FROM loan WHERE id = @id FOR UPDATE", conn, tx);
                selectCmd.Parameters.AddWithValue("id", loanId);
                string InventoryId;
                string status;
                using (var reader = selectCmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new Exception("Emprestimo não localizado.");

                    }
                    InventoryId = reader.GetString(0);
                    status = reader.GetString(1);

                }

                if (status == "Devolvido")
                    throw new Exception("Este empréstimo já foi devolvido");

                var rd = returnDate ?? DateTime.UtcNow;

                //--> Atualizar
                var upLoan = new NpgsqlCommand(@"
                UPDATE loan 
                SET return_date = @return_date,
                status = @status,
                updated_at = @updated_at
                WHERE id = @id", conn, tx);

                upLoan.Parameters.AddWithValue("return_date", rd);
                upLoan.Parameters.AddWithValue("status", "Devolvido");
                upLoan.Parameters.AddWithValue("updated_at", DateTime.UtcNow);
                upLoan.Parameters.AddWithValue("id", loanId);

                var rowsUpdated = upLoan.ExecuteNonQuery();
                if (rowsUpdated == 0)
                    throw new Exception("Falha ao atualizar o empréstimo.");

                //--> Incrementar quantidade no inventário

                var upInv = new NpgsqlCommand(@"
                UPDATE Inventory
                SET quantity = quantity + 1,
                    updated_at = @updated_at
                WHERE id = @id", conn, tx);

                upInv.Parameters.AddWithValue("updated_at", DateTime.UtcNow);
                upInv.Parameters.AddWithValue("id", InventoryId);

                var rowsInv = upInv.ExecuteNonQuery();
                if (rowsInv == 0)
                    throw new Exception("Inventário não encontrado");

                tx.Commit();
                Console.Write("Devolução registrada e inventário atualizado");
            }
            catch (Exception ex)
            {
                tx.Rollback();
                Console.WriteLine($"Erro ao registrar devolução: {ex.Message}");
                throw;
            }

        }

        public void ListActiveLoans()
        {
            using var conn = DataBase.Instance.GetConnection();

            var sql = @"
            SELECT  id, client_id, inventory_id, status, loan_date
            FROM loan
            WHERE status <> 'Devolvido'
            ORDER BY loan_date DESC";

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            Console.WriteLine("\n ---> Empréstimos Ativos");
            while (reader.Read())
            {
                string id = reader.GetString(0);
                string clientId = reader.GetString(1);
                string inventoryId = reader.GetString(2);
                string status = reader.GetString(3);
                DateTime loanDate = reader.GetDateTime(4);

                Console.WriteLine($"ID: {id} | Cliente: {clientId} | Item: {inventoryId} | Status: {status} | Data: {loanDate: dd/MM/yyyy}");
            }
        }


        public List<Loan> GetLoansByClient(string clientId)
        {
            var loans = new List<Loan>();

            using var conn = DataBase.Instance.GetConnection();


            var sql = @"
        SELECT id, client_id, inventory_id, loan_date, due_date, return_date, status, created_at, updated_at
        FROM loan
        WHERE client_id = @client_id
        ORDER BY loan_date DESC";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("client_id", clientId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                loans.Add(new Loan
                {
                    Id = reader.GetString(0),
                    ClientId = reader.GetString(1),
                    InventoryId = reader.GetString(2),
                    LoanDate = reader.GetDateTime(3),
                    DueDate = reader.GetDateTime(4),
                    ReturnDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                    Status = reader.GetString(6),
                    CreatedAt = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7),
                    UpdatedAt = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8)
                });
            }

            return loans;

        }
    }
}

// transaction quando precisar garantir que várias operações sejam atômicas.
//Sempre que uma operação envolver mais de uma tabela ou mais de um comando que precise acontecer juntos → usar (BeginTransaction + Commit/Rollback)