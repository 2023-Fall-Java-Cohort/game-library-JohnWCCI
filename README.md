<p style="text-align: center"><image src="WECANCodeIT.png" /></p>

# GameSolution

## Overview
#### The purpose of this code along project is to get the student accustom to working with more advanced coding technics. Such as Interfaces, Generics, Repositories, Dependance Injection and Web API's and Entity Framework with in an MVC application. 
**Skills Required**
1. multi-Project solution
2. Generic Class
3. Exensions to class
4. Logging
4. Entity Framework
5. MCV Project
6. Web Api project
8. Excepiton handling
#### Suggested Stretch Tasks after the "GameDataLibrary" is complete
A. Add Menu to the _Layout.cshtml
B. Add CSS Back ground Colors/Images to pages
C. Add CSS for Input Text boxs  

## Phase 1. Build an MVC Application 
## Step 1 Solution Setup
- Solution Name **GameSolution**
- Project 1 ASP MVC app **GameLibrary**
- Project 2 Class Library **GameDataLibrary**  
- Project 2 ASP Web API **GameService**  

1. ##### in the **GameDataLibrary** create a folder **Models**


## Step 2. Data Models
1. PublisherModel 
- Table name should be Publishers
- Table Key is Id
- User needs to have a Name
  - Name is Required
  - Name can not be empty
  - Name can not be null
  - Name Can not be longer then 100 characters
 - User wants a list of board games the publisher owns
 - User Need to convert Class to Json String
   - User wants a Json string of the class
     - Override ToString() 
 ```` C#
 public override string ToString()
        {
            return JsonSerializer.Serialize<PublisherModel>(this);
        }
````
  
2. BoardGameModel
- Table name should be BoardGames
- Table Key is Id
- User needs to have a Name
    - Name is Required
    - Name can not be empty
    - Name can not be null
    - Name Can not be longer then 100 characters
- User needs to have a Description
  - Description is Required
  - Description can not be empty
  - Description can not be null
  - Description Can not be longer then 500 characters
- User needs to have a ImageURL
  - ImageURL is Required
  - ImageURL can not be empty
  - ImageURL can not be null
  - ImageURL should be a vaild Url
  - ImageURL Can not be longer then 500 characters
- User needs a reference to the publisher that makes this game
    - PublishersId is required
    - PublishersId is a Foreign Key to the PublisherModel
- User wants a link back to the PublisherModel Data
  - The Publishers property should be Nullable
  - The Publishers property should be virtual
- User needs quicke access to the publishers name
  - Publisher needs to be Not Mapped to the Database
  - Publisher needs to be a read only property
  - Publisher needs to check for a Null Publishers before returning
- User Need to convert Class to Json String
  - User wants a Json string of the class
     - Override ToString() 
 ```` C#
 public override string ToString()
        {
            return JsonSerializer.Serialize<PublisherModel>(this);
        }
````  

##### Add the following NuGet packages to the **GameService**
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
-
 ## Step 3. Data Context
1. ##### in the **GameService** create a folder **Context**
- The GameDataContext should have a DbSet property for the PublisherModel
- The GameDataContext should have a DbSet property for the BoradGameModel
- You will need to override the OnConfiguring method
```` C#
 protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
 {
    string connectionString = "Server=(localdb)\\mssqllocaldb;Database=**GameData**;Trusted_Connection=True;";
    optionsBuilder.UseSqlServer(connectionString);
 }
````
- replace GameData with your SQL Name
- override the OnModelCreating
    - Add your seed Data for the Publisher
    - Add Your seed Data for the Board Games
**Example**

