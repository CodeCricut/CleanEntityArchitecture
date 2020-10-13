# CleanEntityArchitecutre
This library provides you with some boilerplate that will help you build rather complex
CRUD APIs and web projects without having to worry about the tedious details.

## Features
JWT Authorization
EF Core CRUD Repositories
CRUD Controllers
CRUD Helper Services

## Installation
Unfortunately, as this is a library still being tested, you can only install it by cloning
the repository:
`git clone https://github.com/CodeCricut/CleanEntityArchitecture`

### Setup
The default services necessary to run this library can be registered in the `ConfigureServices` method of 
`Startup.cs` with a call to `Services.ConfigureCleanEntityArchitecture(Configuration)`.


### Domain
You may create an EF Core `DbContext` without utilizing the library, but any models in the
`DbSet`s must be inherited from `DomainEntity`. This is mostly for generic constraint
purposes, but also ensures that each entity has a `Id` and `Deleted` property.

To keep things clean, you must separate each entity into three classes, each for a different
purpose:
* one inheriting from `DomainEntity`, which is you actual entity with the most properties in most cases.
* one inheriting from `PostModelDto`, which contains the properties the users are allowed
	to modify and create when posting or putting to a controller.
* one inheriting from `GetModelDto`, which contains the properties you wish to expose through
	the controller read actions.

If required, you may choose to include multiple post or get DTOs for each entity.

Here is an example of a simple note entity, including the DTOs:
```csharp
public class Note : DomainEntity
{
	[Required]
	public User User { get; set; }
	[Required]
	public string Title { get; set; }
	[Required]
	public string Text { get; set; }
}

public class PostNoteModel : PostModelDto
{
	public string Title { get; set; }
	public string Text { get; set; }
}

public class GetNoteModel : GetModelDto
{
	public int Id { get; set; }

	public int UserId { get; set; }
	public string Title { get; set; }
	public string Text { get; set; }
}
```
### Repositories
To create a read repository for an entity, you can either inherit from `ReadEntityRepository<TEntity>` or
implement `IReadEntityRepository<TEntity>`. 

To create a write repository for an entity, you can either inherit from `WriteEntityRepository<TEntity> or
implement `IWriteEntityRepository<TEntity>`.

```csharp
public class ReadNoteRepository : ReadEntityRepository<Note>
{
	public ReadNoteRepository(DbContext context) : base(context)
	{
	}

	public override IQueryable<Note> IncludeChildren(IQueryable<Note> queryable)
	{
		return queryable
			.Include(n => n.User);
	}
}

public class WriteNoteRepository : WriteEntityRepository<Note>
{
	public WriteNoteRepository(DbContext context) : base(context)
	{
	}
}
```

You will be required to register these as services in the `ConfigureServiecs` method of `Startup.cs` on a
per-entity basis:
```csharp
servies.AddScoped<IReadEntityRepository<Note>, ReadNoteRepository>()
		.AddScoped<IWriteEntityRepository<Note>, WriteNoteRepository>();
```

### Helper Services
To separate concerns, most of the code not related to routing and model binding extracted out of the controllers
and into helper services.

In order to use a read controller of a certain entity type, a read service helper must be registered of that type.
The same goes for a write controller and write service helper.

To create a read service helper, inherit from `ReadEntityService<TEntity, TGetModel>` or implement 
`IReadEntityService<TEntity, TGetModel>`.
```csharp
public class ReadNoteService : ReadEntityService<Note, GetNoteModel>
{
	public ReadNoteService(IMapper mapper, IReadEntityRepository<Note> readRepository) : base(mapper, readRepository)
	{
	}
}
```

To create a write service helper, inherit from `WriteEntityService<TEntity, TPostModel>` or implement
`IWriteEntityService<TEntity, TPostModel>`. 
As with most public methods in this library, you can override any method in `WriteEntityService` if you need
to add more buisness logic.
```csharp
public class WriteNoteService : WriteEntityService<Note, PostNoteModel>
	{
	private readonly IUserAuth<User> _userAuth;

	public WriteNoteService(IUserAuth<User> userAuth, IReadEntityRepository<Note> readNoteRepo, IWriteEntityRepository<Note> writeNoteRepo, IMapper mapper)
		: base(mapper, writeNoteRepo, readNoteRepo)
	{
		_userAuth = userAuth;
	}

	public override async Task<TGetModel> PostEntityModelAsync<TGetModel>(PostNoteModel entityModel)
	{
		var user = await _userAuth.GetAuthenticatedUserAsync();

		var entity = _mapper.Map<Note>(entityModel);
		entity.User = user;

		var addedEntity = await _writeRepo.AddEntityAsync(entity);
		await _writeRepo.SaveChangesAsync();

		return _mapper.Map<TGetModel>(addedEntity);
	}
}
```

You will be required to register these as services in the `ConfigureServices` method of `Startup.cs` on a
per-entity basis:
```csharp
servies.AddScoped<IReadEntityService<Note>, ReadNoteService>()
		.AddScoped<IWriteEntityService<Note>, WriteNoteService>();
