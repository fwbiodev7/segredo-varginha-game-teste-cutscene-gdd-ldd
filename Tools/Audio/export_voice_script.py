"""Export exact, attributable speech. Dynamic expressions are expanded or reported, never guessed."""
import csv
import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / 'Assets/Scripts/Game/Varginha'
OUT = ROOT / 'Docs/CampanhaOficial/Dublagem_2026-10-22'
STRING = r'"(?:\\.|[^"\\])*"'


def decode(token):
    return json.loads(token)


def line_id(speaker, text):
    value = 14695981039346656037
    for byte in (speaker + '\n' + text.replace('\r\n', '\n')).encode('utf-8'):
        value = ((value ^ byte) * 1099511628211) & 0xffffffffffffffff
    return f'fala_{value:016x}'


def split_args(source, start):
    args, current, depth, quoted, escaped = [], '', 1, False, False
    for char in source[start:]:
        if quoted:
            current += char
            if escaped:
                escaped = False
            elif char == '\\':
                escaped = True
            elif char == '"':
                quoted = False
        elif char == '"':
            quoted = True
            current += char
        elif char in '([{':
            depth += 1
            current += char
        elif char in ')]}':
            depth -= 1
            if depth == 0:
                args.append(current.strip())
                return args
            current += char
        elif char == ',' and depth == 1:
            args.append(current.strip())
            current = ''
        else:
            current += char
    return []


def exact_candidates(expression):
    tokens = re.findall(STRING, expression)
    if re.fullmatch(STRING, expression.strip()):
        return [decode(tokens[0])]
    if tokens and re.fullmatch(r'\s*TEXT(?:\s*\+\s*TEXT)*\s*', re.sub(STRING,'TEXT',expression)):
        return [''.join(decode(t) for t in tokens)]
    # Conditional branches made only of full literals. Concatenations are not full lines.
    if '?' in expression and '+' not in re.sub(STRING, '', expression):
        branches = re.sub(STRING, 'TEXT', expression)
        if re.search(r'\?\s*TEXT\s*:\s*(?:TEXT|[^?:]+\?\s*TEXT)', branches):
            return [decode(t) for t in tokens]
    return []


