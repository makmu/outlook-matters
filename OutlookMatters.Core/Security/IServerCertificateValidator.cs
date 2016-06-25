namespace OutlookMatters.Core.Security
{
    public interface IServerCertificateValidator
    {
        void EnableValidation();
        void DisableValidation();
    }
}