```

### Controllers
Default CRUD controllers are provided to facilitate routing and model binding.

Create a read controller by inheriting from `ReadController<TEntity, TGetEntityModel>` or implementing
`IReadController<TEntity, TGetEntityModel>`.

Create a write controller by inheriting from `WriteController<TEntity, TPostModel, TGetModel>` or implementing
`IWriteController<TEntity, TPostModel, TGetModel>`.

By default, the write controller actions are decorated with `[Authorize]`, but that can be overridden
by overriding the action and decorating it with `[Authorize(false)]`. Likewise, the read actions can be 
overridden and decorated with `[Authorize]`.

```csharp
[Route("api/Notes")]
public class ReadNoteController : ReadController<Note, GetNoteModel>
{
	public ReadNoteController(IReadEntityService<Note, GetNoteModel> readService) : base(readService)
	{
		[Authorize]
		public override Task<ActionResult<PagedList<GetNoteModel>>> GetAsync([FromQuery] PagingParams pagingParams)
		{
			return base.GetAsync(pagingParams);
		}
	}
}

[Route("api/Notes")]
public class WriteNoteController : WriteController<Note, PostNoteModel, GetNoteModel>
{
	public WriteNoteController(IWriteEntityService<Note, PostNoteModel> writeService) : base(writeService)
	{
		[Authorize(false)]
		public override Task<ActionResult<GetNoteModel>> PostAsync([FromBody] PostNoteModel postModel)
		{
			return base.PostAsync(postModel);
		}
	}
}
```

As with other API projects, controllers can be registered with `services.AddControllers()` and
`app.UseEndpoints(endpoints => endpoints.MapControllers())`.

## Authentication
A number of services, controllers, and middleware are configured to make authentication a breeze.

### User Domain
Starting again with the domain, two classes are provided which can be used or inherited from.

`BaseUser` should be inherited from by the user entity. For example,
```csharp
public class User : BaseUser
{
	public IEnumerable<Note> Notes { get; set; } = new List<Note>();
}
```

This user class can still be made into a `DbSet` just like any other entity. It will be provided with 
the properties of `DomainEntity` and `Username` and `Password`.

`LoginModel` is used as a generic type constraint, and contains `Username` and `Password` properties.

### UserLoginRepository
To facilitary default login capability, the `UserLoginRepository` is used. You may inherit from this class,
or implement `IUserLoginRepository` if custom login behavior is required. If not, then you simply need
to register an `IUserLoginRepository<TLoginModel, TUser>` implementation as a service:

`services.AddScoped<IUserLoginRepository<LoginModel, User>, UserLoginRepository<LoginModel, User>>()`

### IAuthenticateUserService
The `IAuthenticateUserService<TLoginModel, TGetModel>` provides two methods used mostly by the 
authentication controller: 
* `Task<TGetModel> LoginAsync(TLoginModel model)`
* `Task<TGetModel> GetAuthenticatedReturnModelAsync()`

A default implementation is not provided because behavior depends heavily on how authentication, but here
is an example of how it could look if using a JWT approach:

```csharp
public class AuthenticateUserService : IAuthenticateUserService<LoginModel, GetUserModel>
	{
		private readonly IUserLoginRepository<LoginModel, User> _userLoginRepository;
		private readonly IJwtHelper _jwtHelper;
		private readonly IMapper _mapper;
		private readonly IUserAuth<User> _userAuth;

		public AuthenticateUserService(IUserLoginRepository<LoginModel, User> userLoginRepository, IJwtHelper jwtHelper,
			 IMapper mapper, IUserAuth<User> userAuth)
		{
			_userLoginRepository = userLoginRepository;
			_jwtHelper = jwtHelper;
			_mapper = mapper;
			_userAuth = userAuth;
		}

		public async Task<GetUserModel> GetAuthenticatedReturnModelAsync()
		{
			var user = await _userAuth.GetAuthenticatedUserAsync();

			var token = _jwtHelper.GenerateJwtToken(user);

			var privateReturnModel = _mapper.Map<GetUserModel>(user);

			privateReturnModel.JwtToken = token;

			return privateReturnModel;
		}

		public async Task<GetUserModel> LoginAsync(LoginModel model)
		{
			var user = await _userLoginRepository.GetUserByCredentialsAsync(model);

			// return null if not found
			if (user == null) return null;

			// generate token if user found
			var token = _jwtHelper.GenerateJwtToken(user);

			var getModel = _mapper.Map<GetUserModel>(user);

			getModel.JwtToken = token;

			return getModel;
		}
	}
```

### Authentication Controllers
You can inherit from `AuthenticateUserController` or implement `IAuthenticateUserController`. They contain
login actions and actions for getting the authenticate user model.

### Other Authorization Goodies
Other authorization related classes are in the `CleanEntityArchitecture.Authorization` namespace.

The `AuthorizeAttribute` can be used to decorate controller classes or actions and require that incoming
requests contain a valid JWT token authorization header. If so, the action will be run as normal. If not, 
an unauthorized message will be returned.

`JwtHelper`, which implements `IJwtHelper`, is simply used to generate a JWT token when provided with a
`BaseUser`.

`JwtMiddleware` parses a JWT authorization header (if present), and attaches the user id stored in it 
to `HttpContext.Items["UserId"]`. In most cases, you will not need to access the user id through the context.

Instead, you may get the current user (with an ID == HttpContext.Items["UserId"]) by using 
`IUserAuth.GetAuthenticatedUserAsync()`

If a request makes it past an `[Authorize]` attribute, then you can safely assume that a valid user will
be returned by this method.
