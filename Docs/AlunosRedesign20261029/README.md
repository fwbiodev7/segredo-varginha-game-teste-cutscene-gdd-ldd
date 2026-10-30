# Alunos aliados — redesign de 29/10/2026

Base: commit `412cd5f` da branch `main`, restaurado do repositório oficial porque a pasta local estava ausente.

## Comportamento

A mochila mostra os 12 alunos em uma grade de três colunas por quatro linhas. Na batalha final é possível equipar até três alunos, remover uma escolha ou substituir a última vaga ao escolher um quarto aluno. Renan, Ouzana e Padre Fábio continuam na aba de apoio. Fora da batalha final, os alunos podem ser inspecionados, mas não equipados, independentemente da dificuldade.

Os três novos alunos são Giovana (índice 9), Bianca (10) e Moscardini (11). Os índices anteriores 0–8 foram preservados. Anna Sabiá conserva o ID histórico `Anna Sabia` / `AnnaSabia` para manter diálogos, poderes e saves. A rotina que valida os saves foi ampliada para aceitar os novos índices sem convertê-los em alunos antigos.

Giovana, Bianca e Moscardini usam o golpe de apoio já existente no sistema; não foram inventadas novas habilidades pessoais. Os poderes dos alunos antigos e seus efeitos continuam ativos. A figura do aluno durante os golpes usa o novo atlas de uniforme, evitando retornar ao figurino anterior.

## Artes e referências

As três imagens enviadas são a autoridade visual. O trabalho de arte foi dividido entre três subagentes. Os nove personagens existentes foram extraídos como figuras inteiras da folha JPEG, sem redesenho. Seus PNGs fonte e runtime foram substituídos, conservando GUIDs. A fonte original e as versões anteriores foram preservadas em `ArtSources/Students20261029/`.

As caminhadas novas de Giovana, Bianca e Moscardini foram reconstruídas com a ferramenta integrada ImageGen, usando as referências visuais. Giovana e Bianca usam Marcos como referência de pele. Isso preserva a identidade visual, mas não é reprodução pixel a pixel dos JPEGs nem igualdade numérica de todos os tons de pele. Prompts, fontes, recortes e limitações detalhadas:

- `ArtSources/Students20261029/REVIEW.md` — nove alunos e extração técnica.
- `Assets/ArtSource/Allies/Redesign20261029/Giovana/README.md` — Giovana, prompts e auditoria.
- `Assets/ArtSource/Allies/Redesign20261029/Bianca/` e `Moscardini/` — fontes e manifestos dos dois novos alunos.
- `Preview/Students/Redesign20261029/Elenco_12.png` — contato geral do elenco.
- `Preview/AlliesRedesign20261029/` — contatos e ciclos animados dos três novos.

## Contrato de integração

Todos os atlas usados pelo jogo ficam em `Assets/Resources/Varginha/Allies/`: PNG RGBA de 256×256, 16 células de 64×64, linhas frente/sul, esquerda/oeste, direita/leste e costas/norte, quatro quadros por direção. Pés com baseline Unity y=6, filtro Point, sem mipmaps, leitura habilitada. Retratos, caminhada e apresentação dos golpes compartilham o desenho novo. As fontes canônicas ficam na raiz `Assets/ArtSource/Allies/`; fontes brutas em subpastas não são reexportadas automaticamente.

Fábio agora prioriza o atlas novo do catálogo, com fallback para a arte antiga somente se o recurso estiver ausente. Os nove alunos da escola e o resgate permanecem com a mesma composição de elenco; os três novos entram na seleção da luta final.

## Validação

Resultados dos testes e inspeção final serão registrados em `VALIDACAO.md`. A auditoria estrutural de imagens não equivale a aprovação artística humana.
