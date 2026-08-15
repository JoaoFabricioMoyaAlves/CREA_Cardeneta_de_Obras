using CadernetaObras.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CadernetaObras.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Obra> Obras => Set<Obra>();
    public DbSet<AssinaturaObra> AssinaturasObra => Set<AssinaturaObra>();
    public DbSet<RelatoVisita> RelatosVisita => Set<RelatoVisita>();
    public DbSet<AssinaturaRelato> AssinaturasRelato => Set<AssinaturaRelato>();
    public DbSet<Imagem> Imagens => Set<Imagem>();
    public DbSet<TermoConclusao> TermosConclusao => Set<TermoConclusao>();
    public DbSet<AssinaturaTermoConclusao> AssinaturasTermoConclusao => Set<AssinaturaTermoConclusao>();
    public DbSet<LogAuditoria> LogsAuditoria => Set<LogAuditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
