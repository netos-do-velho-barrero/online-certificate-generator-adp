# Gerador de Certificados Online

## Requisitos e guia de implementação

Este documento é a referência funcional e técnica do projeto. Ele transforma a
especificação em orientações práticas para desenvolvimento, revisão e entrega.

## 1. Visão geral

O sistema deve:

1. cadastrar e autenticar usuários;
2. cadastrar e consultar cursos;
3. receber uma lista de alunos para certificação;
4. gerar um PDF individual por aluno;
5. acompanhar o processamento;
6. disponibilizar os PDFs em um arquivo ZIP.

Como PDF e ZIP podem demorar, a solicitação deve ser aceita rapidamente e o
processamento executado de forma assíncrona por mensageria.

### Fora do escopo inicial

- edição ou exclusão de cursos;
- edição de certificados gerados;
- envio por email;
- customização visual do certificado;
- formatos além de PDF e ZIP;
- administração avançada de usuários e permissões.

## 2. Arquitetura

| Projeto | Responsabilidade |
| --- | --- |
| `GeradorCertificadosOnline.Dominio` | Entidades, objetos de valor, regras e abstrações |
| `GeradorCertificadosOnline.Aplicacao` | Commands, Queries, Handlers, validações e orquestração |
| `GeradorCertificadosOnline.Infraestrutura` | Banco, repositórios, mensageria, arquivos e integrações |
| `GeradorCertificadosOnline.WebApi` | HTTP, autenticação, contratos e composição da aplicação |

A API não deve executar regras de negócio. Ela recebe um contrato, chama um
caso de uso e converte o resultado para HTTP. A Aplicação não deve conhecer
detalhes de banco, RabbitMQ, PDF ou armazenamento físico.

### Dependências permitidas

```text
WebApi
├── Aplicacao ────────> Dominio
└── Infraestrutura ────> Dominio
```

Não criar dependências circulares nem referências do Domínio para outras
camadas.

### Organização por módulo

Manter os módulos próximos nas camadas correspondentes:

```text
Dominio/Modulos/{Modulo}/
Aplicacao/Modulos/{Modulo}/
Infraestrutura/Modulos/{Modulo}/
Api/Modulos/{Modulo}/
```

Os módulos são `Usuarios`, `Cursos` e `Certificados`. Tipos realmente
compartilhados devem ficar em `Compartilhado`.

## 3. Convenções

- usar `PascalCase` para tipos e membros públicos e `camelCase` para
  parâmetros e variáveis;
- usar `record` ou `record class` para requests e responses;
- nunca expor entidades diretamente pela API;
- definir um tipo único para identificadores, preferencialmente `Guid`;
- armazenar datas em UTC e serializar em ISO 8601;
- separar Commands, Queries e Handlers;
- usar DI do .NET, sem instanciar serviços com `new` dentro dos casos de uso;
- encapsular registros relacionados em métodos de extensão;
- manter regras no Domínio ou em validadores apropriados, não em controllers.

## 4. Módulo 1 — Usuários e Autenticação

**Responsável: Pedro.**

### Requisitos e regras

- cadastrar por email e senha;
- autenticar por email e senha;
- emitir JWT em login válido;
- validar JWT nas rotas protegidas;
- email obrigatório, válido e único;
- senha com pelo menos 8 caracteres, um dígito e um caractere não alfanumérico;
- armazenar apenas hash seguro da senha;
- manter credenciais e chave JWT fora do código;
- deixar somente cadastro e login públicos.

O login deve retornar mensagem genérica para credenciais inválidas, sem revelar
se o email existe. O token deve conter apenas claims necessárias e ter expiração.

### `POST /auth/cadastro`

```json
{
  "email": "aluno@exemplo.com",
  "senha": "Senha@123"
}
```

Retorna `201 Created`. Falhas de validação retornam `400 Bad Request` e email
duplicado retorna `409 Conflict`. Nunca retornar senha ou hash.

### `POST /auth/login`

```json
{
  "email": "aluno@exemplo.com",
  "senha": "Senha@123"
}
```

Response sugerida:

```json
{
  "accessToken": "jwt",
  "expiresAt": "2026-09-16T20:00:00Z"
}
```

Retorna `200 OK` ou `401 Unauthorized`.

