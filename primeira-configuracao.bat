@echo off
setlocal enabledelayedexpansion
cd /d "%~dp0"

echo ===================================================
echo  Caderneta de Obras Digital - Configuracao inicial
echo  (rodar isso so uma vez, na primeira vez neste PC)
echo ===================================================
echo.

echo [1/7] Verificando pre-requisitos...
where dotnet >nul 2>&1
if errorlevel 1 (
    echo.
    echo ERRO: .NET SDK nao encontrado.
    echo Instale em: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

where docker >nul 2>&1
if errorlevel 1 (
    echo.
    echo ERRO: Docker Desktop nao encontrado.
    echo Instale em: https://www.docker.com/products/docker-desktop/
    echo No Windows, se o Docker nao abrir, rode "wsl --install" num PowerShell e reinicie o PC.
    pause
    exit /b 1
)

set PKG=bun
where bun >nul 2>&1
if errorlevel 1 (
    where npm >nul 2>&1
    if errorlevel 1 (
        echo.
        echo ERRO: nem Bun nem npm foram encontrados. Instale o Node.js: https://nodejs.org
        pause
        exit /b 1
    )
    set PKG=npm
    echo Bun nao encontrado, vou usar npm no lugar dele.
)
echo OK - pre-requisitos encontrados.
echo.

echo [2/7] Instalando dependencias do frontend (%PKG% install)...
if "%PKG%"=="bun" (
    call bun install
) else (
    call npm install
)
if errorlevel 1 (
    echo ERRO ao instalar dependencias do frontend.
    pause
    exit /b 1
)
echo.

echo [3/7] Restaurando dependencias do backend (.NET)...
cd backend
call dotnet restore
if errorlevel 1 (
    echo ERRO ao restaurar dependencias do backend.
    pause
    exit /b 1
)
echo.

echo [4/7] Instalando a ferramenta dotnet-ef (se ainda nao tiver)...
call dotnet tool install --global dotnet-ef >nul 2>&1
echo OK.
echo.

echo [5/7] Subindo Postgres + MinIO (Docker)...
call docker compose up -d
if errorlevel 1 (
    echo ERRO ao subir os containers. Verifique se o Docker Desktop esta aberto.
    pause
    exit /b 1
)
echo Aguardando o banco de dados ficar pronto...
timeout /t 10 /nobreak >nul
echo.

echo [6/7] Aplicando as migrations (criando as tabelas)...
call dotnet ef database update --project src/CadernetaObras.Infrastructure --startup-project src/CadernetaObras.Api
if errorlevel 1 (
    echo ERRO ao aplicar as migrations.
    pause
    exit /b 1
)
echo.

echo [7/7] Aplicando os triggers de imutabilidade no banco...
docker exec -i caderneta-postgres psql -U caderneta_app -d gestao_obras < scripts\immutability-triggers.sql
echo.

cd ..

echo ===================================================
echo  Configuracao concluida!
echo.
echo  A partir de agora, para rodar o sistema, use sempre:
echo      iniciar-projeto.bat
echo ===================================================
pause
