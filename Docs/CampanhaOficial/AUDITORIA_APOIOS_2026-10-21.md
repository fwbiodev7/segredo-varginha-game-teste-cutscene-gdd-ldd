# Auditoria dos apoios — Renan, Ouzana e Padre Fábio

Solicitada pelo usuário durante a integração de áudio. Foram inspecionados o código e os ataques no Player Windows. Os prints foram obtidos numa arena de teste controlada, com o chefe imobilizado e vida ampliada para comparar as habilidades. Isso não representa uma sessão humana de balanceamento.

## Problemas encontrados e ajustes autorizados

1. **Renan: efeito e alcance não comunicavam a mesma coisa.** O desenho antigo mostrava arcos direcionais, enquanto o acerto é radial, de raio 5,5 a partir do lançamento. A aprovação do usuário manteve dano/alcance e depois definiu um estilo cyber. O novo efeito mostra o limite radial, pacotes de dados e fragmentos de interferência nos inimigos cujo impacto foi aceito. O glitch sonoro recebido continua exclusivo dessa habilidade.
2. **Ouzana: área pouco legível.** A imagem original tinha largura máxima de 2,667 unidades, diante de uma área de dano de diâmetro 4. O frasco e a nuvem foram mantidos; o novo atlas acrescenta oito quadros e a expansão da nuvem. A quebra sonora acontece uma vez no impacto, sem repetir nos ticks.
3. **Padre: opacidade e relógios divergentes.** A cúpula anterior cobria os personagens. Agora há contorno dourado com centro transparente. Foi reproduzida a proteção perdendo tempo numa mensagem modal acionada pela API `Say`, enquanto a animação aguardava. A duração agora só avança quando a ação pode avançar. Pausa por ESC e mochila já congelavam `Time.timeScale`; não foram descritas como reprodução desse defeito.
4. **Legendas sobre a ação.** O nome da habilidade cobria os personagens. O aviso do apoio passou para uma faixa inferior, acima da ajuda de controles, mostrando apenas o apoio selecionado.

As correções foram aprovadas pelo usuário antes da alteração; depois ele solicitou explicitamente as novas artes dos três apoios. Os sprites originais de personagens e o atlas anterior foram preservados.

A retomada sonora também respeita pausas combinadas: encerrar uma suspensão local não reativa o escudo enquanto a pausa global ou outra interface continuar ativa. Esse caso foi acrescentado ao teste de reprodução/volume/duração do escudo.

## Mecânicas preservadas

| Apoio | Área / efeito | Dano / proteção | Recarga |
|---|---|---|---|
| Renan | Raio 5,5; alvos com linha de visão | 42; atordoamento de 2,2 s | 8 s |
| Ouzana | Explosão/nuvem com raio 2 | 24 inicial e ticks de 6 a cada 0,65 s durante a nuvem; lentidão | 8 s |
| Padre Fábio | Raio 2,7; repele filhotes próximos | Reduz dano em 55% por 6 s ativos; cura existente de 20 | 12 s |

O arremesso usa o alvo escolhido ao lançar, sem perseguir continuamente um chefe em movimento. Isso comunica esquiva e posicionamento; não foi convertido em projétil guiado. O acerto em área usa a distância à posição do inimigo, com verificação de obstáculos, como antes.

## Limites de efeitos

Os três apoios não criam `ParticleSystem`. Cada invocação tem um objeto de efeito, destruído no fim. Renan acrescenta um único mesh limitado a 256 quads, até seis alvos (chefe e cinco filhotes), sem consumir o RNG de gameplay. As listas são reutilizadas durante a animação. Áudio de impacto é disparado por evento, sem emissão a cada frame.

Esses limites e a limpeza são verificações técnicas; não significam 60 FPS comprovados em outro PC. O balanceamento com alunos, chefe móvel, equipamentos e diferentes dificuldades ainda precisa de playtest humano.

## Capturas

Antes: [Renan](../../output/audio-integration-2026-10-21/before/support-0-cast.png), [Ouzana](../../output/audio-integration-2026-10-21/before/support-1-impact.png), [Padre](../../output/audio-integration-2026-10-21/before/support-2-cast.png).

Depois: [Renan — disparo](../../output/audio-integration-2026-10-21/captures/support-0-cast.png), [Renan — glitch no alvo](../../output/audio-integration-2026-10-21/captures/support-0-impact.png), [Ouzana — arremesso](../../output/audio-integration-2026-10-21/captures/support-1-cast.png), [Ouzana — impacto](../../output/audio-integration-2026-10-21/captures/support-1-impact.png), [Padre — escudo](../../output/audio-integration-2026-10-21/captures/support-2-cast.png), [Padre — sustentação](../../output/audio-integration-2026-10-21/captures/support-2-impact.png).

Resultados medidos e estado final: [relatório e evidências](../../output/audio-integration-2026-10-21/RELATORIO.md). Sugestões seguintes dependem de observação real: clareza do limite contra chefe móvel, tempo da nuvem, utilidade do escudo e volume dos três sons.
