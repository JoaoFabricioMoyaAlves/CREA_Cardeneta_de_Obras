using System.Collections.Concurrent;

namespace CadernetaObras.Application.Documentos;

public interface ILimiteTentativasAssinatura
{
    bool Bloqueado(Guid usuarioId);
    void RegistrarFalha(Guid usuarioId);
    void Limpar(Guid usuarioId);
}

// Em memória porque a API roda em uma única instância; reiniciar a API apenas
// zera a contagem. Depois de 5 erros em 15 minutos, bloqueia até a janela passar.
public class LimiteTentativasAssinatura : ILimiteTentativasAssinatura
{
    private const int MaximoFalhas = 5;
    private static readonly TimeSpan Janela = TimeSpan.FromMinutes(15);

    private readonly ConcurrentDictionary<Guid, List<DateTime>> _falhas = new();

    public bool Bloqueado(Guid usuarioId)
    {
        if (!_falhas.TryGetValue(usuarioId, out var falhas))
            return false;

        lock (falhas)
        {
            falhas.RemoveAll(f => DateTime.UtcNow - f > Janela);
            return falhas.Count >= MaximoFalhas;
        }
    }

    public void RegistrarFalha(Guid usuarioId)
    {
        var falhas = _falhas.GetOrAdd(usuarioId, _ => new List<DateTime>());
        lock (falhas)
        {
            falhas.Add(DateTime.UtcNow);
        }
    }

    public void Limpar(Guid usuarioId) => _falhas.TryRemove(usuarioId, out _);
}
