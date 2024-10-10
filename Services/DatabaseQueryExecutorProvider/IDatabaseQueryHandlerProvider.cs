using SqlScriptRunner.Services.DatabaseQueryHandlers;

namespace SqlScriptRunner.Services.DatabaseQueryExecutorProvider;

public interface IDatabaseQueryHandlerProvider
{
    IDatabaseQueryHandler CreateExecutor();
}