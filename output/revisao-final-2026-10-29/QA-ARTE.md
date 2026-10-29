# QA visual de arte — 29/10/2026

Escopo: inspeção das capturas existentes. Nenhuma arte gerada, nenhum asset/cena/script alterado, nenhum teste/build executado por este agente.

## forest-intact-church.png

- Porta frontal central reconhecível, cruz e vitrais preservam a identidade de igreja. Paleta fria coerente com a mata; luzes da porta ajudam a orientar a entrada.
- Ponte de madeira permanece visível e não foi coberta pela fachada.
- Não aparecem o sino nem o altar antigos no centro visível.
- Comparação posterior com Forest.png confirmou que o objeto arredondado à direita (aproximadamente x680,y440 na captura960×540) é o toco original perto de [1507,403], com anéis de madeira; deve ser preservado. Não é lápide nem resíduo do altar.
- **Limite desta captura forçada:** telhado cortado na borda superior e patamar da entrada cortado na inferior; faixa preta à direita a partir de aproximadamente x720. O root informou que a câmera de captura será corrigida e que o gameplay usa câmera limitada ao mapa; este quadro isolado não permite atribuir os cortes ao gameplay nem confirmar a escala de30% em relação ao jogador.
- Nenhum personagem ou vestígio sign0/1/2 visível neste enquadramento. Legibilidade das pistas e passagem física não avaliáveis nesta imagem.

## church-priest-original.png

Padre e protagonista visíveis, com contornos legíveis contra o piso. O padre tem acabamento mais simples e menos definido que o protagonista, mas esta captura isolada não comprova regressão ou mudança do original. Sem evidência visual de interferência com bancos/órgão neste quadro.

## Casarão/porão — seis capturas iniciais, antes do ajuste final

Inspeção visual dos seis PNGs solicitados,1920×1080. Todos os objetos-alvo são reconhecíveis, estão inteiros e não têm escala absurda em relação ao personagem/móveis. Há oclusões normais pelo protagonista em algumas posições; foi possível conferir os mesmos objetos desobstruídos nas outras capturas.

| Captura | Leitura e apoio |
| --- | --- |
| casarao-plan-visible.png | Planta identificável como documentos/papéis no banco, escala discreta. A posição parece sobre o encosto, acima da faixa do assento; deslocar para baixo até o assento, compactando se necessário, ajudaria a leitura de apoio. |
| casarao-photo-visible.png | Foto sépia com rostos legíveis sobre a escrivaninha; encaixe e escala coerentes. Neste quadro a chave também aparece desobstruída na cômoda. |
| casarao-key-visible.png | Chave reconhecível nas outras capturas, parcialmente encoberta pelo jogador nesta. O problema concreto é o apoio: em casarao-photo-visible ocupa aproximadamente x1627–1692,y370–424, sobre a frente das gavetas, abaixo do tampo y345–370. Ajustar posição/escala para caber no tampo, ou tornar explícito um gancho se a intenção for pendurá-la. |
| basement-pages-visible.png | Papéis/arquivo legíveis na mesa coberta; cores e tamanho coerentes. Duas folhas inferiores avançam para a face vertical do pano, parecendo coladas/penduradas. Compactar/subir esse grupo para o plano superior deixaria o apoio mais natural. |
| basement-photos-visible.png | Fotografia/documentos coerentes sobre o piso de pedra. Foto parcialmente coberta pelo personagem neste quadro, mas completamente visível em basement-pages-visible e basement-materials-visible. |
| basement-materials-visible.png | Materiais com fios/instrumentos identificáveis no piso junto à caixa, escala aceitável. Protagonista os encobre em parte nesta captura; aparecem inteiros e com apoio no chão em basement-pages-visible. |

As três observações de apoio acima são refinamentos de posicionamento, não evidência de perda/corte do sprite. Nenhuma alteração de arte/jogo feita por este agente. Não houve espera pela nova captura da igreja, conforme orientação do root.

## Rechecagem final de apoios — 08:12–08:13

Esta seção substitui as observações iniciais de apoio da planta, chave e páginas. Somente os três PNGs renovados foram inspecionados nesta rodada, sem alteração do jogo/código.

- **casarao-key-visible.png,08:13:07:** chave reconhecível, inteira e agora posicionada sobre o tampo da cômoda, junto aos livros/caixa. Não aparece mais aplicada na frente das gavetas; observação de apoio resolvida.
- **basement-pages-visible.png,08:13:21:** grupo de páginas deslocado para cima, apoiado de forma mais natural no plano superior da mesa coberta. Escala e leitura coerentes, sem corte; pequena proximidade à borda frontal não produz a antiga aparência de folhas coladas na face vertical. Observação de apoio resolvida.
- **casarao-plan-visible.png,08:12:54:** o ator encobre a região central do assento onde foi posicionada a planta. A posição informada [430,690] com worldSize[0.32,0.26] corresponde à proposta de apoio no assento, mas a nova captura não comprova a silhueta/legibilidade desobstruída da planta porque o personagem está exatamente à frente. Trata-se de limite de evidência visual por oclusão, sem sinal de sprite cortado ou defeito comprovado. Não foi pedido novo teste/build por este agente.

## Limites remanescentes

Visão ampliada da igreja com personagem/pistas ainda não inspecionada. Colisões, transição de porta e interação pertencem ao QA funcional do root.

