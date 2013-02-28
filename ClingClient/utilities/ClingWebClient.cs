using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace ClingClient.utilities {
    class ClingWebClient : WebClient {
        private int _timeout;
        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public int timeout {
            get {
                return _timeout;
            }
            set {
                _timeout = value;
            }
        }

        public ClingWebClient() {
            this._timeout = 30000;
        }

        public ClingWebClient(int timeout) {
            this._timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address) {
            var result = base.GetWebRequest(address);
            result.Timeout = this._timeout;
            return result;
        }
    }
}
