using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using Xunit;

namespace api_1.Tests;

public class WeatherForecastTests
{
    [Fact]
    public void WeatherForecast_TemperatureF_ConvertsCorrectly()
    {
        // Arrange
        var temperatureC = 0;
        var expected = 32;
        var forecast = new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), temperatureC, "Freezing");

        // Act
        var result = forecast.TemperatureF;

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, 32)]
    [InlineData(10, 49)]
    [InlineData(20, 68)]
    [InlineData(30, 86)]
    [InlineData(-10, 14)]
    public void WeatherForecast_TemperatureF_ConvertsMultipleValues(int temperatureC, int expected)
    {
        // Arrange
        var forecast = new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), temperatureC, "Test");

        // Act
        var result = forecast.TemperatureF;

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void WeatherForecast_CreatesRecordWithCorrectProperties()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.Now);
        var temperatureC = 25;
        var summary = "Warm";

        // Act
        var forecast = new WeatherForecast(date, temperatureC, summary);

        // Assert
        Assert.Equal(date, forecast.Date);
        Assert.Equal(temperatureC, forecast.TemperatureC);
        Assert.Equal(summary, forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_AllowsNullSummary()
    {
        // Arrange & Act
        var forecast = new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), 20, null);

        // Assert
        Assert.Null(forecast.Summary);
    }
}

public class WeatherForecastEndpointTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/weatherforecast");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsJsonContentType()
    {
        // Act
        var response = await _client.GetAsync("/weatherforecast");

        // Assert
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsFiveForecasts()
    {
        // Act
        var response = await _client.GetAsync("/weatherforecast");
        var content = await response.Content.ReadAsStringAsync();
        var forecasts = JsonSerializer.Deserialize<WeatherForecast[]>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert
        Assert.NotNull(forecasts);
        Assert.Equal(5, forecasts.Length);
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsValidTemperatureRange()
    {
        // Act
        var response = await _client.GetAsync("/weatherforecast");
        var content = await response.Content.ReadAsStringAsync();
        var forecasts = JsonSerializer.Deserialize<WeatherForecast[]>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert
        Assert.NotNull(forecasts);
        foreach (var forecast in forecasts)
        {
            Assert.InRange(forecast.TemperatureC, -20, 55);
        }
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsForecastsWithValidSummaries()
    {
        // Arrange
        var validSummaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

        // Act
        var response = await _client.GetAsync("/weatherforecast");
        var content = await response.Content.ReadAsStringAsync();
        var forecasts = JsonSerializer.Deserialize<WeatherForecast[]>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert
        Assert.NotNull(forecasts);
        foreach (var forecast in forecasts)
        {
            Assert.Contains(forecast.Summary, validSummaries);
        }
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsFutureDate()
    {
        // Act
        var response = await _client.GetAsync("/weatherforecast");
        var content = await response.Content.ReadAsStringAsync();
        var forecasts = JsonSerializer.Deserialize<WeatherForecast[]>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert
        Assert.NotNull(forecasts);
        var today = DateOnly.FromDateTime(DateTime.Now);
        foreach (var forecast in forecasts)
        {
            Assert.True(forecast.Date > today, $"Expected date {forecast.Date} to be greater than today {today}");
        }
    }
}
