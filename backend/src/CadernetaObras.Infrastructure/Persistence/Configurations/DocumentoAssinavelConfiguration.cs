using CadernetaObras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadernetaObras.Infrastructure.Persistence.Configurations;

public class DocumentoAssinavelConfiguration : IEntityTypeConfiguration<DocumentoAssinavel>
{
    public void Configure(EntityTypeBuilder<DocumentoAssinavel> b)
    {
        b.ToTable("documentos_assinaveis");
        b.HasKey(d => d.Id);
        b.Property(d => d.Tipo).HasConversion<string>().HasMaxLength(30);
        b.Property(d => d.DadosJson).HasColumnType("text").IsRequired();
        b.Property(d => d.HashDocumento).HasMaxLength(64).IsRequired();

        // Cada documento é congelado uma única vez.
        b.HasIndex(d => new { d.Tipo, d.ReferenciaId }).IsUnique();
    }
}
