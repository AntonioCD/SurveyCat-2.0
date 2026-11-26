using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Enums;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Helpers;

public class FichasSyncHelper : IFichasSyncHelper
{
    private readonly DataContext _context;
    private readonly string _localFolderPath;

    public FichasSyncHelper(DataContext context, IConfiguration configuration)
    {
        _context = context;
        _localFolderPath = configuration["StorageSettings:LocalFolderPath"] ?? @"C:\SurveyCatFiles\Adjuntos";
    }

    public async Task<ActionResponse<List<FichaEstadoResponseDto>>> ConsultarEstadosAsync(List<string> codEncuestas)
    {
        var codigosNormalizados = codEncuestas?
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c.Trim().ToUpper())
            .Distinct()
            .ToList() ?? new List<string>();

        if (codigosNormalizados.Count == 0)
        {
            return new ActionResponse<List<FichaEstadoResponseDto>>
            {
                WasSuccess = true,
                Result = new List<FichaEstadoResponseDto>()
            };
        }

        var estados = await _context.Fichas
            .AsNoTracking()
            .Where(f => codigosNormalizados.Contains(f.CodEncuesta.ToUpper()))
            .Select(f => new FichaEstadoResponseDto
            {
                CodEncuesta = f.CodEncuesta,
                EstadoId = f.EstadoId,
                EstadoNombre = f.Estado != null ? f.Estado.Nombre : null
            })
            .ToListAsync();

