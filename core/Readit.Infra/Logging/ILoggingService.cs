namespace Readit.Infra.Logging
{
    public interface ILoggingService
    {
        public void LogError(Exception ex, string localException);

        public void LogFilesChapterUploaded(List<string> listaCaminhoArquivos);

        public void LogUsersLogged();
    }
}