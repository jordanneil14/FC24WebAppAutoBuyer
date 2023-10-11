namespace FIFAWebAppWithWorkers.Workers;

public interface IWorkerBase
{
    Task DoWork();
}
