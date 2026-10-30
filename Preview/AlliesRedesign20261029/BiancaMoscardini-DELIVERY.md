# Bianca e Moscardini — integração artística de 29/10/2026

Estado: `visual-reviewed`, sem aprovação humana automática. Assets gravados nos caminhos consumidos pelo jogo; importação e testes Unity são responsabilidade da integração principal.

## Entregas e contrato

- `Assets/Resources/Varginha/Allies/Bianca.png` e `Moscardini.png`: PNG RGBA 256 × 256, 16 quadros completos, células 64 × 64, linhas S/W/E/N, quatro desenhos distintos de caminhada por direção.
- `Assets/ArtSource/Allies/Bianca.png` e `Moscardini.png`: cópia inteira ampliada exatamente 2× em nearest-neighbor, 512 × 512, compatível com o reexportador existente.
- Fontes originais geradas, referências completas, hashes, prompts, manifestos e paletas em `Assets/ArtSource/Allies/Redesign20261029/{Bianca,Moscardini}/`.
- Pivô runtime central `(0.5,0.5)`; PPU `64 / 1.35`, definido em `VarginhaStudentSprites`, não pelo PPU do importador de textura. Altura máxima 50 pixels; apoio inferior exclusivo y58 na origem superior, equivalente Unity yMin6. Timings declarados de 100 ms por quadro para a prévia; o consumidor continua controlando seu próprio tempo.

## Processo visual

Foi usado o gerador integrado `image_gen`, sem CLI/API externa. Dois lotes de oito sprites por personagem; Bianca recebeu correções de pele e uma edição final do atlas inteiro baseada no atlas runtime real de Marcos. As versões anteriores de Bianca foram preservadas para rastreabilidade. O prompt final está em `Bianca/final-skin-match-prompt.json`.

Bianca preserva cabelo castanho escuro volumoso e cacheado, franja, brincos pequenos dourados, uniforme branco/verde, calça longa escura e tênis preto/branco. Moscardini preserva cabelo castanho volumoso com mechas e pontas, uniforme branco/verde, calça longa escura e tênis preto/branco. Os títulos direita/esquerda da referência estavam invertidos: a ordenação usa a orientação visual real dos desenhos.

A edição final de Bianca foi comparada lado a lado com Marcos e Giovana em `Bianca/skin-comparison-final.png`: o tom atual é caramelo quente, visualmente próximo ao de Marcos. É uma correspondência visual generativa, não identidade numérica de todas as cores. Os desenhos de caminhada novos são adaptações fiéis da referência, não cópias pixel a pixel. Cabeça, corpo e membros foram sempre extraídos juntos; não houve transplante de anatomia, espelhamento de direções, deformação ou recoloração por código.

O empacotamento remove apenas ilhas desconectadas no alpha da margem e usa escala uniforme do quadro inteiro em nearest-neighbor. Não foi removida globalmente nenhuma cor de material. O script `Tools/BuildNewStudents20261029.py` lê exclusivamente as fontes preservadas no repositório e aceita `--only Bianca` ou `--only Moscardini`; não depende de Downloads ou do cache ImageGen.

## Verificação

Os contatos foram inspecionados em fundo claro e ampliados, incluindo a comparação final de pele. Silhuetas completas, pés alinhados, transparência real, ausência de restos entre células e desenhos distintos foram conferidos. A auditoria mais recente em `{Bianca,Moscardini}/audit-post-import/` passou: 16 quadros, quatro clips, zero erros e cinco avisos por personagem.

Os avisos são: alpha parcial em mais de 10% dos pixels visíveis (a maioria perto de 253/255 na fonte gerada, preservado); override herdado de compressão de plataforma; textura importada como Default em vez de Sprite; PPU do importador diferente do manifesto; e ausência de verificação runtime pelo helper. O modo Default e PPU do importador são esperados para o consumidor atual, que cria os sprites e seu PPU explicitamente. Nenhum aviso foi convertido em aprovação visual automática.

As prévias GIF e HTML atualizadas estão em `animation-post-import`. A progressão dos quatro desenhos foi inspecionada no contato; a reprodução animada ao vivo não foi verificada neste subagente: o navegador bloqueou a abertura de HTML local `file://`, sem tentativa de contorno. Testes Unity e movimento no jogo precisam constar do relatório principal. As paletas preservam a saída gerada: cerca de 11,5 mil cores RGBA em Bianca e 8 mil em Moscardini; não foi aplicada quantização visual arbitrária.

O reexportador Unity pode recodificar o PNG com os mesmos pixels e alterar o hash de bytes. Os manifestos foram atualizados após essa importação; `audit-post-import` é a auditoria vigente. Os diretórios `audit` e `audit-final` guardam verificações anteriores, incluindo uma falha transitória de hash durante a reimportação de Bianca, já resolvida.
