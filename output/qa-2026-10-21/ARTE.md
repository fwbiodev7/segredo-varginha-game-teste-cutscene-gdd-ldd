# Revisão técnica e visual da arte

Data: 21/10/2026. Skill: `pixel-art-sprite-sheet-master`, instalada em `C:/Users/Usuario/.codex/skills/pixel-art-sprite-sheet-master`.

Os auxiliares da skill, ausentes na sessão de ontem, estão disponíveis hoje. Seus **30 testes passaram**. `Tools/KeyInvestigationArt/audit_art.py` executou inspect, pack, validate e preview sobre os oito PNGs entregues ontem, sem gerar arte nova nem redimensionar os arquivos.

O atlas recomposto e o atlas usado pelo jogo têm o mesmo SHA-256: `bb9202bcafdaf81ba42cca050f5cffa7afe98b8bc3b7ec03c735acc3b1cb8a8d`. As dimensões continuam 512×256, com oito células 128×128, cores limitadas e alpha binário. A validação estrutural não encontrou erros ou avisos.

## Inspeção visual e temporal

Contato, overlays e frames da chave e da caixa foram inspecionados em ampliação inteira. O chaveiro azul, a madeira, as ferragens e o caderno conservam identidade e leitura. As duas poses da chave representam sua rotação ao pousar. A caixa mantém base e volume entre os estados fechado e aberto. Os sprites originais de personagens, Fusca e cenários permanecem intactos.

`Tools/QA/art_preview.mjs` abriu a prévia da skill em Chrome e observou seu relógio real: colocação → repouso e caixa fechada → aberta. Ambos avançaram e permaneceram no último frame, sem repetição indevida ou erro de JavaScript. Os tempos dessa prévia são holds de revisão, não substituem a movimentação cinematográfica de `CampaignHouseSequence.cs`.

Evidências: `art/preservation.json`, `art/validation/validation.json`, `art/preview/playback.json`, `art/preview/contact.png` e capturas `playback-*-start/end.png`.

## Disposição dos três avisos de importação

O validador textual aplicou um preset de importação de sprites. Este projeto importa o atlas como **Texture2D** e cria os sprites em runtime:

| Aviso | Configuração efetivamente usada | Decisão |
|---|---|---|
| TextureType 0, esperado 8 | `CampaignKeyArtImporter` usa Default para permitir o desenho por UV no HUD | Preservar Texture2D |
| PPU 100 no meta, esperado 128 | `CampaignHouseArt.Frame` cria todas as células explicitamente com PPU 128 e pivô central | O valor de SpriteImporter não participa desse caminho |
| Compressão 1 encontrada em Standalone | Esse bloco tem `overridden: 0`; o padrão ativo e o importador usam Uncompressed (0) | Não ativar um override apenas para satisfazer busca textual |

Filtro Point e mipmaps desativados permanecem no importador. Esses avisos foram analisados, não ocultados; o relatório original da skill está em `art/unity-meta-triage/validation.json`. A revisão técnica não representa aprovação artística da equipe, que será registrada quando houver feedback.