````C#
 //ToDo Must seed the publisher first
    modelBuilder.Entity<PublisherModel>().HasData(
            new PublisherModel()
            {
               Id = 1,
               Name = "Days of Wonder"
             },
````
## Step 4. Data Migration
#### in the "Package Manager Console"
1. Type "Add-Migration initial"
2. Type "update-database"
3. Correct any bugs then repeat Step 4.

## Step 5. Data Services
1.  ##### in the **GameService** create a folder **Services**
2. Create Interface "IService"
     This interface will be the primary interface for all the CRUD operations
``` c#
    public interface IService<TEntity> where TEntity : class
    {
        ValueTask<TEntity> AddAsync(TEntity entity);
        ValueTask<bool> DeleteAsync(TEntity entity);
        ValueTask<List<TEntity>> GetAllAsync();
        ValueTask<TEntity?> GetAsync(int id);
        ValueTask<TEntity> UpdateAsync(TEntity entity);
    }
```
3. Create Interface "IPublisherService"
   - inherit IService
   This interface will serve for the CURD operations for PublisherModel
``` c#
  public interface IPublisherService : IService<PublisherModel>
    {
     
    }
````
4. Create Interface "IBoardGameService"
    - inherit IService
     This interface will serve for the CURD operations for BoardGameModel
     In additon we will return a list of publishers for the create and update dropdown box
``` c#
 public interface IBoardGameRepository : IRepository<BoardGameModel>
    {
        ValueTask<IEnumerable<PublisherModel>> GetPublishers();
    }
```
    
5. Create abstract class called RepositoryDb
- inherit IRepository.  
##### This class will server as the generic default Database oerations. All methods will be marked as Virtual and async. So as to allow for override to custom any operations that are outside the default behavior.

``` c# 
 public abstract class RepositoryDb<TEntity> : IRepository<TEntity> where TEntity : class
```
6. add the following protected fields
```` c#
protected readonly ILogger<RepositoryDb<TEntity>> logger;
protected readonly GameDataContext dbContext;
protected readonly string className;
````
7. Add a consturctor 
````c#
  public RepositoryDb(ILogger<RepositoryDb<TEntity>> logger, GameDataContext dbContext)
        {
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.dbContext = dbContext ?? throw new System.ArgumentNullException(nameof(dbContext));
            
            className = typeof(TEntity).FullName ?? "TEntity";
            logger.LogInformation($"Repository Consturctor called for {className}");
        }
````
8. Right click on class Name RepositoryDb
- select "Quick Actions and refactorings"
- select "Excract interface"
- click "Ok"

9. scroll down to **AddAsync**
- add async virtual key words to the method name
``` C#
public virtual async ValueTask<TEntity> AddAsync(TEntity entity)
{
    try
    {
      dbContext.Set<TEntity>().Add(entity);
      await dbContext.SaveChangesAsync();
      dbContext.Entry(entity).State = EntityState.Detached;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, $"Error adding new {className} to the Database");
      throw;
    }
    return entity;
}
````
10. scroll down to **UpdateAsync**
- add async virtual key words to the method name
```` c#
public virtual async ValueTask<TEntity> UpdateAsync(TEntity entity)
{
    try
    {
        dbContext.Set<TEntity>().Add(entity).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
        dbContext.Entry(entity).State = EntityState.Detached;

        return entity;
    }
    catch (Exception ex)
    {
        logger.LogError(ex, $"Error saving {className} changes to the Database");
        throw;
    }
}
````

