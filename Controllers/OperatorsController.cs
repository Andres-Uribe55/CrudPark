using AutoMapper;
using CrudPark.API.DTOs;
using CrudPark.API.Models;
using CrudPark.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrudPark.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OperatorsController : ControllerBase
{
    private readonly IOperatorService _operatorService;
    private readonly IMapper _mapper;

    public OperatorsController(IOperatorService operatorService, IMapper mapper)
    {
        _operatorService = operatorService;
        _mapper = mapper;
    }

    // =======================================================
    // 1. CRUD ADMINISTRATIVO
    // =======================================================

    // GET: Obtener todos los operadores (para el panel de administración)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OperatorResponseDto>>> GetAllOperators()
    {
        var operators = await _operatorService.GetAllAsync();
        // Mapear la colección de Modelos a DTOs de Respuesta (excluyendo el hash de la contraseña)
        return Ok(_mapper.Map<IEnumerable<OperatorResponseDto>>(operators));
    }

    // GET: Obtener operador por ID
    [HttpGet("{id}")]
    public async Task<ActionResult<OperatorResponseDto>> GetOperatorById(int id)
    {
        var @operator = await _operatorService.GetByIdAsync(id);
        
        if (@operator == null)
            return NotFound(new { message = $"Operador con ID {id} no encontrado." });
        
        return Ok(_mapper.Map<OperatorResponseDto>(@operator));
    }
    
    // POST: Crear nuevo operador
    [HttpPost]
    public async Task<ActionResult<OperatorResponseDto>> CreateOperator([FromBody] OperatorCreateDto dto)
    {
        // 1. Mapear DTO de entrada a Modelo. (El campo Password en el Modelo AÚN es texto plano aquí)
        var operatorToCreate = _mapper.Map<Operator>(dto);

        try
        {
            // 2. Llamar al servicio, donde la contraseña será HASHEADA y guardada.
            var created = await _operatorService.CreateAsync(operatorToCreate);
            
            // 3. Devolver la respuesta (la contraseña hasheada NO se incluye en el DTO de respuesta)
            var responseDto = _mapper.Map<OperatorResponseDto>(created); 
            return CreatedAtAction(nameof(GetOperatorById), new { id = responseDto.Id }, responseDto);
        }
        catch (InvalidOperationException ex)
        {
            // Capturar el error de unicidad (Username ya existe)
            return Conflict(new { message = ex.Message }); 
        }
    }

    // PUT: Actualizar datos personales y estado de actividad
    [HttpPut("{id}")]
    public async Task<ActionResult<OperatorResponseDto>> UpdateOperator(int id, [FromBody] OperatorUpdateDto dto)
    {
        var operatorToUpdate = _mapper.Map<Operator>(dto);

        try
        {
            var updated = await _operatorService.UpdateAsync(id, operatorToUpdate);
            
            return Ok(_mapper.Map<OperatorResponseDto>(updated));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // Capturar el error de unicidad (Username ya está en uso)
            return Conflict(new { message = ex.Message });
        }
    }
    
    // =======================================================
    // 2. OPERACIÓN DE SEGURIDAD
    // =======================================================
    
    // PUT: Actualizar Contraseña
    [HttpPut("{id}/password")]
    public async Task<ActionResult> UpdatePassword(int id, [FromBody] OperatorPasswordUpdateDto dto)
    {
        // Nota: Las validaciones de Password y ConfirmNewPassword se hacen automáticamente por el [ApiController]
        
        // 1. El servicio hashea y actualiza la contraseña en la DB.
        bool success = await _operatorService.UpdatePasswordAsync(id, dto.NewPassword);
        
        if (!success)
        {
            return NotFound(new { message = $"Operador con ID {id} no encontrado." });
        }

        return NoContent(); // 204 indica éxito sin contenido de respuesta
    }

    // =======================================================
    // 3. ENDPOINT DE AUTENTICACIÓN (LOGIN)
    // =======================================================
    
    // POST: /api/operators/authenticate
    [HttpPost("authenticate")]
    public async Task<ActionResult<OperatorResponseDto>> Authenticate([FromBody] LoginDto dto)
    {
        // *Nota: Necesitarás crear un 'LoginDto' simple con propiedades Username y Password.*
        
        var @operator = await _operatorService.AuthenticateAsync(dto.Username, dto.Password);

        if (@operator == null)
        {
            // Devolver 401 si no se encuentra el usuario, no está activo o la contraseña es incorrecta
            return Unauthorized(new { message = "Nombre de usuario o contraseña incorrectos." }); 
        }
        
        // Si la autenticación es exitosa, devolver los datos del operador (sin el hash)
        return Ok(_mapper.Map<OperatorResponseDto>(@operator));
    }
}