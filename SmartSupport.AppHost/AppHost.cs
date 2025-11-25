using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

//// Define the FastAPI service using AddExecutable
builder.AddExecutable(
    name: "fastapi-backend",
    command: "../fastapi-backend/.venv/Scripts/uvicorn.exe",
    workingDirectory: "../fastapi-backend",
    args: new[] { "app.main:app", "--host", "0.0.0.0", "--port", "8000" }
)
.WithExternalHttpEndpoints()
.WithUrl("http://localhost:8000/");


builder.AddProject<Projects.frontend>("frontend");


builder.Build().Run();
