-- Triggers de imutabilidade — defesa em profundidade no próprio Postgres.
--
-- A camada Application do backend já recusa alterar/excluir entidades
-- assinadas (RF06, RNF04), mas esses triggers garantem a mesma regra mesmo
-- que exista um bug na Application, um acesso direto ao banco, ou uma
-- migração futura que esqueça de checar o status. Nada que já tenha
-- assinatura pode ser alterado ou excluído, nem pelo Administrador.
--
-- IMPORTANTE: as tabelas e colunas foram criadas pelo EF Core Migrations.
-- Os nomes de TABELA são snake_case (configurados via ToTable(...)), mas os
-- nomes de COLUNA seguem o padrão default do provider Npgsql, que é
-- PascalCase idêntico ao nome da propriedade C# — por isso toda referência
-- de coluna aqui usa aspas duplas com a grafia exata (ex: "Status", "Id"),
-- nunca minúsculo sem aspas (o Postgres dobraria para lowercase e não
-- acharia a coluna).
--
-- Rodar DEPOIS de `dotnet ef database update` (as tabelas precisam existir).
-- Não é aplicado automaticamente pelo docker-compose de propósito.

-- =====================================================================
-- Obra: imutável a partir do status "Ativa", com uma única exceção —
-- a transição Ativa -> Finalizada feita pelo motor de assinatura do termo
-- de conclusão (que não altera nenhum outro campo da obra).
-- =====================================================================
CREATE OR REPLACE FUNCTION bloquear_alteracao_obra()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'DELETE' THEN
        IF OLD."Status" <> 'PendenteAssinatura' THEN
            RAISE EXCEPTION 'Obra % já foi assinada e não pode ser excluída.', OLD."Id";
        END IF;
        RETURN OLD;
    END IF;

    IF OLD."Status" = 'Finalizada' THEN
        RAISE EXCEPTION 'Obra % está finalizada e não pode mais ser alterada.', OLD."Id";
    END IF;

    IF OLD."Status" = 'Ativa' THEN
        IF NEW."Status" <> 'Finalizada'
           OR (to_jsonb(NEW) - 'Status') IS DISTINCT FROM (to_jsonb(OLD) - 'Status') THEN
            RAISE EXCEPTION 'Obra % já foi assinada; só a transição para Finalizada é permitida.', OLD."Id";
        END IF;
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_bloquear_alteracao_obra ON obras;
CREATE TRIGGER trg_bloquear_alteracao_obra
    BEFORE UPDATE OR DELETE ON obras
    FOR EACH ROW EXECUTE FUNCTION bloquear_alteracao_obra();

-- =====================================================================
-- Relato de Visita e Termo de Conclusão: binário — assim que "Assinado",
-- fica congelado para sempre (a API nunca reabre nenhum dos dois).
-- =====================================================================
CREATE OR REPLACE FUNCTION bloquear_alteracao_registro_assinavel()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'DELETE' THEN
        IF OLD."Status" <> 'PendenteAssinatura' THEN
            RAISE EXCEPTION '% (id %) já foi assinado e não pode ser excluído.', TG_TABLE_NAME, OLD."Id";
        END IF;
        RETURN OLD;
    END IF;

    IF OLD."Status" <> 'PendenteAssinatura' THEN
        RAISE EXCEPTION '% (id %) já foi assinado e não pode mais ser alterado.', TG_TABLE_NAME, OLD."Id";
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_bloquear_alteracao_relato_visita ON relatos_visita;
CREATE TRIGGER trg_bloquear_alteracao_relato_visita
    BEFORE UPDATE OR DELETE ON relatos_visita
    FOR EACH ROW EXECUTE FUNCTION bloquear_alteracao_registro_assinavel();

DROP TRIGGER IF EXISTS trg_bloquear_alteracao_termo_conclusao ON termos_conclusao;
CREATE TRIGGER trg_bloquear_alteracao_termo_conclusao
    BEFORE UPDATE OR DELETE ON termos_conclusao
    FOR EACH ROW EXECUTE FUNCTION bloquear_alteracao_registro_assinavel();

-- =====================================================================
-- Assinaturas propriamente ditas: sempre append-only, sem exceção — uma
-- assinatura nunca é editada nem apagada, nem antes nem depois de completar
-- a dupla assinatura.
-- =====================================================================
CREATE OR REPLACE FUNCTION bloquear_alteracao_assinatura()
RETURNS TRIGGER AS $$
BEGIN
    RAISE EXCEPTION 'Registros de assinatura em % são append-only e nunca podem ser alterados ou excluídos.', TG_TABLE_NAME;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_bloquear_alteracao_assinaturas_obra ON assinaturas_obra;
CREATE TRIGGER trg_bloquear_alteracao_assinaturas_obra
    BEFORE UPDATE OR DELETE ON assinaturas_obra
    FOR EACH ROW EXECUTE FUNCTION bloquear_alteracao_assinatura();

DROP TRIGGER IF EXISTS trg_bloquear_alteracao_assinaturas_relato ON assinaturas_relato;
CREATE TRIGGER trg_bloquear_alteracao_assinaturas_relato
    BEFORE UPDATE OR DELETE ON assinaturas_relato
    FOR EACH ROW EXECUTE FUNCTION bloquear_alteracao_assinatura();

DROP TRIGGER IF EXISTS trg_bloquear_alteracao_assinaturas_termo ON assinaturas_termo_conclusao;
CREATE TRIGGER trg_bloquear_alteracao_assinaturas_termo
    BEFORE UPDATE OR DELETE ON assinaturas_termo_conclusao
    FOR EACH ROW EXECUTE FUNCTION bloquear_alteracao_assinatura();

-- =====================================================================
-- Log de auditoria (RF09): também append-only — reaproveita a mesma
-- função das assinaturas, já que a regra é idêntica (nunca editar/apagar).
-- =====================================================================
DROP TRIGGER IF EXISTS trg_bloquear_alteracao_logs_auditoria ON logs_auditoria;
CREATE TRIGGER trg_bloquear_alteracao_logs_auditoria
    BEFORE UPDATE OR DELETE ON logs_auditoria
    FOR EACH ROW EXECUTE FUNCTION bloquear_alteracao_assinatura();
