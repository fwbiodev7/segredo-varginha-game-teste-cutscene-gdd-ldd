# Pacote de áudio do Edelzio — 22/10/2026

O arquivo `C:\Users\Usuario\Downloads\sons do edelzio.rar` foi recebido em 22/10/2026 e extraído localmente para conferência. Ele contém 16 gravações, todas tratadas como efeitos e ambientes do jogo; não há falas de dublagem com texto identificável. A dublagem humana continua separada para 23/10, quando o microfone estiver disponível.

Os arquivos embarcados ficam em `Assets/Resources/Varginha/Audio/Received20261022/` e são referenciados pelo catálogo `Catalog.json`. Os nomes originais foram preservados na tabela para rastreabilidade:

| ID no jogo | Arquivo recebido | Uso |
|---|---|---|
| PortalOpen | `abrindoopopodoedelzioPORtal.mp3` | abertura do portal da sequência sobrenatural |
| FabioAlien | `alieniginafabio.wav` | camada alienígena associada ao Padre Fábio |
| Scratching | `arranhandomadeira.mp3` | arranhões na caixa de 1996 |
| CoffeeDrink | `bebenocafe.mp3` | beber o café na rotina adulta |
| Key | `chavedoedelziobatendo.wav` | chave batendo na mesa e na investigação |
| PowerFailure | `energiacaiuedelzinhobb.mp3` | queda de energia da abertura |
| DoorClose | `fechando porta.wav` | fechamento da porta do Fusca |
| PortalClose | `fechandoportaldabundadoedelzio.mp3` | fechamento do portal |
| FuscaEngine | `fusca.wav` | motor do Fusca em deslocamento; loop de 34,403 s |
| LabAmbience | `laboratorio.mp3` | ambiente do laboratório |
| FaceWash | `lavandorostododedezinho.wav` | lavar o rosto na preparação |
| LevelUp | `levelupedelzinho.mp3` | confirmação de progresso |
| EdelzioFear | `medoedelzio.wav` | esforço vocal/medo, reservado para gatilho dramático |
| PickupPlim | `plim.wav` | confirmação curta de coleta |
| NightExit | `som de sair a noite.wav` | ambiente noturno de saída |
| Typing | `teClado.mp3` | teclado do notebook |

As interações de café, mochila, notebook, lavagem, queda de energia, porta e Fusca já possuem chamadas no runtime. O catálogo antigo de 21/10 continua carregado primeiro; o pacote de 22/10 acrescenta IDs e remapeia apenas `Key`, `Scratching` e `Typing` para os sons fornecidos pelo autor. Nenhum arquivo do pacote anterior foi apagado.

Para a dublagem, ainda faltam as gravações humanas das falas do roteiro. `EdelzioFear` é um efeito vocal longo e não substitui uma fala legendada.
