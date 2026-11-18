// Petiatra: comportamentos da interface (sem dependências além do Bootstrap).
(function () {
    'use strict';

    var $ = function (seletor, raiz) { return (raiz || document).querySelector(seletor); };
    var $$ = function (seletor, raiz) { return Array.prototype.slice.call((raiz || document).querySelectorAll(seletor)); };

    function escapar(texto) {
        var div = document.createElement('div');
        div.textContent = texto == null ? '' : String(texto);
        return div.innerHTML;
    }

    function atrasar(funcao, ms) {
        var temporizador;
        return function () {
            var args = arguments, contexto = this;
            clearTimeout(temporizador);
            temporizador = setTimeout(function () { funcao.apply(contexto, args); }, ms);
        };
    }

    function buscarJson(url) {
        return fetch(url, { headers: { 'Accept': 'application/json' }, credentials: 'same-origin' })
            .then(function (resposta) {
                if (!resposta.ok) throw new Error('HTTP ' + resposta.status);
                return resposta.json();
            });
    }

    function digitando(evento) {
        var alvo = evento.target;
        return alvo.isContentEditable || /^(INPUT|TEXTAREA|SELECT)$/.test(alvo.tagName);
    }

    // ---------- Tema claro/escuro ----------
    $$('[data-alternar-tema]').forEach(function (botao) {
        botao.addEventListener('click', function () {
            var escuro = document.documentElement.getAttribute('data-bs-theme') !== 'dark';
            document.documentElement.setAttribute('data-bs-theme', escuro ? 'dark' : 'light');
            try { localStorage.setItem('petiatra-tema', escuro ? 'escuro' : 'claro'); } catch (e) { }
        });
    });

    // ---------- Busca rápida global (RF14) ----------
    var modalBusca = $('#modal-busca');
    var campoBusca = $('#busca-rapida');
    var resultados = $('#busca-resultados');
    var dicaBusca = $('[data-busca-dica]');

    function abrirBusca() {
        if (!modalBusca || !window.bootstrap) return;
        window.bootstrap.Modal.getOrCreateInstance(modalBusca).show();
    }

    if (modalBusca) {
        modalBusca.addEventListener('shown.bs.modal', function () { campoBusca.focus(); campoBusca.select(); });
        $$('[data-abrir-busca]').forEach(function (botao) { botao.addEventListener('click', abrirBusca); });

        var renderizarGrupo = function (titulo, itens, icone) {
            if (!itens.length) return '';
            return '<h3>' + titulo + '</h3>' + itens.map(function (item) {
                return '<a role="option" aria-selected="false" href="' + escapar(item.url) + '">' +
                    '<span class="inicial" aria-hidden="true"><i class="bi ' + icone + '"></i></span>' +
                    '<span><strong>' + escapar(item.texto) + '</strong><br><span class="texto-secundario">' + escapar(item.detalhe) + '</span></span></a>';
            }).join('');
        };

        var pesquisar = atrasar(function () {
            var termo = campoBusca.value.trim();
            if (termo.length < 2) {
                resultados.innerHTML = '';
                dicaBusca.hidden = false;
                campoBusca.setAttribute('aria-expanded', 'false');
                return;
            }
            buscarJson('/Busca/Sugestoes?termo=' + encodeURIComponent(termo)).then(function (dados) {
                var html = renderizarGrupo('Tutores', dados.tutores, 'bi-person') + renderizarGrupo('Pets', dados.pets, 'bi-heart-pulse');
                resultados.innerHTML = html || '<p class="vazio">Nada encontrado para "' + escapar(termo) + '".</p>';
                dicaBusca.hidden = !!html;
                campoBusca.setAttribute('aria-expanded', html ? 'true' : 'false');
                var primeiro = $('a', resultados);
                if (primeiro) primeiro.setAttribute('aria-selected', 'true');
            }).catch(function () {
                resultados.innerHTML = '<p class="vazio">Não foi possível buscar agora.</p>';
            });
        }, 200);

        campoBusca.addEventListener('input', pesquisar);
        campoBusca.addEventListener('keydown', function (evento) {
            var opcoes = $$('a', resultados);
            if (!opcoes.length) return;
            var atual = opcoes.findIndex(function (a) { return a.getAttribute('aria-selected') === 'true'; });

            if (evento.key === 'ArrowDown' || evento.key === 'ArrowUp') {
                evento.preventDefault();
                var proximo = evento.key === 'ArrowDown' ? Math.min(atual + 1, opcoes.length - 1) : Math.max(atual - 1, 0);
                opcoes.forEach(function (a, i) { a.setAttribute('aria-selected', i === proximo ? 'true' : 'false'); });
                opcoes[proximo].scrollIntoView({ block: 'nearest' });
            } else if (evento.key === 'Enter' && atual >= 0) {
                evento.preventDefault();
                window.location.href = opcoes[atual].href;
            }
        });
    }

    // ---------- Atalhos de teclado (RNF05) ----------
    document.addEventListener('keydown', function (evento) {
        var ctrlK = (evento.ctrlKey || evento.metaKey) && evento.key.toLowerCase() === 'k';
        if (ctrlK) {
            evento.preventDefault();
            abrirBusca();
            return;
        }
        if (digitando(evento) || evento.ctrlKey || evento.metaKey || evento.altKey) return;
        if (document.querySelector('.modal.show')) return;

        if (evento.key === '/') {
            evento.preventDefault();
            abrirBusca();
        } else if (evento.key === 'n' || evento.key === 'N') {
            var novo = $('[data-atalho-novo]');
            if (novo) { evento.preventDefault(); window.location.href = novo.href; }
        } else if (evento.key === '?') {
            var atalhos = $('#modal-atalhos');
            if (atalhos && window.bootstrap) window.bootstrap.Modal.getOrCreateInstance(atalhos).show();
        }
    });

    // ---------- Seletor com busca (combobox acessível) ----------
    function iniciarSeletor(raiz) {
        var texto = $('[data-seletor-texto]', raiz);
        var valor = $('[data-seletor-valor]', raiz);
        var lista = $('[role="listbox"]', raiz);
        var url = raiz.getAttribute('data-url');
        var opcoes = [];
        var ativa = -1;

        function marcarEscolhido() { texto.classList.toggle('escolhido', !!valor.value && valor.value !== '0'); }

        function fechar() {
            lista.hidden = true;
            texto.setAttribute('aria-expanded', 'false');
            texto.removeAttribute('aria-activedescendant');
            ativa = -1;
        }

        function destacar(indice) {
            var itens = $$('li[data-id]', lista);
            itens.forEach(function (li, i) { li.setAttribute('aria-selected', i === indice ? 'true' : 'false'); });
            ativa = indice;
            if (itens[indice]) {
                texto.setAttribute('aria-activedescendant', itens[indice].id);
                itens[indice].scrollIntoView({ block: 'nearest' });
            }
        }

        function escolher(indice) {
            var opcao = opcoes[indice];
            if (!opcao) return;
            valor.value = opcao.id;
            texto.value = opcao.texto;
            fechar();
            marcarEscolhido();
            valor.dispatchEvent(new Event('change', { bubbles: true }));
            if (window.jQuery && window.jQuery.validator) window.jQuery(valor).valid();
        }

        var pesquisar = atrasar(function () {
            var termo = texto.value.trim();
            if (termo.length < 2) { fechar(); return; }
            buscarJson(url + '?termo=' + encodeURIComponent(termo)).then(function (dados) {
                opcoes = dados;
                lista.innerHTML = dados.length
                    ? dados.map(function (opcao, i) {
                        return '<li role="option" id="' + lista.id + '-' + i + '" data-id="' + opcao.id + '" aria-selected="false">' + escapar(opcao.texto) + '</li>';
                    }).join('')
                    : '<li class="vazio" role="option" aria-disabled="true">Nada encontrado. Confira o nome ou cadastre antes.</li>';
                lista.hidden = false;
                texto.setAttribute('aria-expanded', 'true');
                if (dados.length) destacar(0);
            });
        }, 200);

        texto.addEventListener('input', function () {
            if (valor.value) {
                valor.value = '';
                marcarEscolhido();
                valor.dispatchEvent(new Event('change', { bubbles: true }));
            }
            pesquisar();
        });

        texto.addEventListener('keydown', function (evento) {
            if (lista.hidden) return;
            if (evento.key === 'ArrowDown') { evento.preventDefault(); destacar(Math.min(ativa + 1, opcoes.length - 1)); }
            else if (evento.key === 'ArrowUp') { evento.preventDefault(); destacar(Math.max(ativa - 1, 0)); }
            else if (evento.key === 'Enter') { evento.preventDefault(); escolher(ativa); }
            else if (evento.key === 'Escape') { evento.preventDefault(); fechar(); }
        });

        lista.addEventListener('mousedown', function (evento) {
            var li = evento.target.closest('li[data-id]');
            if (!li) return;
            evento.preventDefault();
            escolher($$('li[data-id]', lista).indexOf(li));
        });

        texto.addEventListener('blur', function () { setTimeout(fechar, 150); });
        marcarEscolhido();
    }

    $$('[data-seletor]').forEach(iniciarSeletor);

    // ---------- Horários ocupados do veterinário (RF08) ----------
    $$('[data-ocupados]').forEach(function (painel) {
        var veterinario = document.getElementById(painel.getAttribute('data-veterinario'));
        var data = document.getElementById(painel.getAttribute('data-data'));
        var ignorar = painel.getAttribute('data-ignorar') || '';
        if (!veterinario || !data) return;

        function atualizar() {
            if (!veterinario.value || !data.value) { painel.innerHTML = ''; return; }
            var nome = veterinario.options[veterinario.selectedIndex].text;
            buscarJson('/Consultas/Ocupados?veterinarioId=' + veterinario.value + '&data=' + data.value + '&ignorar=' + ignorar)
                .then(function (horarios) {
                    painel.innerHTML = horarios.length
                        ? '<i class="bi bi-clock-history" aria-hidden="true"></i> Horários já ocupados de ' + escapar(nome) + ' neste dia: ' +
                          horarios.map(function (h) { return '<span class="selo selo-neutro">' + escapar(h) + '</span>'; }).join(' ') +
                          '<br><span class="texto-secundario">Deixe pelo menos 30 minutos entre as consultas.</span>'
                        : '<i class="bi bi-check2-circle" aria-hidden="true"></i> ' + escapar(nome) + ' está com a agenda livre neste dia.';
                })
                .catch(function () { painel.innerHTML = ''; });
        }

        veterinario.addEventListener('change', atualizar);
        data.addEventListener('change', atualizar);
        atualizar();
    });

    // ---------- Reforço da vacina: atalhos de data ----------
    $$('[data-somar-dias]').forEach(function (botao) {
        botao.addEventListener('click', function () {
            var origem = document.getElementById(botao.getAttribute('data-origem'));
            var destino = document.getElementById(botao.getAttribute('data-destino'));
            if (!origem || !origem.value || !destino) return;
            var data = new Date(origem.value + 'T12:00:00');
            data.setDate(data.getDate() + Number(botao.getAttribute('data-somar-dias')));
            destino.value = data.toISOString().slice(0, 10);
            destino.dispatchEvent(new Event('change', { bubbles: true }));
        });
    });

    // ---------- Abre "Mais detalhes" quando há erro dentro dele ----------
    $$('details.mais-detalhes').forEach(function (detalhes) {
        if ($('.field-validation-error, .input-validation-error', detalhes)) detalhes.open = true;
    });

    // ---------- Assistente "Novo atendimento" (RF13) ----------
    var assistente = $('[data-assistente]');
    if (assistente) {
        var passos = $$('[data-passo]', assistente);
        var indicadores = $$('[data-indicador]', assistente);
        var atual = Number(assistente.getAttribute('data-passo-inicial') || 0);
        assistente.classList.add('assistente-js');

        var mostrarPasso = function (indice, focar) {
            atual = indice;
            passos.forEach(function (passo, i) { passo.classList.toggle('atual', i === indice); });
            indicadores.forEach(function (item, i) {
                item.classList.toggle('atual', i === indice);
                item.classList.toggle('feito', i < indice);
                if (i === indice) item.setAttribute('aria-current', 'step'); else item.removeAttribute('aria-current');
            });
            if (focar) {
                var titulo = $('h2', passos[indice]);
                if (titulo) { titulo.setAttribute('tabindex', '-1'); titulo.focus(); }
            }
        };

        var passoValido = function (indice) {
            if (!window.jQuery || !window.jQuery.validator) return true;
            var valido = true;
            $$('input, select, textarea', passos[indice]).forEach(function (campo) {
                if (campo.disabled || campo.type === 'radio') return;
                if (!window.jQuery(campo).valid()) valido = false;
            });
            var erro = $('[data-erro-passo]', passos[indice]);
            if (erro) {
                var mensagem = erro.getAttribute('data-erro-passo') === 'tutor' ? validarEscolhaTutor() : validarEscolhaPet();
                erro.textContent = mensagem || '';
                erro.hidden = !mensagem;
                if (mensagem) valido = false;
            }
            return valido;
        };

        $$('[data-avancar]', assistente).forEach(function (botao) {
            botao.addEventListener('click', function () { if (passoValido(atual)) mostrarPasso(atual + 1, true); });
        });
        $$('[data-voltar]', assistente).forEach(function (botao) {
            botao.addEventListener('click', function () { mostrarPasso(atual - 1, true); });
        });

        // passo 1: tutor já cadastrado ou novo
        var tutorId = $('#TutorExistenteId');
        var blocoTutorExistente = $('[data-bloco="tutor-existente"]', assistente);
        var blocoTutorNovo = $('[data-bloco="tutor-novo"]', assistente);
        var modoTutor = function () { var marcado = $('input[name="modoTutor"]:checked', assistente); return marcado ? marcado.value : 'existente'; };

        var aplicarModoTutor = function () {
            var existente = modoTutor() === 'existente';
            blocoTutorExistente.hidden = !existente;
            blocoTutorNovo.hidden = existente;
            blocoTutorNovo.disabled = existente;
            if (!existente) { tutorId.value = ''; $('[data-seletor-texto]', blocoTutorExistente).value = ''; }
            carregarPetsDoTutor();
        };

        var validarEscolhaTutor = function () {
            return modoTutor() === 'existente' && !tutorId.value ? 'Escolha o tutor na lista ou marque "Cliente novo".' : '';
        };

        // passo 2: pet já cadastrado ou novo
        var listaPets = $('[data-lista-pets]', assistente);
        var blocoPetNovo = $('[data-bloco="pet-novo"]', assistente);
        var petSelecionado = listaPets ? listaPets.getAttribute('data-selecionado') : '';

        var aplicarModoPet = function () {
            var escolhido = $('input[name="PetExistenteId"]:checked', assistente);
            var novo = !escolhido || escolhido.value === '';
            blocoPetNovo.hidden = !novo;
            blocoPetNovo.disabled = !novo;
        };

        var validarEscolhaPet = function () {
            return listaPets.children.length && !$('input[name="PetExistenteId"]:checked', assistente) ? 'Escolha um pet ou "Cadastrar outro pet".' : '';
        };

        var renderizarPets = function (pets) {
            if (!pets.length) { listaPets.innerHTML = ''; aplicarModoPet(); return; }
            listaPets.innerHTML = pets.map(function (pet) {
                var marcado = String(pet.id) === petSelecionado ? ' checked' : '';
                return '<label><input type="radio" name="PetExistenteId" value="' + pet.id + '"' + marcado + '> ' + escapar(pet.texto) + '</label>';
            }).join('') + '<label><input type="radio" name="PetExistenteId" value=""> <i class="bi bi-plus-circle" aria-hidden="true"></i> Cadastrar outro pet</label>';
            aplicarModoPet();
        };

        function carregarPetsDoTutor() {
            if (!listaPets) return;
            if (modoTutor() !== 'existente' || !tutorId.value) { renderizarPets([]); return; }
            buscarJson('/Busca/PetsDoTutor?tutorId=' + tutorId.value).then(renderizarPets);
        }

        $$('input[name="modoTutor"]', assistente).forEach(function (radio) { radio.addEventListener('change', aplicarModoTutor); });
        tutorId.addEventListener('change', function () { petSelecionado = ''; carregarPetsDoTutor(); });
        assistente.addEventListener('change', function (evento) { if (evento.target.name === 'PetExistenteId') aplicarModoPet(); });

        aplicarModoTutor();
        mostrarPasso(atual, false);
    }
})();
