# Retratos individuais — Giovana, Pedro, Yasmin, Luis Martins

Quatro bustos individuais em pixel art mais detalhada, feitos com uma chamada ImageGen por personagem. As referências atuais de corpo inteiro foram inspecionadas e enviadas de fato à ferramenta junto às referências originais. Em Pedro, uma nova edição incorporou a referência prioritária da touca e os óculos redondos; o retrato antigo sem touca fica apenas como candidata histórica.

Cada subpasta contém a fonte bruta, prompt final exato, referência de sprite inteiro, paleta e `portrait.json` com hashes, dimensões e recorte técnico. Não houve recoloração ou criação de desenho por código. O processamento técnico reduziu o busto inteiro com nearest-neighbor, posicionou em canvas 256 × 256 e removeu ilhas desconectadas da transparência. O corte horizontal inferior remove apenas a franja defeituosa de alpha do recorte de busto gerado. Cabeça, cabelo, ombros e touca estão completos.

PNG finais RGBA: `Assets/Resources/Varginha/Allies/Portraits/{ID}.png`. Metas usam Point, mipmaps desligados, Default legível e compressão desativada. Não há painéis, molduras ou fundos incorporados: alpha real confirmado com extremos 0 e 255.

Contato dos quatro visto: `Preview/StudentsCrisp20261029/GroupGiovana/portraits-contact-4.png`. Foram conferidos pele, cabelo, uniforme, presilha de Giovana e touca/óculos de Pedro. Detalhes de cabelo/rosto e tecido estão mais legíveis que os recortes dos atlas de caminhada. Não foram acrescentados acessórios a Yasmin ou Luis Martins. A integração da seleção da mochila e o teste em runtime ficam com o agente principal; nenhum código gameplay foi alterado por este subagente.
