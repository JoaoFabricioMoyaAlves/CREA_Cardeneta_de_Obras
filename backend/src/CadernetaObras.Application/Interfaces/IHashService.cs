namespace CadernetaObras.Application.Interfaces;

// Gera o hash SHA-256 do conteúdo exato de um registro no momento da
// assinatura — é o núcleo da "assinatura eletrônica avançada" (hash +
// identidade do signatário + IP + user-agent + timestamp do servidor).
public interface IHashService
{
    string GerarHashSha256(string conteudoCanonico);
}
