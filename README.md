# Trabalho de REDES

<h1>Jogo da memória - computação UFPel</h1>

Jogo de memória funcionando [OK]<br>
Socket abrindo [OK]<br>
Comunicação por ip local [OK]<br>
Criar Sala [OK]<br>
Mostrar ip local [OK] <br>
Entrar na sala [OK] <br>
Definir protocolo [OK] <br>
Programar o GameLogic (interpretação do protocolo) [] <br>

<h1>Organização de Pastas</h1>
<b>Assets/GameSets</b>: Pasta raiz dos arquivos do projeto<br>
<b>Assets/GameSets/Scripts</b>: Todos os códigos estão nesta pasta<br>
<b>Assets/GameSets/Songs</b>: Todos os Sons estão nesta pasta<br>
<b>Assets/GameSets/Texturas</b>: Todas as imagens estão nesta pasta<br>

<h1>Organização de código</h1>
<b>Assets/GameSets/Scripts/Card</b>: Guarda as informações referentes a cada card<br>
<b>Assets/GameSets/Scripts/CardController</b>: Gerencia cada card individualmente<br>
<b>Assets/GameSets/Scripts/CardManager</b>: Gerencia todos os cards existentes na cena<br>
<b>Assets/GameSets/Scripts/SongManager</b>: Gerencia os sons da cena<br>
<b>Assets/GameSets/Scripts/Socket/User</b>: Gerencia o socket do user, contendo suas informações e gerenciando os métodos de comunicação por socket<br>
<b>Assets/GameSets/Scripts/Socket/Lobby</b>: Gerencia a interface (UI) do Lobby, fazendo requisições de ação para o user<br>
<b>Assets/GameSets/Scripts/Socket/Enemy</b>: Inutilizada (Idealmente controlaria o adversário, mas não será necessário)<br>

<h1>Métodos</h1>
<b>User</b>
<ul>
  <li>
  GameLogic:<br>
  Controla a lógica do jogo com base nas mensagens recebidas pelo socket (Um grande SwitchCase)
  </li>
  <li>
  ServerSocket:<br>
  Cria o socket no modelo Host, e aguarda a existencia de um adversário para começar o jogo
  </li>
  <li>
  MessageDecrypt/Encrypt:
  Reponsável por deixar a mensagem do tipo String em algo legível pelo socket (E vice-versa)
  </li>
</ul>


