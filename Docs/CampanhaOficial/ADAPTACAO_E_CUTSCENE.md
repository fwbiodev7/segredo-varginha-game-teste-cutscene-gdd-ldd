# Adaptação incremental e experiência de abertura

## Estado da primeira entrega

- Origem: `fwbiodev7/misterio_de_varginha-jogofeiratecnica-2026-prot-tipo_principal`, commit `f34e0db`.
- Cópia independente com histórico Git preservado; o remoto aponta somente para o novo repositório.
- Unity 6000.6.0f1 e dependências existentes preservadas.
- Nenhum script, asset ou cena de gameplay foi alterado nesta entrega.
- O GDD do usuário e os três documentos enviados foram copiados integralmente para `Referencias`.
- `Docs/GDD.md` continua como documentação do protótipo existente. Para a nova campanha, consultar `PLANO_20_FASES.md` e o GDD recebido.
- As 20 fases e a cutscene de abertura são planejamento. Não foram criadas cenas prontas nem geradas vozes/imagens novas.

## Reaproveitamento das três fases

| Base atual | Reutilizar | Adaptação necessária |
| --- | --- | --- |
| Casa / `FaseTopView_Varginha` | Móveis, TV, caixa, mochila, chave, caderno, notebook, interação, animações adultas e Fusca | Versão da casa em 1996, personagem infantil, pistas cartográficas e rotina conectada ao roteiro |
| Escola / `Fase2_Escola_Resgate` | Estacionamento, sala de informática, alunos, computadores, cadeiras interativas e chegada/partida | Apresentar a escola em normalidade, acrescentar Renan e investigação; separar o fluxo de combate/resgate da exploração |
| Igreja / `Fase3_Igreja_Guardiao` | Ambiente, padre, livro, iluminação, diálogos e símbolos | Arquivo explorável, puzzles de documentos, revelação do selo vivo e retirada da vitória de protótipo como encerramento da campanha |

Sistemas aproveitáveis: movimento, interação, apresentação de inventário, remapeamento de controles, animações, câmera, diálogos e transições.

O notebook atual usa um quiz de três perguntas. Reutilizar sua apresentação, mas desenvolver os puzzles de código, documentos, mapas e timeline separadamente. Não chamar o quiz atual de sistema completo de investigação.

Renan não foi encontrado nos scripts consultados e não consta na lista atual dos nove alunos. Sua inclusão na Industrial ainda é trabalho futuro.

A fala atual de Fábio sobre o selo ter escolhido Edelzio deve ser revisada. O nome de Edelzio também não deve constar literalmente de um registro pessoal redigido em 1898: distinguir o arquivo histórico da entrada feita em 1996.

## Storyboard inicial — reportagem de aproximadamente 55 segundos

| Tempo | Plano | Recursos |
| --- | --- | --- |
| 0–5 s | Preto, TV liga e sintonia estabiliza | TV atual como referência; som/efeito novos |
| 5–15 s | Apresentador fala dos relatos em Varginha | Estúdio e apresentador novos; paleta coerente |
| 15–27 s | Reportagem externa e depoimentos contraditórios | Objetos urbanos/vegetação reaproveitados e adaptados a 1996; personagens novos |
| 27–37 s | Fotografias, recortes e mapa dos relatos | Material investigativo novo; símbolo conectado a uma pista posterior |
| 37–45 s | Voz repete, sinal falha, imagem congela | Efeito de transmissão e áudio novos; pulsação visual atual como apoio |
| 45–55 s | Câmera revela a TV, a sala e Edelzio criança | Casa/móveis atuais; criança, enquadramento e transição novos |

Depois: exploração livre da casa, anomalia na TV, brilho na janela e sequência curta do clarão. A abertura não revela o acordo nem apresenta a criatura com anatomia legível.

Usar reportagens ficcionais próprias do universo do jogo. Texto inserido manualmente; vozes, legendas, luz e estática em camadas editáveis. Folhas de personagens aprovadas antes da arte final.

## Base técnica já existente

`VarginhaTravelCinematic` e `VarginhaTravelPixelArt` já compõem uma viagem em 384×216, com exposição visual a 12 quadros por segundo, fades e carregamento de cena. Reaproveitar os recursos visuais adequados, sem tratar esse fluxo específico de viagem como controlador genérico de todas as cutscenes.

`Docs/GUIA_PROLOGO_TIMELINE.md` e `Docs/Exemplos/PrologoTimeline` contêm guia e exemplos. O próprio guia informa que cena, animações e assets Timeline ainda precisam ser criados/conectados. A dependência Timeline já consta no projeto.

## Ordem proposta para os próximos testes

1. Revisar o cânone e registrar quais sistemas do protótipo permanecem no jogo oficial.
2. Criar uma cena experimental própria, preservando as cenas atuais como referência.
3. Provar reportagem → TV da casa → criança jogável com placeholders identificados.
4. Acrescentar o clarão e conferir devolução de controle, câmera, áudio e conclusão narrativa.
5. Garantir que término e skip produzam o mesmo estado; permitir skip após primeira visualização e opção de acessibilidade.
6. Adaptar a rotina adulta e incluir o primeiro fragmento do mapa.
7. Apresentar Renan na Industrial e substituir gradualmente o quiz por análise de evidências.
8. Adaptar a Igreja e o Livro do Tombo à revelação do selo vivo.
9. Só depois expandir mapas, puzzles e campanha; manter uma fatia jogável por entrega.

## Validação desta cópia

Nesta entrega a validação é de integridade do repositório e documentação: comparação com o commit de origem, preservação dos arquivos existentes, hashes das quatro referências, links locais dos documentos e verificação do destino remoto.

Não foi executado Play Mode nem produzida uma build nova. Como não há mudanças de gameplay, nenhuma validação de cutscene ou de campanha completa é alegada.
