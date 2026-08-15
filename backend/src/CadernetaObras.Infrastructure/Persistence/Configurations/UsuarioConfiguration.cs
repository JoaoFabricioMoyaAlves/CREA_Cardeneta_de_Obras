using CadernetaObras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadernetaObras.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> b)
    {
        b.ToTable("usuarios");
        b.HasKey(u => u.Id);
        b.Property(u => u.Nome).HasMaxLength(255).IsRequired();
        b.Property(u => u.Cpf).HasMaxLength(14).IsRequired();
        b.Property(u => u.Email).HasMaxLength(255).IsRequired();
        b.Property(u => u.Telefone).HasMaxLength(20).IsRequired();
        b.Property(u => u.SenhaHash).HasMaxLength(255).IsRequired();
        b.Property(u => u.TituloProfissional).HasMaxLength(100);
        b.Property(u => u.NumeroRegistro).HasMaxLength(50);
        b.Property(u => u.Perfil).HasConversion<string>().HasMaxLength(20);

        b.HasIndex(u => u.Cpf).IsUnique();
        b.HasIndex(u => u.Email).IsUnique();
    }
}
