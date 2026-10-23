# Globo escolar dos fragmentos

Pedido aprovado no plano mestre: corrigir o objeto pontudo para um globo esférico, mantendo contexto, suporte e estilo. Fonte observada: globo azul e dourado sobre o armário da biblioteca; vista levemente superior. A substituição é restrita ao objeto e à pequena área necessária para remover seu suporte antigo.

O candidato isolado foi produzido com ImageGen. A normalização do novo desenho usa nearest-neighbor para 40×56, sem redimensionar a arte aprovada da biblioteca. A placa de integração de 44×64 copia os pixels originais do cenário e reaproveita uma faixa adjacente da mesma madeira somente sob a esfera, para eliminar o suporte antigo. O PNG original IndustrialLibrary.png e seu GUID permanecem intactos. Hashes e contagem de diferenças estão em provenance.json.

Integração aditiva: Assets/Resources/Varginha/SchoolGlobe/Globe.png e Atlas.json, instanciados por CampaignIllustratedMaps na fase 6. A placa corresponde à região original [874,126,44,64]. PPU 50, pivô inferior [22,64] na origem superior, posição em pixels [896,190]. Escala mundial unitária, compensando a escala herdada do armário. Profundidade vinculada ao suporte existente, sem novo collider ou alteração do puzzle.

O desenho mostra esfera azul, continentes verdes e ocres, aro/eixo inclinado em latão e base azul com dourado. Prévia em contexto e contact sheet a 4× foram abertos e inspecionados. Um único frame estático; não foi criada nem alegada uma animação de objeto. Não há arquivo Aseprite com camadas: o fonte disponível é o PNG gerado preservado.

Pipeline: pack aprovado estruturalmente; validate sem erros e com um aviso: conteúdo visível toca as bordas. Disposição: esperado, pois a placa contém fundo opaco do cenário até a borda; o globo isolado mantém margem e não está cortado. A paleta ampla (2.213 cores na placa) inclui a textura original da madeira, não uma paleta nova aplicada globalmente. PNG RGBA, zero pixels semitransparentes na placa. preview/contact.png e preview/clip_000.png são evidências; as flags automáticas de revisão permanecem falsas, pois a inspeção é registrada aqui.

Importação e enquadramento no Unity ainda precisam ser verificados. Não tratar o preview offline como aprovação do runtime.

Importação e contexto real verificados: visual-actions-playmode.xml, 16/16 no recorte, e world-review/globe-library.png, aberto e inspecionado. Esfera legível, base apoiada no armário, escala e filtro corretos; investigação preservada.
