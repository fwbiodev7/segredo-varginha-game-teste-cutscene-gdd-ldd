# Site de divulgação — O Segredo de Varginha

Site estático em HTML, CSS e JavaScript, sem dependências de execução. Arquivos publicados: `dist/`.

## Conteúdo

`dist/config.js` centraliza versão, estágio, screenshots, fontes, atualizações, créditos e canais oficiais. `download`, `stores`, `socials`, `credits` e `videos` aguardam materiais confirmados. Links vazios ficam fora da interface. Não publicar arquivos internos do projeto como download do jogo.

Para vídeos, use `{ title, description, url, poster, type }`, com `type: 'youtube'` para uma URL pública de vídeo do YouTube ou `type: 'file'` para um MP4/WebM público via HTTPS. O player carrega somente depois do clique.

Para créditos use `{ role, name }`; para redes use `{ title, url }`. A distribuição exige páginas oficiais válidas. Atualize `siteUrl`, canonical e Open Graph em `index.html` ao mudar de domínio.

## Prévia local

Sirva `dist/` com qualquer servidor estático. O projeto não necessita de instalação ou build. A hospedagem privada está identificada em `.openai/hosting.json`; preserve seu identificador nas próximas atualizações.

## Arte e acessibilidade

As capturas são do próprio jogo. Sprites são recortes técnicos dos atlas aprovados. Os WebP usam compressão sem perdas. A fonte inclui sua licença SIL OFL.

Galeria e documentos abrem em diálogo acessível, com Escape e retorno do foco. Há controles por teclado para comparação e ampliação. O site respeita movimento reduzido, oferece desligamento de efeitos e não reproduz áudio automaticamente. LocalStorage armazena apenas preferência visual e identificadores dos documentos visitados.

## Curadoria

O material público é uma amostra sem respostas dos puzzles. Não copie guias internos, roteiro completo ou registros de teste para `dist/`. As fontes históricas têm links diretos, datas e resumos próprios; os arquivos interativos são identificados como ficção. Fotos de referência de pessoas não são publicadas.

## Verificação

`node checks/static.mjs` verifica referências locais, JavaScript, configuração, links e restrições editoriais. Inspecione também desktop, celular, menu, galeria, documentos e terminal em um navegador ao alterar comportamento ou layout.
