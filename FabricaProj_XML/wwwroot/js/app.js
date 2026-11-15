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
        btnEdit.disabled = !temDados;
        btnEdit.textContent = editando ? 'Cancel' : 'Edit';
        btnSave.disabled = !(temDados && editando);
        btnDownload.disabled = !(temDados && !editando);
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

    // ---------- Buscar: GET /api/nfce?competencia=aaaa-MM ----------
    form.addEventListener('submit', async (e) => {
        e.preventDefault(); // evita recarregar a página

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
        try {
            const objeto = JSON.parse(textarea.value); // valida o JSON editado
            jsonSalvo = JSON.stringify(objeto, null, 2);
            textarea.value = jsonSalvo;
            sairModoEdicao();
            setStatus('Alterações salvas. Use Download .txt para baixar o arquivo.', 'ok');
        } catch (erro) {
            setStatus(`JSON inválido: ${erro.message}`, 'erro');
        }
    });

    // ---------- Download .txt ----------
    // Nome no padrão do app de console: MMAAAA + 5 primeiros dígitos do CNPJ
    function nomeDoArquivo() {
        try {
            const lista = JSON.parse(jsonSalvo);
            const cnpj = String(lista[0]?.cnpjEmitente ?? '').slice(0, 5);
            const [ano, mes] = competenciaConsultada.split('-');
            if (ano && mes && cnpj) return `${mes}${ano}${cnpj}.txt`;
        } catch { /* usa o nome padrão */ }
        return 'nfce.txt';
    }

    btnDownload.addEventListener('click', () => {
        const blob = new Blob([jsonSalvo], { type: 'text/plain;charset=utf-8' });
        const url = URL.createObjectURL(blob);

        const link = document.createElement('a');
        link.href = url;
        link.download = nomeDoArquivo(); // sempre .txt
        document.body.appendChild(link);
        link.click();
        link.remove();

        setTimeout(() => URL.revokeObjectURL(url), 0);
    });

    // Ícone de configurações (sem função por enquanto)
    document.getElementById('btnConfig').addEventListener('click', () => { /* reservado */ });

    atualizarBotoes();
})();