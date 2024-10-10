using SqlScriptRunner.Services.DatabaseQueryHandlers;

namespace SqlScriptRunner.Services.DatabaseQueryHandlerProvider;

public interface IDatabaseQueryHandlerProvider
{
    IDatabaseQueryHandler CreateExecutor();
}