"""Review the supplied recording script and package an actor-facing, traceable version."""
from pathlib import Path
from collections import Counter
from html import escape
import csv
import hashlib
import json
import re
import sys
import unicodedata
import zipfile

from pypdf import PdfReader
from reportlab.pdfgen import canvas
from reportlab.platypus import Paragraph
from reportlab.lib.styles import ParagraphStyle
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.lib.pagesizes import A4

from export_voice_script import line_id

ROOT = Path(__file__).resolve().parents[2]
INPUT = Path(r'C:/Users/Usuario/Downloads/ROTEIRO_EDELZIO.pdf')
OLD = ROOT / 'Docs/CampanhaOficial/Dublagem_2026-10-22/roteiro_gravacao.json'
OUT = ROOT / 'output/dublagem-2026-10-23'
PDFOUT = ROOT / 'output/pdf'
QA = ROOT / 'tmp/pdfs/roteiro-edelzio-2026-10-23'
MAIN = PDFOUT / 'ROTEIRO_EDELZIO_SIMPLES_23-10-2026.pdf'
RESERVE = PDFOUT / 'EDELZIO_PROPOSTAS_PARA_REVISAO_23-10-2026.pdf'
TXT = OUT / 'ROTEIRO_EDELZIO_SIMPLES_23-10-2026.txt'
W, H = A4
M = 45
CW = W - 2 * M

pdfmetrics.registerFont(TTFont('Actor', 'C:/Windows/Fonts/arial.ttf'))
pdfmetrics.registerFont(TTFont('ActorBold', 'C:/Windows/Fonts/arialbd.ttf'))
pdfmetrics.registerFont(TTFont('ActorItalic', 'C:/Windows/Fonts/ariali.ttf'))
styles = {
    'title': ParagraphStyle('Title', fontName='ActorBold', fontSize=23, leading=28),
    'section': ParagraphStyle('Section', fontName='ActorBold', fontSize=16, leading=21),
    'body': ParagraphStyle('Body', fontName='Actor', fontSize=11.5, leading=16),
    'cue': ParagraphStyle('Cue', fontName='ActorItalic', fontSize=10.5, leading=14, textColor='#475569'),
    'label': ParagraphStyle('Label', fontName='ActorBold', fontSize=11, leading=15),
    'speech': ParagraphStyle('Speech', fontName='Actor', fontSize=16, leading=22),
    'small': ParagraphStyle('Small', fontName='Actor', fontSize=10, leading=14),
}


def norm(text):
    return ' '.join(unicodedata.normalize('NFC', text).split())


def put(c, y, text, style='body', after=8):
    p = Paragraph(escape(text), styles[style])
    _, height = p.wrap(CW, H)
    if y - height < 49:
        raise ValueError(f'Texto excedeu a página: {text[:80]}')
    p.drawOn(c, M, y - height)
    return y - height - after


def footer(c, n, title):
    c.setFont('Actor', 8.5)
    c.setFillColorRGB(.27, .31, .36)
    c.drawString(M, 28, title + '   23/10/2026')
    c.drawRightString(W - M, 28, str(n))
    c.setFillColorRGB(0, 0, 0)


def entry(c, y, r):
    y = put(c, y, r['code'] + '   ' + r['context'], 'label', 3)
    y = put(c, y, r['cue'], 'cue', 5)
    return put(c, y, r['fala'], 'speech', 17)


