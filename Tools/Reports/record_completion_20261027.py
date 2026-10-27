from pathlib import Path
import json
import re

ROOT=Path(__file__).resolve().parents[2]
EV=ROOT/'output/completion-2026-10-27'
def read(path):return json.loads(path.read_text(encoding='utf-8-sig'))
build=read(EV/'windows-build.json');package=read(EV/'windows-package.json')
daily=read(ROOT/'output/daily-sprint-2026-10-20/.build/2026-10-27/qa.json')
assert build['result']=='Succeeded' and not build['errors'] and package['success'] and daily['success']
summaries={name:read(EV/(name+'-summary.json')) for name in ['final-editmode','final-player-regression','chapter-exits-final','final-return-and-hud']}
for result in summaries.values():assert result['success'] and result['tests']['total']>0
counts=lambda name:f"{summaries[name]['tests']['passed']}/{summaries[name]['tests']['total']}"
report=f'''# Entrega de 27/10/2026 - fechamento da revisão

As pendências técnicas e de apresentação herdadas de 26/10 foram concluídas nesta rodada. A candidata local é **0.4.9-beta**, Windows x64, com 19 cenas. O projeto conserva 14 capítulos apresentados ao jogador. O teste externo com pessoas e em outros PCs continua como próxima etapa do produto.

## Resultado por pendência

| Pendência | Resultado e evidência |
| --- | --- |
| Notebook, cadeira e piso (02) | Concluído. O shader recompõe as linhas de madeira com amostras das extremidades descobertas, retém a textura e limpa a sombra deslocada da mesa antiga. Capturas de aproximação, uso e retorno inspecionadas em `output/completion-2026-10-27/world-review/`. O teste da cadeira passou no Player no recorte final. Não houve alteração dos PNGs originais nem dos colisores. |
| Conclusão e transições (11) | Concluído no recorte de saídas. Player {counts('chapter-exits-final')}, com 31 capturas de conclusão/chegada/créditos/menu revisadas. Os saves preparados completam a origem e deixam o objetivo do destino pendente. Inclui prólogo, escola, biblioteca, mata, igreja, Ouzana, oficina, arquivos, acesso de serviço/porão, Fábio, memória, criatura, manifestação e epílogo. A viagem casa-escola também passou no recorte final de regressão. |
| Roteiro de 30/10 (19) | Concluído. `Docs/Apresentacao/ROTEIRO_COMPLETO_30-10-2026.md` e `output/pdf/Roteiro_Apresentacao_Varginha_30-10-2026.pdf`, seis páginas revisadas. Dez blocos de fala, demo, tempos, preparação e perguntas. Histórico desde 11/09, narrativa, tecnologias, sistemas, audiovisual, dificuldades e próximos passos. |
| Daily e acesso à apresentação (20) | Concluído como material local. HTML de dez slides atualizado para 27/10, notas coerentes, nove efeitos preservados e versão PDF de dez páginas. O PDF foi criado para leitura no painel de arquivos; não houve navegação nem nova tentativa de abrir `file://` no navegador integrado. |
| Build consolidada | Concluído. Gerada após os ajustes finais, com {build['errors']} erros e {build['warnings']} avisos. Pacote ZIP, CRC e hashes dos arquivos críticos conferidos. {package['source_files_verified']} fontes/cenas/shaders/manifestos foram comparados com a cópia QA usada na compilação. |

## Ajustes encontrados durante o fechamento

- O prólogo agora recusa uma segunda confirmação de avanço, sem alterar seu texto ou painel narrativo.
- Na fase interna 20, a barra de vida fica abaixo do objetivo antes/depois da batalha. Durante a luta, o HUD conserva a disposição de combate. Captura `chapter-exits/18-to-20-arrival.png` inspecionada após o ajuste.
- O teste antigo da Ouzana ainda esperava seis poses por ciclo. Foi atualizado para a seleção de três poses aprovada em 26/10, mantendo a validação dos seis quadros nativos, oito direções, dimensões, filtro e registro. A tentativa inicial teve 412 aprovados e oito falhas dessa expectativa antiga; XML/log preservados em `attempts/`. A repetição completa aprovou {counts('final-editmode')}.

## Testes de 27/10

| Recorte | Resultado | Evidência local |
| --- | --- | --- |
| EditMode completo, depois dos ajustes | {counts('final-editmode')}, zero falhas/ignorados | `final-editmode.xml` |
| Player: sprites, cadeira, chave, Fusca e física | {counts('final-player-regression')}, zero falhas/ignorados | `final-player-regression.xml` |
| Player: saídas e próxima investigação ainda pendente | {counts('chapter-exits-final')}, zero falhas/ignorados | `chapter-exits-final.xml` |
| Player: HUD, chegada ao retorno e travessia/reload/epílogo | {counts('final-return-and-hud')}, zero falhas/ignorados | `final-return-and-hud.xml` |
| Daily | Dez slides, notas, imagens, impressão, teclado e nove efeitos aprovados | `output/daily-sprint-2026-10-20/.build/2026-10-27/qa.json` |

Os XMLs acima ficam em `output/completion-2026-10-27/`. Há sobreposição entre recortes; as contagens não foram somadas como casos únicos. As capturas de transições exercitam os manipuladores reais de saída com progresso preparado, não uma partida manual completa do começo ao fim. Os originais e os saves do usuário foram preservados pela cópia isolada de QA.

## Build e pacote

- Executável: `Builds/Varginha_0.4.9-beta_2026-10-27/SegredoDeVarginha.exe`.
- Pacote: `Builds/Varginha_0.4.9-beta_2026-10-27_Windows_x64.zip`.
- SHA-256 do ZIP: `{package['sha256']}`.
- Arquivos no ZIP: {package['files']}. Tamanho: {package['bytes']:,} bytes.
- `source-manifest.json` acompanha a build. Registra a base Git, alterações locais, hashes das fontes comparadas e hashes dos XMLs de validação.
- Os {build['warnings']} avisos registrados na build referem-se à API Unity obsoleta usada em `CampaignRenanTeacher`. Não bloquearam a geração.
- A build anterior de 26/10 às 11h16 e as demais entregas foram preservadas. Não foi criada uma nova publicação externa ou Release.

## Arquivos e cenas afetados

Código de jogo: `IllustratedMorning.shader`, `VarginhaCampaignPhase1.cs` e `CampaignContinuationController.cs`. Construção: `VarginhaDailyBuild.cs`. Testes: `CampaignChapterExitReviewTests.cs` e `CampaignOuzanaVisualTests.cs`. Ferramentas: runner QA com pasta de evidências parametrizada, empacotador e scripts de documentação/daily.

Nenhum arquivo `.unity` ou textura-fonte foi modificado. Efeitos em runtime: casa adulta (fase interna 2), conclusão do prólogo (1) e HUD da manifestação (20). As demais cenas foram carregadas para validação das saídas. Lista de pendências, atualização diária, README e material da apresentação receberam o fechamento.

## Limites e próximos passos do produto

Ficam para playtest externo o balanceamento, a clareza para quem não conhece os puzzles, a mixagem percebida e a execução em outro computador. Dublagem humana completa, finais alternativos, publicação em lojas e requisitos mínimos medidos não são anunciados como entregues. Os quatro móveis sem aproximação livre da auditoria de 26/10 permanecem documentados naquele recorte.

Arquivos locais anteriores de QA, recuperação e URP temporário foram mantidos. A build usa a configuração URP versionada na cópia isolada, sem restaurar indiscriminadamente o arquivo do workspace. Logs, caches e arquivos de recuperação não integram o pacote do jogador.
'''
(ROOT/'Docs/CampanhaOficial/ENTREGA_2026-10-27.md').write_text(report,encoding='utf-8')
replacements={
 '02':'| 02 | Notebook, cadeira e escrivaninha | Concluído | Fechado em 27/10: piso reconstruído por linha com textura e iluminação locais, sombra antiga da mesa removida. Aproximação, uso e retorno aprovados no Player e inspecionados em `output/completion-2026-10-27/world-review/`. Sprites e colisores originais preservados. |',
 '11':f'| 11 | Conclusão discreta das fases | Concluído | Fechado em 27/10: {counts("chapter-exits-final")} saídas/epílogo no Player, 31 capturas revisadas, próxima investigação pendente e confirmação dupla rejeitada. Viagem casa-escola aprovada; HUD da batalha e travessia final passaram no recorte adicional {counts("final-return-and-hud")}. Evidências em `output/completion-2026-10-27/`. |',
 '19':'| 19 | Roteiro da apresentação completa em 30/10/2026 | Concluído | Roteiro completo em `Docs/Apresentacao/ROTEIRO_COMPLETO_30-10-2026.md` e PDF de seis páginas. Fala de 12 a 15 minutos, histórico desde setembro, narrativa, sistemas, arte/áudio, dificuldades, demo e perguntas prováveis. |',
 '20':'| 20 | Abrir a apresentação atualizada nesta sessão | Concluído | Daily de 27/10 atualizada e verificada em dez slides, com HTML e PDF local. A versão PDF permite leitura no painel de arquivos sem navegar para `file://`. A restrição histórica do navegador integrado não foi contornada. |'
}
for relative in ['Docs/CampanhaOficial/PENDENCIAS_REVISAO_MESTRE_2026-10-26.md','output/revisao-mestre-2026-10-23/PENDENCIAS.md']:
    file=ROOT/relative;content=file.read_text(encoding='utf-8-sig')
    lines=content.splitlines()
    for i,line in enumerate(lines):
        match=re.match(r'^\| (02|11|19|20) \|',line)
        if match:lines[i]=replacements[match[1]]
    content='\n'.join(lines)+'\n'
    if 'Fechamento de 27/10/2026' not in content:
        content += '\n## Fechamento de 27/10/2026\n\nOs itens restantes desta revisão foram concluídos. Estado final, arquivos, testes, build e limites em `Docs/CampanhaOficial/ENTREGA_2026-10-27.md`. As entradas de 23 e 26/10 acima permanecem como histórico.\n'
    file.write_text(content,encoding='utf-8')
