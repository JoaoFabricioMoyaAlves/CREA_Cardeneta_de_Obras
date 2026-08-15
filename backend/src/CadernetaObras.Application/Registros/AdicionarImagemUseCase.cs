using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Registros;

public record AdicionarImagemRequest(int RegistroId, string NomeArquivo, string ContentType, Stream Conteudo);

public class AdicionarImagemUseCase
{
    private readonly IRelatoVisitaRepository _registros;
    private readonly IObraRepository _obras;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storage;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogger _auditLogger;

    public AdicionarImagemUseCase(
        IRelatoVisitaRepository registros, IObraRepository obras, IUnitOfWork unitOfWork,
        IStorageService storage, ICurrentUserService currentUser, IAuditLogger auditLogger)
    {
        _registros = registros;
        _obras = obras;
        _unitOfWork = unitOfWork;
        _storage = storage;
        _currentUser = currentUser;
        _auditLogger = auditLogger;
    }

    public async Task<ImagemResponse> ExecutarAsync(AdicionarImagemRequest request, CancellationToken ct = default)
    {
        if (_currentUser.Perfil != PerfilUsuario.Engenheiro)
            throw new ForbiddenException("Somente o Engenheiro/Arquiteto pode anexar fotos ao registro.");

        var registro = await _registros.ObterPorIdAsync(request.RegistroId, ct)
            ?? throw new NotFoundException("Registro de visita não encontrado.");

        if (registro.Status != StatusRegistro.PendenteAssinatura)
            throw new Domain.Exceptions.EntidadeImutavelException("Registro de visita");

        var obra = await _obras.ObterPorIdAsync(registro.ObraId, ct)
            ?? throw new NotFoundException("Caderneta não encontrada.");
        if (obra.IdProfissional != _currentUser.UsuarioId)
            throw new ForbiddenException("Você não é o responsável técnico desta obra.");

        var storageKey = $"registros/{registro.ObraId}/{registro.Id}/{Guid.NewGuid()}-{request.NomeArquivo}";
        await _storage.SalvarArquivoAsync(storageKey, request.Conteudo, request.ContentType, ct);

        var imagem = new Imagem
        {
            RelatoVisitaId = registro.Id,
            Data = DateTime.UtcNow,
            Name = request.NomeArquivo,
            StorageKey = storageKey,
        };

        registro.Imagens.Add(imagem);
        await _unitOfWork.SalvarAsync(ct); // gera imagem.Id

        _auditLogger.Registrar("ImagemAdicionada", _currentUser.UsuarioId, "RelatoVisita", registro.Id.ToString(), imagem.Name);
        await _unitOfWork.SalvarAsync(ct);

        return new ImagemResponse(imagem.Id, imagem.Name, imagem.StorageKey, imagem.Data);
    }
}
