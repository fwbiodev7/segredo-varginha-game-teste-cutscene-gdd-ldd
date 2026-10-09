# Revisão visual e física — alpha 0.1

Data: 09/10/2026. Plano aprovado pelo usuário antes da implementação.

## Resultado das alterações

- Revisados os 22 layouts da campanha, incluindo as áreas adicionais do casarão e do retorno à escola. A revisão partiu de 315 objetos; foram refinados 114 recortes distintos existentes e adicionadas três plantas com meshes próprios, totalizando 318 entradas.
- Fragmentos: corrigida a regra que aplicava a proteção do tronco indiscriminadamente às paredes; as duas colunas internas bloqueiam pelos contatos no chão. A escada bloqueia na base e seus degraus/longarinas usam partes separadas, deixando os vãos transparentes. A planta suspensa não bloqueia a circulação.
- Plantas das mesas central, de leitura e redonda passaram a ter objetos próprios, com profundidade ligada ao móvel de suporte. A folhagem usa recortes alinhados aos pixels da imagem original, sem recriar ou repintar o PNG.
- Móveis e personagens usam o contato no chão para ordenar a sobreposição. Foram refinadas silhuetas de cadeiras, mesas, estantes, camas, eletrodomésticos, plantas, árvores, colunas e equipamentos.
- Ajustadas quatro bases que ultrapassavam o recorte visual: árvore oeste do jardim do casarão, estante da sala do casarão, caixas oeste do arquivo e cilindro da ruptura.
- Igreja: URP 2D configurado com nove projeções Freeform, em azul, âmbar e violeta, com bordas graduais. O personagem recebe a cor por pixel no material; o cenário tem resposta reduzida para preservar a projeção já pintada na arte. Sem bloom, desfoque, MSAA ou HDR adicional.
- Corrigida a filtragem da arena final e das vistas do Fusca original para Point, sem compressão dos pixels. Mantidos os arquivos PNG, resolução, escala dos mapas, pontos de entrada, iluminação ambiente anterior e coordenadas de todos os objetivos.
- Capturas da pausa e das transições adaptadas à API de renderização do URP, conservando os recursos existentes.

## Arquivos de implementação

| Arquivo | Alteração |
|---|---|
| `Assets/Resources/Varginha/IllustratedMaps/Layouts.json` | Silhuetas, partes de meshes, três plantas, contatos de colisão e projeções dos vitrais |
| `Assets/Scripts/Game/Varginha/Experiment/CampaignIllustratedMaps.cs` | Construção dos meshes compostos e profundidade pelos contatos de apoio |
| `Assets/Scripts/Game/Varginha/Experiment/CampaignIllustratedContour.cs` | Aplicação das silhuetas no editor e no jogo |
| `Assets/Scripts/Game/Varginha/Experiment/CampaignWallBody.cs` | Limites da proteção do corpo em paredes |
| `Assets/Scripts/Game/Varginha/Experiment/CampaignIllustratedLighting.cs` | Material de vitral e recuperação de blocos de propriedades após recompilação |
| `Assets/Scripts/Game/Varginha/Experiment/CampaignStainedGlassLighting.cs` | Luzes 2D e materiais compartilhados dos vitrais |
| `Assets/Resources/Varginha/IllustratedMaps/StainedGlassLighting.shader` | Resposta de iluminação URP por pixel, preservando as texturas |
| `Assets/Scripts/Game/Varginha/VarginhaPixelPresentation.cs` | Captura de câmera compatível com URP |
| `Assets/Scripts/Game/Varginha/Experiment/CampaignCinematics.cs` | Uso da captura compatível na pausa e nas transições |
| `Assets/Settings/CampaignPixelRenderer.asset` | Renderer 2D, textura de iluminação em escala 1 e material padrão unlit |
| `Assets/Settings/CampaignPixelPipeline.asset` | Pipeline sem MSAA/HDR e com escala de renderização 1 |
| `ProjectSettings/GraphicsSettings.asset`, `ProjectSettings/QualitySettings.asset` | Referências válidas ao pipeline em todas as qualidades |
| `Assets/Scripts/Game/Game.asmdef` | Referências às bibliotecas URP já instaladas |
| `Assets/Resources/Varginha/IllustratedMaps/FinalBattleArena.png.meta` | Point e compressão desativada |
| `Assets/Resources/Varginha/StoryEffects/FuscaOriginal*.png.meta` | Point e compressão desativada nas seis vistas |
| `Assets/Scripts/Editor/Testing/VarginhaAlphaBuild.cs` | Nome e pasta da build `jogo_varginha_alpha_0.1` |
| `Assets/Scripts/Editor/Testing/CampaignIllustratedMapBuilder.cs` | Capturas de revisão compatíveis com URP |
| `Assets/Scripts/Editor/Testing/CampaignPixelPipelineBuilder.cs` | Configuração reproduzível do pipeline |
| `Assets/Scripts/Editor/Testing/CampaignPolishRunner.cs` | Execução explícita dos testes e da build no editor aberto |
| `Tools/RefineVisualLayouts.py` | Processo reproduzível dos recortes vetoriais, sem editar as imagens originais |
| `AGENTS.md` | Preferência do usuário: apresentar plano e aguardar aprovação antes de novas alterações |

