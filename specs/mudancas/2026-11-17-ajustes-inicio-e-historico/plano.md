# Plano — Ajustes no Início e no Histórico de Tarefas

Spec: [spec.md](spec.md)

## Abordagem

Seis branches independentes, todas a partir da `main`, uma por item, na ordem abaixo (da mais
simples para a mais delicada). Nenhuma depende da outra; os itens A, B, E e F mexem no mesmo
arquivo (`DashboardDAO.cs`), em trechos diferentes, então o merge é na ordem e sem conflito
esperado. A versão (1.15.0) vai numa branch própria, por último, como manda o CLAUDE.md.

### A. `fix/inicio-atraso-sem-concluidas`

Nas CTEs `Atrasadas` de **Em Atraso — Execução** e **Em Atraso — Teste**, acrescentar
`LEFT JOIN Etapa etx ON etx.Id = tx.EtapaId` e `AND COALESCE(etx.EhEtapaFinal, 0) = 0`. É o mesmo
corte que Carga por Colaborador e Por Prioridade já usam. O filtro fica dentro da CTE (e não no
`WHERE` final) para o total (`COUNT(*) OVER ()`) e a lista saírem do mesmo universo.

### B. `fix/inicio-ranking-texto-credito`

Só texto: a `descricao` do card "Tarefas Concluídas por Colaborador" no `dashboard/page.js`.
O comentário do SQL (`ObterColaboradoresAsync`) já descreve o crédito por elaboração; fica como está.

### C. `fix/rel-historico-ordem-colaborador`

No `useMemo` de `colaboradores` do Histórico de Tarefas, ordenar com
`localeCompare(..., 'pt-BR', { sensitivity: 'base' })`, igual ao Kanban. A lista ordenada serve
ao Combobox e à busca do nome no resumo de filtros; "Todos" continua sendo posto à frente na
montagem das opções.

### D. `feat/rel-historico-etapa-atual`

No `ObterRelatorioHistoricoAsync`, o filtro de etapa troca `h.EtapaId = @EtapaId` (etapa do
evento) por `pt.EtapaId = @EtapaId` (etapa atual da tarefa). As três montagens da consulta
(normal, histórico completo e última ocorrência) já têm `pt` no escopo dos filtros.

Como a tela sempre pede `historicoCompleto`, o efeito é:

- **Agrupada:** as tarefas que estão hoje na etapa, com a história inteira (CA-D4);
- **Detalhada:** todos os eventos do período dessas tarefas, de qualquer etapa (o
  `DentroDoFiltro` passa a marcar todos, porque o filtro deixou de olhar o evento).

O único chamador que manda `etapaId` é o Histórico de Tarefas (a Comissão não usa o filtro), então
a troca não vaza para outro relatório. Tarefa em Atendimento tem `EtapaId` nulo e cai fora sozinha
(CA-D3). Na tela: texto do tour e do resumo de filtros das exportações ("Etapa atual: …").

### E. `feat/inicio-ordem-cliente`

- **Servidor:** `OrdemTarefas(ordem, usarNomeFantasia)` passa a ter dois ramos:
  - `recentes`: como hoje;
  - qualquer outro valor (inclusive `etapa`, que pode vir de navegador antigo): nome exibido do
    cliente (`COALESCE(NULLIF(cli.NomeFantasia, ''), cli.Nome)` quando a configuração é nome
    fantasia, `cli.Nome` quando não), depois `t.DataCadastro DESC` e `t.Id` como desempate final.
  A atribuição de `OrdemTarefaEtapaColaborador` desce para depois do cálculo de
  `usarNomeFantasia` em `ObterDashboardAsync`.
- **Por Etapa (CA-E5):** hoje o `ROW_NUMBER` dessa lista usa uma ordem fixa (colaborador e
  título) e **não** segue o seletor, ao contrário do que a 1.14.0 pediu (CA-7 de
  `inicio-kanban-cliente-negrito-data`). Passa a usar a ordem escolhida dentro de cada faixa. A
  ordem das faixas entre si (Reservado, etapas na ordem do fluxo) não muda.
- **Tela:** o seletor passa a ter `cliente` ("Cliente") e `recentes`. O padrão de
  `usePreferenciaLocal('asm_inicio_ordem', …)` vira `cliente`, e o valor lido é normalizado:
  qualquer coisa diferente de `recentes` vale `cliente` (CA-E3, quem tinha `etapa` guardado).
- A constante `OrdemEtapaColaborador` continua, só como desempate de `recentes`. Todas as listas
  de tarefa que já seguiam o seletor (indicadores, Por Prioridade, Carga por Colaborador)
  continuam seguindo, agora com a ordem por cliente.

### F. `fix/inicio-sem-tarefas-ignora-concluidas`

Na consulta de `SemTarefasAtribuidas` (`ObterColaboradoresAsync`), o `NOT EXISTS` ganha
`LEFT JOIN Etapa e ON e.Id = t.EtapaId` e `AND COALESCE(e.EhEtapaFinal, 0) = 0`, o mesmo universo
da Carga por Colaborador (CA-F4). Texto da descrição do card no `dashboard/page.js`.

## Arquivos afetados