def collect():
    rows, unresolved = [], []

    def add(speaker, text, path, line, scope='CAMPANHA ATUAL', note=''):
        if not text.strip():
            return
        rows.append(dict(id=line_id(speaker, text), personagem=speaker, texto=text, origem=f'{path}:{line}', escopo=scope, observacao=note, estado='TEXTO EXISTENTE'))

    for path in sorted(SOURCE.rglob('*.cs')):
        source = path.read_text(encoding='utf-8-sig')
        for match in re.finditer(r'\b(Say|Speak|ShowDialogue|ShowCarDialogue|ShowRodrigoHint|Cutscene)\s*\(', source):
            name = match.group(1)
            args = split_args(source, match.end())
            speaker = None
            if name == 'ShowRodrigoHint' and len(args) == 1 and re.fullmatch(STRING, args[0]):
                text = decode(args[0]);text = re.sub(r"^Rodrigo:\s*'?", '', text).rstrip("'")
                add('Rodrigo', text, path.relative_to(ROOT).as_posix(), source.count('\n', 0, match.start()) + 1, 'INTERFACE LEGADA', 'Guia externo ao mundo do jogo')
                continue
            if name == 'Cutscene' and len(args) == 4 and re.fullmatch(STRING, args[3]):
                speaker, expression = decode(args[3]), args[1]
            elif name in ('Say', 'Speak', 'ShowDialogue', 'ShowCarDialogue') and len(args) == 2 and re.fullmatch(STRING, args[0]):
                speaker, expression = decode(args[0]), args[1]
            else:
                continue
            if speaker in ('NOTEBOOK', 'Livro do Tombo', 'Fusca Trancado', 'Investigação', 'Pista Histórica', 'Livro do Tombo Secreto'):
                continue
            relative = path.relative_to(ROOT).as_posix()
            line = source.count('\n', 0, match.start()) + 1
            candidates = exact_candidates(expression)
            if candidates:
                scope = 'CAMPANHA ATUAL' if '/Experiment/' in relative else 'FLUXO LEGADO'
                for text in candidates:
                    add(speaker, text, relative, line, scope)
            elif not expression.startswith('string '):
                unresolved.append(dict(personagem=speaker, expressao=expression, origem=f'{relative}:{line}'))

    stage = (SOURCE/'Experiment/VarginhaCampaignStage.cs').read_text(encoding='utf-8-sig')
    names_source = (SOURCE/'VarginhaPhase2Controller.cs').read_text(encoding='utf-8-sig')
    def array(text, marker):
        body = text.split(marker, 1)[1].split('{', 1)[1].split('}', 1)[0]
        return [decode(t) for t in re.findall(STRING, body)]
    names = array(names_source, 'string[] StudentNames')
    activities = array(stage, 'string[] activities')
    first = array(stage, 'string[] first')
    for name, activity, initial in zip(names, activities, first):
        add(name, 'Estou ' + activity + '. ' + initial, 'Assets/Scripts/Game/Varginha/Experiment/VarginhaCampaignStage.cs', stage[:stage.index('string[] first')].count('\n')+1, note='Conversa opcional na Industrial')
        add(name, '23:23... Guarde a foto original. Precisamos comparar se ela mudar de novo.', 'Assets/Scripts/Game/Varginha/Experiment/VarginhaCampaignStage.cs', stage[:stage.index('Say(student.StudentName')].count('\n')+1, note='Somente depois de decifrar o horário')
    house = (SOURCE/'Experiment/CampaignHouseSequence.cs').read_text(encoding='utf-8-sig')
    for match in re.finditer(r'ObserveKeyClue\(', house):
        args = split_args(house, match.end())
        if len(args) == 2:
            for text in exact_candidates(args[1]):
                add('Edelzio', text + ' A observação ficou registrada no caderno.', 'Assets/Scripts/Game/Varginha/Experiment/CampaignHouseSequence.cs', house[:match.start()].count('\n')+1)
    definition = (SOURCE/'Experiment/CampaignContinuationDefinition.cs').read_text(encoding='utf-8-sig')
    for token in re.findall(STRING, definition):
        text = decode(token)
        if text.startswith('Renan: '):
            add('Renan', text[len('Renan: '):], 'Assets/Scripts/Game/Varginha/Experiment/CampaignContinuationDefinition.cs', definition[:definition.index(token)].count('\n')+1)
    # Resolve the serialized legacy priest message from its actual runtime configuration.
    legacy=(SOURCE/'VarginhaPhase3Controller.cs').read_text(encoding='utf-8-sig')
    match=re.search(r'PropType\.PadreFabio,\s*"Padre Fábio",\s*('+STRING+')',legacy)
    if match:add('Padre Fábio',decode(match.group(1)),'Assets/Scripts/Game/Varginha/VarginhaPhase3Controller.cs',legacy[:match.start()].count('\n')+1,'FLUXO LEGADO')
    # The child radio refrain is a canonical GDD line, present as a caption while recording is pending.
    gdd=(ROOT/'Docs/CampanhaOficial/Referencias/GDD_Usuario.md').read_text(encoding='utf-8-sig')
    child_line=gdd[:gdd.index('Não deixa ela sair.')].count('\n')+1
    add('Edelzio criança', 'Não deixa ela sair.', 'Docs/CampanhaOficial/Referencias/GDD_Usuario.md', child_line, 'GDD E CUTSCENE ATUAL', 'Gravar voz seca. Interferência aplicada depois; nunca antecipar ao prólogo.')

    # Keep all uses of duplicate lines without requesting duplicate recordings.
    by_id = {}
    for row in rows:
        if row['id'] in by_id:
            prior = by_id[row['id']]
            if row['origem'] not in prior['origem']:
                prior['origem'] += '; ' + row['origem']
        else:
            by_id[row['id']] = row
    resolved=('line + ','_definition.intro.Substring','message')
    unresolved=[r for r in unresolved if not any(r['expressao'].startswith(p) for p in resolved)]
    return list(by_id.values()), unresolved


def main():
    OUT.mkdir(parents=True, exist_ok=True)
    rows, unresolved = collect()
    with (OUT/'FALAS_EXISTENTES.csv').open('w', encoding='utf-8-sig', newline='') as file:
        writer=csv.DictWriter(file,fieldnames=list(rows[0]));writer.writeheader();writer.writerows(rows)
    (OUT/'falas_existentes.json').write_text(json.dumps(rows,ensure_ascii=False,indent=2),encoding='utf-8')
    (OUT/'expressoes_dinamicas.json').write_text(json.dumps(unresolved,ensure_ascii=False,indent=2),encoding='utf-8')
    print(json.dumps(dict(falas=len(rows),personagens=sorted({r['personagem'] for r in rows}),expressoes_para_conferir=len(unresolved)),ensure_ascii=False))


if __name__ == '__main__':
    main()
