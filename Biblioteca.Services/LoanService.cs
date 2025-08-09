using Biblioteca.Domain;
using Biblioteca.Storage;
using NUlid;

namespace Biblioteca.Services;

    public class LoanService
    {
        private readonly LoanStorage _loanStorage;

        public LoanService()
        {
            _loanStorage = new LoanStorage();
        }

        public void CreateLoan(Loan loan)
        {
            loan.Id = Ulid.NewUlid().ToString();
            loan.CreatedAt = DateTime.UtcNow;
            loan.UpdatedAt = DateTime.UtcNow;

            _loanStorage.Create(loan);
        }

        public Loan GetLoanById(string id)
{
    if (string.IsNullOrWhiteSpace(id))
        throw new ArgumentException("O ID do empréstimo não pode ser vazio.");

    var loan = _loanStorage.GetById(id);

    if (loan == null)
        throw new Exception($"Empréstimo com ID '{id}' não encontrado.");

    return loan;
}


        public List<Loan> ListLoans()
        {
            return _loanStorage.GetAll();
        }

        public void UpdateLoan(Loan loan)
        {
            var existingLoan = _loanStorage.GetById(loan.Id);
            if (existingLoan == null)
            {
                throw new Exception("Empréstimo não encontrado.");
            }

            loan.UpdatedAt = DateTime.UtcNow;
            _loanStorage.Update(loan);
        }

        public void DeleteLoan(string id)
        {
            var existingLoan = _loanStorage.GetById(id);
            if (existingLoan == null)
            {
                throw new Exception("Empréstimo não encontrado.");
            }

            _loanStorage.Delete(id);
        }

    public void Add(Loan loan)
    {
        throw new NotImplementedException();
    }
}
