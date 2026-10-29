# Entrada de dublagem — aguardando os arquivos reais

Colocar aqui um take escolhido por ID, sem música/efeitos e sem juntar várias falas. WAV mono 48 kHz é preferível; MP3/OGG bons são aceitos sem conversão ou nova gravação apenas pelo formato. Preservar os originais e guardar alternativas fora desta pasta para evitar duplicados.

IDs esperados (15):

- REP_01, REP_02
- NAR_01, NAR_02, NAR_03, NAR_04, NAR_05, NAR_06, NAR_07
- NAR_08, NAR_09, NAR_10, NAR_11, NAR_12, NAR_13

Nomear `ID.wav`, `ID.mp3` ou `ID.ogg`, por exemplo `REP_01.wav`. Não criar arquivos vazios ou texto com extensão de áudio para preencher a lista. Esta pasta ainda não contém uma entrega confirmada.

Na raiz do projeto:

```powershell
python Tools/Audio/check_narrator_delivery.py --inputdir output/recebimento-narrador-2026-10-29/entrada
```

O checker gera um JSON fora desta pasta com IDs faltantes/duplicados, hashes e metadados disponíveis. Ele não altera os áudios e não atualiza automaticamente o manifest. Consultar `../manifest.json` e `Docs/CampanhaOficial/DUBLAGEM_NARRADOR_RECEBIMENTO_2026-10-29.md` para gatilhos e validação. Sincronização e integração final aguardam os takes recebidos e aprovados por escuta.
