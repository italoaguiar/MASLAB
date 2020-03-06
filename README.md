# Bem vindo ao MASLAB!

MASLAB é um software de simulação desenvolvido para a disciplina de Modelagem e Análise de Sistemas Lineares da Universidade Federal de Ouro Preto. MASLAB foi desenvolvido com a premissa de simular o comportamento físico de sistemas de tanques contendo líquidos, permitindo modelar e observar seu comportamento ao longo do tempo.

![Tela inicial do MASLAB](https://1.bp.blogspot.com/-7o_Xp4GqpBE/XlZq090ImZI/AAAAAAAAChs/A9Js2rSCUxMISmHCUtbDQx0W7SjpY9JVgCLcBGAsYHQ/s1600/maslab.png)

A interface gráfica do software permite ao usuário desenhar de forma simples um conjunto de tanques a serem simulados. Note que não há um limite máximo de tanques, sendo assim, o limite é definido pelo poder computacional da máquina em que o software é executado.

Cada um dos tanques possui 2 entradas e uma única saída, sendo que cada uma destes acessos pode ser acessado e configurado individualmente para cada tanque. Para isso, o software utiliza a linguagem de programação C#, onde o utilizador poderá inserir um código customizado para controlar cada tanque do modo que preferir.
![Código de simulação do tanque](https://1.bp.blogspot.com/-I0tJrn3Nz74/XlZtT7CJWaI/AAAAAAAACh4/yuDJ0gJPNdEbxl6NYqwL6SAotJxzw8sFACLcBGAsYHQ/s1600/maslab2.png)

O editor de código escrito para o software é baseado no componente AvalonEdit, um dos principais editores de código disponíveis para o dotnet. O editor conta com uma integração avançada à plataforma Roslyn do dotnet, escrita especificamente para este software, permitindo recursos avançados como análise sintática e semântica, auto sugerir, trechos de código automáticos, compilação em tempo de execução, etc.

MASLAB inclui nativamente o suporte para as bibliotecas do [Math.net](https://www.mathdotnet.com/), permitindo que o usuário possa utilizar funções de computação numérica e simbólica sem grandes complicações.

# Plotando Gráficos
MASLAB inclui alguns métodos embutidos no código de simulação de cada tanque. Uma destas funções, **Plot**, permite desenhar gráficos durante a execução da simulação. Plot possui a seguinte sintaxe:

    Plot("Nome_da_série", TimeSpan instante, double valor);
  
**"Nome_da_serie"** define o conjunto que irá receber os pontos da plotagem. Se uma série inexistente for inserida, automaticamente ela será criada.

**instante** é uma variável do tipo *TimeSpan*. Essa variável é fornecida através do parâmetro **tempo** do método **OnUpdate** da simulação. Embora seja aconselhável passar o parâmetro **tempo** diretamente para o método Plot, por algum motivo especial, pode ser necessário utilizar valores customizados. Neste caso, você pode criar quantos objetos do tipo TimeSpan forem necessários. Por exemplo:

    TimeSpan ts = TimeSpan.FromSeconds(5);
    Plot("Minha Serie", ts, 1.873);


**valor** é exatamente isso, o valor a ser plotado. O valor é uma variável do tipo double. Note que em C# o sinal de separação de casas decimais para o tipo double é o ponto final (.).
![enter image description here](https://1.bp.blogspot.com/-bPVanQT06jY/XlZ5I0ldAHI/AAAAAAAACiE/rTkQwa4CsUk-NO6E2gIe86QiGn60QIlAgCLcBGAsYHQ/s1600/maslab3.png)

# Plataformas Suportadas

MASLAB está diponível para o Windows, Linux e macOS.

# Instalação
[![link](https://img.shields.io/github/downloads/italoaguiar/MASLAB/0.0.0.2/total?color=%235b8dde&label=Windows%200.0.0.2&style=for-the-badge)](https://github.com/italoaguiar/MASLAB/releases/download/0.0.0.2/win-x64.zip)

[![link](https://img.shields.io/github/downloads/italoaguiar/MASLAB/0.0.0.2/total?color=%237ad128&label=macOS%200.0.0.2&style=for-the-badge)](https://github.com/italoaguiar/MASLAB/releases/download/0.0.0.2/osx-x64.zip)

[![link](https://img.shields.io/github/downloads/italoaguiar/MASLAB/0.0.0.2/total?label=Linux%200.0.0.2&style=for-the-badge)](https://github.com/italoaguiar/MASLAB/releases/download/0.0.0.2/linux-x64.zip) 

Basta fazer o download da versão adequada ao seu sistema operacional e descompactar o arquivo para uma pasta.

## Executando o software

**Windows**:  Para executar o software no ambiente Windows, basta abrir a pasta em que o software foi descompactado e executar o arquivo **MASLAB.exe**.

**Linux e MacOS**: Após descompactar o software para alguma pasta do computador, execute o o arquivo **MASLAB**. Pode ser necessário conceder alguma permissão ao software.


# Sobre o software
Desenvolvido por [Ítalo A. Aguiar](https://github.com/italoaguiar/)
Sob orientação de Rodrigo Augusto Ricco.

Instituto de Ciências Exatas e Aplicadas - ICEA

Universidade Federal de Ouro Preto - UFOP
