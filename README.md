<p align="center">
  <img src="docs/marca/petiatra-logo.svg" alt="Petiatra" width="420">
</p>

# Petiatra: sistema web de gestão para clínicas veterinárias

Projeto das disciplinas **Projeto de Desenvolvimento de Software I e II** da UniFil.

**Autor(es):** _[nome completo]_ e _[nome da dupla, se houver]_

> **Petiatra** = *pet* + *iatra*, do grego *iatrós* (médico), a mesma origem de "pediatra". Ou seja, "médico dos pets".

## Sobre o sistema

O Petiatra ajuda clínicas veterinárias de pequeno porte a:

- cadastrar **tutores** e **pets**;
- agendar e registrar **consultas** sem conflito de horário;
- manter a **carteira de vacinação** de cada pet;
- ver no painel inicial as consultas do dia e as **vacinas atrasadas**.

## Tecnologias

| Camada | Tecnologia |
|---|---|
| Linguagem | C# / .NET 10 (LTS) |
| Arquitetura | ASP.NET Core MVC (Controllers + Razor Views), SOLID, Clean Code |
| Dados | Entity Framework Core 10 + SQLite (trocável por SQL Server Express) |
| Interface | Bootstrap 5.3 responsivo (mobile-first) |
| Testes | xUnit |
| Modelagem | PlantUML |

## Situação

| Etapa | Situação | Onde está |
|---|---|---|
| Parte I: documentação | Concluída | [`docs/entrega/`](docs/entrega/) (PDF) e [`docs/diagramas/`](docs/diagramas/) |
| Parte II: implementação | A fazer | `src/` e `tests/` |

## Estrutura

```
docs/
├── entrega/     PDF da documentação (Parte I)
├── diagramas/   diagramas UML e DER: fontes PlantUML (.puml) e imagens (.png)
└── marca/       logo, ícone e favicons
```

Para gerar novamente as imagens de um diagrama a partir do arquivo `.puml`, use o [PlantUML](https://plantuml.com):

```bash
java -jar plantuml.jar -tpng docs/diagramas/*.puml
```
