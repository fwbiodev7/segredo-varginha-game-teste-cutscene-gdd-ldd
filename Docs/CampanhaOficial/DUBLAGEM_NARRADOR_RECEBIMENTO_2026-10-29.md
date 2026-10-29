# Recebimento da dublagem do repórter e narrador — 29/10/2026

Pacote de recebimento preparado para as **15 falas já enviadas ao ator**: REP_01–02 e NAR_01–13. **Nenhum áudio recebido foi confirmado neste preparo**. O manifest permanece `aguardando`, com duração real vazia e sem arquivos associados. A sincronização e a integração final no jogo dependem dos takes recebidos.

## Entrega e preservação

Entregar um take escolhido por ID, em arquivo separado: `ID.wav` (preferencial), `ID.mp3` ou `ID.ogg`. Exemplo: `REP_01.wav`, `NAR_01.mp3`. WAV mono a 48 kHz é preferível; MP3/OGG bons são aceitos no formato original, sem recodificação e sem exigir nova gravação apenas pelo formato. Preservar o áudio original. Não enviar um único arquivo com várias falas nem acrescentar sufixos de take à seleção final; guardar alternativas fora da pasta conferida.

Voz isolada, sem música ou efeitos e com pequena pausa antes/depois. Conferir por escuta: texto integral do roteiro, pronúncia de Edelzio/Varginha, começo e fim completos, ausência de estalos, saturação e ruído que impeça entender a fala. As medições do checker não substituem essa escuta.

Colocar os arquivos em `output/recebimento-narrador-2026-10-29/entrada/`. O manifest em `../manifest.json` contém os textos exatos e as restrições. O checker apenas lê os arquivos e gera hashes/relatório; não importa, move, recodifica ou sobrescreve áudio.

## Ordem, tempo e proteção das revelações

Gatilhos abaixo são condições narrativas precisas para a futura integração, não hooks já implementados. Fora da abertura, não existe timestamp aprovado para impor às falas; aguardar o evento indicado e medir cada take recebido. Na abertura atual, `paper_1996` ocupa 0–4 s, `news_1996` 4–9 s, `tv_reveal` 9–13 s e `child_1996` 13–16 s. A divisão exata REP_01/REP_02 depende dos takes; preservar a continuidade entre noticiário e TV. Se a fala ultrapassar a janela, ajustar sincronização na integração, sem cortar palavras ou acelerar automaticamente.

Todas as entradas devem tocar uma vez no contexto autorizado, sem sobrepor diálogo, dublagem existente ou leitura/resolução de puzzle. Uma repetição/reload não deve antecipar descobertas. O disparo final e a persistência serão validados quando as vozes forem integradas.

| ID / nome preferido | Cena | Gatilho autorizado | Tempo | Restrição narrativa |
|---|---|---|---|---|
| REP_01.wav | TV da abertura, 1996 | Início do plano news_1996, após paper_1996. | Aos 4 s da abertura; reportagem REP_01 + REP_02 ocupa aproximadamente 8–9 s, até o fim de tv_reveal. | Começar no noticiário; continuar o áudio durante a revelação da TV, sem reiniciar no corte. |
| REP_02.wav | TV da abertura, 1996 | Imediatamente depois de REP_01; continuar em tv_reveal se necessário. | Dentro da janela global 4–13 s; início exato depende da duração real de REP_01. | Manter a incerteza da reportagem; não acrescentar confirmação da criatura. |
| NAR_01.wav | Apresentação de Edelzio criança | Entrada do plano child_1996, antes da entrega do controle. | Plano global 13–16 s; duração real a medir, sem cortar a fala. | Somente apresentar idade; não antecipar acidente, selo ou vida adulta. |
| NAR_02.wav | Casa de 1996: falha elétrica | Depois da falha elétrica confirmada e quando o brilho externo já estiver visível. | Por evento, após falha + brilho; sem timestamp fixo. | Não anunciar a luz antes de aparecer; não antecipar a queda. |
| NAR_03.wav | Fim da noite de 1996 | Depois do clarão, da queda e do escurecimento da tela. | Após concluir os três eventos; sem timestamp fixo. | Não falar da amnésia enquanto o jogador ainda presencia o acidente. |
| NAR_04.wav | Transição para 2026 | Ao apresentar a vida adulta, depois de concluir a sequência de 1996. | Durante a transição temporal para 2026; sem timestamp fixo. | Não revelar a continuidade do selo ou acontecimentos futuros. |
| NAR_05.wav | Casa adulta: chave desaparecida | Depois de voltar à mesa e constatar que a chave sumiu, com os papéis já apresentados. | Após a constatação do desaparecimento; sem timestamp fixo. | Não disparar ao deixar a chave ou antes de ver o desaparecimento/papéis. |
| NAR_06.wav | Casa adulta: caixa | Somente depois de abrir a caixa e encontrar os recortes. | Após a descoberta dos recortes; sem timestamp fixo. | Não revelar os recortes antes da abertura ou resolver a investigação da chave. |
| NAR_07.wav | Trajeto do Fusca | Depois de terminar a interferência do trajeto. | Após o fim da interferência; sem timestamp fixo. | Não sobrepor a voz da interferência nem explicar sua origem. |
| NAR_08.wav | Escola: investigação dos recortes | Depois de concluir a investigação dos recortes na escola. | Após a conclusão confirmada; sem timestamp fixo. | Não dar a resposta durante a comparação/leitura do puzzle. |
| NAR_09.wav | Floresta: perseguição | Depois de a ameaça aparecer, com a perseguição já iniciada. | Durante a perseguição ativa, após a revelação da ameaça. | Não anunciar o perseguidor antes da aparição; não revelar a identidade. |
| NAR_10.wav | Registro reconstruído: ligação de Edelzio | Depois de reconstruir o registro e descobrir que Edelzio mantém a entidade contida. | Após a reconstrução e a descoberta confirmadas; sem timestamp fixo. | Revelação protegida: nunca antes da descoberta da ligação; ÂNCORA do compartimento não substitui Edelzio como selo. |
| NAR_11.wav | Registros históricos de 1898 | Depois de encontrar e confirmar os registros de 1898. | Após encontrar E confirmar; sem timestamp fixo. | Não revelar a antiguidade antes da investigação/confirmação dos registros. |
| NAR_12.wav | Passagem para a travessia final | Depois de derrotar a manifestação, concluir a calibração e abrir a passagem; antes de atravessar. | Somente quando os três requisitos estiverem concluídos, antes da travessia. | Não disparar com calibração pendente nem declarar a ligação rompida. |
| NAR_13.wav | Desfecho: libertação | Somente depois da travessia concluída e do rompimento confirmado da ligação. | Após travessia E rompimento; sem timestamp fixo. | Revelação final protegida: nunca antes de ambos os eventos. |

