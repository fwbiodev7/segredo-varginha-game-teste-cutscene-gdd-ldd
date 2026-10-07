(() => {
  'use strict';
  const config = window.GAME_SITE;
  if (!config) return;
  const $ = (selector, parent = document) => parent.querySelector(selector);
  const $$ = (selector, parent = document) => [...parent.querySelectorAll(selector)];
  const escape = text => String(text).replace(/[&<>"']/g, char => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[char]));
  const externalURL = url => { try { const value = new URL(url); return value.protocol === 'https:' ? value.href : null; } catch { return null; } };
  const store = {
    get(key, fallback) { try { const value = localStorage.getItem('varginha-site.' + key); return value === null ? fallback : JSON.parse(value); } catch { return fallback; } },
    set(key, value) { try { localStorage.setItem('varginha-site.' + key, JSON.stringify(value)); } catch { /* A visita continua sem armazenamento. */ } }
  };
  const savedVisited = store.get('visited', []);
  const visited = new Set(Array.isArray(savedVisited) ? savedVisited.filter(id => config.archives.some(a => a.id === id)) : []);
  const motionQuery = matchMedia('(prefers-reduced-motion: reduce)');
  let effects = store.get('effects', !motionQuery.matches) === true;
  function updateEffects() {
    document.documentElement.classList.toggle('effects-off', !effects);
    $('#effects-toggle').textContent = 'EFEITOS VISUAIS: ' + (effects ? 'ATIVOS' : 'DESATIVADOS');
    $('#effects-toggle').setAttribute('aria-pressed', String(effects));
  }
  updateEffects();
  $('#effects-toggle').addEventListener('click', () => { effects = !effects; store.set('effects', effects); updateEffects(); });
  motionQuery.addEventListener('change', event => { if (event.matches) { effects = false; updateEffects(); } });

  const nav = $('#nav');
  const menuButton = $('.menu-toggle');
  function closeMenu() { nav.classList.remove('open'); menuButton.setAttribute('aria-expanded','false'); }
  menuButton.addEventListener('click', () => { const open = !nav.classList.contains('open'); nav.classList.toggle('open',open); menuButton.setAttribute('aria-expanded',String(open)); });
  nav.addEventListener('click', event => { if (event.target.closest('a')) closeMenu(); });
  document.addEventListener('keydown', event => { if (event.key === 'Escape' && nav.classList.contains('open')) { closeMenu(); menuButton.focus(); } });
  const navMedia = matchMedia('(min-width:801px)');
  navMedia.addEventListener('change', event => { if (event.matches) closeMenu(); });
  if ('IntersectionObserver' in window) {
    const observer = new IntersectionObserver(entries => {
      for (const entry of entries) if (entry.isIntersecting) {
        $$('#nav a').forEach(link => { if (link.hash === '#' + entry.target.id) link.setAttribute('aria-current','location'); else link.removeAttribute('aria-current'); });
      }
    }, {rootMargin:'-15% 0px -65% 0px',threshold:0});
    $$('main section[id]').forEach(section => observer.observe(section));
  }

  $('#updated-date').textContent = config.updated;
  $('#version-label').textContent = config.version;
  $('#available-label').textContent = config.available + '.';
  $('#planned-label').textContent = config.planned;
  $('#updates-list').innerHTML = config.updates.map(update => `<article class="update"><time>${escape(update.date)}</time><h3>${escape(update.title)}</h3><p>${escape(update.text)}</p></article>`).join('');
  $('#history-timeline').innerHTML = config.historySources.map(source => `<article class="history-entry"><time class="history-date">${escape(source.date)}</time><div><p class="eyebrow">${escape(source.kind)}</p><h3>${escape(source.title)}</h3><p>${escape(source.summary)}</p></div><div class="history-source"><div>${escape(source.institution)}<small>${escape(source.publication)}</small></div><a class="source-link" href="${escape(externalURL(source.url) || '#caso-real')}" target="_blank" rel="noopener noreferrer">CONSULTAR A FONTE<span class="sr-only"> (abre em outra aba)</span></a></div></article>`).join('');
  if (config.credits.length) $('#credits-list').innerHTML = '<ul>' + config.credits.map(credit => `<li>${escape(credit.role)}: ${escape(credit.name)}</li>`).join('') + '</ul>';
  $('#social-links').innerHTML = config.socials.filter(social => externalURL(social.url)).map(social => `<a href="${escape(externalURL(social.url))}" target="_blank" rel="noopener noreferrer">${escape(social.title)}<span class="sr-only"> (abre em outra aba)</span></a>`).join('');
  const distribution = [];
  if (externalURL(config.download)) {
    distribution.push(`<a class="button primary" href="${escape(externalURL(config.download))}" target="_blank" rel="noopener noreferrer">BAIXAR VERSÃO DE TESTES<span class="sr-only"> (abre em outra aba)</span></a>`);
    $('#hero-extra').innerHTML = distribution[0];
  }
  for (const [key,title] of [['steam','STEAM'],['itch','ITCH.IO']]) if (externalURL(config.stores[key])) distribution.push(`<a class="button" href="${escape(externalURL(config.stores[key]))}" target="_blank" rel="noopener noreferrer">${title}<span class="sr-only"> (abre em outra aba)</span></a>`);
  if (distribution.length) $('#distribution-links').innerHTML = distribution.join('');

  let filter = 'all';
  const grid = $('#gallery-grid');
  grid.innerHTML = config.screenshots.map((shot,index) => `<button type="button" class="gallery-item" data-shot="${index}" data-category="${escape(shot.category)}" aria-label="Ampliar: ${escape(shot.title)}"><span class="gallery-image"><img src="${escape(shot.src.replace('.webp','-small.webp'))}" alt="${escape(shot.alt)}" loading="lazy" decoding="async"><span class="category">${escape(shot.category)}</span><span class="expand-image" aria-hidden="true">+</span></span><span class="gallery-caption"><b>${escape(shot.title)}</b><small>${escape(shot.place)}</small></span></button>`).join('');
  $$('.gallery-filters button').forEach(button => button.addEventListener('click', () => {
    filter = button.dataset.filter;
    $$('.gallery-filters button').forEach(item => item.setAttribute('aria-pressed',String(item === button)));
    $$('.gallery-item').forEach(item => { item.hidden = filter !== 'all' && item.dataset.category !== filter; });
    $('#site-announcement').textContent = 'Galeria: ' + $$('.gallery-item:not([hidden])').length + ' imagens.';
  }));

  const dialog = $('#viewer');
  let opener = null;
  function present(title, description, category, content, tools = '', retainFocus = false) {
    $('#viewer-title').textContent = title;
    $('#viewer-description').textContent = description;
    $('#viewer-category').textContent = category;
    $('#viewer-content').innerHTML = content;
    $('#viewer-tools').innerHTML = tools;
    if (!dialog.open) { opener = document.activeElement; dialog.showModal(); document.body.style.overflow = 'hidden'; }
    if (!retainFocus) $('.close-dialog',dialog).focus();
    dialog.scrollTop = 0;
  }
  $('.close-dialog',dialog).addEventListener('click', () => dialog.close());
  dialog.addEventListener('click', event => { if (event.target !== dialog) return; const bounds = dialog.getBoundingClientRect(); if (event.clientX < bounds.left || event.clientX > bounds.right || event.clientY < bounds.top || event.clientY > bounds.bottom) dialog.close(); });
  dialog.addEventListener('close', () => { document.body.style.overflow = ''; $('#viewer-content').replaceChildren(); $('#viewer-tools').replaceChildren(); if (opener?.isConnected) opener.focus(); });
  function galleryView(index, retainFocus = false) {
    const shot = config.screenshots[index];
    const visible = config.screenshots.map((value,i) => ({value,i})).filter(item => filter === 'all' || item.value.category === filter).map(item => item.i);
    const position = visible.indexOf(index);
    const previous = visible[(position - 1 + visible.length) % visible.length];
    const next = visible[(position + 1) % visible.length];
    present(shot.title, shot.place + (shot.category === 'CUTSCENE' ? ' · Imagem usada na abertura, não é um vídeo.' : ' · Captura da versão de desenvolvimento.'), shot.category, `<img class="viewer-image" src="${escape(shot.src)}" alt="${escape(shot.alt)}">`, `<div class="viewer-nav"><button type="button" data-previous>← ANTERIOR</button><span class="mono">${position + 1} / ${visible.length}</span><button type="button" data-next>PRÓXIMA →</button></div>`,retainFocus);
    $('[data-previous]').addEventListener('click', () => { galleryView(previous,true); $('[data-previous]').focus(); });
    $('[data-next]').addEventListener('click', () => { galleryView(next,true); $('[data-next]').focus(); });
  }
  grid.addEventListener('click', event => { const button = event.target.closest('[data-shot]'); if (button) galleryView(Number(button.dataset.shot)); });

  function archiveRows() {
    $('#archive-list').innerHTML = config.archives.map((archive,index) => `<button type="button" class="archive-row${visited.has(archive.id) ? ' visited' : ''}" data-archive="${index}" aria-label="Examinar ${escape(archive.id + ': ' + archive.title)}${visited.has(archive.id) ? ', já visitado' : ''}"><span class="archive-id mono">${escape(archive.id)}</span><span><b>${escape(archive.title)}</b><small>${escape(archive.description)}</small></span><span class="archive-arrow" aria-hidden="true">${archive.type === 'locked' ? '□' : '↗'}</span></button>`).join('');
  }
  archiveRows();
  $('#archive-list').addEventListener('click', event => {
    const button = event.target.closest('[data-archive]');
    if (!button) return;
    const archive = config.archives[Number(button.dataset.archive)];
    const returned = visited.has(archive.id);
    const localOpener = button;
    visited.add(archive.id);store.set('visited',[...visited]);
    localOpener.classList.add('visited');
    localOpener.setAttribute('aria-label','Examinar ' + archive.id + ': ' + archive.title + ', já visitado');
    $('.archive-id',localOpener).style.color = 'var(--cyan)';
    let content = archive.image ? `<img class="viewer-image" src="${escape(archive.image)}" alt="${escape(archive.alt)}">` : '';
    let tools = '';
    if (archive.type === 'compare') {
      content = `<div class="compare" style="--compare:50%"><img src="${escape(archive.second)}" alt="Casa de Edelzio em 2026"><div class="compare-before"><img src="${escape(archive.image)}" alt="Casa de infância em 1996"></div><span class="compare-line" aria-hidden="true"></span><span class="compare-label before">1996</span><span class="compare-label after">2026</span></div>`;
      tools = '<label class="viewer-slider" for="compare-range">Deslize para comparar as duas épocas<input type="range" id="compare-range" min="0" max="100" value="50" aria-valuetext="1996: 50 por cento; 2026: 50 por cento"></label>';
    }
    if (archive.type === 'zoom') {
      content = `<div class="fragment-view"><img src="${escape(archive.image)}" alt="${escape(archive.alt)}"></div>`;
      tools = '<label class="viewer-slider" for="zoom-range">Ampliar o fragmento<input type="range" id="zoom-range" min="100" max="250" step="25" value="100" aria-valuetext="Ampliação: 100 por cento"></label>';
    }
    if (archive.type === 'note') tools = '<button type="button" class="inspect-button" id="inspect-note" aria-expanded="false">EXAMINAR O VERSO →</button><p id="note-text" class="note-reveal" hidden></p>';
    if (archive.type === 'locked') content = '<div class="unavailable">REGISTRO INCOMPLETO<small>A cópia termina antes da próxima página.</small></div>';
    present(archive.id + ' / ' + archive.title,archive.text + (returned && effects && archive.type === 'broadcast' ? ' O sinal continua fraco. Você já esteve aqui.' : ''),'FICÇÃO · ARQUIVO PROMOCIONAL',content,tools);
    if (archive.type === 'compare') $('#compare-range').addEventListener('input', event => { $('.compare').style.setProperty('--compare',event.target.value + '%'); event.target.setAttribute('aria-valuetext','1996: ' + event.target.value + ' por cento; 2026: ' + (100 - event.target.value) + ' por cento'); });
    if (archive.type === 'zoom') $('#zoom-range').addEventListener('input', event => { $('.fragment-view').style.setProperty('--zoom',event.target.value + '%'); event.target.setAttribute('aria-valuetext','Ampliação: ' + event.target.value + ' por cento'); });
    if (archive.type === 'note') $('#inspect-note').addEventListener('click', event => { const text = $('#note-text'); text.hidden = !text.hidden; text.textContent = '“Voltar aos lugares. Fotografar os detalhes. Conferir as datas.”'; event.currentTarget.setAttribute('aria-expanded',String(!text.hidden)); event.currentTarget.textContent = text.hidden ? 'EXAMINAR O VERSO →' : 'GUARDAR A ANOTAÇÃO ←'; });
  });

  const terminalOutput = $('#terminal-output');
  const commands = {
    help: () => 'COMANDOS DISPONÍVEIS\nhelp         Mostra os comandos\narquivos     Lista os fragmentos\nstatus       Mostra os arquivos visitados\ntransmissao  Consulta um trecho do sinal\n\nEste terminal é fictício. A consulta permanece neste navegador.',
    arquivos: () => config.archives.map(archive => archive.id + ' / ' + archive.title).join('\n') + '\n\nExamine os documentos ao lado para abrir cada registro.',
    status: () => 'ARQUIVOS VISITADOS: ' + visited.size + ' / ' + config.archives.length + '\nSINAL: INCOMPLETO\nCONSULTA: LOCAL\n\nA investigação continua dentro do jogo.',
    transmissao: () => 'TRECHO DA ABERTURA / FICÇÃO\n\n“…Não deixe ela sair.”\n\nO restante da transmissão não está nesta cópia.'
  };
  $('#terminal-form').addEventListener('submit', event => {
    event.preventDefault();
    const input = $('#terminal-input');
    const command = input.value.trim().toLowerCase();
    if (!command) return;
    const echo = document.createElement('p');echo.textContent = '> ' + command;
    const response = document.createElement('p');response.textContent = Object.prototype.hasOwnProperty.call(commands,command) ? commands[command]() : 'Comando não reconhecido. Digite help para ver as consultas disponíveis.';
    terminalOutput.append(echo,response);
    while (terminalOutput.children.length > 15) terminalOutput.firstElementChild.remove();
    terminalOutput.scrollTop = terminalOutput.scrollHeight;input.value = '';
  });

  function videoURL(video) {
    const url = externalURL(video.url);if (!url) return null;
    if (video.type === 'file') return /\.(mp4|webm)(?:[?#]|$)/i.test(url) ? url : null;
    if (video.type === 'youtube') { const parsed = new URL(url); let id = null; if (['www.youtube.com','youtube.com'].includes(parsed.hostname)) id = parsed.searchParams.get('v'); if (parsed.hostname === 'youtu.be') id = parsed.pathname.slice(1); return /^[a-zA-Z0-9_-]{11}$/.test(id || '') ? 'https://www.youtube-nocookie.com/embed/' + id + '?autoplay=0' : null; }
    return null;
  }
  const videos = config.videos.map(video => ({...video,resolved:videoURL(video)})).filter(video => video.resolved);
  if (videos.length) {
    $('#video-links').innerHTML = '<a class="text-link" href="#video-grid">VER OS VÍDEOS DISPONÍVEIS →</a>';
    $('#video-grid').innerHTML = videos.map((video,index) => `<article class="video-card">${video.poster ? `<img src="${escape(video.poster)}" loading="lazy" alt="Imagem de apresentação: ${escape(video.title)}">` : ''}<h3>${escape(video.title)}</h3><p>${escape(video.description || '')}</p><button type="button" data-video="${index}">ASSISTIR</button></article>`).join('');
    $('#video-grid').addEventListener('click', event => { const button = event.target.closest('[data-video]');if (!button)return;const video = videos[Number(button.dataset.video)];const media = document.createElement(video.type === 'file' ? 'video' : 'iframe');media.src = video.resolved;if(video.type === 'file'){media.controls = true;media.preload = 'none';media.setAttribute('playsinline','');}else{media.title=video.title;media.allow='fullscreen';media.allowFullscreen=true;media.referrerPolicy='strict-origin-when-cross-origin';}button.replaceWith(media); });
  }
})();
