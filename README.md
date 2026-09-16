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
├── IntegrationTests/
├── ApplicationTests/
└── E2ETests/
```

## Pré-requisitos

- .NET SDK 10;
- banco de dados configurado no ambiente;
- RabbitMQ/CloudAMQP para processamento assíncrono;
- secrets configurados fora do código-fonte.

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
