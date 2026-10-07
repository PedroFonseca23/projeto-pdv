@echo off
set COMPILADOR=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
if not exist "%COMPILADOR%" set COMPILADOR=C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe

"%COMPILADOR%" /target:winexe /win32icon:icone.ico /reference:System.Data.SQLite.dll /reference:System.Security.dll /out:PDVPro.exe MenuPrincipal.cs BancoDeDados.cs Telas.cs AgenteFiscalManager.cs Seguranca.cs FormLogin.cs Criptografia.cs ValidacoesFiscais.cs Estilo.cs

pause