## 5. Módulo 2 — Cursos

**Responsável: Marco.**

### Modelo mínimo

| Campo | Regra |
| --- | --- |
| `id` | Identificador único |
| `nome` | Obrigatório, máximo de 200 caracteres |
| `descricao` | Opcional, máximo de 500 caracteres |
| `cargaHoraria` | Inteiro maior que zero |
| `dataConclusao` | Obrigatória |

### `POST /cursos`

```json
{
  "nome": "Introdução ao C#",
  "descricao": "Fundamentos de C# e .NET.",
  "cargaHoraria": 40,
  "dataConclusao": "2026-09-16"
}
```

Retorna `201 Created`, preferencialmente com `Location` para
`/cursos/{cursoId}`. Sem JWT, retorna `401 Unauthorized`; dados inválidos,
`400 Bad Request`.

### `GET /cursos/{cursoId}`

Retorna `200 OK`. Curso inexistente retorna `404 Not Found`.

O módulo deve entregar uma consulta estável para que Certificados valide a
existência do curso sem duplicar o modelo ou a regra de negócio.

## 6. Módulo 3 — Geração de Certificados

**Responsável: Pedro.** Depende do contrato do módulo de Cursos.

### Requisitos

- aceitar um ou mais alunos;
- persistir um certificado por aluno;
- gerar PDF com nome, curso, carga horária e data;
- registrar caminho e data de geração;
- registrar sucesso ou falha individual;
- consultar status geral;
- gerar ZIP e permitir download.

### `POST /cursos/{cursoId}/certificados`

```json
{
  "alunos": [
    { "nome": "Ana Souza" },
    { "nome": "Bruno Lima" }
  ]
}
```

A lista deve ter pelo menos um item e cada nome deve ser obrigatório e ter no
máximo 200 caracteres. O curso deve existir. Um curso com processamento não
finalizado não pode aceitar outro lote: retornar `409 Conflict`.

Quando aceita, a API cria o processamento, publica uma mensagem e retorna
`202 Accepted`, sem esperar a geração:

```json
{
  "processamentoId": "id",
  "status": "Pendente"
}
```

### Estados

```text
Pendente -> Gerando Certificados -> Gerando Zip -> Concluído
Pendente/Gerando Certificados/Gerando Zip -> Falha
```

Cada certificado pode estar `Pendente`, `Gerando`, `Gerado` ou `Falha`.
O ZIP não deve ser marcado como pronto antes da etapa de geração. Se houver
falha individual, seguir a política definida pelo time; a recomendação é
marcar o processamento como `Falha` e manter os resultados individuais para
diagnóstico.

### Endpoints restantes

| Método e rota | Sucesso | Comportamento |
| --- | --- | --- |
| `GET /cursos/{cursoId}/status` | `200 OK` | Retorna status e contadores |
| `GET /cursos/{cursoId}/certificados` | `200 OK` | Lista contratos de leitura |
| `GET /cursos/{cursoId}/certificados/download` | `200 OK` | Retorna `application/zip` |

O download só ocorre com ZIP pronto. Antes disso, usar `409 Conflict` ou
`404 Not Found`, conforme decisão documentada. Nunca expor caminho físico.

## 7. Processamento assíncrono

Usar MassTransit com RabbitMQ/CloudAMQP. A mensagem deve transportar
identificadores e dados pequenos, nunca arquivos grandes.

O consumidor precisa ser idempotente: reprocessar a mesma mensagem não pode
duplicar certificados nem corromper status. Usar identificador do processamento,
restrições de unicidade e operações transacionais.

Configurar retry para falhas transitórias, limite de tentativas, fila de erro,
logs com `processamentoId` e `certificadoId` e atualização de status em cada
etapa.

O fluxo recomendado é:

1. carregar curso e processamento;
2. selecionar certificados ainda não concluídos;
3. gerar cada PDF;
4. persistir o resultado individual;
5. criar o ZIP;
6. persistir o caminho lógico do ZIP;
7. marcar o processamento como concluído.

QuestPDF ou biblioteca similar deve ficar atrás de uma abstração da
Infraestrutura. Arquivos devem ficar fora de diretórios públicos e nomes devem
ser gerados pelo sistema para impedir path traversal.

## 8. Persistência mínima

### Usuário

