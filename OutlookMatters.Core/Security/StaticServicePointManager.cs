using System.Net;
using System.Net.Security;

namespace OutlookMatters.Core.Security
{
    public class StaticServicePointManager: IServerCertificateValidator
    {
        private RemoteCertificateValidationCallback _backup;

        public void EnableValidation()
        {
            if (_backup != null)
            {
                ServicePointManager.ServerCertificateValidationCallback = _backup;
            }
        }

        public void DisableValidation()
        {
            if (_backup == null)
            {
                _backup = ServicePointManager.ServerCertificateValidationCallback;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            }
        }
    }
}