def prepare():
    rows = [r for r in json.loads(OLD.read_text(encoding='utf-8-sig')) if r['personagem'] in ('Edelzio', 'Edelzio criança')]
    assert len(rows) == 78
    original = PdfReader(INPUT)
    original_text = norm(' '.join(p.extract_text() for p in original.pages))
    missing = [r['id'] for r in rows if norm(r['fala']) not in original_text]
    if missing:
        raise ValueError(f'O PDF fornecido diverge do roteiro indexado: {missing}')

    # Indices in the supplied 78-entry script; the PDF is checked above.
    selection = [
        (17, 'ED01', 'Preparando o notebook', 'Natural, procurando um acesso à cadeira.'),
        (13, 'ED02', 'Olhando a caixa antes do sumiço', 'Cotidiano, com uma lembrança distante.'),
        (22, 'ED03', 'Deixando a chave na mesa', 'Natural, antes de buscar a mochila.'),
        (10, 'ED04', 'Voltando à mesa', 'Tranquilo, a chave ainda está no lugar.'),
        (18, 'ED05', 'Percebendo o sumiço', 'Confuso, observando o local.'),
        (20, 'ED06', 'Conferindo a porta', 'Baixo, raciocinando.'),
        (21, 'ED07', 'Encontrando o papel', 'Curioso, reconhecendo o traço azul.'),
        (12, 'ED08', 'Antes de abrir a caixa', 'Cauteloso, tentando entender.'),
        (14, 'ED09', 'Ouvindo os arranhões', 'Tensão crescente, sem gritar.'),
        (15, 'ED10', 'Depois de resolver a caixa', 'Contido. Leia a mensagem final com inquietação.'),
        (11, 'ED11', 'Com a chave recuperada', 'Firme, um pouco desconfiado.'),
        (16, 'ED12', 'Se tentar sair antes de investigar', 'Orientação calma.'),
        (40, 'ED13', 'Antes de usar o notebook do Renan', 'Natural, tom de conversa.'),
        (41, 'ED14', 'Depois de decifrar o horário', 'Leia 23:23 como vinte e três e vinte e três.'),
        (32, 'CR01', 'Voz de criança no rádio do Fusca', 'Baixo, com medo contido. Grave sem filtro de rádio.'),
    ]
    house_path = 'Assets/Scripts/Game/Varginha/Experiment/CampaignHouseSequence.cs'
    stage_path = 'Assets/Scripts/Game/Varginha/Experiment/VarginhaCampaignStage.cs'
    house = (ROOT / house_path).read_text(encoding='utf-8-sig')
    stage = (ROOT / stage_path).read_text(encoding='utf-8-sig')
    combined = house + '\n' + stage
    main = []
    for index, code, context, cue in selection:
        r = dict(rows[index - 1], code=code, context=context, cue=cue, input_index=index)
        assert r['id'] == line_id(r['personagem'], r['fala'])
        if code == 'CR01':
            assert 'CampaignMessagePresentation.IdFor("Edelzio criança","Não deixa ela sair.")' in house
            r['recording_status'] = 'Fala com gatilho de voz dedicado no Fusca'
            r['current_source'] = house_path
        elif code == 'ED03':
            assert 'BeginMissionShot("world","ANTES DE SAIR","' + r['fala'] + '")' in house
            r['recording_status'] = 'Texto já exibido na cutscene; precisa ligar reprodução de voz'
            r['current_source'] = house_path
        else:
            text = r['fala']
            if code in ('ED05', 'ED06', 'ED07'):
                suffix = ' A observação ficou registrada no caderno.'
                assert text.endswith(suffix)
                assert text[:-len(suffix)] in house
                assert 'line + "' + suffix + '"' in house
            else:
                assert text in combined
            r['recording_status'] = 'Fala atual no canal de diálogo'
            r['current_source'] = house_path if code in ('ED02', 'ED04', 'ED05', 'ED06', 'ED07', 'ED08', 'ED09', 'ED11') else stage_path
        main.append(r)

    # A failed regex extraction captured a conditional comparison operand as speech.
    rejected = rows[18]
    assert rejected['fala'] == 'exitclue A observação ficou registrada no caderno.'
    selected = {r['id'] for r in main}
    vocals = [dict(r, code=f'S{i:02}', context=ctx, cue=cue) for i, (r, ctx, cue) in enumerate(zip(
        [r for r in rows if r['estado'] == 'V'],
        ['Susto adulto', 'Queimadura adulta', 'Fuga adulta', 'Queda adulta', 'Susto criança'],
        ['Um susto curto, sem palavra.', 'Uma reação curta de dor, sem prolongar.', 'Três respirações curtas de esforço, com pausas.', 'Um impacto breve e recuperação do fôlego.', 'Uma reação curta ao clarão, sem palavras.'],
    ), 1)]
    reserve = [dict(r, code=f'REV{i:02}') for i, r in enumerate(
        [r for r in rows if r['estado'] == 'P' and r['id'] not in selected], 1)]
    assert len(main) == 15 and len(vocals) == 5 and len(reserve) == 57
    accounted = [r['id'] for r in main + vocals + reserve] + [rejected['id']]
    assert len(accounted) == len(set(accounted)) == 78
    assert set(accounted) == {r['id'] for r in rows}
    assert len({r['fala'] for r in main}) == 15
    return rows, main, vocals, reserve, rejected


