@echo off
echo Limpando compilacoes anteriores...
rmdir /s /q build
rmdir /s /q dist
del /q *.spec

echo Instalando versoes compativeis das bibliotecas...
py -m pip install --upgrade pip
py -m pip install "pynfe>=0.2.0" reportlab pyinstaller

echo Compilando o agente fiscal...
py -m PyInstaller --noconfirm --onefile --noconsole --collect-all pynfe --collect-all reportlab agente_fiscal.py

echo Compilacao concluida! Pegue o 'agente_fiscal.exe' dentro da pasta 'dist' e jogue na raiz do PDV.
pause