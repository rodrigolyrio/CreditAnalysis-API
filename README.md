# CreditAnalysis API - Motor de Decisões Financeiras

Este repositório contém um microserviço de **Análise de Crédito** de alta performance, desenvolvido com **.NET 8** e **SQL Server**. O sistema automatiza o fluxo de aprovação de crédito utilizando critérios de comprometimento de renda.

---

### 🏗️ Arquitetura e Diferenciais Técnicos

Diferente de implementações convencionais, este projeto utiliza uma abordagem de **lógica de negócio distribuída**, onde a inteligência crítica reside na camada de dados:

* **Processamento via Stored Procedures:** A regra de aprovação (limite de 30% da renda) é executada diretamente no banco de dados, garantindo **atomicidade**, **segurança** e **velocidade**.
* **Acesso a Dados com Dapper:** Utilização de um Micro-ORM para consultas de alta performance, minimizando o overhead de mapeamento.
* **Programação Assíncrona:** Implementação completa de padrões `async/await` para garantir a escalabilidade da API.

---

### 🛠️ Stack Tecnológica

* **Framework:** .NET 8 (C#)
* **Persistence:** SQL Server 🗄️
* **Data Access:** Dapper
* **Documentation:** Swagger / OpenAPI (com suporte a comentários XML)

---

### 🛣️ Endpoints da API

* **POST `/api/Credito`**: Submete uma nova proposta para análise automática.
* **GET `/api/Credito`**: Recupera o histórico completo de propostas registradas.
* **GET `/api/Credito/{id}`**: Busca detalhada de uma proposta por identificador único.
* **PUT `/api/Credito/{id}`**: Atualização de dados cadastrais (Nome/CPF) de propostas existentes.
* **DELETE `/api/Credito/{id}`**: Remoção física de registros do sistema.

---

### 🚀 Instalação e Configuração

1.  **Database:** Execute o script SQL em `Scripts/ScriptBanco.sql` para provisionar as tabelas e procedures necessárias.
2.  **Environment:** * Renomeie `appsettings.Example.json` para `appsettings.json`.
    * Configure a `DefaultConnection` com suas credenciais locais.
3.  **Run:**
    ```bash
    dotnet run --project CreditoApi
    ```
