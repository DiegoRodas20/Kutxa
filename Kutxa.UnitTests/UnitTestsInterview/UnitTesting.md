# Unit Testing — Plantilla de Referencia

> Librerías: **xUnit** + **Moq** + **FluentAssertions**  
> Basado en el proyecto `Kutxa` con el dominio `User` / `IUserRepository`.

---

## Estructura base de un test

```csharp
public class CreateUserCommandHandlerTests
{
	// 1. Dependencias mockeadas
	private readonly Mock<IUserRepository> _repositoryMock;

	// 2. La clase que estamos testeando (SUT - System Under Test)
	private readonly CreateUserCommandHandler _handler;

	// 3. Constructor: se ejecuta antes de CADA test
	public CreateUserCommandHandlerTests()
	{
		_repositoryMock = new Mock<IUserRepository>();
		_handler = new CreateUserCommandHandler(_repositoryMock.Object); // .Object = instancia falsa
	}

	[Fact] // <- marca el método como un test
	public async Task Handle_ShouldReturnValidGuid_WhenCommandIsValid()
	{
		// Arrange  -> prepara datos y configura mocks
		// Act      -> ejecuta el método a testear
		// Assert   -> verifica el resultado
	}
}
```

---

## MOQ

### Setup — configurar qué devuelve el mock

```csharp
// Devuelve un objeto (async)
_repositoryMock
	.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
	.ReturnsAsync(user);

// Devuelve null (async)
_repositoryMock
	.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
	.ReturnsAsync((User?)null);

// Devuelve una lista (async)
_repositoryMock
	.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
	.ReturnsAsync(new List<User> { user1, user2 });

// Devuelve lista vacía (async)
_repositoryMock
	.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
	.ReturnsAsync(new List<User>());

// Método void / Task sin retorno
_repositoryMock
	.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
	.Returns(Task.CompletedTask);
```

---

### It — matchers de argumentos

```csharp
It.IsAny<Guid>()              // cualquier Guid
It.IsAny<CancellationToken>() // cualquier CancellationToken
It.IsAny<User>()              // cualquier User
It.Is<Guid>(id => id == userId)       // Guid específico con condición
It.Is<User>(u => u.Name == "John")    // User que cumpla una condición
```

---

### Callback — capturar el objeto que se pasó al mock

```csharp
User? capturedUser = null;

_repositoryMock
	.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
	.Callback<User, CancellationToken>((user, _) => capturedUser = user) // captura el User
	.Returns(Task.CompletedTask);

// Después del Act, capturedUser tiene el objeto que el handler pasó al repositorio
capturedUser!.Name.Should().Be("John Doe");
```

> **Tip:** `Callback<T1, T2>(...)` — los tipos genéricos deben coincidir con los parámetros del método mockeado, en orden.

---

### Verify — verificar que el mock fue llamado

```csharp
// Fue llamado exactamente 1 vez
_repositoryMock.Verify(
	r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
	Times.Once);

// Fue llamado exactamente N veces
_repositoryMock.Verify(
	r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
	Times.Exactly(2));

// Nunca fue llamado
_repositoryMock.Verify(
	r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
	Times.Never);

// Fue llamado con un argumento específico
_repositoryMock.Verify(
	r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()),
	Times.Once);
```

---

### Setup + Verify juntos (patrón completo)

```csharp
[Fact]
public async Task Handle_ShouldCallUpdateAsync_Once_WhenUserExists()
{
	// Arrange
	var user = User.Create("John Doe", "john@example.com");
	var command = new UpdateUserCommand(user.Id, "John Updated", "john.updated@example.com");

	_repositoryMock
		.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
		.ReturnsAsync(user);

	// Act
	await _handler.Handle(command, CancellationToken.None);

	// Assert
	_repositoryMock.Verify(
		r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
		Times.Once);
}
```

---

## FLUENT ASSERTIONS

### object / clase

```csharp
result.Should().NotBeNull();
result.Should().BeNull();
result.Should().BeOfType<UserResponse>();

// Compara todas las propiedades recursivamente (muy útil)
result.Should().BeEquivalentTo(expectedUser);

// Compara con objeto anónimo (sin necesitar una instancia completa)
result.Should().BeEquivalentTo(new { Name = "John", Email = "john@example.com" });

// Mismo objeto en memoria (referencia)
result.Should().BeSameAs(otherObject);
```

---

### string

```csharp
name.Should().Be("John Doe");
name.Should().NotBe("Jane");
name.Should().BeEmpty();
name.Should().NotBeEmpty();
name.Should().BeNullOrEmpty();
name.Should().NotBeNullOrEmpty();
name.Should().Contain("John");
name.Should().NotContain("Jane");
name.Should().StartWith("John");
name.Should().EndWith("Doe");
name.Should().HaveLength(8);
name.Should().MatchRegex(@"^[A-Z]");
```

---

### Guid

```csharp
result.Should().Be(capturedUser.Id);
result.Should().NotBe(Guid.Empty);
result.Should().NotBeEmpty(); // equivalente a NotBe(Guid.Empty)
```

---

### bool

```csharp
isValid.Should().BeTrue();
isValid.Should().BeFalse();
```

---

### numérico (int, decimal, double...)

