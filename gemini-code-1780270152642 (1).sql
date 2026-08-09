-- 1. Criação do Banco de Dados
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'gestao_obras')
BEGIN
    CREATE DATABASE [gestao_obras];
END;
GO

USE gestao_obras;
GO

-- 2. Criação das Tabelas Independentes (Sem Chaves Estrangeiras)

IF OBJECT_ID(N'dbo.Proprietario', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Proprietario (
        id_proprietario INT IDENTITY(1,1) PRIMARY KEY,
        nome_proprietario VARCHAR(255) NOT NULL,
        cpf VARCHAR(14) NOT NULL UNIQUE,
        Telefone VARCHAR(20) NOT NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.Profissional', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Profissional (
        id_profissional INT IDENTITY(1,1) PRIMARY KEY,
        nome_profissional VARCHAR(255) NOT NULL,
        titulo_profissional VARCHAR(100),
        numero_registro VARCHAR(50) NOT NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios (
        id_user INT IDENTITY(1,1) PRIMARY KEY,
        nome_usuario VARCHAR(255) NOT NULL,
        cpf VARCHAR(14) NOT NULL UNIQUE,
        telefone VARCHAR(20) NOT NULL,
        senha VARCHAR(255) NOT NULL,
        Cargo VARCHAR(100) NOT NULL
    );
END;
GO

-- 3. Criação da Tabela Central (Obra)

IF OBJECT_ID(N'dbo.Obra', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Obra (
        numero_caderneta INT PRIMARY KEY,
        id_profissional INT NOT NULL,
        id_proprietario INT NOT NULL,
        local_obra VARCHAR(255) NOT NULL,
        numero_rt VARCHAR(50) NOT NULL,
        area_construir_m2 DECIMAL(10,2),
        area_ampliar_m2 DECIMAL(10,2),
        area_reformar_m2 DECIMAL(10,2),
        area_regularizar_m2 DECIMAL(10,2),
        area_total_edificada_m2 DECIMAL(10,2) NOT NULL,
        tipo_edificacao VARCHAR(100) NOT NULL,
        tipo_edificacao_outros VARCHAR(255),
        ativ_tecnica_direcao BIT NOT NULL DEFAULT 0,
        ativ_tecnica_execucao BIT NOT NULL DEFAULT 0,
        ativ_tecnica_fiscalizacao BIT NOT NULL DEFAULT 0,
        ativ_tecnica_projeto BIT NOT NULL DEFAULT 0,
        data_recibo_abertura DATE NOT NULL,
        assinado BIT NOT NULL DEFAULT 0,
        nome_empresa VARCHAR(255),
        CNPJ_Empresa VARCHAR(20),
        FOREIGN KEY (id_profissional) REFERENCES dbo.Profissional(id_profissional),
        FOREIGN KEY (id_proprietario) REFERENCES dbo.Proprietario(id_proprietario)
    );
END;
GO

-- 4. Criação das Tabelas Dependentes de Obra

IF OBJECT_ID(N'dbo.Termo_Conclusao', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Termo_Conclusao (
        id_termo INT IDENTITY(1,1) PRIMARY KEY,
        numero_caderneta INT NOT NULL UNIQUE,
        data_conclusao DATE NOT NULL,
        Declaracao VARCHAR(255) NOT NULL,
        assinado BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (numero_caderneta) REFERENCES dbo.Obra(numero_caderneta)
    );
END;
GO

IF OBJECT_ID(N'dbo.Assinatura_obra', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Assinatura_obra (
        id INT IDENTITY(1,1) PRIMARY KEY,
        data DATE NOT NULL,
        cod_hash VARCHAR(255) NOT NULL,
        ip VARCHAR(45) NOT NULL,
        numero_caderneta INT NOT NULL,
        FOREIGN KEY (numero_caderneta) REFERENCES dbo.Obra(numero_caderneta)
    );
END;
GO

IF OBJECT_ID(N'dbo.Relato_Visita', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Relato_Visita (
        id_relato INT IDENTITY(1,1) PRIMARY KEY,
        numero_caderneta INT NOT NULL,
        data_visita DATE NOT NULL,
        posicao_obra VARCHAR(100) NOT NULL,
        decisoes_orientacoes TEXT,
        fase_servicos_preliminares BIT NOT NULL DEFAULT 0,
        fase_fundacao BIT NOT NULL DEFAULT 0,
        fase_alvenarias BIT NOT NULL DEFAULT 0,
        fase_superestrutura BIT NOT NULL DEFAULT 0,
        fase_cobertura BIT NOT NULL DEFAULT 0,
        fase_esquadrias_inst BIT NOT NULL DEFAULT 0,
        fase_revestimento BIT NOT NULL DEFAULT 0,
        fase_pintura BIT NOT NULL DEFAULT 0,
        fase_servicos_comp BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (numero_caderneta) REFERENCES dbo.Obra(numero_caderneta)
    );
END;
GO

-- 5. Criação das Tabelas Dependentes (Assinaturas e Imagens)

IF OBJECT_ID(N'dbo.Assinatura_termo_conclusao', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Assinatura_termo_conclusao (
        id INT IDENTITY(1,1) PRIMARY KEY,
        data DATE NOT NULL,
        cod_hash VARCHAR(255) NOT NULL,
        ip VARCHAR(45) NOT NULL,
        id_termo INT NOT NULL,
        FOREIGN KEY (id_termo) REFERENCES dbo.Termo_Conclusao(id_termo)
    );
END;
GO

IF OBJECT_ID(N'dbo.Imagem', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Imagem (
        id INT IDENTITY(1,1) PRIMARY KEY,
        data DATE NOT NULL,
        name VARCHAR(255) NOT NULL,
        id_relato INT NOT NULL,
        FOREIGN KEY (id_relato) REFERENCES dbo.Relato_Visita(id_relato)
    );
END;
GO

IF OBJECT_ID(N'dbo.Assinatura', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Assinatura (
        id INT IDENTITY(1,1) PRIMARY KEY,
        data DATE NOT NULL,
        cod_hash VARCHAR(255) NOT NULL,
        ip VARCHAR(45) NOT NULL,
        id_relato INT NOT NULL,
        FOREIGN KEY (id_relato) REFERENCES dbo.Relato_Visita(id_relato)
    );
END;
GO