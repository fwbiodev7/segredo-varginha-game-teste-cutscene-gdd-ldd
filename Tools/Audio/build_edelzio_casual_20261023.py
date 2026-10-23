"""Build the approved recording copy and verify that game dialogue matches it."""
import csv
import hashlib
import json
import re
import zipfile
from pathlib import Path
from pypdf import PdfReader
from reportlab.pdfgen import canvas

from export_voice_script import line_id
from build_edelzio_simple_20261023 import put, entry, footer, norm, W, H, M

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / 'output/dublagem-2026-10-23/mapa_de_audios.json'
OUT = ROOT / 'output/dublagem-2026-10-23/versao-descontraida'
PDF = ROOT / 'output/pdf/roteiro_edelzio_definitivo.pdf'
TXT = OUT / 'roteiro_edelzio_definitivo.txt'
ZIP = OUT / 'roteiro_edelzio_definitivo.zip'

REVISIONS = {
    'ED01': ('Vou dar a volta pra chegar no notebook.', 'Natural, falando consigo mesmo.'),
    'ED02': ('Minha caixa de brinquedos... Faz tempo, hein? Depois eu vejo isso.', 'Uma lembrança rápida, sem parar a rotina.'),
    'ED03': ('Vou deixar a chave aqui e pegar a mochila.', 'Tranquilo, sem desconfiar de nada.'),
    'ED04': ('A chave tá aqui. Só falta a mochila.', 'Cotidiano, sem pressa.'),
    'ED05': ('Ué... cadê a chave? Nem no chão ela tá.', 'Estranhando o sumiço.'),
    'ED06': ('A porta tá trancada por dentro. Eu nem mexi nela.', 'Pensando alto, sem tirar conclusões.'),
    'ED07': ('Olha esse papel velho... Esse traço azul parece o dos meus desenhos de criança.', 'Curioso, começando a reconhecer.'),
    'ED08': ('Calma... Antes de abrir essa caixa, deixa eu entender o que tá acontecendo.', 'Cauteloso, sem dramatizar demais.'),
    'ED09': ('A porta não abriu... Esse papel parece ser da caixa. Pera... tem algo arranhando debaixo da cama.', 'Começa pensando; termina prestando atenção ao barulho.'),
    'ED10': ('Pronto, guardei tudo. Mas essa frase... Ela ainda está aqui.', 'Um alívio curto que vira inquietação.'),
    'ED11': ('Dessa vez, essa chave fica comigo.', 'Firme, com um pouco de desconfiança.'),
    'ED12': ('Antes de sair, preciso olhar a caixa e juntar essas páginas.', 'Lembrando o que ainda precisa fazer.'),
    'ED13': ('Deixa eu falar com o Renan antes de mexer no notebook dele.', 'Natural, como numa conversa.'),
    'ED14': ('É esse o horário... O Renan precisa ver isso.', 'Percebeu uma pista; sem ler números em voz alta.'),
    'CR01': ('Não deixa ela sair.', 'Baixo, com medo contido. Sem filtro de rádio.'),
}


def build_pdf(rows, vocals):
    c = canvas.Canvas(str(PDF), pagesize=(W, H))
    c.setTitle('Falas do Edelzio em tom natural')
    c.setAuthor('O Segredo de Varginha')
    groups = [rows[:5], rows[5:11], rows[11:14]]
    for page, group in enumerate(groups, 1):
        y = H - M
        if page == 1:
            y = put(c, y, 'Falas do Edelzio para gravar', 'title', 5)
            y = put(c, y, 'Versão R2   Conversa natural e frases mais curtas', 'small', 11)
            y = put(c, y, 'São 14 falas do adulto e 1 da criança. Leia só o texto maior. Fale como você falaria no dia a dia; a tensão cresce conforme a cena.', 'body', 6)
            y = put(c, y, 'Uma tomada boa basta. Deixe um segundo de silêncio antes e depois. Sem música ou efeitos. Nomeie ED01_t01, ED02_t01... e envie tudo na pasta Edelzio_R2.', 'body', 15)
        else:
            y = put(c, y, 'Edelzio adulto', 'title', 6)
            y = put(c, y, 'Versão R2   Leia só o texto maior.', 'body', 17)
        for r in group:
            y = entry(c, y, r)
        if page == 3:
            y = put(c, y, 'Edelzio criança', 'section', 5)
            y = put(c, y, 'Voz mais jovem e receosa. O efeito de rádio fica para a edição.', 'small', 10)
            y = entry(c, y, rows[14])
            y = put(c, y, 'Sons de voz opcionais', 'section', 5)
            y = put(c, y, 'Faça só se forem pedidos. Interprete o som, sem ler a descrição.', 'small', 8)
            for r in vocals:
                y = put(c, y, r['code'] + '   ' + r['context'] + ': ' + r['cue'], 'small', 5)
            y = put(c, y, 'Envio', 'section', 5)
            y = put(c, y, 'Mande os arquivos originais na melhor qualidade, de preferência juntos em ZIP. Se puder, use WAV. Não precisa converter nem editar. Se repetir uma fala, use t02 no nome.', 'small', 0)
        footer(c, page, 'Edelzio R2')
        c.showPage()
    c.save()


