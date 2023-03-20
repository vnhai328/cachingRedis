using CachingWebApi.Data;
using CachingWebApi.Models;
using CachingWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CachingWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DriversController : ControllerBase
{

    private readonly ILogger<DriversController> _logger;
    private readonly ICacheService _cacheService;
    private readonly AppDBContext _context;

    public DriversController(ILogger<DriversController> logger, ICacheService cacheService, AppDBContext context)
    {
        _logger = logger;
        _cacheService = cacheService;
        _context = context;
    }

    [HttpGet("drivers")]
    public async Task<IActionResult> Get()
    {
        //check cache data
        var cacheData = _cacheService.GetData<IEnumerable<Driver>>("drivers");

        if(cacheData != null && cacheData.Count() > 0)
        {
            return Ok(cacheData);
        }

        cacheData = await _context.Drivers.ToListAsync();

        var expirtyTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData<IEnumerable<Driver>>("drivers", cacheData, expirtyTime);

        return Ok(cacheData);
    }

    [HttpPost("AddDriver")]
    public async Task<IActionResult> Post(Driver value)
    {
        var addedObj = await _context.Drivers.AddAsync(value);

        var expirtyTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData<Driver>($"driver{value.Id}", addedObj.Entity, expirtyTime);
        await _context.SaveChangesAsync();

        return Ok(addedObj.Entity);
    }

    [HttpDelete("DeleteDriver")]
    public async Task<IActionResult> Delete(int id){
        var exits = await _context.Drivers.FirstOrDefaultAsync(x => x.Id == id);

        if(exits != null )
        {
            _context.Remove(exits);
            _cacheService.RemoveData($"driver{id}");
            await _context.SaveChangesAsync();

            return NoContent();
        }
        return NotFound();
    }
}
