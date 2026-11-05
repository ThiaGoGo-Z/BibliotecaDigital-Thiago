using System.Net;
using System.Net.Http.Json;
using Biblioteca.Api.Contracts;
using Xunit;

namespace Biblioteca.IntegrationTests;

public class BooksApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetBooks_ReturnsSeededCatalog()
    {
        var response = await _client.GetAsync("/api/books");
        response.EnsureSuccessStatusCode();

        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        Assert.NotNull(books);
        Assert.True(books.Count >= 3);
        Assert.Contains(books, x => x.Title == "Cien años de soledad");
    }

    [Fact]
    public async Task GetBook_WithUnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/books/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateBook_WithValidData_ReturnsCreated()
    {
        var request = new CreateBookRequest
        {
            Title = "El código Da Vinci", Author = "Dan Brown",
            Isbn = "9780307474278", PublishedYear = 2003, AvailableCopies = 4
        };

        var response = await _client.PostAsJsonAsync("/api/books", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task CreateBook_WithDuplicateIsbn_ReturnsConflict()
    {
        var request = new CreateBookRequest
        {
            Title = "Duplicado", Author = "Autor",
            Isbn = "9780307474728", PublishedYear = 2020, AvailableCopies = 1
        };

        var response = await _client.PostAsJsonAsync("/api/books", request);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteBook_WithUnknownId_ReturnsNotFound()
    {
        // Act: intentamos eliminar un libro que no existe.
        var response = await _client.DeleteAsync("/api/books/9999");

        // Assert: la API debe responder con 404 Not Found.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteBook_WithActiveLoan_ReturnsConflict()
    {
        // Arrange: crear un libro nuevo.
        var bookRequest = new CreateBookRequest
        {
            Title = "Libro con préstamo activo",
            Author = "Autor de prueba",
            Isbn = "978" + Random.Shared.NextInt64(
                0, 10_000_000_000L).ToString("D10"),
            PublishedYear = 2024,
            AvailableCopies = 2
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/books", bookRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var book = await createResponse.Content
            .ReadFromJsonAsync<BookResponse>();

        Assert.NotNull(book);

        // Crear un préstamo activo de ese libro.
        var loanRequest = new CreateLoanRequest
        {
            BookId = book.Id,
            Borrower = "Alumno de prueba",
            LoanDate = DateOnly.FromDateTime(DateTime.Today),
            DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7))
        };

        var loanResponse = await _client.PostAsJsonAsync(
            "/api/loans", loanRequest);

        Assert.Equal(HttpStatusCode.Created, loanResponse.StatusCode);

        // Act: intentar eliminar el libro prestado.
        var deleteResponse = await _client.DeleteAsync(
            $"/api/books/{book.Id}");

        // Assert: debe rechazar la eliminación.
        Assert.Equal(HttpStatusCode.Conflict, deleteResponse.StatusCode);

        // Comprobamos también que el libro sigue existiendo.
        var getResponse = await _client.GetAsync(
            $"/api/books/{book.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteBook_WithoutActiveLoans_ReturnsNoContent()
    {
        // Arrange: creamos un libro sin préstamos.
        var request = new CreateBookRequest
        {
            Title = "Libro para eliminar",
            Author = "Autor de prueba",
            Isbn = "978" + Random.Shared.NextInt64(
                0, 10_000_000_000L).ToString("D10"),
            PublishedYear = 2024,
            AvailableCopies = 2
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/books", request);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var book = await createResponse.Content
            .ReadFromJsonAsync<BookResponse>();

        Assert.NotNull(book);

        // Act: eliminamos el libro.
        var deleteResponse = await _client.DeleteAsync(
            $"/api/books/{book.Id}");

        // Assert: la eliminación debe ser correcta.
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Comprobamos que el libro ya no existe.
        var getResponse = await _client.GetAsync(
            $"/api/books/{book.Id}");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private sealed record BookResponse(int Id, string Title, string Author, string Isbn, int PublishedYear, int AvailableCopies);
}
