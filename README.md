
# ProductCategoryCrud API

Esta es una API REST creada en .NET Core que permite gestionar productos y categorías en una base de datos SQLite. La 
tabla `Products` incluye un campo adicional llamado `Score`.

## Requisitos Previos

- .NET SDK 6.0 o superior
- SQLite
- Entity Framework Core

## Configuración del Proyecto

### 1. Crear el Proyecto

1. Abre una terminal y navega al directorio donde deseas crear el proyecto.
2. Ejecuta el siguiente comando para crear una aplicación Web API:

   ```bash
   dotnet new webapi -n ProductCategoryCrud
   ```

3. Navega al directorio del proyecto:

   ```bash
   cd ProductCategoryCrud
   ```

4. Instala las dependencias de Entity Framework Core y SQLite:

   ```bash
   dotnet add package Microsoft.EntityFrameworkCore
   dotnet add package Microsoft.EntityFrameworkCore.Sqlite
   dotnet add package Microsoft.EntityFrameworkCore.Tools
   ```

### 2. Configurar `AppDbContext`

1. Crea una carpeta llamada `Data` y dentro de ella un archivo `AppDbContext.cs` con el siguiente contenido:

   ```csharp
   using Microsoft.EntityFrameworkCore;
   using ProductCategoryCrud.Models;

   namespace ProductCategoryCrud.Data
   {
       public class AppDbContext : DbContext
       {
           public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

           public DbSet<Category> Categories { get; set; }
           public DbSet<Product> Products { get; set; }
       }
   }
   ```

