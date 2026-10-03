# Campanha — cinco novas fases em 2D pixel art

Continuação da Fase 5, seguindo as **fases 06–10 do LDD de referência**. A numeração desse LDD difere do antigo plano resumido da campanha. A revisão refaz a arquitetura e a mobília das fases 1, 3, 4, 5, 6, 7, 8, 9 e 10 em pixel art 2D, com atlas e objetos produzidos pela ImageGen e referências do projeto original. O arquivo da cena adulta da Fase 2, a fachada real da Industrial e o modelo original do Fusca são preservados. Após o feedback posterior, a casa adulta recebe texturas, móveis proporcionais, luz suave e portais abertos durante o jogo. Não houve redesenho dos alunos.

## Construção dos mapas

A revisão final usa o laboratório do projeto original como referência de remaster para as fases 4 e 5: doze mesas, nove computadores, cadeiras azuis e corredor central. O estacionamento possui vagas e travessia. A cidade tem fachadas orientadas para a rua e pontos de ônibus baseados na foto de Varginha. A igreja possui arquivo, capela e cripta com tochas e vitrais; Ouzana dispõe de microscopia, documentação, recepção e cultivo. A mata e a oficina também têm peças e atmosfera próprias. Detalhes da referência e os prompts finais estão em [REMASTER_CENARIOS.md](C:/Users/Usuario/segredo-varginha-game-teste-cutscene-gdd-ldd-main/Docs/CampanhaOficial/REMASTER_CENARIOS.md).

Cada mapa tem duas etapas independentes: **01_Planta_Paredes_Divisoes** (pisos, cômodos e paredes com colisões) e **02_Mobilia_Colisoes** (móveis com ocupação física própria). Os móveis têm colisões junto à base; imagens altas não bloqueiam toda a sua projeção. Portas dos mapas novos possuem dois metros/unidades de passagem. As rotas são verificadas com margem para o colisor dos pés.

As plantas e a ocupação dos móveis estão em `Preview/CampaignMapsV2`, com PNGs de arquitetura, mobília, objetivos e fachada, além de SVGs de planta e colisões. No SVG, bege identifica paredes, marrom a imagem do móvel, vermelho sua colisão e verde os pontos de interação. Tapetes, janelas e telhados são elementos sem bloqueio adicional; as paredes e as casas sob os telhados continuam sólidas.

**Casa adulta:** a cena `FaseTopView_Varginha` e a cópia adulta da Fase 2 permanecem preservadas. A casa de 1996 possui planta própria com quarto infantil, sala, corredor, cozinha, serviço/banheiro e quintal. Mantém a saída à direita necessária ao encontro do prólogo. O mobiliário infantil e a circulação foram reposicionados, sem reconstruir a planta adulta.

## Fase 6 — Fragmentos / Ato III

Mapa novo: praça com relato de 1996, biblioteca/arquivo municipal e arquivo da Industrial. Consulte os três registros. O arquivo municipal fornece o terceiro fragmento; as outras fontes estabelecem coordenadas e orientação. No notebook, troque as peças até alinhar **árvore → rio → capela**, do oeste ao leste. A conclusão libera a mata. Há duas pistas opcionais: depoimento omitido e veículo sem placa.

Tab/G abre o caderno e a seleção dos três locais. A escolha leva ao acesso de cada microárea, sem colocar Edelzio dentro de móveis. Cada registro é salvo imediatamente.

## Fase 7 — A Mata / Ato III

Mapa novo: entrada, bifurcação, clareira, riacho com passagem, ruína da capela e esconderijo. Investigue os símbolos na ordem árvore, rio e capela. A segunda marca inicia uma perseguição por uma manifestação não combatível, com navegação pelas passagens do mapa. O abrigo interrompe o rastreamento e recupera sanidade; mover-se abandona o abrigo. Ser alcançado retorna ao ponto seguro mantendo as pistas. Depois das três marcas, encontre Padre Fábio na ruína.

## Fase 8 — A Âncora / Ato III

Mapa novo: arquivo de Fábio e subterrâneo religioso. Examine índice, inscrição e ficha; combine **1996 + Âncora + registro 23** no Tombo. A data de 1898 pertence à contenção anterior. A abertura revela `RECEPTÁCULO: EDELZIO / ESTADO DA ÂNCORA: ESTÁVEL`. Pedir explicação ou confrontar Fábio altera a confiança sem impedir a progressão. Um documento de Zé Gomes é a terceira pista opcional desta extensão.

## Fase 9 — Ouzana / Ato IV

Mapa novo: laboratório improvisado, bancada de amostras e depósito de documentação. Apresente o registro de Edelzio a Ouzana; examine controle, resíduo e protocolo. Organize **controle → resíduo → reagente**. A reação anômala convence Ouzana e libera o reagente. O teste de aprendizado é gratuito; seis cargas são entregues à oficina.

## Fase 10 — O Fusca Marcado / Ato IV

Mapa novo: oficina, bancada, área do Fusca e pista de teste. Borrife capô, porta e motor; cada região consome uma carga somente na primeira revelação. Conecte as marcas **árvore → rio → capela** na bancada para instalar o estabilizador temporário. A reserva permite repetir a investigação sem bloqueio por falta de carga. Interaja com o Fusca e conduza com A/D ou setas até a extremidade da pista. A fase só termina depois do teste.

A Fase 11 continua prevista no LDD. Encerrar a Fase 10 conclui esta extensão, sem executar um final da campanha.

## Controles e salvamento

WASD/setas: movimento; E: interação; Tab/G: caderno/mochila; Escape: fechar diálogo/investigação ou pausar. A pausa oferece configurações e salvar/voltar ao menu. As fases 6–10 seguem em `CampaignStory2026.json`, com migração de saves anteriores, pistas opcionais, escolhas, peças parcialmente ordenadas e posição validada contra a planta.

## Arte de Edélzio

Os quatro arquivos recebidos foram preservados em `Assets/Resources/Varginha/TeamArt`. Após a autorização para substituir as folhas com recortes defeituosos, quatro novos PNGs transparentes foram gerados pela ImageGen usando o PNG frontal limpo como referência. São 76 poses: caminhada, soco, sentar/digitar, café, agachar e alcançar. A frente em repouso mantém o PNG original. A seleção dessas poses é exclusiva da campanha; os atlas anteriores do protótipo permanecem preservados. Os prompts estão em `PROMPTS_EDELZIO_V2.md`.

Nas fases 4 e 5, o arquivo fica atrás da fachada, com acesso pelo corredor. Seus móveis são ocultos na visão externa e reaparecem quando se entra. A fachada inteira tem escala uniforme e sua base considera a margem transparente do desenho. O Fusca ocupa uma vaga marcada no estacionamento, fora da circulação do portão. Na Fase 10, a condução horizontal usa o sprite lateral original fornecido pelo usuário.

Os passos da campanha foram reduzidos para 28% do ganho anterior, preservando o controle geral de efeitos.

## Gerar e verificar

No Unity: **Varginha → Campanha → Reconstruir mapas V2 (preservar casa adulta)**. A ferramenta salva as nove cenas reconstruídas, atualiza a lista do build e exporta primeiro plantas, depois mobília. A entrada normal continua em **Jogar → Iniciar campanha**; uma campanha salva na Fase 5 pode seguir pela nova opção **Continuar para a Fase 6**.

Os resultados da revisão V2 ficam em `Logs/maps-v2*.log` e `Logs/maps-v2-*.xml`. A galeria e o relatório de validação acompanham esta entrega.
