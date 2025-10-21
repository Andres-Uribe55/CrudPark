using AutoMapper;
using CrudPark.API.DTOs;
using CrudPark.API.Models;
using CrudPark.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrudPark.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly IMapper _mapper;

    public TicketsController(ITicketService ticketService, IMapper mapper)
    {
        _ticketService = ticketService;
        _mapper = mapper;
    }

    // 1. REGISTRAR ENTRADA (Crear Ticket)
    // POST /api/tickets/entry
    [HttpPost("entry")]
    public async Task<ActionResult<TicketResponseDto>> RegisterEntry([FromBody] TicketEntryDto dto)
    {
        // 1. Mapear DTO de entrada a Modelo.
        var ticketToCreate = _mapper.Map<Ticket>(dto);

        try
        {
            // 2. Llamar al servicio, que verifica membresía, genera folio y hora de entrada.
            var createdTicket = await _ticketService.RegisterEntryAsync(ticketToCreate);
            
            // 3. Devolver la respuesta con el Folio y la hora de entrada.
            var responseDto = _mapper.Map<TicketResponseDto>(createdTicket);
            return CreatedAtAction(nameof(GetTicketStatus), new { identifier = responseDto.Folio }, responseDto);
        }
        catch (InvalidOperationException ex)
        {
            // Capturar el error si ya existe un ticket activo para esa placa.
            return Conflict(new { message = ex.Message }); 
        }
        // Nota: Las validaciones de modelo (DataAnnotations) se manejan automáticamente (400 Bad Request).
    }
    
    // 2. REGISTRAR SALIDA (Calcular Costo y Finalizar)
    // POST /api/tickets/exit
    [HttpPost("exit")]
    public async Task<ActionResult<TicketResponseDto>> RegisterExit([FromBody] TicketExitDto dto)
    {
        // El identificador es la Placa o el Folio. Damos preferencia a la Placa si ambos existen.
        string identifier = dto.LicensePlate ?? dto.Folio;

        if (string.IsNullOrEmpty(identifier))
        {
            return BadRequest(new { message = "Se requiere una placa o un folio para registrar la salida." });
        }
        
        try
        {
            // 1. Llamar al servicio, que busca, calcula el costo, registra la salida y guarda.
            var completedTicket = await _ticketService.RegisterExitAsync(identifier, dto.ExitOperatorId);
            
            // 2. Devolver el ticket finalizado, que incluye el TotalCost y TotalMinutes.
            return Ok(_mapper.Map<TicketResponseDto>(completedTicket));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // Capturar errores de lógica, como la falta de una tarifa activa.
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // 3. CONSULTAR ESTADO (Para la aplicación Java o el lector de tickets)
    // GET /api/tickets/{identifier}
    [HttpGet("{identifier}")]
    public async Task<ActionResult<TicketResponseDto>> GetTicketStatus(string identifier)
    {
        var ticket = await _ticketService.GetTicketStatusAsync(identifier);

        if (ticket == null)
            return NotFound(new { message = $"Ticket activo con identificador '{identifier}' no encontrado." });
        
        // Opcional: Podrías añadir lógica aquí para simular el cálculo de costo
        // en tiempo real antes de la salida, pero por ahora solo devolvemos el ticket activo.

        return Ok(_mapper.Map<TicketResponseDto>(ticket));
    }
}