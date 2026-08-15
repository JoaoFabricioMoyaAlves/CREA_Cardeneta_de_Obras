using CadernetaObras.Domain.Entities;

namespace CadernetaObras.Application.Interfaces;

public interface IPdfService
{
    byte[] GerarPdfRegistro(RelatoVisita registro, Obra obra);
    byte[] GerarPdfCaderneta(Obra obra, List<RelatoVisita> registros, TermoConclusao? termo);
}