| Arquivo | O que muda | Itens |
|---|---|---|
| `Backend/API/DB/DAOs/DashboardDAO.cs` | Corte da etapa final nos atrasos e no "sem tarefas"; ordem por cliente; Por Etapa segue a ordem escolhida | A, E, F |
| `Frontend/src/app/dashboard/page.js` | Textos dos cards; opções e padrão do seletor de ordem | B, E, F |
| `Backend/API/DB/DAOs/ProjetosDAO.cs` | Filtro de etapa pela etapa atual da tarefa | D |
| `Frontend/src/app/relatorios/projetos-historico/page.js` | Colaboradores ordenados; textos do tour e do resumo de filtros | C, D |
| `Frontend/src/versao.js` | 1.15.0 + changelog, em branch própria, por último | — |
| `specs/` | Fechamento: regras novas no domínio, spec marcada `implementada` | todos |

## Banco de dados

Sem alteração. Nenhuma migration nem backfill.

## Riscos e pontos de atenção

- **A:** o número dos cards de atraso cai. É o esperado. Projeto concluído já ficava de fora.
- **D:** muda o significado de um filtro que já existe. Quem filtrava por etapa para ver "o que
  passou por Elaboração no mês" perde essa leitura. Foi o pedido; o tour e o resumo de filtros
  passam a dizer "etapa atual" para não haver engano em PDF antigo × novo.
- **D:** a ação "Reservada" combinada com uma etapa: a linha Reservada continua sendo reconhecida
  pelo evento, então filtrar Elaboração + Reservada traz as tarefas que estão hoje na elaboração e
  tiveram a entrada Reservada no período. Coerente, nada a fazer.
- **E:** a ordem é usada no `ROW_NUMBER` que corta a lista no limite, então trocar a ordem muda
  **quais** tarefas aparecem quando há mais que o limite. É o comportamento de hoje com
  "recentes"; vale também para "cliente".
- **E:** a ordem por nome fantasia depende da configuração; se ela mudar, a ordem muda junto (a
  consulta já recebe a configuração a cada chamada).
- **F:** o ranking "sem tarefas" e a Carga por Colaborador passam a ser complementares; conferir
  que ninguém aparece nos dois.
- Nenhuma regra nova reconhece etapa por nome: todos os cortes usam a flag `EhEtapaFinal`.

## Fechamento

Não existem ainda `dominio/dashboard.md` nem `dominio/relatorios-historico.md`. Para não perder as
regras, o fechamento cria os dois arquivos só com as regras desta mudança (prefixos `DSH` e
`HIS`), seção "Regras" e "Onde está no código", e os registra na tabela de `specs/README.md`
como "parcial". O restante de cada área continua "a escrever".

## Como verificar

Sem testes automatizados no projeto. Build (`dotnet build Backend/ASMTasks-Backend.sln` e
`npm run build` em `Frontend/`) em cada branch, mais o roteiro abaixo na tela, com dados de
homologação.

- **CA-A1/A2** — Tarefa em Concluído com tempo acima do previsto (a "Controle de caixas plásticas"
  do print): some dos dois cards, no número e na lista do hover.
- **CA-A3** — Mover essa tarefa de volta para Atualização: volta a aparecer.
- **CA-A4** — Tarefa atrasada em Elaboração e em Atendimento continuam aparecendo.
- **CA-B1** — Ler a descrição do card.
- **CA-B2/B3** — Escolher um colaborador no ranking e comparar o total com o Relatório de Comissão
  do mesmo período, filtrado por ele.
- **CA-C1/C2** — Abrir o filtro Colaborador: ordem alfabética, "Álvaro" depois de "Alan", "Todos"
  no topo; digitar na busca mantém a ordem.
- **CA-D1/D2** — Filtrar Elaboração: só tarefas hoje em Elaboração; uma que está em Aprovação e
  passou pela Elaboração no período só aparece com Aprovação.
- **CA-D3** — Tarefa em Atendimento não aparece com nenhuma etapa.
- **CA-D4** — Agrupada: o bloco da tarefa traz todas as etapas. Detalhada: eventos de outras
  etapas também aparecem.
- **CA-D5** — Filtrar Elaboração traz Reservadas e iniciadas.
- **CA-D6** — Exportar PDF e Excel: resumo de filtros diz "Etapa atual"; abrir o tour.
- **CA-E1/E2** — Seletor com "Cliente" e "Mais recentes primeiro"; em Cliente, listas por nome de
  cliente; trocar a configuração para nome fantasia e conferir a ordem.
- **CA-E3** — Com `asm_inicio_ordem = "etapa"` no localStorage, a tela abre em Cliente; com
  `"recentes"`, continua em Mais recentes.
- **CA-E4** — Lista do hover de um indicador segue a ordem.
- **CA-E5** — Por Etapa: dentro de cada faixa, ordem por cliente; faixas na ordem do fluxo.
- **CA-F1/F2** — Colaborador só com tarefas em Concluído aparece no card; ao mover uma delas para
  Atualização, sai.
- **CA-F3** — Ler a descrição do card.
- **CA-F4** — Ninguém aparece ao mesmo tempo no card e na Carga por Colaborador.