def main_pdf(lines, vocals):
    c = canvas.Canvas(str(MAIN), pagesize=A4)
    c.setTitle('Falas do Edelzio para gravar')
    c.setAuthor('O Segredo de Varginha')
    groups = [lines[:4], lines[4:9], lines[9:14]]
    for page_no, group in enumerate(groups, 1):
        y = H - M
        if page_no == 1:
            y = put(c, y, 'Falas do Edelzio para gravar', 'title', 5)
            y = put(c, y, 'O Segredo de Varginha   23 de outubro de 2026', 'small', 12)
            y = put(c, y, 'São 14 falas do adulto e 1 da criança. Grave somente o texto maior. Os números e as orientações não são falados.', 'body', 6)
            y = put(c, y, 'Uma boa tomada por fala basta. Deixe 1 segundo de silêncio antes e depois. Voz limpa, sem música ou efeitos. Repita apenas se errar ou quiser outra versão.', 'body', 6)
            y = put(c, y, 'Nomeie pelo número: ED01_t01, ED02_t01 e assim por diante. Mande os arquivos originais juntos, de preferência em um ZIP.', 'body', 16)
        else:
            y = put(c, y, 'Edelzio adulto', 'title', 6)
            y = put(c, y, 'Leia só o texto maior. Uma fala por arquivo.', 'body', 18)
        for r in group:
            y = entry(c, y, r)
        footer(c, page_no, 'Edelzio para gravar')
        c.showPage()
    y = H - M
    y = put(c, y, 'Edelzio criança', 'title', 9)
    y = put(c, y, 'Grave separado do adulto, com voz mais jovem e receosa. O efeito de rádio será feito na edição.', 'body', 15)
    y = entry(c, y, lines[14])
    y = put(c, y, 'Sons de voz opcionais', 'section', 6)
    y = put(c, y, 'Faça apenas se forem pedidos. Não leia a descrição abaixo: interprete o som, sem palavras.', 'body', 15)
    for r in vocals:
        y = put(c, y, r['code'] + '   ' + r['context'], 'label', 2)
        y = put(c, y, r['cue'], 'body', 12)
    y = put(c, y, 'Antes de enviar', 'section', 6)
    y = put(c, y, 'Ouça se a voz está clara e sem cortes. Se o gravador oferecer WAV, use. Se não, mande o arquivo original na melhor qualidade, sem converter. Se precisar repetir: ED01_t02. Não precisa editar nem aplicar filtros.', 'body', 0)
    footer(c, 4, 'Edelzio para gravar')
    c.save()


