(() => {
    'use strict';

    const form = document.getElementById('formFiltro');
    const inputComp = document.getElementById('competencia');
    const btnBuscar = document.getElementById('btnBuscar');
    const statusEl = document.getElementById('status');
    const textarea = document.getElementById('resultado');
    const btnEdit = document.getElementById('btnEdit');
    const btnSave = document.getElementById('btnSave');
    const btnDownload = document.getElementById('btnDownload');
    const btnDownloadJson = document.getElementById('btnDownloadJson');
    const btnConfig = document.getElementById('btnConfig');

    // Estado da tela
    let jsonSalvo = '';              // último JSON válido (da API ou salvo após edição)
    let competenciaConsultada = '';  // competência da última busca (aaaa-MM)
    let temDados = false;
    let editando = false;

    function setStatus(mensagem, tipo = '') {
        statusEl.textContent = mensagem;
        statusEl.className = 'status' + (tipo ? ' ' + tipo : '');
    }

    function atualizarBotoes() {
        const podeBaixar = temDados && !editando;
        btnEdit.disabled = !temDados;
        btnEdit.textContent = editando ? 'Cancel' : 'Edit';
        btnSave.disabled = !(temDados && editando);
        btnDownload.disabled = !podeBaixar;
        btnDownloadJson.disabled = !podeBaixar;
    }

    function entrarModoEdicao() {
        editando = true;
        textarea.readOnly = false;
        textarea.focus();
        setStatus('Modo de edição: altere o JSON e clique em Save.');
        atualizarBotoes();
    }

    function sairModoEdicao() {
        editando = false;
        textarea.readOnly = true;
        atualizarBotoes();
    }

    // ---------- Validação centralizada de JSON ----------
    function validarJson(conteudo) {
        if (!conteudo || !conteudo.trim()) {
            return { valido: false, erro: 'O conteúdo JSON está vazio.' };
        }

        try {
            const dados = JSON.parse(conteudo);
            return {
                valido: true,
                dados,
                formatado: JSON.stringify(dados, null, 2)
            };
        } catch (erro) {
            return {
                valido: false,
                erro: `JSON inválido: ${erro.message}`
            };
        }
    }

    // ---------- Nome e geração do arquivo ----------
    // Nome no padrão do app de console: MMAAAA + 5 primeiros dígitos do CNPJ
    function nomeDoArquivo(dados, extensao) {
        const primeiroItem = Array.isArray(dados) ? dados[0] : dados;
        const cnpj = String(primeiroItem?.cnpjEmitente ?? '').slice(0, 5);
        const [ano, mes] = competenciaConsultada.split('-');

        if (ano && mes && cnpj) {
            return `${mes}${ano}${cnpj}.${extensao}`;
        }

        return `nfce.${extensao}`;
    }

    function baixarArquivo(extensao, mimeType) {
        const validacao = validarJson(jsonSalvo);

        if (!validacao.valido) {
            setStatus(validacao.erro, 'erro');
            return;
        }

        const nomeArquivo = nomeDoArquivo(validacao.dados, extensao);
        const blob = new Blob([validacao.formatado], { type: mimeType });
        const url = URL.createObjectURL(blob);

        const link = document.createElement('a');
        link.href = url;
        link.download = nomeArquivo;
        document.body.appendChild(link);
        link.click();
        link.remove();

        setTimeout(() => URL.revokeObjectURL(url), 0);
        setStatus(`Arquivo ${nomeArquivo} gerado com sucesso.`, 'ok');
    }

    // ---------- Buscar: GET /api/nfce?competencia=aaaa-MM ----------
    form.addEventListener('submit', async (e) => {
        e.preventDefault();

        if (editando && textarea.value !== jsonSalvo &&
            !confirm('Existem edições não salvas. Deseja descartá-las e buscar novamente?')) {
            return;
        }

        const params = new URLSearchParams(new FormData(form)).toString();

        btnBuscar.disabled = true;
        setStatus('Buscando NFC-e...');

        try {
            const response = await fetch(`/api/nfce?${params}`, {
                method: 'GET',
                headers: { 'Accept': 'application/json' }
            });

            if (!response.ok) {
                throw new Error(`A API respondeu com status ${response.status}.`);
            }

            const notas = await response.json();
            const lista = Array.isArray(notas) ? notas : [];

            jsonSalvo = JSON.stringify(notas, null, 2);
            competenciaConsultada = inputComp.value;
            temDados = lista.length > 0;

            textarea.value = jsonSalvo;
            sairModoEdicao();

            setStatus(temDados
                ? `${lista.length} nota(s) encontrada(s).`
                : 'Nenhuma NFC-e encontrada para a competência informada.',
                temDados ? 'ok' : '');
        } catch (erro) {
            console.error(erro);
            temDados = false;
            jsonSalvo = '';
            textarea.value = '';
            sairModoEdicao();
            setStatus(erro instanceof TypeError
                ? 'Não foi possível conectar à API. Verifique se ela está em execução.'
                : erro.message, 'erro');
        } finally {
            btnBuscar.disabled = false;
        }
    });

    // ---------- Edit / Cancel ----------
    btnEdit.addEventListener('click', () => {
        if (!editando) {
            entrarModoEdicao();
        } else {
            textarea.value = jsonSalvo; // descarta as alterações
            sairModoEdicao();
            setStatus('Edição cancelada.');
        }
    });

    // ---------- Save ----------
    btnSave.addEventListener('click', () => {
        const validacao = validarJson(textarea.value);

        if (!validacao.valido) {
            setStatus(validacao.erro, 'erro');
            return;
        }

        jsonSalvo = validacao.formatado;
        textarea.value = jsonSalvo;
        sairModoEdicao();
        setStatus('Alterações salvas. O JSON está válido e pronto para download.', 'ok');
    });

    // ---------- Downloads ----------
    btnDownload.addEventListener('click', () => {
        baixarArquivo('txt', 'text/plain;charset=utf-8');
    });

    btnDownloadJson.addEventListener('click', () => {
        baixarArquivo('json', 'application/json;charset=utf-8');
    });

    // Ícone de configurações (sem função por enquanto)
    btnConfig?.addEventListener('click', () => { /* reservado */ });

    atualizarBotoes();
})();