Também foram adicionados testes de recortes, contatos, movimento real e cores medidas no renderizador. Os auxiliares de captura de quatro testes existentes foram adaptados ao URP, sem alterar suas verificações de gameplay. A lista integral de arquivos é fornecida em `ARQUIVOS_MODIFICADOS.txt`.

## Cenas afetadas e verificadas

As cenas são montadas em runtime pelos scripts e pelo layout. As alterações não exigiram salvar novos objetos nos arquivos `.unity` da campanha.

| Layout | Cena |
|---|---|
| 1 | `Ato1_Fase1_O_Caso_de_Varginha` |
| 2 | `Ato2_Fase2_A_Chave_e_a_Caixa` |
| 3 | `Ato2_Fase3_Nao_Deixa_Ela_Sair` |
| 4 | `Ato2_Fase4_Entre_Aulas_e_Pistas` |
| 5 | `Ato2_Fase5_O_Codigo_das_2323` |
| 6 | `Ato3_Fase6_Fragmentos` |
| 7 | `Ato3_Fase7_A_Mata` |
| 8 | `Ato3_Fase8_A_Ancora` |
| 9 | `Ato4_Fase9_Ouzana` |
| 10 | `Ato4_Fase10_O_Fusca_Marcado` |
| 11 | `Ato4_Fase11_Arquivos_Alterados` |
| 12 | `Ato4_Fase12_Casarao_de_Ze_Gomes` |
| 112 | `Ato4_Fase12_Casarao_de_Ze_Gomes_Area1` |
| 13 | `Ato4_Fase13_Registros_de_1898` |
| 14 | `Ato5_Fase14_O_Que_Fabio_Escondeu` |
| 15 | `Ato5_Fase15_A_Noite_Interrompida` |
| 16 | `Ato5_Fase16_A_Descida` |
| 18 | `Ato6_Fase18_A_Criatura_Ferida` |
| 19 | `Ato6_Fase19_O_Acordo_Esquecido` |
| 20 | `Ato6_Fase20_O_Verdadeiro_Segredo` |
| 21 | `Ato6_Fase21_O_Retorno` |
| 121 | `Ato6_Fase21_O_Retorno_Area1` |

Foram verificadas também as cenas desativadas na sequência atual, sem reintroduzi-las na campanha. A build mantém a sequência de cenas que o jogo já utiliza.

## Evidências

Os resultados finais dos testes, medições de cor e relatório de build estão em `VALIDACAO.md`. As capturas `Fase_*.png`, `Fragmentos_*.png` e `Igreja_Vitral_*.png` registram a verificação no Unity. As folhas `*_Crops.png` mostram os recortes de origem e `*_Meshes.png` são visualizações diagnósticas das silhuetas.

O percurso de todas as cenas, os testes de física e os testes de regressão são automatizados. As capturas foram inspecionadas visualmente; isso não representa uma partida manual completa nem certificação em hardware diferente desta máquina.
