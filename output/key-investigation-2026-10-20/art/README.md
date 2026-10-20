# Arte da investigação

Método: skill `pixel-art-sprite-sheet-master`, referência visual da casa adulta e do Fusca original; geração raster pelo `image_gen` integrado, seguida de montagem e normalização de candidato novo. Nenhum pixel das referências originais foi substituído.

Contrato: atlas 512×256, oito células 128×128, pivô central `[64,64]`, origem dos metadados no canto superior esquerdo, PPU 128, perspectiva RPG superior em três quartos. Chave: latão e chaveiro azul; caixa: madeira vermelha, faixas azuis e fecho de latão; papel creme envelhecido e desenho azul. Luz superior esquerda. Personagens e rostos existentes foram reutilizados, sem gerar uma identidade nova.

Observado nas referências: casa de madeira quente, móveis azuis, caixa vermelha/azul, Fusca azul. Inferido para os novos objetos: chaveiro azul redondo, desenho a lápis e aparência interna da caixa. A aprovação de implementação nesta conversa não equivale à aprovação individual de cada frame; JSON mantém `approved: false`.

`generated-source.png` preserva o candidato bruto. O script `Tools/KeyInvestigationArt/build_art.py` usa recortes revisados, padding, redução inteira 4× com nearest-neighbor, até 32 cores por frame e alpha binário. O resultado e hashes estão em `validation.json`; PNGs individuais são fontes editáveis de cada célula. As duas prévias APNG apresentam os pares colocação/repouso e caixa fechada/aberta; são demonstrações dos estados, não substituem o timing do runtime.

Os helpers e algumas referências citadas pelo SKILL.md não estavam presentes nesta instalação. Foram seguidas as referências disponíveis de identidade, reparo e animação, com montagem e validação próprias no projeto. A revisão abriu o contact sheet com todos os estados e conferiu as poses nas capturas da Unity.

Reproduzir: `python Tools/KeyInvestigationArt/build_art.py` (Pillow necessário); a fonte padrão é o PNG bruto versionado nesta pasta. O atlas de runtime e seu JSON ficam em `Assets/Resources/Varginha/KeyInvestigation/`.
