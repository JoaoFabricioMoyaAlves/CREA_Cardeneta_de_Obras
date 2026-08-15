using CadernetaObras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadernetaObras.Infrastructure.Persistence.Configurations;

public class LogAuditoriaConfiguration : IEntityTypeConfiguration<LogAuditoria>
{
    public void Configure(EntityTypeBuilder<LogAuditoria> b)
    {
        b.ToTable("logs_auditoria");
        b.HasKey(l => l.Id);
        b.Property(l => l.Acao).HasMaxLength(100).IsRequired();
        b.Property(l => l.EntidadeTipo).HasMaxLength(50);
        b.Property(l => l.EntidadeId).HasMaxLength(50);
        b.Property(l => l.Detalhes).HasColumnType("text");
        b.Property(l => l.Ip).HasMaxLength(45).IsRequired();
        b.Property(l => l.UserAgent).HasMaxLength(500).IsRequired();

        b.HasIndex(l => l.DataHoraUtc);
        b.HasIndex(l => l.UsuarioId);
    }
}
