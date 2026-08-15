using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Obras;

public class CriarObraUseCase
{
    private readonly IObraRepository _obras;
    private readonly IUsuarioRepository _usuarios;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CriarObraUseCase(
        IObraRepository obras, IUsuarioRepository usuarios, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _obras = obras;
        _usuarios = usuarios;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ObraResponse> ExecutarAsync(CriarObraRequest request, CancellationToken ct = default)
    {
        if (_currentUser.Perfil != PerfilUsuario.Administrador)
            throw new ForbiddenException("Somente o Administrador do CREA pode criar uma nova obra.");

        var profissional = await _usuarios.ObterPorIdAsync(request.ProfissionalId, ct)
            ?? throw new ValidationAppException("Engenheiro/Arquiteto responsável não encontrado.");
        if (profissional.Perfil != PerfilUsuario.Engenheiro)
            throw new ValidationAppException("O responsável técnico informado não é um Engenheiro/Arquiteto.");

        var proprietario = await _usuarios.ObterPorIdAsync(request.ProprietarioId, ct)
            ?? throw new ValidationAppException("Proprietário não encontrado.");
        if (proprietario.Perfil != PerfilUsuario.Proprietario)
            throw new ValidationAppException("O usuário informado como proprietário não tem esse perfil.");

        var areaTotal = request.AreaConstruirM2 + request.AreaAmpliarM2 + request.AreaReformarM2 + request.AreaRegularizarM2;

        var obra = new Obra
        {
            IdAdministrador = _currentUser.UsuarioId,
            IdProfissional = profissional.Id,
            IdProprietario = proprietario.Id,
            LocalObra = request.LocalObra.Trim(),
            Cidade = request.Cidade.Trim(),
            NumeroRt = request.NumeroRt.Trim(),
            AreaConstruirM2 = request.AreaConstruirM2,
            AreaAmpliarM2 = request.AreaAmpliarM2,
            AreaReformarM2 = request.AreaReformarM2,
            AreaRegularizarM2 = request.AreaRegularizarM2,
            AreaTotalEdificadaM2 = areaTotal,
            TipoEdificacao = request.TipoEdificacao,
            TipoEdificacaoOutros = request.TipoEdificacaoOutros,
            AtivTecnicaDirecao = request.AtivTecnicaDirecao,
            AtivTecnicaExecucao = request.AtivTecnicaExecucao,
            AtivTecnicaFiscalizacao = request.AtivTecnicaFiscalizacao,
            AtivTecnicaProjeto = request.AtivTecnicaProjeto,
            ValorObra = request.ValorObra,
            DataReciboAbertura = request.DataReciboAbertura,
            NomeEmpresa = request.NomeEmpresa,
            CnpjEmpresa = request.CnpjEmpresa,
            Status = StatusObra.PendenteAssinatura,
            NumeroCaderneta = "PENDENTE", // substituído abaixo, após obter o Id gerado
        };

        _obras.Adicionar(obra);
        await _unitOfWork.SalvarAsync(ct); // gera obra.Id

        obra.NumeroCaderneta = $"CAD-{request.DataReciboAbertura.Year}-{obra.Id:D4}";
        await _unitOfWork.SalvarAsync(ct);

        return ObraMapper.ToResponse(obra, profissional, proprietario);
    }
}
