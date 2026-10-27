using CadernetaObras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadernetaObras.Infrastructure.Persistence.Configurations;

public class TokenSenhaConfiguration : IEntityTypeConfiguration<TokenSenha>
{
    public void Configure(EntityTypeBuilder<TokenSenha> b)
    {
        b.ToTable("tokens_senha");
        b.HasKey(t => t.Id);
        b.Property(t => t.TokenHash).HasMaxLength(64).IsRequired();
        b.Property(t => t.Finalidade).HasConversion<string>().HasMaxLength(30);

        b.HasOne<Usuario>().WithMany().HasForeignKey(t => t.UsuarioId).OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(t => t.TokenHash).IsUnique();
        b.HasIndex(t => t.UsuarioId);
    }
}
