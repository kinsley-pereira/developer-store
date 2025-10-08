
------------------------------------------------------------------------

## 🚀 Executando o Projeto

### 1️⃣ Clonar o repositório

``` bash
git clone https://github.com/seu-usuario/sales-api.git
cd sales-api
```

### 2️⃣ Criar o container do PostgreSQL

Execute o seguinte comando para criar e iniciar o banco de dados (necessário ter o docker-desktop):

``` bash
docker run --name postgres -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=admin12345!@# -e POSTGRES_DB=store -p 5432:5432 -d postgres
```

### 3️⃣ Configurar a Connection String

Edite o arquivo **appsettings.Development.json** da API, se necessário
**ConnectionStrings**:

``` json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=store;Username=admin;Password=admin12345!@#"
}
```

### 4️⃣ Executar as migrations

No diretório do projeto **Infrastructure**, execute:

``` bash
dotnet ef database update --startup-project ../Sales.API --project Sales.Infrastructure
```

Para novas migrations:

``` bash
dotnet ef migrations add InitialCreate --startup-project ../Sales.API --project Sales.Infrastructure
```

### 5️⃣ Executar a API

No diretório raiz do projeto:

``` bash
dotnet run --project Sales.API
```

A API será iniciada e estará disponível em:
`https://localhost:7260`\
`http://localhost:5121`

------------------------------------------------------------------------

## 🧪 Executando os Testes

Para executar todos os testes:

``` bash
dotnet test
```

Os testes estão localizados no projeto `Sales.Tests`.

------------------------------------------------------------------------
