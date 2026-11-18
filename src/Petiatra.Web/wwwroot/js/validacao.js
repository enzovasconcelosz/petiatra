// Ajustes da validação no navegador (RNF07). As mesmas regras são repetidas no servidor.
(function ($) {
    'use strict';
    if (!$ || !$.validator) return;

    // campos escondidos dos seletores com busca também são validados;
    // sem validar ao sair do campo, a mensagem não some no meio do clique e o botão não "pula" de lugar
    $.validator.setDefaults({ ignore: ':hidden:not([data-seletor-valor])', onfocusout: false });

    // números no formato brasileiro: "1.234,56" ou "1234,56"
    function paraNumero(valor) {
        var texto = String(valor).trim().replace(/^R\$\s*/, '');
        if (texto.indexOf(',') >= 0) texto = texto.replace(/\./g, '').replace(',', '.');
        return texto === '' ? NaN : Number(texto);
    }

    $.validator.methods.number = function (valor, elemento) {
        return this.optional(elemento) || !isNaN(paraNumero(valor));
    };

    $.validator.methods.range = function (valor, elemento, limites) {
        if (this.optional(elemento)) return true;
        var numero = paraNumero(valor);
        return !isNaN(numero) && numero >= Number(limites[0]) && numero <= Number(limites[1]);
    };

    // CPF: confere os dois dígitos verificadores
    $.validator.addMethod('cpf', function (valor, elemento) {
        if (this.optional(elemento)) return true;
        var cpf = String(valor).replace(/\D/g, '');
        if (cpf.length !== 11 || /^(\d)\1+$/.test(cpf)) return false;
        for (var tamanho = 9; tamanho <= 10; tamanho++) {
            var soma = 0;
            for (var i = 0; i < tamanho; i++) soma += Number(cpf[i]) * (tamanho + 1 - i);
            var resto = (soma * 10) % 11;
            if ((resto === 10 ? 0 : resto) !== Number(cpf[tamanho])) return false;
        }
        return true;
    });
    $.validator.unobtrusive.adapters.addBool('cpf');
})(window.jQuery);