```csharp
count.Should().Be(3);
count.Should().NotBe(0);
count.Should().BeGreaterThan(0);
count.Should().BeGreaterThanOrEqualTo(1);
count.Should().BeLessThan(10);
count.Should().BeInRange(1, 5);
```

---

### DateTime

```csharp
user.CreatedAt.Should().Be(expectedDate);
user.CreatedAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));
user.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddMinutes(1));
user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
```

---

### IEnumerable / List

```csharp
result.Should().BeEmpty();
result.Should().NotBeEmpty();
result.Should().HaveCount(2);
result.Should().HaveCountGreaterThan(0);
result.Should().ContainSingle();                          // exactamente 1 elemento

result.Should().Contain(user);                            // contiene el objeto
result.Should().NotContain(user);

result.Should().Contain(u => u.Name == "John");           // contiene uno que cumpla la condición
result.Should().OnlyContain(u => u.Name != null);         // todos cumplen la condición

result.Should().AllSatisfy(u =>
{
	u.Email.Should().NotBeNullOrEmpty();
	u.Name.Should().NotBeNullOrEmpty();
});

// Obtener un único elemento para hacer asserts individuales
var item = result.Should().ContainSingle().Subject;
item.Name.Should().Be("John");
```

---

### Excepciones (async)

```csharp
// Lanza una excepción
var act = () => _handler.Handle(command, CancellationToken.None);
await act.Should().ThrowAsync<ArgumentException>();

// Lanza con mensaje específico
await act.Should().ThrowAsync<ArgumentException>()
	.WithMessage("*invalid*"); // * = wildcard

// No lanza ninguna excepción
await act.Should().NotThrowAsync();
```

---

## Patrones de tests más comunes

### 1 — Verificar que un método fue llamado (sin retorno)

```csharp
[Fact]
public async Task Handle_ShouldCallAddAsync_Once_WhenCommandIsValid()
{
	var command = new CreateUserCommand("John Doe", "john@example.com");

	await _handler.Handle(command, CancellationToken.None);

	_repositoryMock.Verify(
		r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
		Times.Once);
}
```

---

### 2 — Verificar el objeto pasado al mock (Callback)

```csharp
[Fact]
public async Task Handle_ShouldPassCorrectDataToRepository()
{
	var command = new CreateUserCommand("John Doe", "john@example.com");
	User? capturedUser = null;

	_repositoryMock
		.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
		.Callback<User, CancellationToken>((user, _) => capturedUser = user)
		.Returns(Task.CompletedTask);

	await _handler.Handle(command, CancellationToken.None);

	capturedUser.Should().NotBeNull();
	capturedUser!.Name.Should().Be(command.Name);
	capturedUser.Email.Should().Be(command.Email);
}
```

---

### 3 — Verificar el retorno cuando el repositorio devuelve un objeto

```csharp
[Fact]
public async Task Handle_ShouldReturnMappedUser_WhenUserExists()
{
	var user = User.Create("John Doe", "john@example.com");
	var query = new GetUserByIdQuery(user.Id);

	_repositoryMock
		.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
		.ReturnsAsync(user);

	var result = await _handler.Handle(query, CancellationToken.None);

	result.Should().NotBeNull();
	result!.Id.Should().Be(user.Id);
	result.Name.Should().Be(user.Name);
	result.Email.Should().Be(user.Email);
}
```

---

### 4 — Verificar el retorno cuando el repositorio devuelve null

```csharp
[Fact]
public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
{
	var query = new GetUserByIdQuery(Guid.NewGuid());

	_repositoryMock
		.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
		.ReturnsAsync((User?)null);

	var result = await _handler.Handle(query, CancellationToken.None);

	result.Should().BeNull();
}
```

---

### 5 — Verificar que se llamó con el ID correcto

```csharp
[Fact]
public async Task Handle_ShouldQueryRepositoryWithCorrectId()
{
	var userId = Guid.NewGuid();
	var query = new GetUserByIdQuery(userId);

	_repositoryMock
		.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
		.ReturnsAsync((User?)null);

	await _handler.Handle(query, CancellationToken.None);

	_repositoryMock.Verify(
		r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()),
		Times.Once);
}
```

---

## Resumen rápido

| Qué quiero hacer | Herramienta |
|---|---|
| Hacer que el mock devuelva algo | `Setup(...).ReturnsAsync(valor)` |
| Hacer que el mock devuelva `Task` | `Setup(...).Returns(Task.CompletedTask)` |
| Capturar el objeto enviado al mock | `Setup(...).Callback<T1, T2>((x, _) => ...)` |
| Verificar que fue llamado N veces | `Verify(..., Times.Once/Never/Exactly(n))` |
| Aceptar cualquier valor en un parámetro | `It.IsAny<T>()` |
| Comprobar igualdad | `.Should().Be(valor)` |
| Comprobar que no es null | `.Should().NotBeNull()` |
| Comprobar propiedades de un objeto | `.Should().BeEquivalentTo(...)` |
| Comprobar tamaño de una lista | `.Should().HaveCount(n)` |
| Comprobar que una lista está vacía | `.Should().BeEmpty()` |
| Comprobar que lanza excepción | `await act.Should().ThrowAsync<T>()` |
