# Pesquisa para as transições

Consultadas em 20/10/2026:

- [MDN: Using the Web Animations API](https://developer.mozilla.org/en-US/docs/Web/API/Web_Animations_API/Using_the_Web_Animations_API): animações de elementos, duração, easing, cancelamento e ciclo de vida.
- [MDN: clip-path](https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/Properties/clip-path): máscaras geométricas para revelar slides em círculo, diagonal e losango.
- [MDN: transform](https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/Properties/transform): deslocamentos, escala e perspectiva para portas, persianas, CRT e filme.

As nove transições foram implementadas no próprio HTML com essas APIs. As imagens continuam sendo capturas e arte originais do projeto. Efeitos podem ser desligados com M e respeitam a preferência de movimento reduzido.

# Pesquisa para a economia de saída

- [Distill: Where the token savings come from](https://github.com/samuelfaj/distill/blob/main/docs/token-saver.md): filtros determinísticos antes do modelo utilitário, proteção da saída exata e guarda do original antes de resumir.
- [Distill atual](https://github.com/samuelfaj/distill): agente completo, com papel separado para modelo utilitário.

A adaptação deste repositório usa extração local de resultados e preserva stdout, stderr e código de saída. A nota produzida por um modelo local é opcional. Nenhum percentual de economia de tokens foi presumido.
