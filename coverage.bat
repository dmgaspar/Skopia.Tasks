@echo off
echo ============================================
echo     RODANDO TESTES + CODE COVERAGE
echo ============================================
echo.

:: 1. Remove past coverage results
echo Limpando resultados antigos...
rmdir /S /Q "TestResults" >nul 2>&1
rmdir /S /Q "coverage-report" >nul 2>&1

:: 2. Run tests and collect coverage
echo Executando dotnet test com cobertura...
dotnet test --collect:"XPlat Code Coverage"

IF %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERRO: Os testes falharam. Abortando.
    pause
    exit /b %ERRORLEVEL%
)

:: 3. Generate HTML coverage report
echo.
echo Gerando relatorio HTML...
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html

IF %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERRO: Nao foi possivel gerar o relatorio HTML.
    pause
    exit /b %ERRORLEVEL%
)

:: 4. Open the report in browser
echo.
echo Abrindo relatorio...
start "" "coverage-report\index.html"

echo.
echo ============================================
echo         RELATORIO GERADO COM SUCESSO!
echo ============================================
echo.
pause
