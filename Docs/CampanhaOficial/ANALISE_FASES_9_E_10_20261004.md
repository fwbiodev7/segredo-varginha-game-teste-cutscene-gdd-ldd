# Análise das fases 9 e 10 — 04/10/2026

## Resultado

**17/17 testes PlayMode e 69/69 testes EditMode passaram.** Os testes em execução cobrem movimentação e interação por teclado, respostas incorretas, progressão, recarga, salvamento, retomada, pausa, colisões e conclusão. A confirmação dos puzzles foi exercitada pelo mesmo controlador chamado pela interface; não é uma automação de cliques do mouse em cada botão.

Relatórios completos: [PlayMode](ValidacaoOficina20261004PlayMode.json) e [EditMode](ValidacaoOficina20261004EditMode.json).

## Fase 9 — Ouzana

| Mecânica | Verificação |
|---|---|
| Conversa com Ouzana | Registra as evidências do Livro do Tombo antes de liberar o reagente. |
| Controle, resíduo e protocolo | Os três pontos são alcançáveis com caminhada e interação por E. A ordem de coleta é livre. |
| Protocolo | CONTROLE → RESÍDUO → REAGENTE funciona após a conversa e as três leituras. Sem esses requisitos ou com ordem incorreta, o resultado é recusado. |
| Recompensa | Entrega seis cargas; salvar e carregar preserva o desbloqueio. |
| Teste gratuito | Não consome cargas. Atualmente explica a reação por diálogo; não executa uma demonstração visual própria. |
| Saída | O ponto físico libera a conclusão e a continuação para a oficina quando o reagente está desbloqueado. |
| Personagens | Ouzana usa o atlas de bióloga, e Edelzio mantém sua altura normal fora da igreja. |

## Fase 10 — O Fusca Marcado

A chegada passou a ser jogável. Edelzio começa no Fusca, na rua diante da oficina. WASD/setas conduzem o veículo nas quatro direções. É preciso entrar pela passagem central, parar na vaga marcada com a frente voltada para cima e pressionar E para estacionar e desembarcar. As interações com a lataria ficam disponíveis depois disso.

O veículo usa um único atlas com quatro vistas, todas com interior vazio. As colisões acompanham sua orientação. A chegada salva a posição e a direção do carro; ao retomar, o jogador continua conduzindo. Saves de investigações anteriores que já revelaram marcas continuam com o carro estacionado.

| Mecânica | Verificação |
|---|---|
| Estacionamento | O carro entra pela garagem, para na vaga, libera Edelzio em um ponto sem sobreposição e retoma uma chegada parcialmente salva. |
| Reagente sem carga | Não revela marcas. A reserva recupera seis cargas após o reagente estar desbloqueado. |
| Aplicação | Capô, porta e motor podem ser examinados em qualquer ordem. Cada região nova consome uma carga; repetir uma marca não consome outra. |
| Marcas | ÁRVORE I, RIO II e CAPELA III aparecem na lataria. A posição permanece ligada ao carro e se adapta às vistas da pista. |
| Estabilizador | ÁRVORE → RIO → CAPELA funciona após revelar as três marcas. A ordem incorreta é recusada. |
| Pista | D/seta direita avança e A/seta esquerda recua. Não é possível iniciar antes da estabilização ou concluir apenas por entrar no Fusca. |
| Pausa | Congela o veículo e permite retomar a condução. |
| Retomada do teste | Mantém as marcas, cargas e estabilização; um teste interrompido recomeça na pista. |
| Desembarque final | O carro permanece na pista, sem estacionar em cima do ponto onde Edelzio reaparece. |
| Encerramento | Interagir com a saída inicia a partida do Fusca pela rua, com motor, faróis, faixas de cinema, legenda curta e fade. Depois, a conclusão permite salvar e voltar ao menu. |

## Outros ajustes pedidos

Na fase 1, a falha elétrica revela um brilho pequeno no quintal, perto da origem do clarão. A luz desaparece quando começa o encontro; o efeito respeita a opção de movimento reduzido.

O texto da revelação da igreja e o caderno agora identificam **Edelzio como o selo da entidade**. ÂNCORA permanece somente como o símbolo do compartimento. Os nomes internos de campos antigos de save continuam compatíveis, e os documentos históricos em `Referencias` foram preservados.

O resumo da abertura atual está em [ABERTURA_ATUAL_20261004.md](ABERTURA_ATUAL_20261004.md), e o passo a passo de estacionamento e conclusão foi incorporado ao [guia dos puzzles](GUIA_COMPLETO_PUZZLES.md).

## Recursos e preservação

As quatro vistas do Fusca compartilham uma textura com limite de 1024 pixels; as três marcas compartilham outra com limite de 512. As duas usam Point, sem mipmaps, sem compressão e sem cópia legível na CPU. Os sprites são recortados uma vez e reutilizados. Os faróis reaproveitam os pequenos meshes e o shader já existentes; não foram adicionadas luzes Unity em tempo real.

A conferência de integridade confirmou os nove mapas originais e cinco cenas/artes protegidas idênticos às referências. Os testes não equivalem a uma medição de FPS em todos os equipamentos nem garantem ausência de qualquer bug fora dos fluxos exercitados.

## Arquivos principais

- `Assets/Scripts/Game/Varginha/Experiment/CampaignExpansionController.cs`: chegada, marcas, pista, partida, HUD e textos.
- `Assets/Scripts/Game/Varginha/Experiment/CampaignExpansionState.cs`: persistência e compatibilidade da oficina.
- `Assets/Scripts/Game/Varginha/Experiment/CampaignWorkshopVehicle.cs`: condução, vaga, colisões e vistas do Fusca.
- `Assets/Scripts/Game/Varginha/Experiment/CampaignReagentMarks.cs`: revelação e posicionamento das marcas.
- `Assets/Scripts/Game/Varginha/Experiment/CampaignPreFlashGlow.cs` e `VarginhaCampaignPhase1.cs`: brilho anterior ao clarão.
- `Assets/Resources/Varginha/StoryEffects/`: dois PNGs e seus manifestos, com metadados de importação.
- `Assets/Tests/PlayMode/CampaignLabWorkshopAuditTests.cs`, `CampaignExpansionPlayTests.cs` e `Assets/Tests/EditMode/CampaignWorkshopStateTests.cs`: validações novas e compatibilidade da regressão existente.
- Documentação e capturas nativas em `Docs/CampanhaOficial`, `Preview/IllustratedMaps` e `Preview/UI`.