11. scroll down to **DeleteAsync**
- add async virtual key words to the method name
```` c#
public async virtual ValueTask<bool> DeleteAsync(TEntity entity)
{
    bool retValue = false;
    try
    {
        dbContext.Set<TEntity>().Remove(entity);
        await dbContext.SaveChangesAsync();
        retValue = true;
    }
    catch (Exception ex)
    {
        logger.LogError(ex, $"Error deleting {className} from the Database");
        throw;
    }

    return retValue;
}
````
12. scroll down to **GetAsync**
- add async virtual key words to the method name
```` c#
public virtual async ValueTask<TEntity?> GetAsync(int id)
{
    TEntity? retValue;
    try
    {
        retValue = await dbContext.Set<TEntity>()
            .Where(w => EF.Property<int>(w, "Id") == id)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
    catch (Exception ex)
    {

        logger.LogError(ex, $"Error getting {className} from the Database using Id = {id}");
        throw;
    }

    return retValue;
}
````
12. scroll down to **GetAllAsync**
- add async virtual key words to the method name
```` c#
public virtual async ValueTask<List<TEntity>> GetAllAsync()
{
    try
    {

        return await dbContext.Set<TEntity>()
            .AsNoTracking()
            .ToListAsync();

    }
    catch (Exception ex)
    {
        logger.LogError(ex, $"Error getting all {className} from the Database");
        throw;
    }
}
````

13.  Create class called PublisherRepositoryDb 
 ##### This class will be used to customize the default behavior in the found in the class Repository. 
- inherit RepositoryDb
- inherit IPublisherRepository
```` c#
 public class PublisherRepositoryDb : RepositoryDb<PublisherModel>, IPublisherRepository
````
14. Add a constructor.
````c#
  public PublisherRepositoryDb(ILogger<PublisherRepositoryDb> logger, GameDataContext dbContext)
            : base(logger, dbContext) { }
````
15. We need to override the GetAsync(int Id) to include BoardGames that the Publisher handles
```` c#
public override async ValueTask<PublisherModel?> GetAsync(int id)
{
    PublisherModel? retValue = null;
    try
    {
        retValue = await dbContext.Set<PublisherModel>()
            .Where(w => EF.Property<int>(w, "Id") == id)
            .Include(b => b.BoardGames)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, $"Error getting all {className} from the Database");
        throw;
    }
    return retValue;
}
````
16.  Create class called BoardGameRepositoryDb 
 ##### This class will be used to customize the default behavior in the found in the class Repository. 
- inherit RepositoryDb
- inherit IBoardGameRepository
````c#
 public class BoardGameRepositoryDb : RepositoryDb<BoardGameModel>, IBoardGameRepository
````
17. Add Consturctor
````C#
public BoardGameRepositoryDb(ILogger<BoardGameRepositoryDb> logger,GameDataContext dbContext)
            : base(logger,dbContext){ }
````
18. We need to override the GetAsync(int Id) to include the Publisher handles the BoardGame
````c#
public override async ValueTask<List<BoardGameModel>> GetAllAsync()
{
    try
    {

        return await dbContext.Set<BoardGameModel>()
            .Include(p => p.Publishers) 
            .AsNoTracking()
            .ToListAsync();

    }
    catch (Exception ex)
    {
        logger.LogError(ex, $"Error getting all {className} from the Database");
        throw;
    }
}
````
19. We need to override the GetAllAsync() to include the Publisher handles the BoardGame
````c#
public override async ValueTask<List<BoardGameModel>> GetAllAsync()
{
    try
    {

        return await dbContext.Set<BoardGameModel>()
            .Include(p => p.Publishers) 
            .AsNoTracking()
            .ToListAsync();

    }
    catch (Exception ex)
    {
        logger.LogError(ex, $"Error getting all {className} from the Database");
        throw;
    }
}
````
20. We need a method to return all the publisher sorted by name to populate View publisher dropDown
````C#
public async ValueTask<IEnumerable<PublisherModel>> GetPublishers()
{
    try
    {

        return await dbContext.Set<PublisherModel>()
            .AsNoTracking()
            .OrderBy(p=> p.Name)
            .ToListAsync();

    }
    catch (Exception ex)
    {
               
        throw;
    }
}
````
### Step 6. Dependence Injection
#### we will use a Setup extenision within the assembly GameDataLibrary
1. Add static class Setup
2. Add static method to extend IServiceCollection
    The purpose of this method is to allow DI in this assembly
````c#
 public static class Startup
    {
        public static IServiceCollection AddDbService(this IServiceCollection service)
        {
            service.AddDbContext<GameDataContext>();
            service.AddScoped<IPublisherRepository, PublisherRepositoryDb>();
            service.AddScoped<IBoardGameRepository, BoardGameRepositoryDb>();
            return service;
        }

    }
````

### Step 7. MVC Controllers in GameLibrary
#### The default behavior of the MVC with Entity framework will help, but the underline code will need to be replaced with the reference to the repositories.

- PublisherController
    1. User can display all Publishers
    2. User can add new Publishers
    3. User Can edit exisiting publishers
    4. User can display details of a single publisher
    5. User can delete publisher
    
- BoardGameController
  1. User can display all Boardgames
  2. User can add new Boardgames
  3. User Can edit exisiting Boardgames
  4. User can display details of a single Boardgame
  5. User can delete Boardgames