def reserve_pdf(rows):
    c = canvas.Canvas(str(RESERVE), pagesize=A4)
    c.setTitle('Propostas de falas do Edelzio para revisão')
    page = 1
    y = H - M
    y = put(c, y, 'Propostas para revisão', 'title', 8)
    y = put(c, y, 'Não enviar este anexo como uma ordem de gravação', 'section', 8)
    y = put(c, y, 'Estas 57 entradas vieram do roteiro anterior. São propostas de voz, não falas já ligadas ao canal de diálogo. Algumas retomam textos visuais ou a mesma situação de falas atuais. A equipe deve escolher a versão e o momento antes de pedir os áudios.', 'body', 17)
    previous = None
    for r in rows:
        scene = re.sub(r'^A\d+\s+', '', r['cena'])
        text = f"{r['code']}   {r['personagem']}"
        height = 0
        for value, style in [(text, 'label'), (r['direcao'], 'cue'), (r['fala'], 'speech')]:
            p = Paragraph(escape(value), styles[style])
            height += p.wrap(CW, H)[1] + 8
        if scene != previous:
            height += 37
        if y - height < 65:
            footer(c, page, 'Edelzio propostas para revisão')
            c.showPage()
            page += 1
            y = H - M
            y = put(c, y, 'Propostas para revisão', 'section', 6)
            y = put(c, y, 'Aguardar escolha de texto e momento antes de gravar.', 'small', 15)
            previous = None
        if scene != previous:
            y = put(c, y, scene, 'section', 10)
            previous = scene
        y = put(c, y, text, 'label', 3)
        y = put(c, y, r['direcao'], 'cue', 5)
        y = put(c, y, r['fala'], 'speech', 17)
    footer(c, page, 'Edelzio propostas para revisão')
    c.save()
    return page


