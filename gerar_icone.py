from PIL import Image

# Vai pegar a imagem fundo.jpg (ou coloque o nome da foto que você quer usar)
# e convertê-la para um formato de Ícone perfeito para o Windows.
try:
    img = Image.open('icone.ico') 
    img.save('icone.ico', format='ICO', sizes=[(256, 256)])
    print("Ícone criado com sucesso!")
except Exception as e:
    print("Erro:", e)