1. add a using statement to Program.cs in GameLibrary
````c#
using GameDataLibrary;
````
2. add builder.Services.AddDbService() after builder.Services.AddControllersWithViews()
```` c#
// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Database services
builder.Services.AddDbService();
````

3. Add PublisherController to GameLibrary
- Right click on the Controllers Folder in the GameLibrary Project
- Select Add
- Select Controllers
- Select MVC Controller with Views, using Entity Framework
- Click Add
- Data Model should be "PublisherModel"
- Data Context should be "GameDataContext"
- Change Controller name to "PublisherController"

4. Add BoardGameController to GameLibrary
- Right click on the Controllers Folder in the GameLibrary Project
- Select Add
- Select Controllers
- Select MVC Controller with Views, using Entity Framework
- Click Add
- Data Model should be "BoardGameModel"
- Data Context should be "GameDataContext"
- Change Controller name to "BoardGameController"

## Run the applicaion check to make sure things are working.

5. Modifiy PublisherController
6. Modifiy Private fields
````c#
 private readonly ILogger<PublisherController> logger;
 private readonly IPublisherRepository repository;
````
7. Modifiy Constructor
```` c#
public PublisherController(ILogger<PublisherController> logger, IPublisherRepository repository)
 {
    this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
    this.repository = repository ?? throw new System.ArgumentNullException(nameof(repository));
    logger.LogInformation($"PublisherController started");
 }
````
8. Modifiy Index
````c#
public async Task<IActionResult> Index()
{
    logger.LogInformation($"{nameof(Index)}");
    return View(await repository.GetAllAsync());
}
````
9. Modifiy Details
```` c#
public async Task<IActionResult> Details(int id)
{
    logger.LogInformation($"{nameof(Details)}");
    var publisherModel = await repository.GetAsync(id);
    if (publisherModel == null)
    {
        logger.LogInformation($"{nameof(Details)} of {id} not found");
        return NotFound();
    }

    return View(publisherModel);
}
````
10. Modifiy Create
```` c#
public IActionResult Create()
{
    logger.LogInformation($"{nameof(Create)}");
    return View();
}

````
11. Modifiy Create save
```` c#
public async Task<IActionResult> Create([Bind("Id,Name")] PublisherModel publisherModel)
{
    logger.LogInformation($"{nameof(Create)} Save model");
    if (ModelState.IsValid)
    {
        await repository.AddAsync(publisherModel);
        return RedirectToAction(nameof(Index));
    }
    else
    {
       logger.LogInformation($"{nameof(Create)} invalid model");
    }
    return View(publisherModel);
}
````
12. Modifiy Edit
```` C#
public async Task<IActionResult> Edit(int id)
{
    logger.LogInformation($"{nameof(Edit)}");
    var publisherModel = await repository.GetAsync(id);
    if (publisherModel == null)
    {
        logger.LogInformation($"{nameof(Edit)} of {id} not found");
        return NotFound();
    }
    return View(publisherModel);
}
````
13. Modifiy Edit Save
```` c#
public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] PublisherModel publisherModel)
{
    logger.LogInformation($"{nameof(Edit)} Save");
    if (ModelState.IsValid)
    {
        await repository.UpdateAsync(publisherModel);
        return RedirectToAction(nameof(Index));
    }
    else
    {
        logger.LogInformation($"{nameof(Edit)} invalid model.");
    }
    return View(publisherModel);
}
````
14. Modifiy Delete
```` c#
public async Task<IActionResult> Delete(int id)
{
    logger.LogInformation($"{nameof(Delete)} {id}");
    var publisherModel = await repository.GetAsync(id);

    if (publisherModel == null)
    {
        logger.LogInformation($"{nameof(Delete)} {id} not found");
        return NotFound();
    }

    return View(publisherModel);
}
````
15. Modifiy Delete Confirmed
```` c#
public async Task<IActionResult> DeleteConfirmed(int id)
{
    logger.LogInformation($"{nameof(DeleteConfirmed)} {id}");
    var publisherModel = await repository.GetAsync(id);
    if (publisherModel != null)
    {
        await repository.DeleteAsync(publisherModel);
    }
    else
    {
       logger.LogInformation($"{nameof(DeleteConfirmed)} {id} not found");
    }
    return RedirectToAction(nameof(Index));
}
````
## Run the applicaion check to make sure things are working.