def artifacts(rows, lines, vocals, reserve, rejected):
    OUT.mkdir(parents=True, exist_ok=True)
    PDFOUT.mkdir(parents=True, exist_ok=True)
    QA.mkdir(parents=True, exist_ok=True)
    main_pdf(lines, vocals)
    reserve_pages = reserve_pdf(reserve)
    text = [
        'FALAS DO EDELZIO PARA GRAVAR', 'O Segredo de Varginha - 23/10/2026', '',
        '14 falas do adulto e 1 da criança. Leia só a fala. Não leia o número nem a orientação.',
        'Uma boa tomada por fala. Deixe 1 segundo de silêncio antes e depois. Sem música ou efeitos.',
        'Nomeie: ED01_t01, ED02_t01... Se repetir: ED01_t02. Mande os arquivos originais juntos, de preferência em ZIP.', '',
    ]
    for r in lines:
        text += [r['code'] + ' - ' + r['context'], '(' + r['cue'] + ')', r['fala'], '']
    text += ['SONS OPCIONAIS - SOMENTE SE PEDIDOS', 'Interprete o som. Não leia a descrição.', '']
    for r in vocals:
        text += [r['code'] + ' - ' + r['context'] + ': ' + r['cue'], '']
    text += ['ENVIO', 'Se puder, grave em WAV. Caso contrário, envie o arquivo original na melhor qualidade. Não precisa converter, editar nem aplicar filtros.', '']
    TXT.write_text('\n'.join(text), encoding='utf-8-sig')
    (OUT / 'mapa_de_audios.json').write_text(json.dumps({'date': '2026-10-23', 'main': lines, 'optional_vocals': vocals, 'review_only': reserve, 'rejected': [dict(rejected, reason='Identificador da comparação condicional capturado pelo extrator, não uma fala.')], 'runtime_edited': False}, ensure_ascii=False, indent=2), encoding='utf-8')
    with (OUT / 'MAPA_TECNICO_AUDIOS.csv').open('w', encoding='utf-8-sig', newline='') as f:
        writer = csv.DictWriter(f, fieldnames=['codigo', 'personagem', 'arquivo_recebido', 'id_do_jogo', 'arquivo_final', 'situacao', 'texto'])
        writer.writeheader()
        for r in lines + vocals + reserve:
            writer.writerow({'codigo': r['code'], 'personagem': r['personagem'], 'arquivo_recebido': r['code'] + '_t01.ext', 'id_do_jogo': r['id'], 'arquivo_final': r['id'] + '.wav', 'situacao': r.get('recording_status', 'Esforço opcional, sem gatilho garantido' if r['estado'] == 'V' else 'Proposta, revisar e ligar à cena antes de usar'), 'texto': r['fala']})
    review = f'''# Revisão do roteiro do Edelzio em 23 de outubro de 2026

O PDF de Downloads é idêntico ao PDF de 22/10 mantido no projeto. As 78 entradas foram conferidas contra o texto extraído das dez páginas e indexadas individualmente. A história geral acompanha a campanha atual, mas o documento não é uma lista de 78 falas já prontas para integração.

## Problemas corrigidos na entrega para o ator

- O roteiro antigo mistura 15 entradas marcadas E, 58 marcadas P e cinco esforços vocais. Uma entrada E é inválida: `exitclue A observação ficou registrada no caderno.`. O extrator incluiu a string de uma comparação condicional como se fosse uma alternativa de fala. Essa entrada foi excluída da gravação, sem substituir a fala válida da porta.
- As 13 falas adultas com canal de diálogo atual e a frase infantil do rádio foram preservadas literalmente, incluindo pontuação, caixa e os complementos que participam do hash do áudio.
- A antiga proposta `Vou deixar a chave na mesa do café. Depois pego a mochila.` já consta da cutscene `ANTES DE SAIR`. Ela foi colocada no roteiro principal como ED03. O texto existe, mas `CampaignTimedSubtitle` usa owner nulo e não liga sua voz automaticamente. O mapa técnico registra essa pendência, sem declarar integração concluída.
- As outras 57 entradas de propostas foram preservadas em um anexo separado para revisão. Algumas reproduzem ou dramatizam textos já visíveis em leituras. Isso não as transforma em chamadas de voz atribuídas ao personagem. Não pedir ao ator que decida entre as versões equivalentes.
- Os cinco esforços de voz ficam opcionais e separados das falas. O efeito `EdelzioFear` recebido em 22/10 não substitui dublagem textual, mas deve ser ouvido antes de pedir esforços redundantes.
- O ator recebe números curtos, uma orientação breve e o texto em destaque. O CSV mantém a correspondência com os IDs originais. Uma tomada boa basta; gravações extras são opcionais.

## Continuidade verificada

A chave usa a mesa do café e os fragmentos mantêm as bifurcações aprovadas. O horário continua 23:23. A frase infantil `Não deixa ela sair.` pertence ao rádio do Fusca e à lembrança correspondente, sem antecipação na abertura. Os anos de referência são 1898, 1996 e 2026. O roteiro de propostas mantém a travessia antes de encerrar o selo; não transforma a criatura ferida em alvo de extermínio nem acrescenta finais não implementados. Os documentos GDD/LDD têm divergências já registradas, portanto a implementação atual orientou esta conferência.

## Entrega

- Principal: `{MAIN.relative_to(ROOT).as_posix()}`. Quatro páginas, 14 falas adultas e uma infantil, com cinco esforços opcionais na última página.
- Texto para celular: `{TXT.relative_to(ROOT).as_posix()}`.
- Pacote para enviar ao ator: `output/dublagem-2026-10-23/PARA_ENVIAR_AO_EDELZIO_23-10-2026.zip`, contendo somente o PDF principal e o TXT.
- Anexo de revisão da equipe: `{RESERVE.relative_to(ROOT).as_posix()}`. {reserve_pages} páginas com as 57 propostas preservadas.
- Mapa de integração: `MAPA_TECNICO_AUDIOS.csv` e `mapa_de_audios.json` nesta pasta. Não enviar os códigos longos ao ator como instrução de gravação.

## Fontes e limites

Fontes: PDF fornecido; `Docs/CampanhaOficial/Dublagem_2026-10-22/roteiro_gravacao.json`; `Assets/Scripts/Game/Varginha/Experiment/CampaignHouseSequence.cs`; `VarginhaCampaignStage.cs`; `CampaignFuscaCinema.cs`; `CampaignTimedSubtitle.cs`; `CampaignMessagePresentation.cs`; `CampaignContinuationController.cs`; `CampaignContinuationDefinition.cs`; GDD/LDD em `Docs/CampanhaOficial/Referencias/`; `PUZZLE_FRAGMENTOS_BIFURCACOES_2026-10-21.md`; créditos de áudio de 22/10.

O prefixo `Edelzio:` no texto de sucesso da criatura ferida aparece em mensagem de leitura da investigação, não numa chamada Speak. O epílogo também mistura narração e frase do personagem em leitura. Essas propostas não foram anunciadas como áudio automaticamente utilizável. ED12 e ED01 soam como orientação, mas o código atual as atribui a Edelzio no canal de diálogo; foram preservadas para evitar mudar o jogo nesta tarefa.

Nenhuma fala, mecânica, arte ou cena do jogo foi alterada. O PDF original em Downloads foi preservado. O extrator geral e os roteiros completos de 22/10 também foram preservados; esta revisão documenta o erro e gera uma seleção rastreável sem reescrever a documentação do elenco inteiro.
'''
    (OUT / 'REVISAO_ROTEIRO_EDELZIO.md').write_text(review, encoding='utf-8')
    with zipfile.ZipFile(OUT / 'PARA_ENVIAR_AO_EDELZIO_23-10-2026.zip', 'w', zipfile.ZIP_DEFLATED) as z:
        z.write(MAIN, MAIN.name)
        z.write(TXT, TXT.name)
    return reserve_pages


