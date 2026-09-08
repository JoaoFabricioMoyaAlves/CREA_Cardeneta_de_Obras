@echo off
cd /d "%~dp0"

echo ===================================================
echo  Caderneta de Obras Digital - Iniciando o sistema
echo ===================================================
echo.
echo Vou abrir 3 janelas de terminal, uma para cada parte
echo do sistema. NAO feche essas janelas enquanto estiver
echo usando o sistema - para desligar tudo, feche as 3.
echo.

echo Abrindo Terminal 1/3: Postgres + MinIO (Docker)...
start "1 - Banco de dados (Docker)" cmd /k "cd /d %~dp0backend && docker compose up"
timeout /t 6 /nobreak >nul

echo Abrindo Terminal 2/3: Backend (API .NET)...
start "2 - Backend (.NET API)" cmd /k "cd /d %~dp0backend && dotnet run --project src/CadernetaObras.Api"
timeout /t 4 /nobreak >nul

echo Abrindo Terminal 3/3: Frontend...
where bun >nul 2>&1
if errorlevel 1 (
    start "3 - Frontend" cmd /k "cd /d %~dp0 && npm run dev"
) else (
    start "3 - Frontend" cmd /k "cd /d %~dp0 && bun run dev"
)

echo.
echo ===================================================
echo  Tudo iniciado! Aguarde uns 20-30 segundos para o
echo  backend e o frontend terminarem de compilar, depois
echo  acesse:
echo.
echo      http://localhost:8080
echo ===================================================
pause
