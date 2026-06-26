using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Backend.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class AdjuntosController : GenericController<Adjunto>
{
    private readonly IAdjuntosUnitOfWork _adjuntosUnitOfWork;
    private readonly string _localFolderPath;

    public AdjuntosController(IGenericUnitOfWork<Adjunto> unitOfWork, IAdjuntosUnitOfWork adjuntosUnitOfWork, IConfiguration configuration) : base(unitOfWork)
    {
        _adjuntosUnitOfWork = adjuntosUnitOfWork;
        _localFolderPath = configuration["StorageSettings:LocalFolderPath"] ?? @"C:\SurveyCatFiles\Adjuntos";
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _adjuntosUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalRecords")]
    public override async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _adjuntosUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpGet("{id:int}")]
    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _adjuntosUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }

    [HttpPost("cargar")]
    public async Task<IActionResult> CargarAdjuntoConArchivoAsync([FromForm] IFormFile archivo, [FromForm] AdjuntoUploadDTO dto)
    {
        if (archivo == null || archivo.Length == 0)
        {
            return BadRequest("El archivo binario no fue recibido correctamente.");
        }

        try
        {
            var finalPath = Path.Combine(_localFolderPath, dto.CodEncuesta);

            // 1. Asegurar la creación del directorio físico
            if (!Directory.Exists(finalPath))
            {
                Directory.CreateDirectory(finalPath);
            }

            // 2. Armar la ruta física final absoluta
            var fullPath = Path.Combine(finalPath, dto.NombreArchivo);

            // 3. Escribir y guardar el archivo en el disco duro local
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            // 4. Mapear los datos del DTO a la entidad de dominio real
            var nuevoAdjunto = new Adjunto
            {
                DocumentoAnexoId = dto.DocumentoAnexoId,
                ItemPagina = dto.ItemPagina,
                NombreArchivo = dto.NombreArchivo,
                // Mantenemos la ruta web relativa para el Frontend
                Ruta = $"/localfiles/{dto.CodEncuesta}/{dto.NombreArchivo}"
            };

            // 5. Persistencia
            var actionResponse = await _adjuntosUnitOfWork.AddAsync(nuevoAdjunto);

            if (actionResponse.WasSuccess)
            {
                return Ok(actionResponse.Result);
            }

            return BadRequest(actionResponse.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno al escribir en el disco privado: {ex.Message}");
        }
    }
}