# Revisão após playtest - 22/10/2026 - 0.4.6-beta

O autor testou a build 0.4.5-beta e reportou sobreposições nas janelas da casa adulta e na mesa do Renan. O plano visual foi aprovado com “yes aprovado bro”. A revisão corrige esses pontos sem alterar a solução das missões.

## Resultado

- Os vidros diurnos acompanham a profundidade de suas paredes. A ordem global 23000 colocava a paisagem por cima de todos os personagens, cortando o Edelzio. Agora cada vidro segue a parede que contém a sua região e fica logo acima dela.
- A mesa conserva a arte e a colisão originais. Mochila cinza, notebook com a traseira da tela voltada ao jogador e três pincéis de quadro usam sprites separados com transparência real, apoiados no tampo. Foram retirados dessa composição a caneca e o caderno anteriores.
- O conjunto é criado uma única vez por controlador; objetos não se duplicam numa segunda chamada. O notebook continua abrindo a fotografia da investigação.
- A daily mantém dez slides e o encerramento para dúvidas/opiniões/feedbacks. As capturas da revisão estão no slide 7 e a validação específica no slide 8.
- A lista de efeitos para a equipe distingue material já recebido, substituições de síntese provisória, complementos e gatilhos novos. A dublagem humana foi adiada pelo autor para 23/10.

## Arquivos e cenas

- `Assets/Scripts/Game/Varginha/Experiment/CampaignAdultMorning.cs`: suporte de profundidade dos vidros.
- `Assets/Scripts/Game/Varginha/Experiment/CampaignRenanTeacher.cs`: composição e contatos dos objetos sobre a mesa.
- `Assets/Resources/Varginha/StoryCharacters/RenanDeskProps.png` e `.json`: novo atlas e recortes; `CampaignSchoolFurnitureImporter.cs`: importação sem redução, Point e sem mipmaps/compressão.
- `CampaignCinematicOpeningPlayTests.cs` e `CampaignSchoolDeskPlayTests.cs`: regressão das janelas e teste integrado de cenário/interação.
- `VarginhaDailyBuild.cs`: entrega em pasta nova 0.4.6-beta. `Tools/QA/run_school_feedback.ps1`: isolamento e restauração das configurações.
- `Tools/SchoolDeskArt/prepare_art.py`: receita/proveniência da arte. `art/technical-report.md`: especificações e revisão visual.
- `Docs/CampanhaOficial/PEDIDO_EFEITOS_SONOROS_2026-10-22.md`: pedido para som. Lista copiável e CSV em `output/audio-request-2026-10-22/`.
- Cenas afetadas em runtime: casa adulta (`Ato2_Fase2_A_Chave_e_a_Caixa`), escola (`Ato2_Fase4_Entre_Aulas_e_Pistas`), sua retomada para a investigação e o retorno à escola da continuação, onde o componente da mesa é reutilizado. Nenhum arquivo `.unity` foi regravado; não há nova missão/puzzle.

## Testes e evidências

| Verificação | Resultado | Evidência |
|---|---|---|
| EditMode completo | 398/398, zero falhas/skips | `editmode.xml`, `editmode-summary.json` |
| Player Windows direcionado | 3/3, zero falhas/skips | `player-native.xml`, `player-summary.json` |
| Atlas da skill | 3 frames/3 clips estáticos, zero erros/warnings estruturais | `art/validation/validation.json` |
| Daily | 10 slides, nove transições, fonte/imagens/navegação/movimento reduzido e overflow aprovados | `daily-qa.json` |
| Build Windows | Succeeded, zero erros, um aviso do pacote opcional Unity.Pipeline | `windows-build.json` |
| Pacote | SHA-256, manifesto e CRC conferidos, sem assemblies de testes | `windows-package.json` |

Os três casos do Player verificaram: (1) manhã e progresso intactos; (2) objetos apoiados, sem sobreposição entre os três recortes, sem novas colisões, sem duplicação, com fotografia acessível; (3) Edelzio à frente das duas janelas superiores. As seis capturas estão em `captures/`. A mesa, as duas posições do personagem e os slides 7/8 foram abertos e examinados visualmente.

Estes são **três testes direcionados desta revisão**, não uma repetição dos 16 casos únicos da entrega anterior nem 19 novos casos. O uso de `productName` exclusivo isola o persistentDataPath e o registro do Player de QA do jogo que o autor deixou aberto. ProjectSettings e o arquivo URP anterior do usuário foram restaurados ao final. As alterações preexistentes em URP e os arquivos de recuperação permanecem fora do commit.

## Pendências reais

Reteste do autor na nova build, playtest de outra pessoa, dublagem humana em 23/10, produção/recebimento dos sons pedidos, revisão das falas propostas, mixagem e desempenho em outro PC. Não foi realizado novo playtest humano nem gravada voz nesta revisão.