def verify(lines, reserve, reserve_pages):
    main_doc = PdfReader(MAIN)
    reserve_doc = PdfReader(RESERVE)
    main_text = norm(' '.join(p.extract_text() for p in main_doc.pages))
    reserve_text = norm(' '.join(p.extract_text() for p in reserve_doc.pages))
    checks = {
        'main_pages': len(main_doc.pages) == 4,
        'reserve_pages': len(reserve_doc.pages) == reserve_pages,
        'all_main_lines': all(norm(r['fala']) in main_text for r in lines),
        'all_review_lines': all(norm(r['fala']) in reserve_text for r in reserve),
        'all_short_codes': all(len(re.findall(r'\b' + r['code'] + r'\b', main_text)) == 1 for r in lines),
        'no_parser_garbage': 'exitclue' not in main_text and 'exitclue' not in reserve_text,
        'no_technical_ids_for_actor': 'fala_' not in main_text and 'Assets/' not in main_text,
        'no_replacement_glyphs': all('\ufffd' not in p.extract_text() and '\u25a0' not in p.extract_text() for p in list(main_doc.pages) + list(reserve_doc.pages)),
        'exact_ids_preserved': all(r['id'] == line_id(r['personagem'], r['fala']) for r in lines + reserve),
        'no_blank_pages': all(len(norm(p.extract_text())) > 90 for p in list(main_doc.pages) + list(reserve_doc.pages)),
    }
    with zipfile.ZipFile(OUT / 'PARA_ENVIAR_AO_EDELZIO_23-10-2026.zip') as z:
        checks['zip_integrity'] = z.testzip() is None and set(z.namelist()) == {MAIN.name, TXT.name}
    report = {'success': all(checks.values()), 'checks': checks, 'original_entries': 78, 'main_lines': 15, 'optional_vocals': 5, 'review_lines': 57, 'rejected_entries': 1, 'main_pages': 4, 'reserve_pages': reserve_pages, 'input_sha256': hashlib.sha256(INPUT.read_bytes()).hexdigest(), 'main_sha256': hashlib.sha256(MAIN.read_bytes()).hexdigest(), 'game_tests_run': False, 'failures': [k for k, v in checks.items() if not v]}
    (OUT / 'qa-conteudo.json').write_text(json.dumps(report, ensure_ascii=False, indent=2), encoding='utf-8')
    print(json.dumps(report, ensure_ascii=False))
    if not report['success']:
        raise SystemExit(1)


if __name__ == '__main__':
    rows, lines, vocals, reserve, rejected = prepare()
    pages = artifacts(rows, lines, vocals, reserve, rejected)
    verify(lines, reserve, pages)
