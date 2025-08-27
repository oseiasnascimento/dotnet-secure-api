# WebApi .NET 8 - Gerenciamento de Usuários e Autenticação

Esta aplicação é uma API desenvolvida em .NET 8 para gerenciamento de contas de usuários, autenticação e autorização. Ela utiliza arquitetura em camadas (Domain, Application, Infrastructure, WebApi) e segue boas práticas de desenvolvimento.

## Funcionalidades

- Cadastro de usuários
- Atualização de contas
- Autenticação (login)
- Gerenciamento de roles/permissões
- Seed de usuários e roles padrão
- Configuração flexível via arquivos `appsettings`

## Endpoints da API

| Método | Rota                          | Descrição                        |
| ------ | ----------------------------- | -------------------------------- |
| GET    | /usuarios/me                  | Dados do usuário autenticado     |
| GET    | /usuarios                     | Listar usuários                  |
| GET    | /usuarios/buscar              | Buscar usuários                  |
| POST   | /usuarios/auth                | Autenticação de usuário          |
| POST   | /usuarios/cadastro            | Cadastro de novo usuário         |
| PUT    | /usuarios/{userId}            | Atualizar dados do usuário       |
| PUT    | /usuarios/{userId}/cargos     | Atualizar cargos do usuário      |
| PUT    | /usuarios/{userId}/status     | Alterar status do usuário        |
| POST   | /usuarios/refresh             | Gerar novo token de autenticação |
| POST   | /usuarios/revogar             | Revogar token                    |
| POST   | /usuarios/esqueci-minha-senha | Solicitar recuperação de senha   |
| POST   | /usuarios/recuperar-senha     | Recuperar senha                  |
| POST   | /usuarios/alterar-senha       | Alterar senha                    |

## Tabelas do Banco de Dados

| Tabela    | Campos Principais                                   |
| --------- | --------------------------------------------------- |
| Users     | Id, UserName, Email, PasswordHash, RoleId, IsActive |
| Roles     | Id, Name, Description                               |
| UserRoles | UserId, RoleId                                      |

## Estrutura do Projeto

- **Domain**: Entidades e regras de negócio
- **Application**: DTOs, casos de uso e lógica de aplicação
- **Infrastructure**: Implementações de persistência e identidade
- **WebApi**: Endpoints, configuração e helpers

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Banco de dados configurado (ver `appsettings.json`)

## Instalação

1. Clone o repositório: git clone <url-do-repositorio>
2. Restaure os pacotes: dotnet restore
3. Configure o banco de dados em `WebApi/appsettings.json`.
4. Execute as migrações e o seed: dotnet run --project WebApi

## Uso

- A API expõe endpoints para cadastro, autenticação e gerenciamento de usuários.
- Utilize ferramentas como [Postman](https://www.postman.com/) ou [Swagger](https://swagger.io/) para testar os endpoints.

## Configuração

- Os arquivos de configuração estão em `WebApi/appsettings.json`, `appsettings.Development.json` e `appsettings.Container.json`.
- Para ambiente de desenvolvimento, utilize `appsettings.Development.json`.

## Scripts e Helpers

- **SeedDatabaseHelper**: Realiza o seed inicial do banco de dados.
- **MigrationsHelper**: Auxilia na execução de migrações.

## Contribuição

1. Fork este repositório
2. Crie uma branch (`git checkout -b feature/nova-feature`)
3. Commit suas alterações (`git commit -am 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## Licença

Este projeto está sob a licença MIT.

---

> Para dúvidas técnicas, consulte a documentação oficial do .NET ou abra uma issue neste repositório.
