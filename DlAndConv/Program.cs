using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Models;

namespace DlAndConv
{
    class Program
    {
        private static bool _isFinished = false;

        static void Main(string[] args)
        {
            if (args.Length == 4)
            {
                Run(args[0], args[1], args[2], args[3]);
                while (!_isFinished)
                {
                    Thread.Sleep(1000);
                }
            }
            else
            {
                Console.WriteLine("Wystąpił błąd");
            }
        }

        static async Task Run(string api, string temp, string target, string quality)
        {
            api = api ?? throw new ArgumentNullException(nameof(api));
            temp = temp ?? throw new ArgumentNullException(nameof(temp));
            target = target ?? throw new ArgumentNullException(nameof(target));

            var songNames = ExtractSongNamesFromSite();

            if (!Directory.Exists(temp))
            {
                Directory.CreateDirectory(temp);
            }

            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }

            foreach (var item in songNames)
            {
                var id = GetYoutubeVideoID(item, api);

                var client = new YoutubeClient();
                var video = await client.GetVideoInfoAsync(id);

                var streaminfo = video.AudioStreams.OrderBy(x => x.Bitrate).Last();

                string filename = Path.Combine(temp, $"{video.Title}.{streaminfo.Container.GetFileExtension()}");
                string fname = Path.Combine(target, $"{video.Title}.mp3");

                Console.WriteLine($"Rozpoczęto pobieranie: {item}");
                await client.DownloadMediaStreamAsync(streaminfo, filename);
                Console.WriteLine($"Zakończono pobieranie: {item}");

                Console.WriteLine($"Rozpoczynanie konwersji audio dla: {item}");

                ProcessStartInfo info = new ProcessStartInfo("ffmpeg.exe");
                info.CreateNoWindow = false;
                info.RedirectStandardOutput = true;
                info.UseShellExecute = false;
                info.WindowStyle = ProcessWindowStyle.Hidden;
                info.Arguments = $"-i \"{filename}\" -vn -ab {quality}k -ar 44100 -y \"{fname}\"";
                var proc = Process.Start(info);

                //while (!proc.StandardOutput.EndOfStream)
                //{
                //    Console.WriteLine(proc.StandardOutput.ReadLine());
                //}

                proc.WaitForExit();

                Console.WriteLine($"Zakończono konwersję audio dla: {item}");
            }

            Console.WriteLine("Zakończono pobieranie i konwertowanie");
            _isFinished = true;
        }

        private static string GetYoutubeVideoID(string queryText, string api)
        {

            string apikey = api;

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apikey,
                ApplicationName = "MarcinEU Song Downloader"
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = queryText ?? throw new ArgumentNullException(nameof(queryText));
            searchListRequest.MaxResults = 6;

            var response = searchListRequest.Execute();

            List<(string title, string id)> videos = new List<(string title, string id)>();

            foreach (var searchResult in response.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    videos.Add((searchResult.Snippet.Title, searchResult.Id.VideoId));
                }
            }

            string foundVideoID = "";

            foreach (var video in videos)
            {
                if (video.title.Contains("Official"))
                {
                    foundVideoID = video.id;
                }
            }

            if (String.IsNullOrWhiteSpace(foundVideoID))
            {
                foundVideoID = videos.First().id;
            }

            Console.WriteLine($"Znaleziono audio dla: {videos.Where(x => x.id == foundVideoID).Select(x => x.title).First()}");

            //if (foundVideoID != null && foundVideoID != "")
            //{
            return foundVideoID;
            //}
            //else
            //{
            //    throw new Exception(String.Format("No download link for {0} has been found.", queryText));
            //}
        }

        private static List<string> ExtractSongNamesFromSite(string url = "http://www.rmfmaxxx.pl/hopbec")
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            List<string> songNames = new List<string>();

            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(url);

            var nodes = doc.DocumentNode.Descendants("div").Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "list-songs").ToList();

            foreach (var item in nodes)
            {
                foreach (var descendant in item.Descendants("a"))
                {
                    if (descendant.Attributes.Contains("class")
                        && descendant.Attributes["class"].Value == "is-title tov"
                        && descendant.Attributes.Contains("title"))
                    {
                        songNames.Add(descendant.Attributes["title"].Value);
                    }
                }
            }

            return songNames;

        }
    } 
}