using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Shared.DTOs;

namespace SurveyCat.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DashboardController : ControllerBase
{
    private readonly DataContext _context;

    public DashboardController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetResumenAsync()
    {
        var response = new DashboardResponseDTO();

        // 1. KPIs (Ya los tenías)
        response.TotalFichas = await _context.Fichas.CountAsync();
        response.TotalFichasControlCalidad = await _context.Fichas.CountAsync(f => f.EstadoId == 4);
        response.TotalFichasEnCorrecion = await _context.Fichas.CountAsync(f => f.EstadoId == 5);
        response.TotalFichasAprobadas = await _context.Fichas.CountAsync(f => f.EstadoId == 6);

        // 2. Gráfico: Lógica para los últimos 12 meses
        var fechaInicio = DateTime.Now.AddMonths(-5); // Hace 5 meses + el actual = 6 meses

        // Obtenemos los datos agrupados desde la base de datos
        var dataAgrupada = await _context.Fichas
        .Where(f => f.FechaEncuesta >= fechaInicio)
        .GroupBy(f => new { f.FechaEncuesta.Year, f.FechaEncuesta.Month })
        .Select(g => new
        {
            Anio = g.Key.Year,
            Mes = g.Key.Month,
            Total = g.Count()
        })
        .ToListAsync();

        // Limpiamos los meses y valores anteriores si existieran
        response.MesesGrafico = new List<string>();
        response.ValoresGrafico = new List<double>();

        // Llenamos los 6 meses cronológicamente
        for (int i = 0; i < 6; i++)
        {
            var mesActual = DateTime.Now.AddMonths(-5 + i);

            response.MesesGrafico.Add(mesActual.ToString("MMM", new System.Globalization.CultureInfo("es-ES")));

            var dato = dataAgrupada.FirstOrDefault(x => x.Anio == mesActual.Year && x.Mes == mesActual.Month);
            response.ValoresGrafico.Add(dato != null ? dato.Total : 0);
        }

        // 3. Obtener últimas 5 fichas (Tu lógica existente)
        response.FichasRecientes = await _context.Fichas
            .Include(f => f.Municipio)
            .Include(f => f.Estado)
            .OrderByDescending(f => f.FechaEncuesta)
            .Take(5)
            .Select(f => new FichaRecienteDTO
            {
                Id = f.Id,
                Codigo = f.CodEncuesta,
                Departamento = f.Municipio!.Nombre,
                FechaEncuesta = f.FechaEncuesta,
                Estado = f.Estado!.Nombre
            })
            .ToListAsync();

        return Ok(response);
    }

}