continuity=ROOT/'output/revisao-mestre-2026-10-23/CONTINUIDADE.md'
with continuity.open('a',encoding='utf-8') as stream:
    stream.write(f'\n## Fechamento verificado - 27/10/2026\n\nPedido atual: concluir as pendências e criar build atualizada. Itens 02, 11, 19 e 20 fechados; candidata 0.4.9-beta no diretório Builds do workspace. EditMode {counts("final-editmode")}; regressão Player {counts("final-player-regression")}; saídas {counts("chapter-exits-final")}; HUD/travessia {counts("final-return-and-hud")}. Daily HTML/PDF de dez slides e roteiro completo de seis páginas concluídos. Consultar Docs/CampanhaOficial/ENTREGA_2026-10-27.md antes de retomar: diagnósticos antigos parciais não são a situação atual. Não repetir testes nem alterar jogo sem novo motivo. Preservados arquivos locais anteriores, cenas, texturas e saves. Não houve publicação externa nesta rodada.\n')
daily_note=ROOT/'Docs/CampanhaOficial/ATUALIZACAO_2026-10-26.md'
with daily_note.open('a',encoding='utf-8') as stream:
    stream.write('\n## Fechamento posterior em 27/10/2026\n\nAs pendências deste registro foram encerradas na entrega 0.4.9-beta. Consulte [a entrega de 27/10](ENTREGA_2026-10-27.md) para estado atual, evidências, arquivos e limitações. Este texto conserva o histórico do trabalho de 26/10.\n')
readme=ROOT/'README.md';text=readme.read_text(encoding='utf-8-sig')
text=text.replace('**Em desenvolvimento · 14 capítulos · candidata Windows 0.4.4-beta · atualização: 21/10/2026.**','**Em desenvolvimento · 14 capítulos · candidata local Windows 0.4.9-beta · atualização: 27/10/2026.**')
anchor='Revisão atual: '
text=text.replace(anchor,'Entrega local de 27/10: [fechamento da revisão, testes e build](Docs/CampanhaOficial/ENTREGA_2026-10-27.md), [daily atualizada](output/daily-sprint-2026-10-20/A_Sprint_Final_Varginha.html) e [roteiro para 30/10](Docs/Apresentacao/ROTEIRO_COMPLETO_30-10-2026.md). Pacote: `Builds/Varginha_0.4.9-beta_2026-10-27_Windows_x64.zip`. As versões públicas e os registros abaixo são históricos.\n\nRevisão histórica de 21/10: ',1)
readme.write_text(text,encoding='utf-8')
print(json.dumps({'success':True,'report':'Docs/CampanhaOficial/ENTREGA_2026-10-27.md','closed_items':[2,11,19,20]},ensure_ascii=False))