16. Modifiy BoardGameController
17. Modifiy Private fields
````c#
 private readonly ILogger<BoardGameController> logger;
 private readonly IBoardGameRepository repository;
````
18. Modifiy Constructor
```` c#
public BoardGameController(ILogger<BoardGameController> logger, IBoardGameRepository repository)
{
    this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
    this.repository = repository ?? throw new System.ArgumentNullException(nameof(repository));
    logger.LogInformation($"BoardGameController started");
}
````
19. Modifiy Index
````c#
public async Task<IActionResult> Index()
{
    logger.LogInformation($"{nameof(Index)}");
    return View(await repository.GetAllAsync());
}
````
20. Modifiy Details
```` c#
public async Task<IActionResult> Details(int id)
{

    logger.LogInformation($"{nameof(Details)} of {id}");
    var boardGameModel = await repository.GetAsync(id);

    if (boardGameModel == null)
    {
        logger.LogInformation($"{nameof(Details)} of {id} not found");
        return NotFound();
    }

    return View(boardGameModel);
}
````
21. Modifiy Create
```` c#
public async Task<IActionResult> Create()
{
    logger.LogInformation($"{nameof(Create)}");
    ViewData["PublishersId"] = new SelectList(await repository.GetPublishers(), "Id", "Name");
    return View();
}
````
22. Modifiy Create save
```` c#
public async Task<IActionResult> Create([Bind("Id,Name,Description,PublishersId,ImageURL")] BoardGameModel boardGameModel)
{
        logger.LogInformation($"{nameof(Create)} Save model");
        if (ModelState.IsValid)
    {
        await repository.AddAsync(boardGameModel);
        logger.LogInformation($"{nameof(Create)} new BoardGame {boardGameModel}");
        return RedirectToAction(nameof(Index));
    }
    else
    {
        logger.LogInformation($"{nameof(Create)} invalid model");
    }
    ViewData["PublishersId"] = new SelectList(await repository.GetPublishers(), "Id", "Name");
    return View(boardGameModel);
}
````
23. Modifiy Edit
```` C#
public async Task<IActionResult> Edit(int id)
{
    logger.LogInformation($"{nameof(Edit)}");
    var boardGameModel = await repository.GetAsync(id);
    if (boardGameModel == null)
    {
        logger.LogInformation($"{nameof(Edit)} BoardGameModel not found");
        return NotFound();
    }
    ViewData["PublishersId"] = new SelectList(await repository.GetPublishers(), "Id", "Name");
    return View(boardGameModel);
}
````
24. Modifiy Edit Save
```` c#
public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,PublishersId,ImageURL")] BoardGameModel boardGameModel)
{
    logger.LogInformation($"{nameof(Edit)} Save");
   if (ModelState.IsValid)
    {
        await repository.UpdateAsync(boardGameModel);
        return RedirectToAction(nameof(Index));
    }
    else
    {
        logger.LogInformation($"{nameof(Edit)} invalid model.");
    }
    ViewData["PublishersId"] = new SelectList(await repository.GetPublishers(), "Id", "Name");
    return View(boardGameModel);
}
````
25. Modifiy Delete
```` c#
public async Task<IActionResult> Delete(int id)
{
    logger.LogInformation($"{nameof(Delete)} {id}");
    var boardGameModel = await repository.GetAsync(id);
    if (boardGameModel == null)
    {
        logger.LogInformation($"{nameof(Delete)} BoardGameModel not found");
        return NotFound();
    }
    return View(boardGameModel);
}
````
26. Modifiy Delete Confirmed
```` c#
public async Task<IActionResult> DeleteConfirmed(int id)
{
    logger.LogInformation($"{nameof(DeleteConfirmed)} {id}");
    var boardGameModel = await repository.GetAsync(id);
    if (boardGameModel != null)
    {
        await repository.DeleteAsync(boardGameModel);
    }
    else
    {
        logger.LogInformation($"{nameof(Delete)} BoardGameModel not found, can not delete");
    }
    return RedirectToAction(nameof(Index));
}
````
## Run the applicaion check to make sure things are working.
## Step 8. Modifity Views
1. BoardGame Delete View
- Add link to Publisher Details to replace publisher Name
````HTML
  <dd class="col-sm-10">
        <a asp-route-id="@Model.PublishersId" asp-controller="Publisher" asp-action="Details">@Model.Publisher</a>
  </dd>
