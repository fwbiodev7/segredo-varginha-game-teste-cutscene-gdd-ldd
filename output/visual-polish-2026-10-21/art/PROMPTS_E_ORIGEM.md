# Arte desta revisão

Ferramenta: `image_gen` integrada, sem API/CLI alternativo. Resumo dos prompts de produção e fontes usadas:

| Saída | Referências e instruções principais | Arquivo gerado original |
|---|---|---|
| FuscaCabinCartoon | Editar a shell anterior mantendo enquadramento, banco, janelas/alpha e porta. Contornos fortes, volumes arredondados, rampas simples e material fosco. Sem motorista, volante, coluna ou painel. Driving.png como referência de estilo | exec-43e6522c-0420-47ba-b678-313c41c1a20e.png |
| FuscaDashboardCartoon | Editar painel anterior e combinar com a shell nova. Preservar silhueta e posições do mostrador/rádio, reduzir brilho/grão/madeira realista. Fundo transparente, sem volante/coluna; mostrador e rádio escuros para animação do jogo | exec-9fbc6835-dc28-4a4f-bc4f-7b5812e6cd6d.png |
| Landmarks | Atlas 3×2: árvore, tronco horizontal, ponte horizontal, muro, capela e portão aberto. Contorno pixel art e fundo transparente. Segunda geração recolocou a cruz inteiramente na célula da capela e pediu remoção de halos | exec-bdbb20ba-2ca8-47c1-be1b-540a5e009fe3.png |
| Terrain | Clareira horizontal 3:1, rio vertical central, bordas de vegetação e centro livre. Sem caminhos, marcos ou UI, que são compostos pelo jogo | exec-976d33fa-d766-49ec-ba70-1d5f56cdf8a0.png |
| ChildRoom1996 | Sala comum noturna, madeira, sofá azul, mesa com jornal e CRT à direita com tela vazia. Baseada nos móveis de Child1996Divisoes e estilo de ChildEdelzio. Sem pessoa, clarão ou objeto adulto | exec-d7e890dd-e245-4497-8ee8-b52869b37556.png |
| ChildWatchingTV | Oito poses 4×2 de Edelzio aos seis anos sentado no chão, três quartos/costas voltado à direita. Cabelo e camiseta amarela/bermuda azul da referência; respiração e atenção discretas; mesma base e escala. Sem cenário/glow/UI | exec-9a725d76-400e-44ce-8d6a-a71edb8aa1c3.png |
| Newspaper1996 | Jornal sobre mesa de madeira, fotos genéricas de rua à noite e manchete em branco para texto do jogo. Sem pessoa identificável, mapa, entidade, símbolo, solução ou depoimento real | exec-6c2a891a-3fb7-4971-acb8-7ba753214de4.png |
| MorningTown | Foto enviada codex-clipboard-895109b0-316c-4ae4-abef-080be0973733.png como referência visual. Cidade, relevo, vias curvas e marco arquitetônico em forma de nave, manhã às 7h, pixel art cartunesca, sem sobreposições de redes sociais, texto ou elemento sobrenatural | exec-2dab1b97-0120-4d46-84a6-f027657372b4.png |

As fotografias pessoais de referência não entram no pacote do jogo. Candidato inicial de marcos e panorama genérico anterior foram descartados. PNGs novos são camadas achatadas, não arquivos Aseprite com camadas. Resoluções efetivas, hashes e cores estão em `skill-exports/source-inventory.json`; não se declara uma paleta estrita de 16/32 cores.

O atlas gerado da criança tem 1774×887. As receitas da skill usam crops inteiros explícitos e padding até 444×444 para QA, sem redesenhar/redimensionar pixels; a integração usa UVs das oito células e resolução de alpha na GPU. A base opaca medida fica em y=428 em todos os crops. O atlas de marcos tem seis células 512×512. Atlas de QA, metadados canônicos, planos de slicing e animações estão em `skill-exports/`.

Avisos da skill: fontes geradas possuem pixels de alpha baixo nas bordas/halos. Os warnings foram preservados. A composição de runtime usa alpha binário e Point; foram revistas capturas reais dos marcos, criança e loop. A presença de JSON/validação não concede aprovação artística da equipe, ainda pendente.
