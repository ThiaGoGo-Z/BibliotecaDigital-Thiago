
using Biblioteca.Api.Contracts;
using Biblioteca.Api.Data;
using Biblioteca.Api.Models;
using Biblioteca.Api.Services;
using Xunit;

namespace Biblioteca.UnitTests;

public class CoverageTests
{
    [Fact]
    public void UpdateBook_UnknownId_DoesNotModifyRepository()
    {
        // Intentar actualizar un libro inexistente.
        var repository = new InMemoryBookRepository();
        var initialCount = repository.GetAll().Count;

        repository.Update(new Book
        {
            Id = 9999,
            Title = "Libro inexistente",
            Author = "Autor de prueba",
            Isbn = "9999999999999"
        });

        Assert.Equal(initialCount, repository.GetAll().Count);
        Assert.Null(repository.GetById(9999));
    }

    [Fact]
    public void UpdateLoan_UnknownId_DoesNotModifyRepository()
    {
        // Intentar actualizar un préstamo inexistente.
        var repository = new InMemoryLoanRepository();

        repository.Update(new Loan
        {
            Id = 9999,
            BookId = 1,
            Borrower = "Alumno de prueba",
            Status = "Activo"
        });

        Assert.Null(repository.GetById(9999));
        Assert.Empty(repository.GetAll());
    }

    [Fact]
    public void CreateLoan_UnknownBook_ReturnsError()
    {
        // Intentar prestar un libro que no existe.
        var books = new InMemoryBookRepository();
        var loans = new InMemoryLoanRepository();
        var service = new LibraryService(books, loans);

        var request = new CreateLoanRequest
        {
            BookId = 9999,
            Borrower = "Alumno de prueba",
            LoanDate = new DateOnly(2026, 11, 5),
            DueDate = new DateOnly(2026, 11, 19)
        };

        var result = service.CreateLoan(request);

        Assert.Null(result.Loan);
        Assert.Equal(
            "No existe el libro indicado.",
            result.Error);
    }
}