````
- Add img display to replace ImageURL display
````HTML
<dd class="col-sm-10">
   <img class="rounded-circle" src="@Model.ImageURL" alt="@Model.Name"
       style="max-width: 150px">
</dd>
````
2. BoardGame Details View
- Add link to Publisher Details to replace publisher Name
````HTML
  <dd class="col-sm-10">
        <a asp-route-id="@Model.PublishersId" asp-controller="Publisher" asp-action="Details">@Model.Publisher</a>
  </dd>
````
- Add img display to replace ImageURL display
````HTML
<dd class="col-sm-10">
   <img class="rounded-circle" src="@Model.ImageURL" alt="@Model.Name"
       style="max-width: 150px">
</dd>
````
3. BoardGame Index View
- Move the image column to column 0 (the 1st)
````HTML
 <tr>
    <th>
      @Html.DisplayNameFor(model => model.ImageURL)
    </th>
    <th>
      @Html.DisplayNameFor(model => model.Name)
    </th>
    <th>
      @Html.DisplayNameFor(model => model.Description)
    </th>
     <th>
        @Html.DisplayNameFor(model => model.Publishers)
     </th>
         
    <th></th>
</tr>
````
- Add img display to replace ImageURL display
````HTML
<td>
   <img class="rounded-circle" src="@item.ImageURL" alt="@item.Name"
           style="max-width: 150px">
</td>
````
- Add Actionlink to Publisher Details to replace publisher Name
````HTML
   <td>
       @Html.ActionLink(item.Publisher,
            "Details",
            "Publisher",
              new { id = item.PublishersId })
  </td>
````
4. Publisher Delete View
- Add Warning Message at the top
````HTML
<h2 class="text-danger">Deleting this Publisher will delete all Board Games</h2>
````
-Add display for the Board Games Associated with publisher at the bottom of the screen
````HTML
  @if (Model.BoardGames != null)
    {
           <dl class="row">
            @foreach (var b in Model.BoardGames)
            {
                    <dt class = "col-sm-2">
                        <a asp-route-id="@b.Id" asp-controller="BoardGame" asp-action="Details">@b.Name</a>
                    </dt>
                     <dd class = "col-sm-10">
                            <img class="rounded-circle" src="@b.ImageURL" alt="@b.Name"
                 style="max-width: 150px">     
                    </dd>    

            }
            </dl>
    }
````
5. Publisher Details View
- -Add display for the Board Games Associated with publisher at the bottom of the screen
````HTML
     @if (Model.BoardGames != null)
    {
        <dl class="row">
            @foreach (var b in Model.BoardGames)
            {
                <dt class="col-sm-2">
                    <a asp-route-id="@b.Id" asp-controller="BoardGame" asp-action="Details">@b.Name</a>
                </dt>
                <dd class="col-sm-10">
                    <img class="rounded-circle" src="@b.ImageURL" alt="@b.Name"
                 style="max-width: 150px">
                </dd>

            }
        </dl>
    }
````
6. Shared _Layout view
- Add menu links for Board Games and Publisher
````HTML
 <li class="nav-item">
    <a class="nav-link text-dark" asp-area="" asp-controller="BoardGame" asp-action="Index">Board Games</a>
 </li>
 <li class="nav-item">
    <a class="nav-link text-dark" asp-area="" asp-controller="Publisher" asp-action="Index">Publisher</a>
 </li>
````

## To set multiple startup projects
1. In Solution Explorer, select the solution (the top node).
2. Choose the solution node's context (right-click) menu and then choose Properties. ...
3. Expand the Common Properties node, and choose Startup Project.
4. Choose the Multiple Startup Projects option and set the appropriate actions.

### You must start the GameService before starting the GameLibrary