        return new ActionResponse<List<FichaEstadoResponseDto>>
        {
            WasSuccess = true,
            Result = estados
        };
    }

    public async Task<ActionResponse<FichaSyncResultDTO>> SincronizarFichaCompletaAsync(FichaSyncDTO dto, string? userId)
    {
        if (string.IsNullOrWhiteSpace(dto.CodEncuesta))
        {
            return new ActionResponse<FichaSyncResultDTO>
            {
                WasSuccess = false,
                Message = "El código de encuesta es obligatorio."
            };
        }

        string codEncuestaNormalizado = dto.CodEncuesta.Trim().ToUpper();
        var archivosCreadosEnDisco = new List<string>();
        var cachePersonasPorClave = new Dictionary<string, Persona>(StringComparer.OrdinalIgnoreCase);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Resolver Persona del Informante (si viene)
            long? informanteId = null;
            if (dto.Informante != null)
            {
                var personaInformante = await ResolverOActualizarPersonaAsync(dto.Informante, userId, cachePersonasPorClave);
                informanteId = personaInformante.Id;
            }

            // 2. Buscar si la Ficha ya existe (por CodEncuesta único) para soportar reintentos/ediciones
            var ficha = await _context.Fichas
                .FirstOrDefaultAsync(f => f.CodEncuesta.ToUpper() == codEncuestaNormalizado);

            bool fueActualizada = ficha != null;

            if (ficha == null)
            {
                ficha = new Ficha
                {
                    CodEncuesta = codEncuestaNormalizado,
                    CreatedDate = DateTime.UtcNow,
                    CreatorUserId = userId
                };
                _context.Fichas.Add(ficha);
            }
            else
            {
                ficha.UpdatedDate = DateTime.UtcNow;
                ficha.UpdaterUserId = userId;
            }

            // Lista de estados que bloquean modificaciones desde el móvil
            // 814: Digitado | 815: En Control de Calidad | 817: Aprobado | 818: Migrado | Archivado
            var estadosBloqueados = new HashSet<int> { 814, 815, 817, 818 };

            if (ficha != null && estadosBloqueados.Contains(ficha.EstadoId))
            {
                var nombreEstadoActual = await _context.Diccionarios
                    .AsNoTracking()
                    .Where(d => d.Id == ficha.EstadoId)
                    .Select(d => d.Nombre)
                    .FirstOrDefaultAsync() ?? "Bloqueado";

                throw new InvalidOperationException(
                    $"La ficha '{ficha.CodEncuesta}' se encuentra en estado '{nombreEstadoActual}' en el servidor y no puede ser modificada desde la aplicación móvil."
                );
            }

            //Consultar Estado "Enviado"
            int estadoEnviadoId = await ObtenerEstadoEnviadoIdAsync(dto.EstadoId);

            // Mapear campos de la Ficha
            ficha.JornadaId = dto.JornadaId;
            ficha.TipoSector = dto.TipoSector;
            ficha.MunicipioId = dto.MunicipioId;
            ficha.SectorId = dto.SectorId;
            ficha.BarrioComarcaId = dto.BarrioComarcaId;
            ficha.CaserioId = dto.CaserioId;
            ficha.Direccion = dto.Direccion;
            ficha.Manzana = dto.Manzana?.Trim().ToUpper();
            ficha.Lote = dto.Lote?.Trim().ToUpper();

            ficha.CodParcela = dto.CodParcela?.Trim().ToUpper();
            ficha.NombreFinca = dto.NombreFinca?.Trim().ToUpper();

            ficha.EncuestadorId = dto.EncuestadorId;
            ficha.CoordinadorId = dto.CoordinadorId;
            ficha.TecnicoCatastralId = dto.TecnicoCatastralId;

            ficha.TipoEncuesta = dto.TipoEncuesta;
            ficha.CodSiscat = dto.CodSiscat?.Trim().ToUpper();
            ficha.TipoUso = dto.TipoUso;
            ficha.DescripcionTipoUso = dto.DescripcionTipoUso?.Trim();
            ficha.AreaEstimada = dto.AreaEstimada;
            ficha.UnidadMedidaId = dto.UnidadMedidaId;
            ficha.OrigenTierraId = dto.OrigenTierraId;

            ficha.Servidumbre = dto.Servidumbre;
            ficha.ServidumbreAguaId = dto.Servidumbre ? dto.ServidumbreAguaId : null;
            ficha.ServidumbrePaseId = dto.Servidumbre ? dto.ServidumbrePaseId : null;
            ficha.ServidumbreOtraId = dto.Servidumbre ? dto.ServidumbreOtraId : null;

            ficha.PresentaConflicto = dto.PresentaConflicto || dto.Conflictos.Any();
            ficha.FechaEncuesta = dto.FechaEncuesta;
            ficha.VerificadoCoordinador = dto.VerificadoCoordinador;
            ficha.EstadoId = estadoEnviadoId;
            ficha.Observacion = dto.Observacion?.Trim();

            ficha.InformanteId = informanteId;
            ficha.RelacionInformanteParcelaId = dto.RelacionInformanteParcelaId;
            ficha.RelacionInformantePropietarioId = dto.RelacionInformantePropietarioId;

            // Guardar Ficha para obtener Ficha.Id
            await _context.SaveChangesAsync();

            // 3. Si es actualización/reenvío, limpiar registros hijos previos respetando DeleteBehavior.Restrict
            if (fueActualizada)
            {
                await LimpiarHijosDeFichaAsync(ficha.Id);
            }

            // 4. Guardar Propietarios (validando unicidad FichaId + PersonaId)
            var personasPropietariosUsadas = new HashSet<long>();
            int totalPropietarios = 0;

            foreach (var propDto in dto.Propietarios)
            {
                var persona = await ResolverOActualizarPersonaAsync(propDto.Persona, userId, cachePersonasPorClave);

                if (!personasPropietariosUsadas.Add(persona.Id))
                {
                    throw new InvalidOperationException(
                        $"La persona '{persona.NombreCompleto}' está duplicada en la lista de propietarios.");
                }

                var propietario = new Propietario
                {
                    FichaId = ficha.Id,
                    PersonaId = persona.Id,
                    Perfil = propDto.Perfil,
                    EspecificarPerfil = propDto.EspecificarPerfil?.Trim(),
                    Hijos = propDto.Hijos,
                    Hijas = propDto.Hijas,
                    TipoDerecho = propDto.TipoDerecho,
                    PresentaDocumento = propDto.PresentaDocumento,
                    DocumentoId = propDto.PresentaDocumento ? propDto.DocumentoId : null,
                    AutorDocumento = propDto.PresentaDocumento ? propDto.AutorDocumento?.Trim() : null,
                    FechaDocumento = propDto.PresentaDocumento ? propDto.FechaDocumento : null,
                    AreaTitulada = propDto.PresentaDocumento ? propDto.AreaTitulada : null,
                    UnidadMedidaId = propDto.PresentaDocumento ? propDto.UnidadMedidaId : null,
                    FechaAdquisicion = propDto.PresentaDocumento ? propDto.FechaAdquisicion : null,
                    FechaRegistro = propDto.PresentaDocumento ? propDto.FechaRegistro : null,
                    Finca = propDto.PresentaDocumento ? propDto.Finca?.Trim().ToUpper() : null,
                    Tomo = propDto.PresentaDocumento ? propDto.Tomo?.Trim().ToUpper() : null,
                    Folio = propDto.PresentaDocumento ? propDto.Folio?.Trim().ToUpper() : null,
                    Asiento = propDto.PresentaDocumento ? propDto.Asiento?.Trim().ToUpper() : null
                };

                _context.Propietarios.Add(propietario);
                totalPropietarios++;
            }

            // 5. Guardar Ocupantes (validando unicidad FichaId + PersonaId y asignando Item consecutivo)
            var personasOcupantesUsadas = new HashSet<long>();
            int itemOcupante = 0;

            foreach (var ocupDto in dto.Ocupantes)
            {
                var persona = await ResolverOActualizarPersonaAsync(ocupDto.Persona, userId, cachePersonasPorClave);

                if (!personasOcupantesUsadas.Add(persona.Id))
                {
                    throw new InvalidOperationException(
                        $"La persona '{persona.NombreCompleto}' está duplicada en la lista de ocupantes.");
                }

                itemOcupante++;
                var ocupante = new Ocupante
                {
                    FichaId = ficha.Id,
                    Item = itemOcupante,
                    PersonaId = persona.Id,
                    TipoOcupanteId = ocupDto.TipoOcupanteId,
                    ParentescoId = ocupDto.ParentescoId
                };

                _context.Ocupantes.Add(ocupante);
            }

            // 6. Guardar Conflictos
            int totalConflictos = 0;
            foreach (var confDto in dto.Conflictos)
            {
                var conflicto = new Conflicto
                {
                    FichaId = ficha.Id,
                    TipoConflictoId = confDto.TipoConflictoId,
                    ViaGestionId = confDto.ViaGestionId,
                    ConEstado = confDto.ConEstado,
                    Descripcion = confDto.Descripcion?.Trim()
                };

                _context.Conflictos.Add(confDto.Descripcion != null ? conflicto : conflicto);
                totalConflictos++;
            }

            // 7. Guardar Documentos Anexos y sus Adjuntos (BD + Disco)
            int totalDocumentosAnexos = 0;
            int totalAdjuntos = 0;

            foreach (var docDto in dto.DocumentosAnexos)
            {
                var documentoAnexo = new DocumentoAnexo
                {
                    FichaId = ficha.Id,
                    DocumentoId = docDto.DocumentoId,
                    Descripcion = docDto.Descripcion?.Trim(),
                    NumeroPaginas = docDto.NumeroPaginas > 0
                        ? docDto.NumeroPaginas
                        : Math.Max(1, docDto.Adjuntos.Count),
                    Codigo = docDto.Codigo?.Trim().ToUpper()
                };

                _context.DocumentosAnexos.Add(documentoAnexo);
                await _context.SaveChangesAsync(); // Obtener DocumentoAnexo.Id
                totalDocumentosAnexos++;

                int paginaConsecutiva = 0;
                foreach (var adjDto in docDto.Adjuntos.OrderBy(a => a.ItemPagina))
                {
                    paginaConsecutiva++;
                    int itemPagina = adjDto.ItemPagina > 0 ? adjDto.ItemPagina : paginaConsecutiva;
                    string nombreArchivoSeguro = Path.GetFileName(adjDto.NombreArchivo.Trim());

                    if (nombreArchivoSeguro.Length > 50)
                    {
                        string ext = Path.GetExtension(nombreArchivoSeguro);
                        nombreArchivoSeguro = nombreArchivoSeguro[..(50 - ext.Length)] + ext;
                    }

                    // Escribir archivo físico si viene en Base64
                    if (!string.IsNullOrWhiteSpace(adjDto.ContenidoBase64))
                    {
                        var carpetaEncuesta = Path.Combine(_localFolderPath, codEncuestaNormalizado);
                        if (!Directory.Exists(carpetaEncuesta))
                        {
                            Directory.CreateDirectory(carpetaEncuesta);
                        }

                        var rutaFisicaCompleta = Path.Combine(carpetaEncuesta, nombreArchivoSeguro);
                        byte[] bytesArchivo = Convert.FromBase64String(adjDto.ContenidoBase64);
                        await File.WriteAllBytesAsync(rutaFisicaCompleta, bytesArchivo);
                        archivosCreadosEnDisco.Add(rutaFisicaCompleta);
                    }

                    var adjunto = new Adjunto
                    {
                        DocumentoAnexoId = documentoAnexo.Id,
                        ItemPagina = itemPagina,
                        NombreArchivo = nombreArchivoSeguro,
                        Ruta = $"/localfiles/{codEncuestaNormalizado}/{nombreArchivoSeguro}"
                    };

                    _context.Adjuntos.Add(adjunto);
                    totalAdjuntos++;
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ActionResponse<FichaSyncResultDTO>
            {
                WasSuccess = true,
                Result = new FichaSyncResultDTO
                {
                    FichaId = ficha.Id,
                    CodEncuesta = ficha.CodEncuesta,
                    FueActualizada = fueActualizada,
                    TotalPropietarios = totalPropietarios,
                    TotalOcupantes = itemOcupante,
                    TotalConflictos = totalConflictos,
                    TotalDocumentosAnexos = totalDocumentosAnexos,
                    TotalAdjuntos = totalAdjuntos
                }
            };
        }
        catch (DbUpdateException dbEx)
        {
            await transaction.RollbackAsync();
            LimpiarArchivosHuerfanos(archivosCreadosEnDisco);

            var detalle = dbEx.InnerException?.Message ?? dbEx.Message;
            return new ActionResponse<FichaSyncResultDTO>
            {
                WasSuccess = false,
                Message = $"Error de integridad en base de datos al sincronizar la ficha: {detalle}"
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            LimpiarArchivosHuerfanos(archivosCreadosEnDisco);

            return new ActionResponse<FichaSyncResultDTO>
            {
                WasSuccess = false,
                Message = $"Error al sincronizar la ficha completa: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Busca una Persona existente por Identificación normalizada (evitando duplicados por índice único)
    /// o la crea/actualiza en SQL Server y retorna la entidad con su Id real.
    /// </summary>
    private async Task<Persona> ResolverOActualizarPersonaAsync(
        PersonaSyncDTO pDto,
        string? userId,
        Dictionary<string, Persona> cacheLocal)
    {
        string? cedulaLimpia = string.IsNullOrWhiteSpace(pDto.Identificacion)
            ? null
            : pDto.Identificacion.Replace("-", "").Replace(" ", "").Trim().ToUpper();

        string claveCache = cedulaLimpia != null
            ? $"CED:{cedulaLimpia}"
            : (pDto.Id > 0 ? $"ID:{pDto.Id}" : $"TMP:{Guid.NewGuid()}");

        if (cacheLocal.TryGetValue(claveCache, out var personaEnCache))
        {
            return personaEnCache;
        }

        Persona? personaBd = null;

        if (cedulaLimpia != null)
        {
            personaBd = await _context.Personas.FirstOrDefaultAsync(p =>
                p.Identificacion != null &&
                p.Identificacion.Replace("-", "").Replace(" ", "").Trim().ToUpper() == cedulaLimpia);
        }

        if (personaBd == null && pDto.Id > 0)
        {
            personaBd = await _context.Personas.FirstOrDefaultAsync(p => p.Id == pDto.Id);
        }

        bool esNatural = pDto.TipoPersona == TipoPersona.Natural;

        if (personaBd == null)
        {
            personaBd = new Persona
            {
                TipoPersona = pDto.TipoPersona,
                TipoIdentificacionId = pDto.TipoIdentificacionId,
                Identificacion = cedulaLimpia,
                PrimerNombre = esNatural ? pDto.PrimerNombre?.Trim().ToUpper() : null,
                SegundoNombre = esNatural ? pDto.SegundoNombre?.Trim().ToUpper() : null,
                PrimerApellido = esNatural ? pDto.PrimerApellido?.Trim().ToUpper() : null,
                SegundoApellido = esNatural ? pDto.SegundoApellido?.Trim().ToUpper() : null,
                NombreCompleto = pDto.NombreCompleto.Trim().ToUpper(),
                Genero = esNatural ? pDto.Genero : null,
                Edad = esNatural ? pDto.Edad : null,
                EstadoCivilId = esNatural ? pDto.EstadoCivilId : null,
                ProfesionId = esNatural ? pDto.ProfesionId : null,
                MunicipioId = pDto.MunicipioId,
                BarrioComarcaId = pDto.BarrioComarcaId,
                CaserioId = pDto.CaserioId,
                Direccion = pDto.Direccion,
                TipoPersonaJuridicaId = !esNatural ? pDto.TipoPersonaJuridicaId : null,
                RegistradaEn = !esNatural ? pDto.RegistradaEn?.Trim().ToUpper() : null,
                FechaRegistro = !esNatural ? pDto.FechaRegistro : null,
                CreatedDate = DateTime.UtcNow,
                CreatorUserId = userId
            };

            _context.Personas.Add(personaBd);
            await _context.SaveChangesAsync(); // Necesario para obtener Persona.Id inmediatamente
        }
        else
        {
            // Actualizar datos complementarios si fueron editados en campo
            personaBd.TipoPersona = pDto.TipoPersona;
            personaBd.TipoIdentificacionId = pDto.TipoIdentificacionId ?? personaBd.TipoIdentificacionId;
            if (cedulaLimpia != null) personaBd.Identificacion = cedulaLimpia;

            if (esNatural)
            {
                if (!string.IsNullOrWhiteSpace(pDto.PrimerNombre)) personaBd.PrimerNombre = pDto.PrimerNombre.Trim().ToUpper();
                personaBd.SegundoNombre = pDto.SegundoNombre?.Trim().ToUpper();
                if (!string.IsNullOrWhiteSpace(pDto.PrimerApellido)) personaBd.PrimerApellido = pDto.PrimerApellido.Trim().ToUpper();
                personaBd.SegundoApellido = pDto.SegundoApellido?.Trim().ToUpper();
                if (!string.IsNullOrWhiteSpace(pDto.NombreCompleto)) personaBd.NombreCompleto = pDto.NombreCompleto.Trim().ToUpper();
                personaBd.Genero = pDto.Genero ?? personaBd.Genero;
                personaBd.Edad = pDto.Edad ?? personaBd.Edad;
                personaBd.EstadoCivilId = pDto.EstadoCivilId ?? personaBd.EstadoCivilId;
                personaBd.ProfesionId = pDto.ProfesionId ?? personaBd.ProfesionId;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(pDto.NombreCompleto)) personaBd.NombreCompleto = pDto.NombreCompleto.Trim().ToUpper();
                personaBd.TipoPersonaJuridicaId = pDto.TipoPersonaJuridicaId ?? personaBd.TipoPersonaJuridicaId;
                personaBd.RegistradaEn = pDto.RegistradaEn?.Trim().ToUpper() ?? personaBd.RegistradaEn;
                personaBd.FechaRegistro = pDto.FechaRegistro ?? personaBd.FechaRegistro;
            }

            personaBd.MunicipioId = pDto.MunicipioId ?? personaBd.MunicipioId;
            personaBd.BarrioComarcaId = pDto.BarrioComarcaId ?? personaBd.BarrioComarcaId;
            personaBd.CaserioId = pDto.CaserioId ?? personaBd.CaserioId;
            if (!string.IsNullOrWhiteSpace(pDto.Direccion)) personaBd.Direccion = pDto.Direccion;

            personaBd.UpdatedDate = DateTime.UtcNow;
            personaBd.UpdaterUserId = userId;
        }

        cacheLocal[claveCache] = personaBd;
        return personaBd;
    }

    private async Task LimpiarHijosDeFichaAsync(long fichaId)
    {
        var docsIds = await _context.DocumentosAnexos
            .Where(d => d.FichaId == fichaId)
            .Select(d => d.Id)
            .ToListAsync();

        if (docsIds.Any())
        {
            var adjuntosPrevios = await _context.Adjuntos
                .Where(a => docsIds.Contains(a.DocumentoAnexoId))
                .ToListAsync();
            _context.Adjuntos.RemoveRange(adjuntosPrevios);

            var docsPrevios = await _context.DocumentosAnexos
                .Where(d => d.FichaId == fichaId)
                .ToListAsync();
            _context.DocumentosAnexos.RemoveRange(docsPrevios);
        }

        var conflictosPrevios = await _context.Conflictos
            .Where(c => c.FichaId == fichaId)
            .ToListAsync();
        _context.Conflictos.RemoveRange(conflictosPrevios);

        var ocupantesPrevios = await _context.Ocupantes
            .Where(o => o.FichaId == fichaId)
            .ToListAsync();
        _context.Ocupantes.RemoveRange(ocupantesPrevios);

        var propietariosPrevios = await _context.Propietarios
            .Where(p => p.FichaId == fichaId)
            .ToListAsync();
        _context.Propietarios.RemoveRange(propietariosPrevios);

        await _context.SaveChangesAsync();
    }

    private static void LimpiarArchivosHuerfanos(IEnumerable<string> rutas)
    {
        foreach (var ruta in rutas)
        {
            try
            {
                if (File.Exists(ruta))
                {
                    File.Delete(ruta);
                }
            }
            catch
            {
                // Ignorar errores secundarios de limpieza en rollback
            }
        }
    }

    private async Task<int> ObtenerEstadoEnviadoIdAsync(int estadoFallbackId)
    {
        var estadoEnviadoId = await _context.Diccionarios
            .AsNoTracking()
            .Where(d => d.Catalogo == "EstadoEncuesta" && d.Nombre == "Enviado")
            .OrderByDescending(d => d.Activo)
            .Select(d => (int?)d.Id)
            .FirstOrDefaultAsync();

        if (estadoEnviadoId.HasValue && estadoEnviadoId.Value > 0)
        {
            return estadoEnviadoId.Value;
        }

        // Respaldo si el catálogo no tiene "Enviado" en el entorno actual
        return estadoFallbackId > 0 ? estadoFallbackId : 812;
    }
}