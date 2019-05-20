using System;

namespace SongDownloaderV2
{
    public class DataStore
    {
        public DataStore(string api, string target, int quality)
        {
            ApiKey = api ?? throw new ArgumentNullException(nameof(api));
            TargetPath = target ?? throw new ArgumentNullException(nameof(target));
            Quality = quality;
        }

        public string ApiKey { get; private set; }

        public string TargetPath { get; private set; }

        public int Quality { get; private set; }
    }
}
