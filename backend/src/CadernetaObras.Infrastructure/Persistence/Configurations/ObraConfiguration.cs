using CadernetaObras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadernetaObras.Infrastructure.Persistence.Configurations;

public class ObraConfiguration : IEntityTypeConfiguration<Obra>
{
    public void Configure(EntityTypeBuilder<Obra> b)
    {
        b.ToTable("obras");
        b.HasKey(o => o.Id);
        b.Property(o => o.NumeroCaderneta).HasMaxLength(50).IsRequired();
        b.Property(o => o.LocalObra).HasMaxLength(255).IsRequired();
        b.Property(o => o.Cidade).HasMaxLength(150).IsRequired();
        b.Property(o => o.NumeroRt).HasMaxLength(50).IsRequired();
        b.Property(o => o.TipoEdificacao).HasMaxLength(100).IsRequired();
        b.Property(o => o.TipoEdificacaoOutros).HasMaxLength(255);
        b.Property(o => o.NomeEmpresa).HasMaxLength(255);
        b.Property(o => o.CnpjEmpresa).HasMaxLength(20);
        b.Property(o => o.Status).HasConversion<string>().HasMaxLength(30);

        b.Property(o => o.AreaConstruirM2).HasColumnType("decimal(10,2)");
        b.Property(o => o.AreaAmpliarM2).HasColumnType("decimal(10,2)");
        b.Property(o => o.AreaReformarM2).HasColumnType("decimal(10,2)");
        b.Property(o => o.AreaRegularizarM2).HasColumnType("decimal(10,2)");
        b.Property(o => o.AreaTotalEdificadaM2).HasColumnType("decimal(10,2)");
        b.Property(o => o.ValorObra).HasColumnType("decimal(14,2)");

        b.HasIndex(o => o.NumeroCaderneta).IsUnique();

        b.HasMany(o => o.Assinaturas)
            .WithOne(a => a.Obra)
            .HasForeignKey(a => a.ObraId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(o => o.RelatosVisita)
            .WithOne(r => r.Obra)
            .HasForeignKey(r => r.ObraId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(o => o.TermoConclusao)
            .WithOne(t => t.Obra)
            .HasForeignKey<TermoConclusao>(t => t.ObraId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AssinaturaObraConfiguration : IEntityTypeConfiguration<AssinaturaObra>
{
    public void Configure(EntityTypeBuilder<AssinaturaObra> b)
    {
        b.ToTable("assinaturas_obra");
        b.HasKey(a => a.Id);
        b.Property(a => a.CodHash).HasMaxLength(255).IsRequired();
        b.Property(a => a.Ip).HasMaxLength(45).IsRequired();
        b.Property(a => a.UserAgent).HasMaxLength(500).IsRequired();
        b.Property(a => a.Papel).HasConversion<string>().HasMaxLength(20);
        b.Property(a => a.TsaToken).HasColumnType("text");
        b.Property(a => a.TsaAutoridade).HasMaxLength(255);

        // Um usuário só pode assinar uma vez cada obra.
        b.HasIndex(a => new { a.ObraId, a.UsuarioId }).IsUnique();
    }
}