Identificador, email único, hash da senha e datas de auditoria.

### Curso

Identificador, nome, descrição, carga horária, conclusão e datas de auditoria.

### Processamento

Identificador, curso, status, caminho do ZIP e datas de criação, início,
conclusão e falha.

### Certificado

Identificador, processamento, curso, nome do aluno, status, caminho do PDF,
data de geração e erro público quando aplicável.

Alterações devem ser entregues com migrations versionadas. Nunca versionar
senhas, tokens, secrets ou caminhos dependentes do ambiente.

## 9. HTTP e tratamento de erros

Usar `ProblemDetails` em erros:

```json
{
  "type": "https://httpstatuses.com/400",
  "title": "A requisição é inválida.",
  "status": 400,
  "detail": "Existem campos inválidos.",
  "instance": "/cursos"
}
```

Mapeamento mínimo:

| Status | Uso |
| --- | --- |
| `201` | Recurso criado |
| `202` | Processamento aceito |
| `200` | Consulta ou download concluído |
| `400` | Entrada inválida |
| `401` | JWT ausente ou inválido |
| `404` | Recurso inexistente |
| `409` | Email duplicado ou processamento em andamento |
| `500` | Falha inesperada sem detalhes internos |

Não retornar stack trace, caminhos físicos, senha, hash ou token em respostas.

## 10. Segurança e configuração

- usar variáveis de ambiente, Secret Manager ou cofre de secrets;
- nunca versionar chave JWT, senha de banco ou credenciais do broker;
- habilitar HTTPS em ambientes publicados;
- limitar tamanho e conteúdo dos requests;
- não confiar em nomes de arquivos enviados;
- não registrar credenciais ou tokens;
- proteger todos os endpoints, exceto cadastro e login;
- revisar CORS, retenção de arquivos e permissões de armazenamento.

## 11. Estratégia de testes

### UnitTests

Validar regras isoladas: senha, email, limites de curso e aluno, estados,
lote vazio e bloqueio de solicitação concorrente.

### IntegrationTests

Validar persistência, constraints, migrations, mensageria e armazenamento.

Os testes de unidade cobrem regras de domínio. Os testes de integração cobrem
persistência, autenticação, contratos HTTP e efeitos persistidos quando o fluxo
exigir infraestrutura.

## 12. Divisão de trabalho

### Pedro

Implementa Usuários e Autenticação e Geração de Certificados: cadastro, login,
JWT, políticas de autenticação, processamento, PDFs, ZIP, mensagens,
consumidor, status, download e testes dos módulos 1 e 3.

Pedro deve alinhar com Marco o tipo do identificador, o contrato de Curso e o
comportamento para curso inexistente.

### Marco

Implementa Cursos: entidade, persistência, cadastro, consulta, validações,
contratos e testes do módulo 2.

Marco deve entregar uma consulta estável de curso e documentar sua resposta.

### Acordos antes do desenvolvimento paralelo

Registrar em issue ou decisão técnica:

- tipo dos identificadores;
- nomes JSON;
- formato de datas;
- formato de erro;
- política para falha parcial;
- comportamento de download antes da conclusão;
- concorrência por curso;
- armazenamento lógico de arquivos.

### Contrato compartilhado proposto

Esta é a proposta inicial para revisão de Pedro e Marco:

- **Identificadores:** `Guid`, serializado como texto em JSON.
- **JSON:** propriedades em `camelCase`, como `cursoId`, `cargaHoraria` e
  `dataConclusao`.
- **Datas:** ISO 8601; datas de negócio como `yyyy-MM-dd` e timestamps como
  UTC, por exemplo `2026-09-16T20:00:00Z`.
- **Erros:** `ProblemDetails`, com `400` para validação, `401` para
  autenticação, `404` para recurso inexistente e `409` para conflito.
- **Curso:** o contrato mínimo para Certificados contém `id`, `nome`,
  `cargaHoraria` e `dataConclusao`.
- **Curso inexistente:** a solicitação de certificados retorna `404 Not Found`.
- **Concorrência:** apenas um processamento não finalizado por curso; uma nova
  solicitação retorna `409 Conflict`.
- **Falha parcial:** qualquer falha na geração impede `Concluído`; o
  processamento fica como `Falha` e os resultados individuais permanecem
  consultáveis.
