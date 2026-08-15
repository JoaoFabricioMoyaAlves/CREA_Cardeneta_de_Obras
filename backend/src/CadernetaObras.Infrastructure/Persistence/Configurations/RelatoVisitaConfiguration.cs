using CadernetaObras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadernetaObras.Infrastructure.Persistence.Configurations;

public class RelatoVisitaConfiguration : IEntityTypeConfiguration<RelatoVisita>
{
    public void Configure(EntityTypeBuilder<RelatoVisita> b)
    {
        b.ToTable("relatos_visita");
        b.HasKey(r => r.Id);
        b.Property(r => r.PosicaoObra).HasMaxLength(100).IsRequired();
        b.Property(r => r.DecisoesOrientacoes).HasColumnType("text");
        b.Property(r => r.Status).HasConversion<string>().HasMaxLength(30);

        b.HasMany(r => r.Imagens)
            .WithOne(i => i.RelatoVisita)
            .HasForeignKey(i => i.RelatoVisitaId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(r => r.Assinaturas)
            .WithOne(a => a.RelatoVisita)
            .HasForeignKey(a => a.RelatoVisitaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AssinaturaRelatoConfiguration : IEntityTypeConfiguration<AssinaturaRelato>
{
    public void Configure(EntityTypeBuilder<AssinaturaRelato> b)
    {
        b.ToTable("assinaturas_relato");
        b.HasKey(a => a.Id);
        b.Property(a => a.CodHash).HasMaxLength(255).IsRequired();
        b.Property(a => a.Ip).HasMaxLength(45).IsRequired();
        b.Property(a => a.UserAgent).HasMaxLength(500).IsRequired();
        b.Property(a => a.Papel).HasConversion<string>().HasMaxLength(20);
        b.Property(a => a.TsaToken).HasColumnType("text");
        b.Property(a => a.TsaAutoridade).HasMaxLength(255);

        b.HasIndex(a => new { a.RelatoVisitaId, a.UsuarioId }).IsUnique();
    }
}

public class ImagemConfiguration : IEntityTypeConfiguration<Imagem>
{
    public void Configure(EntityTypeBuilder<Imagem> b)
    {
        b.ToTable("imagens");
        b.HasKey(i => i.Id);
        b.Property(i => i.Name).HasMaxLength(255).IsRequired();
        b.Property(i => i.StorageKey).HasMaxLength(500).IsRequired();
    }
}
