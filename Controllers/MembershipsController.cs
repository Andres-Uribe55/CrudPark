using AutoMapper;
using CrudPark.API.Models;
using CrudPark.API.Services;
using CrudPark.API.DTOs; 
using Microsoft.AspNetCore.Mvc;

namespace CrudPark.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembershipsController : ControllerBase
{
    private readonly IMembershipService _membershipService;
    private readonly IMapper _mapper;

    public MembershipsController(IMembershipService membershipService, IMapper mapper)
    {
        _membershipService = membershipService;
        _mapper = mapper;
    }
    
    // GET: api/memberships
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MembershipResponseDto>>> GetAllMemberships()
    {
        var memberships = await _membershipService.GetAllMembershipsAsync();
        // Mapear la colección a DTOs
        var responseDtos = _mapper.Map<IEnumerable<MembershipResponseDto>>(memberships);
        return Ok(responseDtos);
    }

    // GET: api/memberships/5
    [HttpGet("{id}")]
    public async Task<ActionResult<MembershipResponseDto>> GetMembershipById(int id)
    {
        var membership = await _membershipService.GetMembershipByIdAsync(id);
        
        if (membership == null)
            return NotFound(new { message = $"No se encontró la mensualidad con ID {id}" });
        
        // Mapear la entidad única a DTO
        var responseDto = _mapper.Map<MembershipResponseDto>(membership);
        return Ok(responseDto);
    }

    // GET: api/memberships/search?plate=ABC123
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<MembershipResponseDto>>> SearchByLicensePlate([FromQuery] string plate)
    {
        if (string.IsNullOrWhiteSpace(plate))
            return BadRequest(new { message = "La placa es requerida" });

        var memberships = await _membershipService.SearchByLicensePlateAsync(plate);
        //  Mapear la colección a DTOs
        var responseDtos = _mapper.Map<IEnumerable<MembershipResponseDto>>(memberships);
        return Ok(responseDtos);
    }

    // POST: api/memberships (Ya estaba correcto)
    [HttpPost]
    public async Task<ActionResult<MembershipResponseDto>> CreateMembership([FromBody] MembershipCreateDto dto)
    {
        var membershipToCreate = _mapper.Map<Membership>(dto); 

        try
        {
            var created = await _membershipService.CreateMembershipAsync(membershipToCreate);
            
            var responseDto = _mapper.Map<MembershipResponseDto>(created); 

            return CreatedAtAction(nameof(GetMembershipById), new { id = responseDto.Id }, responseDto);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message }); 
        }
    }

    // PUT: api/memberships/5 (Ya estaba correcto)
    [HttpPut("{id}")]
    public async Task<ActionResult<MembershipResponseDto>> UpdateMembership(int id, [FromBody] MembershipUpdateDto dto)
    {
        var membershipToUpdate = _mapper.Map<Membership>(dto);

        try
        {
            var updated = await _membershipService.UpdateMembershipAsync(id, membershipToUpdate);
        
            var responseDto = _mapper.Map<MembershipResponseDto>(updated);
        
            return Ok(responseDto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PATCH: api/memberships/5/status (No necesita DTO de entrada/salida)
    [HttpPatch("{id}/status")]
    public async Task<ActionResult> ToggleMembershipStatus(int id)
    {
        try
        {
            var newStatus = await _membershipService.ToggleMembershipStatusAsync(id);
            return Ok(new { 
                message = "Estado actualizado correctamente", 
                isActive = newStatus 
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // GET: api/memberships/expiring?days=3
    [HttpGet("expiring")]
    public async Task<ActionResult<IEnumerable<MembershipResponseDto>>> GetExpiringMemberships([FromQuery] int days = 3)
    {
        var memberships = await _membershipService.GetExpiringMembershipsAsync(days);
        // Mapear la colección a DTOs
        var responseDtos = _mapper.Map<IEnumerable<MembershipResponseDto>>(memberships);
        return Ok(responseDtos);
    }
}