# QA visual — cabine, carro e quadros

Conferência somente por capturas; nenhum código, asset, cena, imagem ou Unity alterado/executado.

## Conferência inicial — 29/10/2026, aproximadamente 07:24

`captures/fusca-01.png` a `fusca-07.png`, `school-passing-music-car.png` e `school-exit-car.png`.

- Cabine: nos quadros limpos 01 e 06, o motorista está sentado diante do volante; os dois antebraços terminam em mãos em contato com o aro, sem mão solta ou deslocamento evidente entre personagem e volante. O quadro 05 usa um insert separado de mão/chave na ignição, com base da cabine alinhada. Os quadros 02–04 contêm interferência gráfica intensa e não servem para aprovar detalhes finos das mãos; a sequência de posições preserva o mesmo eixo do motorista.
- Carro com música: aparece na faixa inferior de asfalto, separado do Fusca estacionado e do protagonista na calçada/saída do estacionamento. A captura isolada não comprova toda a rota nem áudio/Doppler.
- **Problema histórico, agora resolvido:** na versão dessas capturas das 07:24, `school-passing-music-car.png` mostrava a parte inferior do veículo cortada pela borda da ilustração (aproximadamente y=825 na captura 1920×1080); `school-exit-car.png` mostrava o carro sobre a faixa preta lateral esquerda (aproximadamente x=15–119; cenário começa x=120). Os mesmos nomes de arquivo foram posteriormente atualizados; a observação fica preservada aqui para rastrear a correção.

## Nova conferência — 29/10/2026, 08:02, horário de Brasília

Capturas atualizadas na mesma pasta `output/revisao-final-2026-10-29/captures/`:

| Arquivo | Gravação | Resultado visual |
| --- | --- | --- |
| `school-exit-car.png` | 08:02:17 | Só a frente do carro entra pela borda esquerda do cenário, com recorte correto em x=120; nenhum pixel invade a margem preta. Protagonista e Fusca permanecem separados do veículo. |
| `school-passing-music-car.png` | 08:02:19 | Veículo inteiro visível sobre o asfalto, incluindo rodas e base, sem corte inferior. Proporção horizontal coerente e passagem separada do Fusca estacionado e do protagonista na calçada. |
| `school-arrival-gate.png` | 08:02:22 | Protagonista na passagem/calçada da escola; Fusca preservado na vaga, sem sobreposição com carro em movimento. |

**Resultado:** corte inferior e invasão da margem preta observados às 07:24 resolvidos nas capturas atualizadas das 08:02. Essa aprovação é visual dos estados capturados; não comprova áudio espacial, Doppler ou a totalidade da trajetória entre os quadros. Apenas este relatório foi atualizado nesta nova conferência.

## Evidências antigas — 28/10/2026

Os arquivos não estavam na pasta final; foram inspecionados em `output/revisao-completa-2026-10-28/captures/`.

- `03_Casarao_Causas.png`: quatro provas e quatro destinos preenchidos, vínculos 4/4. Textos e linhas de vínculo inteiros, sem clipping ou sobreposição com DESFAZER e ações inferiores.
- `04_Tombo_Datas.png`: três provas/destinos preenchidos, vínculos 3/3. Anos, nomes e linhas de vínculo cabem nos cartões; ações ficam dentro do painel. O cinza é discreto, mas legível nas capturas.

Esses dois quadros aprovam somente a diagramação exibida na evidência antiga; não comprovam ausência de regressão na build final atual. Não foi refeito QA do menu já aprovado. As imagens atuais exibem Development Build.
