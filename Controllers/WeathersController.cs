using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cancellation_token_demo.Controllers;

[ApiController]
[Route("tasks")]
public class WeathersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<WeathersController> _logger;

    public WeathersController(
        AppDbContext context,
        ILogger<WeathersController> logger
    )
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("no-cancellation")]
    public async Task<IActionResult> Get()
    {
        await GetWeatherAsync();
        return Ok();
    }

    [HttpGet("with-cancellation-source")]
    public async Task<IActionResult> GetWithCancellationSource()
    {
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await GetWeatherAsync(cancellationTokenSource.Token);
        return Ok();
    }

    [HttpGet("with-request-cancellation")]
    public async Task<IActionResult> GetWithRequestCancellation()
    {
        await GetWeatherAsync(HttpContext.RequestAborted);
        return Ok();
    }

    [HttpGet("with-ef-query-cancellation")]
    public async Task<IActionResult> GetWithEfQueryCancellation()
    {
        await _context.Database
            .ExecuteSqlRawAsync($"WAITFOR DELAY '00:00:10'; SELECT 1", cancellationToken: HttpContext.RequestAborted);
        return Ok();
    }

    private async Task GetWeatherAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            for (int i = 0; i < 30; i++)
            {
                await Task.Delay(1000, cancellationToken);
                _logger.LogInformation($"Weather {i}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}