## Conferência de chegada

Executar na raiz do projeto:

```powershell
python Tools/Audio/check_narrator_delivery.py --inputdir output/recebimento-narrador-2026-10-29/entrada
```

O relatório JSON é criado por padrão em `output/recebimento-narrador-2026-10-29/validation-<UTC>.json`, com inventário, faltantes, duplicados, tamanho, SHA-256, duração/taxa/canais disponíveis, avisos e erros. Código de saída 1 indica lote incompleto/inválido; 0 indica inventário tecnicamente aprovado para escuta, sem confirmar integração. `--report caminho.json` escolhe um destino novo fora da entrada; arquivos existentes não são sobrescritos.

O checker procura `ffprobe` no PATH; `--ffprobe caminho/ffprobe.exe` permite indicar um executável. Quando disponível, exige stream de áudio e duração positiva. Sem ele, WAV tem estrutura/amostras verificadas e MP3/OGG têm somente cabeçalho inspecionado, com limitação explícita no relatório. A ausência de ffprobe, outra taxa ou estéreo não reprova sozinha a entrega. Texto renomeado, arquivo vazio, extensão/ID desconhecido, faltante e duplicado reprovam. A detecção de formato não garante que o take contém a fala correta; manter escuta obrigatória.

Após receber: conferir quantidade/nomes/hashes, ouvir os takes, registrar duração real e só então selecionar os pontos de reprodução, balancear volume e validar legenda/pausa/reload/controle com os arquivos reais. **Não marcar voz recebida/aprovada nem integração concluída apenas por este preparo.**

## Fontes e pontos auxiliares existentes

- Texto aprovado para gravação: [roteiro de produção](ROTEIRO_NARRADOR_REPORTER_2026-10-29.md) e [TXT enviado ao ator](ROTEIRO_PARA_DUBLADOR_2026-10-29.txt).
- `Assets/Scripts/Game/Varginha/Experiment/CampaignOpeningCinema.cs`: quatro planos e durações atuais da abertura; entradas `voice` ainda vazias.
- `Assets/Scripts/Game/Varginha/Experiment/CampaignHouseSequence.cs`: eventos da chave/caixa e sequência do Fusca. São referências de localização, sem alteração nesta preparação.

## Verificação do pacote

Nove verificações de CLI/estrutura passaram: faltantes; duplicados entre extensão, caixa e subpasta; SHA-256/preservação dos originais; 15 textos renomeados rejeitados; vazios/IDs/extensões inválidos; proteção de relatório existente; leitura de um WAV real já existente sem alteração; 15 entradas ainda aguardando; textos iguais ao roteiro do ator. Os fixtures foram temporários e continham texto, sem gerar vozes fictícias. Evidência: `output/agent-logs/2026-10-29T16-12-49-498Z-104a54.stdout.log`.

`ffprobe` não estava disponível no PATH deste ambiente; o ramo de metadados com esse executável não foi exercitado. A conferência da pasta real de entrada retornou código 1 esperado, com zero áudios e os 15 IDs faltantes: `output/recebimento-narrador-2026-10-29/validation-20261029T161305342232Z.json`. Esse resultado registra espera, não aprovação ou falha da gravação. Nenhum teste Unity foi executado.
