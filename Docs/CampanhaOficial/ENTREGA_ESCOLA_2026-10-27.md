# Entrega do redesign escolar — 27/10/2026

Implementação do plano completo aprovado pelo usuário. A candidata local **0.5.0-beta**, Windows x64, inclui o fechamento anterior e o novo puzzle **Padrões Ocultos**. A 0.4.9-beta permanece preservada como a entrega anterior ao redesign; o fechamento das pendências de 26/10 está em [ENTREGA_2026-10-27.md](ENTREGA_2026-10-27.md).

## Fluxo entregue

1. Na casa, abrir a caixa recupera uma única vez os recortes e as anotações infantis junto da chave e do caderno. A investigação da chave, as páginas e a viagem do Fusca continuam jogáveis.
2. Na Industrial, Renan e os alunos têm conversas cotidianas. A análise não exige respostas ou confirmação de Renan.
3. A mesa permite ordenar três jornais por datas impressas, destacar fatos repetidos, ligar três anotações aos trechos que as sustentam e comparar a origem C-96 com o mural.
4. A dedução aponta para o acervo de história local da **biblioteca da Industrial**, a área já existente na fase interna 6. A ação de partir inicia a transição, sem confirmação extra. O próximo puzzle chega pendente.

As reportagens são ficcionais, identificadas no próprio documento. As datas de janeiro de 1996, os relatos e as lembranças são conteúdo examinável; a resolução não depende de saber a lore. A escola não antecipa 1898, diocese ou selo. Os campos antigos de fotografia/horário ficam apenas como ponte de compatibilidade com inventário e fragmentos posteriores; o quiz antigo não pode desbloquear o capítulo.

## Interface e progresso

Os documentos completos abrem em leitor próprio e permanecem no caderno nos capítulos posteriores, independentemente da hotbar. O caderno tem anterior/próximo para consulta por teclado e controle, além da rolagem nativa. Os cartões usam quebra de linhas, destaques e conexões visuais; cada ligação pode ser desfeita/corrigida. Erros preservam as demais escolhas. Dicas têm três níveis, com solução identificada como spoiler.

O save v4 conserva documentos recuperados/examinados, ordem, destaques, ligações, destino selecionado, etapa e nível de dica. A migração lê versões 1/2/3, preserva conclusões e evidências posteriores e cria uma cópia `.vN.bak` do formato anterior quando ela ainda não existe. Quem já passou da escola não a repete. Partidas novas começam sem a análise concluída. Um save parcial da escola inicia o novo puzzle com seus materiais recuperados. A cópia anterior é necessária para recuperar o formato compatível ao voltar a uma build antiga.

## Testes e evidências

| Recorte | Resultado |
| --- | --- |
| `school-editmode-release.xml` | 437/437, zero falhas/ignorados |
| `school-player-final.xml` | 10/10, zero falhas/ignorados |
| `school-player-readable.xml` | 2/2, zero falhas/ignorados |
| `school-journal-controller.xml` | 1/1, zero falhas/ignorados |

XMLs e resumos ficam em `output/school-patterns-2026-10-27/`. O recorte de dez testes inclui o percurso real casa/caixa/páginas/Fusca/escola, saída para biblioteca, mesa/cadeira/globo/janelas, controles e persistência de escolhas erradas/dicas/reload. O recorte de dois repete a interface completa com teclado e controle após corrigir a legibilidade e a referência à biblioteca existente. O teste adicional verifica a consulta do caderno pelo controle. São treze execuções de Player e onze casos distintos; os dois percursos de interface foram repetidos.

As dezesseis capturas de percurso em `school-patterns/` mostram as quatro etapas, leitor, mural, conclusão e chegada à biblioteca com cada dispositivo. Há uma captura adicional da última anotação consultada pelo controle no caderno. O teste dedicado retira os itens da hotbar e percorre o catálogo até o fim. Os testes usam o sistema de entrada e os botões reais de IMGUI; não são mockups. A validação completa foi feita no Player de testes, sem anunciar uma partida manual de toda a campanha ou um teste de pessoa leiga.

Na primeira rodada de EditMode houve 433/436 aprovados: corrigidos a criação do ponto do mural quando o mapa é construído diretamente e o fixture que representava um save legado sem informar sua versão. Na primeira rodada de Player houve 8/10: o teste começava pelo último recorte da ordem embaralhada e tentava avançar além do documento final; a sequência do teste foi corrigida. XMLs/logs dessas tentativas foram preservados. A revisão visual posterior encontrou textos pequenos nos botões, corrigidos antes da rodada final.

## Build e pacote

- Executável: `Builds/Varginha_0.5.0-beta_2026-10-27/SegredoDeVarginha.exe`.
- ZIP: `Builds/Varginha_0.5.0-beta_2026-10-27_Windows_x64.zip`.
- Unity: 6000.6.0f1; 19 cenas; 0 erros e 2 avisos na geração.
- Pacote: 314,348,795 bytes; 198 arquivos; CRC e hashes críticos conferidos.
- SHA-256: `699e37f8209e7ea943e4e457750e27eb423a3a0dacb873851e28a9809de11fa1`.
- 366 fontes, testes, cenas, shaders e manifestos comparados com a cópia QA. `source-manifest.json` e `windows-build.json` acompanham a build.

O nome interno do produto foi preservado para localizar o progresso existente. Os testes usam uma cópia QA e uma identidade de dados separada. A configuração URP versionada foi usada na cópia de compilação; o arquivo temporário do workspace e os arquivos preexistentes de QA/recuperação foram preservados. Não houve nova publicação externa.

## Arquivos e cenas afetados

Novos scripts: `CampaignSchoolInvestigation.cs` (catálogo e regras), `CampaignSchoolSequence.cs` (leitor e interface), `CampaignSchoolInvestigationTests.cs` e `CampaignSchoolPatternsPlayTests.cs`.

Integrações: `CampaignHouseSequence.cs`, `VarginhaCampaignStage.cs`, `CampaignStory.cs`, `CampaignSequence.cs`, `CampaignJournal.cs`, `CampaignGuidance.cs`, `CampaignHints.cs`, `CampaignMapPlan.cs`, `CampaignIllustratedMaps.cs`, `CampaignRenanTeacher.cs` e `CampaignExpansionController.cs` (identificação do original C-96 no livro existente). Construção/empacotamento: `VarginhaDailyBuild.cs`, `package_school_20261027.py` e este gerador de relatório. Testes de campanha, saídas, mesa, versões de save e orientação foram atualizados para o fluxo aprovado.

Cenas afetadas em runtime: casa adulta **2**, escola **4** e entrada da biblioteca **6**. O caderno apresenta os materiais nos capítulos posteriores. Nenhum `.unity` ou PNG original foi alterado para o redesign. Arquitetura, sprites, colisões e puzzle da biblioteca foram conservados. A documentação da campanha de 14 capítulos foi atualizada; as referências históricas GDD/LDD foram preservadas.

## Validações externas pendentes

O teste manual com mouse não foi realizado: a automação de janelas foi interrompida pelo Escape do usuário e não foi retomada. Os botões compartilham a implementação nativa de mouse da interface existente; isso não substitui uma sessão validada com ponteiro. A candidata final não foi aberta por automação de janelas nesta etapa.

Também permanece pendente a sessão com uma pessoa que desconheça a lore e o teste em outro computador. O [roteiro de playtest](PLAYTEST_ESCOLA_PADROES_OCULTOS.md) está pronto. Não há evidência de uma sessão humana realizada. Os materiais de apresentação/daily da entrega 0.4.9 continuam como registro daquele fechamento, anterior a este redesign.
