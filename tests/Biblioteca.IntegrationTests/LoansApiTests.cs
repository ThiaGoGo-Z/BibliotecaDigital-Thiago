using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Biblioteca.Api.Contracts;
using Xunit;

namespace Biblioteca.IntegrationTests;

public class LoansApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateLoan_WithAvailableBook_ReturnsCreated()
    {
        var request = new CreateLoanRequest
        {
            BookId = 1, Borrower = "María Pérez",
            LoanDate = new DateOnly(2026, 11, 3),
            DueDate = new DateOnly(2026, 11, 17)
        };

        var response = await _client.PostAsJsonAsync("/api/loans", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateLoan_WithUnavailableBook_ReturnsBadRequest()
    {
        var request = new CreateLoanRequest
        {
            BookId = 3, Borrower = "María Pérez",
            LoanDate = new DateOnly(2026, 11, 3),
            DueDate = new DateOnly(2026, 11, 17)
        };

        var response = await _client.PostAsJsonAsync("/api/loans", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReturnLoan_UnknownId_ReturnsNotFound()
    {
        var response = await _client.PutAsync("/api/loans/9999/return", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetLoans_ReturnsOkAndLoanList()
    {
        // Act: solicitamos todos los préstamos.
        var response = await _client.GetAsync("/api/loans");

        // Assert: la API debe responder correctamente.
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Comprobamos que devuelve una lista JSON.
        var loans = await response.Content
            .ReadFromJsonAsync<List<JsonElement>>();

        Assert.NotNull(loans);
    }
    
    [Fact]
    public async Task GetLoan_WithUnknownId_ReturnsNotFound()
    {
        // Act: solicitamos un préstamo inexistente.
        var response = await _client.GetAsync("/api/loans/9999");

        // Assert: debe responder con 404 Not Found.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetLoan_WithExistingId_ReturnsOk()
    {
        // Arrange: creamos un préstamo válido.
        var request = new CreateLoanRequest
        {
            BookId = 1,
            Borrower = "Prueba de consulta",
            LoanDate = new DateOnly(2026, 11, 5),
            DueDate = new DateOnly(2026, 11, 19)
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/loans", request);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        // Obtenemos la dirección del préstamo creado.
        var location = createResponse.Headers.Location;
        Assert.NotNull(location);

        // Act: consultamos el préstamo recién creado.
        var getResponse = await _client.GetAsync(location);

        // Assert: el préstamo debe existir.
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var loan = await getResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            "Prueba de consulta",
            loan.GetProperty("borrower").GetString());
    }
        
    [Fact]
    public async Task ReturnLoan_WithActiveLoan_ReturnsOk()
    {
        // Arrange: creamos un préstamo activo.
        var request = new CreateLoanRequest
        {
            BookId = 1,
            Borrower = "Prueba de devolución",
            LoanDate = new DateOnly(2026, 11, 5),
            DueDate = new DateOnly(2026, 11, 19)
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/loans", request);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var location = createResponse.Headers.Location;
        Assert.NotNull(location);

        // Obtenemos el ID del préstamo recién creado.
        var loanId = int.Parse(
            location.ToString().TrimEnd('/').Split('/').Last());

        // Act: devolvemos el préstamo.
        var returnResponse = await _client.PutAsync(
            $"/api/loans/{loanId}/return", null);

        // Assert: la devolución debe completarse correctamente.
        Assert.Equal(HttpStatusCode.OK, returnResponse.StatusCode);
    }
    
    [Fact]
    public async Task ReturnLoan_AlreadyReturned_ReturnsConflict()
    {
        // Crear un préstamo activo.
        var request = new CreateLoanRequest
        {
            BookId = 1,
            Borrower = "Prueba devolución duplicada",
            LoanDate = new DateOnly(2026, 11, 5),
            DueDate = new DateOnly(2026, 11, 19)
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/loans", request);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var location = createResponse.Headers.Location;
        Assert.NotNull(location);

        var loanId = int.Parse(
            location.ToString().TrimEnd('/').Split('/').Last());

        // Primera devolución: correcta.
        var firstReturn = await _client.PutAsync(
            $"/api/loans/{loanId}/return", null);

        Assert.Equal(HttpStatusCode.OK, firstReturn.StatusCode);

        // Segunda devolución: debe rechazarse.
        var secondReturn = await _client.PutAsync(
            $"/api/loans/{loanId}/return", null);

        Assert.Equal(HttpStatusCode.Conflict, secondReturn.StatusCode);
    }

    [Fact]
    public async Task CreateLoan_WithoutDates_ReturnsBadRequest()
    {
        // Enviamos un JSON sin las fechas obligatorias.
        var request = new
        {
            BookId = 1,
            Borrower = "Prueba sin fechas"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/loans", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateLoan_WithInvalidBookId_ReturnsBadRequest()
    {
        // BookId debe ser mayor que cero.
        var request = new CreateLoanRequest
        {
            BookId = 0,
            Borrower = "Prueba ID inválido",
            LoanDate = new DateOnly(2026, 11, 5),
            DueDate = new DateOnly(2026, 11, 19)
        };

        var response = await _client.PostAsJsonAsync(
            "/api/loans", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateLoan_WithEmptyBorrower_ReturnsBadRequest()
    {
        // El nombre del prestatario es obligatorio.
        var request = new CreateLoanRequest
        {
            BookId = 1,
            Borrower = "",
            LoanDate = new DateOnly(2026, 11, 5),
            DueDate = new DateOnly(2026, 11, 19)
        };

        var response = await _client.PostAsJsonAsync(
            "/api/loans", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


}
