# Ajustes no Início e no Histórico de Tarefas

- **Status:** aprovada (Daniel, 2026-11-17)
- **Pedido por:** Daniel, 2026-11-17 (com prints da tela); item F em 2026-11-18
- **Branches:** uma por item, indicada em cada seção
- **Versão:** <!-- preencher ao implementar -->
- **Specs de domínio afetadas:** `dashboard.md` (a escrever); `relatorios-historico.md` (a escrever);
  `dominio/comissao.md` (COM-10, COM-11, só citadas)

Seis ajustes pequenos e independentes, numa spec só. Cada seção vira uma branch.

## A. Tarefa concluída não fica "Em atraso" — `fix/inicio-atraso-sem-concluidas`

Os indicadores **Em Atraso — Execução** e **Em Atraso — Teste** do Início contam tarefas que já
estão na etapa final. No print, "Controle de caixas plásticas" (Concluído · Ociosa) aparece em
Em Atraso — Execução. Hoje o único corte é o projeto estar ativo e não concluído; a etapa atual
da tarefa não é olhada. Tarefa concluída não tem mais o que atrasar.

- **CA-A1.** Dada uma tarefa que está hoje na etapa final, com o tempo de elaboração acima do
  previsto, então ela não entra em Em Atraso — Execução: nem no número, nem na lista.
- **CA-A2.** O mesmo vale para Em Atraso — Teste.
- **CA-A3.** Se a tarefa sair da etapa final (reaberta) e continuar acima do previsto, ela volta
  a aparecer nos dois indicadores, como hoje.
- **CA-A4.** Tarefas que não estão na etapa final continuam como hoje, inclusive as que estão no
  Atendimento.

## B. Ranking de concluídas credita quem elaborou — `fix/inicio-ranking-texto-credito`

O pedido é que **Tarefas Concluídas por Colaborador** conte para quem elaborou, não para quem
concluiu. **O cálculo já faz isso desde a 1.14.0**: usa o mesmo crédito do Relatório de Comissão
(COM-10 e COM-11), que é quem saiu da etapa de elaboração por último até o fim do período. O que
ficou para trás é o texto do card, que ainda diz "por quem estava com a etapa".

- **CA-B1.** A descrição do card passa a dizer que o crédito é de quem elaborou a tarefa (texto
  sugerido: "Tarefas que chegaram na etapa final no período, por quem elaborou.").
- **CA-B2.** Dada uma tarefa elaborada por Fulano, testada por Beltrano e movida para Concluído
  por Beltrano dentro do período, então ela conta 1 para Fulano e nada para Beltrano.
- **CA-B3.** O total de cada colaborador no ranking bate com a quantidade de tarefas do Relatório
  de Comissão no mesmo período e para o mesmo colaborador, crédito por elaboração.

## C. Colaboradores em ordem alfabética no Histórico de Tarefas — `fix/rel-historico-ordem-colaborador`

No filtro **Colaborador** do Relatório de Histórico de Tarefas a lista vem na ordem do cadastro
(no print: Daniel, Anderson, João Carlos, Elizabeth…). As outras telas (Kanban, Comissão) já
ordenam por nome.

- **CA-C1.** A lista do filtro Colaborador vem em ordem alfabética, sem diferença de maiúsculas
  e acentos ("Álvaro" fica entre "Alan" e "Anderson"). "Todos" continua no topo.
- **CA-C2.** A busca dentro do filtro continua funcionando e mantém a ordem.

## D. Filtro de etapa do Histórico: só o que está na etapa hoje — `feat/rel-historico-etapa-atual`

Hoje o filtro **Etapa** traz os eventos que aconteceram naquela etapa, então aparece também tarefa
que já passou dela. O pedido é ver só as tarefas que **estão** na etapa: se já passou ou ainda não
chegou, não aparece.

- **CA-D1.** Com uma etapa escolhida, entram só as tarefas cuja etapa **atual** é a escolhida,
  qualquer que seja o período consultado (mesma ideia da Situação, que também é a de hoje).
- **CA-D2.** Dada uma tarefa que está hoje em Aprovação e passou por Elaboração no período, quando
  o filtro é Elaboração, então ela não aparece; quando é Aprovação, aparece.
- **CA-D3.** Tarefa que ainda está no Atendimento (sem etapa) não aparece com nenhuma etapa
  escolhida.
- **CA-D4.** Para cada tarefa que entra, o relatório mostra os eventos dela no período, com todas
  as etapas, nas duas visões (ver pergunta 3).
