using System.Security.Cryptography;
using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CadernetaObras.Application.Usuarios;

public class CriarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUser;
    private readonly IConfiguration _configuration;

    public CriarUsuarioUseCase(
        IUsuarioRepository usuarios,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ICurrentUserService currentUser,
        IConfiguration configuration)
    {
        _usuarios = usuarios;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _currentUser = currentUser;
        _configuration = configuration;
    }

    public async Task<UsuarioCriadoResponse> ExecutarAsync(CriarUsuarioRequest request, CancellationToken ct = default)
    {
        if (_currentUser.Perfil != PerfilUsuario.Administrador)
            throw new ForbiddenException("Somente o Administrador do CREA pode cadastrar usuários.");

        if (await _usuarios.ObterPorCpfAsync(request.Cpf, ct) is not null)
            throw new ValidationAppException("Já existe um usuário cadastrado com este CPF.");

        if (await _usuarios.ObterPorEmailAsync(request.Email, ct) is not null)
            throw new ValidationAppException("Já existe um usuário cadastrado com este e-mail.");

        if (request.Perfil == PerfilUsuario.Engenheiro &&
            (string.IsNullOrWhiteSpace(request.TituloProfissional) || string.IsNullOrWhiteSpace(request.NumeroRegistro)))
        {
            throw new ValidationAppException(
                "Para Engenheiro/Arquiteto, informe título profissional e número de registro (CREA/CAU).");
        }

        if (request.Perfil == PerfilUsuario.Administrador)
        {
            var senhaEsperada = _configuration["SegurancaAdmin:SenhaSecretaDev"];
            if (string.IsNullOrEmpty(senhaEsperada) || request.SenhaSecretaDev != senhaEsperada)
                throw new ForbiddenException("Senha secreta de desenvolvedor incorreta. Cadastro de Administrador negado.");
        }

        var senhaProvisoria = GerarSenhaProvisoria();

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome.Trim(),
            Cpf = request.Cpf.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Telefone = request.Telefone.Trim(),
            Perfil = request.Perfil,
            TituloProfissional = request.Perfil == PerfilUsuario.Engenheiro ? request.TituloProfissional : null,
            NumeroRegistro = request.Perfil == PerfilUsuario.Engenheiro ? request.NumeroRegistro : null,
            SenhaHash = _passwordHasher.Hash(senhaProvisoria),
            CriadoEm = DateTime.UtcNow,
        };

        _usuarios.Adicionar(usuario);
        await _unitOfWork.SalvarAsync(ct);

        var response = new UsuarioResponse(
            usuario.Id, usuario.Nome, usuario.Cpf, usuario.Email, usuario.Telefone,
            usuario.Perfil.ToString(), usuario.TituloProfissional, usuario.NumeroRegistro);

        return new UsuarioCriadoResponse(response, senhaProvisoria);
    }

    private static string GerarSenhaProvisoria()
    {
        // 12 caracteres alfanuméricos criptograficamente aleatórios.
        const string alfabeto = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
        var bytes = RandomNumberGenerator.GetBytes(12);
        var chars = new char[12];
        for (var i = 0; i < 12; i++)
            chars[i] = alfabeto[bytes[i] % alfabeto.Length];
        return new string(chars);
    }
}
