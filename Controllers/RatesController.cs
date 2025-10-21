using AutoMapper;
using CrudPark.API.DTOs;
using CrudPark.API.Models;
using CrudPark.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrudPark.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatesController : ControllerBase
{
    private readonly IRateService _rateService;
    private readonly IMapper _mapper;

    public RatesController(IRateService rateService, IMapper mapper)
    {
        _rateService = rateService;
        _mapper = mapper;
    }

    // GET: Obtener todas las tarifas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RateResponseDto>>> GetAllRates()
    {
        var rates = await _rateService.GetAllRatesAsync();
        // Mapear la colección de Modelos a DTOs de Respuesta
        var responseDtos = _mapper.Map<IEnumerable<RateResponseDto>>(rates);
        return Ok(responseDtos);
    }

    // GET: Obtener tarifa por ID
    [HttpGet("{id}")]
    public async Task<ActionResult<RateResponseDto>> GetRateById(int id)
    {
        var rate = await _rateService.GetRateByIdAsync(id);
        
        if (rate == null)
            return NotFound(new { message = $"No se encontró la tarifa con ID {id}" });
        
        // Mapear el Modelo a DTO de Respuesta
        var responseDto = _mapper.Map<RateResponseDto>(rate);
        return Ok(responseDto);
    }

    // POST: Crear una nueva tarifa
    [HttpPost]
    public async Task<ActionResult<RateResponseDto>> CreateRate([FromBody] RateCreateDto dto)
    {
        // 1. Mapear DTO de entrada a Modelo de Dominio
        var rateToCreate = _mapper.Map<Rate>(dto);

        try
        {
            // 2. Llamar al servicio (aquí se aplica la lógica de unicidad)
            var created = await _rateService.CreateRateAsync(rateToCreate);
            
            // 3. Mapear la entidad creada a DTO de Respuesta
            var responseDto = _mapper.Map<RateResponseDto>(created); 

            // Devolver 201 CreatedAtAction
            return CreatedAtAction(nameof(GetRateById), new { id = responseDto.Id }, responseDto);
        }
        catch (InvalidOperationException ex)
        {
            // Capturar el error de negocio (ej. "Ya existe una tarifa activa para este tipo de vehículo")
            return Conflict(new { message = ex.Message }); 
        }
    }

    
    // PUT: Actualizar una tarifa existente
    [HttpPut("{id}")]
    public async Task<ActionResult<RateResponseDto>> UpdateRate(int id, [FromBody] RateUpdateDto dto)
    {
        // 1. Mapear DTO de entrada a Modelo de Dominio
        var rateToUpdate = _mapper.Map<Rate>(dto);

        try
        {
            // 2. Llamar al servicio, pasando el ID de la URL y la entidad mapeada
            var updated = await _rateService.UpdateRateAsync(id, rateToUpdate);
        
            // 3. Mapear la entidad resultante a DTO de Respuesta
            var responseDto = _mapper.Map<RateResponseDto>(updated);
        
            return Ok(responseDto); // Devolver 200 OK
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // Capturar el error de negocio (ej. intento de activar una tarifa con otra ya activa)
            return Conflict(new { message = ex.Message });
        }
    }

    // DELETE: Eliminar una tarifa
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRate(int id)
    {
        var success = await _rateService.DeleteRateAsync(id);
        
        if (!success)
        {
            return NotFound(new { message = $"No se encontró la tarifa con ID {id} para eliminar." });
        }

        return NoContent(); // 204 No Content
    }
}