- **CA-D5.** Escolhendo a etapa de elaboração, entram tanto as tarefas Reservadas quanto as já
  iniciadas.
- **CA-D6.** O tour e o resumo de filtros das exportações explicam que a etapa é a de hoje.

## E. Ordem das listas do Início: "Cliente" no lugar de "Etapa e colaborador" — `feat/inicio-ordem-cliente`

O seletor de ordem das listas do Início (criado na 1.14.0, CA-5 de
[inicio-kanban-cliente-negrito-data](../2026-10-31-inicio-kanban-cliente-negrito-data/spec.md))
tem "Etapa e colaborador" e "Mais recentes primeiro". A opção "Etapa e colaborador" troca por
"Cliente".

- **CA-E1.** O seletor passa a ter **Cliente** e **Mais recentes primeiro**.
- **CA-E2.** Em **Cliente**, as listas de tarefas ficam em ordem alfabética do nome do cliente,
  como ele aparece na tela (razão social ou nome fantasia, conforme a configuração). Desempate:
  data de lançamento, mais recente primeiro.
- **CA-E3.** **Cliente** é o padrão. Quem tinha "Etapa e colaborador" guardado no navegador passa
  a ver **Cliente**; quem tinha "Mais recentes primeiro" continua com ela.
- **CA-E4.** Como hoje, a ordem vale para todas as listas de tarefas do Início, inclusive a que
  abre ao passar o mouse num indicador, e o corte do limite de itens respeita a ordem.
- **CA-E5.** A lista **Por Etapa** continua separada por etapa; a ordem escolhida vale dentro de
  cada faixa.

## F. Colaborador só com tarefas concluídas aparece "sem tarefas" — `fix/inicio-sem-tarefas-ignora-concluidas`

O card **Colaboradores sem Tarefas Atribuídas** do Início só mostra quem não é responsável por
nenhuma tarefa em projeto ativo, e conta inclusive as tarefas na etapa final. Quem só tem tarefas
em Concluído (projeto ainda aberto) não aparece, embora não tenha nada para fazer.

- **CA-F1.** Dado um colaborador ativo cujas tarefas em projeto ativo estão todas na etapa final,
  então ele aparece no card.
- **CA-F2.** Dado um colaborador com pelo menos uma tarefa fora da etapa final em projeto ativo,
  então ele não aparece, como hoje.
- **CA-F3.** A descrição do card deixa claro que tarefa concluída não conta (texto sugerido:
  "Colaboradores ativos sem nenhuma tarefa em aberto em projeto ativo.").
- **CA-F4.** O card usa o mesmo universo da **Carga por Colaborador** (tudo que não está na etapa
  final): quem aparece aqui não aparece lá, e vice-versa.

## Fora de escopo

- Listas de atendimentos e de projetos do Início: a ordem delas não muda (E).
- Rever o tempo previsto de tarefas que estão no teste ou depois dele (A): só a etapa final sai.
- Mudar a regra de crédito por elaboração (B): ela é a da comissão e continua a mesma.

## Decisões

Todas de 2026-11-17 (Daniel): valem as recomendações.

1. **(A)** Tarefa que já saiu da elaboração e estourou o tempo de execução continua em Em Atraso —
   Execução. Só a etapa final sai.
2. **(B)** Sem caso concreto de crédito errado: o item B é só o ajuste de texto (CA-B1); CA-B2 e
   CA-B3 servem de conferência.
3. **(D)** A tarefa que está na etapa escolhida mostra todos os eventos dela no período (CA-D4).
4. **(D)** Etapa final com Situação "Não concluídas" fica vazio, com a mensagem de vazio de sempre.
5. **(E)** Desempate dentro do mesmo cliente: lançamento, mais recente primeiro (CA-E2).
6. **(E)** "Cliente" é o padrão (CA-E3).
7. **(F)** Item incluído em 2026-11-18 a pedido de Daniel, junto com o pedido do plano.

## Onde está no código

- A, B, E, F: `Backend/API/DB/DAOs/DashboardDAO.cs` (`ObterTarefasAsync`, `ObterColaboradoresAsync`,
  `OrdemTarefas`); `Frontend/src/app/dashboard/page.js` (seletor de ordem, texto do ranking)
- B: `Backend/API/DB/DAOs/TarefaSql.cs` (`CreditoElaboracao`)
- C, D: `Frontend/src/app/relatorios/projetos-historico/page.js`; D também em
  `Backend/API/DB/DAOs/ProjetosDAO.cs` (filtro `h.EtapaId = @EtapaId` do histórico de tarefas)