- **Download:** o ZIP só pode ser baixado quando o processamento estiver
  `Concluído`; antes disso retorna `409 Conflict`.
- **Estados gerais:** `Pendente`, `GerandoCertificados`, `GerandoZip`,
  `Concluido` e `Falha`.
- **Estados individuais:** `Pendente`, `Gerando`, `Gerado` e `Falha`.
- **Mensageria:** a mensagem carrega `processamentoId` e `cursoId`; os PDFs e
  o ZIP não são transportados pela fila.
- **Arquivos:** a API expõe download por identificador, nunca caminho físico.

Pedro e Marco devem revisar esta seção antes de criar os contratos definitivos.
Se houver alteração, atualizar este documento e a issue de contratos antes do
desenvolvimento paralelo.

## 13. Issues, commits e Project Board

Criar issues pequenas, com objetivo, dependências, critérios de aceite, casos
de erro e definição de pronto. Sugestões:

- `[Arquitetura] Definir contratos compartilhados`;
- `[Usuários] Implementar cadastro`;
- `[Usuários] Implementar login e JWT`;
- `[Cursos] Implementar cadastro`;
- `[Cursos] Implementar consulta`;
- `[Certificados] Criar solicitação e status`;
- `[Certificados] Implementar consumidor`;
- `[Certificados] Gerar PDFs`;
- `[Certificados] Gerar ZIP e download`;
- `[Qualidade] Criar testes por camada`;
- `[DevOps] Publicar API, banco e broker`.

Fluxo do board:

```text
Backlog -> Ready -> In Progress -> Review -> Done
```

Usar labels de módulo, camada e tipo. Mover o card conforme o trabalho avança.

Commits devem ser pequenos, frequentes e descritivos:

```text
feat(auth): add user registration flow
feat(courses): add course creation validation
feat(certificates): process pdf batch asynchronously
test(courses): cover course validation rules
docs: describe certificate processing flow
```

## 14. Publicação

O ambiente publicado deve conter API, banco com migrations, RabbitMQ ou
CloudAMQP, armazenamento persistente de PDFs/ZIPs, secrets, logs e
observabilidade.

Checklist:

- configurar variáveis de ambiente;
- validar banco e broker;
- aplicar migrations;
- habilitar HTTPS;
- revisar CORS e permissões;
- garantir armazenamento persistente;
- configurar health check;
- confirmar que nenhum secret está no Git;
- executar testes no pipeline antes do deploy.

## 15. Critérios de aceite

Uma entrega está pronta quando:

- atende requisitos e regras do módulo;
- não expõe entidades nem dados sensíveis;
- usa `ProblemDetails`;
- usa DI e respeita as camadas;
- possui testes adequados;
- possui migrations e documentação necessárias;
- build e testes passam;
- processamento assíncrono é idempotente;
- logs permitem rastrear um lote sem revelar credenciais;
- issue e Project Board estão atualizados.

## 16. Ordem recomendada

1. alinhar contratos entre Pedro e Marco;
2. implementar domínio e persistência de Cursos;
3. implementar cadastro e login;
4. proteger rotas com JWT;
5. implementar solicitação e consulta de processamento;
6. configurar MassTransit e RabbitMQ;
7. implementar PDFs;
8. implementar ZIP e download;
9. adicionar testes unitários de domínio e testes de integração;
10. configurar pipeline, board e publicação.

## 17. Development branches

Use one branch per developer and bounded responsibility:

### Pedro

```text
feat/pedro-users-certificates
```

This branch contains the Users and Authentication module and the Certificate
Generation module. Keeping both related responsibilities together allows Pedro
to implement authentication, authorization, asynchronous processing, PDF/ZIP
generation, status tracking and download as one coherent flow.

### Marco

```text
feat/marco-courses
```

This branch contains the Courses module, including course creation, querying,
validation, persistence and tests. The branch remains isolated so Marco can
deliver the course contract that the Certificate Generation module depends on.

Both branches should start from an updated `main` branch. Changes must be
merged through pull requests, and shared contracts such as identifiers,
property names, dates, errors and processing states should be agreed upon
before parallel implementation. Do not create a single branch containing both
developers' work; this keeps reviews, conflict resolution and rollback simple.
