# CleanEntityArchitecture
This library provides you with some boilerplate that will help you build rather complex
CRUD APIs and web projects without having to worry about the tedious details.

Example project: [CleanNoteTaker](https://github.com/CodeCricut/CleanNoteTaker)

## Features
* JWT Authorization
* EF Core CRUD Repositories
* CRUD Controllers
* CRUD Helper Services

## Installation
Unfortunately, as this is a library still being tested, you can only install it by cloning
the repository:
`git clone https://github.com/CodeCricut/CleanEntityArchitecture`

## Setup
The default services necessary to run this library can be registered in the `ConfigureServices` method of 
`Startup.cs` with a call to `Services.ConfigureCleanEntityArchitecture(Configuration)`.

## Architecture
There are four major components that you must be aware of for any domain entity you wish to have CRUD control over:
1. Domain Objects - the data objects which you store in your database, interact with, and allow users to read and write.
2. Repositories - solely responsible for interacting with the database through EF Core, but not for performing
business logic.
3. Helper services - provide a buffer between the controllers and repositories that perform business logic and other things
that shouldn't be performed in controllers nor repositories.
4. Controllers - the highest abstraction in the application, and are responsible only for routing, model binding,
	model validating, and calling the helper services.

Controllers depend on helper services, helper services depend on repositories, and repositories act on domain objects.
If you have a `ReadNoteController` that inherits from `ReadController<Note, GetNoteModel>`, then you must also have 
registered services for
* `IReadEntityService<Note, GetNoteModel>`
* `IReadEntityRepository<Note>`

and these objects
* `Note : DomainEntity`
* `GetNoteModel : GetModelDto`

In terms of the authentication pipeline, to have a `DefaultAuthenticateUserController` that inherits from 
`AuthenticateUserController<GetUserModel, LoginModel>`, then you must also have registered services for
* `IAuthenticateUserService<LoginModel, GetUserModel>`,
* `IUserLoginrepository<LoginModel, User>`

and these objects
* `User : BaseUser`
* `LoginModel`
* `GetUserModel : GetModelDto`

### Domain
Entities that are in a `DbSet<TEntity>` must inherit from `DomainEntity`. This will provide them with `Id` and `Deleted` properties.
Entities are further split up into post and retrieve DTOs. You must have at least one of each for each entity, 
inheriting from `PostModelDto` and `GetModelDto`, respectively.

If you wish to integrate authentication and users into your application, you must use or implement `BaseUser` and
`LoginModel`. You can also create a post and get model for registration and retrieval (extending `PostModelDto` and
`GetModelDto`).

For each entity, you must configure an AutoMapper profile that includes a map from `DomainEntity -> DomainEntity`,
`PostModelDto -> DomainEntity`, `DomainEntity -> GetModelDto`

### Repositories
The familiar CRUD repository is split into read and write repositories, as you don't need to implement both if 
you so choose.

Most behavior is taken care of out of the box. You simply must implement or extend `IReadEntityRepository`, `ReadEntityRepository`;
`IWriteEntityRepository`, and `WriteEntityRepository` as you see fit.

These must be registered as services on a per-entity basis.
```csharp
servies.AddScoped<IReadEntityRepository<Note>, ReadNoteRepository>()
		// You can use the generic WriteEntityRepository without inheriting from it.
		.AddScoped<IWriteEntityRepository<Note>, WriteEntityRepository<Note>>();
```

To facilitate retrieving a user given a login model, you can provide a service for `IUserLoginRepository<TLoginModel, TUser>`.
A default service is provided, and you can simply register it if it is satisfactory:
```csharp
services.AddScoped<IUserLoginRepository<LoginModel, User>, UserLoginRepository<LoginModel, User>>();
```
### Helper Services
Helper services implement `IReadEntityService<TEntity, TGetModel>` and `IWriteEntityService<TEntity, TPostModel>`.

Default helper service implementation is provided. You can simply register these services like this:
```csharp
services.AddScoped<IReadEntityService<Note, GetNoteModel>, ReadEntityService<Note, GetNoteModel>>()
		.AddScoped<IWriteEntityService<Note, PostNoteModel>, WriteEntityService<Note, PostNoteModel>>();
```

If you want to perform business logic, such as creating relationships between entities before saving them
to the database, or authentication, you can implement `ReadEntityService<TEntity, TGetModel>` or 
`WriteEntityService<TEntity, TPostModel>` and override the methods.

`IAuthenticateUserService<TLoginModel, TGetModel>` provides methods for logging
in and getting the current user from the database. The defualt implementation is `AuthenticateUserService<TLoginModel, TGetModel>`.

`IJwtService` and `JwtService` provide methods of generating JWTs based on a login model. This service does the heavy lifting of the `JwtController`.

Registration of these services is required, but as you would expect.

### Controllers
CRUD controllers implement `IWriteController<TEntity, TPostModel, TGetModel>` or
`IReadController<TEntity, TGetModel>`. Some default behavior is provided by 
`WriteController<TEntity, TPostModel, TGetModel>` and `ReadController<TEntity, TGetModel>`, but these are abstract 
because you must decorate each controller with a route attribute. For example, `[Route("api/Notes")]`.

Most actions on `WriteController` are decorated with `[Authorize]`. If you don't want to verify users are 
authorized before calling those actions, you can override them and decorate them with `[Authorize(false)]`:
```csharp
[Authorize(false)]
public override Task<ActionResult<GetNoteModel>> PostAsync([FromBody] PostNoteModel postModel)
{
	return base.PostAsync(postModel);
}
```

Conversely, most actions on `ReadController` are not decorated with `[Authorize]`, but can be by overriding them
and decorating them manually:
```csharp
[Authorize]
public override Task<ActionResult<PagedList<GetNoteModel>>> GetAsync([FromQuery] PagingParams pagingParams)
{
	return base.GetAsync(pagingParams);
}
```

`IAuthenticateUserController<TGetUserModel, TLoginModel>` provides actions for logging in and getting the current
user. 

`IJwtController` provides an action for generating a JWT token based on a login model. This token can then be sent in the authorization header
in further requests.

Controllers can be registered with `services.AddControllers()` and
`app.UseEndpoints(endpoints => endpoints.MapControllers())`.

### Authorization Goodies
Other authorization related classes are in the `CleanEntityArchitecture.Authorization` namespace.

The `JwtAuthorizeAttribute` can be used to decorate controller classes or actions and require that incoming
requests contain a valid JWT token authorization header. If so, the action will be run as normal. If not, 
an unauthorized message will be returned.

`JwtHelper`, which implements `IJwtHelper`, is simply used to generate a JWT token when provided with a
`BaseUser`. The token will contain a `UserId` claim that can be extracted later to get the current user.

`JwtMiddleware` parses a JWT authorization header (if present), and attaches the user id stored in it 
to `HttpContext.Items["UserId"]`. In most cases, you will not need to access the user id through the context.

Instead, you may get the current user (with an ID == HttpContext.Items["UserId"]) by using 
`IUserAuth.GetAuthenticatedUserAsync()`

If a request makes it past an `[JwtAuthorize]` attribute, then you can safely assume that a valid user will
be returned by this method.

You are required to provide a secret string in order to generate JWT tokens. Inside `appsettings.json`, add the 
following branch:
```json
{
	...
	"JwtSettings": {
		"JwtSecret":  "Example JWT Secret."
	}
}
```