2. En `appsettings.json`, agrega la cadena de conexión para SQLite:

   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Data Source=products.db"
   }
   ```

3. En `Program.cs`, configura `AppDbContext` para que use SQLite:

   ```csharp
   using Microsoft.EntityFrameworkCore;
   using ProductCategoryCrud.Data;

   var builder = WebApplication.CreateBuilder(args);

   builder.Services.AddControllers();
   builder.Services.AddEndpointsApiExplorer();
   builder.Services.AddSwaggerGen();

   // Configurar AppDbContext con SQLite
   builder.Services.AddDbContext<AppDbContext>(options =>
       options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

   var app = builder.Build();

   if (app.Environment.IsDevelopment())
   {
       app.UseSwagger();
       app.UseSwaggerUI();
   }

   app.UseHttpsRedirection();
   app.UseAuthorization();
   app.MapControllers();
   app.Run();
   ```

### 3. Crear los Modelos `Category` y `Product`

1. Crea una carpeta `Models` y dentro de ella un archivo `Category.cs`:

   ```csharp
   namespace ProductCategoryCrud.Models
   {
       public class Category
       {
           public int Id { get; set; }
           public string Name { get; set; }
       }
   }
   ```

2. En la misma carpeta, crea el archivo `Product.cs` y agrega el campo `Score`:

   ```csharp
   namespace ProductCategoryCrud.Models
   {
       public class Product
       {
           public int Id { get; set; }
           public string Name { get; set; }
           public decimal Price { get; set; }
           public int CategoryId { get; set; }
           public int Score { get; set; }  // Campo Score
       }
   }
   ```

### 4. Crear las Migraciones y Actualizar la Base de Datos

1. Crea una migración inicial para la base de datos:

   ```bash
   dotnet ef migrations add InitialCreate
   ```

2. Aplica la migración a la base de datos:

   ```bash
   dotnet ef database update
   ```

### 5. Crear los Controladores `CategoriesController` y `ProductsController`

1. En la carpeta `Controllers`, crea el archivo `CategoriesController.cs`:

   ```csharp
   using Microsoft.AspNetCore.Mvc;
   using Microsoft.EntityFrameworkCore;
   using ProductCategoryCrud.Data;
   using ProductCategoryCrud.Models;
   using System.Collections.Generic;
   using System.Threading.Tasks;

   namespace ProductCategoryCrud.Controllers
   {
       [Route("api/[controller]")]
       [ApiController]
       public class CategoriesController : ControllerBase
       {
           private readonly AppDbContext _context;

           public CategoriesController(AppDbContext context)
           {
               _context = context;
           }

           [HttpGet]
           public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
           {
               return await _context.Categories.ToListAsync();
           }

           [HttpGet("{id}")]
           public async Task<ActionResult<Category>> GetCategory(int id)
           {
               var category = await _context.Categories.FindAsync(id);
               if (category == null) return NotFound();
               return category;
           }

           [HttpPost]
           public async Task<ActionResult<Category>> PostCategory(Category category)
           {
               _context.Categories.Add(category);
               await _context.SaveChangesAsync();
               return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
           }

           [HttpPut("{id}")]
           public async Task<IActionResult> PutCategory(int id, Category category)
           {
               if (id != category.Id) return BadRequest();
               _context.Entry(category).State = EntityState.Modified;
               await _context.SaveChangesAsync();
               return NoContent();
           }

           [HttpDelete("{id}")]
           public async Task<IActionResult> DeleteCategory(int id)
           {
               var category = await _context.Categories.FindAsync(id);
               if (category == null) return NotFound();
               _context.Categories.Remove(category);
               await _context.SaveChangesAsync();
               return NoContent();
           }
       }
   }
   ```

2. Crea el archivo `ProductsController.cs`:

   ```csharp
   using Microsoft.AspNetCore.Mvc;
   using Microsoft.EntityFrameworkCore;
   using ProductCategoryCrud.Data;
   using ProductCategoryCrud.Models;
   using System.Collections.Generic;
   using System.Threading.Tasks;

   namespace ProductCategoryCrud.Controllers
   {
       [Route("api/[controller]")]
       [ApiController]
       public class ProductsController : ControllerBase
       {
           private readonly AppDbContext _context;

           public ProductsController(AppDbContext context)
           {
               _context = context;
           }

           [HttpGet]
           public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
           {
               return await _context.Products.ToListAsync();
           }

           [HttpGet("{id}")]
           public async Task<ActionResult<Product>> GetProduct(int id)
           {
               var product = await _context.Products.FindAsync(id);
               if (product == null) return NotFound();
               return product;
           }

           [HttpPost]
           public async Task<ActionResult<Product>> PostProduct(Product product)
           {
               _context.Products.Add(product);
               await _context.SaveChangesAsync();
               return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
           }

           [HttpPut("{id}")]
           public async Task<IActionResult> PutProduct(int id, Product product)
           {
               if (id != product.Id) return BadRequest();
               _context.Entry(product).State = EntityState.Modified;
               await _context.SaveChangesAsync();
               return NoContent();
           }

           [HttpDelete("{id}")]
           public async Task<IActionResult> DeleteProduct(int id)
           {
               var product = await _context.Products.FindAsync(id);
               if (product == null) return NotFound();
               _context.Products.Remove(product);
               await _context.SaveChangesAsync();
               return NoContent();
           }
       }
   }
   ```

### 6. Probar la API

1. Ejecuta el proyecto:

   ```bash
   dotnet run
   ```

2. Accede a [http://localhost:5287/swagger](http://localhost:5287/swagger) para probar los endpoints de `Categories` y `Products` en Swagger.

3. Realiza operaciones de CRUD y asegúrate de que el campo `Score` esté funcionando correctamente en la tabla `Products`.

---

Este README cubre desde la creación del proyecto hasta la adición del campo `Score` y las pruebas de los endpoints.
---------------------------------------------------------------------------------------------------------------------------------------
ACTUALIZACION


# ProductCategoryCrud - CRUD de Roles y Hashing de Contraseñas

Este proyecto implementa un sistema para gestionar productos, categorías y usuarios, incluyendo un CRUD completo para roles y funcionalidad de hashing seguro para contraseñas.

## Tabla de Contenidos
1. [Requisitos](#requisitos)
2. [Configuración Inicial](#configuración-inicial)
3. [CRUD de Roles](#crud-de-roles)
    - [Modelo `Role`](#modelo-role)
    - [Controlador `RolesController`](#controlador-rolescontroller)
    - [Migración y Actualización de la Base de Datos](#migración-y-actualización-de-la-base-de-datos)
    - [Pruebas del CRUD](#pruebas-del-crud)
4. [Hashing de Contraseñas](#hashing-de-contraseñas)
    - [Modelo `User`](#modelo-user)
    - [Registro de Usuarios](#registro-de-usuarios)
5. [Pruebas de Usuario y Login](#pruebas-de-usuario-y-login)

---

## Requisitos

1. **Herramientas necesarias**:
   - .NET 6 o superior
   - Entity Framework Core
   - Visual Studio o Visual Studio Code
   - SQLite (para la base de datos local)

2. **Dependencias**:
   - Swashbuckle para Swagger.
   - BCrypt.Net para hashing de contraseñas.

---

## Configuración Inicial

1. **Clonar el repositorio**:
   ```bash
   git clone <repositorio-url>
   cd ProductCategoryCrud
   ```

2. **Instalar dependencias**:
   ```bash
   dotnet restore
   ```

3. **Configurar la base de datos** en el archivo `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=products.db"
     }
   }
   ```

4. **Migrar y actualizar la base de datos**:
   ```bash
   dotnet ef migrations add InitialSetup
   dotnet ef database update
   ```

---

## CRUD de Roles

### Modelo `Role`

El modelo `Role` representa los roles en el sistema.

```csharp
namespace ProductCategoryCrud.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
```

### Controlador `RolesController`

El controlador permite gestionar roles con los siguientes métodos:
- **Listar roles (`GET /api/Roles`)**
- **Obtener un rol por ID (`GET /api/Roles/{id}`)**
- **Crear un rol (`POST /api/Roles`)**
- **Actualizar un rol (`PUT /api/Roles/{id}`)**
- **Eliminar un rol (`DELETE /api/Roles/{id}`)**

```csharp
[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly AppDbContext _context;

    public RolesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
    {
        return await _context.Roles.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Role>> GetRole(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return NotFound();
        return role;
    }

    [HttpPost]
    public async Task<ActionResult<Role>> PostRole(Role role)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutRole(int id, Role role)
    {
        if (id != role.Id) return BadRequest();
        _context.Entry(role).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Roles.Any(r => r.Id == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return NotFound();
        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
```

### Migración y Actualización de la Base de Datos

1. **Crear una migración**:
   ```bash
   dotnet ef migrations add AddRolesTable
   ```

2. **Actualizar la base de datos**:
   ```bash
   dotnet ef database update
   ```

### Pruebas del CRUD

- **Crear un rol (`POST /api/Roles`)**:
  ```json
  {
    "name": "Admin"
  }
  ```

- **Actualizar un rol (`PUT /api/Roles/{id}`)**:
  ```json
  {
    "id": 1,
    "name": "Updated Role"
  }
  ```

- **Eliminar un rol (`DELETE /api/Roles/{id}`)**.

---

## Hashing de Contraseñas

### Modelo `User`

El modelo `User` utiliza `PasswordHash` para almacenar contraseñas hasheadas.

```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public int RoleId { get; set; }
    public Role Role { get; set; }
}
```

### Registro de Usuarios

El método para registrar usuarios incluye el hashing de contraseñas:

```csharp
[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto)
{
    if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
    {
        return BadRequest("El nombre de usuario ya existe.");
    }

    var role = await _context.Roles.FindAsync(registerDto.RoleId);
    if (role == null)
    {
        return BadRequest("El rol especificado no existe.");
    }

    var user = new User
    {
        Username = registerDto.Username,
        PasswordHash = PasswordHasher.HashPassword(registerDto.Password),
        RoleId = registerDto.RoleId
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();
    return Ok("Usuario registrado exitosamente.");
}
```

### DTO para Registro de Usuarios

```csharp
public class UserRegisterDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
}
```

---

## Pruebas de Usuario y Login

- **Registro de un usuario (`POST /api/Login/register`)**:
  ```json
  {
    "username": "newuser",
    "password": "password123",
    "roleId": 1
  }
  ```

- **Login de un usuario (`POST /api/Login/login`)**:
  ```json
  {
    "username": "newuser",
    "password": "password123"
  }
  ```

---

## Notas Finales

Este proyecto está configurado para trabajar con SQLite como base de datos por defecto. Asegúrate de actualizar las cadenas de conexión si decides usar otro proveedor.


