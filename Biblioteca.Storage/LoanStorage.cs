using Biblioteca.Domain;
using Npgsql;

namespace Biblioteca.Storage
{
    public class LoanStorage
    {
        public void Create(Loan loan)
        {
            using var conn = DataBase.GetConnection();
            

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

            cmd.ExecuteNonQuery();
        }

        public List<Loan> GetAll()
        {
            var loans = new List<Loan>();

            using var conn = DataBase.GetConnection();
            

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
            using var conn = DataBase.GetConnection();
            

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
            using var conn = DataBase.GetConnection();
            

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

            cmd.ExecuteNonQuery();
        }

        public void Delete(string id)
        {
            using var conn = DataBase.GetConnection();
            

            var cmd = new NpgsqlCommand("DELETE FROM loan WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);

            cmd.ExecuteNonQuery();
        }
    }
}
