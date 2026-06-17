using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Implementations;

public class DocumentosAnexosUnitOfWork : GenericUnitOfWork<DocumentoAnexo>, IDocumentosAnexosUnitOfWork
{
    private readonly IDocumentosAnexosRepository _documentosAnexosRepository;

    public DocumentosAnexosUnitOfWork(IGenericRepository<DocumentoAnexo> repository, IDocumentosAnexosRepository documentosAnexosRepository) : base(repository)
    {
        _documentosAnexosRepository = documentosAnexosRepository;
    }

    public override async Task<ActionResponse<IEnumerable<DocumentoAnexo>>> GetAsync(PaginationDTO pagination) => await _documentosAnexosRepository.GetAsync(pagination);

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _documentosAnexosRepository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<DocumentoAnexo>> GetAsync(long id) => await _documentosAnexosRepository.GetAsync(id);

    public async Task<ActionResponse<DocumentoAnexo>> DeleteByLongAsync(long id) => await _documentosAnexosRepository.DeleteByLongAsync(id);
}