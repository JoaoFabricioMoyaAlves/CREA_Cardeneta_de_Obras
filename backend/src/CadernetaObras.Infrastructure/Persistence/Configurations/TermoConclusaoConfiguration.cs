using CadernetaObras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadernetaObras.Infrastructure.Persistence.Configurations;

public class TermoConclusaoConfiguration : IEntityTypeConfiguration<TermoConclusao>
{
    public void Configure(EntityTypeBuilder<TermoConclusao> b)
    {
        b.ToTable("termos_conclusao");
        b.HasKey(t => t.Id);
        b.Property(t => t.Declaracao).HasColumnType("text").IsRequired();
        b.Property(t => t.Status).HasConversion<string>().HasMaxLength(30);

        b.HasIndex(t => t.ObraId).IsUnique();

        b.HasMany(t => t.Assinaturas)
            .WithOne(a => a.TermoConclusao)
            .HasForeignKey(a => a.TermoConclusaoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AssinaturaTermoConclusaoConfiguration : IEntityTypeConfiguration<AssinaturaTermoConclusao>
{
    public void Configure(EntityTypeBuilder<AssinaturaTermoConclusao> b)
    {
        b.ToTable("assinaturas_termo_conclusao");
        b.HasKey(a => a.Id);
        b.Property(a => a.CodHash).HasMaxLength(255).IsRequired();
        b.Property(a => a.Ip).HasMaxLength(45).IsRequired();
        b.Property(a => a.UserAgent).HasMaxLength(500).IsRequired();
        b.Property(a => a.Papel).HasConversion<string>().HasMaxLength(20);
        b.Property(a => a.TsaToken).HasColumnType("text");
        b.Property(a => a.TsaAutoridade).HasMaxLength(255);

        b.HasIndex(a => new { a.TermoConclusaoId, a.UsuarioId }).IsUnique();
    }
}
