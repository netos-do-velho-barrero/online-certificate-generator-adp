# Gerador de Certificados Online

Sistema web para cadastrar cursos e automatizar a geração de certificados
individuais em PDF, agrupados em um arquivo ZIP para download.

Este repositório está organizado em camadas e preparado para evoluir com
ASP.NET Core, CQRS/MediatR, persistência em banco de dados e processamento
assíncrono por mensageria.

## Documentação

O documento principal está em
[docs/REQUISITOS-E-GUIA-DE-IMPLEMENTACAO.md](docs/REQUISITOS-E-GUIA-DE-IMPLEMENTACAO.md).
Ele reúne requisitos, contratos HTTP, arquitetura, fluxo de geração,
segurança, testes, divisão de responsabilidades e publicação.

## Estrutura da solução

```text
src/
├── Api/             # Interface HTTP e composição da aplicação
├── Aplicacao/       # Casos de uso, Commands, Queries e Handlers
├── Dominio/         # Entidades, regras e abstrações
└── Infraestrutura/  # Persistência, mensageria e integrações

tests/
├── UnitTests/
└── IntegrationTests/
```

## Pré-requisitos

- .NET SDK 10;
- Docker Desktop;
- secrets configurados fora do código-fonte.

## Ambiente local, desde a instalação

1. Instale o [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
   e o [Docker Desktop](https://www.docker.com/products/docker-desktop/).
2. Clone o repositório e entre na pasta do projeto.
3. Confirme as ferramentas:

   ```powershell
   dotnet --info
   docker --version
   docker compose version
   ```

4. Inicie o PostgreSQL:

   ```powershell
   docker compose up -d postgres
   ```

   O banco estará em `localhost:5432`, com banco
   `gerador_certificados`, usuário `postgres` e senha local padrão `postgres`.
   Cada desenvolvedor terá seu próprio volume e seus próprios dados.

5. Configure o User Secret da API:

   ```powershell
   dotnet user-secrets init --project .\src\Api
   dotnet user-secrets set "ConnectionStrings:PostgresEF" `
     "Host=localhost;Port=5432;Database=gerador_certificados;Username=postgres;Password=postgres" `
     --project .\src\Api
   ```

6. Valide a base:

   ```powershell
   dotnet restore .\GeradorCertificadosOnline.slnx
   dotnet build .\GeradorCertificadosOnline.slnx
   dotnet test .\tests\UnitTests\GeradorCertificadosOnline.UnitTests.csproj
   dotnet test .\tests\IntegrationTests\GeradorCertificadosOnline.IntegrationTests.csproj
   ```

7. O RabbitMQ ainda é opcional. Para iniciá-lo quando o processamento
   assíncrono for implementado:

   ```powershell
   docker compose --profile messaging up -d rabbitmq
   ```

   O painel ficará em `http://localhost:15672`, com `guest/guest`.

### Comandos úteis

```powershell
docker compose ps
docker compose logs postgres
docker compose stop
docker compose down
```

Não use `docker compose down -v` sem querer apagar os dados locais.

## Validar a solução

```bash
dotnet restore GeradorCertificadosOnline.slnx
dotnet build GeradorCertificadosOnline.slnx
dotnet test GeradorCertificadosOnline.slnx
dotnet sln GeradorCertificadosOnline.slnx list
```

## Participantes

- **Pedro:** 
- **Marco:** 

Contratos compartilhados devem ser combinados antes da implementação para
evitar divergências de identificadores, estados e respostas HTTP.
