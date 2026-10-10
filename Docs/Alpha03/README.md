# O Segredo de Varginha — Alpha 0.3

Build Windows x64 gerada com Unity 6000.6.0f1. Mantém as 14 fases e inclui as áreas adicionais da campanha.

Inclui a revisão dos três mapas do casarão, das divisões dos cômodos e dos móveis; melhorias de HUD e pistas dos puzzles; progressão de atmosfera; e correções de recortes e alinhamento dos personagens. As 55 folhas de personagens usam suas dimensões nativas, filtro Point e geometria que exclui fragmentos de frames vizinhos.

Extraia todo o pacote `jogo_varginha_alpha_0.3_Windows_x64.zip` e abra `jogo_varginha_alpha_0.3.exe`. Mantenha a pasta `_Data` e as bibliotecas junto do executável. O Unity Editor não é necessário. Use **Continuar** para retomar o progresso; **Iniciar campanha** substitui o save local.

O [manifesto da build](build-manifest.json) registra o resultado, as cenas, o tamanho e os checksums do pacote. O código, a versão e esse registro estão no mesmo commit. Os executáveis permanecem fora do histórico Git, conforme a configuração do projeto.

Validação dos personagens: três testes Play Mode aprovados, cobrindo 928 quadros e poses. Essa conferência não representa uma jogada completa das 14 fases.

O executável permaneceu ativo no menu por 12 segundos, sem exceções de inicialização. O ZIP passou na verificação de CRC. O Unity concluiu a build com sucesso; o único erro contado no relatório é um timeout da chamada de automação, e o aviso informa que a automação do Editor está desativada no Player. Ambos estão registrados em [diagnósticos](build-diagnostics.json).

Para reproduzir, use **Varginha → Build → Alpha Windows x64** no Unity e execute `Tools/PackageAlpha.py` para empacotar a saída e verificar o CRC do ZIP.