def main():
    original = json.loads(SOURCE.read_text(encoding='utf-8'))
    assert len(original['main']) == 15
    revised = []
    for old in original['main']:
        text, cue = REVISIONS[old['code']]
        r = dict(old)
        r.update(fala=text, cue=cue, original_text=old['fala'], original_id=old['id'],
                 id=line_id(old['personagem'], text), revision='R2',
                 recording_status='Texto aprovado aplicado ao diálogo do jogo' if old['code'] not in ('CR01','ED03') else 'Fala infantil preservada com o mesmo ID' if old['code']=='CR01' else 'Texto aprovado aplicado à cutscene; reprodução de voz ainda precisa ser ligada')
        r['estado']='E'
        revised.append(r)
    house=(ROOT/'Assets/Scripts/Game/Varginha/Experiment/CampaignHouseSequence.cs').read_text(encoding='utf-8-sig')
    stage=(ROOT/'Assets/Scripts/Game/Varginha/Experiment/VarginhaCampaignStage.cs').read_text(encoding='utf-8-sig')
    for r in revised[:-1]:
        assert r['fala'] in house+'\n'+stage, 'Fala aprovada ausente do jogo: '+r['code']
    assert 'else Say("Edelzio",line);' in house
    assert 'line + " A observação ficou registrada no caderno."' not in house
    assert 'CampaignMessagePresentation.IdFor("Edelzio criança","Não deixa ela sair.")' in house
    assert '23:23' in stage and 'schoolTimeChoice' in stage
    vocals = original['optional_vocals']
    OUT.mkdir(parents=True, exist_ok=True)
    PDF.parent.mkdir(parents=True, exist_ok=True)
    build_pdf(revised, vocals)
    text = ['FALAS DO EDELZIO PARA GRAVAR', 'VERSÃO R2 - TOM NATURAL', '',
            '14 falas do adulto e 1 da criança. Leia só a fala, sem o número e a orientação.',
            'Fale como no dia a dia. Uma tomada boa basta. Deixe um segundo de silêncio antes e depois.',
            'Nomeie ED01_t01, ED02_t01... e envie os originais na pasta Edelzio_R2, de preferência em ZIP.', '']
    for r in revised:
        text += [r['code'] + ' - ' + r['context'], '(' + r['cue'] + ')', r['fala'], '']
    text += ['SONS OPCIONAIS - SOMENTE SE PEDIDOS', 'Interprete o som. Não leia a descrição.', '']
    for r in vocals:
        text += [r['code'] + ' - ' + r['context'] + ': ' + r['cue'], '']
    text += ['ENVIO', 'Mande os arquivos originais na melhor qualidade. Se puder, use WAV. Não precisa converter nem editar. Para repetir uma fala: ED01_t02.', '']
    TXT.write_text('\n'.join(text), encoding='utf-8-sig')
    with (OUT / 'MAPA_TECNICO_R2.csv').open('w', encoding='utf-8-sig', newline='') as f:
        writer = csv.DictWriter(f, fieldnames=['codigo', 'personagem', 'texto_anterior', 'texto_novo', 'id_anterior', 'id_novo', 'arquivo_recebido', 'arquivo_final', 'situacao'])
        writer.writeheader()
        for r in revised:
            writer.writerow(dict(codigo=r['code'], personagem=r['personagem'], texto_anterior=r['original_text'], texto_novo=r['fala'], id_anterior=r['original_id'], id_novo=r['id'], arquivo_recebido='Edelzio_R2/' + r['code'] + '_t01.ext', arquivo_final=r['id'] + '.wav', situacao=r['recording_status']))
    (OUT / 'mapa_de_audios_R2.json').write_text(json.dumps(dict(revision='R2', main=revised, optional_vocals=vocals, game_modified=True, future_proposals_modified=False, integration_approval_pending=False), ensure_ascii=False, indent=2), encoding='utf-8')
    plan = ['# Revisão descontraída das falas do Edelzio', '',
            'A solicitação pede um tom mais descontraído, menos específico e com menos datas. Foram reescritas as 14 falas adultas do primeiro lote. A frase infantil foi preservada. O anexo de 57 propostas futuras continua separado, sem alterações.', '',
            '## Problemas encontrados', '',
            'As falas atuais repetem o registro do caderno, listam itens do inventário e contêm orientações em tom de tutorial. Uma fala menciona o ano da infância e outra recita o horário resolvido. O ID da voz depende do texto exato, portanto reescrever só o roteiro não atualiza o diálogo do jogo.', '',
            '## Solução aprovada e aplicada ao jogo', '',
            'Substituir as falas adultas pelos textos R2 abaixo; remover apenas o complemento falado sobre registro no caderno, preservando o registro e o save; manter o horário 23:23 no puzzle e as datas nos documentos; preservar a frase infantil e a progressão das pistas. Não antecipar a causa do sumiço nem a natureza da entidade.', '',
            'O usuário aprovou o plano e pediu o nome roteiro_edelzio_definitivo.pdf. As falas foram aplicadas em CampaignHouseSequence.cs e VarginhaCampaignStage.cs. A rotina de build foi atualizada para gerar uma candidata 0.4.8-beta em pasta separada.', '',
            '## Textos aprovados', '']
    for r in revised:
        plan += ['### ' + r['code'] + ' ' + r['context'], '', '**Antes:** ' + r['original_text'], '', '**R2:** ' + r['fala'], '']
    plan += ['## Continuidade preservada', '',
             'A mochila ainda motiva sair da mesa. A chave continua ausente da mesa e do chão. A porta permanece trancada por dentro. O papel é antigo, tem traço azul e remete aos desenhos de infância. A caixa e os arranhões continuam sob a cama. A frase da página é mantida. Renan continua sendo consultado antes do notebook. A fala do horário só ocorre depois da resolução; o número continua disponível na interface.', '',
             '## Arquivos e validação', '',
             'O PDF definitivo, TXT, ZIP, JSON de revisão e CSV com IDs novos/anterior foram gerados. O roteiro anterior permanece disponível. As novas falas não contêm datas nem números falados. Os textos estão nos scripts de casa/escola, sem alterar os arquivos de cena, arte ou as respostas dos puzzles. O PDF definitivo está em output/pdf/roteiro_edelzio_definitivo.pdf. O ZIP contém esse mesmo nome e o TXT. A linha ED03 continua como legenda de cutscene; não foi declarada uma dublagem já integrada ou recebida.', '']
    (OUT / 'PLANO_E_COMPARACAO_R2.md').write_text('\n'.join(plan), encoding='utf-8')
    with zipfile.ZipFile(ZIP, 'w', zipfile.ZIP_DEFLATED) as z:
        z.write(PDF, PDF.name)
        z.write(TXT, TXT.name)
    pdf = PdfReader(PDF)
    body = norm(' '.join(p.extract_text() for p in pdf.pages))
    main_text = ' '.join(r['fala'] for r in revised)
    checks = dict(pages=len(pdf.pages) == 3,
                  all_lines=all(norm(r['fala']) in body for r in revised),
                  unique_codes=len({r['code'] for r in revised}) == 15,
                  no_spoken_numbers=re.search(r'\d', main_text) is None,
                  no_spoken_log_notice='A observação ficou registrada no caderno.' not in main_text,
                  child_line_preserved=revised[-1]['fala'] == 'Não deixa ela sair.' and revised[-1]['id'] == revised[-1]['original_id'],
                  no_technical_ids='fala_' not in body,
                  all_new_hashes_correct=all(r['id'] == line_id(r['personagem'], r['fala']) for r in revised),
                  fourteen_revised_ids=sum(r['id'] != r['original_id'] for r in revised) == 14,
                  no_blank_pages=all(len(norm(p.extract_text())) > 90 for p in pdf.pages),
                  approved_dialogue_matches_game=all(r['fala'] in house+'\n'+stage for r in revised[:-1]),
                  spoken_clue_suffix_removed='else Say("Edelzio",line);' in house,
                  exact_requested_filename=PDF.name=='roteiro_edelzio_definitivo.pdf')
    with zipfile.ZipFile(ZIP) as z:
        checks['zip_integrity'] = z.testzip() is None and len(z.namelist()) == 2
    report = dict(success=all(checks.values()), checks=checks, pages=len(pdf.pages), lines=15, rewritten=14, optional_vocals=5, game_modified=True, integration_approval_pending=False, pdf_sha256=hashlib.sha256(PDF.read_bytes()).hexdigest(), failures=[k for k, v in checks.items() if not v])
    (OUT / 'qa-conteudo-R2.json').write_text(json.dumps(report, ensure_ascii=False, indent=2), encoding='utf-8')
    print(json.dumps(report, ensure_ascii=False))
    if not report['success']:
        raise SystemExit(1)


if __name__ == '__main__':
    main()
