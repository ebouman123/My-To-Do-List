using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// middleware that reroutes users to the todos route if they try and call /tasks
app.UseRewriter(new RewriteOptions().AddRedirect("tasks/(.*)", "todos/$1"));

var todos = new List<Todo>();

// return all todos
app.MapGet("/todos", () => todos);

// returns either 200 with a Todo in the body, or 404
app.MapGet("/todos/{id}", Results<Ok<Todo>, NotFound> (int id) => 
{
    var targetTodo = todos.SingleOrDefault(t => id == t.Id);
    return targetTodo is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(targetTodo);
});

// posts a task to the todos List
app.MapPost("/todos", (Todo task) =>
{
    todos.Add(task);
    return TypedResults.Created("/todos/{id}", task);
});

// delete a task
app.MapDelete("todos/{id}", (int id) => 
{
    todos.RemoveAll(t => id == t.Id);
    return TypedResults.NoContent();
});

app.Run();


public record Todo(int Id, string Name, DateTime DueDate, bool IsCompleted);