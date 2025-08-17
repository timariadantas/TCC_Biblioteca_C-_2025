using System;
using System.Collections.Generic;
using System.Data;
using Biblioteca.Domain;
using Npgsql;

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
        List<Loan> GetLoansByClient(string clientId);
        List<Loan> GetActiveAndOverdueLoans();
        void ListActiveLoans();
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
                using var cmd = new NpgsqlCommand(@"
                    INSERT INTO loan 
                    (id, client_id, inventory_id, loan_date, due_date, return_date, status, created_at, updated_at) 
                    VALUES 
                    (@id, @client_id, @inventory_id, @loan_date, @due_date, @return_date, @status, @created_at, @updated_at)", conn, tx);

                cmd.Parameters.AddWithValue("id", loan.Id);
                cmd.Parameters.AddWithValue("client_id", loan.ClientId);
                cmd.Parameters.AddWithValue("inventory_id", loan.InventoryId);
                cmd.Parameters.AddWithValue("loan_date", loan.LoanDate);
                cmd.Parameters.AddWithValue("due_date", loan.DueDate);
                cmd.Parameters.AddWithValue("return_date", (object?)loan.ReturnDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("status", loan.Status ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("created_at", loan.CreatedAt ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("updated_at", loan.UpdatedAt ?? (object)DBNull.Value);

                cmd.ExecuteNonQuery();
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public void Update(Loan loan)
        {
            using var conn = DataBase.Instance.GetConnection();
            if (conn.State != ConnectionState.Open) conn.Open();

            using var tx = conn.BeginTransaction();

            try
            {
                using var cmd = new NpgsqlCommand(@"
                    UPDATE loan SET
                        client_id = @client_id,
                        inventory_id = @inventory_id,
                        loan_date = @loan_date,
                        due_date = @due_date,
                        return_date = @return_date,
                        status = @status,
                        updated_at = @updated_at
                    WHERE id = @id", conn, tx);

                cmd.Parameters.AddWithValue("id", loan.Id);
                cmd.Parameters.AddWithValue("client_id", loan.ClientId);
                cmd.Parameters.AddWithValue("inventory_id", loan.InventoryId);
                cmd.Parameters.AddWithValue("loan_date", loan.LoanDate);
                cmd.Parameters.AddWithValue("due_date", loan.DueDate);
                cmd.Parameters.AddWithValue("return_date", (object?)loan.ReturnDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("status", loan.Status ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("updated_at", loan.UpdatedAt ?? DateTime.UtcNow);

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
            if (conn.State != ConnectionState.Open) conn.Open();

            using var cmd = new NpgsqlCommand("DELETE FROM loan WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.ExecuteNonQuery();
        }

        public Loan? GetById(string id)
        {
            using var conn = DataBase.Instance.GetConnection();
            if (conn.State != ConnectionState.Open) conn.Open();

            using var cmd = new NpgsqlCommand("SELECT * FROM loan WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return MapReaderToLoan(reader);
            }

            return null;
        }

        public List<Loan> GetAll()
        {
            var loans = new List<Loan>();

            using var conn = DataBase.Instance.GetConnection();
            if (conn.State != ConnectionState.Open) conn.Open();

            using var cmd = new NpgsqlCommand("SELECT * FROM loan", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                loans.Add(MapReaderToLoan(reader));
            }

            return loans;
        }

        public List<Loan> GetLoansByClient(string clientId)
        {
            var loans = new List<Loan>();

            using var conn = DataBase.Instance.GetConnection();
            if (conn.State != ConnectionState.Open) conn.Open();

            using var cmd = new NpgsqlCommand(@"
                SELECT id, client_id, inventory_id, loan_date, due_date, return_date, status, created_at, updated_at
                FROM loan
                WHERE client_id = @client_id
                ORDER BY loan_date DESC", conn);

            cmd.Parameters.AddWithValue("client_id", clientId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                loans.Add(MapReaderToLoan(reader));
            }

            return loans;
        }

        public List<Loan> GetActiveAndOverdueLoans()
        {
            var loans = new List<Loan>();

            using var conn = DataBase.Instance.GetConnection();

            using var cmd = new NpgsqlCommand(@"
                SELECT *
                FROM loan
                WHERE status <> 'Devolvido'
                ORDER BY due_date ASC", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                loans.Add(MapReaderToLoan(reader));
            }

            return loans;
        }

        public void ListActiveLoans()
        {
            using var conn = DataBase.Instance.GetConnection();
            if (conn.State != ConnectionState.Open) conn.Open();

            using var cmd = new NpgsqlCommand(@"
                SELECT id, client_id, inventory_id, status, loan_date
                FROM loan
                WHERE status <> 'Devolvido'
                ORDER BY loan_date DESC", conn);

            using var reader = cmd.ExecuteReader();

            Console.WriteLine("\n ---> Empréstimos Ativos");
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader.GetString(0)} | Cliente: {reader.GetString(1)} | Item: {reader.GetString(2)} | Status: {reader.GetString(3)} | Data: {reader.GetDateTime(4):dd/MM/yyyy}");
            }
        }

        public void ReturnLoanAndRestock(string loanId, DateTime? returnDate = null)
        {
            using var conn = DataBase.Instance.GetConnection();
            if (conn.State != ConnectionState.Open) conn.Open();

            using var tx = conn.BeginTransaction();

            try
            {
                // Seleciona empréstimo
                using var selectCmd = new NpgsqlCommand("SELECT inventory_id, status FROM loan WHERE id = @id FOR UPDATE", conn, tx);
                selectCmd.Parameters.AddWithValue("id", loanId);

                string inventoryId;
                string status;

                using (var reader = selectCmd.ExecuteReader())
                {
                    if (!reader.Read())
                        throw new Exception("Empréstimo não localizado.");

                    inventoryId = reader.GetString(0);
                    status = reader.GetString(1);
                }

                if (status == "Devolvido")
                    throw new Exception("Este empréstimo já foi devolvido.");

                var rd = returnDate ?? DateTime.UtcNow;

                // Atualiza empréstimo
                using var upLoan = new NpgsqlCommand(@"
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

                // Atualiza inventário
                using var upInv = new NpgsqlCommand(@"
                    UPDATE Inventory
                    SET quantity = quantity + 1,
                        updated_at = @updated_at
                    WHERE id = @id", conn, tx);

                upInv.Parameters.AddWithValue("updated_at", DateTime.UtcNow);
                upInv.Parameters.AddWithValue("id", inventoryId);

                var rowsInv = upInv.ExecuteNonQuery();
                if (rowsInv == 0)
                    throw new Exception("Inventário não encontrado.");

                tx.Commit();
                Console.WriteLine("Devolução registrada e inventário atualizado");
            }
            catch (Exception ex)
            {
                tx.Rollback();
                Console.WriteLine($"Erro ao registrar devolução: {ex.Message}");
                throw;
            }
        }

        // Helper para mapear reader para Loan
        private Loan MapReaderToLoan(NpgsqlDataReader reader)
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
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("created_at")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("updated_at"))
            };
        }
    }
}
