using System;

namespace MPC_HC.Domain
{
    public class MediaPlayerConfig
    {
        public string PathToMediaPlayerExecutable { get; }
        public Uri BaseUri { get; }

        public MediaPlayerConfig(string baseUri, string pathToMediaPlayerExecutable)
        {
            PathToMediaPlayerExecutable = pathToMediaPlayerExecutable;
            BaseUri = new Uri(baseUri);
        }
    }
}