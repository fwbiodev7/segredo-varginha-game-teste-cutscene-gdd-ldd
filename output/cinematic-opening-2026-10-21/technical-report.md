# Relatório técnico de arte e animação — 21/10/2026

Skills: `pixel-art-sprite-sheet-master` e `imagegen`. Escopo final: cabine redesenhada do zero a partir da fotografia de Fusca fornecida pelo usuário, close da ignição, reutilização dos sprites de Edelzio, desembarque no mapa da escola e interferência distribuída. Nenhum sprite original de personagem, Fusca ou evidência foi repintado.

## Resultado e identidade

A cabine usa duas camadas novas: estrutura azul/porta/banco de vinil preto (`FuscaCabinPhotoShell`) e painel horizontal marrom com velocímetro, rádio antigo e porta-luvas (`FuscaDashboardPhoto`). Essa separação permite posicionar o painel imediatamente atrás do volante que já existe no sprite. Não há segundo volante nem haste longa acrescentada. O mesmo Edelzio dirige com quatro frames leste originais, rosto/óculos/cabelo/camisa preservados; escala uniforme 2,05 no canvas 1280×720, cerca de 24% maior que a prévia inicial.

A ignição tem três poses: segurar a chave inserida, soltar e afastar a mão. A chave de latão com chaveiro azul fica no mesmo cilindro. O antebraço está exposto, sem punho de manga comprida. Não há queimadura, ferimento ou explicação nova para o reinício espontâneo.

O desembarque reutiliza `IllustratedMaps/School.png`, o layout da fase 4, a vista superior original do Fusca e os frames `WalkGray`. A rota usa as colisões da fase e termina no ponto autoral onde começa a jogabilidade; não substitui o cenário por outra fachada.

## Especificações e arquivos

- Cabine: 1672×941, janelas transparentes, partes internas opacas.
- Painel: 2172×724, módulo isolado com alpha. Exibição em (552,325,780,260); instrumentos animados registrados no mostrador e rádio.
- Ignição: 2172×724, três células 724×724, sem padding, referência de pivô (314,284), tempos 850/650/2800 ms, one-shot de 4300 ms. PPU documental 724; o runtime usa recortes UV da textura.
- Direção: quatro crops do manifesto original, QA em canvas 320×320, pivô (150,300), 250 ms/frame e loop de 1 s. O atlas de QA não substitui a fonte aprovada do jogo.
- Importação: Point, sem mipmap, compressão, escala NPOT ou cópia CPU. Painel e ignição usam limite 4096 para conservar a largura. O resolver GPU reduz apenas os novos fundos/painel e resolve alpha binário; a ignição opaca é mostrada diretamente. Os sprites aprovados não passam por esse filtro.
- As fontes geradas são PNGs achatados com muitos tons, não produção nativa em 256 px ou paleta estrita de 16/32 cores. Não foi produzida fonte Aseprite em camadas. Há camadas PNG independentes, receitas, JSON canônico, planos de slicing e código de composição.

Fontes/hashes: `art/source-hashes.json`, `art/photo-cabin-provenance.json`, `art/ignition-provenance.json`. Prompt final da ignição: `art/ignition-prompt.txt`. Metadados/preview/validação: `art/driver` e `art/ignition`. As fontes de cabine anteriores permanecem preservadas como referência; não representam o runtime final.

## Interferência

Focos variáveis em duas dimensões, comprimentos/opacidades distintos, relógios independentes por foco, fragmentos da cabine deslocados, ecos cromáticos e partículas/campos verdes e violetas. As rajadas deixam espaço livre entre os danos e intervalos de leitura. Um teste verifica os quatro quadrantes ao longo do tempo e limita a área coberta. As legendas ficam fora do viewport afetado. Movimento reduzido usa assinatura estável de baixa opacidade, sem ecos móveis. Não há símbolos inventados ou rosto/entidade revelados; o RNG do gameplay não é consumido.

## Revisão e evidências

Foram abertos contact sheets, overlays do loop de direção e das três poses da ignição. A página da skill foi reproduzida em velocidade normal e confirmou avanço dos clips (`art/playback.json`). As quatro montagens `cabin-registration-0..3.png` verificam o encaixe na cabine final; são prévias de composição, não testes Unity. Capturas reais do Player e a prévia silenciosa verificam a montagem integrada, a liberação de controle e as alternativas normal/reduzida.

A validação da ignição tem zero erros e três avisos de conteúdo tocando o canvas: são tomadas opacas que preenchem o quadro; a saída do antebraço é intencional. O atlas original de direção conserva seu alpha; relatórios iniciais e contrato revisado permanecem preservados. Validação geométrica e hashes não concedem aprovação artística humana.

A primeira arte adicional apresentou alteração de cor no tratamento GPU; a quantização RGB foi removida. A mão isolada e o enquadramento com haste foram substituídos após a revisão do usuário. Na primeira tela do novo puzzle, as pistas longas ficaram cortadas; foram reduzidas a frases de duas linhas e capturadas novamente. Resultados finais e limites: `RELATORIO.md`. Playtest humano, avaliação da equipe e outro PC continuam pendentes; dublagem adiada.


Paleta integral do inventário final: `art/cabin-inventory/palette.zip`, contendo o `palette.json` original sem alterações. SHA-256 dos bytes descompactados e CRC conferidos em `palette-archive.json`. `palette.png` é apenas uma amostra visual, não uma alegação de paleta limitada. Prompts finais da cabine: `art/photo-cabin-prompts.md`.
