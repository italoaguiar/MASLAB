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

MASLAB foi escrito em C# e XAML utilizando o [.net Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/current/runtime) e o framework gráfico [Avalonia UI](https://avaloniaui.net/).  Atualmente o .net Core 3.1 e Avalonia possuem suporte para Windows, Linux e MacOS.

# Instalação
Você pode fazer o download do software [aqui](https://github.com/italoaguiar/MASLAB/raw/master/bin/source.zip). Após o download, você pode descompactar o arquivo para uma pasta de sua preferência.

Para executar o software, o único requisito inicial é ter o **.net Core 3.1 runtime** instalado no computador. Você pode fazer a instalação seguindo as intruções nos links abaixo:

**Windows**: ([x64](https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-3.1.2-windows-x64-installer)) | ([x86](https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-3.1.2-windows-x86-installer)) Installer

**MacOS**: [(x64) Installer](https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-3.1.2-macos-x64-installer)

**Linux:** Abra um terminal e execute os seguintes comandos:
*O Comando abaixo refere-se ao Ubuntu 19.04, para outras versões, [siga este link](https://docs.microsoft.com/pt-br/dotnet/core/install/linux-package-manager-fedora31).*

    wget -q https://packages.microsoft.com/config/ubuntu/19.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb sudo dpkg -i packages-microsoft-prod.deb

Em seguida execute:

    sudo apt-get update 
    sudo apt-get install apt-transport-https 
    sudo apt-get update 
    sudo apt-get install dotnet-runtime-3.1



## Executando o software
***Atenção!** Antes de executar o software, certifique-se de que a instalação do .net Core 3.1 runtime foi efetuada.*

**Windows**:  Para executar o software no ambiente Windows, basta abrir a pasta em que o software foi descompactado e executar o arquivo **MASLAB.exe**.

**Linux e MacOS**: Após descompactar o software para alguma pasta do computador, inicie um terminal dentro da pasta do software. Execute o seguinte comando:

    $ dotnet MASLAB.dll

Obs: Este comando também é válido para o ambiente Windows.

# Sobre o software
Desenvolvido por [Ítalo A. Aguiar](https://github.com/italoaguiar/)
Sob orientação de Rodrigo Augusto Ricco.
Instituto de Ciências Exatas e Aplicadas - ICEA
Universidade Federal de Ouro Preto - UFOP